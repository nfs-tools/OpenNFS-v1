using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static OpenNFSUI.Database.Game;

namespace OpenNFSUI.Database
{
    public struct SupportedDatabase
    {
        public static readonly Game[] SupportedGames = {
            new Game(NFS_HP2_ID, "Need for Speed: Hot Pursuit 2", "nfshp2.exe", Properties.Resources.nfshp2),
            new Game(NFS_UG_ID, "Need for Speed: Underground", "speed.exe", Properties.Resources.ug),
            new Game(NFS_UG2_ID, "Need for Speed: Underground 2", "speed2.exe", Properties.Resources.ug2),
            new Game(NFS_MW_ID, "Need for Speed: Most Wanted", "speed.exe",  Properties.Resources.mw),
            new Game(NFS_CARBON_ID, "Need for Speed: Carbon", "nfsc.exe",  Properties.Resources.carbon),
            new Game(NFS_PROSTREET_ID, "Need for Speed: ProStreet", "nfs.exe",  Properties.Resources.nfsprostreet),
            new Game(NFS_UNDERCOVER_ID, "Need for Speed: Undercover", "nfs.exe",  Properties.Resources.nfsuc),
            new Game(NFS_WORLD_ID, "Need for Speed: World", "nfsw.exe",  Properties.Resources.nfsw),
        };
    }
}
