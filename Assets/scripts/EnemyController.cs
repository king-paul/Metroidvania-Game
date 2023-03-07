using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float visionRaidus = 5.0f;
    public float walkSpeed = 1;
    public float minDistanceFromPlayer = 1;

    [Header("Enemy Fire")]
    public GameObject bullet;
    public float fireRate = 1.0f;
    public float offsetDistance = 1.0f;

    private int health;
    private Transform player;
    private Rigidbody2D rb;
    private bool hasFired = false;

    private GameManager gameManager;
    
    void Start()
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
        // distance check        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionRaidus && distanceToPlayer > minDistanceFromPlayer)
        {
            MoveTowardsPlayer();
        }
        else 
        {
             rb.velocity = Vector2.zero;

            if (distanceToPlayer <= minDistanceFromPlayer && !hasFired)
            {
                hasFired = true;
                Invoke("FireShot", fireRate);
            }
        }

        //if (SolidTileBelow(Vector2.right))
        //rb.AddForce(Vector2.right * walkSpeed * Time.deltaTime);

    }

    private void FixedUpdate()
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
        Vector2 offset;

        if (transform.position.x < player.position.x)
            offset = Vector2.right * offsetDistance;
        else
            offset = Vector2.left * offsetDistance;

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
        if (transform.position.x > player.position.x)
        {
            if (SolidTileBelow(Vector2.left)) // ground check
            {                
                rb.velocity = Vector2.left * walkSpeed;// * Time.deltaTime;
                Debug.Log("Moving Left");
            }
            else
                rb.velocity = Vector2.zero;
        }
        else if (transform.position.x < player.position.x)
        {
            if (SolidTileBelow(Vector2.right)) // ground check
            {
                rb.velocity = Vector2.right * walkSpeed;// * Time.deltaTime;
                Debug.Log("Moving Right");
            }
            else
                rb.velocity = Vector2.zero;
        }
    }
    
    // Checks if there is a solide tile below to the left or right of the enemy
    private bool SolidTileBelow(Vector2 direction)
    {
        var raycast = Physics2D.Raycast((Vector2)transform.position + direction * offsetDistance, Vector2.down, 1.4f);        

        if (raycast)
        { 
            return true;
        }

        return false;        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRaidus);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);

        // raycast points
        Gizmos.color = Color.white;       
        Gizmos.DrawCube((Vector2)transform.position + Vector2.left * offsetDistance + Vector2.down * 1.3f, 
            new Vector3(0.2f, 0.2f, 0));
        Gizmos.DrawCube((Vector2)transform.position + Vector2.right * offsetDistance + Vector2.down * 1.3f,
            new Vector3(0.2f, 0.2f, 0));
    }

}