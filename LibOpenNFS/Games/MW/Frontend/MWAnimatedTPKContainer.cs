using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW.Frontend
{
    public class MWAnimatedTPKContainer : Container<AnimatedTexturePack>
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

        public MWAnimatedTPKContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader,
            containerSize)
        {
        }

        public override AnimatedTexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _texturePack = new AnimatedTexturePack(ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM, ContainerSize, BinaryReader.BaseStream.Position);

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
                        for (var j = 0; j < BinaryUtil.ComputeEntryCount<AnimatedTextureStruct>(chunkSize); j++)
                        {
                            var anim = BinaryUtil.ByteToType<AnimatedTextureStruct>(BinaryReader);
                            
                            _texturePack.Textures.Add(new AnimatedTexture
                            {
                                FramesPerSecond = anim.FPS,
                                Hash = anim.Hash,
                                Name = anim.Name,
                                NumFrames = anim.NumFrames
                            });
                        }
                        
                        break;
                    }
                    case (long) AnimatedTPKChunks.AnimatedTPKHashes:
                    {
                        DebugUtil.EnsureCondition(n => _texturePack.Textures.Any(), () => "No textures, but there's a hash section?");
                        
                        foreach (var texture in _texturePack.Textures)
                        {
                            var sizeRemaining = chunkRunTo - BinaryReader.BaseStream.Position;

                            DebugUtil.EnsureCondition(
                                n => sizeRemaining / 16 >= texture.NumFrames,
                                () => $"Not enough hashes for animation {texture.Name}! Expected at least {texture.NumFrames}, got {sizeRemaining / 16}"
                            );
                            
                            Console.WriteLine($"Animation: {texture.Name}");

                            for (var j = 0; j < texture.NumFrames; j++)
                            {
                                texture.FrameHashes.Add(BinaryReader.ReadInt32());
                                BinaryReader.BaseStream.Seek(12, SeekOrigin.Current);
                                
                                Console.WriteLine($"Animation hash #{j + 1:00}: 0x{texture.FrameHashes[j]:X8}");
                            }
                            
//                            DebugUtil.EnsureCondition(n => (chunkRunTo - BinaryReader.BaseStream.Position) / 16 >= texture.NumFrames,
//                                () => $"Not enough hashes! Expected at least {texture.NumFrames}");
                        }
                        
//                        DebugUtil.EnsureCondition(n => (chunkSize / 16) % 2 == 0 && chunkSize / 16 == _texturePack.Textures.Count, 
//                            () => $"Invalid hash count: {_texturePack.Textures.Count} (expected {chunkSize / 16})");
                        
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