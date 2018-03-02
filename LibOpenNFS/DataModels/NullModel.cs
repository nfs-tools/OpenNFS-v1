using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class NullModel : BaseModel
    {
        public NullModel(long id, long size, long position) : base(id, size, position)
        {
        }
        
        public NullModel(ChunkID id, long size, long position) : base(id, size, position)
        {
        }
    }
}