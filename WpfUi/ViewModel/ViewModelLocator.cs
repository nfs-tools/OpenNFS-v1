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
            SimpleIoc.Default.Register<MenuViewModel>();
            SimpleIoc.Default.Register<DockManagerViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public GreetViewModel Greet => SimpleIoc.Default.GetInstance<GreetViewModel>();
        public AboutViewModel About => SimpleIoc.Default.GetInstance<AboutViewModel>();
    }
}
