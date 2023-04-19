using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private CardData data;

    public Card(MainEffectData meData)
    {
        data = new CardData(meData);
        // Add the main effect as an effect
        addEffect(meData.getEffectType(), meData.getPower());
    }

    public CardData getData()
    {
        return data;
    }

    public void addEffect(EffectType effectType, float power)
    {
        CardEffect ce = new CardEffect(effectType, power);
        data.addCardEffect(ce);
    }
}
