using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Frontend.Readers
{
    public class AnimatedTPKContainer : Container<AnimatedTexturePack>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct AnimatedTextureStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public readonly string Name;

            public readonly int Hash;
            public readonly int NumFrames;
            public readonly int FPS;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            private readonly byte[] _blank;
        }

        private enum AnimatedTPKChunks : long
        {
            AnimatedTPKHeader = 0x30300101,
            AnimatedTPKEntries = 0x30300102,
            AnimatedTPKHashes = 0x30300103,
        }

        public AnimatedTPKContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader,
            containerSize)
        {
        }

        public override AnimatedTexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _texturePack = new AnimatedTexturePack(ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM, ContainerSize,
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

                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType(), 1,
                    typeof(AnimatedTPKChunks));

                switch (normalizedId)
                {
                    case (long) AnimatedTPKChunks.AnimatedTPKEntries:
                    {
                        DebugUtil.EnsureCondition(_texturePack.Texture == null,
                            () => "There is already an animated texture in this pack!");
                        var anim = BinaryUtil.ReadStruct<AnimatedTextureStruct>(BinaryReader);

                        _texturePack.Texture = new AnimatedTexture
                        {
                            FramesPerSecond = anim.FPS,
                            Hash = anim.Hash,
                            Name = anim.Name,
                            NumFrames = anim.NumFrames
                        };

                        break;
                    }
                    case (long) AnimatedTPKChunks.AnimatedTPKHashes:
                    {
                        DebugUtil.EnsureCondition(n => _texturePack.Texture != null,
                            () => "No texture, but there's a hash section?");

                        var sizeRemaining = chunkRunTo - BinaryReader.BaseStream.Position;

                        DebugUtil.EnsureCondition(
                            n => sizeRemaining / 16 >= _texturePack.Texture.NumFrames,
                            () =>
                                $"Not enough hashes for animation {_texturePack.Texture.Name}! Expected at least {_texturePack.Texture.NumFrames}, got {sizeRemaining / 16}"
                        );

                        for (var j = 0; j < _texturePack.Texture.NumFrames; j++)
                        {
                            _texturePack.Texture.FrameHashes.Add(BinaryReader.ReadInt32());
                            BinaryReader.BaseStream.Seek(12, SeekOrigin.Current);
                        }

                        break;
                    }
                    default:
                    {
                        var data = new byte[chunkSize];
                        BinaryReader.Read(data, 0, data.Length);

                        Console.WriteLine(BinaryUtil.HexDump(data));

                        break;
                    }
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private AnimatedTexturePack _texturePack;
    }
}