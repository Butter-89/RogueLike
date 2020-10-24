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
    public Pickups PickupOnTile => pickupOnTile;
    private bool reachable;
    private Color prevColor;

    private void Start()
    {
        actorOnTile = null;
        pickupOnTile = null;
        prevColor = Color.white; //GetComponentInChildren<Renderer>().material.color;
    }

    public GameObject SpawnActor(GameObject i_prefab)
    {
        GameObject spawnedObj = Instantiate(i_prefab, room.transform);
        spawnedObj.transform.position = transform.position;
        actorOnTile = spawnedObj.GetComponent<GameActor>();
        actorOnTile.SetCurrentPosition(transform.position);
        actorOnTile.SetDestination(transform.position);
        actorOnTile.currentTile = this;

        return spawnedObj;
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
        reachable = true;
        GetComponentInChildren<Renderer>().material.color = Color.yellow;
        if (!room.HighlightedTiles.Contains(this))
            room.HighlightedTiles.Add(this);
    }

    public void Unhighlight()
    {
        reachable = false;
        GetComponentInChildren<Renderer>().material.color = prevColor;
        if (room.HighlightedTiles.Contains(this))
            room.HighlightedTiles.Remove(this);
    }

    public bool IsReachable() => reachable && walkable;
}

