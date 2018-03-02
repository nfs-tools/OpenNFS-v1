using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW.TrackStreamer
{
    public class MWSolidListContainer : Container<SolidList>
    {
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
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public readonly string Section;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
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
            TextureRefs = 0x00134012
        }

        public MWSolidListContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
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

                if (Enum.IsDefined(typeof(SolidListChunks), normalizedId))
                {
                    if (_paddedChunks.Contains((SolidListChunks) normalizedId))
                    {
                        // padding byte = 0x11
                        uint pad = 0;

                        while (BinaryReader.ReadByte() == 0x11)
                        {
                            pad++;
                        }

                        BinaryReader.BaseStream.Seek(-1, SeekOrigin.Current);

                        Console.WriteLine($"Applied padding: {pad} byte(s)");
                        chunkSize -= pad;
                    }
                }

                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), 1,
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
                        var fileInfo = BinaryUtil.ByteToType<FileInfo>(BinaryReader);
                        
                        Console.WriteLine($"Catalog: {fileInfo.Path} [{fileInfo.Section}]");
                        
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

                            Console.WriteLine($"    Hash #{j + 1:00}: 0x{_solidList.Hashes[j]:X8}");
                        }

                        break;
                    }
                    case (long) SolidListChunks.ObjectHeader:
                    {
                        goto default;
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

                            Console.WriteLine($"    Texture Hash #{j + 1:00}: 0x{hash:X8}");
                        }
                        
                        break;
                    }
                    case (long) SolidListChunks.MeshVertices:
                    {
                        break;
                    }
                    default:
                    {
                        if (chunkSize > 0)
                        {
                            var data = new byte[chunkSize];
                            BinaryReader.Read(data, 0, data.Length);

                            Console.WriteLine(BinaryUtil.HexDump(data));
                        }

                        break;
                    }
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private SolidList _solidList;

        private readonly SolidListChunks[] _paddedChunks =
        {
            SolidListChunks.ObjectHeader,
            SolidListChunks.MeshVertices,
        };
    }
}