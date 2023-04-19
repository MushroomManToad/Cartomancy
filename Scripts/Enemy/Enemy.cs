using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    private LevelManager lm;

    [SerializeField]
    private int currHealth, maxHealth;

    [SerializeField]
    protected GameObject target;

    [SerializeField]
    protected GameObject enemyBullet;

    [SerializeField]
    protected Collider2D enemyCollider;

    private List<CardEffect> activeEffects = new List<CardEffect>();

    [SerializeField]
    GameObject deathParticle;

    public void setCurrHealth(int amount) { currHealth = amount; }
    public int getCurrHealth() { return currHealth; }

    public void damage(int amount)
    {
        if(amount < 1)
        {
            amount = 1;
        }
        setCurrHealth(getCurrHealth() - amount);

        if(getCurrHealth() <= 0)
        {
            kill();
        }
    }

    public void kill()
    {
        Instantiate(deathParticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        lm.removeEnemy(this);
        Destroy(gameObject);
    }

    public Vector2 getTargetPos()
    {
        return new Vector2(target.transform.position.x, target.transform.position.y);
    }

    public Vector2 getShootDir()
    {
        Vector2 v2 = new Vector2(getTargetPos().x - transform.position.x, getTargetPos().y - transform.position.y);
        return v2.normalized;
    }

    public float distToTarget()
    {
        return Mathf.Sqrt(Mathf.Pow(getTargetPos().x - transform.position.x, 2) + Mathf.Pow(getTargetPos().y - transform.position.y, 2));
    }

    public void shootTarget()
    {
        Vector2 dirVec = getShootDir();
        Vector3 spawnLoc = new Vector3(transform.position.x + dirVec.x * 0.5f, transform.position.y + dirVec.y * 0.5f, 0.0f);
        spawnEnemyBullet(dirVec, spawnLoc);

        if(getEffectPower(EffectType.SPREAD_SHOT) > 0)
        {
            dirVec = Rotate(dirVec, 15.0f);
            spawnEnemyBullet(dirVec, spawnLoc);

            dirVec = Rotate(dirVec, -30.0f);
            spawnEnemyBullet(dirVec, spawnLoc);
        }
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public void spawnEnemyBullet(Vector2 dirVec, Vector3 spawnLoc)
    {
        // Instantiate object
        GameObject bullet = Instantiate(enemyBullet, spawnLoc, Quaternion.identity);
        EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
        eb.setDirection(dirVec);

        if (getEffectPower(EffectType.GLASS_CANNON) > 0)
        {
            eb.setDamage(30);
        }

        // Disable bullet edge collision
        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.disableEdgeCollisions(bullet.GetComponent<Collider2D>());
        }

        // Disable bullet colliding with shooter.
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), enemyCollider, true);
    }

    public void removeEffect(CardEffect effect)
    {
        activeEffects.Remove(effect);
    }

    public void addEffect(CardEffect effect)
    {
        if(effect.getType() == EffectType.HEALTH)
        {
            maxHealth = maxHealth + 20;
            setCurrHealth(getCurrHealth() + 20);
        }
        else if(effect.getType() == EffectType.REROLL)
        {
            // Do nothing, lol
        }
        else
        {
            activeEffects.Add(effect);
        }
    }

    // Returns the power of a given activeEffect. If it does not exist, returns 0.
    public float getEffectPower(EffectType type)
    {
        foreach(CardEffect c in activeEffects)
        {
            if(type == c.getType())
            {
                return c.getPower(); 
            }
        }
        return 0.0f;
    }
}
