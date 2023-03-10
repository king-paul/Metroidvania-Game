using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    /*
    [Header("Tilemaps")]
    public Tilemap backgroundTiles;
    public Tilemap collisionTiles;
    public Tilemap foregroundTiles;*/

    [Tooltip("The y coordinate of the bottom boundary")]
    public float minYPosition = -10;
    public float minCameraY = -2;

    [Header("GUI and Text")]
    public GameObject gameOverDialog;
    public GameObject alertMessage;
    public float alertTime = 1.0f;

    public bool GameRunning { get; private set; } = true;

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

    void Start()
    {
        gameOverDialog.SetActive(false);
        alertMessage.SetActive(false);

        Time.timeScale = 1;
    }

    public void EndGame()
    {
        gameOverDialog.SetActive(true);
        Time.timeScale = 0;
        GameRunning = false;

        Cursor.lockState = CursorLockMode.Confined;
    }

    public IEnumerator ShowAlert()
    {
        alertMessage.SetActive(true);
        yield return new WaitForSeconds(alertTime);
        alertMessage.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1); // restart first level
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector2(0, minYPosition), new Vector2(100, 1));
    }

}
