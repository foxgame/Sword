using UnityEngine;
using System.Collections;



public class GameCache : Singleton< GameCache >
{
	public override void initSingleton()
	{

	}

	public void addCache( GameObject obj )
	{
		obj.transform.parent = transform;
		obj.SetActive( false );
	}



	
}
