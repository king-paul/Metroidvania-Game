using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap backgroundTiles;
    public Tilemap collisionTiles;
    public Tilemap foregroundTiles;

    public GameObject gameOverDialog;
    public GameObject alertMessage;

    public float alertTime = 1.0f;

    public static GameManager Instance { get; private set; } // singleton

    private void Awake()
    {
        // If there is an instance, and it is not this one, delete it.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverDialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        gameOverDialog.SetActive(true);
        Time.timeScale = 0;        
    }

    public IEnumerator ShowAlert()
    {
        alertMessage.SetActive(true);
        yield return new WaitForSeconds(alertTime);
        alertMessage.SetActive(false);
    }

}
