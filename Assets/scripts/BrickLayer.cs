using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrickLayerCondition
{
    None,

    AboveGroundLevel,
    GroundLevel,
    BelowGroundLevel,
}

[System.Serializable]
public class BrickLayer
{
    public string name = "unknown brick layer";
    public BrickType brickType;
    public float weight;
    public BrickLayerCondition[] conditions;
    public virtual float Bid(int y, float mountainValue, float blobValue, Chunk chunk)
    {
        float bid = 0;
        foreach (BrickLayerCondition condition in conditions)
        {
            switch (condition)
            {
                case BrickLayerCondition.None:
                    bid++;
                    break;
                case BrickLayerCondition.AboveGroundLevel:
                    if (y > 10) bid++;
                    break;
                case BrickLayerCondition.GroundLevel:
                    if (y > 8&&y<12) bid++;
                    break;
                case BrickLayerCondition.BelowGroundLevel:
                    if (y < 10) bid++;
                    break;
            }
        }
        //in case of forget to set weight
        if (weight == 0) return bid;
        return bid * weight;
    }
}
