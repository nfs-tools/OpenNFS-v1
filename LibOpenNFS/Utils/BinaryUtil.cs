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

        public static void PrintPosition(BinaryReader reader, Type classType)
        {
            Console.WriteLine($"[{classType.Name}]: Current position: 0x{reader.BaseStream.Position:X16}");
        }

        public static void PrintID(BinaryReader reader, uint id, long normalizedId, uint size, Type classType, int level = 0)
        {
            var pad = "    ".Repeat(level);
            Console.Write($"{pad}[{classType.Name}]: chunk: 0x{id:X8} [{size} bytes] @ 0x{reader.BaseStream.Position:X16}");
            
            if (Enum.IsDefined(typeof(ChunkID), normalizedId))
            {
                Console.Write(" | Type: {0}", ((ChunkID) normalizedId).ToString());
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
            var bytesLength = bytes.Length;

            var hexChars = "0123456789ABCDEF".ToCharArray();

            const int firstHexColumn = 8 // 8 characters for the address
                                       + 3; // 3 spaces

            var firstCharColumn = firstHexColumn
                                  + bytesPerLine * 3 // - 2 digit for the hexadecimal value and 1 space
                                  + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                                  + 2; // 2 spaces 

            var lineLength = firstCharColumn
                             + bytesPerLine // - characters to show the ascii value
                             + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            var line = (new string(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            var expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            var result = new StringBuilder(expectedLines * lineLength);

            for (var i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

                var hexColumn = firstHexColumn;
                var charColumn = firstCharColumn;

                for (var j = 0; j < bytesPerLine; j++)
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
                        var b = bytes[i + j];
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = AsciiSymbol(b);
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                result.Append(line);
            }

            return result.ToString();
        }

        static char AsciiSymbol(byte val)
        {
            if (val < 32) return '.'; // Non-printable ASCII
            if (val < 127) return (char) val; // Normal ASCII
            // Handle the hole in Latin-1
            if (val == 127) return '.';
            if (val < 0x90) return "€.‚ƒ„…†‡ˆ‰Š‹Œ.Ž."[val & 0xF];
            if (val < 0xA0) return ".‘’“”•–—˜™š›œ.žŸ"[val & 0xF];
            if (val == 0xAD) return '.'; // Soft hyphen: this symbol is zero-width even in monospace fonts
            return (char) val; // Normal Latin-1
        }
    }
}