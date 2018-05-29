using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WpfUi.ViewModel
{
    public class AboutViewModel : ViewModelBase
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

        public AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
