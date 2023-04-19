using System.Collections;
using UnityEngine;

public class MainEffectData
{
    // DO NOT STORE ANY CARD-SPECIFIC INFORMATION HERE, OR COLLAPSE THINGS SUCH AS THE MINMAX PAIR.
    // THIS IS THE ONLY INSTANCE OF A GLOBAL, STATIC OBJECT THAT WILL BE USED FOR ALL CARDS.
    // THERE SHOULD *ONLY* BE DATA STORAGE HERE!!!

    private EffectType effectType;
    private int weightCap;
    private MinMaxPair range;
    private int weight;
    private string spriteName;

    // Should ONLY be called in CardUtil#registerMainEffect.
    // If you *really* need to instantiate one of these for some reason, use a new constructor to not update weights.
    public MainEffectData(EffectType type, int weight, MinMaxPair range, string spriteName)
    {
        this.effectType = type;
        this.weightCap = CardUtil.addToMEWeights(weight);
        this.weight = weight;
        this.range = range;
        this.spriteName = spriteName;
    }

    public EffectType getEffectType() { return this.effectType; }
    public int getWeightCap() { return this.weightCap; }
    public float getPower() { return this.range.val(); }
    public string getSpriteName() { return this.spriteName; }
}