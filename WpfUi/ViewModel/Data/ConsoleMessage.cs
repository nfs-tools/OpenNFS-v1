using System.Windows.Media;

namespace WpfUi.ViewModel.Data
{
    public enum MessageLevel
    {
        General,
        Info,
        Success,
        Warning,
        Error,
    }

    /// <summary>
    /// A console message entity.
    /// </summary>
    public class ConsoleMessage
    {
        public MessageLevel Level { get; set; }

        public SolidColorBrush Foreground
        {
            get
            {
                switch (Level)
                {
                    case MessageLevel.Info:
                    {
                        return new SolidColorBrush(Colors.DeepSkyBlue);
                    }
                    case MessageLevel.Success:
                    {
                        return new SolidColorBrush(Colors.LightGreen);
                    }
                    case MessageLevel.Warning:
                    {
                        return new SolidColorBrush(Colors.Orange);
                    }
                    case MessageLevel.Error:
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                    case MessageLevel.General:
                    {
                        return new SolidColorBrush(Colors.DarkGray);
                    }
                    default:
                    {
                        return new SolidColorBrush(Colors.DarkGray);
                    }
                }
            }
        }
        public string Message { get; set; }
        public string FormattedTime { get; set; }
    }
}