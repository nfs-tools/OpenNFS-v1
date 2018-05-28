using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using LibOpenNFS.Core;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUi.Messages;
using WpfUi.Utils;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    /// The view model for the top menu.
    /// <see cref="P:WpfUi.ViewModel.MenuViewModel.OpenCommand" />
    /// <see cref="P:WpfUi.ViewModel.MenuViewModel.AboutCommand" />
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        private IDialogCoordinator _dialogCoordinator;

        public RelayCommand OpenCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand BinHashCommand { get; }
        public RelayCommand JenkinsHashCommand { get; }
        public RelayCommand Crc32Command { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public MenuViewModel()
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            OpenCommand = new RelayCommand(DoOpenCommand);
            BinHashCommand = new RelayCommand(DoBinHashCommand);
            JenkinsHashCommand = new RelayCommand(DoJenkinsHashCommand);
        }

        /// <summary>
        /// Open the Jenkins hash dialog.
        /// </summary>
        private async void DoJenkinsHashCommand()
        {
            var strToHash = await _dialogCoordinator.ShowInputAsync(this, "Jenkins Hash", "What would you like to hash?");

            if (string.IsNullOrWhiteSpace(strToHash)) return;

            var hash32 = JenkinsHash.GetHash32(strToHash);
            var hash64 = JenkinsHash.GetHash64(strToHash);
            await _dialogCoordinator.ShowMessageAsync(this, "Hash Result",
                $"The 32-bit Jenkins hash of \"{strToHash}\" is 0x{hash32:X8}.\nThe 64-bit Jenkins hash is 0x{hash64:X16}.");
        }

        /// <summary>
        /// Open the bin-hash dialog.
        /// </summary>
        private async void DoBinHashCommand()
        {
            var strToHash = await _dialogCoordinator.ShowInputAsync(this, "BIN Hash", "What would you like to hash?");

            if (string.IsNullOrWhiteSpace(strToHash)) return;

            var hash = BinHash.Hash(strToHash);
            await _dialogCoordinator.ShowMessageAsync(this, "Hash Result",
                $"The BIN hash of \"{strToHash}\" is 0x{hash:X8}.");
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
