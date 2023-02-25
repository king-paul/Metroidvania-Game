using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovment : MonoBehaviour
{
    public float moveSpeed = 200;

    [Header("Jumping and falling")]
    public float jumpPower = 100;
    public float fallSpeed = 10;
    public float maxJumpHeight = 3;

    [Header("Camera")]
    public bool usePlayerPoistionAsOffset = true;
    public Vector2 cameraOffset;

    // components
    private Rigidbody2D body;
    private Transform camera;
    private SpriteRenderer sprite;

    private bool onGround = false;
    private float inputX;
    private bool jumpPressed = false;
    private bool jumping = false;

    private Vector2 jumpPoint;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        camera = Camera.main.transform;

        if (usePlayerPoistionAsOffset)
        {
            cameraOffset.x = -transform.position.x;
            cameraOffset.y = -transform.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        inputX = Input.GetAxisRaw("Horizontal");        

        if (Input.GetButtonDown("Jump") && onGround)
        {
            //Debug.Log("Jump Pressed");
            jumpPressed = true;

            jumpPoint = transform.position;
        }
        
        if(Input.GetButtonUp("Jump"))
        {
            jumpPressed = false;
        }  

    }

    private void FixedUpdate()
    {
        // set horizontal velocity
        body.velocity = new Vector2(inputX * moveSpeed * Time.fixedDeltaTime, body.velocity.y);

        if (!onGround)
        {
            body.AddForce(Physics.gravity * fallSpeed * Time.fixedDeltaTime);            
        }

        if (jumpPressed && onGround)
        {           
            body.AddForce(Vector3.up * jumpPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
            jumping = true;
        }

        if (jumping && Vector2.Distance(transform.position, jumpPoint) > maxJumpHeight)
        {
            Debug.Log("Hit max jump height");
            body.velocity = new Vector2(body.velocity.x, Physics.gravity.y);
            //body.AddForce(Physics.gravity * fallSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
    }

    private void LateUpdate()
    {
        // update the face direction
        if (body.velocity.x > 0) // facing right
            sprite.flipX= false;

        if (body.velocity.x < 0) // facing left
            sprite.flipX = true;
            

        // make camera follow player
        camera.position = new Vector3(transform.position.x + cameraOffset.x, 
                                      transform.position.y + cameraOffset.y, -10);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3 && transform.position.y > collision.transform.position.y)
        {
            Debug.Log("hit ground");
            onGround = true;
            jumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && transform.position.y > collision.transform.position.y)
        {
            Debug.Log("left ground");
            onGround = false;
        }
    }
}
