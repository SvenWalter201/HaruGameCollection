using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    public static bool Exists => _instance != null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
