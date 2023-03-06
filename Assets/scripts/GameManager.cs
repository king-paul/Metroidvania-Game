using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverDialog;
    public GameObject alertMessage;

    public float alertTime = 1.0f;

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
