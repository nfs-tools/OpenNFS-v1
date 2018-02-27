using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW
{
    public class MWTrackListContainer : Container<TrackList>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TrackStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string TrackName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string TrackPath;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public readonly string LocRegionShortcode;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string LocRegionPath;

            public readonly uint LocationNumber;
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
            public readonly string LocationName;

            private readonly uint unknownMarker;

            private readonly short unknown;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x96)]
            private readonly byte[] restOfData;
        }
        
        public MWTrackListContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override TrackList Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _trackList = new TrackList(ChunkID.BCHUNK_TRACKINFO, ContainerSize);

            ReadChunks(ContainerSize);
            
            return _trackList;
        }

        protected override void ReadChunks(long totalSize)
        {
            var numTracks = totalSize / Marshal.SizeOf(typeof(TrackStruct));

            for (var i = 0; i < numTracks; i++)
            {
                var track = BinaryUtil.ByteToType<TrackStruct>(BinaryReader);
                
//                Console.WriteLine($"Track #{i + 1:00}: {track.TrackName} ({track.TrackPath}) in {track.LocRegionShortcode} ({track.LocRegionPath} / {track.LocationName} / {track.LocationNumber})");
                _trackList.Tracks.Add(new Track
                {
                    Name = track.TrackName,
                    TrackPath = track.TrackPath,
                    LocationId = track.LocationName,
                    LocationNumber = track.LocationNumber,
                    LocRegionPath = track.LocRegionPath,
                    LocRegionShortcode = track.LocRegionShortcode
                });
            }
        }

        private TrackList _trackList;
    }
}