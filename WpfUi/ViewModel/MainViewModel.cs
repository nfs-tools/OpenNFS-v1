using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using LibOpenNFS.Core;
using WpfUi.Messages;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _windowTitle;

        public DockManagerViewModel DockManagerViewModel { get; }

        public ResourcesViewModel ResourcesViewModel { get; }

        public ConsoleViewModel ConsoleViewModel { get; }

        public MenuViewModel MenuViewModel { get; }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                RaisePropertyChanged();
            }
        }

        public MainViewModel()
        {
            WindowTitle = "OpenNFS";
            DockManagerViewModel = SimpleIoc.Default.GetInstance<DockManagerViewModel>();
            ResourcesViewModel = SimpleIoc.Default.GetInstance<ResourcesViewModel>();
            ConsoleViewModel = SimpleIoc.Default.GetInstance<ConsoleViewModel>();
            MenuViewModel = SimpleIoc.Default.GetInstance<MenuViewModel>();

            Initialize();

            Messenger.Default.Send(new ConsoleLogMessage
            {
                Level = MessageLevel.Info,
                Message = "Started OpenNFS."
            });
        }

        /// <summary>
        /// Initialize listeners.
        /// </summary>
        private void Initialize()
        {
            Messenger.Default.Register<WindowTitleMessage>(this, HandleWindowTitle);
        }

        private void HandleWindowTitle(WindowTitleMessage message)
        {
            WindowTitle = message.Title;
        }
    }
}
