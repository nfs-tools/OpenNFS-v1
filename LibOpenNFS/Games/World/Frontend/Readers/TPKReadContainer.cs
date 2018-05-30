using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World.Frontend.Readers
{
    public class TpkReadContainer : ReadContainer<TexturePack>
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
        private struct TpkCompressionHeader
        {
            public readonly uint TextureHash;
            public readonly uint AbsoluteOffset;
            public readonly uint Size;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            private readonly uint[] unknown;
        }

        private enum TpkChunks : long
        {
            TpkRoot = 0xb3310000,
            TpkInfo = 0x33310001,
            TpkTextureHashes = 0x33310002,
            TpkCompressionHeaders = 0x33310003,
            TpkTextureHeaders = 0x33310004,
            TpkDxtHeaders = 0x33310005,
            TpkDataRoot = 0xb3320000,
            TpkData = 0x33320002
        }

        public TpkReadContainer(BinaryReader binaryReader, string fileName, long? containerSize)
            : base(binaryReader, fileName, containerSize)
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
                    typeof(TpkChunks));

                switch (normalizedId)
                {
                    case (long) TpkChunks.TpkRoot: // TPK root
                    case (long) TpkChunks.TpkDataRoot: // TPK data root
                        _logLevel = 2;
                        ReadChunks(chunkSize);
                        _logLevel = 1;
                        break;
                    case (long) TpkChunks.TpkInfo: // TPK info
                    {
                        var header = BinaryUtil.ReadStruct<TpkInfoHeader>(BinaryReader);

                        _texturePack.Name = header.Name;
                        _texturePack.Path = header.Path;
                        _texturePack.Hash = header.Hash;

                        break;
                    }
                    case (long) TpkChunks.TpkTextureHashes: // Texture hashes
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
                    case (long) TpkChunks.TpkCompressionHeaders:
                    {
                        _texturePack.IsCompressed = true;
                        _compressionHeaders.AddRange(
                            BinaryUtil.ReadList<TpkCompressionHeader>(BinaryReader, chunkSize));

                        if (File.Exists("complib.dll"))
                        {
                            Console.WriteLine("Found CompLib!");
                            _compLibEnabled = true;
                        }
#if DEBUG
                        foreach (var header in _compressionHeaders)
                        {
                            Console.WriteLine(
                                $"compression: texture=0x{header.TextureHash:X8},absOff={header.AbsoluteOffset},size={header.Size}");
                        }
#endif

                        break;
                    }
                    case (long) TpkChunks.TpkTextureHeaders: // Texture headers
                    {
                        foreach (var _ in _texturePack.Hashes)
                        {
                            // 12 0x00 at the beginning of each header
                            BinaryReader.BaseStream.Seek(12, SeekOrigin.Current);

                            var textureHash = BinaryReader.ReadUInt32();
                            var typeHash = BinaryReader.ReadUInt32();
                            BinaryReader.ReadInt32();
                            var dataSize = BinaryReader.ReadUInt32();
                            BinaryReader.ReadInt32();
                            var width = BinaryReader.ReadInt32();
                            var height = BinaryReader.ReadInt32();
                            var mipMap = BinaryReader.ReadInt32();
                            BinaryReader.ReadUInt32();
                            BinaryReader.ReadUInt32();
                            BinaryReader.BaseStream.Seek(24, SeekOrigin.Current);
                            BinaryReader.ReadUInt32();
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
                    case (long) TpkChunks.TpkDxtHeaders: // DXT headers
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
                    case (long) TpkChunks.TpkData: // data container
                    {
                        if (_texturePack.IsCompressed)
                        {
                            ReadCompressed();
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

        private void ReadCompressed()
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
                            } else if (_compLibEnabled)
                            {
                                var decompressed = new byte[cbh.OutSize];
                                Compression.Decompress(data, decompressed);
                                blocks.Add(decompressed);
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("CompLib disabled, can't decompress");
#endif
                                skip = true;
                                break;
                            }

                            data = null;
                            readBytes += cbh.TotalBlockSize;
                            partSizes[blocks.Count - 1] = cbh.OutSize;
                        }
                    }

                    if (skip) continue;

                    if (blocks.Count == 1)
                    {
                        var infoBlock = blocks[0];
                        var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                        // 12 0x00 at the beginning of each header
                        blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                        var textureHash = blockReader.ReadUInt32();
                        var typeHash = blockReader.ReadUInt32();
                        blockReader.ReadInt32();
                        var dataSize = blockReader.ReadUInt32();
                        blockReader.ReadInt32();
                        var width = blockReader.ReadInt32();
                        var height = blockReader.ReadInt32();
                        var mipMap = blockReader.ReadInt32();
                        blockReader.ReadUInt32();
                        blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                        blockReader.ReadUInt32();
                        var dataOffset = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                        var nameLength = (int) blockReader.ReadByte();
                        var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                        Console.WriteLine(
                            $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {dataOffset}");
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
                            DataOffset = dataOffset,
                            DataSize = (uint) (infoBlock.Length - 212),
                            CompressionType = blockReader.ReadInt32(),
                            Data = data
                        };

                        _texturePack.Textures.Add(texture);

                        infoBlock = null;
                        data = null;
                        blockReader.Dispose();
                        blocks.Clear();
                    }
                    else
                    {
                        var infoBlock = blocks[blocks.Count - 2];
                        var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                        blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                        // 12 0x00 at the beginning of each header
                        blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                        var textureHash = blockReader.ReadUInt32();
                        var typeHash = blockReader.ReadUInt32();
                        blockReader.ReadInt32();
                        var dataSize = blockReader.ReadUInt32();
                        blockReader.ReadInt32();
                        var width = blockReader.ReadInt32();
                        var height = blockReader.ReadInt32();
                        var mipMap = blockReader.ReadInt32();
                        blockReader.ReadUInt32();
                        blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                        blockReader.ReadUInt32();
                        var dataOffset = blockReader.ReadUInt32();
                        blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                        var nameLength = (int) blockReader.ReadByte();
                        var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                        Console.WriteLine(
                            $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {dataOffset}");
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
                            DataOffset = dataOffset,
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

                        infoBlock = null;
                        blockReader.Dispose();
                        blocks.Clear();
                        data.Clear();
                    }

                    partSizes = null;
                }
                else
                {
                    // Files are located in a folder with the same name as the texture pack.
                    var baseDir = Path.Combine(Path.GetDirectoryName(FileName) ?? throw new InvalidOperationException(), _texturePack.Name);

                    using (var textureReader =
                        new BinaryReader(File.OpenRead(Path.Combine(baseDir, $"{compHeader.TextureHash:X8}.stp"))))
                    {
                        var readBytes = 0L;
                        var blocks = new List<byte[]>();
                        var partSizes = new uint[0xFFFF];

                        while (readBytes < compHeader.Size)
                        {
                            if (textureReader.ReadInt32() == 0x55441122)
                            {
                                textureReader.BaseStream.Position -= 4;
                                var cbh = BinaryUtil.ReadStruct<CommonStructs.CompressBlockHead>(textureReader);
                                var flag = textureReader.ReadChars(4);
                                textureReader.BaseStream.Position -= 4;
                                var data = textureReader.ReadBytes((int)(cbh.TotalBlockSize - 24));

                                if (flag[0] == 'J' && flag[1] == 'D' && flag[2] == 'L' && flag[3] == 'Z')
                                {
                                    blocks.Add(JDLZ.Decompress(data));
                                }
                                else if (_compLibEnabled)
                                {
                                    var decompressed = new byte[cbh.OutSize];
                                    Compression.Decompress(data, decompressed);
                                    blocks.Add(decompressed);
                                }
                                else
                                {
#if DEBUG
                                    Console.WriteLine("CompLib disabled, can't decompress");
#endif
                                    skip = true;
                                    break;
                                }

                                readBytes += cbh.TotalBlockSize;
                                partSizes[blocks.Count - 1] = cbh.OutSize;
                                data = null;
                            }
                        }

                        if (skip) continue;

                        if (blocks.Count == 1)
                        {
                            var infoBlock = blocks[0];
                            var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                            blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                            // 12 0x00 at the beginning of each header
                            blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                            var textureHash = blockReader.ReadUInt32();
                            var typeHash = blockReader.ReadUInt32();
                            blockReader.ReadInt32();
                            var dataSize = blockReader.ReadUInt32();
                            blockReader.ReadInt32();
                            var width = blockReader.ReadInt32();
                            var height = blockReader.ReadInt32();
                            var mipMap = blockReader.ReadInt32();
                            blockReader.ReadUInt32();
                            blockReader.ReadUInt32();
                            blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                            blockReader.ReadUInt32();
                            var dataOffset = blockReader.ReadUInt32();
                            blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                            var nameLength = (int)blockReader.ReadByte();
                            var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                            Console.WriteLine(
                                $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {dataOffset}");
#endif
                            blockReader.BaseStream.Position = 0;
                            var data = blockReader.ReadBytes((int)(blockReader.BaseStream.Length - 212));
                            blockReader.BaseStream.Position = blockReader.BaseStream.Length - 20;

                            var texture = new Texture
                            {
                                TextureHash = textureHash,
                                TypeHash = typeHash,
                                Name = name,
                                Width = width,
                                Height = height,
                                MipMap = mipMap,
                                DataOffset = dataOffset,
                                DataSize = (uint)(infoBlock.Length - 212),
                                CompressionType = blockReader.ReadInt32(),
                                Data = data
                            };

                            _texturePack.Textures.Add(texture);

                            infoBlock = null;
                            blockReader.Dispose();
                            blocks.Clear();
                            data = null;
                        }
                        else
                        {
                            var infoBlock = blocks[blocks.Count - 2];
                            var blockReader = new BinaryReader(new MemoryStream(infoBlock));
                            blockReader.BaseStream.Position = blockReader.BaseStream.Length - 212;

                            // 12 0x00 at the beginning of each header
                            blockReader.BaseStream.Seek(12, SeekOrigin.Current);

                            var textureHash = blockReader.ReadUInt32();
                            var typeHash = blockReader.ReadUInt32();
                            blockReader.ReadInt32();
                            var dataSize = blockReader.ReadUInt32();
                            blockReader.ReadInt32();
                            var width = blockReader.ReadInt32();
                            var height = blockReader.ReadInt32();
                            var mipMap = blockReader.ReadInt32();
                            blockReader.ReadUInt32();
                            blockReader.ReadUInt32();
                            blockReader.BaseStream.Seek(24, SeekOrigin.Current);
                            blockReader.ReadUInt32();
                            var dataOffset = blockReader.ReadUInt32();
                            blockReader.BaseStream.Seek(60, SeekOrigin.Current);
                            var nameLength = (int)blockReader.ReadByte();
                            var name = new string(blockReader.ReadChars(nameLength).Where(c => c != '\0').ToArray());

#if DEBUG
                            Console.WriteLine(
                                $"{name} (0x{textureHash:X8}/0x{typeHash:X8}) - {width}x{height}, {dataSize} bytes, @ {dataOffset}");
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
                                DataOffset = dataOffset,
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
                                texture.DataSize = (uint)data.Count;
                            }

                            texture.Data = data.ToArray();
                            _texturePack.Textures.Add(texture);

                            data.Clear();
                            blocks.Clear();
                            infoBlock = null;
                            blockReader.Dispose();
                        }

                        partSizes = null;
                    }
                }
            }

            GC.Collect();
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
        private bool _compLibEnabled;

        private readonly List<TpkCompressionHeader> _compressionHeaders = new List<TpkCompressionHeader>();
    }
}