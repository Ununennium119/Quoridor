using UnityEngine;
using UnityEngine.EventSystems;

public class WallPlaceController : MonoBehaviour, IPointerEnterHandler
{
    private GameManager _gameManager;
    private Position _position;
    private bool _containsWall;

    /// <value>Represents the wall-place's position in board.</value>
    public Position Position
    {
        get => _position;
        set => _position = value;
    }

    /// <value>Specifies whether the wall-place contains a wall or not.</value>
    public bool ContainsWall
    {
        get => _containsWall;
        set => _containsWall = value;
    }
    
    
    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Initializes necessary fields of the wall-place.
    /// Must be called after creating an instance.
    /// </summary>
    /// <param name="wallPlacePosition"></param>
    public void Initialize(Position wallPlacePosition)
    {
        Position = wallPlacePosition;
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        _gameManager.SetActiveWallPlace(this);
    }
}
