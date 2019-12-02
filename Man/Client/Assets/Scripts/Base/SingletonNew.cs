using UnityEngine;
using System.Collections;


public abstract class SingletonNew< T > where T : new()
{
	static protected T Instance = default(T);
	static public T instance
	{
		get
		{
			if ( Instance == null )
			{
                Instance = new T();
			}
			return Instance;
		}
	}
	
}
