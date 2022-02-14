using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class PlayerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private PlayerNumber playerNumber;
    [SerializeField] private Position playerPosition;
    private const float MoveDuration = 0.8f;
    private const float MoveHeight = 0.2f;
    private const float JumpHeight = 1.3f;
    private GameManager _gameManager;
    private bool _isSelected = false;
    private bool _isFinished = false;

    /// <value>Represents the player's position in board.</value>
    public Position PlayerPosition
    {
        get => playerPosition;
        set => playerPosition = value;
    }

    /// <value>Specifies whether the player is selected or not.</value>
    public bool IsSelected
    {
        get => _isSelected;
        private set => _isSelected = value;
    }

    /// <value>Specifies whether the player has reached a finishing position or not.</value>
    public bool IsFinished
    {
        get => _isFinished;
        set => _isFinished = value;
    }


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("HighlightHover"));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("Default"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeInteractedWith())
        {
            Select();
            _gameManager.ShowReachableCells(playerPosition);
        }
    }


    /// <returns>A boolean that shows player's interactability.</returns>
    public bool CanBeInteractedWith()
    {
        return !IsFinished &&
               !IsSelected &&
               _gameManager.currentPlayerNumber == playerNumber &&
               _gameManager.selectedWall == null;
    }


    /// <summary>
    /// Sets the player as selected.
    /// </summary>
    public void Select()
    {
        IsSelected = true;
        SetLayer(LayerMask.NameToLayer("HighlightSelected"));
    }

    /// <summary>
    /// Deselects the player.
    /// </summary>
    public void Deselect()
    {
        IsSelected = false;
        SetLayer(LayerMask.NameToLayer("Default"));
    }


    /// <summary>
    /// Changes the player's layer to the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="layer">the new layer.</param>
    private void SetLayer(int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = layer;
        }
    }


    public void Move(Position boardPosition, Vector3 position, bool needJumping)
    {
        StartCoroutine(MoveOverSeconds(position, MoveDuration, needJumping ? JumpHeight : MoveHeight));
        playerPosition = boardPosition;
        Deselect();
    }

    private IEnumerator MoveOverSeconds(Vector3 endPos, float seconds, float height)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(endPos, startPos);
        float elapsedTime = 0.0f;
        while (elapsedTime < seconds)
        {
            transform.position =
                Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, Easing.InOutSine(elapsedTime / seconds)));
            float horizontalDistanceCovered = Vector3.Distance(startPos, transform.position);
            float distanceRatio = horizontalDistanceCovered / distance;
            transform.position += new Vector3(0, -4 * height * distanceRatio * (distanceRatio - 1), 0);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;
    }
}