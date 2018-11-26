using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public byte[,,] map;
    public Mesh mesh;
    protected MeshRenderer meshRenderer;
    protected MeshCollider meshCollider;
    protected MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        map = new byte[World.Instance.chunkWidth,  World.Instance.chunkHeight,World.Instance.chunkWidth];
        for (int x = 0; x < World.Instance.chunkWidth; x++)
        {
            for (int z = 0; z < World.Instance.chunkWidth; z++)
            {
                map[x, 0, z] = 1;
                map[x, 1, z] = (byte)Random.Range(0, 1) ;
            }
        }
        CreateVisualMesh();
    }

    public virtual void CreateVisualMesh()
    {
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < World.Instance.chunkWidth; x++)
        {
            for (int z = 0; z < World.Instance.chunkWidth; z++)
            {
                for (int y = 0; y < World.Instance.chunkHeight; y++)
                {
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
