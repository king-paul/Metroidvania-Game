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
    public float moveSpeed = 200;

    [Header("Jumping and falling")]
    public float jumpPower = 100;
    public float fallSpeed = 10;
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
    private Transform camera;
    private SpriteRenderer sprite;
    private Animator animator;

    private bool onGround = false;
    private float inputX;
    private bool jumpPressed = false;
    private bool jumping = false;

    private Vector2 jumpPoint;

    private Vector2 velocity = new Vector2();

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

        if (onGround && body.velocity.x != 0)
            animator.SetBool("walking", true);
        else if (!onGround || body.velocity.x == 0)
            animator.SetBool("walking", false);


        onGround = PlayerOnGround();

        /*if (onGround)
            Debug.Log("On Ground");
        else
            Debug.Log("Not On Ground");*/        

    }

    private void FixedUpdate()
    {
        // set horizontal velocity
        //if ((!AgainstWallLeft() && inputX < 0) || (!AgainstWallRight() && inputX > 0))
        //{
            body.AddForce(new Vector2(inputX * moveSpeed * Time.fixedDeltaTime, 0));
        //}

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
            //Debug.Log("Hit max jump height");
            body.velocity = new Vector2(body.velocity.x, Physics.gravity.y);
            //body.AddForce(Physics.gravity * fallSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        //Debug.Log("Velocity:" + body.velocity);
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

    private bool PlayerOnGround()
    {
        RaycastHit2D leftCheck = Physics2D.Raycast((Vector2)transform.position + leftOffset, Vector2.down, castDistance, groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast((Vector2)transform.position + rightOffset, Vector2.down, castDistance, groundLayer);

        Debug.DrawRay((Vector2)transform.position + leftOffset, Vector2.down * castDistance, Color.yellow);
        Debug.DrawRay((Vector2)transform.position + rightOffset, Vector2.down * castDistance, Color.yellow);

        if (leftCheck || rightCheck)
            return true;

        return false;
    }

    private bool AgainstWallLeft()
    {
        Vector2 midLeftOffset = new Vector2(-0.4f, 0);

        RaycastHit2D leftCheck = Physics2D.Raycast((Vector2)transform.position + leftOffset, Vector2.left, castDistance, groundLayer);      
        Debug.DrawRay((Vector2)transform.position + midLeftOffset, Vector2.left * castDistance, Color.yellow);

        if (leftCheck)
        {
            Debug.Log("Against wall on left");
            return true;
        }

        return false;
    }

    private bool AgainstWallRight()
    {
        Vector2 midRightOffset = new Vector2(0.4f, 0);

        RaycastHit2D rightCheck = Physics2D.Raycast((Vector2)transform.position + rightOffset, Vector2.right, castDistance, groundLayer);        
        Debug.DrawRay((Vector2)transform.position + midRightOffset, Vector2.right * castDistance, Color.yellow);

        if (rightCheck)
        {
            Debug.Log("Against wall on right");
            return true;
        }

        return false;
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var tileMap = collision.gameObject.GetComponent<Tilemap>();

        if (collision.gameObject.layer == 3 && TileBelow(tileMap))//transform.position.y > collision.transform.position.y)
        {
            Debug.Log("hit ground");
            onGround = true;
            jumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {        
        var tileMap = collision.gameObject.GetComponent<Tilemap>();      

        //grid.WorldToCell(collision.transform.position); 

        if (collision.gameObject.layer == 3 && TileBelow(tileMap))//transform.position.y > collision.transform.position.y)
        {
            Debug.Log("left ground");
            onGround = false;
        }
    }

    private bool TileBelow(Tilemap tileMap)
    {
        Vector3Int playerPos = tileMap.WorldToCell(transform.position);

        foreach (Vector3Int tilePosition in tileMap.cellBounds.allPositionsWithin)
        {
            if (tilePosition.x == playerPos.x && tilePosition.y == playerPos.y - 1)
            {
                //Debug.Log("tile position: " + tilePosition);

                return true;
            }
        }

        return false;
    }
    */

}
