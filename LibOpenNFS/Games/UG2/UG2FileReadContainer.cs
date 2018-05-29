using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.Utils;
using LibOpenNFS.DataModels;
using LibOpenNFS.Games.UG2.Frontend.Readers;
using LibOpenNFS.Games.UG2.InGame.Readers;
using LibOpenNFS.Games.UG2.TrackStreamer.Readers;

namespace LibOpenNFS.Games.UG2
{
    public class UG2FileReadContainer : ReadContainer<List<BaseModel>>
    {
        public UG2FileReadContainer(BinaryReader binaryReader, string fileName,
            ContainerReadOptions options)
            : base(binaryReader, fileName, 0)
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
            if (BinaryReader.BaseStream.Length == 0)
                return;
            
            var curPos = BinaryReader.BaseStream.Position;

            if (BinaryReader.ReadChar() == 'J'
                && BinaryReader.ReadChar() == 'D'
                && BinaryReader.ReadChar() == 'L'
                && BinaryReader.ReadChar() == 'Z')
            {
#if DEBUG
                Console.WriteLine("JDLZ compressed!");
#endif
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
                        _dataModels.Add(new SectionListReadContainer(BinaryReader, FileName, chunkSize).Get());
                        break;
                    case (long) ChunkID.BCHUNK_SPEED_ELIGHT_CHUNKS:
                    {
                        _dataModels.Add(new LightListReadContainer(BinaryReader, FileName, chunkSize).Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS:
                    {
                        _dataModels.Add(new SolidListReadContainer(BinaryReader, FileName, chunkSize).Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS:
                    {
                        _dataModels.Add(new TPKReadContainer(BinaryReader, FileName, chunkSize, false).Get());
                        break;
                    }
                    default:
                        _dataModels.Add(new NullModel(normalizedId, chunkSize, BinaryReader.BaseStream.Position));

                        if (DebugUtil.IsContainerChunk(chunkId))
                        {
                            ReadChunks(chunkSize);
                        }

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