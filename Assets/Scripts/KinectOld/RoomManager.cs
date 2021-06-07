using UnityEngine;
class RoomManager : Singleton<RoomManager>
{
    Room room;
    public Room R => room;
    const float cornerRadius = 300f;

    string defaultPath = Application.dataPath + "Room.json";

    Room defaultRoom = new Room(new Vector3[] { 
        new Vector3(5,0,0), 
        new Vector3(-5,0,0), 
        new Vector3(5,0,5), 
        new Vector3(-5,0,5), 
    });

    public void SaveRoom()
    {
        if(room == null)
        {
            Debug.LogWarning("room was null");
            return;
        }

        JsonFileManager.Save(defaultPath, room);
    }

    public Room LoadRoom()
    {
        if(room == null)
        {
            if (JsonFileManager.Load(defaultPath, out Room room))
            {
                this.room = room;
            }
            else
            {
                Debug.LogWarning("No preconfigured room found. Please run RoomConfiguration program to set one up");
                this.room = defaultRoom;
            }
        }
        return room;
    }

    /// <summary>
    /// Create a room with specified corners
    /// </summary>
    /// <param name="corners"></param>
    /// <param name="save">save the room to a file after creation. Default is false. Warning: Saving a room will overwrite any pre-existing room files. </param>
    public void CreateRoom(Vector3[] corners, bool save = false)
    {
        room = new Room(corners);
        if (save)
        {
            SaveRoom();
        }

    }

    public bool CheckIfPersonIsInCorner(int corner, Bounds bodyBounds)
    {
        Vector3 cornerPosition = room.Corners[corner];
        return bodyBounds.IntersectXZCircle(cornerPosition, cornerRadius);
    }
}

