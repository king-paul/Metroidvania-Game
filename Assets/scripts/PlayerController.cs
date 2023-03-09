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
    public float spawnOffsetDistance = 0.5f;
    public float delayBetweenShot = 0.1f;
    public bool continuousFire = true;

    private bool canShoot = true;
    private int health;
    private int ammo;

    private Portal portal = null;

    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
        ammo = startingAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if(canShoot)
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

        Vector2 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (clickedPos - (Vector2)transform.position).normalized;
        //Debug.Log("Direction: " + direction);
        Vector2 spawnPosition = (Vector2)transform.position + direction * spawnOffsetDistance;
        //Debug.Log("Spawn Position: " + spawnPosition);
        //Debug.Log("Rotation: " + );
        var bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        //Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg));
        //bulletInstance.transform.forward = direction;
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

}
