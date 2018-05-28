using System;
using System.Text;

namespace WpfUi.Utils
{
    public static class BinHash
    {
        /// <summary>
        /// Calculates BIN hash, which is used at many places in NFS games.
        /// String is converted to a byte array before processing.
        /// </summary>
        public static int Hash(string stringToHash)
        {
           var byteArrayToHash = Encoding.GetEncoding(1252).GetBytes(stringToHash);

           return Hash(byteArrayToHash);
        }

        /// <summary>
        /// Calculates BIN hash, which is used at many places in NFS games.
        /// </summary>
        public static int Hash(byte[] byteArrayToHash)
        {
            var v1 = 0;
            var i = -1;

            while (v1 < byteArrayToHash.Length)
            {
                i = byteArrayToHash[v1] + 33 * i;
                v1++;
            }

            return i;
        }

        /*
        /// <summary>
        /// Unhashes BIN hash, may not work correctly because of limitations.
        /// </summary>
        public static byte[] UnHash(int IntToUnhash)
        {
            if (IntToUnhash == -1) return null;

            var ByteArrayUnhashed = new List<byte>();

            int UnHashAttempt = IntToUnhash;

            while (UnHashAttempt != -1)
            {
                int Cr = 0;
                var FailedAttempts = new List<byte>();

                for (int i=32; i<127; i++)
                {
                    if (FailedAttempts.Count != 0)
                    {
                        i = FailedAttempts[Cr] + 1;
                    }
                    if (UnHashAttempt < 32)
                    {
                        FailedAttempts = ByteArrayUnhashed;
                        UnHashAttempt = IntToUnhash;
                        Cr = 0;
                    }

                    if ((UnHashAttempt - i == -1))
                    {
                        UnHashAttempt = -1;
                        ByteArrayUnhashed.Add(Convert.ToByte(i));
                        break;
                    }

                    if (((UnHashAttempt - i) % 33 == 0))
                    {
                        UnHashAttempt = (UnHashAttempt - i) / 33;
                        ByteArrayUnhashed.Add(Convert.ToByte(i));
                        i = 31;
                    }
                }
                break;
            }

            ByteArrayUnhashed.Reverse();

            return ByteArrayUnhashed.ToArray();
        }
        */
    }
}
