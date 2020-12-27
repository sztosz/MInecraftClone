using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public int seed;
    
    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public BlockType[] blockTypes;
    private readonly List<ChunkCoords> _activeCHunks = new List<ChunkCoords>();

    private readonly Chunk[,] _chunks = new Chunk[VoxelData.WorldLedgeSizeInChunks, VoxelData.WorldLedgeSizeInChunks];
    private ChunkCoords _playerChunkCoords;
    private ChunkCoords _playerLastChunkCoords;

    private void Start()
    {
        Random.InitState(seed);
        GenerateWorld();
        _playerLastChunkCoords = GetChunkCoordsFromVector3(player.position);
    }

    public void Update()
    {
        _playerChunkCoords = GetChunkCoordsFromVector3(player.position);
        if (!_playerChunkCoords.Equals(_playerLastChunkCoords))
            CheckViewDistance();
    }

    // public void FixedUpdate()
    // {
    //     
    // }

    private void GenerateWorld()
    {
        for (var x = VoxelData.WorldLedgeSizeInChunks / 2 - VoxelData.ViewDistanceInChunks;
            x < VoxelData.WorldLedgeSizeInChunks / 2 + VoxelData.ViewDistanceInChunks;
            x++)
        for (var z = VoxelData.WorldLedgeSizeInChunks / 2 - VoxelData.ViewDistanceInChunks;
            z < VoxelData.WorldLedgeSizeInChunks / 2 + VoxelData.ViewDistanceInChunks;
            z++)
            CreateNewChunk(x, z);
        player.position = new Vector3(
            VoxelData.WorldLedgeSizeInChunks * VoxelData.ChunkWidth / 2f,
            VoxelData.ChunkHeight + 2f,
            VoxelData.WorldLedgeSizeInChunks * VoxelData.ChunkWidth / 2f);
        ;
    }

    private static ChunkCoords GetChunkCoordsFromVector3(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x / VoxelData.ChunkWidth);
        var z = Mathf.FloorToInt(position.z / VoxelData.ChunkWidth);
        return new ChunkCoords(x, z);
    }

    private void CheckViewDistance()
    {
        var chunkX = Mathf.FloorToInt(player.position.x / VoxelData.ChunkWidth);
        var chunkZ = Mathf.FloorToInt(player.position.z / VoxelData.ChunkWidth);


        var previouslyActiveChunks = new List<ChunkCoords>(_activeCHunks);

        for (var x = chunkX - VoxelData.ViewDistanceInChunks; x < chunkX + VoxelData.ViewDistanceInChunks; x++)
        for (var z = chunkZ - VoxelData.ViewDistanceInChunks; z < chunkZ + VoxelData.ViewDistanceInChunks; z++)
        {
            var thisChunk = new ChunkCoords(x, z);
            if (IsChunkInWorld(x, z))
            {
                if (_chunks[x, z] == null)
                {
                    CreateNewChunk(thisChunk);
                }
                else if (!_chunks[x, z].IsActive)
                {
                    _chunks[x, z].IsActive = true;
                    _activeCHunks.Add(thisChunk);
                }
            }

            for (var i = 0; i < previouslyActiveChunks.Count; i++)
                if (previouslyActiveChunks[i].Equals(thisChunk))
                    previouslyActiveChunks.RemoveAt(i);
        }

        foreach (var coords in previouslyActiveChunks)
            _chunks[coords.X, coords.Z].IsActive = false;
    }

    private void CreateNewChunk(int x, int z)
    {
        CreateNewChunk(new ChunkCoords(x, z));
    }

    private void CreateNewChunk(ChunkCoords coords)
    {
        _chunks[coords.X, coords.Z] = new Chunk(coords, this);
        _activeCHunks.Add(coords);
    }

    public static byte GetVoxel(Vector3 position)
    {
        if (IsVoxelInWorld(position))
            return 0;
        if (position.y < 1)
            return 1;
        if (position.y == VoxelData.ChunkHeight - 1)
        {
            var tempNoise = Noise.Get2DPerlin(new Vector2(position.x, position.z), 0, 0.1f);
            if (tempNoise < 0.5)
                return 3;
            else
                return 5;

        }
        return 2;
    }

    private bool IsChunkInWorld(int x, int z)
    {
        return IsChunkInWorld(new ChunkCoords(x, z));
    }

    private bool IsChunkInWorld(ChunkCoords coords)
    {
        return coords.X >= 0 && coords.X < VoxelData.WorldLedgeSizeInChunks - 1 &&
               coords.Z >= 0 && coords.Z < VoxelData.WorldLedgeSizeInChunks - 1;
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

    [Header("Texture Values")]
    public int backFaceTexure;
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