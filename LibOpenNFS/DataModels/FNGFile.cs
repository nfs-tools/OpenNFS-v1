using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class FNGColor
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }
    }

    public class FNGFile : BaseModel
    {
        public FNGFile(ChunkID id, long size) : base(id, size)
        {
        }

        public string Name { get; set; }
        
        public string Path { get; set; }

        public List<FNGColor> Colors { get; set; } = new List<FNGColor>();
    }
}