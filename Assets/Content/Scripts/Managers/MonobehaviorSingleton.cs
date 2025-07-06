using UnityEngine;

public abstract class MonobehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = GameObject.FindFirstObjectByType<T>();

            if (_instance) DontDestroyOnLoad(_instance.gameObject);
            else Debug.LogError($"MISSING SINGLETON {typeof(T)}");

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(Instance != this) Destroy(gameObject);
    }
}
