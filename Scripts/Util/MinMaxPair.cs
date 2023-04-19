using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxPair
{
    public float min, max;

    public MinMaxPair(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float val()
    {
        return Random.Range(min, max);
    }
}
