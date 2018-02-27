using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OpenNFSUI.Explorer
{
    public class ExplorerItem
    {
        /// <summary>
        /// Items inside this <see cref="ExplorerItem"/>.
        /// <para>Can be null.</para>
        /// </summary>
        public List<ExplorerItem> Items { get; private set; }

        /// <summary>
        /// The <see cref="FileExtensionsData"/> class of this <see cref="ExplorerItem"/>.
        /// </summary>
        public FileExtensionsData FileData { get; private set; }

        /// <summary>
        /// The full path of this <see cref="ExplorerItem"/>.
        /// <para>Example: C:\Directory\File.txt</para>
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// The name of this <see cref="ExplorerItem"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The image index of this <see cref="ExplorerItem"/>.
        /// </summary>
        public int ImageIndex { get; private set; }

        /// <summary>
        /// Returns a value indicates whether this <see cref="ExplorerItem"/> a file or not.
        /// </summary>
        public bool IsFile { get; private set; }

        public ExplorerItem(string fullPath)
        {
            FullPath = fullPath;
            FileAttributes attr = File.GetAttributes(fullPath);
            Items = new List<ExplorerItem>();
            IsFile = false;
            if (attr.HasFlag(FileAttributes.Directory) && !attr.HasFlag(FileAttributes.Hidden))
            {
                DirectoryInfo di = new DirectoryInfo(fullPath);
                Name = di.Name;

                DirectoryInfo[] directories = di.GetDirectories();
                FileInfo[] files = di.GetFiles();

                for (int i = 0; i < directories.Length; i++)
                {
                    Items.Add(new ExplorerItem(directories[i].FullName));
                }

                for (int i = 0; i < files.Length; i++)
                {
                    Items.Add(new ExplorerItem(files[i].FullName));
                }
            }
            else
            {
                FileInfo fi = new FileInfo(fullPath);
                Name = fi.Name;
                IsFile = true;
                //FileData = new FileExtensionsData(fi.Extension);
            }
        }
    }
}
