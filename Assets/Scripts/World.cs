using System;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
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