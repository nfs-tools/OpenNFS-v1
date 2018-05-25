using GalaSoft.MvvmLight.Ioc;

namespace WpfUi.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<GreetViewModel>();
            SimpleIoc.Default.Register<ResourcesViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public GreetViewModel Greet => SimpleIoc.Default.GetInstance<GreetViewModel>();
    }
}
