using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private readonly GameObject _chunkObject;
    private readonly MeshFilter _meshFilter;

    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<Vector3> _vertices = new List<Vector3>();

    private readonly byte[,,] _voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private readonly World _world;

    private int _vertexIndex;

    public Chunk(ChunkCoords coords, World world)
    {
        _world = world;

        _chunkObject = new GameObject();
        var meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();

        meshRenderer.material = _world.material;
        _chunkObject.transform.SetParent(_world.transform);
        _chunkObject.transform.position =
            new Vector3(coords.X * VoxelData.ChunkWidth, 0f, coords.Z * VoxelData.ChunkWidth);
        _chunkObject.name = $"Chunk {coords.X}, {coords.Z}";

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    public bool IsActive
    {
        get => _chunkObject.activeSelf;
        set => _chunkObject.SetActive(value);
    }

    private Vector3 Position => _chunkObject.transform.position;

    private void AddVoxelDataToChunk(Vector3 position)
    {
        for (var i = 0; i < 6; i++)
            if (!CheckVoxel(position + VoxelData.FaceChecks[i]))
            {
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 0]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 1]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 2]]);
                _vertices.Add(position + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, 3]]);

                AddTexure(_world.blockTypes[_voxelMap[(int) position.x, (int) position.y, (int) position.z]]
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
            _voxelMap[x, y, z] = World.GetVoxel(new Vector3(x, y, z) + Position);
    }

    private void CreateMeshData()
    {
        for (var y = 0; y < VoxelData.ChunkHeight; y++)
        for (var x = 0; x < VoxelData.ChunkWidth; x++)
        for (var z = 0; z < VoxelData.ChunkWidth; z++)

            AddVoxelDataToChunk(new Vector3(x, y, z));
    }

    private static bool IsVoxelInChunk(int x, int y, int z)
    {
        return x >= 0 && x <= VoxelData.ChunkWidth - 1 &&
               y >= 0 && y <= VoxelData.ChunkHeight - 1
               && z >= 0 && z <= VoxelData.ChunkWidth - 1;
    }

    private bool CheckVoxel(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x);
        var y = Mathf.FloorToInt(position.y);
        var z = Mathf.FloorToInt(position.z);
        return IsVoxelInChunk(x, y, z)
            ? _world.blockTypes[_voxelMap[x, y, z]].isSolid
            : _world.blockTypes[World.GetVoxel(position + Position)].isSolid;
    }

    private void CreateMesh()
    {
        var mesh = new Mesh {vertices = _vertices.ToArray(), triangles = _triangles.ToArray(), uv = _uvs.ToArray()};

        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
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

public class ChunkCoords
{
    public ChunkCoords(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; }
    public int Z { get; }

    public bool Equals(ChunkCoords other)
    {
        if (other == null)
            return false;
        return other.X == X && other.Z == Z;
    }
}