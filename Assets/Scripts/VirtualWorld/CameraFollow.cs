using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    Vector3 offset;
    Camera c;

    void Start()
    {
        c = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (c.enabled)
        {
            transform.position = target.position + offset;
            transform.LookAt(target, Vector3.up);
        }

    }
}
