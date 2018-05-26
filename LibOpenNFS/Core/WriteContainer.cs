using System.Collections.Generic;
using System.IO;
using LibOpenNFS.DataModels;

namespace LibOpenNFS.Core
{
    public abstract class WriteContainer
    {
        /// <summary>
        /// Write a list of data models to a file.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> instance.</param>
        /// <param name="writer">The <see cref="BinaryWriter"/> instance.</param>
        /// <param name="models">The models to write.</param>
        public abstract void Write(BinaryReader reader, BinaryWriter writer, List<BaseModel> models);
    }
}