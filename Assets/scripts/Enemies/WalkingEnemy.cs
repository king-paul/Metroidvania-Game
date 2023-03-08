using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : EnemyController
{
    [Header("Walking Enemy Variables")]
    public float walkSpeed = 1;
    [Range(0f, 10f)]
    public float minDistanceFromPlayer = 1;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
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
                Invoke("FireShot", timeBetweenShots);
            }
        }
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

    private void FireShot()
    {
        Vector2 offset;

        if (transform.position.x < player.position.x)
            offset = Vector2.right * offsetDistance;
        else
            offset = Vector2.left * offsetDistance;

        offset.y = -0.2f;

        if (transform.position.x != player.position.x)
        {
            var newBullet = Instantiate(bulletPrefab, (Vector2)transform.position + offset, Quaternion.Euler(0, 0, 90));

            if (transform.position.x < player.position.x)
                newBullet.GetComponent<Bullet>().Direction = Vector2.right;

            if (transform.position.x > player.position.x)
                newBullet.GetComponent<Bullet>().Direction = Vector2.left;
        }

        hasFired = false;
        //Invoke("FireShot", fireRate);
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

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
