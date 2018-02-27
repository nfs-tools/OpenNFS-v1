using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public abstract class BaseModel
    {
        protected BaseModel(ChunkID id, long size)
        {
            this._id = id;
            this._size = size;
        }

        public ChunkID GetId()
        {
            return _id;
        }

        public long GetIdLong()
        {
            return (long) _id;
        }

        public long GetSize()
        {
            return _size;
        }

        private readonly ChunkID _id;
        private readonly long _size;
    }
}
