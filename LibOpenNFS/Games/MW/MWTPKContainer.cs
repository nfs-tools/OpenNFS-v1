using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW
{
    public class MWTPKContainer : Container<TexturePack>
    {
        [StructLayout(LayoutKind.Explicit)]
        struct TPKInfoHeader
        {
            [FieldOffset(0)]
            public uint Marker;

            [FieldOffset(4)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
            public string Name;

            [FieldOffset(32)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string Path;

            [FieldOffset(96)]
            public int Hash;

            [FieldOffset(100)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] empty;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct TPKTextureHeader
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
            public byte[] zero;

            [FieldOffset(0xC)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public string Name;

            [FieldOffset(36)]
            public int TextureHash;

            [FieldOffset(40)]
            public int TypeHash;

            [FieldOffset(44)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] blankOne;

            [FieldOffset(48)]
            public uint DataOffset;

            [FieldOffset(52)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] blankTwo;

            [FieldOffset(56)]
            public uint DataSize;

            [FieldOffset(60)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] blankThree;

            [FieldOffset(68)]
            public short Width;

            [FieldOffset(70)]
            public short Height;

            [FieldOffset(72)]
            public short MipMapLow;

            [FieldOffset(74)]
            public short MipMap;

            [FieldOffset(76)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] restOfData;
        }

        public MWTPKContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override TexturePack Get()
        {
            if (containerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _texturePack = new TexturePack((long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS, containerSize);

            ReadChunks(containerSize);

            return _texturePack;
        }

        protected override uint ReadChunks(long TotalSize)
        {
            var runTo = binaryReader.BaseStream.Position + TotalSize;

            for (int i = 0; i < 0xFFFF && binaryReader.BaseStream.Position < runTo; i++)
            {
                uint chunkId = binaryReader.ReadUInt32();
                uint chunkSize = binaryReader.ReadUInt32();
                var chunkRunTo = binaryReader.BaseStream.Position + chunkSize;

                Console.WriteLine("    chunk: 0x{0} [{1} bytes]", chunkId.ToString("X8"), chunkSize);

                switch (chunkId)
                {
                    case 0xb3310000: // TPK root
                    case 0xb3320000: // TPK data root
                        ReadChunks(chunkSize);
                        break;
                    case 0x33310001: // TPK info
                        {
                            TPKInfoHeader header = BinaryUtil.ByteToType<TPKInfoHeader>(binaryReader);
                            Console.WriteLine("TPK: {0} [{1}] (0x{2})",
                                header.Name, header.Path, header.Hash.ToString("X8"));

                            _texturePack.Name = header.Name;
                            _texturePack.Path = header.Path;
                            _texturePack.Hash = header.Hash;

                            break;
                        }
                    case 0x33310002: // Texture hashes
                        {
                            // Every entry is 8 bytes; a 4-byte hash and 4 bytes of 0x00.
                            uint numTextures = chunkSize / 8;

                            for (int j = 0; j < numTextures; j++)
                            {
                                uint hash = binaryReader.ReadUInt32();
                                binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                                _texturePack.Hashes.Add(hash);

                                Console.WriteLine("Texture Hash #{0}: 0x{1}", (j + 1).ToString(), hash.ToString("X8"));
                            }

                            break;
                        }
                    case 0x33310004: // Texture headers
                        {
                            for (int j = 0; j < _texturePack.Hashes.Count; j++)
                            {
                                TPKTextureHeader textureHeader = BinaryUtil.ByteToType<TPKTextureHeader>(binaryReader);

                                Console.WriteLine("Texture #{0}: {1} [{2} by {3}, data @ 0x{4} ({5} bytes)]",
                                    j + 1, textureHeader.Name, textureHeader.Width.ToString(), textureHeader.Height.ToString(), 
                                    textureHeader.DataOffset.ToString("X8"), textureHeader.DataSize.ToString());

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
                                binaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                                texture.CompressionType = binaryReader.ReadUInt32();
                                Console.WriteLine("Texture {0} is type 0x{1}", texture.Name, texture.CompressionType.ToString("X8"));
                                binaryReader.BaseStream.Seek(0x08, SeekOrigin.Current);
                            }

                            break;
                        }
                    case 0x33320002: // data container
                        {
                            binaryReader.BaseStream.Seek(0x78, SeekOrigin.Current);

                            var dataStart = binaryReader.BaseStream.Position;

                            foreach (var texture in _texturePack.Textures)
                            {
                                Console.WriteLine("Reading data for {0} - offset: 0x{1}", texture.Name, texture.DataOffset.ToString("X8"));

                                binaryReader.BaseStream.Seek(dataStart + texture.DataOffset, SeekOrigin.Begin);
                                texture.Data = new byte[texture.DataSize];

                                binaryReader.Read(texture.Data, 0, (int) texture.DataSize);
                            }

                            break;
                        }
                    default:
                        break;
                }

                if (binaryReader.BaseStream.Position > chunkRunTo)
                {
                    throw new Exception(string.Format(
                        "Buffer overflow? Chunk runs to 0x{0}, we're at 0x{1} (diff: {2})",
                        chunkRunTo.ToString("X16"),
                        binaryReader.BaseStream.Position.ToString("X16"),
                        (binaryReader.BaseStream.Position - chunkRunTo).ToString("X16")));
                }

                binaryReader.BaseStream.Seek(chunkRunTo - binaryReader.BaseStream.Position, SeekOrigin.Current);
            }

            return 0;
        }

        private TexturePack _texturePack;
    }
}
