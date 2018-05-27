namespace WpfUi.ViewModel.Data
{
    public struct SupportedDatabase
    {
        public static readonly Game[] SupportedGames = {
            new Game(Game.NFS_HP2_ID, "Need for Speed: Hot Pursuit 2", "nfshp2.exe"),
            new Game(Game.NFS_UG_ID, "Need for Speed: Underground", "speed.exe"),
            new Game(Game.NFS_UG2_ID, "Need for Speed: Underground 2", "speed2.exe"),
            new Game(Game.NFS_MW_ID, "Need for Speed: Most Wanted", "speed.exe"),
            new Game(Game.NFS_CARBON_ID, "Need for Speed: Carbon", "nfsc.exe"),
            new Game(Game.NFS_PROSTREET_ID, "Need for Speed: ProStreet", "nfs.exe"),
            new Game(Game.NFS_UNDERCOVER_ID, "Need for Speed: Undercover", "nfs.exe"),
            new Game(Game.NFS_WORLD_ID, "Need for Speed: World", "nfsw.exe"),
        };
    }
}
