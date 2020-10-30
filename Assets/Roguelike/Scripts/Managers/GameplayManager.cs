using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefab;
    public GameObject pickupPrefab;
    public GameObject upgradePrefab;
    public Material lavaMaterial;
    public DungeonGenerator generator;
    [SerializeField]
    private DungeonBoard board;
    private Room currentRoom;
    private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "GameManager";
        canvasGroup = GameObject.Find("UpgradeMenu").GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnBoardGenerated()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<DungeonBoard>();
        if (board == null)
            throw new System.Exception("Dungeon Board Not Found");

        SpawnPlayer();
        SpawnEnemies();
        //SpawnCollectibles();
        SpawnUpgrade();
        SpawnLava();
        MoveCamera();
    }

    public void EnterNewRoom(Room i_newRoom)
    {
        currentRoom = i_newRoom;
        if (i_newRoom.RoomDepth == generator.victoryDepth)
        {
            // setup a victory tile
            Tile vTile = i_newRoom.FindFreeTile();
            vTile.gameObject.AddComponent<VictoryTile>();
            vTile.GetComponentInChildren<Renderer>().material.color = Color.red;
        }
        SpawnEnemies();
        SpawnCollectibles();
        SpawnUpgrade();
        SpawnLava();
        // move the camera in place
        MoveCamera();
    }

    private void SpawnPlayer()
    {
        Room startRoom = board.rooms[0];
        currentRoom = startRoom;
        Tile spawnTile = startRoom.GetTileAt(new Vector2Int(4, 4));
        TurnManager tm = GetComponent<TurnManager>();
        tm.Player = spawnTile.SpawnActor(playerPrefab).GetComponent<GameActor>();
        tm.PlayerTurnStart();
    }

    private void SpawnEnemies()
    {
        currentRoom = GetComponent<TurnManager>().Player.currentTile.room;
        int maxArchers = (currentRoom.RoomDepth + 1) / 3;
        int warriors = currentRoom.RoomDepth + 1 - maxArchers;
        // genereate warriors
        for (int j = 0; j < warriors; j++)
        {
            SpawnEnemyActor(enemyPrefab[0]);
        }
        // generate archers
        for (int j = 0; j < maxArchers; j++)
        {
            SpawnEnemyActor(enemyPrefab[1]);
        }
    }

    private void SpawnEnemyActor(GameObject i_prefab)
    {
        Tile spawnTile = currentRoom.FindFreeTile();
        spawnTile.Occupied = true;
        if(spawnTile == null)
        {
            return;
        }
        TurnManager tm = GetComponent<TurnManager>();
        GameActor spawnedActor = spawnTile.SpawnActor(i_prefab).GetComponent<GameActor>();
        tm.RegisterEnemy(spawnedActor);
    }

    private void SpawnCollectibles()
    {
        Tile spawnTile = currentRoom.FindFreeTile();
        Pickups spawned = spawnTile.SpawnPickup(pickupPrefab);
        TurnManager tm = GetComponent<TurnManager>();
        tm.RegisterPickup(spawned);
    }

    private void SpawnUpgrade()
    {
        Tile spawnTile = currentRoom.FindFreeTile();
        Upgrades spawned = spawnTile.SpawnUpgrade(upgradePrefab);
        TurnManager tm = GetComponent<TurnManager>();
        tm.RegisterUpgrade(spawned);
    }

    private void SpawnLava()
    {
        for(int i = 0; i < currentRoom.RoomDepth / 2; i++)
        {
            Tile availableTile = currentRoom.FindFreeTile();
            if(availableTile)
            {
                availableTile.gameObject.AddComponent<Lava>();
                availableTile.GetComponentInChildren<Renderer>().material = new Material(lavaMaterial);
            }
        }
    }

    private void MoveCamera()
    {
        GameObject.Find("Depth").GetComponent<Text>().text = "Current Depth: " + currentRoom.RoomDepth;

        Vector2Int roomCenterPosition = currentRoom.originBoardPosition + new Vector2Int(4, 4); //currentRoom.tiles[currentRoom.tiles.Count / 2].roomPosition;
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cam.transform.position = new Vector3(roomCenterPosition.x, cam.transform.position.y, roomCenterPosition.y);
    }

}
