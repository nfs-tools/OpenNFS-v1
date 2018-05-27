using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.Core.Structures;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World.Frontend.Readers
{
    public class TPKReadContainer : ReadContainer<TexturePack>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TpkInfoHeader
        {
            private readonly uint Marker;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
            public readonly string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public readonly string Path;

            public readonly uint Hash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            private readonly byte[] empty;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TpkTextureHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
            private readonly byte[] zero;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public readonly string Name;

            public readonly int TextureHash;

            public readonly int TypeHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] blankOne;

            public readonly uint DataOffset;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] blankTwo;

            public readonly uint DataSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private readonly byte[] blankThree;

            public readonly short Width;

            public readonly short Height;

            private readonly short MipMapLow;

            public readonly short MipMap;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            private readonly byte[] restOfData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TPKCompressionHeader
        {
            public readonly uint TextureHash;
            public readonly uint AbsoluteOffset;
            public readonly uint Size;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public readonly uint[] unknown;
        }

        private enum TPKChunks : long
        {
            TPKRoot = 0xb3310000,
            TPKInfo = 0x33310001,
            TPKTextureHashes = 0x33310002,
            TPKCompressionHeaders = 0x33310003,
            TPKTextureHeaders = 0x33310004,
            TPKDXTHeaders = 0x33310005,
            TPKDataRoot = 0xb3320000,
            TPKData = 0x33320002
        }

        public TPKReadContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader,
            containerSize)
        {
        }

        public override TexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _texturePack = new TexturePack(ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS, ContainerSize,
                BinaryReader.BaseStream.Position);

            ReadChunks(ContainerSize);

            return _texturePack;
        }

        protected override void ReadChunks(long totalSize)
        {
            var runTo = BinaryReader.BaseStream.Position + totalSize;

            for (var i = 0; i < 0xFFFF && BinaryReader.BaseStream.Position < runTo; i++)
            {
                var chunkId = BinaryReader.ReadUInt32();
                var chunkSize = BinaryReader.ReadUInt32();
                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

                var normalizedId = (long) (chunkId & 0xffffffff);
                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), _logLevel,
                    typeof(TPKChunks));

                switch (normalizedId)
                {
                    case (long) TPKChunks.TPKRoot: // TPK root
                    case (long) TPKChunks.TPKDataRoot: // TPK data root
                        _logLevel = 2;
                        ReadChunks(chunkSize);
                        _logLevel = 1;
                        break;
                    case (long) TPKChunks.TPKInfo: // TPK info
                    {
                        var header = BinaryUtil.ReadStruct<TpkInfoHeader>(BinaryReader);

                        _texturePack.Name = header.Name;
                        _texturePack.Path = header.Path;
                        _texturePack.Hash = header.Hash;

                        break;
                    }
                    case (long) TPKChunks.TPKTextureHashes: // Texture hashes
                    {
                        // Every entry is 8 bytes; a 4-byte hash and 4 bytes of 0x00.
                        var numTextures = chunkSize / 8;

                        for (var j = 0; j < numTextures; j++)
                        {
                            var hash = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                            _texturePack.Hashes.Add(hash);
                        }

                        break;
                    }
                    case (long) TPKChunks.TPKCompressionHeaders:
                    {
                        _texturePack.IsCompressed = true;
                        _compressionHeaders.AddRange(
                            BinaryUtil.ReadList<TPKCompressionHeader>(BinaryReader, chunkSize));

#if DEBUG
                        foreach (var header in _compressionHeaders)
                        {
                            Console.WriteLine(
                                $"compression: texture=0x{header.TextureHash:X8},absOff={header.AbsoluteOffset},size={header.Size}");
                        }
#endif

                        break;
                    }
                    case (long) TPKChunks.TPKTextureHeaders: // Texture headers
                    {
                        foreach (var hash in _texturePack.Hashes)
                        {
                            // 12 0x00 at the beginning of each header
                            BinaryReader.BaseStream.Seek(12, SeekOrigin.Current);

                            var textureHash = BinaryReader.ReadUInt32();
                            var typeHash = BinaryReader.ReadUInt32();
                            var unknownHash = BinaryReader.ReadInt32();
                            var dataSize = BinaryReader.ReadUInt32();
                            var unknown1 = BinaryReader.ReadInt32();
                            var width = BinaryReader.ReadInt32();
                            var height = BinaryReader.ReadInt32();
                            var mipMap = BinaryReader.ReadInt32();
                            var unknown2 = BinaryReader.ReadUInt32();
                            var unknown3 = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(24, SeekOrigin.Current);
                            var maybeSize = BinaryReader.ReadUInt32();
                            var maybeOffset = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(60, SeekOrigin.Current);
                            var nameLength = (int) BinaryReader.ReadByte();
                            var name = new string(BinaryReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                            Console.WriteLine(
                                $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {maybeOffset}");
#endif
                            var texture = new Texture
                            {
                                TextureHash = textureHash,
                                TypeHash = typeHash,
                                Name = name,
                                Width = width,
                                Height = height,
                                MipMap = mipMap,
                                DataOffset = maybeOffset,
                                DataSize = dataSize
                            };

                            _texturePack.Textures.Add(texture);
                        }

//                        for (var j = 0; j < _texturePack.Hashes.Count; j++)
//                        {
//                            var textureHeader = BinaryUtil.ByteToType<TpkTextureHeader>(BinaryReader);
//
//                            var texture = new Texture
//                            {
//                                TextureHash = textureHeader.TextureHash,
//                                TypeHash = textureHeader.TypeHash,
//                                Name = textureHeader.Name,
//                                Width = textureHeader.Width,
//                                Height = textureHeader.Height,
//                                MipMap = textureHeader.MipMap,
//                                DataOffset = textureHeader.DataOffset,
//                                DataSize = textureHeader.DataSize
//                            };
//
//                            _texturePack.Textures.Add(texture);
//                        }

                        break;
                    }
                    case (long) TPKChunks.TPKDXTHeaders: // DXT headers
                    {
                        foreach (var texture in _texturePack.Textures)
                        {
                            BinaryReader.BaseStream.Seek(12, SeekOrigin.Current);
                            texture.CompressionType = BinaryReader.ReadInt32();
                            BinaryReader.BaseStream.Seek(0x10, SeekOrigin.Current);
#if DEBUG
                            Console.WriteLine($"DDS Type: 0x{texture.CompressionType:X8}");
#endif
                        }

                        break;
                    }
                    case (long) TPKChunks.TPKData: // data container
                    {
                        if (_texturePack.IsCompressed)
                        {
                            ReadCompressed(chunkSize);
                        }
                        else
                        {
                            ReadUncompressed();
                        }

                        break;
                    }
                    // ReSharper disable once RedundantEmptySwitchSection
                    default:
                        break;
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private void ReadCompressed(long chunkSize)
        {
            foreach (var compHeader in _compressionHeaders)
            {
#if DEBUG
                Console.WriteLine($"0x{compHeader.TextureHash:X8} @ 0x{compHeader.AbsoluteOffset:X8}");
#endif
                var skip = false;

                if (compHeader.AbsoluteOffset != 0)
                {
                    BinaryReader.BaseStream.Position =
                        compHeader.AbsoluteOffset; // relative to the beginning of the chunk or file
                    var readBytes = 0L;
                    var blocks = new List<byte[]>();
                    var partSizes = new uint[0xFFFF];
                    var textureDataSize = 0L;

                    while (readBytes < compHeader.Size)
                    {
                        if (BinaryReader.ReadInt32() == 0x55441122)
                        {
                            BinaryReader.BaseStream.Position -= 4;
                            var cbh = BinaryUtil.ReadStruct<CommonStructs.CompressBlockHead>(BinaryReader);
                            var flag = BinaryReader.ReadChars(4);
                            BinaryReader.BaseStream.Position -= 4;
                            var data = BinaryReader.ReadBytes((int) (cbh.TotalBlockSize - 24));

                            if (flag[0] == 'J' && flag[1] == 'D' && flag[2] == 'L' && flag[3] == 'Z')
                            {
                                blocks.Add(JDLZ.Decompress(data));
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("Unsupported compression type");
#endif
                                skip = true;
                                break;
                            }

                            readBytes += cbh.TotalBlockSize;
                            partSizes[blocks.Count - 1] = cbh.OutSize;
                            textureDataSize += cbh.OutSize;
                        }
                    }

                    if (skip) continue;

                    textureDataSize -= partSizes[blocks.Count == 1 ? 0 : blocks.Count - 2];

                    if (blocks.Count == 1)
                    {
                        var infoBlock = blocks[0];
                        var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                        // 12 0x00 at the beginning of each header
                        blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                        var textureHash = blockReader.ReadUInt32();
                        var typeHash = blockReader.ReadUInt32();
                        var unknownHash = blockReader.ReadInt32();
                        var dataSize = blockReader.ReadUInt32();
                        var unknown1 = blockReader.ReadInt32();
                        var width = blockReader.ReadInt32();
                        var height = blockReader.ReadInt32();
                        var mipMap = blockReader.ReadInt32();
                        var unknown2 = blockReader.ReadUInt32();
                        var unknown3 = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                        var maybeSize = blockReader.ReadUInt32();
                        var maybeOffset = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                        var nameLength = (int) blockReader.ReadByte();
                        var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                        Console.WriteLine(
                            $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {maybeOffset}");
#endif
                        blockReader.BaseStream.Position = 0;
                        var data = blockReader.ReadBytes((int) (blockReader.BaseStream.Length - 212));
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 20;

                        var texture = new Texture
                        {
                            TextureHash = textureHash,
                            TypeHash = typeHash,
                            Name = name,
                            Width = width,
                            Height = height,
                            MipMap = mipMap,
                            DataOffset = maybeOffset,
                            DataSize = (uint) (infoBlock.Length - 212),
                            CompressionType = blockReader.ReadInt32(),
                            Data = data
                        };

                        _texturePack.Textures.Add(texture);
                    }
                    else
                    {
                        textureDataSize -= partSizes[_compressionHeaders.Count - 2];

                        var infoBlock = blocks[blocks.Count - 2];
                        var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                        // 12 0x00 at the beginning of each header
                        blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                        var textureHash = blockReader.ReadUInt32();
                        var typeHash = blockReader.ReadUInt32();
                        var unknownHash = blockReader.ReadInt32();
                        var dataSize = blockReader.ReadUInt32();
                        var unknown1 = blockReader.ReadInt32();
                        var width = blockReader.ReadInt32();
                        var height = blockReader.ReadInt32();
                        var mipMap = blockReader.ReadInt32();
                        var unknown2 = blockReader.ReadUInt32();
                        var unknown3 = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                        var maybeSize = blockReader.ReadUInt32();
                        var maybeOffset = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                        var nameLength = (int) blockReader.ReadByte();
                        var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                        Console.WriteLine(
                            $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {maybeOffset}");
#endif
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 20;

                        var texture = new Texture
                        {
                            TextureHash = textureHash,
                            TypeHash = typeHash,
                            Name = name,
                            Width = width,
                            Height = height,
                            MipMap = mipMap,
                            DataOffset = maybeOffset,
                            DataSize = 0,
                            CompressionType = blockReader.ReadInt32()
                        };

#if DEBUG
                        Console.WriteLine($"0x{texture.CompressionType:X8}");
#endif
                        var data = new List<byte>();

                        foreach (var block in blocks)
                        {
                            if (blocks.IndexOf(block) == blocks.Count - 2) continue;

                            data.AddRange(block);
                            texture.DataSize = (uint) data.Count;
                        }

                        texture.Data = data.ToArray();
                        _texturePack.Textures.Add(texture);

//                            Console.WriteLine(BinaryUtil.HexDump(blockReader.ReadBytes((int) (blockReader.BaseStream.Length - blockReader.BaseStream.Position))));
                    }
                }
                else
                {
                }

//                for (var i = 0; i < blocks.Count; i++)
//                {
//                    var block = blocks[i];
//                    var decompressed = JDLZ.Decompress(block);
//
//                    if (i == blocks.Count - 2)
//                    {
//                        var blockReader = new BinaryReader(new MemoryStream(decompressed));
//                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;
//
//                        // 12 0x00 at the beginning of each header
//                        blockReader.BaseStream.Seek(12, SeekOrigin.Current);
//
//                        var textureHash = blockReader.ReadInt32();
//                        var typeHash = blockReader.ReadInt32();
//                        var unknownHash = blockReader.ReadInt32();
//                        var dataSize = blockReader.ReadUInt32();
//                        var unknown1 = blockReader.ReadInt32();
//                        var width = blockReader.ReadInt32();
//                        var height = blockReader.ReadInt32();
//                        var mipMap = blockReader.ReadInt32();
//                        var unknown2 = blockReader.ReadUInt32();
//                        var unknown3 = blockReader.ReadUInt32();
//                        blockReader.BaseStream.Seek(24, SeekOrigin.Current);
//                        var maybeSize = blockReader.ReadUInt32();
//                        var maybeOffset = blockReader.ReadUInt32();
//                        blockReader.BaseStream.Seek(60, SeekOrigin.Current);
//                        var nameLength = (int) blockReader.ReadByte();
//                        var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());
//
//                        Console.WriteLine(
//                            $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {maybeOffset}");
////                            Console.WriteLine(BinaryUtil.HexDump(blockReader.ReadBytes((int) (blockReader.BaseStream.Length - blockReader.BaseStream.Position))));
//                    }
//                }
            }

//            while (BinaryReader.BaseStream.Position < dataEnd)
//            {
//                if (BinaryReader.ReadInt32() == 0x55441122)
//                {
//                    BinaryReader.BaseStream.Position -= 4;
//                    var cbh = BinaryUtil.ReadStruct<CommonStructs.CompressBlockHead>(BinaryReader);
//
//                    var cf = BinaryReader.ReadChars(4);
//                    BinaryReader.BaseStream.Position -= 4;
//
//                    if (cf[0] == 'J' && cf[1] == 'D' && cf[2] == 'L' && cf[3] == 'Z')
//                    {
//                        var data = BinaryReader.ReadBytes((int) (cbh.TotalBlockSize - 24));
//                        var uncompressedData = JDLZ.Decompress(data);
//
////                        Console.WriteLine(BinaryUtil.HexDump(uncompressedData));
//                    }
//                }
//            }
        }

        private void ReadUncompressed()
        {
            BinaryReader.BaseStream.Seek(0x78, SeekOrigin.Current);
            var dataStart = BinaryReader.BaseStream.Position;

            foreach (var texture in _texturePack.Textures)
            {
                BinaryReader.BaseStream.Seek(dataStart + texture.DataOffset, SeekOrigin.Begin);
                texture.Data = new byte[texture.DataSize];

                BinaryReader.Read(texture.Data, 0, (int) texture.DataSize);
            }
        }

        private TexturePack _texturePack;
        private int _logLevel = 1;

        private readonly List<TPKCompressionHeader> _compressionHeaders = new List<TPKCompressionHeader>();
    }
}