using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class FNGColor
    {
        public uint Red { get; set; }
        public uint Green { get; set; }
        public uint Blue { get; set; }
        public uint Alpha { get; set; }
    }

    public class FNGFile : BaseModel
    {
        public FNGFile(ChunkID id, long size) : base(id, size)
        {
        }

        public string Name { get; set; }

        public List<FNGColor> Colors { get; set; } = new List<FNGColor>();
    }
}