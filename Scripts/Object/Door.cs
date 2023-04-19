using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ClearEffect
{
    [SerializeField]
    BoxCollider2D collider2;

    [SerializeField]
    GameObject closedSprite, openSprite;

    public override void onClear()
    {
        openSprite.SetActive(true);
        closedSprite.SetActive(false);
        collider2.isTrigger = true;
    }
}
