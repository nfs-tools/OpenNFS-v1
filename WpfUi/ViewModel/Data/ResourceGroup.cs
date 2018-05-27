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
        private ObservableCollection<ResourceGroup> _subGroups = new ObservableCollection<ResourceGroup>();
        private ObservableCollection<GameResource> _resources = new ObservableCollection<GameResource>();

        public ObservableCollection<ResourceGroup> SubGroups
        {
            get => _subGroups;
            set
            {
                _subGroups = value;
                UpdateItems();
            }
        }

        public ObservableCollection<GameResource> Resources
        {
            get => _resources;
            set
            {
                _resources = value;
                UpdateItems();
            }
        }

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Update the item tree.
        /// </summary>
        public void UpdateItems()
        {
            var children = new ObservableCollection<object>();

            foreach (var group in SubGroups)
                children.Add(group);
            foreach (var resource in Resources)
                children.Add(resource);

            Items.Clear();

            foreach (var child in children)
                Items.Add(child);
        }
    }
}
