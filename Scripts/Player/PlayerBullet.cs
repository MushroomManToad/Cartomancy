using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    Vector2 direction;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Rigidbody2D r2;


    private int maxLife = 100;
    private int duration = 0;
    private int damage = 10;

    private void FixedUpdate()
    {
        if(duration >= maxLife)
        {
            Destroy(gameObject);
        }
        r2.MovePosition(new Vector2(transform.position.x + direction.x * speed, transform.position.y + direction.y * speed));
        duration++;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Enemy hit = collider.GetComponent<Enemy>();
        if (hit != null)
        {
            if(hit.getEffectPower(EffectType.GLASS_CANNON) > 0)
            {
                hit.kill();
            }
            else
            {
                hit.damage(damage);
            }
        }
        if (collider.GetComponent<PlayerBullet>() == null && collider.GetComponent<EnemyBullet>() == null)
        {
            Destroy(gameObject);
        }
    }

    public void setDirection(Vector2 newDir)
    {
        direction = newDir.normalized;
    }

    public void setSpeed(float newVel)
    {
        speed = newVel;
    }

    public void setDamage(int val)
    {
        damage = val;
    }

    public int getDamage()
    {
        return damage;
    }
}
