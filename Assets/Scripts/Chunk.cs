using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<Vector3> _vertices = new List<Vector3>();

    private readonly byte[,,] _voxelMap = new byte[VoxelData.ChunkHeight, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private int _vertexIndex;

    private World world;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void AddVoxelDataToChunk(Vector3 position)
    {
        for (var i = 0; i < 6; i++)
            if (!CheckVoxel(position + VoxelData.FaceChecks[i]))
            {
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 0]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 1]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 2]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 3]]);

                AddTexure(world.blockTypes[_voxelMap[(int) position.x, (int) position.y, (int) position.z]]
                    .GetTextureId(i));

                _triangles.Add(_vertexIndex);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 3);

                _vertexIndex += 4;
            }
    }

    private void PopulateVoxelMap()
    {
        for (var y = 0; y < VoxelData.ChunkHeight; y++)
        for (var x = 0; x < VoxelData.ChunkWidth; x++)
        for (var z = 0; z < VoxelData.ChunkWidth; z++)
            if (y < 1)
                _voxelMap[x, y, z] = 0;
            else if (y == VoxelData.ChunkHeight - 1)
                _voxelMap[x, y, z] = 2;
            else
                _voxelMap[x, y, z] = 1;
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

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 ||
            y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        return world.blockTypes[_voxelMap[x, y, z]].isSolid;
    }


    private void CreateMesh()
    {
        var mesh = new Mesh {vertices = _vertices.ToArray(), triangles = _triangles.ToArray(), uv = _uvs.ToArray()};

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void AddTexure(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        var x = textureID - y * VoxelData.TextureAtlasSizeInBlocks;

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        // TODO Flip texture in atlas to start from bottom left corner and get rid of this 
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        _uvs.Add(new Vector2(x, y));
        _uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }
}