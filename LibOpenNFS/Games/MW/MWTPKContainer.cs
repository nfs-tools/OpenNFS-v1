using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW
{
    public class MWTPKContainer : Container<TexturePack>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TpkInfoHeader
        {
            private readonly uint Marker;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
            public readonly string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public readonly string Path;

            public readonly int Hash;

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

        public MWTPKContainer(BinaryReader binaryReader, long? containerSize, bool compressed) : base(binaryReader,
            containerSize)
        {
            _compressed = compressed;
        }

        public override TexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _texturePack = new TexturePack(ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS, ContainerSize);

            ReadChunks(ContainerSize);

            return _texturePack;
        }

        protected override void ReadChunks(long totalSize)
        {
            var runTo = (_compressed
                            ? BinaryReader.BaseStream.Position - 8
                            : BinaryReader.BaseStream.Position) + totalSize;

            for (var i = 0; i < 0xFFFF && BinaryReader.BaseStream.Position < runTo; i++)
            {
                var chunkId = BinaryReader.ReadUInt32();
                var chunkSize = BinaryReader.ReadUInt32();
                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

//                Console.WriteLine("    chunk #{0:00}: 0x{1:X8} [{2} bytes]", i + 1, chunkId, chunkSize);

                switch (chunkId)
                {
                    case 0xb3310000: // TPK root
                    case 0xb3320000: // TPK data root
                        ReadChunks(chunkSize);
                        break;
                    case 0x33310001: // TPK info
                    {
                        var header = BinaryUtil.ByteToType<TpkInfoHeader>(BinaryReader);
//                        Console.WriteLine("TPK: {0} [{1}] (0x{2:X8})",
//                            header.Name, header.Path, header.Hash);

                        _texturePack.Name = header.Name;
                        _texturePack.Path = header.Path;
                        _texturePack.Hash = header.Hash;

                        break;
                    }
                    case 0x33310002: // Texture hashes
                    {
                        // Every entry is 8 bytes; a 4-byte hash and 4 bytes of 0x00.
                        var numTextures = chunkSize / 8;

                        for (var j = 0; j < numTextures; j++)
                        {
                            var hash = BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                            _texturePack.Hashes.Add(hash);

//                            Console.WriteLine("Texture Hash #{0}: 0x{1:X8}", (j + 1), hash);
                        }

                        break;
                    }
                    case 0x33310004: // Texture headers
                    {
                        for (var j = 0; j < _texturePack.Hashes.Count; j++)
                        {
                            var textureHeader = BinaryUtil.ByteToType<TpkTextureHeader>(BinaryReader);

//                            Console.WriteLine("Texture #{0}: {1} [{2} by {3}, data @ 0x{4:X8} ({5} bytes)]",
//                                j + 1, textureHeader.Name, textureHeader.Width,
//                                textureHeader.Height, textureHeader.DataOffset, textureHeader.DataSize);

                            var texture = new Texture
                            {
                                TextureHash = textureHeader.TextureHash,
                                TypeHash = textureHeader.TypeHash,
                                Name = textureHeader.Name,
                                Width = textureHeader.Width,
                                Height = textureHeader.Height,
                                MipMap = textureHeader.MipMap,
                                DataOffset = textureHeader.DataOffset,
                                DataSize = textureHeader.DataSize
                            };

                            _texturePack.Textures.Add(texture);
                        }

                        break;
                    }
                    case 0x33310005: // DXT headers
                    {
                        foreach (var texture in _texturePack.Textures)
                        {
                            BinaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                            texture.CompressionType = BinaryReader.ReadUInt32();
//                            Console.WriteLine("Texture {0} is type 0x{1:X8}", texture.Name, texture.CompressionType);
                            BinaryReader.BaseStream.Seek(0x08, SeekOrigin.Current);
                        }

                        break;
                    }
                    case 0x33320002: // data container
                    {
                        BinaryReader.BaseStream.Seek(0x78, SeekOrigin.Current);

                        var dataStart = BinaryReader.BaseStream.Position;

                        foreach (var texture in _texturePack.Textures)
                        {
//                            Console.WriteLine("Reading data for {0} - offset: 0x{1:X8}", texture.Name,
//                                texture.DataOffset);

                            BinaryReader.BaseStream.Seek(dataStart + texture.DataOffset, SeekOrigin.Begin);
                            texture.Data = new byte[texture.DataSize];

                            BinaryReader.Read(texture.Data, 0, (int) texture.DataSize);
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

        private TexturePack _texturePack;
        private readonly bool _compressed;
    }
}