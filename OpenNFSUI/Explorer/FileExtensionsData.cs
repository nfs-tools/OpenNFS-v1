using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFSUI.Explorer
{
    public class FileExtensionsData
    {
        public string Extension { get; private set; }
        public string Type { get; private set; }
        public int ImageIndex { get; private set; }

        public FileExtensionsData(string extension)
        {
            string ext = extension.ToLower();
            ImageIndex = 1;
            Type = "File";
            switch (ext)
            {
                case ".bun":
                    ImageIndex = 2;
                    Type = "Bundle Archive";
                    break;

                case ".dll":
                    ImageIndex = 3;
                    Type = "Dynamic Link Library";
                    break;

                case ".lzc":
                    ImageIndex = 4;
                    Type = "Bundle Archive";
                    break;

                case ".bin":
                    ImageIndex = 5;
                    Type = "Binary File";
                    break;

                case ".abk":
                    ImageIndex = 6;
                    Type = "Audio File";
                    break;

                case ".gin":
                    Type = "Sound File";
                    break;

                case ".big":
                    Type = "EA Game Data File";
                    break;

                case ".fen":
                    Type = "FNG File";
                    break;

                case ".dds":
                    Type = "DirectDraw Surface (Texture)";
                    break;

                case ".xml":
                    Type = "XML File";
                    break;

                case ".txt":
                    Type = "Text File";
                    break;

                case ".hoo":
                    Type = "HOO Log File";
                    break;

                case ".evt":
                    Type = "Audio Event File";
                    break;
            }
        }
    }
}
