using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Frontend.Readers
{
    public class CompressedFNGReadContainer : ReadContainer<FNGFile>
    {
        public CompressedFNGReadContainer(BinaryReader binaryReader, string fileName, long? containerSize)
            : base(binaryReader, fileName, containerSize)
        {
        }

        public override FNGFile Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            BinaryReader.BaseStream.Seek(4, SeekOrigin.Current);

            var test = BinaryReader.ReadChars(4);
            BinaryReader.BaseStream.Seek(-4, SeekOrigin.Current);

            if (test[0] != 'J' || test[1] != 'D' || test[2] != 'L' || test[3] != 'Z')
            {
                return new FNGFile(ChunkID.BCHUNK_FENG_PACKAGE, ContainerSize, BinaryReader.BaseStream.Position)
                {
                    Name = "__UNSUPPORTED__",
                    HasData = false
                };
            }

            var data = new byte[ContainerSize - 4];

            BinaryReader.BaseStream.Read(data, 0, data.Length);

            var decompressed = JDLZ.Decompress(data);
            const string newName = "_tmpCompressedFENG.dejdlz";

            using (var stream = new FileStream(newName, FileMode.Create))
            {
                stream.Write(decompressed, 0, decompressed.Length);
            }

            var readStream = new FileStream(newName, FileMode.Open);
            var fngContainer = new FNGReadContainer(new BinaryReader(readStream), FileName, decompressed.Length);
            var result = fngContainer.Get();
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