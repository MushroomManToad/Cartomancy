using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : Enemy
{
    [SerializeField]
    // Max CD between shots.
    private int shootCD;

    [SerializeField]
    Rigidbody2D r2;

    [SerializeField]
    // Max range for this enemy.
    private float range;

    [SerializeField]
    int framesEachDir;
    int walkTimer;

    [SerializeField]
    float speed;

    [SerializeField]
    bool isLeftRight;

    bool isMovingFirstDir = false;

    [SerializeField]
    GameObject front, back, right, left;

    [SerializeField]
    [Header("Set starting timer")]
    [Tooltip("Makes it so all enemies don't shoot on the same frame.")]
    // Time remaining until it can shoot again.
    private int shootTimer = 200;
    private void FixedUpdate()
    {
        if (distToTarget() < range && shootTimer <= 0)
        {
            shootTarget();
            float multiplier = (float)getEffectPower(EffectType.ATTACK_RATE_UP);
            if (multiplier == 0) multiplier = 1;

            shootTimer = (int)((float)shootCD * multiplier);
        }
        shootTimer--;

        walk();
    }

    private void walk()
    {
        if (isLeftRight)
        {
            if (isMovingFirstDir)
            {
                r2.MovePosition(new Vector2(transform.position.x + speed, transform.position.y));
                setFacing(Facing.EAST);
            }
            else
            {
                r2.MovePosition(new Vector2(transform.position.x - speed, transform.position.y));
                setFacing(Facing.WEST);
            }
        }
        else
        {
            if (isMovingFirstDir)
            {
                r2.MovePosition(new Vector2(transform.position.x, transform.position.y + speed));
                setFacing(Facing.NORTH);
            }
            else
            {
                r2.MovePosition(new Vector2(transform.position.x, transform.position.y - speed));
                setFacing(Facing.SOUTH);
            }
        }

        walkTimer++;
        if (walkTimer >= framesEachDir)
        {
            walkTimer = 0;
            isMovingFirstDir = !isMovingFirstDir;
        }
    }

    private void setFacing(Facing dir)
    {
        switch (dir)
        {
            case (Facing.NORTH):
                if(!back.activeInHierarchy)
                {
                    disableAllSprites();
                    back.SetActive(true);
                }
                break;
            case (Facing.EAST):
                if(!right.activeInHierarchy)
                {
                    disableAllSprites();
                    right.SetActive(true);
                }
                break;
            case (Facing.WEST):
                if(!left.activeInHierarchy)
                {
                    disableAllSprites();
                    left.SetActive(true);
                }
                break;
            case (Facing.SOUTH):
                if(!front.activeInHierarchy)
                {
                    disableAllSprites();
                    front.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    private void disableAllSprites()
    {
        back.SetActive(false);
        front.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
    }
}
