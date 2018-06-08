namespace WpfUi.ViewModel.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Resource class for a <see cref="T:LibOpenNFS.DataModels.SolidList" />.
    /// </summary>
    public class SolidListResource : GameResource
    {
        /// <summary>
        /// The group ID.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The section ID.
        /// </summary>
        public string SectionId { get; set; }

        /// <summary>
        /// The list name.
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// The tooltip text.
        /// </summary>
        public string ToolTip { get; set; }

        public SolidListResource(string name)
        {
            ListName = name;
        }
    }
}
