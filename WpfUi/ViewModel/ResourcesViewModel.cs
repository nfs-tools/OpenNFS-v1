using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using WpfUi.Messages;
using WpfUi.Services;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class ResourcesViewModel : ViewModelBase
    {
        private readonly IResourceService _resourceService;
        private NFSGame _currentGame;

        public RelayCommand<GameFile> LoadFileCommand { get; }

        public RelayCommand<TexturePackResource> ViewTexturePackCommand { get; }

        public ObservableCollection<ResourceGroup> Groups { get; set; }

        public ResourcesViewModel(IResourceService resourceService)
        {
            _resourceService = resourceService;
            _currentGame = NFSGame.Undetermined;

            Groups = new ObservableCollection<ResourceGroup>();

            Messenger.Default.Register<LoadGameMessage>(this, LoadGame);

            LoadFileCommand = new RelayCommand<GameFile>(LoadFile);
            ViewTexturePackCommand = new RelayCommand<TexturePackResource>(ViewTexturePack);
        }

        /// <summary>
        /// Open a new texture pack view.
        /// </summary>
        /// <param name="obj"></param>
        private void ViewTexturePack(TexturePackResource obj)
        {
            Messenger.Default.Send(new OpenTexturePackMessage
            {
                Pack = obj,
            });
        }

        /// <summary>
        /// Load resources from a file.
        /// </summary>
        /// <param name="obj"></param>
        private async void LoadFile(GameFile obj)
        {
            Messenger.Default.Send(new ConsoleLogMessage
            {
                Level = MessageLevel.Info,
                Message = $"Loading file: {obj.FullPath}"
            });

            var groupId = Path.GetFileNameWithoutExtension(obj.FullPath);
            var models = await _resourceService.Load(obj.FullPath, groupId, _currentGame);
            var groupTree = CreateResourceTree(models, groupId);

            obj.SubGroups = new ObservableCollection<ResourceGroup>(groupTree);

            foreach (var modelGroup in models.GroupBy(m => m.GetType()))
            {
                Messenger.Default.Send(new ConsoleLogMessage
                {
                    Level = MessageLevel.Info,
                    Message = $"{modelGroup.Key.FullName}: {modelGroup.Count()}"
                });
            }

            Messenger.Default.Send(new ConsoleLogMessage
            {
                Level = MessageLevel.Success,
                Message = $"Loaded {models.Count} model(s) from {obj.FullPath}"
            });
        }

        /// <summary>
        /// Load a game directory and build the directory tree.
        /// </summary>
        /// <param name="message"></param>
        private void LoadGame(LoadGameMessage message)
        {
            // Reset data
            _resourceService.PurgeResources();
            Groups.Clear();

            if (!Directory.Exists(message.Directory))
            {
                throw new FileNotFoundException($"Invalid directory: {message.Directory}");
            }

            foreach (var directory in Directory.EnumerateDirectories(message.Directory))
            {
                Groups.Add(CreateTreeGroup(directory, message.Directory));
            }

            foreach (var file in Directory.EnumerateFiles(message.Directory))
            {
                var gameFile = new GameFile
                {
                    Name = file.Replace($"{message.Directory}\\", ""),
                    Extension = Path.GetExtension(file).Replace(".", ""),
                    Resources = new ObservableCollection<GameResource>(),
                    SubGroups = new ObservableCollection<ResourceGroup>(),
                    FullPath = file
                };

                gameFile.Actions = BuildActionList(gameFile);

                Groups.Add(gameFile);
            }

            _currentGame = message.Game;

            Messenger.Default.Send(new WindowTitleMessage
            {
                Title = $"OpenNFS | {message.Directory}"
            });

            Messenger.Default.Send(new ConsoleLogMessage
            {
                Level = MessageLevel.Info,
                Message = $"Loaded game. Detected: {message.Game}"
            });
        }

        private ResourceGroup CreateTreeGroup(string directory, string baseDirectory)
        {
            var display = directory.Replace($"{baseDirectory}\\", "");
            var group = new ResourceGroup
            {
                Name = display,
                Resources = new ObservableCollection<GameResource>(),
                SubGroups = new ObservableCollection<ResourceGroup>()
            };

            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                group.SubGroups.Add(CreateTreeGroup(subdirectory, directory));
            }

            foreach (var file in Directory.EnumerateFiles(directory))
            {
                var gameFile = new GameFile
                {
                    Name = file.Replace($"{directory}\\", ""),
                    Extension = Path.GetExtension(file).Replace(".", ""),
                    Resources = new ObservableCollection<GameResource>(),
                    SubGroups = new ObservableCollection<ResourceGroup>(),
                    FullPath = file
                };

                gameFile.Actions = BuildActionList(gameFile);

                group.Resources.Add(gameFile);
            }

            group.UpdateItems();

            return group;
        }

        private ObservableCollection<ContextAction<GameFile>> BuildActionList(GameFile file)
        {
            var collection = new ObservableCollection<ContextAction<GameFile>>();

            if (file.IsBundle)
            {
                collection.Add(new GameFileContextAction
                {
                    Name = "Load",
                    Command = LoadFileCommand,
                    File = file
                });
            }

            return collection;
        }

        /// <summary>
        /// Generate a resource tree for a file record.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="groupId"></param>
        private IEnumerable<ResourceGroup> CreateResourceTree(IEnumerable<BaseModel> models, string groupId)
        {
            var groups = new List<ResourceGroup>();

            foreach (var modelGroup in models.GroupBy(m => m.GetType()))
            {
                if (modelGroup.Key == typeof(TexturePack))
                {
                    var resources = modelGroup
                        .Cast<TexturePack>()
                        .Select(m => new TexturePackResource(m)
                        {
                            GroupId = groupId
                        });

                    groups.Add(new ResourceGroup
                    {
                        Name = "Texture Packs",
                        Resources = new ObservableCollection<GameResource>(resources),
                        SubGroups = new ObservableCollection<ResourceGroup>(),
                    });
                }
            }

            return groups;
        }
    }
}
