using System;
using Unity.VisualScripting;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T instance;

    public static T Instance;

    public virtual void Awake()
    {
        Init();
    }

    public virtual void Init(bool isPersistent = false)
    {
        _removeDuplicates(isPersistent);
        
        if (instance == null)
        {
            instance = (T)FindAnyObjectByType(typeof(T));
                
            if (instance == null)
                _setupInstance(isPersistent);
        }
        Instance = instance;
    }
    
    private static void _setupInstance(bool isPersistent)
    {
        instance = (T)FindAnyObjectByType(typeof(T));
        if (instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).Name;
            instance = obj.AddComponent<T>();
            
            if(isPersistent) DontDestroyOnLoad(obj);
        }
    }

    private void _removeDuplicates(bool isPersistent = false)
    {
        if (instance == null)
        {
            instance = this as T;
            if(isPersistent) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
