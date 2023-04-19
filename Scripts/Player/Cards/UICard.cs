using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    private Card card;

    [SerializeField]
    Image cardSprite;

    private bool applyToPlayer, applyToLevel;

    public void setApplyToLevel(bool val)
    {
        applyToLevel = val;
    }

    public bool playerSlot()
    {
        return applyToPlayer;
    }

    public bool levelSlot()
    {
        return applyToLevel;
    }

    public void setApplyToPlayer(bool val)
    {
        applyToPlayer = val;
    }

    public void setCard(Card c)
    {
        this.card = c;
        // Update renderer accordingly -- Using the magic of Addressables!
        cardSprite.sprite = c.getData().getCardSprite();
    }

    public Card getCard()
    {
        return card;
    }
}
