using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibOpenNFS.Core;
using LibOpenNFS.Utils;
using LibOpenNFS.DataModels;

namespace LibOpenNFS.Games.MW
{
    public class MWFileContainer : Container<List<BaseModel>>
    {
        public MWFileContainer(BinaryReader binaryReader, long? containerSize, ContainerReadOptions options) 
            : base(binaryReader, containerSize)
        {
            if (containerSize == null)
            {
                this.containerSize = binaryReader.BaseStream.Length;
            } else
            {
                this.containerSize = (long) containerSize;
            }

            if (options == null)
            {
                _options = new ContainerReadOptions
                {
                    Start = -1,
                    End = -1
                };
            } else
            {
                if (options.Start > options.End)
                {
                    throw new Exception("Invalid start and end");
                }

                _options = options;
                binaryReader.BaseStream.Seek(_options.Start, SeekOrigin.Begin);
                this.containerSize = _options.End - _options.Start;
            }
        }

        public override List<BaseModel> Get()
        {
            ReadChunks(containerSize);

            return _dataModels;
        }

        protected override uint ReadChunks(long TotalSize)
        {
            var runTo = binaryReader.BaseStream.Position + TotalSize;

            for (int i = 0; 
                i < 0xFFFF && binaryReader.BaseStream.Position < runTo;
                i++
            ) {
                int chunkId = binaryReader.ReadInt32();
                uint chunkSize = binaryReader.ReadUInt32();

                Console.WriteLine("chunk: 0x{0} [{1} bytes]", chunkId.ToString("X8"), chunkSize);

                //long idToLong = (long)chunkId;
                //Console.WriteLine((idToLong << 32).ToString("X16"));
                //Console.WriteLine((((long)OtherChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS) << 32).ToString("X16"));

                long normalizedId = chunkId & 0xffffffff;
                //Console.WriteLine("<< 32 = {0}", longChunkId.ToString("X16"));
                //Console.WriteLine("test << 32 = {0}", ((long)OtherChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS).ToString("X16"));

                if (Enum.IsDefined(typeof(ChunkID), normalizedId))
                {
                    Console.WriteLine("{0}", ((ChunkID)normalizedId).ToString());
                }

                switch (normalizedId)
                {
                    case (long)ChunkID.BCHUNK_CARINFO_ARRAY:
                        MWCarListContainer carListContainer = new MWCarListContainer(binaryReader, chunkSize);
                        _dataModels.Add(carListContainer.Get());
                        break;
                    case (long)ChunkID.BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS:
                        MWTPKContainer tpkContainer = new MWTPKContainer(binaryReader, chunkSize);
                        _dataModels.Add(tpkContainer.Get());

                        break;
                    default:
                        binaryReader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                        break;
                }

                //if (chunkId == ChunkID.BCHUNK_TRACKSTREAMER_SECTIONS)
                //{
                //    Console.WriteLine("Sections!");
                //}
            }

            return 0;
        }

        private List<BaseModel> _dataModels = new List<BaseModel>();
        private ContainerReadOptions _options;
    }
}
