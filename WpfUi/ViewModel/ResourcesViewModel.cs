using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using WpfUi.Messages;
using WpfUi.Services;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class ResourcesViewModel : ViewModelBase
    {
        private IResourceService _resourceService;

        public ObservableCollection<ResourceGroup> Groups { get; set; }

        public ResourcesViewModel(IResourceService resourceService)
        {
            _resourceService = resourceService;

            Groups = new ObservableCollection<ResourceGroup>();

            Messenger.Default.Register<LoadGameMessage>(this, LoadGame);
        }

        private void LoadGame(LoadGameMessage message)
        {
            // Reset data
            Groups.Clear();

            if (!Directory.Exists(message.Directory))
            {
                throw new FileNotFoundException($"Invalid directory: {message.Directory}");
            }

            foreach (var directory in Directory.GetDirectories(message.Directory))
            {
                Groups.Add(CreateTreeGroup(directory, message.Directory));
            }

            foreach (var file in Directory.GetFiles(message.Directory))
            {
                Groups.Add(new GameFile
                {
                    Name = file.Replace($"{message.Directory}\\", ""),
                    Resources = new ObservableCollection<GameResource>(),
                    SubGroups = new ObservableCollection<ResourceGroup>()
                });
            }

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

            foreach (var subdirectory in Directory.GetDirectories(directory))
            {
                group.SubGroups.Add(CreateTreeGroup(subdirectory, directory));
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                group.Resources.Add(new GameFile
                {
                    Name = file.Replace($"{directory}\\", ""),
                    Resources = new ObservableCollection<GameResource>(),
                    SubGroups = new ObservableCollection<ResourceGroup>()
                });
            }

            return group;
        }
    }
}
