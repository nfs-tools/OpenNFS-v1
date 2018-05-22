using System.Runtime.InteropServices;

namespace LibOpenNFS.Core
{
    /// <summary>
    /// P/Invoke based wrapper for complib
    /// </summary>
    public static class Compression
    {
        [DllImport("complib", EntryPoint = "init")]
        public static extern void Init();
        
        [DllImport("complib", EntryPoint = "decompressDK2")]
        public static extern void DecompressDk2(
            [In] byte[] inData,
            [Out] byte[] outData
        );
        
        [DllImport("complib", EntryPoint = "decompressHuff")]
        public static extern void DecompressHuff(
            [In] byte[] inData,
            [Out] byte[] outData
        );
    }
}