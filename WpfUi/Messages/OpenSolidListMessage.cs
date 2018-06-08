using WpfUi.ViewModel.Data;

namespace WpfUi.Messages
{
    /// <summary>
    /// Sent when a solid list view needs to be shown.
    /// </summary>
    public class OpenSolidListMessage
    {
        /// <summary>
        /// The solid list resource.
        /// </summary>
        public SolidListResource SolidList { get; set; }
    }
}
