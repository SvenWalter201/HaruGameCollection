using UnityEngine;
class RoomManager : Singleton<RoomManager>
{
    Room room;
    public Room R => room;
    const float cornerRadius = 300f;


    Room defaultRoom = new Room(new Vector3[] { 
        new Vector3(5,0,0), 
        new Vector3(-5,0,0), 
        new Vector3(5,0,5), 
        new Vector3(-5,0,5), 
    });

    public Room LoadRoom()
    {
        if(room == null)
        {
            if (JsonFileManager.Load("", out Room room))
            {
                this.room = room;
            }
            else
            {
                Debug.LogWarning("No preconfigured room found");
                this.room = defaultRoom;
            }
        }
        return room;
    }

    public void CreateRoom(Vector3[] corners)
    {
        room = new Room(corners);
    }

    public bool CheckIfPersonIsInCorner(int corner, Bounds bodyBounds)
    {
        Vector3 cornerPosition = room.Corners[corner];
        return bodyBounds.IntersectXZCircle(cornerPosition, cornerRadius);
    }
}

