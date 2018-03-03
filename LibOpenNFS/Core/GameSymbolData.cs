using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using static LibOpenNFS.Core.CoreFunctions;

namespace LibOpenNFS.Core
{
    public static class GameSymbolData
    {
        /// <summary>
        /// EAGL::SymbolEntry
        /// </summary>
        public const string EAGL_SYMBOL_ENTRY   = "EAGL::SymbolEntry";

        /// <summary>
        /// EAGL4::SymbolEntry
        /// </summary>
        public const string EAGL4_SYMBOL_ENTRY  = "EAGL4::SymbolEntry";

        /// <summary>
        /// EAGL4::SymbolEntry
        /// </summary>
        public const string NFS_UC_STRING       = "NeedForSpeedUndercover";

        // EAGL Symbol Entry addresses
        /// <summary>
        /// Hot Pursuit 2 EAGL::SymbolEntry address
        /// </summary>
        public const int EAGL_SYMBOL_ENTRY_HP2              = 0x002B7058;
        /// <summary>
        /// Underground 1 EAGL::SymbolEntry address
        /// </summary>
        public const int EAGL_SMYBOL_ENTRY_UG1              = 0x002A7760;

        /// <summary>
        /// Underground 2 EAGL::SymbolEntry address
        /// </summary>
        public const int EAGL_SMYBOL_ENTRY_UG2              = 0x003C0620;

        /// <summary>
        /// Most Wanted EAGL4::SymbolEntry address
        /// </summary>
        public const int EAGL4_SMYBOL_ENTRY_MW              = 0x00495708;

        /// <summary>
        /// Carbon EAGL4::SymbolEntry address
        /// </summary>
        public const int EAGL4_SMYBOL_ENTRY_CARBON          = 0x005C9FE8;

        /// <summary>
        /// ProStreet EAGL4::SymbolEntry address
        /// </summary>
        public const int EAGL4_SMYBOL_ENTRY_PROSTREET       = 0x00571350;

        /// <summary>
        /// World EAGL:SymbolEntry address
        /// </summary>
        public const int EAGL_SYMBOL_ENTRY_WORLD            = 0x007DF4B8;

        public const int UC_NFS_UNDERCOVER_STRING_ADDRESS   = 0x0080ECD8;

        /// <summary>
        /// Returns a <see cref="NFSGame"/> value by it's SymbolEntry address.
        /// </summary>
        /// <param name="exePath"></param>
        /// <returns></returns>
        public static NFSGame GetGameBySymbolEntry(string exePath)
        {
            if (!File.Exists(exePath))
                return NFSGame.None;

            byte[] exeByteArray = File.ReadAllBytes(exePath);

            // Start with EAGL
            List<int> positions = SearchBytePattern(Encoding.ASCII.GetBytes(EAGL_SYMBOL_ENTRY), exeByteArray);

            // TODO: use for loops instead because they are faster
            foreach (var item in positions)
            {
                switch(item)
                {
                    case EAGL_SYMBOL_ENTRY_HP2:
                        return NFSGame.HotPursuit2;

                    case EAGL_SMYBOL_ENTRY_UG1:
                        return NFSGame.Underground;

                    case EAGL_SMYBOL_ENTRY_UG2:
                        return NFSGame.Underground2;

                    case EAGL_SYMBOL_ENTRY_WORLD:
                        return NFSGame.World;
                }
            }

            // If the first byte pattern search didn't return anything, continue with EAGL4 symbols
            positions = SearchBytePattern(Encoding.ASCII.GetBytes(EAGL4_SYMBOL_ENTRY), exeByteArray);

            // TODO: use for loops instead because they are faster
            foreach (var item in positions)
            {
                switch (item)
                {
                    case EAGL4_SMYBOL_ENTRY_MW:
                        return NFSGame.MW;

                    case EAGL4_SMYBOL_ENTRY_CARBON:
                        return NFSGame.Carbon;

                    case EAGL4_SMYBOL_ENTRY_PROSTREET:
                        return NFSGame.ProStreet;
                }
            }

            // If the last two byte pattern search didn't return anything, continue with Undercover symbols
            positions = SearchBytePattern(Encoding.ASCII.GetBytes(NFS_UC_STRING), exeByteArray);
            foreach(var item in positions)
            {
                switch(item)
                {
                    case UC_NFS_UNDERCOVER_STRING_ADDRESS:
                        return NFSGame.Undercover;
                }
            }

            // If the byte patterns didn't work just return NFSGame.Undetermined.
            return NFSGame.Undetermined;
        }
    }
}
