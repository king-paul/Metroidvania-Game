using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverDialog;

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

}
