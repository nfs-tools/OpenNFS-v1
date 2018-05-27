using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibOpenNFS.Utils
{
    public static class BinaryUtil
    {
        public static long PaddingAlign(long num, int alignTo)
        {
            if (num % alignTo == 0)
            {
                return 0;
            }

            return alignTo - num % alignTo;
        }

        /// <summary>
        /// Read a C-style string from a binary file.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(BinaryReader stream)
        {
            var str = new StringBuilder();
            char ch;
            while ((ch = (char) stream.ReadByte()) != 0)
                str.Append(ch);
            return str.ToString();
        }

        /// <summary>
        /// Read a structure from a binary file.
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadStruct<T>(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        /// <summary>
        /// Write a structure to a binary file.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public static void WriteStruct<T>(BinaryWriter writer, T instance)
        {
            writer.Write(MarshalStruct(instance));
        }

        /// <summary>
        /// Marshal a structure to a byte array.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] MarshalStruct<T>(T instance)
        {
            var size = Marshal.SizeOf(instance);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(instance, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        /// <summary>
        /// Repeatedly read a struct of a given type from a binary file into a list.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ReadList<T>(BinaryReader reader, long size)
        {
            var boundary = reader.BaseStream.Position + size;
            var items = new List<T>();
            var itemCount = size / Marshal.SizeOf(typeof(T));

            DebugUtil.EnsureCondition(
                reader.BaseStream.Position + itemCount * Marshal.SizeOf(typeof(T)) <= boundary,
                () => $"Cannot read items of type {typeof(T)} from file! Invalid size ({size}) or type?");

            for (var i = 0; i < itemCount; i++)
                items.Add(ReadStruct<T>(reader));

            return items;
        }

        public static long ComputeEntryCount<T>(long size)
        {
            return size / Marshal.SizeOf(typeof(T));
        }

        public static void ReadPadding(BinaryReader reader, ref uint chunkSize)
        {
            uint pad = 0;

            while (reader.ReadByte() == 0x11)
            {
                pad++;
            }

            // This is a bad hack to get around the fact that sometimes padded chunk data actually starts with 0x11...
            // Padding is always even so if we detect uneven padding, we just jump back 2 bytes instead of 1.
            reader.BaseStream.Seek(pad % 2 == 0 ? -1 : -2, SeekOrigin.Current);

            chunkSize -= pad % 2 == 0 ? pad : pad - 1;
        }

        public static void PrintPosition(BinaryReader reader, Type classType)
        {
#if DEBUG
            Console.WriteLine(
                $"[{classType.Name}]: Current position: 0x{reader.BaseStream.Position:X16} ({reader.BaseStream.Position})");
#endif
        }

        public static void PrintID(BinaryReader reader, uint id, long normalizedId, uint size, Type classType,
            int level = 0, Type enumType = null)
        {
#if DEBUG
            var pad = "    ".Repeat(level);
            Console.Write(
                $"{pad}[{classType.Name}]: chunk: 0x{id:X8} [{size} bytes] @ 0x{reader.BaseStream.Position:X16}");

            enumType = enumType ?? typeof(ChunkID);

            if (Enum.IsDefined(enumType, normalizedId))
            {
                Console.Write(" | Type: {0}", enumType.GetEnumName(normalizedId));
            }

            if (DebugUtil.IsContainerChunk(id))
            {
                Console.Write(" | Container");
            }

            Console.WriteLine();
#endif
        }

        public static void ValidatePosition(BinaryReader reader, long boundary, Type classType)
        {
            if (reader.BaseStream.Position > boundary)
            {
                throw new Exception(
                    $"[{classType.Name}]: Buffer overflow? Chunk runs to 0x{boundary:X16}, we're at 0x{reader.BaseStream.Position:X16} (diff: {(reader.BaseStream.Position - boundary):X16})");
            }
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16, int length = -1)
        {
            if (bytes == null) return "<null>";
            int bytesLength = (length == -1 ? bytes.Length : length);

            char[] hexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                8 // 8 characters for the address
                + 3; // 3 spaces

            int firstCharColumn = firstHexColumn
                                  + bytesPerLine * 3 // - 2 digit for the hexadecimal value and 1 space
                                  + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                                  + 2; // 2 spaces 

            int lineLength = firstCharColumn
                             + bytesPerLine // - characters to show the ascii value
                             + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine)
                .ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

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
                        line[hexColumn] = hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char) b);
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