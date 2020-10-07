using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int seed = 0;
    public int victoryDepth = 2;
    public int maxTries = 100;

    public DungeonBoard board;
    public TextAsset[] roomAsset;

    private float[] noiseValues = new float[1000];
    private int valueIndex = 0;
    [System.Serializable]
    public struct TileData
    {
        public char TileType;
        public Tile Prefab;
    }

    public List<TileData> tileDatas;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        GameObject tileObj = new GameObject("Tile Board");
        board = tileObj.AddComponent<DungeonBoard>();

        int roomIndex = 0;
        string roomStr = roomAsset[roomIndex].text;
        Room startRoom = CreateRoomFromString(roomStr, Vector2Int.zero);
        SpawnRoom(startRoom);
        RegisterTiles(startRoom);

        // Initialize the random values
        Random.InitState(seed);
        for (int i = 0; i < noiseValues.Length; i++)
            noiseValues[i] = Random.value;

        GenerateConnectedRooms(startRoom, victoryDepth, ref valueIndex);
        RemoveUnconnectedDoors();
    }

    private Room CreateRoomFromString(string i_roomStr, Vector2Int i_roomPosition)
    {
        GameObject roomObj = new GameObject($"Room {board.rooms.Count}");
        roomObj.transform.SetParent(board.transform);
        Room room = roomObj.AddComponent<Room>();
        room.roomStr = i_roomStr;
        room.originBoardPosition = i_roomPosition;
        board.rooms.Add(room);
        StoreRoomLayout(room);
        //SpawnRoom(room);

        return room;
    }

    private void StoreRoomLayout(Room i_room)
    {
        Vector2Int roomPos = Vector2Int.zero;
        foreach (char ch in i_room.roomStr)
        {
            if (ch == '\r')
                continue;

            if (ch == 'x')
            {
                roomPos.x++;
                continue;
            }

            if (ch == '\n')
            {
                roomPos.x = 0;
                roomPos.y++;
                continue;
            }

            TempTileData tile = new TempTileData();
            tile.tileType = ch;
            tile.room = i_room;
            tile.roomPosition = roomPos;
            i_room.TempTileDatas.Add(tile);

            if (ch == 'N' || ch == 'S' || ch == 'W' || ch == 'E')
            {
                TempDoorData door = new TempDoorData();
                //door.room = i_room;
                //door.tile = tile;
                door.direction = ch;
                door.roomPostion = roomPos;
                i_room.TempDoorDatas.Add(door);
            }

            roomPos.x++;
        }
    }

    private void SpawnRoom(Room i_room)
    {
        //i_room.tiles.Clear();
        //i_room.doors.Clear();
        Vector2Int roomPos = Vector2Int.zero;
        foreach (char ch in i_room.roomStr)
        {
            if (ch == '\n')
            {
                roomPos.x = 0;
                roomPos.y++;
                continue;
            }

            SpawnTile(ch, i_room, roomPos);

            roomPos.x++;
        }
    }

    private float GetTileRotation(char i_tileType)
    {
        float rotation = 0f;
        switch(i_tileType)
        {
            case 'N':
            case 'S':
                rotation = 90f;
                break;
        }
        return rotation;
    }

    private Tile SpawnTile(char i_tileType, Room i_room, Vector2Int i_roomPosition)
    {
        TileData tileData = tileDatas.Find(td => td.TileType == i_tileType);
        if (tileData.Prefab == null)
            return null;

        Tile tile = Instantiate(tileData.Prefab, i_room.transform);
        tile.transform.localPosition = new Vector3(i_roomPosition.x, 0, i_roomPosition.y);  // due to the right-hand coordinate system, need to swap x and z
        float yRotation = GetTileRotation(i_tileType);
        tile.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        tile.tileType = i_tileType;
        tile.room = i_room;
        tile.roomPosition = i_roomPosition;

        Door door = tile.GetComponent<Door>();
        if(door != null)
        {
            i_room.doors.Add(door);
            door.tile = tile;
            door.direction = i_tileType;
            door.room = i_room;
        }
        i_room.tiles.Add(tile);

        return tile;
    }

    private Room GenerateConnectableRoom(char i_doorDir, ref int i_index, Room i_room)     
    {
        
        Room temp = null;
        for (int i = 0; i < maxTries; i++)
        {
            string roomStr = roomAsset[i_index % roomAsset.Length].text;
            temp = CreateRoomFromString(roomStr, i_room.originBoardPosition);
            if(HasDoor(i_doorDir, temp))
            {
                i_index++;
                return temp;
            }

            RemoveRoom(temp);
            i_index++;
        }

        Debug.LogWarning($"Exceeded max tries when finding aligned room for {i_room.transform.name}");
        return temp;
    }

    private char GetTargetDoorDir(char i_doorDir)
    {
        char targetDir = 'N';
        switch (i_doorDir)
        {
            case 'N':
                targetDir = 'S';
                break;
            case 'S':
                targetDir = 'N';
                break;
            case 'W':
                targetDir = 'E';
                break;
            case 'E':
                targetDir = 'W';
                break;
        }

        return targetDir;
    }

    private bool HasDoor(char i_doorDir, Room i_gRoom) // gRoom should be the generated room
    {
        bool found = false;
        char targetDir = GetTargetDoorDir(i_doorDir);

        foreach(TempDoorData door in i_gRoom.TempDoorDatas)
        {
            if(door.direction == targetDir)
            {
                found = true;
                return found;
            }
        }

        // TODO: see if the room is overlapped with anyother room
        return found;
    }

    // Simulate if the generated room data can be aligned
    private void AlignRoomToDoor(Door i_door, Room i_gRoom)
    {
        char targetDoorDir = GetTargetDoorDir(i_door.direction);   // the corresponding door for gRoom
        TempDoorData gDoorAligned = i_gRoom.FindTempDoorWithDir(targetDoorDir);

        if (gDoorAligned == null)
            throw new System.InvalidOperationException($"Corresponding door of generated room {i_gRoom.transform.name} not found");

        Vector2Int gDoorPos = gDoorAligned.roomPostion;
        Vector2Int doorPos = i_door.tile.roomPosition;
        Vector2Int offset = doorPos - gDoorPos;

        switch(i_door.direction)
        {
            case 'N':
                offset.y--;
                break;
            case 'S':
                offset.y++;
                break;
            case 'W':
                offset.x--;
                break;
            case 'E':
                offset.x++;
                break;
        }

        //offset *= new Vector2Int(1, -1);
        i_gRoom.originBoardPosition += offset;

        i_gRoom.transform.localPosition 
            = new Vector3(i_gRoom.originBoardPosition.x,
                          0,
                          i_gRoom.originBoardPosition.y);

        
    }

    private void ConnectDoors(Door i_door, Room i_gRoom)
    {
        char targetDoorDir = GetTargetDoorDir(i_door.direction);   // the corresponding door for gRoom
        Door gDoorAligned = i_gRoom.FindDoorWithDir(targetDoorDir);

        i_door.connectedDoor = gDoorAligned;
        gDoorAligned.connectedDoor = i_door;
    }

    private void GenerateConnectedRooms(Room i_room, int i_depth, ref int i_index)
    {
        if (i_depth == 0)
            return;

        foreach(Door door in i_room.doors)
        {
            if (door.connectedDoor != null)
            {
                Debug.Log(i_room.name + " " +door.direction);
                continue;
            }
                
            // try some times to generate a room not overlapped
            Room gRoom = null;
            for (int i = 0; i < 100; i++)
            {
                gRoom = GenerateConnectableRoom(door.direction, ref i_index, i_room);

                AlignRoomToDoor(door, gRoom);
                if (!IsOverlapped(gRoom)) // !IsOverlapped(gRoom)
                {
                    SpawnRoom(gRoom);
                    ConnectDoors(door, gRoom);
                    RegisterTiles(gRoom);
                    break;
                }
                else
                {
                    //Debug.Log("Overlapped when testing " + i_room.name + " Door " + door.direction);
                    RemoveRoom(gRoom);
                }
                    
            }

            if (gRoom == null)
                Debug.Log("Did not find proper room");

            if (i_depth > 0 && gRoom != null)
                GenerateConnectedRooms(gRoom, i_depth - 1, ref i_index);
        }

    }

    private void RegisterTiles(Room i_room)
    {
        foreach(Tile tile in i_room.tiles)
        {
            Vector2Int tilePos_w = i_room.originBoardPosition + tile.roomPosition;
            //if(!board.floorTiles.Contains(tilePos_w))
            board.floorTiles.Add(tilePos_w);
        }
    }

    private bool IsOverlapped(Room i_room)  // detect if the room is overlapped w/ any other
    {
        foreach(TempTileData tile in i_room.TempTileDatas)
        {
            Vector2Int tilePos_w = tile.roomPosition + i_room.originBoardPosition;
            if (board.floorTiles.Contains(tilePos_w))
            {
                return true;
            }
            
        }
        return false;
    }

    private void RemoveRoom(Room i_room)
    {
        //Debug.Log("Removing " + i_room.name);
        Destroy(i_room.gameObject);
        board.rooms.Remove(i_room);
    }

    private void RemoveUnconnectedDoors()
    {

    }
}
