using System.Collections.Generic;

namespace LibOpenNFS.DataModels
{
    public class Texture
    {
        public int TextureHash
        {
            get
            {
                return _textureHash;
            }
            set
            {
                _textureHash = value;
            }
        }
        public int TypeHash
        {
            get
            {
                return _typeHash;
            }
            set
            {
                _typeHash = value;
            }
        }
        public uint DataOffset
        {
            get
            {
                return _dataOffset;
            }
            set
            {
                _dataOffset = value;
            }
        }
        public uint DataSize
        {
            get
            {
                return _dataSize;
            }
            set
            {
                _dataSize = value;
            }
        }
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        public int MipMap
        {
            get
            {
                return _mipMap;
            }
            set
            {
                _mipMap = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public uint CompressionType
        {
            get
            {
                return _compressionType;
            }
            set
            {
                _compressionType = value;
            }
        }
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        private int _textureHash;
        private int _typeHash;
        private uint _dataOffset;
        private uint _dataSize;
        private int _width;
        private int _height;
        private int _mipMap;
        private string _name;
        private uint _compressionType;
        private byte[] _data;
    }

    public class TexturePack : BaseModel
    {
        public TexturePack(long id, long size) : base(id, size)
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        public int Hash
        {
            get
            {
                return _hash;
            }
            set
            {
                _hash = value;
            }
        }

        public List<Texture> Textures
        {
            get
            {
                return _textures;
            }
        }

        public List<uint> Hashes
        {
            get
            {
                return _hashes;
            }
        }

        private string _name;
        private string _path;
        private int _hash;
        private List<Texture> _textures = new List<Texture>();
        private List<uint> _hashes = new List<uint>();
    }
}
