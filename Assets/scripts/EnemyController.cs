using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float visionRaidus = 5.0f;
    public float walkSpeed = 1;
    public float minDistanceFromPlayer = 1;

    [Header("Enemy Fire")]
    public GameObject bullet;
    public float fireRate = 1.0f;
    public float bulletSpawnOffsetDistance = 1.0f;

    private int health;
    private Transform player;
    private Rigidbody2D rb;

    private bool hasFired = false;
   
    void Start()
    {
        health = maxHealth;
        player = GameObject.FindWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();

        // ignore collisions between player and enemy
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>());
    }

    void Update()
    {
        // distance check
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionRaidus && distanceToPlayer > minDistanceFromPlayer)
        {
            MoveTowardsPlayer();
        }
        else if(distanceToPlayer <= minDistanceFromPlayer && !hasFired)
        {
            rb.velocity = Vector2.zero;
            hasFired = true;
            Invoke("FireShot", fireRate);            
        }

    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if(health <= 0)
            GameObject.Destroy(gameObject);
    }

    public void FireShot()
    {
        Vector2 offset;

        if (transform.position.x < player.position.x)
            offset = Vector2.right * bulletSpawnOffsetDistance;
        else
            offset = Vector2.left * bulletSpawnOffsetDistance;

        offset.y = -0.2f;

        if (transform.position.x != player.position.x)
        {
            var newBullet = Instantiate(bullet, (Vector2)transform.position + offset, Quaternion.Euler(0, 0, 90));

            if (transform.position.x < player.position.x)
                newBullet.GetComponent<Bullet>().Direction = Vector2.right;

            if (transform.position.x > player.position.x)
                newBullet.GetComponent<Bullet>().Direction = Vector2.left;
        }

        hasFired = false;
        //Invoke("FireShot", fireRate);
    }

    private void MoveTowardsPlayer()
    {
        if(transform.position.x > player.position.x)
            rb.AddForce(Vector2.left * walkSpeed * Time.deltaTime);
        else if(transform.position.x < player.position.x)
            rb.AddForce(Vector2.right * walkSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRaidus);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
    }

}