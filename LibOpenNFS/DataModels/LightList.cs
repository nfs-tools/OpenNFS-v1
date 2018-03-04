using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class Light
    {
        
    }
    
    public class LightList : BaseModel
    {
        public LightList(long id, long size, long position) : base(id, size, position)
        {
        }

        public LightList(ChunkID id, long size, long position) : base(id, size, position)
        {
            DebugUtil.EnsureCondition(
                id == ChunkID.BCHUNK_SPEED_ELIGHT_CHUNKS,
                () => $"Expected BCHUNK_SPEED_ELIGHT_CHUNKS, got {id.ToString()}");
        }
        
        public uint NumLights { get; set; }
        
        public List<Light> Lights { get; } = new List<Light>();
    }
}