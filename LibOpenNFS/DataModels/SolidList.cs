using System.Collections.Generic;
using LibOpenNFS.Core;
using LibOpenNFS.Utils;

namespace LibOpenNFS.DataModels
{
    public class Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public uint Color { get; set; }
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
        public string Name { get; set; }

        public uint Hash { get; set; }

        public CommonStructs.Point3D MinPoint { get; set; }

        public CommonStructs.Point3D MaxPoint { get; set; }

        public CommonStructs.Matrix Matrix { get; set; }

        public SolidMesh Mesh { get; set; }
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
    }
}