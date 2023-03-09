using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    public HUD gui;

    [Header("Player Stats")]
    public int startingHealth = 10;
    public int startingAmmo = 0;
    public int ammoPerPickup = 5;

    [Header("Projectile Firing")]
    public GameObject bulletPrefab;
    public Transform gunPivot;
    public Transform crosshair;
    public float spawnOffsetDistance = 0.5f;
    public float delayBetweenShot = 0.1f;
    public bool continuousFire = true;

    [Header("Aiming")]
    public float aimingLineLength = 5;
    public bool followCursorPosition = false;
    public bool rotateWithMouseWheel = false;
    public float mouseScrollSpeed = 1;

    private bool canShoot = true;
    private int health;
    private int ammo;

    private Portal portal = null;

    // aiming
    private LineRenderer aimingLine;
    private float pivotAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
        ammo = startingAmmo;

        aimingLine = GetComponentInChildren<LineRenderer>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();

        if (canShoot)
        {
            if (ammo > 0)
            {
                if (continuousFire && Input.GetButton("Fire1"))
                    StartCoroutine(FireShot());
                else if (Input.GetButtonDown("Fire1"))
                    StartCoroutine(FireShot());
            }
            else if(Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(gameManager.ShowAlert());
            }
        }

        // up pressed while standing infront of a portal
        if (Input.GetAxis("Vertical") > 0 && portal)
        {
            portal.EnterPortal();
        }
    }

    private IEnumerator FireShot()
    {
        canShoot = false;

        Vector2 direction = (crosshair.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + direction * spawnOffsetDistance;
        float spawnAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        var bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.Euler(0, 0, spawnAngle));
        
        bulletInstance.GetComponent<Bullet>().Direction = direction;

        ammo--;
        gui.SetAmmoText(ammo);

        yield return new WaitForSeconds(delayBetweenShot);

        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 9) // enemy projectile layer
        {
            TakeDamage(1);            
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ammo")
        {
            AddAmmo(ammoPerPickup);
            GameObject.Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == 9) // enemy projectile
        {
            TakeDamage(1);
        }

        if(collision.gameObject.layer == 12) // portal layer
        {           
            portal = collision.GetComponent<Portal>();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12) // portal layer
        {
            
            portal = null;
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;

        //Debug.Log("Took " + amount + " damage. Health Left: " + health);
        gui.SetHealthText(health);

        if (health <=0)
        {
            gameManager.EndGame();
        }
    }

    private void AddAmmo(int Amount)
    {
        ammo += ammoPerPickup;
        gui.SetAmmoText(ammo);
    }

    private void Aim()
    {
        Vector2 direction = new Vector2();
        var sprite = GetComponent<SpriteRenderer>();

        if (rotateWithMouseWheel)
        {

            if (Input.mouseScrollDelta.y > 0)
            {
                pivotAngle += mouseScrollSpeed;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                pivotAngle -= mouseScrollSpeed;
            }

            if (pivotAngle >= 360 || pivotAngle <= -360)
                pivotAngle = 0;

            gunPivot.rotation = Quaternion.Euler(0, 0, pivotAngle);

            direction = (crosshair.position - transform.position).normalized;
        }
        else if (followCursorPosition)
        {
            
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (worldPos - (Vector2)transform.position).normalized;

            // rotate the pivot
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunPivot.rotation = Quaternion.Euler(0, 0, angle);  
        }
        else
        {
            pivotAngle -= Input.GetAxisRaw("Mouse X");
            pivotAngle += Input.GetAxisRaw("Mouse Y");            

            if (pivotAngle >= 360 || pivotAngle <= -360)
                pivotAngle = 0;

            gunPivot.rotation = Quaternion.Euler(0, 0, pivotAngle);

            direction = (crosshair.position - transform.position).normalized;
        }

        // flip the sprite if pointing to the left
        if (direction.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        // update the line            
        Vector2 startPos = (Vector2)transform.position + direction * spawnOffsetDistance;        

        aimingLine.SetPosition(0, startPos);
        aimingLine.SetPosition(1, crosshair.position);
    }    


}
