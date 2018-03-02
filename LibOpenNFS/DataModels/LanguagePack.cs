using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class LanguageEntry
    {
        public uint HashOne { get; set; }
        public uint HashTwo { get; set; }
        public string Text { get; set; }
    }

    public class LanguagePack : BaseModel
    {
        public LanguagePack(ChunkID id, long size, long position) : base(id, size, position)
        {
        }

        public uint NumStrings { get; set; }

        public string Name { get; set; } = "No name";

        public List<LanguageEntry> Entries { get; set; } = new List<LanguageEntry>();
    }
}