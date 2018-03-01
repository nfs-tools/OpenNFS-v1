using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibOpenNFS.Utils
{
    public static class BinaryUtil
    {
        public static string ReadNullTerminatedString(BinaryReader stream)
        {
            var str = new StringBuilder();
            char ch;
            while ((ch = (char) stream.ReadByte()) != 0)
                str.Append(ch);
            return str.ToString();
        }

        public static T ByteToType<T>(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static long ComputeEntryCount<T>(long size)
        {
            return size / Marshal.SizeOf(typeof(T));
        }
        
        public static void PrintPosition(BinaryReader reader, Type classType)
        {
            Console.WriteLine(
                $"[{classType.Name}]: Current position: 0x{reader.BaseStream.Position:X16} ({reader.BaseStream.Position})");
        }

        public static void PrintID(BinaryReader reader, uint id, long normalizedId, uint size, Type classType,
            int level = 0, Type enumType = null)
        {
            var pad = "    ".Repeat(level);
            Console.Write(
                $"{pad}[{classType.Name}]: chunk: 0x{id:X8} [{size} bytes] @ 0x{reader.BaseStream.Position:X16}");

            enumType = (enumType == null ? typeof(ChunkID) : enumType);

            if (Enum.IsDefined((enumType), normalizedId))
            {
                Console.Write(" | Type: {0}", enumType.GetEnumName(normalizedId));
            }

            Console.WriteLine();
        }

        public static void ValidatePosition(BinaryReader reader, long boundary, Type classType)
        {
            if (reader.BaseStream.Position > boundary)
            {
                throw new Exception(
                    $"[{classType.Name}]: Buffer overflow? Chunk runs to 0x{boundary:X16}, we're at 0x{reader.BaseStream.Position:X16} (diff: {(reader.BaseStream.Position - boundary):X16})");
            }
        }
        
        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }
    }
}