using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { Spear};

public class Pickups : MonoBehaviour
{
    public Tile currentTile;
    public PickupType type;
    public void OnPickUp()
    {
        currentTile.room.TurnManager.DeregisterPickup(this);
        Destroy(transform.gameObject);
    }
}
