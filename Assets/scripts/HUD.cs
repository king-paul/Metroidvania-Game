using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        healthText.text = player.startingHealth.ToString();
        ammoText.text = player.startingAmmo.ToString();
    }

    private void Update()
    {
        
    }

    public void SetHealthText(int amount)
    {
        healthText.text = amount.ToString();
    }

    public void SetAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

}
