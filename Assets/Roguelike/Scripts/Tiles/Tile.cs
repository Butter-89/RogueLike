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
    public bool walkable;

    [SerializeField]
    private GameActor actorOnTile;
    [SerializeField]
    private Pickups pickupOnTile;

    private Color prevColor;

    private void Start()
    {
        actorOnTile = null;
        pickupOnTile = null;
        prevColor = GetComponentInChildren<Renderer>().material.color;
    }

    public void SpawnActor(GameObject i_prefab)
    {
        GameObject spawnedObj = Instantiate(i_prefab, room.transform) as GameObject;
        spawnedObj.transform.position = transform.position;
        actorOnTile = spawnedObj.GetComponent<GameActor>();
        actorOnTile.SetCurrentPosition(transform.position);
        actorOnTile.SetDestination(transform.position);
        actorOnTile.currentTile = this;
    }

    public void HighlightAdjacentTile()
    {
        room.GetTileAt(roomPosition + new Vector2Int(1,0)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(-1, 0)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(0, 1)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(0, -1)).HighlightTile();
    }

    public void HighlightTile()
    {
        GetComponentInChildren<Renderer>().material.color = Color.yellow;
    }

    public void QuitHighlight()     // ??? need a proper name for this function
    {
        GetComponentInChildren<Renderer>().material.color = prevColor;
    }
}

