using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTileData
{
    public char tileType;
    public Room room;   // reference to the room it belongs to
    public Vector2Int roomPosition;
    public TempDoorData doorData;
}

public class Tile : MonoBehaviour
{
    public char tileType;
    public Room room;   // reference to the room it belongs to
    public Vector2Int roomPosition;

    
}
