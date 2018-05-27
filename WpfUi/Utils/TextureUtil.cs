using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUi.Utils
{
    public static class TextureUtil
    {
        public static string CompressionTypeString(int type)
        {
            switch (type)
            {
                case 0x31545844:
                {
                    return "DXT1";
                }
                case 0x33545844:
                {
                    return "DXT3";
                }
                case 0x35545844:
                {
                    return "DXT5";
                }
                case 0x32495441:
                {
                    return "ATI2";
                }
                case 0x15:
                {
                    return "P8"; // Unsure about this one
                }
            }

            return "Unknown";
        }
    }
}
