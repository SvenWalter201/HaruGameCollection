using UnityEngine;

public class Room
{
    private Vector3[]corners;
    public Vector3[] Corners => corners;

    public Room(Vector3[] corners)
    {
        this.corners = corners;
    }
}

public enum Corner
{
    LEFT_UP,
    LEFT_DOWN,
    RIGHT_UP,
    RIGHT_DOWN
}
