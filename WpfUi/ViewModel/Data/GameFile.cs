using System;
using System.Collections.ObjectModel;

namespace WpfUi.ViewModel.Data
{
    public class GameFile : ResourceGroup
    {
        /// <summary>
        /// The file extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The full path to the file.
        /// </summary>
        public string FullPath { get; set; }

        public bool IsBundle =>
            string.Equals(Extension, "bun", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(Extension, "bin", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(Extension, "lzc", StringComparison.InvariantCultureIgnoreCase);

        public ObservableCollection<ContextAction<GameFile>> Actions { get; set; }
    }
}
