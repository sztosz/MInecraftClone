using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<Vector3> _vertices = new List<Vector3>();

    private readonly bool[,,] _voxelMap = new bool[VoxelData.ChunkHeight, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private int _vertexIndex;

    private void Start()
    {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void AddVoxelDataToChunk(Vector3 position)
    {
        for (var i = 0; i < 6; i++)
            if (!CheckVoxel(position + VoxelData.FaceChecks[i]))
                for (var j = 0; j < 6; j++)
                {
                    _vertices.Add(VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, j]] + position);
                    _triangles.Add(_vertexIndex);
                    _uvs.Add(VoxelData.VoxelUvs[j]);
                    _vertexIndex++;
                }
    }

    private void PopulateVoxelMap()
    {
        for (var y = 0; y < VoxelData.ChunkHeight; y++)
        for (var x = 0; x < VoxelData.ChunkWidth; x++)
        for (var z = 0; z < VoxelData.ChunkWidth; z++)
            _voxelMap[x, y, z] = true;
    }

    private void CreateMeshData()
    {
        for (var y = 0; y < VoxelData.ChunkHeight; y++)
        for (var x = 0; x < VoxelData.ChunkWidth; x++)
        for (var z = 0; z < VoxelData.ChunkWidth; z++)
            AddVoxelDataToChunk(new Vector3(x, y, z));
    }

    private bool CheckVoxel(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x);
        var y = Mathf.FloorToInt(position.y);
        var z = Mathf.FloorToInt(position.z);

        return !OutsideOfChunk(x, y, z) && _voxelMap[x, y, z];
    }


    private static bool OutsideOfChunk(int x, int y, int z)
    {
        return x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 ||
               y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1;
    }

    private void CreateMesh()
    {
        var mesh = new Mesh {vertices = _vertices.ToArray(), triangles = _triangles.ToArray(), uv = _uvs.ToArray()};

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}