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
        protected BaseModel(long id, long size, long position)
        {
            _id = id;
            _size = size;
            _position = position;
        }
        
        protected BaseModel(ChunkID id, long size, long position)
        {
            _id = (long) id;
            _size = size;
            _position = position;
        }

        public ChunkID GetId()
        {
            return (ChunkID) _id;
        }

        public long GetIdLong()
        {
            return _id;
        }

        public long GetSize()
        {
            return _size;
        }

        public long GetPosition()
        {
            return _position;
        }

        private readonly long _id;
        private readonly long _size;
        private readonly long _position;
    }
}
