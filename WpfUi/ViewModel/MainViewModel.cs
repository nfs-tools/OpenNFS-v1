using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace WpfUi.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public DockManagerViewModel DockManagerViewModel { get; }

        public ResourcesViewModel ResourcesViewModel { get; }

        public MainViewModel()
        {
            DockManagerViewModel = new DockManagerViewModel();
            ResourcesViewModel = SimpleIoc.Default.GetInstance<ResourcesViewModel>();
        }
    }
}
