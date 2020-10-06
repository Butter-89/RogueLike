using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBoard : MonoBehaviour
{
    public List<Room> rooms = new List<Room>();
    public List<Vector2Int> floorTiles = new List<Vector2Int>();
}
