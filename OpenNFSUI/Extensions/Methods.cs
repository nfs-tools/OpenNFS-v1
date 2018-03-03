using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using OpenNFSUI.Database;
using LibOpenNFS;
using LibOpenNFS.Core;

namespace OpenNFSUI.Extensions
{
    public static class Methods
    {

        public static LibOpenNFS.Core.NFSGame GetNFSGameFromPath(string path)
        {
            Game game = null;
            
            for(int i = 0; i < SupportedDatabase.SupportedGames.Length; i++)
            {
                string fileName = SupportedDatabase.SupportedGames[i].ExectuableFileName;
                string final = Path.Combine(path, fileName);

                if(File.Exists(final))
                {
                    return GameSymbolData.GetGameBySymbolEntry(final);
                }
                
            }

            return NFSGame.Undetermined;
        }

        public static bool IsPathValid(string path)
        {
            return !String.IsNullOrEmpty(path) || Directory.Exists(path) || !String.IsNullOrWhiteSpace(path);
        }

        public static string GetPathByGame(Game game)
        {
            switch (game.ID)
            {
                case Game.NFS_HP2_ID:
                    return Program.MainConfig.HP2DirPath;

                case Game.NFS_UG_ID:
                    return Program.MainConfig.UGDirPath;

                case Game.NFS_UG2_ID:
                    return Program.MainConfig.UG2DirPath;

                case Game.NFS_MW_ID:
                    return Program.MainConfig.MWDirPath;

                case Game.NFS_CARBON_ID:
                    return Program.MainConfig.CarbonDirPath;

                case Game.NFS_PROSTREET_ID:
                    return Program.MainConfig.ProStreetDirPath;

                case Game.NFS_UNDERCOVER_ID:
                    return Program.MainConfig.UndercoverDirPath;

                case Game.NFS_WORLD_ID:
                    return Program.MainConfig.WorldDirPath;

                default:
                    return null;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
            foreach (T item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }

        public static T[] CopySlice<T>(this T[] source, int index, int length, bool padToLength = false)
        {
            int n = length;
            T[] slice = null;

            if (source.Length < index + length)
            {
                n = source.Length - index;
                if (padToLength)
                {
                    slice = new T[length];
                }
            }

            if (slice == null) slice = new T[n];
            Array.Copy(source, index, slice, 0, n);
            return slice;
        }

        public static IEnumerable<T[]> Slices<T>(this T[] source, int count, bool padToLength = false)
        {
            for (var i = 0; i < source.Length; i += count)
                yield return source.CopySlice(i, count, padToLength);
        }

        // Parse a file size.
        private static readonly string[] SizeSuffixes =
            { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string ParseFileSize(double len)
        {
            int order = 0;
            while (len >= 1024 && order < SizeSuffixes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, SizeSuffixes[order]);
        }

        public static long DirectorySize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirectorySize(di);
            }
            return size;
        }
    }
}
