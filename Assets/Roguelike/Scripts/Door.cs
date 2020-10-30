using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDoorData
{
    public char direction;
    public Vector2Int roomPostion;
}

public class Door : MonoBehaviour
{
    public Room room;   // may not be used
    public Door connectedDoor;
    public Tile tile;
    public char direction;

    public void ToNextRoom(GameActor i_player)
    {
        i_player.currentTile.room.ResetHighlightedTiles();
        i_player.currentTile.room.ClearRoom();
        // move player to the next room
        Vector2Int startPosition = connectedDoor.tile.roomPosition;
        switch(connectedDoor.direction)
        {
            case 'S':
                startPosition += new Vector2Int(0, -1);
                break;
            case 'N':
                startPosition += new Vector2Int(0, 1);
                break;
            case 'E':
                startPosition += new Vector2Int(-1, 0);
                break;
            case 'W':
                startPosition += new Vector2Int(1, 0);
                break;
        }

        i_player.currentTile = connectedDoor.room.GetTileAt(startPosition);
        i_player.transform.SetParent(i_player.currentTile.room.transform);

        i_player.SetDestination(new Vector3(
            connectedDoor.tile.room.originBoardPosition.x + startPosition.x, 
            0, 
            connectedDoor.tile.room.originBoardPosition.y + startPosition.y));

        i_player.mana = i_player.MaxMana;
        // set up room and manager references
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameplayManager>().EnterNewRoom(i_player.currentTile.room);

        // 
    }
}
