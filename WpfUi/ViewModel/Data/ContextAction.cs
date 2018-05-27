using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace WpfUi.ViewModel.Data
{
    public class ContextAction<TC> : ObservableObject
    {
        private string _name;
        private RelayCommand<TC> _command;
        private Brush _icon;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<TC> Command
        {
            get => _command;
            set
            {
                _command = value;
                RaisePropertyChanged();
            }
        }

        public Brush Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                RaisePropertyChanged();
            }
        }
    }
}
