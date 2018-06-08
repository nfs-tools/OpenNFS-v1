using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibOpenNFS.Games.MW;
using LibOpenNFS.DataModels;
using System.IO;
using System.Text;
using LibOpenNFS.Core;
using LibOpenNFS.Core.Crypto;
using LibOpenNFS.Core.Structures;
using LibOpenNFS.Games.UG2;
using LibOpenNFS.Games.Undercover;
using LibOpenNFS.Games.World;
using LibOpenNFS.Utils;

namespace OpenNFSCLI
{
    public static class Program
    {
        private static Dictionary<Type, string> _streamKeyDict = new Dictionary<Type, string>
        {
            {typeof(MWFileReadContainer), "L2R"},
            {typeof(UG2FileReadContainer), "L4R"},
            {typeof(WorldFileReadContainer), "L5R"},
            {typeof(UCFileReadContainer), "L8R"},
        };

        public static void Main(string[] args)
        {
            /*if (args.Length < 2)
            {
                Console.WriteLine("Please provide a file path and game");
                return;
            }*/

            var path = args[0];
            var game = args[1];

            // leo no touchy please
            //var path = @"D:\Games\Electronic Arts\Need for Speed Most Wanted\TRACKS\STREAML2RA.BUN";
            //var game = "mw";

            if (game != "mw" && game != "ug2" && game != "uc" && game != "world" && game != "carbon" && game != "ps")
            {
                Console.Error.WriteLine("Invalid game");
                return;
            }

            ReadContainer<List<BaseModel>> container = null;

            var readStream = new FileStream(path, FileMode.Open);
            switch (game)
            {
                case "mw":
                    container = new MWFileReadContainer(
                        new BinaryReader(
                            readStream
                        ),
                        path,
                        null
                    );
                    break;
                case "ug2":
                    container = new UG2FileReadContainer(
                        new BinaryReader(
                            readStream
                        ),
                        path,
                        null
                    );
                    break;
                case "uc":
                    container = new UCFileReadContainer(
                        new BinaryReader(
                            readStream
                        ),
                        path,
                        null
                    );
                    break;
                case "world":
                    container = new WorldFileReadContainer(
                        new BinaryReader(
                            readStream
                        ),
                        path,
                        null
                    );
                    break;
                default:
                    break;
            }

            if (container == null)
            {
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var results = container.Get();
                stopwatch.Stop();

                Console.WriteLine("Results:");
                Console.WriteLine("    Chunks read in {0} ms", stopwatch.ElapsedMilliseconds);

                ProcessResults(results, path, container.GetType());

                if (game == "world")
                {
                    var writeContainer = new WorldFileWriteContainer();
                    {
                        using (var writeStream = File.OpenWrite($"{path}_mod"))
                        {
                            foreach (var model in results)
                            {
                                if (model is TexturePack tpk)
                                {
                                    foreach (var t in tpk.Textures)
                                    {
                                        var ddsFile = $"{t.Name}.dds";

                                        if (!File.Exists(ddsFile)) continue;

                                        Console.WriteLine($"Found DDS: {ddsFile}");

                                        using (var ddsStream = new BinaryReader(File.OpenRead(ddsFile)))
                                        {
                                            var ddsHeader = BinaryUtil.ReadStruct<DDSHeader>(ddsStream);

                                            t.DataSize = (uint) (ddsStream.BaseStream.Length - 0x80);
                                            t.Data = ddsStream.ReadBytes((int) t.DataSize);

                                            Console.WriteLine($"DDS Data Size: {t.DataSize} - Mipmap: {ddsHeader.MipMapCount}");

                                            t.MipMap = ddsHeader.MipMapCount;
                                            t.Width = ddsHeader.Width;
                                            t.Height = ddsHeader.Height;

                                            t.CompressionType = ddsHeader.PixelFormat.Flags >= 0x40 
                                                ? 0x15 
                                                : ddsHeader.PixelFormat.FourCC;

                                            //t.CompressionType = ddsHeader.PixelFormat.FourCC;
                                        }
                                    }
                                }
                            }

                            writeContainer.Write(new BinaryReader(readStream), new BinaryWriter(writeStream), results);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Uh oh! Error:");
                Console.Error.WriteLine($"{e}");
            }
        }

        private static void ProcessResults(IEnumerable<BaseModel> results, string path,
            Type containerType)
        {
            foreach (var model in results.GroupBy(t => t.GetType()))
            {
                if (model.Key == typeof(TexturePack))
                {
                    Console.WriteLine("    Texture Packs: {0}", model.Count());
                    foreach (var pack in model.Cast<TexturePack>())
                    {
                        Console.WriteLine("    Texture Pack: {0} [{1} / 0x{2:X8}] | {3} texture(s)",
                            pack.Name, pack.Path, pack.Hash, pack.Hashes.Count);

                        foreach (var textureItem in pack.Textures.Select((value, i) => new {i = i + 1, value}))
                        {
                            Console.WriteLine(
                                "        Texture #{0} - {1} (0x{2:X8}): [{3} by {4}, data @ 0x{5:X8} ({6} bytes)]",
                                textureItem.i,
                                textureItem.value.Name, textureItem.value.TextureHash,
                                textureItem.value.Width, textureItem.value.Height, textureItem.value.DataOffset,
                                textureItem.value.DataSize);
                        }
                    }
                }
                else if (model.Key == typeof(SolidList))
                {
                    Console.WriteLine("    Solid Lists: {0}", model.Count());
                    foreach (var list in model.Cast<SolidList>())
                    {
                        foreach (var solidObject in list.Objects)
                        {
                            if (solidObject.IsSupported)
                            {
                                Console.WriteLine($"{solidObject.Name} is supported");
                                using (var outStream = File.OpenWrite($"{solidObject.Name}.obj"))
                                {
                                    using (var writer = new StreamWriter(outStream))
                                    {
                                        writer.WriteLine($"g {solidObject.Name}");

                                        foreach (var texture in solidObject.Textures)
                                        {
                                            writer.WriteLine($"# uses texture 0x{texture:X8}");
                                        }

                                        foreach (var vertex in solidObject.Mesh.Vertices)
                                        {
                                            writer.WriteLine($"v {vertex.X.ToString(FormatStrings.DoubleFixedPoint)} {vertex.Y.ToString(FormatStrings.DoubleFixedPoint)} {vertex.Z.ToString(FormatStrings.DoubleFixedPoint)}");
                                        }

                                        foreach (var face in solidObject.Mesh.Faces)
                                        {
                                            writer.WriteLine($"f {face.VertexA} {face.VertexB} {face.VertexC}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"{solidObject.Name} is not supported");
                            }
                        }
                    }
                }
                else if (model.Key == typeof(SectionList))
                {
                    foreach (var list in model.Cast<SectionList>())
                    {
                        Console.WriteLine($"    Section List: {list.Sections.Count} section(s)");

                        if (containerType == typeof(WorldFileReadContainer))
                        {
                            ProcessNFSWSections(list, path);
                        }
                    }
                }
            }
        }

        private static void ProcessNFSWSections(SectionList sectionList, string path)
        {
            foreach (var section in sectionList.Sections)
            {
                Console.WriteLine($"Reading section {section.ID} ({section.StreamChunkNumber})");

                var fileName = path.Replace("L5RA.BUN", $"STREAML5RA_{section.StreamChunkNumber}.BUN");

                if (!File.Exists(fileName))
                {
                    Console.WriteLine("    Can't find –– skipping");
                    continue;
                }
                
                var container = new WorldFileReadContainer(new BinaryReader(File.OpenRead(fileName)), fileName, null);
                
                ProcessResults(container.Get(), path, container.GetType());
            }
        }
    }
}