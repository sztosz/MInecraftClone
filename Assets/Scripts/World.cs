using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

    private readonly Chunk[,] _chunks = new Chunk[VoxelData.WorldLedgeInChunks, VoxelData.WorldLedgeInChunks];

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (var x = 0; x < VoxelData.WorldLedgeInChunks; x++)
        for (var z = 0; z < VoxelData.WorldLedgeInChunks; z++)
            CreateNewChunk(x, z);
    }

    public byte GetVoxel(Vector3 position)
    {
        if (IsVoxelInWorld(position))
            return 0;
        if (position.y < 1)
            return 1;
        if (position.y == VoxelData.ChunkHeight - 1)
            return 3;
        return 2;
    }

    private void CreateNewChunk(int x, int z)
    {
        _chunks[x, z] = new Chunk(new ChunkCoords(x, z), this);
    }

    private bool IsChunkInWorld(ChunkCoords coords)
    {
        return coords.X >= 0 && coords.X < VoxelData.WorldLedgeInChunks - 1 &&
               coords.Z >= 0 && coords.Z < VoxelData.WorldLedgeInChunks - 1;
    }

    private static bool IsVoxelInWorld(Vector3 position)
    {
        return position.x < 0 || position.x > VoxelData.WorldSizeInBlocks - 1 ||
               position.y < 0 || position.y > VoxelData.ChunkHeight - 1 ||
               position.z < 0 || position.z > VoxelData.WorldSizeInBlocks - 1;
    }
}

[Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")] public int backFaceTexure;

    public int frontFaceTexure;
    public int topFaceTexure;
    public int bottomFaceTexure;
    public int leftFaceTexure;
    public int rightFaceTexure;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureId(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: return backFaceTexure;
            case 1: return frontFaceTexure;
            case 2: return topFaceTexure;
            case 3: return bottomFaceTexure;
            case 4: return leftFaceTexure;
            case 5: return rightFaceTexure;
            default:
                Debug.Log($"Error in GetTextureId, invalid face index {faceIndex}");
                return 0;
        }
    }
}