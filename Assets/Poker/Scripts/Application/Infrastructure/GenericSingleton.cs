using UnityEngine;
public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
     private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuitting = false;

    /// <summary>
    /// The public instance of the Singleton.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T).Name} already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Prevents duplicate instances of the Singleton.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T).Name} found. Destroying...");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the application quitting flag to true.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    /// <summary>
    /// Resets the Singleton when destroyed in play mode.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
