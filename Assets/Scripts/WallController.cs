using UnityEngine;
using UnityEngine.EventSystems;

public class WallController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameManager _gameManager;

    /// <value>Represents the wall owner player number.</value>
    public PlayerNumber OwnerNumber { get; private set; }

    /// <value>Specifies whether the wall is selected or not.</value>
    public bool IsSelected { get; private set; } = false;

    /// <value>Specifies whether the wall is places on board or not.</value>
    public bool IsPlaced { get; private set; } = false;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected && CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("HighlightHover"));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected && CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("Default"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeInteractedWith())
        {
            Select();
            _gameManager.SelectedWall = this;
        }
    }


    /// <summary>
    /// Initializes necessary fields of the wall.
    /// Must be called after creating an instance.
    /// </summary>
    /// <param name="ownerNumber"></param>
    public void Initialize(PlayerNumber ownerNumber)
    {
        OwnerNumber = ownerNumber;
    }


    /// <returns>A boolean that shows wall's interactability.</returns>
    private bool CanBeInteractedWith()
    {
        return !IsPlaced &&
               !_gameManager.IsGameOver &&
               _gameManager.SelectedWall == null &&
               _gameManager.CurrentPlayerNumber == OwnerNumber &&
               !_gameManager.IsCurrentPlayerSelected();
    }


    /// <summary>
    /// Set's the wall as selected.
    /// </summary>
    private void Select()
    {
        IsSelected = true;
        SetLayer(LayerMask.NameToLayer("HighlightSelected"));
    }

    /// <summary>
    /// Deselects the wall.
    /// </summary>
    public void Deselect()
    {
        IsSelected = false;
        SetLayer(LayerMask.NameToLayer("Default"));
    }

    /// <summary>
    /// Changes the wall's layer to the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="layer">the new layer.</param>
    private void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }

    /// <summary>
    /// Moves the wall at the given <paramref name="position"/> and <paramref name="rotation"/>. 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void PlaceWall(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        GetComponent<BoxCollider>().enabled = false;
        IsPlaced = true;
    }
}