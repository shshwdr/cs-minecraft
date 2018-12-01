using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public int chunkWidth = 16;
    public int chunkHeight = 16;
    public int seed = 0;
    public Chunk chunkPrefab;
    public float viewRange = 30;
    public float brickHeight = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        if (seed == 0)
        {
            seed = Random.Range(0, int.MaxValue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (float x = transform.position.x - viewRange;x < transform.position.x + viewRange; x+=chunkWidth)
        {
            //don't worry about y for now
            //for (float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y++)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z+=chunkWidth)
                {
                    if (Chunk.chunks.Count >= 200)
                    {
                        Debug.LogError("too many chunks");
                        continue;
                    }
                    Vector3 pos = new Vector3(x, 0, z);
                    pos.x = Mathf.Floor(pos.x / (float)chunkWidth) * chunkWidth;
                    pos.z = Mathf.Floor(pos.z / (float)chunkWidth) * chunkWidth;
                    //Debug.Log("pos " + pos);
                    Chunk chunk = Chunk.FindChunk(pos);
                    if (chunk != null)
                    {
                        continue;
                    }
                    //chunk will add into chunks in its start
                    chunk = Instantiate(chunkPrefab, pos, Quaternion.identity) as Chunk;
                    
                }
            }
        }
    }
}
