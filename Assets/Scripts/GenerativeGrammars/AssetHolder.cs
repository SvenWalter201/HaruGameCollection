using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] AssetPositions;

    //0 = lefthand, 1= righthand, 2= hatpos
    public Vector3 GetPosition(int pos)
    {
        return AssetPositions[pos].transform.position;
    }

}

enum PositionAtCharakter
{
    LEFTHAND = 0,
    RIGHTHAND = 1,
    HATPOS = 2
}
