using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LibOpenNFS.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUi.Messages;
using WpfUi.Utils;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    /// <summary>
    /// The view model for the top menu.
    /// <see cref="OpenCommand"/>
    /// <see cref="AboutCommand"/>
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        public RelayCommand OpenCommand { get; set; }

        public RelayCommand AboutCommand { get; set; }

        public MenuViewModel()
        {
            OpenCommand = new RelayCommand(DoOpenCommand);
        }

        /// <summary>
        /// Show the open-folder dialog and handle the result.
        /// </summary>
        private void DoOpenCommand()
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select game directory",
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
                var game = GameUtil.GetGameFromPath(dialog.FileName);

                if (game == NFSGame.Undetermined)
                {
                    Messenger.Default.Send(new ConsoleLogMessage
                    {
                        Message = "Could not detect game.",
                        Level = MessageLevel.Error
                    });

                    return;
                }

                if (!GameUtil.IsGameSupported(game))
                {
                    Messenger.Default.Send(new ConsoleLogMessage
                    {
                        Message = "This game is unsupported.",
                        Level = MessageLevel.Error
                    });

                    return;
                }

                Messenger.Default.Send(new LoadGameMessage
                {
                    Directory = dialog.FileName,
                    Game = game
                });
            }
        }
    }
}
