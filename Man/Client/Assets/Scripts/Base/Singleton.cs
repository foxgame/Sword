using UnityEngine;
using System.Collections;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static private T Instance = null;
    static public T instance
    {
        get
        {
            return Instance;
        }
    }

    private void Awake()
    {
        if ( Instance == null )
        {
            Instance = this as T;
            Instance.initSingleton();
        }
    }

    public virtual void initSingleton()
    {

    }

}

