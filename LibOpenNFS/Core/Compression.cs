using System.Runtime.InteropServices;

namespace LibOpenNFS.Core
{
    /// <summary>
    /// P/Invoke based wrapper for complib
    /// </summary>
    public static class Compression
    {
        [DllImport("complib", EntryPoint = "LZDecompress", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Decompress(
            [In] byte[] inData,
            [Out] byte[] outData
        );
    }
}