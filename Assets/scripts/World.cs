using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public Biome[] biomes;
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
        int incase = 0;
        for (int multix = 0; viewRange > multix * chunkWidth; multix++)
        {
            incase++;
            if (incase > 500)
            {
                Debug.LogError("woops");
                return;
            }
            for (int multiz = 0; viewRange > multiz * chunkWidth; multiz++)
            {
                incase++;
                if (incase > 500)
                {
                    Debug.LogError("woops");
                    return;
                }
                
                for (int ix = -multix; ix <= multix; ix += 2* multix)
                {
                    incase++;
                    if (incase > 500)
                    {
                        Debug.LogError("woops");
                        return;
                    }
                    for (int iz = -multiz; iz <= multiz; iz += 2* multiz)
                    {
                        incase++;
                        if (incase > 500)
                        {
                            Debug.LogError(ix+"woops"+iz+" "+ multiz+" "+ multix);
                            return;
                        }
                        float x = transform.position.x + ix*chunkWidth;
                        float z = transform.position.z + iz * chunkWidth;
                        if (Chunk.chunks.Count >= 100)
                        {
                            Debug.LogError("too many chunks");
                            continue;
                        }
                        Vector3 pos = new Vector3(x, 0, z);
                        pos.x = Mathf.Floor(pos.x / (float)chunkWidth) * chunkWidth;
                        pos.z = Mathf.Floor(pos.z / (float)chunkWidth) * chunkWidth;
                        //Debug.Log("pos " + pos);
                        Chunk chunk = Chunk.FindChunk(pos);
                        if (chunk == null)
                        {
                            //chunk will add into chunks in its start
                            chunk = Instantiate(chunkPrefab, pos, Quaternion.identity) as Chunk;
                        }
                        if(iz == 0)
                        {
                            break;
                        }
                    }
                    if (ix == 0)
                    {
                        break;
                    }
                }
            }
        }
        
    }
}
