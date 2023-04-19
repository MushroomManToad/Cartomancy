using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : ClearEffect
{
    [SerializeField]
    int currHealth = 100;
    [SerializeField]
    int maxHealth = 100;
    [SerializeField]
    int currEnergy = 0;
    [SerializeField]
    int maxEnergy = 100;

    int maxCards = 3;

    [SerializeField]
    LevelManager lm;

    List<CardEffect> activeEffects = new List<CardEffect>();

    List<CardEffect> levelEffects = new List<CardEffect>();

    List<Card> cards = new List<Card>();

    bool firstSetup = false;

    [SerializeField]
    PlayerHPEnergyController pHPEC;

    private void Start()
    {
        if(!firstSetup)
        {
            generateRandomDeck();
            firstSetup = true;
        }
    }

    public void setCurrHealth(int amount) 
    { 
        if(amount > maxHealth)
        {
            currHealth = maxHealth;
        }
        else
        {
            currHealth = amount;
        }
    }
    public int getCurrHealth() { return currHealth; }

    public void addPlayerEffect(CardEffect effect)
    {
        if (effect.getType() == EffectType.HEALTH)
        {
            setCurrHealth(getCurrHealth() + 20);
            pHPEC.updateBarRendering(currHealth, maxHealth, currEnergy, maxEnergy);
        }
        else if (effect.getType() == EffectType.REROLL)
        {
            cards.Clear();
            generateRandomDeck();
        }
        else
        {
            activeEffects.Add(effect);
        }
    }

    public void addLevelEffect(CardEffect effect)
    {
        // If the room is cleared already, we store the card effect to be used for the next room.
        if (lm.getSceneCleared())
        {
            levelEffects.Add(effect);
        }
        // Otherwise, add it to the current room.
        else
        {
            lm.addEffect(effect);
        }
    }

    public void damage(int amount)
    {
        if (amount < 1)
        {
            amount = 1;
        }
        setCurrHealth(getCurrHealth() - amount);

        // Sync to HP Bar
        if (pHPEC != null) pHPEC.updateBarRendering(currHealth, maxHealth, currEnergy, maxEnergy);
    }

    public void loadFrom(STMPacket packet)
    {
        // Load values
        firstSetup = packet.firstSetup;
        currHealth = packet.currHealth;
        maxHealth = packet.maxHealth;
        currEnergy = packet.currEnergy;
        maxEnergy = packet.maxEnergy;
        activeEffects = packet.activeEffects;
        maxCards = packet.maxCards;
        cards = packet.cards;

        // Send leftover level effects to the new Level Manager.
        foreach(CardEffect ce in levelEffects)
        {
            lm.addEffect(ce);
        }

        // Sync with UI
        if(pHPEC != null) pHPEC.updateBarRendering(currHealth, maxHealth, currEnergy, maxEnergy);
    }

    public STMPacket packet()
    {
        STMPacket packet = new STMPacket();
        packet.firstSetup = firstSetup;
        packet.currHealth = currHealth;
        packet.maxHealth = maxHealth;
        packet.currEnergy = currEnergy;
        packet.maxEnergy = maxEnergy;
        packet.activeEffects = activeEffects;
        packet.levelEffects = levelEffects;
        packet.cards = cards;
        packet.maxCards = maxCards;
        return packet;
    }

    public override void onClear()
    {
        levelEffects.Clear();
        activeEffects.Clear();
        generateRandomDeck();
    }

    public List<Card> getCards()
    {
        return cards;
    }

    public void removeCard(Card c)
    {
        c.getData().OnDestroy();
        cards.Remove(c);
    }

    // Returns the power of a given activeEffect. If it does not exist, returns 0.
    public float getEffectPower(EffectType type)
    {
        foreach (CardEffect c in activeEffects)
        {
            if (type == c.getType())
            {
                return c.getPower();
            }
        }
        return 0.0f;
    }

    public void generateRandomDeck()
    {
        int toGen = maxCards - cards.Count;
        for(int i = 0; i < toGen; i++)
        {
            cards.Add(CardUtil.buildRandom());
        }
    }
}
