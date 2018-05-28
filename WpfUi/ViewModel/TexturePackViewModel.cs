using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using LibOpenNFS.Core.Structures;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUi.Messages;
using WpfUi.Services;
using WpfUi.Utils;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class TextureProxy : ObservableObject
    {
        private string _name;
        private int _width;
        private int _height;
        private uint _hash;
        private uint _type;
        private string _format;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public uint Hash
        {
            get => _hash;
            set
            {
                _hash = value;
                RaisePropertyChanged();
            }
        }

        public uint Type
        {
            get => _type;
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }
    }

    public class TexturePackViewModel : DockWindowViewModel
    {
        private readonly IResourceService _resourceService;
        private TexturePack _texturePack;
        private string _groupId;

        public string GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                RaisePropertyChanged();
            }
        }

        public TexturePack Pack
        {
            get => _texturePack;
            set
            {
                _texturePack = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ExportSelectedCommand { get; }
        public RelayCommand<TextureProxy> ViewTextureCommand { get; }

        public ObservableCollection<TextureProxy> Textures { get; }

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <param name="texturePack"></param>
        public TexturePackViewModel(TexturePackResource texturePack)
        {
            _resourceService = SimpleIoc.Default.GetInstance<IResourceService>();
            _groupId = texturePack.GroupId;

            Pack = texturePack.Pack;
            Textures = new ObservableCollection<TextureProxy>(
                texturePack.Pack.Textures.Select(
                    tex => 
                        new TextureProxy
                        {
                            Name = tex.Name,
                            Height = tex.Height,
                            Width = tex.Width,
                            Hash = tex.TextureHash,
                            Type = tex.TypeHash,
                            Format = TextureUtil.CompressionTypeString(tex.CompressionType)
                        }
            ));

            ExportSelectedCommand = new RelayCommand(ExportSelected);
            ViewTextureCommand = new RelayCommand<TextureProxy>(ViewTexture);
            Title = $"Texture Pack - {texturePack.Pack.Name}";
        }

        /// <summary>
        /// Open a texture view document.
        /// </summary>
        /// <param name="obj"></param>
        private void ViewTexture(TextureProxy obj)
        {
            Messenger.Default.Send(new OpenTextureMessage
            {
                Hash = obj.Hash,
                GroupId = _groupId
            });
        }

        /// <summary>
        /// Export the selected textures to DDS files.
        /// </summary>
        private void ExportSelected()
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select output directory",
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                EnsureFileExists = false,
                EnsurePathExists = false,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = false,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var ddsHeader = new DDSHeader();

                var selectedTextures = Textures.Where(tex => tex.IsSelected).ToList();
                foreach (var texture in selectedTextures)
                {
                    var fullTex = _resourceService.FindTexture(texture.Hash, _groupId);
                    var outPath = Path.Combine(dialog.FileName,
                        $"{GroupId}_{Pack.Name}_{texture.Name}_0x{texture.Hash:X8}.dds");

                    Messenger.Default.Send(new ConsoleLogMessage
                    {
                        Level = MessageLevel.Info,
                        Message = $"Exporting texture {texture.Name} to {outPath}"
                    });

                    ddsHeader.Init(fullTex);
                    
                    using (var stream = File.OpenWrite(outPath))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            BinaryUtil.WriteStruct(writer, ddsHeader);
                            writer.Write(fullTex.Data, 0, fullTex.Data.Length);
                        }
                    }
                }

                Messenger.Default.Send(new ConsoleLogMessage
                {
                    Level = MessageLevel.Info,
                    Message = $"Exported {selectedTextures.Count} texture(s)"
                });

                Process.Start(dialog.FileName);
            }
        }
    }
}
