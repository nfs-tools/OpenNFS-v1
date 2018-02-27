using System;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Frontend
{
    public class MWFNGContainer : Container<FNGFile>
    {
        public MWFNGContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override FNGFile Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _fngFile = new FNGFile(ChunkID.BCHUNK_FENG_PACKAGE, ContainerSize);
            ReadChunks(ContainerSize);

            return _fngFile;
        }

        protected override void ReadChunks(long totalSize)
        {
            var runTo = BinaryReader.BaseStream.Position + totalSize;

            BinaryReader.BaseStream.Seek(40, SeekOrigin.Current);

            _fngFile.Name = BinaryUtil.ReadNullTerminatedString(BinaryReader);

            Console.WriteLine($"FENG Package: {_fngFile.Name}");
            Console.WriteLine($"FENG Path: {BinaryUtil.ReadNullTerminatedString(BinaryReader)}");

            BinaryUtil.PrintPosition(BinaryReader, GetType());
        }

        private FNGFile _fngFile;
    }
}