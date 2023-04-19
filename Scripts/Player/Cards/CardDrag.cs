using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour
{
    private Canvas canvas;
    private PlayerStats stats;

    [SerializeField]
    private UICard card;

    private CardScreen cScreen;

    Vector2 initialPos;

    private void Start()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            transform.position,
            canvas.worldCamera,
            out initialPos);
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out mousePos);

        // Reset apply values. Apply values are used only on UI Close.
        card.setApplyToPlayer(false);
        card.setApplyToLevel(false);

        transform.position = canvas.transform.TransformPoint(mousePos);
    }

    public void DropHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out mousePos);

        Vector2 snapPos;

        // Snap to add to Player
        if(mousePos.x > -380 && mousePos.x < -160 && mousePos.y < -30 && mousePos.y > -300)
        {
            snapPos = new Vector2(-269, -169);
            card.setApplyToPlayer(true);
        }
        // Snap to add to Level
        else if(mousePos.x > 160 && mousePos.x < 380 && mousePos.y < -30 && mousePos.y > -300)
        {
            snapPos = new Vector2(269, -169);
            card.setApplyToLevel(true);
        }
        else
        {
            snapPos = new Vector2(initialPos.x, initialPos.y);
        }

        transform.position = canvas.transform.TransformPoint(snapPos);
    }

    public void pointerHandler(BaseEventData data)
    {
        cScreen.setTextByCard(card.getCard());
    }

    public void setCanvas(Canvas c)
    {
        this.canvas = c;
    }

    public void setStats(PlayerStats stats)
    {
        this.stats = stats;
    }

    public void setCardScreen(CardScreen cardScreen)
    {
        this.cScreen = cardScreen;
    }
}
