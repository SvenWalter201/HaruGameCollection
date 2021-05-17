using UnityEngine;

[CreateAssetMenu(fileName = "Context", menuName = "ScriptableObjects/Context", order = 1)]
public class Context : ScriptableObject
{
    public string title;
    public GameObject uiHolder;
    public GameObject context;
}
