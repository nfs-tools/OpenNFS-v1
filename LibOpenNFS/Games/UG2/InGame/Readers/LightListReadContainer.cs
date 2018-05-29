using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.UG2.InGame.Readers
{
    public class LightListReadContainer : ReadContainer<LightList>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct HeaderStruct
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly uint[] unknowns;

            public readonly uint NumLightEntries;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LightStruct
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
            public readonly byte[] Data;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
            public readonly string Name;
        }
        
        private enum LightListChunks : long
        {
            Header = 0x135001,
            Unknown = 0x00135002,
            Entries = 0x00135003
        }

        public LightListReadContainer(BinaryReader binaryReader, string fileName, long? containerSize)
            : base(binaryReader, fileName, containerSize)
        {
        }

        public override LightList Get()
        {
            if (ContainerSize == 0)
            {
                throw new NFSException("containerSize is not set!");
            }

            _lightList = new LightList(ChunkID.BCHUNK_SPEED_ELIGHT_CHUNKS, ContainerSize,
                BinaryReader.BaseStream.Position);

            ReadChunks(ContainerSize);

            return _lightList;
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
//                    typeof(LightListChunks));

                switch (normalizedId)
                {
                    case (long) LightListChunks.Header:
                    {
                        var header = BinaryUtil.ReadStruct<HeaderStruct>(BinaryReader);

                        _lightList.NumLights = header.NumLightEntries;
                        
                        break;
                    }
                    case (long) LightListChunks.Entries:
                    {
                        DebugUtil.EnsureCondition(
                            _lightList.NumLights == BinaryUtil.ComputeEntryCount<LightStruct>(chunkSize),
                            () => $"Expected {_lightList.NumLights} light(s), ComputeEntryCount reported {BinaryUtil.ComputeEntryCount<LightStruct>(chunkSize)}");

                        var lights = BinaryUtil.ReadList<LightStruct>(BinaryReader, chunkSize);
                        
                        break;
                    }
                    default:
                        break;
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private LightList _lightList;

        private readonly List<long> _paddedChunks = new List<long>
        {
            (long) LightListChunks.Header,
            (long) LightListChunks.Unknown,
            (long) LightListChunks.Entries,
        };

        private int _logLevel = 1;
    }
}