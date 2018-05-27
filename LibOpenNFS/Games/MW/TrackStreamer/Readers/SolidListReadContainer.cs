using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.TrackStreamer.Readers
{
    public class SolidListReadContainer : ReadContainer<SolidList>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MaterialBurst
        {
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

        /**
         * Vertex36: Used in cars, similar to Vertex24 but with 3 extra floats
         */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Vertex36
        {
            public readonly float X;

            public readonly float Y;

            public readonly float Z;

            public readonly float nX;

            public readonly float nY;

            public readonly float nZ;

            public readonly int Color;

            public readonly float U;

            public readonly float V;
        }

        /**
         * Vertex24: basic vertex. xyz coords, color, UV, that's it
         */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Vertex24
        {
            public readonly float X;

            public readonly float Y;

            public readonly float Z;

            public readonly int Color;

            public readonly float U;

            public readonly float V;
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

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public readonly string Section;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            private readonly uint[] unknownData;

            private readonly uint unkVarA;
            private readonly uint unkVarB;
            private readonly uint unkVarC;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly uint[] unknownData2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Face
        {
            public readonly int vA;
            public readonly int vB;
            public readonly int vC;
        }

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
            Material = 0x00134B02
        }

        public SolidListReadContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader,
            containerSize)
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

                if (_paddedChunks.Contains(normalizedId))
                {
                    uint pad = 0;

                    while (BinaryReader.ReadByte() == 0x11)
                    {
                        pad++;
                    }

                    // This is a bad hack to get around the fact that sometimes padded chunk data actually starts with 0x11...
                    // Padding is always even so if we detect uneven padding, we just jump back 2 bytes instead of 1.
                    BinaryReader.BaseStream.Seek(pad % 2 == 0 ? -1 : -2, SeekOrigin.Current);
                    BinaryUtil.PrintPosition(BinaryReader, GetType());

                    chunkSize -= (pad % 2 == 0 ? pad : pad - 1);
                }

                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

//                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), _logLevel,
//                    typeof(SolidListChunks));

                switch (normalizedId)
                {
                    case (long) SolidListChunks.Header:
                    {
#if DEBUG
                        Console.WriteLine($"Point3D size = {Marshal.SizeOf(typeof(CommonStructs.Point3D))}");
                        Console.WriteLine($"Matrix size = {Marshal.SizeOf(typeof(CommonStructs.Matrix))}");
                        Console.WriteLine($"header size = {Marshal.SizeOf(typeof(ObjectHeader))}");
                        Console.WriteLine($"MatBurst size = {Marshal.SizeOf(typeof(MaterialBurst))}");
#endif
                        goto case (long) SolidListChunks.Object;
                    }
                    case (long) SolidListChunks.Object:
                    {
                        _logLevel = 2;
                        ReadChunks(chunkSize);
                        _logLevel = 1;
                        break;
                    }
                    case (long) SolidListChunks.MeshHeader:
                    {
                        _logLevel = 3;
                        ReadChunks(chunkSize);
                        _logLevel = 2;
                        break;
                    }
                    case (long) SolidListChunks.FileInfo:
                    {
                        var fileInfo = BinaryUtil.ReadStruct<FileInfo>(BinaryReader);

                        _solidList.Path = fileInfo.Path;
                        _solidList.SectionId = fileInfo.Section;

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
                        DebugUtil.EnsureCondition(
                            _solidList.Hashes.Count >= _solidList.Objects.Count,
                            () =>
                                $"Expected enough hashes for {_solidList.Objects.Count} object(s); we only have {_solidList.Hashes.Count}");

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

                        break;
                    }
                    case (long) SolidListChunks.TextureRefs:
                    {
                        // each entry is 8 bytes.
                        // first 4 bytes are the texture hash.
                        // last 4 are 0x00. as usual.

                        var numEntries = chunkSize / 8;

                        for (var j = 0; j < numEntries; j++)
                        {
                            var hash = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);

#if DEBUG
                            Console.WriteLine($"    Texture Hash #{j + 1:00}: 0x{hash:X8}");
#endif
                        }

                        break;
                    }
                    case (long) SolidListChunks.MeshVertices:
                    {
                        break;
                    }
                    case (long) SolidListChunks.Material:
                    {
#if DEBUG
                        var data = new byte[chunkSize];
                        BinaryReader.Read(data, 0, data.Length);

                        Console.WriteLine(BinaryUtil.HexDump(data));
#endif
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private SolidList _solidList;

        private readonly long[] _paddedChunks =
        {
            (long) SolidListChunks.ObjectHeader,
            (long) SolidListChunks.MeshVertices,
            (long) SolidListChunks.Material,
        };

        private int _logLevel = 1;
    }
}