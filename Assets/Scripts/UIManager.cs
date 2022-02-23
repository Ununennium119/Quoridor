using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;

    private const float TurnChangedTextFadeInSeconds = 0.4f;
    private const float TurnChangedTextFadeOutSeconds = 0.4f;
    private const float TurnChangedTextStopSeconds = 0.8f;
    [SerializeField] private TextMeshProUGUI turnChangedText;

    private const float GameOverFadeInSeconds = 1.5f;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [SerializeField] private GameObject scoreboard;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject pauseMenu;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        turnChangedText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        scoreboard.SetActive(false);
        backButton.SetActive(false);
    }


    private void Start()
    {
        scoreboard.GetComponent<ScoreboardController>().Initialize(_gameManager.PlayersCount);
    }


    [UsedImplicitly]
    public void OnPauseToggle(InputAction.CallbackContext context)
    {
        if (!_gameManager.IsGameOver)
        {
            if (context.started)
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
        }
    }

    [UsedImplicitly]
    public void OnShowScoreboard(InputAction.CallbackContext context)
    {
        if (!_gameManager.IsGameOver)
        {
            if (context.started)
            {
                scoreboard.SetActive(true);
            }
            else if (context.canceled)
            {
                scoreboard.SetActive(false);
            }
        }
    }

    private void OnPause()
    {
        pauseMenu.SetActive(true);
        _gameManager.PauseGame();
    }

    private void OnResume()
    {
        pauseMenu.SetActive(false);
        _gameManager.ResumeGame();
    }

    public void OnTurnChanged(string playerNickname)
    {
        turnChangedText.text = $"It's {playerNickname}'s Turn!";
        StartCoroutine(ShowTurnChangedText());
    }

    public void OnGameOver()
    {
        Debug.Log(backButton.GetComponent<Image>());
        Debug.Log(backButton.GetComponentInChildren<TextMeshProUGUI>());
        Debug.Log(scoreboard.GetComponent<RawImage>());
        pauseButton.GetComponent<Button>().interactable = false;
        backButton.SetActive(true);
        scoreboard.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        StartCoroutine(FadeInGraphic(GameOverFadeInSeconds, backButton.GetComponent<Image>()));
        StartCoroutine(FadeInGraphic(GameOverFadeInSeconds, backButton.GetComponentInChildren<TextMeshProUGUI>()));
        StartCoroutine(FadeInGraphic(GameOverFadeInSeconds, scoreboard.GetComponent<RawImage>()));
        StartCoroutine(FadeInGraphic(GameOverFadeInSeconds, gameOverText));
    }


    private IEnumerator ShowTurnChangedText()
    {
        turnChangedText.gameObject.SetActive(true);
        yield return StartCoroutine(FadeInGraphic(TurnChangedTextFadeInSeconds, turnChangedText));
        yield return new WaitForSeconds(TurnChangedTextStopSeconds);
        yield return StartCoroutine(FadeOutGraphic(TurnChangedTextFadeOutSeconds, turnChangedText));
        turnChangedText.gameObject.SetActive(false);
    }


    private static IEnumerator FadeOutGraphic(float fadeSeconds, Graphic graphic)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeSeconds)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b,
                1.0f - (elapsedTime / fadeSeconds));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
    }

    private static IEnumerator FadeInGraphic(float fadeSeconds, Graphic graphic)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeSeconds)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, elapsedTime / fadeSeconds);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
    }
}