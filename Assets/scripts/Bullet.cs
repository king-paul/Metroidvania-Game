using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;    
    public float maxDistance = 100;

    private Rigidbody2D rb;
    private float distanceTraveled;
    private Vector2 origin;
    public Vector2 Direction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origin = transform.position;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Direction * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        //transform.Translate(Direction * speed * Time.fixedDeltaTime);

        distanceTraveled = Vector2.Distance(origin, transform.position);

        if (distanceTraveled > maxDistance)
            GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.layer == 8 && // player projectile layer
            collision.gameObject.layer == 7) // enemy layer
        {
            collision.GetComponent<EnemyController>().TakeDamage(1);
            
        }

        GameObject.Destroy(gameObject);
    }

}