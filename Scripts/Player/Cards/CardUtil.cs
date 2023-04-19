using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static Class with helper methods for generating a Card
public static class CardUtil
{
    // A map between effects and their weight (higher weight = higher chance of being chosen)
    static List<MainEffectData> mainEffectList = new List<MainEffectData>();
    // The total weights of every main effect in the registry
    private static int totalMainEffectWeights = 0;

    // Makes a random card. This algorithm is used by every part of the game that generates a random card.
    public static Card buildRandom()
    {
        // If lists have not been constructed in this game instance, construct them now.
        if (mainEffectList.Count <= 0)
        {
            buildLists();
        }
        // Choose the main effect
        MainEffectData me = chooseMainEffect();
        // Instantiate card
        Card c = new Card(me);

        // Generate secondary effects


        return c;
    }

    // Throws an exception if an invalid card is attempted to be chosen and crashes.
    private static MainEffectData chooseMainEffect()
    {
        // Should eventually consider weights to choose a random effect.

        int val = Random.Range(0, totalMainEffectWeights);
        foreach (MainEffectData effect in mainEffectList)
        {
            if(effect.getWeightCap() > val)
            {
                return effect;
            }
        }
        throw new System.Exception("Invalid Card attempted. Used weight " + val + " on a max weight of " + getTotalMEWeight());
    }

    // Registry. Call this to rebuild all lists from scratch.

    // Called on first attempt to generate a card. Should be moved to newGame / loadGame eventually.
    private static void buildLists()
    {
        // Main Effect
        MainRegistry.register();
    }

    // Should be called any time a card is *removed* from the registry to update weight caps accordingly.
    public static void syncWeightCaps()
    {
        // TODO: Not yet implemented
    }

    // Private helper method for adding a new MainEffect to the registry.
    // Used primarily by MainRegistry#register
    private static void registerMainEffect(MainEffectData meData)
    {
        mainEffectList.Add(meData);
    }

    // Helper method. Check references if weight is off.
    public static int addToMEWeights(int amount) { totalMainEffectWeights += amount; return totalMainEffectWeights; }

    // Helper method. Check references if weight is off.
    public static int removeFromMEWeights(int amount) { totalMainEffectWeights -= amount; return totalMainEffectWeights; }

    public static int getTotalMEWeight() { return totalMainEffectWeights; }

    private static class MainRegistry
    {
        public static void register()
        {
            registerMainEffect(new MainEffectData(EffectType.ATTACK_RATE_UP, 10, new MinMaxPair(0.33f, 0.66f), "RATE_UP_CARD.png"));
            registerMainEffect(new MainEffectData(EffectType.GLASS_CANNON, 10, new MinMaxPair(1, 1), "GLASS_CANNON_CARD.png"));
            registerMainEffect(new MainEffectData(EffectType.HEALTH, 10, new MinMaxPair(10, 20), "HEALTH_CARD.png"));
            registerMainEffect(new MainEffectData(EffectType.SPREAD_SHOT, 10, new MinMaxPair(1, 1), "SPREADSHOT_CARD.png"));
            registerMainEffect(new MainEffectData(EffectType.BOMB, 1, new MinMaxPair(1, 4), "BOMB_CARD.png"));
            registerMainEffect(new MainEffectData(EffectType.REROLL, 2, new MinMaxPair(1, 1), "REROLL_CARD.png"));
        }
    }
}