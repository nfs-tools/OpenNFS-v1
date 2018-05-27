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
            Material = 0x00134B02
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

                BinaryUtil.ReadPadding(BinaryReader, ref chunkSize);

                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), 0,
                    typeof(SolidListChunks));

                switch (normalizedId)
                {
                    case (long) SolidListChunks.Header:
                    case (long) SolidListChunks.Object:
                    case (long) SolidListChunks.MeshHeader:
                    {
                        ReadChunks(chunkSize);
                        break;
                    }
                    case (long) SolidListChunks.FileInfo:
                    {
                        var info = BinaryUtil.ReadStruct<FileInfo>(BinaryReader);
#if DEBUG
                        Console.WriteLine($"{info.Path} [{info.Section}]");
#endif
                        break;
                    }
                    case (long) SolidListChunks.MeshFaces:
                    {
                        DebugUtil.EnsureCondition(chunkSize % 6 == 0, () => $"{chunkSize} is not divisible by 6");

                        while (BinaryReader.BaseStream.Position < chunkRunTo)
                        {
                            var v1 = BinaryReader.ReadUInt16() + 1;
                            var v2 = BinaryReader.ReadUInt16() + 1;
                            var v3 = BinaryReader.ReadUInt16() + 1;

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
                            while (BinaryReader.BaseStream.Position < chunkRunTo)
                            {
                                var x = BinaryReader.ReadSingle();
                                var y = BinaryReader.ReadSingle();
                                var z = BinaryReader.ReadSingle();

                                BinaryReader.BaseStream.Seek(24, SeekOrigin.Current);

#if DEBUG
                                Console.WriteLine($"v {x} {y} {z}");
#endif
                            }
                        }
                        else
                        {
#if DEBUG
                            Console.Error.WriteLine($"Unsupported size: {chunkSize}");
#endif
                        }

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