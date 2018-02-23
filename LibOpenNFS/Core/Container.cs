using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibOpenNFS.Core
{
    public abstract class Container<T>
    {
        public Container(BinaryReader binaryReader, long? containerSize)
        {
            this.binaryReader = binaryReader;

            if (containerSize != null)
            {
                this.containerSize = (long) containerSize;
            }
        }

        public abstract T Get();
        protected abstract uint ReadChunks(long TotalSize);

        protected BinaryReader binaryReader;
        protected long containerSize = 0;
    }
}
