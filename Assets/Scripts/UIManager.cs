using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;

    private const float TurnChangedTextFadeInSeconds = 0.7f;
    private const float TurnChangedTextFadeOutSeconds = 0.6f;
    private const float TurnChangedTextStopSeconds = 1.0f;
    [SerializeField] private TextMeshProUGUI turnChangedText;

    [SerializeField] private GameObject pauseMenu;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        turnChangedText.gameObject.SetActive(false);
        turnChangedText.color =
            new Color(turnChangedText.color.r, turnChangedText.color.g, turnChangedText.color.b, 0.0f);
    }


    [UsedImplicitly]
    public void OnPauseToggle()
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

    public void OnTurnChanged(string playerNickname)
    {
        turnChangedText.text = $"It's {playerNickname}'s Turn!";
        StartCoroutine(ShowTurnChangedText());
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
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeSeconds)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f - (elapsedTime / fadeSeconds));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
    }
    
    private static IEnumerator FadeInGraphic(float fadeSeconds, Graphic graphic)
    {
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