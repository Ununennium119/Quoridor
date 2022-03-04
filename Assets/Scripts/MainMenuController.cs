using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private GameObject _currentMenu;

    private int _playersCount;
    private int _currentPlayer;
    private string[] _playerNicknames;
    
    [Header("Menus")] [SerializeField] private GameObject welcomeMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject promptNicknameMenu;

    [SerializeField] private TextMeshProUGUI nicknamePromptText;
    [SerializeField] private TMP_InputField nicknameFieldText;


    private void Awake()
    {
        _currentMenu = welcomeMenu;
    }


    [UsedImplicitly]
    public void SetPlayMenu()
    {
        SetMenu(playMenu);
    }

    [UsedImplicitly]
    public void SetWelcomeMenu()
    {
        SetMenu(welcomeMenu);
    }

    private void SetMenu(GameObject menu)
    {
        _currentMenu.SetActive(false);
        menu.SetActive(true);
        _currentMenu = menu;
    }


    [UsedImplicitly]
    public void StartGame(int playersCount)
    {
        _playersCount = playersCount;
        _currentPlayer = 0;
        _playerNicknames = new string[playersCount];
        SetMenu(promptNicknameMenu);
        nicknamePromptText.text = $"Enter Player {_currentPlayer + 1}'s Nickname";
        nicknameFieldText.Select();
    }

    [UsedImplicitly]
    public void SubmitNickname()
    {
        _playerNicknames[_currentPlayer] = nicknameFieldText.text;
        _currentPlayer++;
        if (_currentPlayer < _playersCount)
        {
            nicknamePromptText.text = $"Enter Player {_currentPlayer + 1}'s Nickname";
            nicknameFieldText.Select();
            nicknameFieldText.text = "";
        }
        else
        {
            GlobalVariables.PlayersNicknames = _playerNicknames;
            SceneManager.LoadScene("GameScene");
        }
    }

    [UsedImplicitly]
    public void CancelStartGame()
    {
        SetMenu(welcomeMenu);
    }
    
    
    [UsedImplicitly]
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}