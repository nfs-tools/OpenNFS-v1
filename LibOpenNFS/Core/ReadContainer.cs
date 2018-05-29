using System.IO;

namespace LibOpenNFS.Core
{
    public abstract class ReadContainer<T>
    {
        protected ReadContainer(BinaryReader binaryReader, string file, long? containerSize)
        {
            BinaryReader = binaryReader;

            if (containerSize != null)
            {
                ContainerSize = (long) containerSize;
            }

            FileName = file;
        }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract T Get();

        // ReSharper disable once UnusedMemberInSuper.Global
        protected abstract void ReadChunks(long totalSize);
        
        protected BinaryReader BinaryReader;
        protected long ContainerSize;
        protected string FileName;
    }
}