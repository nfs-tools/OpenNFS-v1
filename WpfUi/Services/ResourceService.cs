using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Games.MW;
using LibOpenNFS.Games.World;

namespace WpfUi.Services
{
    public class ResourceService : IResourceService
    {
        /// <summary>
        /// The texture cache.
        /// Structure: group key -> dictionary[textureHash,texture]
        /// </summary>
        private readonly Dictionary<string, Dictionary<uint, Texture>> _textures 
            = new Dictionary<string, Dictionary<uint, Texture>>();
        /// <summary>
        /// The texture pack cache.
        /// Structure: group key -> dictionary[packHash,texturePack]
        /// </summary>
        private readonly Dictionary<string, Dictionary<uint, TexturePack>> _texturePacks
            = new Dictionary<string, Dictionary<uint, TexturePack>>();
        /// <summary>
        /// The solid list cache.
        /// Structure: group key -> dictionary[listPath,solidList]
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, SolidList>> _solidLists
            = new Dictionary<string, Dictionary<string, SolidList>>();

        public async Task<List<BaseModel>> Load(string file, string group, NFSGame game)
        {
            return await Task.Run(() =>
            {
                List<BaseModel> results;

                using (var reader = File.OpenRead(file))
                {
                    switch (game)
                    {
                        case NFSGame.MW:
                        {
                            results = new MWFileReadContainer(new BinaryReader(reader),
                                file,
                                null).Get();

                            break;
                        }
                        case NFSGame.World:
                        {
                            results = new WorldFileReadContainer(new BinaryReader(reader),
                                file,
                                null).Get();

                            break;
                        }
                        default:
                            throw new InvalidOperationException($"Unsupported game: {game}");
                    }
                }

                ProcessModels(results, group);

                return results;
            });
        }

        public TexturePack FindPack(string name, string group)
        {
            if (!_texturePacks.ContainsKey(group))
            {
                throw new ArgumentException($"Unknown group: {group}", nameof(group));
            }

            if (!_texturePacks[group].Any(tpk =>
                string.Equals(tpk.Value.Name, name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException($"Unknown pack: {name}", nameof(name));
            }

            return _texturePacks[group].First(tpk =>
                    string.Equals(
                        tpk.Value.Name, name, StringComparison.InvariantCultureIgnoreCase)
                    ).Value;
        }

        public TexturePack FindPack(uint hash, string group)
        {
            if (!_texturePacks.ContainsKey(group))
            {
                throw new ArgumentException($"Unknown group: {group}", nameof(group));
            }

            if (!_texturePacks[group].ContainsKey(hash))
            {
                throw new ArgumentException($"Unknown hash: 0x{hash:X8}", nameof(hash));
            }

            return _texturePacks[group][hash];
        }

        public Texture FindTexture(string name, string group)
        {
            if (!_textures.ContainsKey(group))
            {
                throw new ArgumentException($"Unknown group: {group}", nameof(group));
            }

            if (!_textures[group].Any(tex =>
                string.Equals(tex.Value.Name, name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException($"Unknown pack: {name}", nameof(name));
            }

            return _textures[group].First(tex =>
                string.Equals(
                    tex.Value.Name, name, StringComparison.InvariantCultureIgnoreCase)
            ).Value;
        }

        public Texture FindTexture(uint hash, string group)
        {
            if (!_textures.ContainsKey(group))
            {
                throw new ArgumentException($"Unknown group: {group}", nameof(group));
            }

            if (!_textures[group].ContainsKey(hash))
            {
                throw new ArgumentException($"Unknown hash: 0x{hash:X8}", nameof(hash));
            }

            return _textures[group][hash];
        }

        public Texture FindTexture(string pack, string name, string group)
        {
            var tPack = FindPack(pack, group);

            if (!tPack.Textures.Any(t => string.Equals(
                t.Name, name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException($"Unknown name: {name}", nameof(name));
            }

            return tPack.Textures.Find(t => string.Equals(
                t.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        public Texture FindTexture(string pack, uint hash, string group)
        {
            var tPack = FindPack(pack, group);

            if (!tPack.Hashes.Contains(hash))
            {
                throw new ArgumentException($"Unknown hash: 0x{hash:X8}", nameof(hash));
            }

            return tPack.Textures.Find(t => t.TextureHash == hash);
        }

        public void PutTexture(string group, uint hash, Texture texture)
        {
            throw new NotImplementedException();
        }

        public SolidList FindSolidList(string name, string @group)
        {
            throw new NotImplementedException();
        }

        public void PurgeResources()
        {
            _texturePacks.Clear();
            _textures.Clear();
        }

        private void ProcessModels(IEnumerable<BaseModel> models, string group)
        {
            // Initial setup
            if (!_texturePacks.ContainsKey(group))
            {
                _texturePacks.Add(group, new Dictionary<uint, TexturePack>());
            }

            if (!_textures.ContainsKey(group))
            {
                _textures.Add(group, new Dictionary<uint, Texture>());
            }

            // Group the models by their types
            foreach (var modelGroup in models.GroupBy(m => m.GetType()))
            {
                // Process texture packs
                if (modelGroup.Key == typeof(TexturePack))
                {
                    foreach (var pack in modelGroup.Cast<TexturePack>())
                    {
                        _texturePacks[group].Add(pack.Hash, pack);

                        foreach (var texture in pack.Textures)
                        {
                            _textures[group].Add(texture.TextureHash, texture);
                        }
                    }
                }
            }
        }
    }
}
