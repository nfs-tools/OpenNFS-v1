using System.Collections.Generic;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class Track
    {
        public string Name { get; set; }
        
        public string TrackPath { get; set; }
        
        public string LocRegionShortcode { get; set; }
        
        public string LocRegionPath { get; set; }
        
        public string LocationId { get; set; }
        
        public uint LocationNumber { get; set; }
    }
    
    public class TrackList : BaseModel
    {
        public TrackList(ChunkID id, long size) : base(id, size)
        {
        }
        
        public List<Track> Tracks { get; } = new List<Track>();
    }
}