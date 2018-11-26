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
                map[x, 1, z] = (byte)Random.Range(0, 2) ;
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
                    if (map[x, y, z] == 0)
                    {
                        continue;
                    }
                    //left
                    if (isTransparent(x - 1, y, z))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.forward, false, verts, uvs, triangles);
                    }
                    //right
                    if (isTransparent(x + 1, y, z))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x + 1, y, z), Vector3.up, Vector3.forward, true, verts, uvs, triangles);
                    }
                    //bottom
                    if (isTransparent(x, y-1, z))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.forward, Vector3.right, false, verts, uvs, triangles);
                    }
                    //up
                    if (isTransparent(x , y+1, z))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, true, verts, uvs, triangles);
                    }
                    //back
                    if (isTransparent(x , y, z-1))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.right, true, verts, uvs, triangles);
                    }
                    //front
                    if (isTransparent(x , y, z+1))
                    {
                        BuildFaces(map[x, y, z], new Vector3(x, y, z + 1), Vector3.up, Vector3.right, false, verts, uvs, triangles);
                    }
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

    protected bool isTransparent(int x, int y, int z) {
        byte brick = GetByte(x, y, z);
        switch (brick)
        {
            case 0:
                return true;
            case 1:
                return false;
        }
        return false;
    }

    protected virtual byte GetByte(int x,int y,int z)
    {
        if (x<0 ||y<0||z<0 || x>=World.Instance.chunkWidth||y >= World.Instance.chunkHeight || z >= World.Instance.chunkWidth)
        {
            return 0;
        }
        return map[x, y, z];
    }

    public virtual void BuildFaces(byte brick, Vector3 corner,Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        int index = verts.Count;
        //because triangles go in clockwise
        verts.Add(corner);
        verts.Add(corner + up);
        verts.Add(corner + up + right);
        verts.Add(corner + right);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        if (reversed) {
            triangles.Add(index + 0);
            triangles.Add(index + 1);
            triangles.Add(index + 2);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
            triangles.Add(index + 0);
        }
        else
        {
            triangles.Add(index + 1);
            triangles.Add(index + 0);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
            triangles.Add(index + 2);
            triangles.Add(index + 0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
