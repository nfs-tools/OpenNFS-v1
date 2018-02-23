using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFS.UI
{
    public class Group : Entry
    {
        public ObservableCollection<Group> SubGroups { get; set; } = new ObservableCollection<Group>();
        public ObservableCollection<Entry> Entries { get; set; } = new ObservableCollection<Entry>();

        public new bool IsChecked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
            }
        }

        public ObservableCollection<object> Items
        {
            get
            {
                ObservableCollection<object> childNodes = new ObservableCollection<object>();
                foreach (var group in SubGroups)
                    childNodes.Add(group);
                foreach (var entry in Entries)
                    childNodes.Add(entry);

                return childNodes;
            }
        }
    }
}
