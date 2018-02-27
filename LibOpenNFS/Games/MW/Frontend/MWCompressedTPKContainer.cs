using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Frontend
{
    public class MWCompressedTPKContainer : Container<TexturePack>
    {
        public MWCompressedTPKContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override TexturePack Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }
            
            var data = new byte[ContainerSize];

            BinaryReader.BaseStream.Read(data, 0, data.Length);

            var decompressed = JDLZ.Decompress(data);
            const string newName = "_tmpCompressedTpk.dejdlz";

            using (var stream = new FileStream(newName, FileMode.Create))
            {
                stream.Write(decompressed, 0, decompressed.Length);
            }

            var readStream = new FileStream(newName, FileMode.Open);
            readStream.Seek(8, SeekOrigin.Current);
            var tpkContainer = new MWTPKContainer(new BinaryReader(readStream), decompressed.Length, true);
            var result = tpkContainer.Get();
            readStream.Close();
                        
            File.Delete(newName);

            return result;
        }

        protected override void ReadChunks(long totalSize)
        {
            throw new NotImplementedException();
        }
    }
}