using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using WpfUi.Messages;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class ConsoleViewModel : ViewModelBase
    {
        public ObservableCollection<ConsoleMessage> Messages { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public ConsoleViewModel()
        {
            Messages = new ObservableCollection<ConsoleMessage>();
            Messenger.Default.Register<ConsoleLogMessage>(this, HandleConsoleLog);
        }

        /// <summary>
        /// Handle a <see cref="ConsoleLogMessage"/> message.
        /// </summary>
        /// <param name="message"></param>
        private void HandleConsoleLog(ConsoleLogMessage message)
        {
            Messages.Insert(0, new ConsoleMessage
            {
                Level = message.Level,
                Message = message.Message,
                FormattedTime = $"{DateTime.Now:T}"
            });
        }
    }
}
