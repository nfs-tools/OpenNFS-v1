using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOpenNFS.Core;
using WpfUi.ViewModel.Data;

namespace WpfUi.Utils
{
    public static class GameUtil
    {
        public static bool IsGameSupported(NFSGame game)
        {
            return game == NFSGame.World;
        }

        public static NFSGame GetGameFromPath(string path)
        {
            foreach (var game in SupportedDatabase.SupportedGames)
            {
                var fileName = game.ExectuableFileName;
                var final = Path.Combine(path, fileName);

                if (File.Exists(final))
                {
                    return GameSymbolData.GetGameBySymbolEntry(final);
                }
            }

            return NFSGame.Undetermined;
        }
    }
}