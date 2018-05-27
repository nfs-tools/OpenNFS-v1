using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUi.ViewModel.Data
{
    public class GameFileContextAction : ContextAction<GameFile>
    {
        public GameFile File { get; set; }
    }
}
