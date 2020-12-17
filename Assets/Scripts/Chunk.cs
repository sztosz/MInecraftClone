using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private void Start()
    {
        var vertexIndex = 0;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        for (var i = 0; i < 6; i++)
        for (var j = 0; j < 6; j++)
        {
            vertices.Add(VoxelData.VoxelVertices[VoxelData.VoxelTriangles[i, j]]);
            triangles.Add(vertexIndex);

            uvs.Add(VoxelData.VoxelUvs[j]);

            vertexIndex++;
        }

        var mesh = new Mesh {vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = uvs.ToArray()};

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}