using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenNFSUI.Explorer
{
    public class FileExtensionsData
    {
        public const int FILE_ICON = 0;
        public const int FILE_AUDIO_ICON = 1;
        public const int FILE_BUNDLE_ICON = 2;
        public const int FILE_DLL_ICON = 3;
        public const int FILE_MODEL_ICON = 4;
        public const int FILE_TEXTURE_ICON = 5;
        public const int FILE_VIDEO_ICON = 6;
        public const int FILE_FOLDER = 7;

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
                    ImageIndex = FILE_BUNDLE_ICON;
                    Type = "Bundle Archive";
                    break;

                case ".dll":
                    ImageIndex = FILE_DLL_ICON;
                    Type = "Dynamic Link Library";
                    break;

                case ".lzc":
                    ImageIndex = FILE_BUNDLE_ICON;
                    Type = "Bundle Archive";
                    break;

                case ".bin":
                    ImageIndex = FILE_BUNDLE_ICON;
                    Type = "Binary File";
                    break;

                case ".abk":
                    ImageIndex = FILE_AUDIO_ICON;
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

    public static class StaticControls
    {
    }
}
