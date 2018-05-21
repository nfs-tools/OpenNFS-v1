namespace LibOpenNFS.Core.Crypto
{
    /// <summary>
    /// Taken from https://github.com/MWisBest/OpenNFSTools/blob/master/LibNFS/Crypto/CRC32NFS.cs.
    /// Credit to MWisBest.
    /// </summary>
    public static class NFSCRC
    {
        private static readonly uint[] CrcTable = new uint[256];

        /// <summary>
        /// This static initializer generates the crc table.
        /// The games have it hardcoded, but obviously they had to be generated somehow! :)
        /// </summary>
        static NFSCRC()
        {
            for (uint dividend = 0; dividend < 256; ++dividend)
            {
                var remainder = dividend << 24;

                for (uint bit = 0; bit < 8; ++bit)
                {
                    if ((remainder & 0x80000000u) > 0)
                    {
                        remainder = (remainder << 1) ^ 0x04C11DB7u;
                    }
                    else
                    {
                        remainder = remainder << 1;
                    }
                }

                CrcTable[dividend] = remainder;
            }
        }

        /// <summary>
        /// Gets the 'CRC32-NFS' of a given byte array.
        /// </summary>
        /// <param name="data">bytes to hash</param>
        /// <returns>calculated CRC32-NFS hash</returns>
        public static uint GetHash(byte[] data)
        {
            var crc32 = 0x00000000u;

            if (data.Length < 4) return crc32;
            
            var index = 0;
            crc32 = ~(uint) (data[index + 3] | (data[index + 2] << 8) | (data[index + 1] << 16) |
                             (data[index] << 24));
            index += 4;
            while (index < data.Length)
            {
                crc32 = CrcTable[crc32 >> 24] ^ ((crc32 << 8) | data[index++]);
            }

            crc32 = ~crc32;

            return crc32;
        }
    }
}