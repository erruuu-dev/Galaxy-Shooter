using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManagerScript : MonoBehaviour
{
    [SerializeField] private bool _isGameOver = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            restartScene();
        }
        if (Input.GetKeyDown(KeyCode.Q) && _isGameOver)
        {
            quitGame();
        }
    }

    public bool GameOver()
    {
        return _isGameOver;
    }

    public void setGameOver()
    {
        _isGameOver = true;
    }

    public void restartScene()
    {
        Debug.Log("R is Pressed");
        SceneManager.LoadScene(1);
    }

    void quitGame()
    {
        Debug.Log("Q is Pressed. The player has quit the game.");
        Application.Quit();
    }
}
