using TMPro;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    private const float FourPlayersHeight = 650.0f;
    private const float TwoPlayersHeight = 450.0f;

    private RectTransform _rectTransform;
    
    [SerializeField] private GameObject[] entries;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }


    public void Initialize(int playersCount)
    {
        switch (playersCount)
        {
            case 2:
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TwoPlayersHeight);
                entries[2].SetActive(false);
                entries[3].SetActive(false);
                break;
            case 4:
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, FourPlayersHeight);
                break;
        }
    }

    public void OnUpdateScoreboard(ScoreboardData[] scoreboardData)
    {
        for (int i = 0; i < scoreboardData.Length; i++)
        {
            ScoreboardData data = scoreboardData[i];
            GameObject entry = entries[i];
            
            entry.transform.Find("Nickname").GetComponent<TextMeshProUGUI>().SetText(data.Nickname);
            entry.transform.Find("Moves").GetComponent<TextMeshProUGUI>().SetText(data.MovesCount);
            entry.transform.Find("Status").GetComponent<TextMeshProUGUI>().SetText(data.Status);
        }
    }
}
