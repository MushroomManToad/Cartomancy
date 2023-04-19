using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardScreen : MonoBehaviour
{
    List<GameObject> cards = new List<GameObject>();

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    PlayerStats stats;

    [SerializeField]
    private GameObject cardObject;

    [SerializeField]
    TMP_Text text;

    // Populate cards
    public void onUIOpen(List<Card> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            Vector2 spaceInUI = new Vector2(0, 0);
            // Make a UICard, place it, and store it to the cards list.
            GameObject uiCard = Instantiate(cardObject, spaceInUI, Quaternion.identity);

            UICard card = uiCard.GetComponent<UICard>();
            if(card != null)
            {
                card.setCard(cards[i]);

                CardDrag drag = uiCard.GetComponent<CardDrag>();
                if(drag != null)
                {
                    drag.setCanvas(canvas);
                    drag.setStats(stats);
                    drag.setCardScreen(this);
                }

                uiCard.transform.SetParent(gameObject.transform);
                Image im = uiCard.GetComponent<Image>();
                if(im != null)
                {
                    im.rectTransform.anchoredPosition = new Vector2(-400.0f + i * 400.0f, 192.0f);
                }

                this.cards.Add(uiCard);
            }
        }
    }

    // Remove cards and add effects
    public void onUIClose()
    {
        foreach(GameObject c in cards)
        {
            // Find what slot it's in, act accordingly, then destroy the UICard object.
            if(c != null)
            {
                UICard card = c.GetComponent<UICard>();
                if (card != null)
                {
                    if (card.playerSlot())
                    {
                        Card cardObj = card.getCard();
                        if (card != null)
                        {
                            CardData cData = cardObj.getData();
                            if (cData != null)
                            {
                                List<CardEffect> effects = cData.getCardEffects();
                                foreach (CardEffect e in effects)
                                {
                                    stats.addPlayerEffect(e);
                                }
                            }
                        }
                        // Remove card from Stats.
                        stats.removeCard(cardObj);
                    }
                    else if (card.levelSlot())
                    {
                        Card cardObj = card.getCard();
                        if (card != null)
                        {
                            CardData cData = cardObj.getData();
                            if (cData != null)
                            {
                                List<CardEffect> effects = cData.getCardEffects();
                                foreach (CardEffect e in effects)
                                {
                                    stats.addLevelEffect(e);
                                }
                            }
                        }
                        // Remove card from Stats.
                        stats.removeCard(cardObj);
                    }
                }
                Destroy(c);
            }
        }
    }

    public void setTextByCard(Card card)
    {
        switch (card.getData().getMainEffect())
        {
            case (EffectType.SPREAD_SHOT):
                text.SetText("Spread Shot");
                break;
            case (EffectType.ATTACK_RATE_UP):
                text.SetText("Throw Rate Up");
                break;
            case (EffectType.BOMB):
                text.SetText("Bomb");
                break;
            case (EffectType.GLASS_CANNON):
                text.SetText("Glass Cannon");
                break;
            case (EffectType.HEALTH):
                text.SetText("Heal");
                break;
            case (EffectType.REROLL):
                text.SetText("Discard Hand and Draw 3");
                break;
            default:
                text.SetText("");
                break;
        }
    }
}
