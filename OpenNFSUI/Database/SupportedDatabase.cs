using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFSUI.Database
{
    public struct SupportedDatabase
    {
        public static readonly Game[] SupportedGames = {
            new Game("Need for Speed: Underground", "speed.exe", Properties.Resources.application_16x),
            new Game("Need for Speed: Underground 2", "speed2.exe", Properties.Resources.ug2),
            new Game("Need for Speed: Most Wanted", "speed.exe",  Properties.Resources.mw),
            new Game("Need for Speed: Carbon", "nfsc.exe",  Properties.Resources.carbon),
            new Game("Need for Speed: ProStreet", "nfs.exe",  Properties.Resources.nfsprostreet),
            new Game("Need for Speed: Undercover", "nfs.exe",  Properties.Resources.nfsuc),
            new Game("Need for Speed: World", "nfsw.exe",  Properties.Resources.nfsw),
        };
    }
}
