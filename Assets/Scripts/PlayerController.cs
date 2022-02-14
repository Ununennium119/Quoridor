using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private PlayerNumber playerNumber;
    [SerializeField] private Position playerPosition;
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
        }
    }

    
    /// <returns>A boolean that shows player's interactability.</returns>
    public bool CanBeInteractedWith()
    {
        return !IsFinished &&
               !IsSelected &&
               _gameManager.currentPlayer == playerNumber &&
               _gameManager.selectedWall == null;
    }
    

    // ToDo: clean up select and deselect
    /// <summary>
    /// Sets the player as selected.
    /// </summary>
    public void Select()
    {
        IsSelected = true;
        SetLayer(LayerMask.NameToLayer("HighlightSelected"));
        _gameManager.ShowSelectableCells(PlayerPosition);
    }

    /// <summary>
    /// Deselects the player.
    /// </summary>
    public void Deselect()
    {
        IsSelected = false;
        SetLayer(LayerMask.NameToLayer("Default"));
        _gameManager.HideSelectableCells();
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
}