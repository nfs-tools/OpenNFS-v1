using System.Collections.Generic;
using System.Threading.Tasks;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;

namespace WpfUi.Services
{
    /// <summary>
    /// A service for loading resources from a file.
    /// </summary>
    /// <seealso cref="LibOpenNFS.Games.MW.MWFileContainer"/>
    /// <seealso cref="LibOpenNFS.Games.UG2.UG2FileContainer"/>
    /// <seealso cref="LibOpenNFS.Games.Undercover.UCFileContainer"/>
    /// <seealso cref="LibOpenNFS.Games.World.WorldFileContainer"/>
    public interface IResourceService
    {
        /// <summary>
        /// Load a file and persist its resources in the resource cache.
        /// </summary>
        /// <param name="file">The path of the file to load data from.</param>
        /// <param name="game">The game ID.</param>
        /// <returns></returns>
        Task<List<BaseModel>> Load(string file, NFSGame game);
    }
}
