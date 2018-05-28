using System.Collections.Generic;
using System.Linq;
using LibOpenNFS.Core;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    /// <summary>
    /// A material applied to an object.
    /// </summary>
    /// <remarks>Working on this as I research the format.</remarks>
    public class Material
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The hash of the texture that this material uses.
        /// </summary>
        public uint TextureHash { get; set; }
    }
    
    public class Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public int Color { get; set; }
    }
    
    public class Face
    {
        public short VertexA { get; set; }
        public short VertexB { get; set; }
        public short VertexC { get; set; }
    }
    
    public class SolidMesh
    {
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public List<Face> Faces { get; } = new List<Face>();
        public List<uint> UsedTextures { get; set; }
    }

    public class SolidObject
    {
        public bool IsSupported { get; set; }
        
        public string Name { get; set; }

        public uint Hash { get; set; }

        public CommonStructs.Point3D MinPoint { get; set; }

        public CommonStructs.Point3D MaxPoint { get; set; }

        public CommonStructs.Matrix Matrix { get; set; }

        public SolidMesh Mesh { get; set; }
        
        public List<uint> Textures { get; } = new List<uint>();
        public List<Material> Materials { get; } = new List<Material>();
    }

    public class SolidList : BaseModel
    {
        public SolidList(ChunkID id, long size, long position) : base(id, size, position)
        {
            DebugUtil.EnsureCondition(
                id == ChunkID.BCHUNK_SPEED_ESOLID_LIST_CHUNKS,
                () => $"Expected BCHUNK_SPEED_ESOLID_LIST_CHUNKS, got {id.ToString()}");
        }

        public List<uint> Hashes { get; } = new List<uint>();

        public List<SolidObject> Objects { get; } = new List<SolidObject>();

        public string Path { get; set; }

        public string SectionId { get; set; }

        public SolidObject LastObject
        {
            get
            {
                DebugUtil.EnsureCondition(Objects.Any(), () => "No objects in this list!");

                return Objects[Objects.Count - 1];
            }
        }
    }
}