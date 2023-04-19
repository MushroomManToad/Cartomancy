using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect
{
    private EffectType type;
    private float power;

    public CardEffect(EffectType type, float power)
    {
        this.type = type;
        this.power = power;
    }

    public EffectType getType() { return type; }
    public float getPower() { return power; }

}
