using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Games.World.Frontend.Readers;
using LibOpenNFS.Games.World.TrackStreamer.Readers;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.World
{
    public class WorldFileContainer : Container<List<BaseModel>>
    {
        public WorldFileContainer(BinaryReader binaryReader, string fileName,
            ContainerReadOptions options)
            : base(binaryReader, 0)
        {
            _fileName = fileName;

            ContainerSize = binaryReader.BaseStream.Length;

            if (options == null) return;

            if (options.Start > options.End)
            {
                throw new Exception("Invalid start and end");
            }

            if (ContainerSize == 0)
            {
                throw new Exception("Cannot read from an empty container!");
            }

            binaryReader.BaseStream.Seek(options.Start, SeekOrigin.Begin);
            ContainerSize = options.End - options.Start;
        }

        public override List<BaseModel> Get()
        {
            ReadChunks(ContainerSize);

            return _dataModels;
        }

        protected override void ReadChunks(long totalSize)
        {
            var curPos = BinaryReader.BaseStream.Position;

            if (BinaryReader.ReadChar() == 'J'
                && BinaryReader.ReadChar() == 'D'
                && BinaryReader.ReadChar() == 'L'
                && BinaryReader.ReadChar() == 'Z')
            {
                Console.WriteLine("JDLZ compressed!");

                BinaryReader.BaseStream.Seek(curPos, SeekOrigin.Begin);

                var data = new byte[BinaryReader.BaseStream.Length];

                BinaryReader.BaseStream.Read(data, 0, data.Length);

                var decompressed = JDLZ.Decompress(data);
                var newName = _fileName + ".dejdlz";

                var stream = new FileStream(newName, FileMode.CreateNew);
                stream.Write(decompressed, 0, decompressed.Length);
                stream.Close();
                BinaryReader = new BinaryReader(new FileStream(newName, FileMode.Open));
                File.Delete(newName);
            }
            else
            {
                BinaryReader.BaseStream.Seek(curPos, SeekOrigin.Begin);
            }

            var runTo = BinaryReader.BaseStream.Position + totalSize;

            for (var i = 0;
                i < 0xFFFF && BinaryReader.BaseStream.Position < runTo;
                i++
            )
            {
                var chunkId = BinaryReader.ReadUInt32();
                var chunkSize = BinaryReader.ReadUInt32();
                var chunkRunTo = BinaryReader.BaseStream.Position + chunkSize;

                var normalizedId = (int) chunkId & 0xffffffff;

                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType());

                switch (normalizedId)
                {
                    case (long) ChunkID.BCHUNK_TRACKSTREAMER_SECTIONS:
                        _dataModels.Add(new SectionListContainer(BinaryReader, chunkSize).Get());
                        break;
                    case (long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS:
                        _dataModels.Add(new TPKContainer(BinaryReader, chunkSize).Get());
                        break;
                    default:
                        _dataModels.Add(new NullModel(normalizedId, chunkSize, BinaryReader.BaseStream.Position));
                        
                        break;
                }

                BinaryUtil.ValidatePosition(BinaryReader, chunkRunTo, GetType());
                BinaryReader.BaseStream.Seek(chunkRunTo - BinaryReader.BaseStream.Position, SeekOrigin.Current);
            }
        }

        private readonly List<BaseModel> _dataModels = new List<BaseModel>();
        private readonly string _fileName;
    }
}