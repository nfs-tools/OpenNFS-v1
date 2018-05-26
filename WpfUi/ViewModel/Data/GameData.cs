using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibOpenNFS.DataModels;

namespace WpfUi.ViewModel.Data
{
    public class GameData<TM> : GameResource where TM : BaseModel
    {
        public TM Model { get; set; }
    }
}
