using System.Runtime.InteropServices;

namespace LibOpenNFS.Core
{
    public static class CommonStructs
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CompressBlockHead
        {
            public uint CompressBlockMagic; // = 0x55441122
            public uint OutSize; // =0x8000
            public uint TotalBlockSize; // Skip back to before CompressBlockMagic, then jump TotalBlockSize to get to the next block (or, subtract 24)
            public uint Unknown2; // += OutSize
            public uint Unknown3; // += TotalBlockSize
            public uint Unknown4;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct JDLZHeader
        {
            public uint Marker;
            public short Flags; // 0x1002
            public short Unused;
            public int UncompressedLength;
            public int CompressedLength; // includes header!
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Point3D
        {
            public readonly float X;
            
            public readonly float Y;
            
            public readonly float Z;
            
            public readonly float W;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Matrix
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public readonly float[] Data;
        }
    }
}