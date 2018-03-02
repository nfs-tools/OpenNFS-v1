using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class Section
    {
        public string ID { get; set; }

        public uint StreamChunkNumber { get; set; }
        public uint StreamChunkHash { get; set; }

        public uint MasterStreamChunkNumber { get; set; }
        public uint MasterStreamChunkOffset { get; set; }

        public uint Size1 { get; set; }
        public uint Size2 { get; set; }
        public uint Size3 { get; set; }

        public float XPos { get; set; }
        public float YPos { get; set; }
        public float ZPos { get; set; }
    }

    public class SectionList : BaseModel
    {
        public SectionList(ChunkID id, long size, long position) : base(id, size, position)
        {
        }
        
        public List<Section> Sections { get; } = new List<Section>();
    }
}