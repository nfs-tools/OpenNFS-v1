using System.Reflection;
using GalaSoft.MvvmLight;

namespace WpfUi.ViewModel
{
    public class GreetViewModel : ViewModelBase
    {
        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                RaisePropertyChanged();
            }
        }

        public GreetViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
