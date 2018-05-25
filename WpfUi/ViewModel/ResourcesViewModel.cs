using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class ResourcesViewModel : ViewModelBase
    {
        public ObservableCollection<ResourceGroup> Groups { get; set; }

        public ResourcesViewModel()
        {
            Groups = new ObservableCollection<ResourceGroup>
            {
                new ResourceGroup
                {
                    Name = "Test",
                    Resources = new ObservableCollection<GameResource>
                    {
                        new GameFile
                        {
                            Name = "Test file"
                        }
                    }
                }
            };
        }
    }
}
