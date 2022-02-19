using JetBrains.Annotations;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject pauseMenu;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    [UsedImplicitly]
    private void OnPauseToggle()
    {
        if (_gameManager.IsGamePaused)
        {
            OnResume();
        }
        else
        {
            OnPause();
        }
    }
    
    private void OnPause()
    {
        pauseMenu.SetActive(true);
        _gameManager.PauseGame();
        Debug.Log("pause");
    }

    private void OnResume()
    {
        pauseMenu.SetActive(false);
        _gameManager.ResumeGame();
        Debug.Log("resume");
    }
}