using System.Collections.Generic;
using System.Diagnostics;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class AnimatedTexture
    {
        public string Name { get; set; }
        public int Hash { get; set; }
        public int NumFrames { get; set; }
        public int FramesPerSecond { get; set; }
    }

    public class AnimatedTexturePack : BaseModel
    {
        public AnimatedTexturePack(ChunkID id, long size) : base(id, size)
        {
            Debug.Assert(id == ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM);
        }

        public List<AnimatedTexture> Textures { get; } = new List<AnimatedTexture>();

        public List<uint> Hashes { get; } = new List<uint>();
    }
}