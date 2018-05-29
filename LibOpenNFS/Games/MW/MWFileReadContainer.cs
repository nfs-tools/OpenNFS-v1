using System;
using System.Collections.Generic;
using System.IO;
using LibOpenNFS.Core;
using LibOpenNFS.Utils;
using LibOpenNFS.DataModels;
using LibOpenNFS.Games.MW.Frontend.Readers;
using LibOpenNFS.Games.MW.TrackStreamer.Readers;

namespace LibOpenNFS.Games.MW
{
    public class MWFileReadContainer : ReadContainer<List<BaseModel>>
    {
        public MWFileReadContainer(BinaryReader binaryReader, string fileName,
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

#if DEBUG
                BinaryUtil.PrintID(BinaryReader, chunkId, normalizedId, chunkSize, GetType());
#endif

                switch (normalizedId)
                {
                    case (long) ChunkID.BCHUNK_CARINFO_ARRAY:
                        var carListContainer = new MWCarListReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(carListContainer.Get());
                        break;
                    case (long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS:
                    {
                        var tpkContainer = new TPKReadContainer(BinaryReader, _fileName, chunkSize, false);
                        _dataModels.Add(tpkContainer.Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM:
                    {
                        var tpkContainer = new AnimatedTPKReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(tpkContainer.Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_COMPRESSED:
                    {
                        var tpkContainer = new CompressedTPKReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(tpkContainer.Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_LANGUAGE:
                        var languageContainer = new LanguageReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(languageContainer.Get());
                        break;
                    case (long) ChunkID.BCHUNK_TRACKINFO:
                        var trackListContainer = new TrackListReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(trackListContainer.Get());
                        break;
                    case (long) ChunkID.BCHUNK_TRACKSTREAMER_SECTIONS:
                        var sectionsContainer = new SectionListReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(sectionsContainer.Get());
                        break;
                    case (long) ChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS:
                        var solidListContainer = new SolidListReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(solidListContainer.Get());
                        break;
                    case (long) ChunkID.BCHUNK_FENG_PACKAGE:
                    {
                        var fngContainer = new FNGReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(fngContainer.Get());
                        break;
                    }
                    case (long) ChunkID.BCHUNK_FENG_PACKAGE_COMPRESSED:
                    {
                        var fngContainer = new CompressedFNGReadContainer(BinaryReader, _fileName, chunkSize);
                        _dataModels.Add(fngContainer.Get());
                        break;
                    }
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