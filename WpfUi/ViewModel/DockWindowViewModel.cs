using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace WpfUi.ViewModel
{
    public abstract class DockWindowViewModel : ViewModelBase
    {
        #region Properties

        #region CloseCommand
        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand => _closeCommand ?? 
                                            (_closeCommand = new RelayCommand(Close));

        #endregion

        #region IsClosed
        private bool _isClosed;
        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                if (_isClosed != value)
                {
                    _isClosed = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region CanClose
        private bool _canClose;
        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Title
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #endregion

        protected DockWindowViewModel()
        {
            this.CanClose = true;
            this.IsClosed = false;
            this.IsSelected = true;
        }

        public void Close()
        {
            this.IsClosed = true;
        }
    }
}
