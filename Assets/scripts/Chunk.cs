using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

[RequireComponent(typeof(Mesh))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static List<Chunk> chunks = new List<Chunk>();
    public static int width
    {
        get { return World.Instance.chunkWidth; }
    }
    public static int height
    {
        get { return World.Instance.chunkHeight; }
    }
    public byte[,,] map;
    public Mesh mesh;
    protected MeshRenderer meshRenderer;
    protected MeshCollider meshCollider;
    protected MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        chunks.Add(this);
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        map = new byte[width, height, width];
        for (int x = 0; x < width; x++)
        {
            float noiseX = (float)x / width;
            for (int y = 0; y < height; y++)
            {
                float noiseY = (float)y / height;
                for (int z = 0; z < width; z++)
                {
                    float noiseZ = (float)z / width;
                    //value passed in generate should be float between 0 and 1
                    //value output is between -1 to 1
                    float noise = Noise.Generate(noiseX,noiseY,noiseZ);
                    float halfHeightFloat = width / 2f;
                    //y smaller means height is smaller
                    //this makes ground solid and don't have mesh on sky
                    noise += (halfHeightFloat - (float)y) / halfHeightFloat;
                    if (noise > 0.2f)
                    {
                        map[x, y, z] = 1;
                    }
                }
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

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y, z] == 0)
                    {
                        continue;
                    }
                    //left
                    if (isTransparent(x - 1, y, z))
                    {
                        //use up and forward, draw another face
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.forward, false, verts, uvs, triangles);
                    }
                    //right
                    if (isTransparent(x + 1, y, z))
                    {
                        //use up and forward, draw this face
                        BuildFaces(map[x, y, z], new Vector3(x + 1, y, z), Vector3.up, Vector3.forward, true, verts, uvs, triangles);
                    }
                    //bottom
                    if (isTransparent(x, y-1, z))
                    {
                        //use forward and right, draw another face
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.forward, Vector3.right, false, verts, uvs, triangles);
                    }
                    //up
                    if (isTransparent(x , y+1, z))
                    {
                        //use forward and right, draw this face
                        BuildFaces(map[x, y, z], new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, true, verts, uvs, triangles);
                    }
                    //front
                    if (isTransparent(x , y, z-1))
                    {
                        //use up and right, draw this face
                        BuildFaces(map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.right, true, verts, uvs, triangles);
                    }
                    //back
                    if (isTransparent(x , y, z+1))
                    {
                        //use up and right, draw another face
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
        if (x<0 ||y<0||z<0 || x>=width||y >= height || z >= width)
        {
            return 0;
        }
        return map[x, y, z];
    }

    public virtual void BuildFaces(byte brick, Vector3 corner,Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        int index = verts.Count;
        //because triangles go in clockwise, otherwise it is invisible
        //  1 - 2
        //  |   |
        //  0 - 3
        verts.Add(corner);
        verts.Add(corner + up);
        verts.Add(corner + up + right);
        verts.Add(corner + right);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        //reversed = false means we want to draw it counter clock
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

    public static Chunk FindChunk(Vector3 pos)
    {
        for(int i = 0; i < chunks.Count; i++)
        {
            Vector3 chunkPos = chunks[i].transform.position;
            if (pos.x < chunkPos.x || pos.z < chunkPos.z ||
                //take care this place need to be >= instead of >, 
                //otherwise a chunk on this position will think as already existed, 
                //and cause some chunks missing in world
                pos.x >= chunkPos.x + width ||  pos.z >= chunkPos.z + width
               // || pos.y >= chunkPos.y + height || pos.y < chunkPos.y 
                )
            {
                continue;
            }
            return chunks[i];
        }

        return null;
    }
}
