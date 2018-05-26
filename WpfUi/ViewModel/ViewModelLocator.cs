using GalaSoft.MvvmLight.Ioc;
using WpfUi.Services;

namespace WpfUi.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<IResourceService, ResourceService>();

            SimpleIoc.Default.Register<GreetViewModel>();
            SimpleIoc.Default.Register<ResourcesViewModel>();
            SimpleIoc.Default.Register<ConsoleViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public GreetViewModel Greet => SimpleIoc.Default.GetInstance<GreetViewModel>();
    }
}
