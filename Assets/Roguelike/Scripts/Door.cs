using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Room room;   // may not be used
    public Door connectedDoor;
    public Tile tile;
    public char direction;
}
