using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUi.ViewModel.Data
{
    public class ResourceGroup : GameResource
    {
        public ObservableCollection<ResourceGroup> SubGroups { get; set; } = new ObservableCollection<ResourceGroup>();
        public ObservableCollection<GameResource> Resources { get; set; } = new ObservableCollection<GameResource>();

        public ObservableCollection<object> Items
        {
            get
            {
                var children = new ObservableCollection<object>();
                
                foreach (var group in SubGroups)
                    children.Add(group);
                foreach (var resource in Resources) 
                    children.Add(resource);
                return children;
            }
        }
    }
}
