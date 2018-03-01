using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class NullModel : BaseModel
    {
        public NullModel(long id, long size) : base(id, size)
        {
        }
        
        public NullModel(ChunkID id, long size) : base(id, size)
        {
        }
    }
}