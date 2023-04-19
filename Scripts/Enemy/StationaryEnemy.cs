using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemy : Enemy
{
    [SerializeField]
    // Max CD between shots.
    private int shootCD;

    [SerializeField]
    // Max range for this enemy.
    private float range;

    [SerializeField]
    GameObject front, back, left, right;

    [SerializeField]
    [Header("Set starting timer")]
    [Tooltip("Makes it so all enemies don't shoot on the same frame.")]
    // Time remaining until it can shoot again.
    private int shootTimer = 200;
    private void FixedUpdate()
    {
        if(distToTarget() < range && shootTimer <= 0)
        {
            shootTarget();
            float multiplier = (float)getEffectPower(EffectType.ATTACK_RATE_UP);
            if (multiplier == 0) multiplier = 1;

            shootTimer = (int)((float)shootCD * multiplier);
        }
        shootTimer--;

        updateFacing();
    }

    public void updateFacing()
    {
        if(target.transform.position.x > transform.position.x + 10)
        {
            if (!right.activeInHierarchy)
            {
                disableAllDirs();
                right.SetActive(true);
            }
        }
        else if (target.transform.position.x < transform.position.x - 10)
        {
            if (!left.activeInHierarchy)
            {
                disableAllDirs();
                left.SetActive(true);
            }
        }
        else if(target.transform.position.y > transform.position.y)
        {
            if (!back.activeInHierarchy)
            {
                disableAllDirs();
                back.SetActive(true);
            }
        }
        else
        {
            if (!front.activeInHierarchy)
            {
                disableAllDirs();
                front.SetActive(true);
            }
        }
    }

    private void disableAllDirs()
    {
        front.SetActive(false);
        back.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
    }
}
