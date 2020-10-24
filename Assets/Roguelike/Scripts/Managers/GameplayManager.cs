using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefab;
    [SerializeField]
    private DungeonBoard board;
    private Room currentRoom;
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "GameManager";
    }

    public void OnBoardGenerated()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<DungeonBoard>();
        if (board == null)
            throw new System.Exception("Dungeon Board Not Found");

        SpawnPlayer();
        SpawnEnemies();
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
        for(int i = 0; i < currentRoom.RoomDepth + 1; i++)
        {
            // genereate warriors
            for(int j = 0; j < warriors; j++)
            {
                SpawnEnemyActor(enemyPrefab[0]);
            }
            // generate archers
            for(int j = 0; j < maxArchers; j++)
            {

            }
        }
    }

    private void SpawnEnemyActor(GameObject i_prefab)
    {
        Tile spawnTile = currentRoom.FindFreeTile();
        if(spawnTile == null)
        {
            return;
        }
        TurnManager tm = GetComponent<TurnManager>();
        GameActor spawnedActor = spawnTile.SpawnActor(i_prefab).GetComponent<GameActor>();
        tm.RegisterEnemy(spawnedActor);
    }
}
