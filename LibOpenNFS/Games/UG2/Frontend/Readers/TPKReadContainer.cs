using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.UG2.Frontend.Readers
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

            public readonly uint TextureHash;

            public readonly uint TypeHash;

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

        private enum TPKChunks : long
        {
            TPKRoot = 0xb3310000,
            TPKInfo = 0x33310001,
            TPKTextureHashes = 0x33310002,
            TPKTextureHeaders = 0x33310004,
            TPKDXTHeaders = 0x33310005,
            TPKDataRoot = 0xb3320000,
            TPKData = 0x33320002
        }

        public TPKReadContainer(BinaryReader binaryReader, long? containerSize, bool compressed) : base(binaryReader,
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

            _texturePack = new TexturePack(ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS, ContainerSize,
                BinaryReader.BaseStream.Position);

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

                var normalizedId = (long) (chunkId & 0xffffffff);

                BinaryUtil.ReadPadding(BinaryReader, ref chunkSize);
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
                    case (long) TPKChunks.TPKTextureHeaders: // Texture headers
                    {
                        for (var j = 0; j < _texturePack.Hashes.Count; j++)
                        {
                            var textureHeader = BinaryUtil.ReadStruct<TpkTextureHeader>(BinaryReader);
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
                    case (long) TPKChunks.TPKDXTHeaders: // DXT headers
                    {
                        foreach (var texture in _texturePack.Textures)
                        {
                            BinaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                            texture.CompressionType = BinaryReader.ReadInt32();
                            BinaryReader.BaseStream.Seek(0x08, SeekOrigin.Current);
                        }

                        break;
                    }
                    case (long) TPKChunks.TPKData:
                    {
                        if (_texturePack.Hashes.Any() && !_texturePack.Textures.Any())
                        {
                            // Probably compressed?
#if DEBUG
                            Console.WriteLine("Seems to be a compressed TPK");
#endif
                            var jdlzPositions = new List<long>();

                            while (BinaryReader.BaseStream.Position < chunkRunTo)
                            {
                                var tmpCompressionFlag = BinaryReader.ReadBytes(4);

                                if (tmpCompressionFlag[0] == 'J' && tmpCompressionFlag[1] == 'D' &&
                                    tmpCompressionFlag[2] == 'L' && tmpCompressionFlag[3] == 'Z')
                                {
#if DEBUG
                                    Console.WriteLine("JDLZ!");
#endif
                                    var headerPos = BinaryReader.BaseStream.Position - 4;

                                    jdlzPositions.Add(headerPos);
                                }
                            }

                            foreach (var jdlzPos in jdlzPositions)
                            {
                                BinaryReader.BaseStream.Seek(jdlzPos, SeekOrigin.Begin);

                                var header = BinaryUtil.ReadStruct<CommonStructs.JDLZHeader>(BinaryReader);

#if DEBUG
                                Console.WriteLine(
                                    $"JDLZ: {header.CompressedLength} bytes compressed, {header.UncompressedLength} uncompressed | header pos 0x{jdlzPos:X8}");
#endif
                                var compressedData = new byte[header.CompressedLength];
                                BinaryReader.BaseStream.Position = jdlzPos;

                                BinaryReader.Read(compressedData, 0, compressedData.Length);
                                var uncompressedData = JDLZ.Decompress(compressedData);

#if DEBUG
                                Console.WriteLine(BinaryUtil.HexDump(uncompressedData));
#endif
                            }
                        }
                        else if (_texturePack.Hashes.Count != _texturePack.Textures.Count)
                        {
                            // What?
                            throw new NFSException(
                                $"Expected {_texturePack.Hashes.Count} textures, only got {_texturePack.Textures.Count}");
                        }
                        else
                        {
                        }

                        break;
                    }
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private TexturePack _texturePack;
        private readonly bool _compressed;
        private int _logLevel = 1;
    }
}