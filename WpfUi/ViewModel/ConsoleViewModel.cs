using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using WpfUi.Messages;
using WpfUi.ViewModel.Data;

namespace WpfUi.ViewModel
{
    public class ConsoleViewModel : ViewModelBase
    {
        public ObservableCollection<ConsoleMessage> Messages { get; }

        public ConsoleViewModel()
        {
            Messages = new ObservableCollection<ConsoleMessage>();
            Messenger.Default.Register<ConsoleLogMessage>(this, HandleConsoleLog);
        }

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
