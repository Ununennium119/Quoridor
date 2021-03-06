using UnityEngine;
using UnityEngine.EventSystems;

public class CellController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameManager _gameManager;
    private readonly bool[] _isBlocked = new bool[4];

    /// <value>Represents the cell's position in board.</value>
    public Position CellPosition { get; private set; }

    /// <value>Specifies whether a player is on the cell or not.</value>
    public bool ContainsPlayer { get; set; } = false;

    /// <value>If a player is selected, Specifies whether the cell is reachable by the selected player or not.</value>
    public bool IsReachable { get; private set; } = false;

    /// <value>If the cell is reachable, specifies the player need to jump to reach the cell or not.</value>
    public bool NeedJumping { get; private set; } = false;

    /// <param name="direction"></param>
    /// <returns>Is the cell in the given <paramref name="direction"/> blocked or not.</returns>
    public bool IsBlocked(Direction direction)
    {
        return _isBlocked[(int) direction];
    }

    /// <summary>
    /// Sets whether the cell in the given <paramref name="direction"/> is blocked or not.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="isBlocked"></param>
    public void SetBlocked(Direction direction, bool isBlocked)
    {
        _isBlocked[(int) direction] = isBlocked;
    }


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsReachable)
        {
            SetLayer(LayerMask.NameToLayer("HighlightHover"));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsReachable)
        {
            SetLayer(LayerMask.NameToLayer("HighlightSelectable"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsReachable)
        {
            _gameManager.MovePlayer(this);
        }
    }


    /// <summary>
    /// Initializes necessary fields of the cell.
    /// Must be called after creating an instance.
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <param name="isUpBlocked"></param>
    /// <param name="isDownBlocked"></param>
    /// <param name="isRightBlocked"></param>
    /// <param name="isLeftBlocked"></param>
    public void Initialize(Position cellPosition, bool isUpBlocked, bool isDownBlocked, bool isRightBlocked,
        bool isLeftBlocked)
    {
        CellPosition = cellPosition;
        _isBlocked[(int) Direction.Up] = isUpBlocked;
        _isBlocked[(int) Direction.Down] = isDownBlocked;
        _isBlocked[(int) Direction.Right] = isRightBlocked;
        _isBlocked[(int) Direction.Left] = isLeftBlocked;
    }


    /// <summary>
    /// Sets the cell as reachable.
    /// </summary>
    /// <param name="needJumping"></param>
    public void SetReachable(bool needJumping)
    {
        IsReachable = true;
        NeedJumping = needJumping;
        SetLayer(LayerMask.NameToLayer("HighlightSelectable"));
    }

    /// <summary>
    /// Sets the cell as unreachable
    /// </summary>
    public void ResetReachable()
    {
        IsReachable = false;
        SetLayer(LayerMask.NameToLayer("Default"));
    }


    /// <summary>
    /// Changes the cell's layer to the given <paramref name="layer"/>.
    /// </summary>
    /// <param name="layer">the new <paramref name="layer"/>.</param>
    private void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }
}