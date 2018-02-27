using System.IO;

namespace LibOpenNFS.Core
{
    public abstract class Container<T>
    {
        protected Container(BinaryReader binaryReader, long? containerSize)
        {
            BinaryReader = binaryReader;

            if (containerSize != null)
            {
                ContainerSize = (long) containerSize;
            }
        }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract T Get();
        
        // ReSharper disable once UnusedMemberInSuper.Global
        protected abstract void ReadChunks(long totalSize);

        protected BinaryReader BinaryReader;
        protected long ContainerSize;
    }
}