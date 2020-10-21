using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefab;
    [SerializeField]
    private DungeonBoard board;
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
    }

    void SpawnPlayer()
    {
        Room startRoom = board.rooms[0];
        Tile spawnTile = startRoom.GetTileAt(new Vector2Int(4, 4));
        spawnTile.SpawnActor(playerPrefab);
    }
}
