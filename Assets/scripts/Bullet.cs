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

        distanceTraveled = Vector2.Distance(origin, transform.position);

        if (distanceTraveled > maxDistance)
            GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) // enemy layer
            GameObject.Destroy(gameObject);
    }

}