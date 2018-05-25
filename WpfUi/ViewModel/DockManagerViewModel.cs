using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace WpfUi.ViewModel
{
    public class DockManagerViewModel : ViewModelBase
    {
        public ObservableCollection<DockWindowViewModel> DockWindows { get; }

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
        }
    }
}
