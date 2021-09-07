using UnityEngine;

public class PositionController : MonoBehaviour
{
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;
    [SerializeField] private Transform backRright;
    [SerializeField] private Transform backLeft;
    [SerializeField] private Transform background;
    [SerializeField] private Transform foreground;
    [SerializeField] private Transform down;
    [SerializeField] private Transform up;


    public Transform GetRight { get => right; }
    public Transform GetLeft { get => left; }
    public Transform GetBackRight { get => backRright; }
    public Transform GetBackLeft { get => backLeft; }
    public Transform GetBackground { get => background; }
    public Transform GetForeground { get => foreground; }
    public Transform GetDown { get => down; }

    void Start()
    {
    }

}