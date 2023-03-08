using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float visionRaidus = 5.0f;    

    [Header("Enemy Fire")]
    public GameObject bulletPrefab;
    public float timeBetweenShots = 1.0f;
    public float offsetDistance = 1.0f;

    protected int health;
    protected Transform player;
    protected Rigidbody2D rb;
    protected bool hasFired = false;

    private GameManager gameManager;

    protected void Start()
    {
        health = maxHealth;
        player = GameObject.FindWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.Instance;

        // ignore collisions between player and enemy
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>());        
    }

    void Update()
    {
        //if (SolidTileBelow(Vector2.right))
        //rb.AddForce(Vector2.right * walkSpeed * Time.deltaTime);

    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if(health <= 0)
            GameObject.Destroy(gameObject);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRaidus);        
    }

}