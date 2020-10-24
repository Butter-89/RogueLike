using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int originBoardPosition;
    public List<Tile> tiles = new List<Tile>();
    public List<Door> doors = new List<Door>();
    [TextArea]
    public string roomStr;

    private List<Vector2Int> tilePositions = new List<Vector2Int>();
    public List<Vector2Int> TilePositions => tilePositions;

    private List<TempTileData> tempTileDatas = new List<TempTileData>();
    public List<TempTileData> TempTileDatas => tempTileDatas;

    private List<TempDoorData> tempDoorDatas = new List<TempDoorData>();
    public List<TempDoorData> TempDoorDatas => tempDoorDatas;

    private List<Tile> highlightedTiles = new List<Tile>();
    public List<Tile> HighlightedTiles => highlightedTiles;

    private TurnManager turnManager;
    public TurnManager TurnManager => turnManager;

    public int RoomDepth { get; set; }

    private void Start()
    {
        turnManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        if (turnManager == null)
            throw new System.Exception("Turn manager not found for room " + transform.name);
    }

    public Door FindDoorWithDir(char i_dir)
    {
        foreach(Door door in doors)
        {
            if (door.direction == i_dir)
                return door;
        }

        return null;
    }
    public TempDoorData FindTempDoorWithDir(char i_dir)
    {
        foreach (TempDoorData door in tempDoorDatas)
        {
            if (door.direction == i_dir)
                return door;
        }

        return null;
    }

    public void AddTilePosData(Vector2Int i_roomPos)
    {
        tilePositions.Add(i_roomPos);
    }

    public Tile FindFreeTile()
    {
        for(int i = 0; i < 100; i++)
        {
            Tile randomTile = tiles[Random.Range(0, tiles.Count - 1)];
            if(randomTile.IsReachable() && randomTile.PickupOnTile == null)
            {
                return randomTile;
            }
        }
        Debug.LogError("Exceeds max tries to find free tile");
        return null;
    }

    public Tile GetTileAt(Vector2Int i_position)
    {
        Tile tileFound = null;
        foreach(Tile tile in tiles)
        {
            if (tile.roomPosition == i_position)
                tileFound = tile;
        }
        return tileFound;
    }

    public void ResetHighlightedTiles()
    {
        for(int i = HighlightedTiles.Count - 1; i >= 0; i--)
        {
            HighlightedTiles[i].Unhighlight();
        }

    }
}
