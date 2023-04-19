using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cupsnail : Enemy
{
    [SerializeField]
    // Max CD between shots.
    private int shootCD;

    [SerializeField]
    // Max range for this enemy.
    private float range;

    [SerializeField]
    Animator open, close;

    [SerializeField]
    [Header("Set starting timer")]
    [Tooltip("Makes it so all enemies don't shoot on the same frame.")]
    // Time remaining until it can shoot again.
    private int shootTimer = 200;
    private int cupTimer;

    private void Start()
    {
        cupTimer = 1000 - (int)(600 * Random.value);
    }

    private void FixedUpdate()
    {
        if (distToTarget() < range && shootTimer <= 0)
        {
            shootTarget();
            float multiplier = (float)getEffectPower(EffectType.ATTACK_RATE_UP);
            if (multiplier == 0) multiplier = 1;

            shootTimer = (int)((float)shootCD * multiplier);
        }
        if(cupTimer == 120)
        {
            closeCup();
        }
        if(cupTimer == 0)
        {
            openCup();
            cupTimer = 1000 - (int)(600 * Random.value);
        }
        cupTimer--;
        shootTimer--;
    }

    private void openCup()
    {
        open.Play("Base Layer.snail_hide", -1, 0f);
    }

    private void closeCup()
    {
        open.Play("Base Layer.snail_appear", -1, 0f);
    }
}
