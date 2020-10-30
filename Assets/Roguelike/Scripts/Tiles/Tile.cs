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
    public bool walkable;   // if an actor can step on the tile

    public GameActor actorOnTile;
    [SerializeField]
    private Pickups pickupOnTile;
    public Pickups PickupOnTile => pickupOnTile;
    public Upgrades upgradeOnTile;
    public bool Reachable { get; set; } // can the player actor reach the tile
    public bool Occupied { get; set; }  // is the tile occupied by actor or pickup?
    private Color prevColor;

    private void Start()
    {
        actorOnTile = null;
        pickupOnTile = null;
        Occupied = false;
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

    public Pickups SpawnPickup(GameObject i_prefab)
    {
        GameObject spawnedObj = Instantiate(i_prefab, room.transform);
        spawnedObj.transform.position = transform.position;
        Pickups spawnedPickup = spawnedObj.GetComponent<Pickups>();
        spawnedPickup.currentTile = this;
        pickupOnTile = spawnedObj.GetComponent<Pickups>();

        return spawnedPickup;
    }

    public Upgrades SpawnUpgrade(GameObject i_prefab)
    {
        GameObject spawnedObj = Instantiate(i_prefab, room.transform);
        spawnedObj.transform.position = transform.position;
        Upgrades spawnedUpgrade = spawnedObj.GetComponent<Upgrades>();
        spawnedUpgrade.currentTile = this;
        upgradeOnTile = spawnedUpgrade.GetComponent<Upgrades>();
        //pickupOnTile = spawnedObj.GetComponent<Pickups>();

        return spawnedUpgrade;
    }

    public void HighlightAdjacentTile()
    {
        room.GetTileAt(roomPosition + new Vector2Int(1,0)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(-1, 0)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(0, 1)).HighlightTile();
        room.GetTileAt(roomPosition + new Vector2Int(0, -1)).HighlightTile();
    }

    public void HighlightTilesInRange(int i_range)
    {
        if(room.GetTileAt(roomPosition + new Vector2Int(i_range, 0)))
            room.GetTileAt(roomPosition + new Vector2Int(i_range, 0)).HighlightTile();
        if (room.GetTileAt(roomPosition + new Vector2Int(-i_range, 0)))
            room.GetTileAt(roomPosition + new Vector2Int(-i_range, 0)).HighlightTile();
        if (room.GetTileAt(roomPosition + new Vector2Int(0, i_range)))
            room.GetTileAt(roomPosition + new Vector2Int(0, i_range)).HighlightTile();
        if (room.GetTileAt(roomPosition + new Vector2Int(0, -i_range)))
            room.GetTileAt(roomPosition + new Vector2Int(0, -i_range)).HighlightTile();
    }

    public void HighlightTile()
    {
        if (!walkable)
            return;
        Reachable = true;
        if(GetComponent<VictoryTile>() == null)
        {
            if(GetComponent<Door>())
            {
                foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
                {
                    renderer.material.color = Color.yellow;
                }
                if (!room.HighlightedTiles.Contains(this))
                    room.HighlightedTiles.Add(this);
                return;
            }

            GetComponentInChildren<Renderer>().material.color = Color.yellow;
            if (!room.HighlightedTiles.Contains(this))
                room.HighlightedTiles.Add(this);
        }
    }

    public void Unhighlight()
    {
        Reachable = false;
        if (GetComponent<Door>())
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = prevColor;
            }
            if (room.HighlightedTiles.Contains(this))
                room.HighlightedTiles.Remove(this);
            return;
        }
        GetComponentInChildren<Renderer>().material.color = prevColor;
        if (room.HighlightedTiles.Contains(this))
            room.HighlightedTiles.Remove(this);
    }

    public bool IsReachable() => Reachable && walkable;
}

