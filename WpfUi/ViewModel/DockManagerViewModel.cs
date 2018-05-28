using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using WpfUi.Messages;

namespace WpfUi.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    /// The view model for the main dock manager.
    /// Manages documents.
    /// </summary>
    public class DockManagerViewModel : ViewModelBase
    {
        /// <summary>
        /// The observable list of documents.
        /// </summary>
        public ObservableCollection<DockWindowViewModel> DockWindows { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public DockManagerViewModel()
        {
            DockWindows = new ObservableCollection<DockWindowViewModel>
            {
                new WelcomeViewModel
                {
                    Title = "Welcome",
                    CanClose = false
                }
            };
            
            RegisterMessages();
        }

        /// <summary>
        /// Register listeners for opening messages.
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<OpenTexturePackMessage>(this, HandleOpenTexturePack);
            Messenger.Default.Register<OpenTextureMessage>(this, HandleOpenTexture);
        }

        /// <summary>
        /// Handle an <see cref="OpenTextureMessage"/> message.
        /// </summary>
        /// <param name="message"></param>
        private void HandleOpenTexture(OpenTextureMessage message)
        {
            DockWindows.Add(new TextureViewModel(message.GroupId, message.Hash));
        }

        /// <summary>
        /// Handle an <see cref="OpenTexturePackMessage"/> message.
        /// </summary>
        /// <param name="message"></param>
        private void HandleOpenTexturePack(OpenTexturePackMessage message)
        {
            DockWindows.Add(new TexturePackViewModel(message.Pack));
        }
    }
}
