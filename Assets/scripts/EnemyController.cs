using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 5;

    [Header("Enemy Fire")]
    public GameObject bullet;
    public float fireRate = 1.0f;
    public Vector2 bulletSpawnOffset;

    private int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        Invoke("FireShot", fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if(health <= 0)
            GameObject.Destroy(gameObject);
    }

    public void FireShot()
    {
        var newBullet = Instantiate(bullet, (Vector2)transform.position + bulletSpawnOffset, Quaternion.identity);
        newBullet.GetComponent<Bullet>().Direction = Vector2.left;

        Invoke("FireShot", fireRate);
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8) // player projectile
        {
            health -= 1;

            if (health == 0)
                GameObject.Destroy(gameObject);
        }
    }*/

}