using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace OpenNFS.UI
{
    public class Entry : IEntry, INotifyPropertyChanged
    {
        //public enum EntryType
        //{
        //    StandardEntry,
        //    FileEntry,
        //    Group,
        //    TPKGroup,
        //    TPKGroupItem,
        //    SolidsGroup,
        //    SolidsGroupItem
        //}

        protected bool _checked;

        public int Key { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsEnabled { get; set; }
        public IEntry Parent { get; set; }

        public bool IsChecked
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
