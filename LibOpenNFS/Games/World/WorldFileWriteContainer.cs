using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World
{
    public class WorldFileWriteContainer : WriteContainer
    {
        public override void Write(BinaryReader reader, BinaryWriter writer, List<BaseModel> models)
        {
            var dataTable = new Dictionary<long, byte[]>(); // position, data

            foreach (var model in models)
            {
                reader.BaseStream.Position = model.GetPosition();
                dataTable[model.GetPosition()] = reader.ReadBytes((int) model.GetSize());
            }

            foreach (var model in models)
            {
                if (model.GetId() == 0)
                {
                    continue;
                }

                {
                    writer.Write((int) model.GetId());
                    writer.Write((int) model.GetSize());
                    writer.Write(dataTable[model.GetPosition()]);
                }
            }
        }

        private void WriteTexturePack(BinaryWriter writer, TexturePack texturePack)
        {
            // Calculate chunk sizes
            
            // Each info block = 168 bytes
            // Hash table = 8 * numTextures
            // Info = 0x7C

            var packInfoSize = 124; // 0x7C
            var hashTableSize = 8 * texturePack.Textures.Count;
            var textureInfoSize = 168 * texturePack.Textures.Count;
            var dxtHeaderSize = 32 * texturePack.Textures.Count;

            var tpkChunkSize = (packInfoSize + 8) + (hashTableSize + 8) + (textureInfoSize + 8) + (dxtHeaderSize + 8);

            var tpkChunkEndOffset = tpkChunkSize + 0x8 + 0x40;
            var tpkChunkAlignSize = (tpkChunkEndOffset - (tpkChunkEndOffset % 0x80)) + 0x100;
            tpkChunkAlignSize -= tpkChunkEndOffset + 8;

            var dataOffset = 0;

            var tpkDataChunkSize = (0x78 + dataOffset) + 8;
            
            
        }
    }
}