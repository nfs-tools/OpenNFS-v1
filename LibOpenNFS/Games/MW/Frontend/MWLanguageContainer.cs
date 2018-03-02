using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW.Frontend
{
    public class MWLanguageContainer : Container<LanguagePack>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MWLanguageHeader
        {
            private readonly uint Marker;

            public readonly uint NumStrings;

            public readonly uint HashTableOffset;

            public readonly uint StringTableOffset;
        }
        
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
//        private struct UG2LanguageHeader
//        {
//            private readonly uint Marker;
//
//            public readonly uint NumStrings;
//
//            public readonly uint HashTableOffset;
//
//            public readonly uint StringTableOffset;
//        }

//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
//        private struct CarbonLanguageHeader
//        {
//            public readonly uint NumStrings;
//            public readonly uint HashTableOffset;
//            public readonly uint StringTableOffset;
//            
//            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
//            public readonly string Name;
//        }

        public MWLanguageContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override LanguagePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _languagePack = new LanguagePack(ChunkID.BCHUNK_LANGUAGE, ContainerSize, BinaryReader.BaseStream.Position);

            ReadChunks(ContainerSize);

            return _languagePack;
        }

        protected override void ReadChunks(long totalSize)
        {
            var chunkRunTo = BinaryReader.BaseStream.Position + totalSize;
            var curPos = BinaryReader.BaseStream.Position;
            var header = BinaryUtil.ByteToType<MWLanguageHeader>(BinaryReader);

//            Console.WriteLine(
//                $"Language: {header.NumStrings} strings; hash table at 0x{header.HashTableOffset:x8}; text table at 0x{header.StringTableOffset:x8}");

            // seek back to after size
            BinaryReader.BaseStream.Seek(curPos, SeekOrigin.Begin);
            BinaryReader.BaseStream.Seek(header.HashTableOffset, SeekOrigin.Current);

            /**
             * Read hash table
             */
            for (var i = 0; i < header.NumStrings; i++)
            {
                var entry = new LanguageEntry
                {
                    HashOne = BinaryReader.ReadUInt32(),
                    HashTwo = BinaryReader.ReadUInt32(),
                };

                _languagePack.Entries.Add(entry);
            }

            BinaryReader.BaseStream.Seek(curPos, SeekOrigin.Begin);
            BinaryReader.BaseStream.Seek(header.StringTableOffset, SeekOrigin.Current);

            for (var i = 0; i < header.NumStrings; i++)
            {
                _languagePack.Entries[i].Text = BinaryUtil.ReadNullTerminatedString(BinaryReader);
            }

            if (BinaryReader.BaseStream.Position > chunkRunTo)
            {
                throw new Exception(
                    $"Buffer overflow? Chunk runs to 0x{chunkRunTo:X16}, we're at 0x{BinaryReader.BaseStream.Position:X16} (diff: {(BinaryReader.BaseStream.Position - chunkRunTo):X16})");
            }

            BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
        }

        private LanguagePack _languagePack;
    }
}