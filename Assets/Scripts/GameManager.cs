using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float moveHeight = 0.2f;
    [SerializeField] private float jumpHeight = 1.3f;

    [SerializeField] private GameType gameType;
    public PlayerNumber currentPlayer = PlayerNumber.PlayerOne;
    private PlayerController[] _playerControllers;

    private const float CellInitPos = -8.4f;
    private const float CellSpacing = 2.1f;
    private readonly CellController[,] _cells = new CellController[9, 9];
    private readonly List<CellController> _selectableCells = new List<CellController>();
    [SerializeField] private GameObject cellPrefab;

    private const float WallCellInitDisplacement = 11.5f;
    private readonly CellController[,] _wallCells = new CellController[4, 9];
    [SerializeField] private GameObject wallCellPrefab;

    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private GameObject player3Prefab;
    [SerializeField] private GameObject player4Prefab;

    private const float WallInitPos = 9.45f;
    private const float WallInitDisplacement = 11.5f;
    private const float WallSpacing = 2.1f;
    [SerializeField] private GameObject wallPrefab;
    public WallController selectedWall;

    private const float WallPlaceLeftDownPos = -7.35f;
    private const float WallPlaceSpacing = 2.1f;
    private readonly WallPlaceController[,] _wallPlaceControllers = new WallPlaceController[8, 8];
    [SerializeField] private GameObject wallPlacePrefab;
    [SerializeField] private WallPlaceController activeWallPlaceController;

    [SerializeField] private GameObject ghostWall;

    private bool _isGhostWallHorizontal = false;
    private Material _ghostWallDefaultMaterial;
    [SerializeField] private Material ghostWallInvalidMaterial;


    private void Start()
    {
        _ghostWallDefaultMaterial = ghostWall.GetComponent<Renderer>().material;

        CreateCells();
        CreateWallCells();
        CreatePlayers();
        CreateWalls();
        CreateWallPlaces();
    }

    private void CreateCells()
    {
        float currentXPos = CellInitPos;
        float currentZPos = CellInitPos;
        for (int z = 0; z < 9; z++)
        {
            for (int x = 0; x < 9; x++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.transform.position = new Vector3(currentXPos, cell.transform.position.y, currentZPos);
                CellController cellController = cell.GetComponent<CellController>();
                cellController.Initialize(new Position(x, z), z >= 8, z <= 0, x >= 8, x <= 0);
                _cells[x, z] = cellController;

                currentXPos += CellSpacing;
            }

            currentZPos += CellSpacing;
            currentXPos = CellInitPos;
        }
    }

    private void CreateWallCells()
    {
        float currentX = CellInitPos;
        float currentZ = WallCellInitDisplacement;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab);
            wallCell.transform.position = new Vector3(currentX, wallCell.transform.position.y, currentZ);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(i, 9),true, true, true, true);
            _wallCells[0, i] = wallCellController;

            currentX += CellSpacing;
        }

        currentX = CellInitPos;
        currentZ = -WallCellInitDisplacement;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab);
            wallCell.transform.position = new Vector3(currentX, wallCell.transform.position.y, currentZ);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(i, -1),true, true, true, true);
            _wallCells[1, i] = wallCellController;

            currentX += CellSpacing;
        }

        currentX = WallCellInitDisplacement;
        currentZ = CellInitPos;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab);
            wallCell.transform.position = new Vector3(currentX, wallCell.transform.position.y, currentZ);
            wallCell.transform.Rotate(0, 90, 0);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(9, i),true, true, true, true);
            _wallCells[2, i] = wallCellController;

            currentZ += CellSpacing;
        }

        currentX = -WallCellInitDisplacement;
        currentZ = CellInitPos;
        for (int i = 0; i < 9; i++)
        {
            GameObject wallCell = Instantiate(wallCellPrefab);
            wallCell.transform.position = new Vector3(currentX, wallCell.transform.position.y, currentZ);
            wallCell.transform.Rotate(0, 90, 0);

            CellController wallCellController = wallCell.GetComponent<CellController>();
            wallCellController.Initialize(new Position(-1, i),true, true, true, true);
            _wallCells[3, i] = wallCellController;

            currentZ += CellSpacing;
        }
    }

    private void CreatePlayers()
    {
        _playerControllers = new PlayerController[gameType == GameType.FourPlayers ? 4 : 2];
        _playerControllers[0] = Instantiate(player1Prefab).GetComponent<PlayerController>();
        _playerControllers[1] = Instantiate(player2Prefab).GetComponent<PlayerController>();
        if (gameType == GameType.FourPlayers)
        {
            _playerControllers[2] = Instantiate(player3Prefab).GetComponent<PlayerController>();
            _playerControllers[3] = Instantiate(player4Prefab).GetComponent<PlayerController>();
        }
        
        foreach (PlayerController playerController in _playerControllers)
        {
            Position position = playerController.PlayerPosition;
            _cells[position.X, position.Z].ContainsPlayer = true;
        }
    }

    private void CreateWalls()
    {
        int wallsCount = (gameType == GameType.TwoPlayers) ? 10 : 5;

        PlayerNumber ownerNumber = PlayerNumber.PlayerOne;
        float currentX = -WallInitPos;
        float currentZ = -WallInitDisplacement;
        for (int i = 0; i < wallsCount; i++)
        {
            GameObject wall = Instantiate(wallPrefab);
            wall.transform.position = new Vector3(currentX, wall.transform.position.y, currentZ);
            wall.GetComponent<WallController>().Initialize(ownerNumber);

            currentX += WallSpacing;
        }

        ownerNumber = PlayerNumber.PlayerTwo;
        currentX = WallInitPos;
        currentZ = WallInitDisplacement;
        for (int i = 0; i < wallsCount; i++)
        {
            GameObject wall = Instantiate(wallPrefab);
            wall.transform.position = new Vector3(currentX, wall.transform.position.y, currentZ);
            wall.GetComponent<WallController>().Initialize(ownerNumber);

            currentX -= WallSpacing;
        }

        if (gameType == GameType.FourPlayers)
        {
            ownerNumber = PlayerNumber.PlayerThree;
            currentX = -WallInitDisplacement;
            currentZ = WallInitPos;
            for (int i = 0; i < wallsCount; i++)
            {
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.position = new Vector3(currentX, wall.transform.position.y, currentZ);
                wall.transform.Rotate(0, 90, 0);
                wall.GetComponent<WallController>().Initialize(ownerNumber);

                currentZ -= WallSpacing;
            }

            ownerNumber = PlayerNumber.PlayerFour;
            currentX = WallInitDisplacement;
            currentZ = -WallInitPos;
            for (int i = 0; i < wallsCount; i++)
            {
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.position = new Vector3(currentX, wall.transform.position.y, currentZ);
                wall.transform.Rotate(0, 90, 0);
                wall.GetComponent<WallController>().Initialize(ownerNumber);

                currentZ += WallSpacing;
            }
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
                GameObject wallPlace = Instantiate(wallPlacePrefab);
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
    private void OnSelectPlayer()
    {
        PlayerController currentPlayerController = _playerControllers[(int) currentPlayer];
        if (currentPlayerController.CanBeInteractedWith())
        {
            currentPlayerController.Select();
        }
    }

    [UsedImplicitly]
    private void OnDeselect()
    {
        PlayerController player = _playerControllers[(int) currentPlayer];
        if (player.IsSelected)
        {
            player.Deselect();
        } else if (selectedWall != null)
        {
            DeselectWall();
        }
    }

    [UsedImplicitly]
    private void OnRotateWall()
    {
        if (ghostWall.activeSelf)
        {
            ghostWall.transform.Rotate(0, 90, 0);
            _isGhostWallHorizontal = !_isGhostWallHorizontal;
            ValidateGhostWall();
        }
    }

    [UsedImplicitly]
    private void OnPlaceWall()
    {
        if (activeWallPlaceController != null && IsWallPlaceValid())
        {
            PlaceWall();
        }
    }


    public void InitPlayerPosition(Position playerPosition)
    {
        _cells[playerPosition.X, playerPosition.Z].ContainsPlayer = true;
    }


    public void ShowSelectableCells(Position originPosition)
    {
        CellController cellController = _cells[originPosition.X, originPosition.Z];
        CheckDirection(cellController, Direction.Up, originPosition.Up());
        CheckDirection(cellController, Direction.Right, originPosition.Right());
        CheckDirection(cellController, Direction.Down, originPosition.Down());
        CheckDirection(cellController, Direction.Left, originPosition.Left());
    }

    private void CheckDirection(CellController cellController, Direction direction, Position targetPosition)
    {
        bool isJumping = false;
        while (!cellController.IsBlocked(direction))
        {
            {
                CellController selectableCell = _cells[targetPosition.X, targetPosition.Z];
                if (selectableCell.ContainsPlayer)
                {
                    cellController = selectableCell;
                    targetPosition = targetPosition.GetDirectionPosition(direction);
                    isJumping = true;
                }
                else
                {
                    selectableCell.IsReachable = true;
                    selectableCell.NeedJumping = isJumping;
                    selectableCell.SetLayer(LayerMask.NameToLayer("HighlightSelectable"));
                    _selectableCells.Add(selectableCell);
                    break;
                }
            }
        }

        if ((int) direction == (int) currentPlayer)
        {
            CellController selectableCell = null;
            if (targetPosition.X < 0 || targetPosition.X > 8)
            {
                selectableCell = _wallCells[(int) currentPlayer, targetPosition.Z];
            }
            else if (targetPosition.Z < 0 || targetPosition.Z > 8)
            {
                selectableCell = _wallCells[(int) currentPlayer, targetPosition.X];
            }

            if (selectableCell != null)
            {
                selectableCell.IsReachable = true;
                selectableCell.NeedJumping = isJumping;
                selectableCell.SetLayer(LayerMask.NameToLayer("HighlightSelectable"));
                _selectableCells.Add(selectableCell);
            }
        }
    }

    public void HideSelectableCells()
    {
        foreach (CellController cellController in _selectableCells)
        {
            cellController.IsReachable = false;
            cellController.SetLayer(LayerMask.NameToLayer("Default"));
        }

        _selectableCells.Clear();
    }


    public bool IsCurrentPlayerSelected()
    {
        return _playerControllers[(int) currentPlayer].IsSelected;
    }

    public void MovePlayer(Position cellPosition, bool isJumping)
    {
        PlayerController playerController = _playerControllers[(int) currentPlayer];
        Vector3 targetPosition = new Vector3(
            cellPosition.X * CellSpacing + CellInitPos,
            playerController.transform.position.y,
            cellPosition.Z * CellSpacing + CellInitPos);
        StartCoroutine(JumpOverSeconds(playerController.gameObject, targetPosition, 0.8f,
            isJumping ? jumpHeight : moveHeight));

        _cells[playerController.PlayerPosition.X, playerController.PlayerPosition.Z].ContainsPlayer = false;
        if (cellPosition.X >= 0 && cellPosition.X <= 8 && cellPosition.Z >= 0 && cellPosition.Z <= 8)
        {
            _cells[cellPosition.X, cellPosition.Z].ContainsPlayer = true;
        }
        else
        {
            playerController.IsFinished = true;
        }

        playerController.PlayerPosition = cellPosition;
        playerController.Deselect();
        HideSelectableCells();

        NextTurn();
    }

    private static IEnumerator JumpOverSeconds(GameObject objectToMove, Vector3 endPos, float seconds, float height)
    {
        Vector3 startPos = objectToMove.transform.position;
        float distance = Vector3.Distance(endPos, startPos);
        float elapsedTime = 0.0f;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position =
                Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, Easing.InOutSine(elapsedTime / seconds)));
            // float timeRatio = Mathf.SmoothStep(0.0f, seconds, elapsedTime) / seconds;
            float horizontalDistanceCovered = Vector3.Distance(startPos, objectToMove.transform.position);
            float distanceRatio = horizontalDistanceCovered / distance;
            objectToMove.transform.position += new Vector3(0, -4 * height * distanceRatio * (distanceRatio - 1), 0);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = endPos;
    }


    public void SelectWall(WallController wallController)
    {
        if (selectedWall == null)
        {
            _playerControllers[(int) currentPlayer].Deselect();

            wallController.Select();
            selectedWall = wallController;
        }
    }

    private void DeselectWall()
    {
        selectedWall.Deselect();
        selectedWall = null;
        activeWallPlaceController = null;
        ghostWall.SetActive(false);
    }

    private void PlaceWall()
    {
        activeWallPlaceController.ContainsWall = true;
        selectedWall.transform.position = ghostWall.transform.position;
        selectedWall.transform.rotation = ghostWall.transform.rotation;
        selectedWall.GetComponent<BoxCollider>().enabled = false;
        selectedWall.IsPlaced = true;
        UpdateCells(activeWallPlaceController.Position, true);
        DeselectWall();
        NextTurn();
    }


    public void SetActiveWallPlace(WallPlaceController wallPlaceController)
    {
        if (selectedWall != null)
        {
            ghostWall.SetActive(true);
            activeWallPlaceController = wallPlaceController;

            Vector3 position = wallPlaceController.gameObject.transform.position;
            ghostWall.transform.position = new Vector3(position.x, ghostWall.transform.position.y, position.z);

            ValidateGhostWall();
        }
    }

    private void ValidateGhostWall()
    {
        ghostWall.GetComponent<Renderer>().material =
            IsWallPlaceValid() ? _ghostWallDefaultMaterial : ghostWallInvalidMaterial;
    }

    private bool IsWallPlaceValid()
    {
        if (activeWallPlaceController == null)
        {
            return false;
        }

        int wallPlaceX = activeWallPlaceController.Position.X;
        int wallPlaceZ = activeWallPlaceController.Position.Z;
        if (_wallPlaceControllers[wallPlaceX, wallPlaceZ].ContainsWall)
        {
            return false;
        }

        bool result;
        if (_isGhostWallHorizontal)
        {
            result = !_cells[wallPlaceX, wallPlaceZ].IsBlocked(Direction.Up) &&
                     !_cells[wallPlaceX + 1, wallPlaceZ].IsBlocked(Direction.Up);
        }
        else
        {
            result = !_cells[wallPlaceX, wallPlaceZ].IsBlocked(Direction.Right) &&
                     !_cells[wallPlaceX, wallPlaceZ + 1].IsBlocked(Direction.Right);
        }

        if (result)
        {
            UpdateCells(activeWallPlaceController.Position, true);
            for (int i = 0; i < (gameType == GameType.FourPlayers ? 4 : 2); i++)
            {
                PlayerController playerController = _playerControllers[i];
                if (playerController.IsFinished) continue;
                if (!HasPathToEnd((PlayerNumber) i, playerController.PlayerPosition))
                {
                    UpdateCells(activeWallPlaceController.Position, false);
                    return false;
                }
            }

            UpdateCells(activeWallPlaceController.Position, false);
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
        nextControllers.Enqueue(_cells[position.X, position.Z]);
        while (nextControllers.Count != 0)
        {
            CellController currentController = nextControllers.Dequeue();
            if (!currentController.IsBlocked(Direction.Up))
            {
                CellController controller =
                    _cells[currentController.CellPosition.X, currentController.CellPosition.Z + 1];
                if (!isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z])
                {
                    isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z] = true;
                    if (IsFinishPosition(playerNumber, controller.CellPosition)) return true;

                    nextControllers.Enqueue(controller);
                }
            }

            if (!currentController.IsBlocked(Direction.Down))
            {
                CellController controller =
                    _cells[currentController.CellPosition.X, currentController.CellPosition.Z - 1];
                if (!isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z])
                {
                    isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z] = true;
                    if (IsFinishPosition(playerNumber, controller.CellPosition)) return true;

                    nextControllers.Enqueue(controller);
                }
            }

            if (!currentController.IsBlocked(Direction.Right))
            {
                CellController controller =
                    _cells[currentController.CellPosition.X + 1, currentController.CellPosition.Z];
                if (!isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z])
                {
                    isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z] = true;
                    if (IsFinishPosition(playerNumber, controller.CellPosition)) return true;

                    nextControllers.Enqueue(controller);
                }
            }

            if (!currentController.IsBlocked(Direction.Left))
            {
                CellController controller =
                    _cells[currentController.CellPosition.X - 1, currentController.CellPosition.Z];
                if (!isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z])
                {
                    isVisitedArray[controller.CellPosition.X, controller.CellPosition.Z] = true;
                    if (IsFinishPosition(playerNumber, controller.CellPosition)) return true;

                    nextControllers.Enqueue(controller);
                }
            }
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
        if (_isGhostWallHorizontal)
        {
            _cells[placedWallX, placedWallZ].SetBlocked(Direction.Up, isBlocked);
            _cells[placedWallX + 1, placedWallZ].SetBlocked(Direction.Up, isBlocked);
            _cells[placedWallX, placedWallZ + 1].SetBlocked(Direction.Down, isBlocked);
            _cells[placedWallX + 1, placedWallZ + 1].SetBlocked(Direction.Down, isBlocked);
        }
        else
        {
            _cells[placedWallX, placedWallZ].SetBlocked(Direction.Right, isBlocked);
            _cells[placedWallX, placedWallZ + 1].SetBlocked(Direction.Right, isBlocked);
            _cells[placedWallX + 1, placedWallZ].SetBlocked(Direction.Left, isBlocked);
            _cells[placedWallX + 1, placedWallZ + 1].SetBlocked(Direction.Left, isBlocked);
        }
    }


    private void NextTurn()
    {
        int playersCount = gameType == GameType.FourPlayers ? 4 : 2;
        int cycleCount = 0;
        do
        {
            currentPlayer = (PlayerNumber) (((int) currentPlayer + 1) % playersCount);
            cycleCount += 1;
        } while (_playerControllers[(int) currentPlayer].IsFinished && cycleCount <= playersCount);

        if (cycleCount > playersCount)
        {
            Debug.Log("Game Finished!");
        }
    }
}