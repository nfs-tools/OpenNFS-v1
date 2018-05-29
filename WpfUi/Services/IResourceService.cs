using System.Collections.Generic;
using System.Threading.Tasks;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;

namespace WpfUi.Services
{
    /// <summary>
    /// A service for loading resources from a file.
    /// </summary>
    /// <seealso cref="LibOpenNFS.Games.MW.MWFileReadContainer"/>
    /// <seealso cref="LibOpenNFS.Games.UG2.UG2FileReadContainer"/>
    /// <seealso cref="LibOpenNFS.Games.Undercover.UCFileReadContainer"/>
    /// <seealso cref="LibOpenNFS.Games.World.WorldFileReadContainer"/>
    public interface IResourceService
    {
        /// <summary>
        /// Load a file and persist its resources in the resource cache.
        /// </summary>
        /// <param name="file">The path of the file to load data from.</param>
        /// <param name="group">The resource group to load into.</param>
        /// <param name="game">The game ID.</param>
        /// <returns></returns>
        Task<List<BaseModel>> Load(string file, string group, NFSGame game);

        #region Texture Pack Functions
        /// <summary>
        /// Find a texture pack by the given name, in the given group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        TexturePack FindPack(string name, string group);

        /// <summary>
        /// Find a texture pack by the given hash, in the given group.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        TexturePack FindPack(uint hash, string group);

        /// <summary>
        /// Find a texture by the given name, in the given group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Texture FindTexture(string name, string group);

        /// <summary>
        /// Find a texture by the given hash, in the given group.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        Texture FindTexture(uint hash, string group);

        /// <summary>
        /// Put a texture into the texture cache for the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="hash"></param>
        /// <param name="texture"></param>
        void PutTexture(string group, uint hash, Texture texture);
        #endregion

        #region Solid List Functions
        /// <summary>
        /// Find a solid list by the given name, in the given group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        SolidList FindSolidList(string name, string group);
        #endregion

        /// <summary>
        /// Purge all of the resource caches.
        /// </summary>
        void PurgeResources();
    }
}
