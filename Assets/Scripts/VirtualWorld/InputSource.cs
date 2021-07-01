using UnityEngine;

/// <summary>
/// Used for any non UI-related input in the virtual world. This allows the Input to be temporarily disabled during the execution of a game
/// </summary>
public abstract class InputSource : MonoBehaviour
{
    public bool active = true;

    void Awake()
    {
        VirtualWorldController.Instance.AddInputSource(this);
    }

    void Update()
    {
        if (active)
            TestForInput();
    }

    public abstract void TestForInput();
}