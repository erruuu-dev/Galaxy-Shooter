using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class uiManager : MonoBehaviour
{
    [Header("Score UI Setting")]
    [SerializeField] private Text _ScoreText;

    [Header("Game Over UI Setting")]
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _quitText;

    [Header("Lives UI Settings")]
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesImage;

    private playerScript PlayerScript;
    private gameManagerScript GameManagerScript;
    void Start()
    {
        PlayerScript = GameObject.Find("Player").GetComponent<playerScript>();
        if (PlayerScript == null )
        {
            Debug.LogError("Spawn Manager is Null! (This code is in gameManagerScript)");

        }

        GameManagerScript = GameObject.Find("Game_Manager").GetComponent<gameManagerScript>();
        if (GameManagerScript == null)
        {
            Debug.LogError("Game Manager is Null! (This code is in gameManagerScript)");

        }
    }
    
    void Update()
    {
        _ScoreText.text = "Score: " + PlayerScript.showScore();
        updateLivesUI();
        showGameOverScreen();
    }
    void updateLivesUI()
    {
        if (PlayerScript.showLives() > 0)
        {
            _livesImage.sprite = _livesSprites[PlayerScript.showLives()];
        }
        else
        {
            _livesImage.sprite = _livesSprites[0];
        }
    }
    void showGameOverScreen()
    {
        if (GameManagerScript.GameOver())
        {
            Animator anim = _gameOverText.GetComponent<Animator>();
            _gameOverText.gameObject.SetActive(true);
            anim.SetBool("isGameOverAnim", true);
            _restartText.gameObject.SetActive(true);
            _quitText.gameObject.SetActive(true);
        }
    }
}
