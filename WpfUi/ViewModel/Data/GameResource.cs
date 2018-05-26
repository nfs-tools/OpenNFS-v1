using System.Collections.ObjectModel;

namespace WpfUi.ViewModel.Data
{
    /// <summary>
    /// The base class for game resource items.
    /// These are used to display the resource tree.
    /// </summary>
    public abstract class GameResource
    {
        public string Name { get; set; }
        public GameResource Parent { get; set; }

        protected GameResource()
        {
        }
    }
}
