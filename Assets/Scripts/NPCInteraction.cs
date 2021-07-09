using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField]
    GameObject menuPrefab;


    void OnMouseDown()
    {
        if (!VirtualWorldController.Instance.InputState)
            return;

        Instantiate(menuPrefab);
    }
}
