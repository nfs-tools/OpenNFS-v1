using System;
using System.IO;
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
            Object = 0x80134010,
            ObjectHeader = 0x00134011,
            MeshHeader = 0x80134100,
            MeshVertices = 0x00134b01,
            MeshFaces = 0x00134b03,
            TextureRefs = 0x00134012,
            Material = 0x00134B02,
            MaterialName = 0x00134C02 // At least, I think so. Whatever.
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
//                            Console.WriteLine($"f {v1} {v2} {v3}");
#endif
                        }
//                        DebugUtil.EnsureCondition(chunkSize % 6 == 0, () => $"{chunkSize} is not divisible by 6");
//
//                        while (BinaryReader.BaseStream.Position < chunkRunTo)
//                        {
//                            var v1 = BinaryReader.ReadUInt16() + 1;
//                            var v2 = BinaryReader.ReadUInt16() + 1;
//                            var v3 = BinaryReader.ReadUInt16() + 1;
//
//#if DEBUG
//                            Console.WriteLine($"f {v1} {v2} {v3}");
//#endif
//                        }

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
                    default: break;
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private SolidList _solidList;
    }
}