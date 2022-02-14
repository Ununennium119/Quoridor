using UnityEngine;
using UnityEngine.EventSystems;

public class WallController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameManager _gameManager;
    private PlayerNumber _ownerNumber;
    private bool _isPlaced = false;
    private bool _isSelected = false;

    /// <value>Specifies whether the wall is places on board or not.</value>
    public bool IsPlaced
    {
        get => _isPlaced;
        set => _isPlaced = value;
    }


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Initializes necessary fields of the wall.
    /// Must be called after creating an instance.
    /// </summary>
    /// <param name="ownerNumber"></param>
    public void Initialize(PlayerNumber ownerNumber)
    {
        _ownerNumber = ownerNumber;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isSelected && CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("HighlightHover"));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isSelected && CanBeInteractedWith())
        {
            SetLayer(LayerMask.NameToLayer("Default"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeInteractedWith())
        {
            Select();
            _gameManager.selectedWall = this;
        }
    }


    /// <returns>A boolean that shows wall's interactability.</returns>
    private bool CanBeInteractedWith()
    {
        return !_isPlaced &&
               _gameManager.selectedWall == null &&
               _gameManager.currentPlayerNumber == _ownerNumber &&
               !_gameManager.IsCurrentPlayerSelected();
    }


    /// <summary>
    /// Set's the wall as selected.
    /// </summary>
    private void Select()
    {
        _isSelected = true;
        SetLayer(LayerMask.NameToLayer("HighlightSelected"));
    }

    /// <summary>
    /// Deselects the wall.
    /// </summary>
    public void Deselect()
    {
        _isSelected = false;
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


    public void PlaceWall(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        GetComponent<BoxCollider>().enabled = false;
        _isPlaced = true;
    }
}