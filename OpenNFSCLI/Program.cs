using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibOpenNFS.Games.MW;
using LibOpenNFS.DataModels;
using System.IO;

namespace OpenNFSCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            MWFileContainer container = new MWFileContainer(
                new System.IO.BinaryReader(
                    new FileStream("C:\\Users\\leodo\\Desktop\\Need for Speed Most Wanted 2005\\GLOBAL\\GLOBALB.BUN", FileMode.Open)
                ),
                null,
                null
            );

            var results = container.Get();
            //Console.Clear();
            Console.WriteLine("Results:");
            Console.WriteLine("    Texture Packs: {0}", results.Count(m => m is TexturePack).ToString());

            foreach (var model in results)
            {
                if (model is TexturePack)
                {
                    TexturePack pack = model as TexturePack;
                    Console.WriteLine("    Texture Pack: {0} [{1} / 0x{2}] | {3} entries",
                        pack.Name, pack.Path, pack.Hash.ToString("X8"), pack.Hashes.Count.ToString());

                    foreach (var textureItem in pack.Textures)
                    {
                        Console.WriteLine("        Texture {0} (0x{1}): [{2} by {3}, data @ 0x{4} ({5} bytes)]",
                                    textureItem.Name, textureItem.TextureHash.ToString("X8"), 
                                    textureItem.Width.ToString(), textureItem.Height.ToString(),
                                    textureItem.DataOffset.ToString("X8"), textureItem.DataSize.ToString());
                    }
                } else
                {
                    Console.WriteLine("Type: {0}", model.GetType().ToString());
                }
            }

            Console.ReadKey();
        }
    }
}
