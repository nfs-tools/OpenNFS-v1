using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Games.World;

namespace WpfUi.Services
{
    public class ResourceService : IResourceService
    {
        public async Task<List<BaseModel>> Load(string file, NFSGame game)
        {
            return await Task.Run(() =>
            {
                List<BaseModel> results;

                switch (game)
                {
                    case NFSGame.World:
                    {
                        using (var reader = File.OpenRead(file))
                        {
                            results = new WorldFileContainer(new BinaryReader(reader), file, null).Get();
                        }

                        break;
                    }
                    default:
                        throw new InvalidOperationException($"Unsupported game: {game}");
                }

                return results;
            });
        }
    }
}
