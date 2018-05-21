using System.Runtime.InteropServices;
using LibOpenNFS.DataModels;

namespace LibOpenNFS.Core.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PixelFormat
    {
        public int Size;
        public int Flags;
        public int FourCC;
        public int RGBBitCount;
        public int RBitMask;
        public int GBitMask;
        public int BBitMask;
        public int AlphaBitMask;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DDSCaps
    {
        public int Caps1;
        public int Caps2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] Reserved;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DDSHeader
    {
        public int Magic;
        public int Size;
        public int Flags;
        public int Height;
        public int Width;
        public int PitchOrLinearSize;
        public int Depth;
        public int MipMapCount;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public int[] Reserved1;

        public PixelFormat PixelFormat;
        public DDSCaps DDSCaps;

        public int Reserved2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void Init(Texture texture)
        {
            Magic = 0x20534444; // "DDS "
            Size = 0x7C;
            Flags = 0x81007; // DDSD_CAPS | DDSD_PIXELFORMAT | DDSD_WIDTH | DDSD_HEIGHT | DDSD_LINEARSIZE;
            Height = texture.Height;
            Width = texture.Width;
            PitchOrLinearSize = (int) texture.DataSize;
            PixelFormat.Size = 0x20;
            PixelFormat.Flags = (texture.CompressionType & 0xFFFF) == 0x5844 ? 4 // DDPF_FOURCC
                : 0;
            PixelFormat.FourCC = texture.CompressionType;
            
            if (0 == PixelFormat.Flags)
            {
                PixelFormat.RBitMask = unchecked((int) 0xFF000000); // Because C#.
                PixelFormat.GBitMask = 0x00FF0000;
                PixelFormat.BBitMask = 0x0000FF00;
                PixelFormat.AlphaBitMask = 0x000000FF;
            }

            DDSCaps.Caps1 = 0x1000; // DDSCAPS_TEXTURE
        }
    }
}