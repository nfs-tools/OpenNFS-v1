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
                dataTable[model.GetPosition()] = reader.ReadBytes((int)model.GetSize());
            }

            foreach (var model in models)
            {
                if (model is TexturePack tpk)
                {
                    WriteTexturePack(writer, tpk);
                }
                else
                {
                    writer.Write((int)model.GetId());
                    writer.Write((int)model.GetSize());
                    writer.Write(dataTable[model.GetPosition()]);
                }
            }
        }

        private void WriteTexturePack(BinaryWriter writer, TexturePack texturePack)
        {
            // Compute sizes
            var dataSize = (uint) (texturePack.Textures.Sum(tex => tex.DataSize) + 0x78);
            var dataChunkSize = 0u;

            dataChunkSize += 8 + 0x04; // Child 1
            dataChunkSize += 8 + 0x50; // Child 2
            dataChunkSize += 8 + 0x0C; // null chunk
            dataChunkSize += 8 + dataSize; // data chunk

            var child5Size = 32 * texturePack.Textures.Count;
            var child4Size = 0;
            var child2Size = 0x8 * texturePack.Textures.Count;

            foreach (var texture in texturePack.Textures)
            {
                child4Size += 144;
                child4Size += 1;
                child4Size += texture.Name.Length;

                var namePad = 4 - child4Size % 4;
                child4Size += namePad;
            }

            var tpkChunkSize = (uint) (0x7c + 8 + child2Size + 8 + child4Size + 8 + child5Size + 8);

            var capsuleSize = tpkChunkSize + 0x8 + 0x38 + 0x8 + dataChunkSize + 0x8 + 0x38;

            writer.Write(unchecked((int) 0xB3300000));
            writer.Write(capsuleSize);

            // write first null
            writer.Write(0x00000000);
            writer.Write(0x30);
            writer.Write(new byte[0x30]);

            // write TPK chunk
            writer.Write(unchecked((int) 0xB3310000));
            writer.Write(tpkChunkSize);

            // write child 1
            writer.Write(0x33310001);
            writer.Write(0x7C);

            writer.Write(0x9); // whatever this is

            var tpkTypeName = new char[0x1C];
            texturePack.Name.CopyTo(0, tpkTypeName, 0, texturePack.Name.Length);

            var tpkPath = new char[0x40];
            texturePack.Path.CopyTo(0, tpkPath, 0, texturePack.Path.Length);

            writer.Write(tpkTypeName);
            writer.Write(tpkPath);
            writer.Write(texturePack.Hash);
            writer.Write(new byte[24]);

            // write hashes
            writer.Write(0x33310002);
            writer.Write(child2Size);

            foreach (var texture in texturePack.Textures)
            {
                writer.Write(texture.TextureHash);
                writer.Write(0x00000000);
            }

            // write texture entries
            writer.Write(0x33310004);
            writer.Write(child4Size);

            var dataOffset = 0;
            var child4Bytes = 0;

            foreach (var texture in texturePack.Textures)
            {
                writer.Write(new byte[12]); // null
                writer.Write(texture.TextureHash);
                writer.Write(texture.TypeHash);
                writer.Write((int) texture.Properties["Unknown1"]);
                writer.Write(texture.Data.Length);
                writer.Write((int)texture.Properties["Unknown2"]);
                writer.Write(texture.Width);
                writer.Write(texture.Height);
                writer.Write(texture.MipMap);
                writer.Write((uint)texture.Properties["Unknown3"]);
                writer.Write((uint)texture.Properties["Unknown4"]);
                writer.Write((byte[]) texture.Properties["Unknown5"]);
                writer.Write((uint)texture.Properties["Unknown6"]);
                writer.Write(dataOffset);
                writer.Write((byte[])texture.Properties["Unknown7"]);

                child4Bytes += 144;
                child4Bytes += 1 + texture.Name.Length;
                var namePad = 4 - child4Bytes % 4;
                child4Bytes += namePad;

                writer.Write((byte) (texture.Name.Length + namePad));
                writer.Write(Encoding.GetEncoding(1252).GetBytes(texture.Name));
                writer.Write(new byte[namePad]);

                dataOffset += texture.Data.Length;
            }

            // write DDS headers
            writer.Write(0x33310005);
            writer.Write(child5Size);

            foreach (var texture in texturePack.Textures)
            {
                writer.Write(new byte[12]);
                writer.Write(texture.CompressionType);
                writer.Write(new byte[0x10]);
            }

            // --------------------
            writer.Write(0x00000000);
            writer.Write(0x38);
            writer.Write(new byte[0x38]);

            // write data 
            writer.Write(0xB3320000);
            writer.Write(dataChunkSize);

            // write odd-child 1
            writer.Write(0x33330001);
            writer.Write(0x4);
            writer.Write(new byte[4]);

            // write actual child 1
            writer.Write(0x33320001);
            writer.Write(0x50);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(0x2);
            writer.Write(texturePack.Hash);
            writer.Write(0x00000000);
            writer.Write(0x00000000);
            writer.Write(new byte[56]);

            // write null
            writer.Write(0x00000000);
            writer.Write(0x0C);
            writer.Write(new byte[0x0C]);
            
            // write data
            writer.Write(0x33320002);
            writer.Write(dataSize);
            
            for (var i = 0; i <= 0x77; i++)
                writer.Write((byte) 0x11);
            foreach (var texture in texturePack.Textures)
            {
                writer.Write(texture.Data);
            }

            //writer.Write();
        }
    }
}