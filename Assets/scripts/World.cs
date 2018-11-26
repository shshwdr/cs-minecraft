using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public int chunkWidth = 16;
    public int chunkHeight = 16;
    public int seed = 0;
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
        
    }
}
