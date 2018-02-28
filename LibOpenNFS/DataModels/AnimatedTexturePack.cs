using System.Collections.Generic;
using System.Diagnostics;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class AnimatedTexturePack : BaseModel
    {
        public AnimatedTexturePack(ChunkID id, long size) : base(id, size)
        {
            Debug.Assert(id == ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM);
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public int Hash { get; set; }

        public List<Texture> Textures { get; } = new List<Texture>();

        public List<uint> Hashes { get; } = new List<uint>();
    }
}