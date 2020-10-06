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

    public Door FindDoorWithDir(char i_dir)
    {
        foreach(Door door in doors)
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

    
}
