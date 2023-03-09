using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovment : MonoBehaviour
{
    public float walkSpeed = 10;
    public float climbSpeed = 1;

    [Header("Jumping and falling")]
    public float jumpPower = 20;
    public float gravityMultiplier = 1;
    public float maxJumpHeight = 3;

    [Header("Camera")]
    public bool usePlayerPoistionAsOffset = true;
    public Vector2 cameraOffset;

    [Header("Layers")]
    public LayerMask groundLayer;

    // raycasts
    private Vector2 leftOffset = new Vector2(-0.4f, -1);
    private Vector2 rightOffset = new Vector2(0.4f, -1);
    private float castDistance = 0.2f;

    // components
    private Rigidbody2D body;
    private new Transform camera;
    private SpriteRenderer sprite;
    private Animator animator;

    private bool onGround = false;
    private float inputX, inputY;
    private bool jumping = false;
    private bool jumpPressed = false;
    private Vector2 jumpPoint;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        jumpPressed = Input.GetButtonDown("Jump");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        // ground check
        onGround = PlayerOnGround();

        if (body.velocity.y <= 0)
            jumping = false;

        /*
        if (onGround)
        {
            if (!jumping && PlayerOnEdge())
                body.gravityScale = 0;
            else
                body.gravityScale = gravityMultiplier;
        }*/

        HorizontalMovement();
        VerticalMovement();

        if (onGround && body.velocity.x != 0)
            animator.SetBool("walking", true);
        else if (!onGround || body.velocity.x == 0)
            animator.SetBool("walking", false);

        //Debug.Log("Velocity: " + body.velocity);
    }

    private void HorizontalMovement()
    {
        // horizontal movement
        if (inputX < 0)
            body.velocity =new Vector2(-walkSpeed, body.velocity.y);
        else if (inputX > 0)
            body.velocity = new Vector2(walkSpeed, body.velocity.y);
        else if (onGround)
            body.velocity = new Vector2(0, body.velocity.y);
    }

    private void VerticalMovement()
    {        
        if (jumpPressed && onGround)
        {
            jumping = true;
            jumpPoint = transform.position;
            body.gravityScale = gravityMultiplier;

            body.velocity = new Vector2(body.velocity.x, jumpPower);
        }
        
        if (jumping && Vector2.Distance(transform.position, jumpPoint) > maxJumpHeight)
        {
            Debug.Log("Hit max jump height");
            body.velocity = new Vector2(body.velocity.x, Physics.gravity.y);
            
        }

    }

    private void LateUpdate()
    {
        // update the face direction
        if (body.velocity.x > 0) // facing right
            sprite.flipX = false;

        if (body.velocity.x < 0) // facing left
            sprite.flipX = true;            

        // make camera follow player
        camera.position = new Vector3(transform.position.x + cameraOffset.x, 
                                      transform.position.y + cameraOffset.y, -10);
    }

    private bool PlayerOnGround()
    {
        RaycastHit2D leftCheck = Physics2D.Raycast((Vector2)transform.position + leftOffset, Vector2.down, castDistance, groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast((Vector2)transform.position + rightOffset, Vector2.down, castDistance, groundLayer);

        //Debug.DrawRay((Vector2)transform.position + leftOffset, Vector2.down * castDistance, Color.yellow);
        //Debug.DrawRay((Vector2)transform.position + rightOffset, Vector2.down * castDistance, Color.yellow);

        if (leftCheck || rightCheck)
            return true;

        return false;
    }
    
    private bool PlayerOnEdge()
    {        
        RaycastHit2D middleCheck = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down, 0.2f, groundLayer);

        //Debug.DrawRay((Vector2)transform.position + Vector2.down, Vector2.down * 0.2f, Color.yellow);
               
        if (!middleCheck)
            return true;        

        return false;            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) // climbable layer
        {
            body.gravityScale = 0;
            body.velocity = new Vector2(body.velocity.x, inputY * climbSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) // climbable layer
        {
            body.gravityScale = gravityMultiplier;
        }
    }

}
