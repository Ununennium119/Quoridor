using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public bool IsGamePaused { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;

    private const float CellLeftDownPos = -8.4f;
    private const float CellSpacing = 2.1f;
    private readonly CellController[,] _cellControllers = new CellController[9, 9];
    private readonly List<CellController> _reachableCells = new List<CellController>();

    private const float WallCellHorizontalOffset = 8.4f;
    private const float WallCellVerticalOffset = 11.5f;
    private const float WallCellSpacing = 2.1f;
    private readonly CellController[,] _wallCellControllers = new CellController[4, 9];

    [NonSerialized] public PlayerNumber CurrentPlayerNumber = PlayerNumber.PlayerOne;
    public int PlayersCount { get; private set; }
    private PlayerController[] _playerControllers;

    [NonSerialized] public WallController SelectedWall;
    private const float WallHorizontalOffset = 9.45f;
    private const float WallVerticalOffset = 11.5f;
    private const float WallSpacing = 2.1f;
    private int _wallsCount;

    private const float WallPlaceLeftDownPos = -7.35f;
    private const float WallPlaceSpacing = 2.1f;
    private readonly WallPlaceController[,] _wallPlaceControllers = new WallPlaceController[8, 8];
    private WallPlaceController _activeWallPlaceController;

    private GhostWallController _ghostWallController;

    [Header("Game Settings")]
    [SerializeField] private GameType gameType;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject wallCellPrefab;
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallPlacePrefab;
    [SerializeField] private GameObject ghostWallPrefab;
    
    [Header("Parents")]
    [SerializeField] private GameObject cellsParent;
    [SerializeField] private GameObject wallCellsParent;
    [SerializeField] private GameObject playersParent;
    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject wallPlacesParent;
    
    [Header("Events")]
    [SerializeField] private UnityEvent<string> turnChanged;
    [SerializeField] private UnityEvent<ScoreboardData[]> updateScoreboard;
    [SerializeField] private UnityEvent gameOver;
    

    private void Awake()
    {
        PlayersCount = GlobalVariables.PlayersNicknames.Length;
        gameType = PlayersCount == 2 ? GameType.TwoPlayers : GameType.FourPlayers;
        _wallsCount = gameType == GameType.TwoPlayers ? 10 : 5;
    }

    private void Start()
    {
        GameObject ghostWall = Instantiate(ghostWallPrefab, wallsParent.transform);
        _ghostWallController = ghostWall.GetComponent<GhostWallController>();

        CreateCells();
        CreateWallCells();
        CreatePlayers();
        CreateWalls();
        CreateWallPlaces();
        
        updateScoreboard.Invoke(GetScoreboardData());
    }


    private void CreateCells()
    {
        float currentXPos = CellLeftDownPos;
        float currentZPos = CellLeftDownPos;
        for (int z = 0; z < 9; z++)
        {
            for (int x = 0; x < 9; x++)
            {
                GameObject cell = Instantiate(cellPrefab, cellsParent.transform);
                cell.transform.position = new Vector3(currentXPos, cell.transform.position.y, currentZPos);

                CellController cellController = cell.GetComponent<CellController>();
                cellController.Initialize(new Position(x, z), z >= 8, z <= 0, x >= 8, x <= 0);
                _cellControllers[x, z] = cellController;

                currentXPos += CellSpacing;
            }

            currentZPos += CellSpacing;
            currentXPos = CellLeftDownPos;
        }
    }


    private void CreateWallCells()
    {
        CreateWallCellRow(WallCellVerticalOffset, 9, 0);
        CreateWallCellRow(-WallCellVerticalOffset, -1, 1);
        CreateWallCellColumn(WallCellVerticalOffset, 9, 2);
        CreateWallCellColumn(-WallCellVerticalOffset, -1, 3);
    }

    private void CreateWallCellRow(float startZ, int positionZ, int rowIndex)
    {
        float currentX = -WallCellHorizontalOffset;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab, wallCellsParent.transform);
            wallCell.transform.position = new Vector3(currentX, wallCell.transform.position.y, startZ);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(i, positionZ), true, true, true, true);
            _wallCellControllers[rowIndex, i] = wallCellController;

            currentX += WallCellSpacing;
        }
    }

    private void CreateWallCellColumn(float startX, int positionX, int columnIndex)
    {
        float currentZ = -WallCellHorizontalOffset;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab, wallCellsParent.transform);
            wallCell.transform.position = new Vector3(startX, wallCell.transform.position.y, currentZ);
            wallCell.transform.Rotate(0, 90, 0);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(positionX, i), true, true, true, true);
            _wallCellControllers[columnIndex, i] = wallCellController;

            currentZ += WallCellSpacing;
        }
    }


    private void CreatePlayers()
    {
        _playerControllers = new PlayerController[PlayersCount];
        for (int i = 0; i < PlayersCount; i++)
        {
            PlayerController playerController =
                Instantiate(playerPrefabs[i], playersParent.transform).GetComponent<PlayerController>();
            playerController.Nickname = GlobalVariables.PlayersNicknames[i];
            Position position = playerController.PlayerPosition;
            _playerControllers[i] = playerController;

            _cellControllers[position.X, position.Z].ContainsPlayer = true;
        }
    }


    private void CreateWalls()
    {
        CreateWallRow(-WallHorizontalOffset, -WallVerticalOffset, WallSpacing, PlayerNumber.PlayerOne);
        CreateWallRow(WallHorizontalOffset, WallVerticalOffset, -WallSpacing, PlayerNumber.PlayerTwo);
        if (gameType == GameType.FourPlayers)
        {
            CreateWallColumn(-WallVerticalOffset, WallHorizontalOffset, -WallSpacing, PlayerNumber.PlayerThree);
            CreateWallColumn(WallVerticalOffset, -WallHorizontalOffset, WallSpacing, PlayerNumber.PlayerFour);
        }
    }

    private void CreateWallRow(float startX, float startZ, float xChange, PlayerNumber ownerNumber)
    {
        float currentX = startX;
        for (int i = 0; i < _wallsCount; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallsParent.transform);
            wall.transform.position = new Vector3(currentX, wall.transform.position.y, startZ);
            wall.GetComponent<WallController>().Initialize(ownerNumber);

            currentX += xChange;
        }
    }

    private void CreateWallColumn(float startX, float startZ, float zChange, PlayerNumber ownerNumber)
    {
        float currentZ = startZ;
        for (int i = 0; i < _wallsCount; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallsParent.transform);
            wall.transform.position = new Vector3(startX, wall.transform.position.y, currentZ);
            wall.transform.Rotate(0, 90, 0);
            wall.GetComponent<WallController>().Initialize(ownerNumber);

            currentZ += zChange;
        }
    }


    private void CreateWallPlaces()
    {
        float currentXPos = WallPlaceLeftDownPos;
        float currentZPos = WallPlaceLeftDownPos;
        for (int z = 0; z < 8; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject wallPlace = Instantiate(wallPlacePrefab, wallPlacesParent.transform);
                wallPlace.transform.position = new Vector3(currentXPos, wallPlace.transform.position.y, currentZPos);

                WallPlaceController wallPlaceController = wallPlace.GetComponent<WallPlaceController>();
                wallPlaceController.Initialize(new Position(x, z));
                _wallPlaceControllers[x, z] = wallPlaceController;

                currentXPos += WallPlaceSpacing;
            }

            currentZPos += WallPlaceSpacing;
            currentXPos = WallPlaceLeftDownPos;
        }
    }


    [UsedImplicitly]
    public void OnSelectPlayer(InputAction.CallbackContext context)
    {
        if (!IsGameOver)
        {
            if (context.started)
            {
                PlayerController currentPlayerController = _playerControllers[(int) CurrentPlayerNumber];
                if (currentPlayerController.CanBeInteractedWith())
                {
                    currentPlayerController.Select();
                    ShowReachableCells(currentPlayerController.PlayerPosition);
                }
            }
        }
    }

    [UsedImplicitly]
    public void OnDeselect(InputAction.CallbackContext context)
    {
        if (!IsGameOver)
        {
            if (context.started)
            {
                PlayerController player = _playerControllers[(int) CurrentPlayerNumber];
                if (player.IsSelected)
                {
                    player.Deselect();
                    HideSelectableCells();
                }
                else if (SelectedWall != null)
                {
                    DeselectWall();
                }
            }
        }
    }

    [UsedImplicitly]
    public void OnRotateWall(InputAction.CallbackContext context)
    {
        if (!IsGameOver)
        {
            if (context.started)
            {
                if (_ghostWallController.gameObject.activeSelf)
                {
                    _ghostWallController.Rotate();
                    _ghostWallController.SetValid(IsWallPlaceValid());
                }
            }
        }
    }

    public void OnPlaceWall()
    {
        if (!IsGameOver)
        {
            if (_activeWallPlaceController != null && _ghostWallController.IsValid)
            {
                PlaceWall();
            }
        }
    }


    public void ShowReachableCells(Position originPosition)
    {
        CellController cellController = _cellControllers[originPosition.X, originPosition.Z];
        CheckDirection(cellController, Direction.Up, originPosition.Up());
        CheckDirection(cellController, Direction.Right, originPosition.Right());
        CheckDirection(cellController, Direction.Down, originPosition.Down());
        CheckDirection(cellController, Direction.Left, originPosition.Left());
    }

    private void CheckDirection(CellController cellController, Direction direction, Position targetPosition)
    {
        bool needJumping = false;
        while (!cellController.IsBlocked(direction))
        {
            {
                CellController reachableCell = _cellControllers[targetPosition.X, targetPosition.Z];
                if (reachableCell.ContainsPlayer)
                {
                    // if the cell contains a player, check the next cell.
                    cellController = reachableCell;
                    targetPosition = targetPosition.GetDirectionPosition(direction);
                    needJumping = true;
                }
                else
                {
                    reachableCell.SetReachable(needJumping);
                    _reachableCells.Add(reachableCell);
                    break;
                }
            }
        }

        // check if target position is a finishing cell
        if ((int) direction == (int) CurrentPlayerNumber)
        {
            CellController reachableCell = null;
            if (targetPosition.X < 0 || targetPosition.X > 8)
            {
                reachableCell = _wallCellControllers[(int) CurrentPlayerNumber, targetPosition.Z];
            }
            else if (targetPosition.Z < 0 || targetPosition.Z > 8)
            {
                reachableCell = _wallCellControllers[(int) CurrentPlayerNumber, targetPosition.X];
            }

            if (reachableCell != null)
            {
                reachableCell.SetReachable(needJumping);
                _reachableCells.Add(reachableCell);
            }
        }
    }

    private void HideSelectableCells()
    {
        foreach (CellController reachableCellController in _reachableCells)
        {
            reachableCellController.ResetReachable();
        }

        _reachableCells.Clear();
    }


    public bool IsCurrentPlayerSelected()
    {
        return _playerControllers[(int) CurrentPlayerNumber].IsSelected;
    }

    public void MovePlayer(CellController targetCellController)
    {
        PlayerController playerController = _playerControllers[(int) CurrentPlayerNumber];
        Position cellPosition = targetCellController.CellPosition;
        CellController baseCellController =
            _cellControllers[playerController.PlayerPosition.X, playerController.PlayerPosition.Z];
        baseCellController.ContainsPlayer = false;
        if (cellPosition.X >= 0 && cellPosition.X <= 8 && cellPosition.Z >= 0 && cellPosition.Z <= 8)
        {
            targetCellController.ContainsPlayer = true;
        }
        else
        {
            playerController.IsFinished = true;
        }

        Vector3 targetPosition = new Vector3(
            cellPosition.X * CellSpacing + CellLeftDownPos,
            playerController.transform.position.y,
            cellPosition.Z * CellSpacing + CellLeftDownPos);
        playerController.Move(targetCellController.CellPosition, targetPosition, targetCellController.NeedJumping);


        HideSelectableCells();
        NextTurn();
    }


    private void DeselectWall()
    {
        SelectedWall.Deselect();
        SelectedWall = null;
        _activeWallPlaceController = null;
        _ghostWallController.gameObject.SetActive(false);
    }

    private void PlaceWall()
    {
        _activeWallPlaceController.ContainsWall = true;
        SelectedWall.PlaceWall(_ghostWallController.transform.position, _ghostWallController.transform.rotation);
        UpdateCells(_activeWallPlaceController.Position, true);
        DeselectWall();
        NextTurn();
    }


    public void SetActiveWallPlace(WallPlaceController wallPlaceController)
    {
        if (SelectedWall != null)
        {
            _activeWallPlaceController = wallPlaceController;
            _ghostWallController.Move(wallPlaceController.transform.position);
            _ghostWallController.SetValid(IsWallPlaceValid());
        }
    }

    private bool IsWallPlaceValid()
    {
        if (_activeWallPlaceController == null)
        {
            return false;
        }

        int wallPlaceX = _activeWallPlaceController.Position.X;
        int wallPlaceZ = _activeWallPlaceController.Position.Z;
        if (_wallPlaceControllers[wallPlaceX, wallPlaceZ].ContainsWall)
        {
            return false;
        }

        bool result;
        if (_ghostWallController.IsHorizontal)
        {
            result = !_cellControllers[wallPlaceX, wallPlaceZ].IsBlocked(Direction.Up) &&
                     !_cellControllers[wallPlaceX + 1, wallPlaceZ].IsBlocked(Direction.Up);
        }
        else
        {
            result = !_cellControllers[wallPlaceX, wallPlaceZ].IsBlocked(Direction.Right) &&
                     !_cellControllers[wallPlaceX, wallPlaceZ + 1].IsBlocked(Direction.Right);
        }

        if (result)
        {
            UpdateCells(_activeWallPlaceController.Position, true);
            for (int i = 0; i < PlayersCount; i++)
            {
                PlayerController playerController = _playerControllers[i];
                if (playerController.IsFinished) continue;
                if (!HasPathToEnd((PlayerNumber) i, playerController.PlayerPosition))
                {
                    UpdateCells(_activeWallPlaceController.Position, false);
                    return false;
                }
            }

            UpdateCells(_activeWallPlaceController.Position, false);
            return true;
        }

        return false;
    }


    private bool HasPathToEnd(PlayerNumber playerNumber, Position position)
    {
        if (IsFinishPosition(playerNumber, position))
        {
            return true;
        }

        bool[,] isVisitedArray = new bool[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                isVisitedArray[j, i] = false;
            }
        }

        isVisitedArray[position.X, position.Z] = true;
        Queue<CellController> nextControllers = new Queue<CellController>();
        nextControllers.Enqueue(_cellControllers[position.X, position.Z]);
        while (nextControllers.Count != 0)
        {
            CellController currentController = nextControllers.Dequeue();
            if (!currentController.IsBlocked(Direction.Up))
            {
                CellController controller =
                    _cellControllers[currentController.CellPosition.X, currentController.CellPosition.Z + 1];
                if (CheckCell(playerNumber, isVisitedArray, controller, nextControllers)) return true;
            }

            if (!currentController.IsBlocked(Direction.Down))
            {
                CellController controller =
                    _cellControllers[currentController.CellPosition.X, currentController.CellPosition.Z - 1];
                if (CheckCell(playerNumber, isVisitedArray, controller, nextControllers)) return true;
            }

            if (!currentController.IsBlocked(Direction.Right))
            {
                CellController controller =
                    _cellControllers[currentController.CellPosition.X + 1, currentController.CellPosition.Z];
                if (CheckCell(playerNumber, isVisitedArray, controller, nextControllers)) return true;
            }

            if (!currentController.IsBlocked(Direction.Left))
            {
                CellController controller =
                    _cellControllers[currentController.CellPosition.X - 1, currentController.CellPosition.Z];
                if (CheckCell(playerNumber, isVisitedArray, controller, nextControllers)) return true;
            }
        }

        return false;
    }

    private static bool CheckCell(PlayerNumber playerNumber, bool[,] isVisitedArray, CellController controller,
        Queue<CellController> nextControllers)
    {
        if (!isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z])
        {
            isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z] = true;
            if (IsFinishPosition(playerNumber, controller.CellPosition))
            {
                return true;
            }

            nextControllers.Enqueue(controller);
        }

        return false;
    }

    private static bool IsFinishPosition(PlayerNumber playerNumber, Position position)
    {
        return playerNumber switch
        {
            PlayerNumber.PlayerOne => position.Z == 8,
            PlayerNumber.PlayerTwo => position.Z == 0,
            PlayerNumber.PlayerThree => position.X == 8,
            PlayerNumber.PlayerFour => position.X == 0,
            _ => false
        };
    }

    private void UpdateCells(Position placedWallPosition, bool isBlocked)
    {
        int placedWallX = placedWallPosition.X;
        int placedWallZ = placedWallPosition.Z;
        if (_ghostWallController.IsHorizontal)
        {
            _cellControllers[placedWallX, placedWallZ].SetBlocked(Direction.Up, isBlocked);
            _cellControllers[placedWallX + 1, placedWallZ].SetBlocked(Direction.Up, isBlocked);
            _cellControllers[placedWallX, placedWallZ + 1].SetBlocked(Direction.Down, isBlocked);
            _cellControllers[placedWallX + 1, placedWallZ + 1].SetBlocked(Direction.Down, isBlocked);
        }
        else
        {
            _cellControllers[placedWallX, placedWallZ].SetBlocked(Direction.Right, isBlocked);
            _cellControllers[placedWallX, placedWallZ + 1].SetBlocked(Direction.Right, isBlocked);
            _cellControllers[placedWallX + 1, placedWallZ].SetBlocked(Direction.Left, isBlocked);
            _cellControllers[placedWallX + 1, placedWallZ + 1].SetBlocked(Direction.Left, isBlocked);
        }
    }


    private void NextTurn()
    {
        _playerControllers[(int) CurrentPlayerNumber].IncrementMovesCount();
        int cycleCount = 0;
        do
        {
            CurrentPlayerNumber = (PlayerNumber) (((int) CurrentPlayerNumber + 1) % PlayersCount);
            cycleCount += 1;
        } while (_playerControllers[(int) CurrentPlayerNumber].IsFinished && cycleCount <= PlayersCount);

        updateScoreboard.Invoke(GetScoreboardData());

        if (cycleCount > PlayersCount)
        {
            IsGameOver = true;
            gameOver.Invoke();
        }
        else
        {
            turnChanged.Invoke(_playerControllers[(int) CurrentPlayerNumber].Nickname);
        }
    }


    public void PauseGame()
    {
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
    }


    private ScoreboardData[] GetScoreboardData()
    {
        ScoreboardData[] scoreboardData = new ScoreboardData[PlayersCount];
        for (int i = 0; i < PlayersCount; i++)
        {
            PlayerController playerController = _playerControllers[i];

            string status;
            if (playerController.IsFinished)
            {
                status = "Finished";
            }
            else if ((int) CurrentPlayerNumber == i)
            {
                status = "Active";
            }
            else
            {
                status = "Waiting";
            }

            scoreboardData[i] =
                new ScoreboardData(playerController.Nickname, playerController.MovesCount.ToString(), status);
        }

        return scoreboardData;
    }
}