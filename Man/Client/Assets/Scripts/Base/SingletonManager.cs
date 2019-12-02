using UnityEngine;
using System.Collections;


public abstract class SingletonManager< T > : MonoBehaviour where T : SingletonManager< T >
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
			DontDestroyOnLoad( gameObject );
            Instance.initSingleton();
		}
		else
		{
			Destroy( gameObject );
		}
	}
	
	public virtual void initSingleton()
	{
		
	}
	
}

