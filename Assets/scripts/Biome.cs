using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrickType
{
    None,

    RoughStone,
    SmoothStone,
    DarkStone,
    PowderStone,
    Granite,
    Dirt,
    Sand,
    GreenLattice,

    Taint,
    Dust,
    Rust,
    Ice,
    Snow,
    DirtyIce,
    Streaks,
    Lava,
}

[System.Serializable]
public class Biome
{
    public string name = "unknown biome";
    [Multiline]
    public string desc = "no desc";
    public float mountainPower = 1;
    public float minHeight = 10;
    public float maxHeight = 10;
    public float mountainPowerBonus = 0;
    //in each biome, we have several layer, 
    //and in each layer, we have different conditions to decide what brick we are going to use
    //say biome ground has layer dirt and water.
    //dirt layer has 0.5 weight to show when it is above(y>10) or equal(8<y<12) to ground
    //water layer has 1 weight to show when it is below(y<10) the ground
    //so when we in this biome and y < 10, (water bid = 1)> (dirt bid = 0.5), so will show water
    //when y >=10 (dirt layer = 1 or 0.5)>(water bit = 0), so will show dirt
    //so easy to make mistakes..
    public BrickLayer[] brickLayers;
    public byte GetBrick(int y, float mountainValue, float blobValue, Chunk chunk)
    {
        BrickLayer bestBid = null;
        float bestBidValue = 0;
        foreach (BrickLayer brickLayer in brickLayers)
        {
            float bidValue = brickLayer.Bid(y, mountainValue, blobValue, chunk);
            if (bidValue > bestBidValue)
            {
                bestBidValue = bidValue;
                bestBid = brickLayer;
            }
        }
        if (bestBid == null)
        {
            return 0;
        }
        else
        {
            return (byte)bestBid.brickType;
        }
    }
}
