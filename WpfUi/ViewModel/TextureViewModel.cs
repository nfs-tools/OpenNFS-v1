using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Ioc;
using LibOpenNFS.Core.Structures;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using WpfUi.Services;
using WpfUi.Utils;

namespace WpfUi.ViewModel
{
    public class TextureViewModel : DockWindowViewModel
    {
        private readonly IResourceService _resourceService;

        private BitmapImage _textureBitmap;
        private Texture _texture;
        private bool _preserveAlpha;

        /// <summary>
        /// The texture entity.
        /// </summary>
        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Whether to preserve the alpha channel in the preview or not.
        /// </summary>
        public bool PreserveAlpha
        {
            get => _preserveAlpha;
            set
            {
                _preserveAlpha = value;
                RaisePropertyChanged();
                GeneratePreview();
            }
        }

        /// <summary>
        /// The bitmap used for the texture preview.
        /// </summary>
        public BitmapImage TextureBitmap
        {
            get => _textureBitmap;
            set
            {
                _textureBitmap = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public TextureViewModel(string group, uint hash)
        {
            _resourceService = SimpleIoc.Default.GetInstance<IResourceService>();

            Texture = _resourceService.FindTexture(hash, group);
            PreserveAlpha = true;
            Title = $"Texture - {Texture.Name}";
        }

        /// <summary>
        /// Generate the texture bitmap.
        /// </summary>
        private void GeneratePreview()
        {
            // Generate a temporary stream
            using (var ms = new MemoryStream())
            {
                var ddsHeader = new DDSHeader();
                ddsHeader.Init(Texture);

                using (var bw = new BinaryWriter(ms))
                {
                    BinaryUtil.WriteStruct(bw, ddsHeader);
                    bw.Write(Texture.Data, 0, Texture.Data.Length);
                }

                var dds = DDS.LoadImage(ms.ToArray());
                TextureBitmap = ImageUtil.BitmapToBitmapImage(dds);
            }

            //TextureBitmap = ImageUtil.BitmapToBitmapImage(
            //    DDS.LoadImage(Texture.Data, PreserveAlpha));
        }
    }
}
