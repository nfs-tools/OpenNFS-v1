using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class SolidList : BaseModel
    {
        public SolidList(ChunkID id, long size, long position) : base(id, size, position)
        {
        }

        public List<uint> Hashes { get; set; } = new List<uint>();
        
        public string Name { get; set; }
        
        public string SectionId { get; set; }
    }
}