using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

[RequireComponent(typeof(Mesh))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static List<Chunk> chunksWaitToInit = new List<Chunk>();
    public static List<Chunk> chunks = new List<Chunk>();
    public static int width
    {
        get { return World.Instance.chunkWidth; }
    }
    public static int height
    {
        get { return World.Instance.chunkHeight; }
    }
    public static float brickHeight
    {
        get { return World.Instance.brickHeight; }
    }
    public byte[,,] map;
    public Mesh mesh;
    protected MeshRenderer meshRenderer;
    protected MeshCollider meshCollider;
    protected MeshFilter meshFilter;
    protected bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        chunks.Add(this);
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        chunksWaitToInit.Add(this);
        if(chunksWaitToInit[0] == this)
        {
            StartCoroutine(CalculateMap());
        }
    }

    public static byte GetTheoreticalByte(Vector3 worldPos)
    {
        Random.InitState(World.Instance.seed);
        Vector3 offset0 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        Vector3 offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        Vector3 offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        return CalculteByte(worldPos, offset0, offset1, offset2);
    }

    public static byte CalculteByte(Vector3 worldPos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
    {
        float biomeValue = CalculateNoise(worldPos, offset1, 0.02f);
        int biomeIndex = Mathf.FloorToInt(biomeValue * World.Instance.biomes.Length);
        Biome biome = World.Instance.biomes[biomeIndex];

        float heightBase = biome.minHeight;
        float maxHeight = biome.maxHeight;
        float mountainHeight = maxHeight - heightBase;
        
        //Debug.Log(height+" "+ heightBase+" "+ maxHeight+" "+mountainHeight);
        float mountainValue = CalculateNoise(worldPos, offset0, 0.009f);
        float smallMountainValue = CalculateNoise(worldPos, offset2, 0.05f);

        mountainValue += biome.mountainPowerBonus;
        if (mountainValue < 0)
        {
            mountainValue = 0;
        }
        mountainValue = Mathf.Pow(mountainValue, biome.mountainPower);


        byte brick = biome.GetBrick(Mathf.FloorToInt(worldPos.y), mountainValue, smallMountainValue, null);

        mountainValue *= mountainHeight;
        
        mountainValue += heightBase;

        //mountainValue += CalculateNoise(worldPos, offset1, 0.02f) * 5 - 5;
        mountainValue += smallMountainValue * 10 - 5;



        ////large scale means more steep, less fluent, 
        ////so we should make the value less important, because we want it overall to be fluent
        ////this is to add more detail to the map generated with scalel 0.007
        ////but without the map generated with scale 0.007, this will just be blobs of small island
        //float noise = CalculateNoise(worldPos, offset0, 0.05f);
        ////this means on ground this value are more prone to have value(because noise is larger when y is smaller)
        //noise /= ((float)worldPos.y / 5.0f);
        //noise = Mathf.Max(noise, CalculateNoise(worldPos, offset1, 0.02f));
        //noise /= ((float)worldPos.y / 5.0f);
        //noise = Mathf.Max(noise, CalculateNoise(worldPos, offset2, 0.007f));
        ////Debug.Log("noise " + noiseX + " " + noiseY + " " + noiseZ+" "+noise);
        ////float halfHeightFloat = width / 2f;
        ////y smaller means height is smaller
        ////this makes ground solid and don't have mesh on sky
        ////noise += (halfHeightFloat - (float)y) / halfHeightFloat;
        //if (noise > 0.2f)
        //{
        //    return brick;
        //}
        if (mountainValue >= worldPos.y)
        {
            return brick;
        }
        return 0;
    }

    public virtual IEnumerator CalculateMap()
    {
        map = new byte[width, height, width];

        Random.InitState(World.Instance.seed);
        Vector3 offset0 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        Vector3 offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        Vector3 offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
            
                for (int z = 0; z < width; z++)
                {

                    map[x, y, z] = CalculteByte(new Vector3(x, y, z)+transform.position, offset0, offset1, offset2);
                }
            }
            yield return 0;
        }
        StartCoroutine( CreateVisualMesh());
        initialized = true;
        yield return 0;

        chunksWaitToInit.Remove(this);
        if (chunksWaitToInit.Count >0)
        {
            StartCoroutine(chunksWaitToInit[0]. CalculateMap());
        }

    }

    public static float CalculateNoise(Vector3 pos, Vector3 offset, float scale)
    {
        float noiseX = Mathf.Abs((float)pos.x + offset.x) * scale;
        float noiseY = Mathf.Abs((float)pos.y + offset.y) * scale;
        float noiseZ = Mathf.Abs((float)pos.z + offset.z) * scale;
        //value passed in generate should not be integer
        //value output is between -1 to 1
        return Mathf.Max(0,Noise.Generate(noiseX, noiseY, noiseZ));
    }

    public virtual IEnumerator CreateVisualMesh()
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
                        BuildFaces(y,map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.forward, false, verts, uvs, triangles);
                    }
                    //right
                    if (isTransparent(x + 1, y, z))
                    {
                        //use up and forward, draw this face
                        BuildFaces(y,map[x, y, z], new Vector3(x + 1, y, z), Vector3.up, Vector3.forward, true, verts, uvs, triangles);
                    }
                    //bottom
                    if (isTransparent(x, y - 1, z))
                    {
                        //use forward and right, draw another face
                        BuildFaces(y,map[x, y, z], new Vector3(x, y, z), Vector3.forward, Vector3.right, false, verts, uvs, triangles);
                    }
                    //up
                    if (isTransparent(x, y + 1, z))
                    {
                        //use forward and right, draw this face
                        BuildFaces(y,map[x, y, z], new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, true, verts, uvs, triangles);
                    }
                    //front
                    if (isTransparent(x, y, z - 1))
                    {
                        //use up and right, draw this face
                        BuildFaces(y,map[x, y, z], new Vector3(x, y, z), Vector3.up, Vector3.right, true, verts, uvs, triangles);
                    }
                    //back
                    if (isTransparent(x, y, z + 1))
                    {
                        //use up and right, draw another face
                        BuildFaces(y,map[x, y, z], new Vector3(x, y, z + 1), Vector3.up, Vector3.right, false, verts, uvs, triangles);
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
        yield return 0;
    }

    protected bool isTransparent(int x, int y, int z) {
        //dont draw the bottom
        if (y < 0) { return false; }
        byte brick = GetByte(x, y, z);
        switch (brick)
        {
            case 0:
                return true;
            default:
                return false;
        }
    }

    public virtual byte GetByte(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return GetByte( Mathf.FloorToInt( localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z));
    }

    protected virtual byte GetByte(int x, int y, int z)
    {
        if (y < 0 || y >= height)
        {
            return 0;//there will not be chunks above or below, so is empty
        }

        Vector3 worldPos = new Vector3(x, y, z) + transform.position;
        if (!initialized)
        {
            return GetTheoreticalByte(worldPos);
        }
        if (x<0 ||z<0 || x>=width|| z >= width)
        {
            Chunk chunk = Chunk.FindChunk(worldPos);
            if (chunk == this)
            {
                Debug.LogError("when position out of bound, it should not be the same chunk on " + new Vector3(x, y, z) + transform.position);
                return 0;
            }else if (chunk == null)
            {
                return GetTheoreticalByte(worldPos);
            }
            else
            {
                return chunk.GetByte(worldPos);
            }

        }
        return map[x, y, z];
    }

    public virtual void BuildFaces(int y,byte brick, Vector3 corner,Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> triangles)
    {
        int index = verts.Count;

        corner.y *= brickHeight;
        up.y *= brickHeight;
        right.y *= brickHeight;

        //because triangles go in clockwise, otherwise it is invisible
        //  1 - 2
        //  |   |
        //  0 - 3
        verts.Add(corner);
        verts.Add(corner + up);
        verts.Add(corner + up + right);
        verts.Add(corner + right);

        Vector2 uvWidth = new Vector2(0.125f, 0.125f);


        float uvCol = ((brick - 1)%8) * 0.125f;
        //a looped layer of depth of color: 0,1,2,3,2,1
        float uvRow = y % 6;
        if (uvRow >= 4) uvRow = 6 - uvRow;
        uvRow /= 4f;
        Vector2 uvCorner = new Vector2(uvCol, uvRow);
        //the texture has two rows now
        if (brick <= 8)
        {
            uvCorner.y += 0.125f;
        }

        uvs.Add(uvCorner);
        uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
        uvs.Add(new Vector2(uvCorner.x+uvWidth.x, uvCorner.y + uvWidth.y));
        uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));

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

    public bool SetBrick(byte brick, Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return SetBrick(brick, Mathf.FloorToInt( localPos.x), Mathf.FloorToInt(localPos.y),Mathf.FloorToInt(localPos.z));
    }
    public bool SetBrick(byte brick, int x,int y,int z) 
    {
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= width)
        {
            Debug.LogError("click on a position that should not be able to be clicked " + new Vector3(x, y, z));
            return false;
        }
        if(map[x,y,z] == brick)
        {
            //same brick, no need to recalculate
            return false;
        }
        map[x, y, z] = brick;
        StartCoroutine(CreateVisualMesh());

        UpdateNeighborChunk(x, 0, new Vector3(x - 2, y, z));
        UpdateNeighborChunk(x, width-1, new Vector3(x + 2, y, z));
        UpdateNeighborChunk(y, 0, new Vector3(x, y-2, z));
        UpdateNeighborChunk(y, height-1, new Vector3(x, y+2, z));
        UpdateNeighborChunk(z, 0, new Vector3(x, y, z-2));
        UpdateNeighborChunk(z, width - 1, new Vector3(x, y, z+2));

        return true;
    }

    protected virtual void UpdateNeighborChunk(int compare1,int compare2, Vector3 vec)
    {
        if (compare1 == compare2)
        {
            //on the edge, also need to recalculate another chunk connect to it
            Chunk chunk = FindChunk(vec + transform.position);
            if (chunk)
            {
                StartCoroutine(chunk.CreateVisualMesh());
            }
        }
    }
}
