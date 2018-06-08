using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World.TrackStreamer.Readers
{
    public class SolidListReadContainer : ReadContainer<SolidList>
    {
        private enum SolidListChunks : long
        {
            Header = 0x80134001,
            FileInfo = 0x00134002,
            HashTable = 0x00134003,
            Something = 0x80134008, // Something? What is this something? I don't know, and I don't really care.
            CompressionHeaders = 0x00134004, // 36-byte entries
            Object = 0x80134010,
            ObjectHeader = 0x00134011,
            MeshHeader = 0x80134100,
            MeshVertices = 0x00134b01,
            MeshFaces = 0x00134b03,
            TextureRefs = 0x00134012,
            Material = 0x00134B02,
            MaterialName = 0x00134C02 // At least, I think so. Whatever.
        }

        /// <summary>
        /// This is the same as the TPK compression structure.
        /// How fitting.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ObjectCompressionHeader
        {
            public readonly uint ObjectHash;
            public readonly uint AbsoluteOffset;
            public readonly uint Size;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            private readonly uint[] unknown;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ObjectHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            private readonly byte[] zero;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] pad;

            public readonly uint NumTris;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private readonly byte[] pad2;

            public readonly CommonStructs.Point3D MinPoint;
            public readonly CommonStructs.Point3D MaxPoint;
            public readonly CommonStructs.Matrix Matrix;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private readonly uint[] unknown;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FileInfo
        {
            private readonly uint blank1;
            private readonly uint blank2;
            private readonly uint tagA;
            private readonly uint tagB;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]
            public readonly string Path;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public readonly string Section;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly uint[] unknownData;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly uint[] unknownData2;
        }

        public SolidListReadContainer(BinaryReader binaryReader, string fileName, long? containerSize)
            : base(binaryReader, fileName, containerSize)
        {
        }

        public override SolidList Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _solidList = new SolidList(ChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS, ContainerSize,
                BinaryReader.BaseStream.Position);

            ReadChunks(ContainerSize);

            return _solidList;
        }

        protected override void ReadChunks(long totalSize)
        {
            var runTo = BinaryReader.BaseStream.Position + totalSize;
            var endChecks = true; // used for 22 11 44 55 compression
            var stop = false;

            for (var i = 0; i < 0xFFFF && BinaryReader.BaseStream.Position < runTo; i++)
            {
                var chunkId = BinaryReader.ReadUInt32();
                var chunkSize = BinaryReader.ReadUInt32();
                var normalizedId = (long) (chunkId & 0xffffffff);

                BinaryUtil.ReadPadding(BinaryReader, ref chunkSize);

                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), 0,
                    typeof(SolidListChunks));

                switch (normalizedId)
                {
                    case (long) SolidListChunks.Header:
                    case (long) SolidListChunks.Object:
                    {
                        ReadChunks(chunkSize);
                        break;
                    }
                    case (long) SolidListChunks.MeshHeader:
                    {
                        _solidList.LastObject.Mesh = new SolidMesh();
                        ReadChunks(chunkSize);
                        break;
                    }
                    case (long) SolidListChunks.FileInfo:
                    {
                        var info = BinaryUtil.ReadStruct<FileInfo>(BinaryReader);

                        _solidList.SectionId = info.Section;
                        _solidList.Path = info.Path;

#if DEBUG
                        Console.WriteLine($"{info.Path} [{info.Section}]");
#endif
                        break;
                    }
                    case (long) SolidListChunks.HashTable:
                    {
                        // each hash entry is 8 bytes: 4 bytes for the hash and 4 bytes of 0x00

                        var numEntries = chunkSize / 8;

                        for (var j = 0; j < numEntries; j++)
                        {
                            _solidList.Hashes.Add(BinaryReader.ReadUInt32());
                            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                        }

                        break;
                    }
                    case (long) SolidListChunks.Something: // Something is in the car GEOMETRY.bin files. It's always empty.
                    {
                        if (chunkSize == 0 && _solidList.SectionId == "DEFAULT")
                        {
                            // This should always be triggered for car files.
                            _isCompressed = true;
                            _solidList.Compressed = true;
                        }
                        break;
                    }
                    case (long) SolidListChunks.CompressionHeaders:
                    {
                        _compressionHeaders.AddRange(BinaryUtil.ReadList<ObjectCompressionHeader>(BinaryReader, chunkSize));
                        break;
                    }
                    case (long) SolidListChunks.ObjectHeader:
                    {
                        var objectHeader = BinaryUtil.ReadStruct<ObjectHeader>(BinaryReader);
                        var objectName = BinaryUtil.ReadNullTerminatedString(BinaryReader);
                        var objectHash = _solidList.Hashes[_solidList.Objects.Count];

                        _solidList.Objects.Add(new SolidObject
                        {
                            Hash = objectHash,
                            Matrix = objectHeader.Matrix,
                            MaxPoint = objectHeader.MaxPoint,
                            MinPoint = objectHeader.MinPoint,
                            Name = objectName
                        });

#if DEBUG
                        Console.WriteLine($"Object: {objectName}");
#endif

                        break;
                    }
                    case (long) SolidListChunks.MeshFaces:
                    {
                        for (var j = 0; j < chunkSize / 6; j++)
                        {
                            var v1 = (short) (BinaryReader.ReadInt16() + 1);
                            var v2 = (short) (BinaryReader.ReadInt16() + 1);
                            var v3 = (short) (BinaryReader.ReadInt16() + 1);

                            _solidList.LastObject.Mesh.Faces.Add(new Face
                            {
                                VertexA = v1,
                                VertexB = v2,
                                VertexC = v3
                            });

#if DEBUG
                                Console.WriteLine($"f {v1} {v2} {v3}");
#endif
                            }

                        break;
                    }
                    case (long) SolidListChunks.MeshVertices:
                    {
                        if (chunkSize % 36 == 0)
                        {
                            _solidList.LastObject.IsSupported = true;

                            while (BinaryReader.BaseStream.Position < chunkRunTo)
                            {
                                var x = BinaryReader.ReadSingle();
                                var y = BinaryReader.ReadSingle();
                                var z = BinaryReader.ReadSingle();

                                BinaryReader.BaseStream.Seek(24, SeekOrigin.Current);

                                _solidList.LastObject.Mesh.Vertices.Add(new Vertex
                                {
                                    Color = 0,
                                    U = 0.0f,
                                    V = 0.0f,
                                    X = x,
                                    Y = y,
                                    Z = z
                                });

#if DEBUG
//                                Console.WriteLine($"v {x} {y} {z}");
#endif
                            }
                        }
                        else if (chunkSize % 44 == 0)
                        {
                            _solidList.LastObject.IsSupported = true;

                            while (BinaryReader.BaseStream.Position < chunkRunTo)
                            {
                                var x = BinaryReader.ReadSingle();
                                var y = BinaryReader.ReadSingle();
                                var z = BinaryReader.ReadSingle();

                                BinaryReader.BaseStream.Seek(32, SeekOrigin.Current);

                                _solidList.LastObject.Mesh.Vertices.Add(new Vertex
                                {
                                    Color = 0,
                                    U = 0.0f,
                                    V = 0.0f,
                                    X = x,
                                    Y = y,
                                    Z = z
                                });

#if DEBUG
//                                Console.WriteLine($"v {x} {y} {z}");
#endif
                            }
                        }

                        else
                        {
                            _solidList.LastObject.IsSupported = false;
#if DEBUG
                            Console.Error.WriteLine($"Unsupported size: {chunkSize}");
#endif
                        }

                        break;
                    }
                    case (long) SolidListChunks.TextureRefs:
                    {
                        // Every entry is 8 bytes; a 4-byte hash and 4 bytes of 0x00.
                        var numTextures = chunkSize / 8;

                        for (var j = 0; j < numTextures; j++)
                        {
                            var hash = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                            _solidList.LastObject.Textures.Add(hash);
                        }

                        break;
                    }
                    case (long) SolidListChunks.MaterialName:
                    {
//                        var name = BinaryUtil.ReadNullTerminatedString(BinaryReader);
//                        
//                        _solidList.LastObject.Materials.Add(new Material
//                        {
//                            Name = name
//                        });


                        break;
                    }
                    case (long) SolidListChunks.Material:
                    {
                        // Each entry is 116 bytes long.
                        // Thanks, BlackBox.

                        for (var j = 0; j < chunkSize / 116; j++)
                        {
                            var marker = BinaryReader.ReadUInt32();
                            var textureHash = BinaryReader.ReadUInt32();

                            BinaryReader.ReadInt64(); // 0x00000000 x2
                            
                            // After this: X, Y, Z? Hash? Not sure

                            _solidList.LastObject.Materials.Add(new Material
                            {
                                Name = $"Unnamed Material 0x{textureHash:X8}",
                                TextureHash = textureHash
                            });

                            BinaryReader.BaseStream.Seek(100, SeekOrigin.Current);
                        }

//                        while (BinaryReader.BaseStream.Position < chunkRunTo)
//                        {
//                            // I've only seen 00 40 00 00.
//                            var marker = BinaryReader.ReadUInt32();
//                            var textureHash = BinaryReader.ReadUInt32();
//                            
//                            _solidList.LastObject.Materials.Add(new Material
//                            {
//                                Name = $"Unnamed Material 0x{textureHash:X8}",
//                                TextureHash = textureHash
//                            });
//
//                            BinaryReader.BaseStream.Seek(112, SeekOrigin.Current);
//
////                            var materialData = BinaryReader.ReadBytes(116);
//                        }

                        break;
                    }
                    case 0x55441122: // Good old compression
                    {
                        endChecks = false;
                        BinaryReader.BaseStream.Position -= 8;

                        foreach (var compHeader in _compressionHeaders)
                        {
                            BinaryReader.BaseStream.Position = compHeader.AbsoluteOffset;

                            Console.WriteLine($"0x{compHeader.ObjectHash:X8} @ 0x{compHeader.AbsoluteOffset}");

                            var bytesRead = 0L;
                            var blocks = new List<byte[]>();
                            var decompressedBytes = 0L;
                            
                            while (bytesRead < compHeader.Size)
                            {
                                var cbh = BinaryUtil.ReadStruct<CommonStructs.CompressBlockHead>(BinaryReader);
                                bytesRead += cbh.TotalBlockSize;
                                //Console.WriteLine($"bytesRead: {bytesRead}/{compHeader.Size}");

                                var data = BinaryReader.ReadBytes((int) (cbh.TotalBlockSize - 24));
                                var outBuffer = new byte[cbh.OutSize];
                                
                                Compression.Decompress(data, outBuffer);

                                blocks.Add(outBuffer);

                                decompressedBytes += cbh.OutSize;
                                //Console.WriteLine($"decompressedBytes: {decompressedBytes}");
                            }

                            // There are 2 types of compression:
                            // The single-block type, aka normal chunk sequence,
                            // and the multi-block type, where the last block is the info block.
                            // This is like TPK compression. But slightly more annoying.

                            if (blocks.Count == 1)
                            {
                                _uncompressedBlock = new BinaryReader(new MemoryStream(blocks[0]));
                                ReadUncompressedBlock((uint) blocks[0].Length);
                            }
                            else if (blocks.Count > 1)
                            {
                                // Sort the blocks into their proper order.
                                var sorted = new List<byte>();
                                
                                sorted.AddRange(blocks[blocks.Count - 1]);

                                for (var j = 0; j < blocks.Count; j++)
                                {
                                    if (j != blocks.Count - 1)
                                    {
                                        sorted.AddRange(blocks[j]);
                                    }
                                }

                                _uncompressedBlock = new BinaryReader(new MemoryStream(sorted.ToArray()));
                                ReadUncompressedBlock((uint) sorted.Count);

                                sorted.Clear();
                            }
                            else
                            {
                                // Do nothing.
                            }


                            //var allData = blocks.SelectMany(b => b).ToList();

                            //using (var writeStream = File.OpenWrite($"object_0x{compHeader.ObjectHash:X8}.bin"))
                            //{
                            //    writeStream.Write(allData.ToArray(), 0, allData.Count);
                            //}
                        }

                        stop = true; // Stop the world, I want to get off

                        break;
                    }
                    default: break;
                }

                if (stop)
                {
                    break;
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        /// <summary>
        /// Read data from an uncompressed HUFF or JDLZ block.
        /// </summary>
        /// <param name="totalSize"></param>
        private void ReadUncompressedBlock(uint totalSize)
        {
            var runTo = _uncompressedBlock.BaseStream.Position + totalSize;

            for (var i = 0; i < 0xFFFF && _uncompressedBlock.BaseStream.Position < runTo; i++)
            {
                var chunkId = _uncompressedBlock.ReadUInt32();
                var chunkSize = _uncompressedBlock.ReadUInt32();
                var normalizedId = (long)(chunkId & 0xffffffff);
                var chunkRunTo = _uncompressedBlock.BaseStream.Position + chunkSize;

                BinaryUtil.ReadPadding(_uncompressedBlock, ref chunkSize);
                BinaryUtil.PrintID(_uncompressedBlock, chunkId, normalizedId, chunkSize, GetType(), 0,
                    typeof(SolidListChunks));

                switch (normalizedId)
                {
                    case (long) SolidListChunks.Header:
                    case (long) SolidListChunks.Object:
                    case (long) SolidListChunks.MeshHeader:
                    {
                        ReadUncompressedBlock(chunkSize);
                        break;
                    }
                    case (long) SolidListChunks.MeshVertices:
                    {
                        break;
                    }
                    default: break;
                }

                _uncompressedBlock.BaseStream.Seek(chunkRunTo - _uncompressedBlock.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private SolidList _solidList;
        private bool _isCompressed; // <----- This thing right here is the bane of my existence.
        private BinaryReader _uncompressedBlock;
        private readonly List<ObjectCompressionHeader> _compressionHeaders = new List<ObjectCompressionHeader>();
    }
}