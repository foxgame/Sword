using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Networking;

public class GameSetting : SingletonNew< GameSetting > 
{
    public enum GameLocation
    {
        SimplifiedChinese = 0,
        TraditionalChinese,

        Count
    }
    public enum GameMode
    {
        Normal = 0,
        Hard,

        Count
    }

    public enum GameQuality
	{
		High = 0,
		Middle,
		Low
	}

	
	public byte[] msgCode = {41,35,62,4,97,108,86,46,82,16,73,113,113,59,105,107,51,38,91,60,
		7,12,62,25,36,94,13,28,6,55,71,94,51,18,77,72,67,59,11,38,31,3,90,125,9,56,37,
		31,93,84,75,124,22,117,69,59,19,13,9,10,28,91,46,50,32,26,80,110,64,120,54,125,
		18,73,50,118,30,125,73,92,45,79,20,114,68,64,102,80,107,68,48,55,50,59,33,34,118,
		34,17,29,97,11,31,90,48,74,25,2,57,114,29,73,44,0,126,69,25,85,105,0,50,106,73,76,
		83,63,103,86,63,20,86,126,45,92,14,102,3,111,87,73,97,127,105,15,97,77,81,30,29,28,
		22,114,114,102,29,112,4,79,74,119,2,87,104,57,44,83,75,73,18,30,51,116,30,12,116,85,
		84,31,84,36,89,126,53,79,50,34,116,76,79,83,16,45,72,83,15,117,102,89,29,42,101,64,
		119,43,120,1,7,68,14,95,80,0,84,97,13,62,123,5,21,7,59,51,2,31,24,112,18,90,100,84,
		78,49,5,62,105,21,120,70,106,4,22,115,14,89,22,47,103,104,84,119,74,74,80,87,104,118,
		99
	};


	public int quality = 0;
	public bool enabledMusic = true;
	public bool enabledSound = true;

    public float touchScale = 1.0f;

    public GameMode mode = GameMode.Normal;
    public GameLocation location = GameLocation.SimplifiedChinese;

	public Vector2 sizeInPixel;
	public Vector2 maxGrid = new Vector2( 17 , 17 );

	public int version = 1006002;

	public string getVersion()
	{
		int mainVer = version / 1000000;
		int mVer = ( version / 1000 ) % 1000;
		int sourceVer = version % 1000;

		return "V " + mainVer + "." + string.Format( "{0:D2}", mVer ) + "." + string.Format( "{0:D3}", sourceVer ) ;
	}

    public int getBad()
    {
        return PlayerPrefs.GetInt( "bad" , 0 );
    }

    public void setBad( int n )
    {
        PlayerPrefs.SetInt( "bad" , n );
        PlayerPrefs.Save();
    }

    public void save()
	{
        PlayerPrefs.SetFloat( "touchScale" , touchScale );

        PlayerPrefs.SetInt( "location" , (int)location );
        PlayerPrefs.SetInt( "mode" , (int)mode );

        PlayerPrefs.SetInt( "enableMusic" , enabledMusic ? 1 : 0 );
		PlayerPrefs.SetInt( "enableSound" , enabledSound ? 1 : 0 );

		PlayerPrefs.SetInt( "quality" , quality );
		PlayerPrefs.Save();
	}

	public void load()
	{
        touchScale = PlayerPrefs.GetFloat( "touchScale" , 1.0f );

        location = (GameLocation)PlayerPrefs.GetInt( "location" , 0 );
        mode = (GameMode)PlayerPrefs.GetInt( "mode" , 0 );

        enabledMusic = PlayerPrefs.GetInt( "enableMusic" , 1 ) > 0 ? true : false;
		enabledSound = PlayerPrefs.GetInt( "enableSound" , 1 ) > 0 ? true : false;

		if( !PlayerPrefs.HasKey( "quality" ) )
		{
			int n = SystemInfo.systemMemorySize;
			if ( n < 513 )
			{
				GameSetting.instance.quality = (int)GameSetting.GameQuality.Low;
			}
			else if ( n > 900 )
			{
				GameSetting.instance.quality = (int)GameSetting.GameQuality.High;
			}
			else
			{
				GameSetting.instance.quality = (int)GameSetting.GameQuality.Middle;
			}
		}
		else
		{
			quality = PlayerPrefs.GetInt( "quality" );
		}

#if UNITY_EDITOR
		Debug.Log( "quality:" + " " + GameSetting.instance.quality );
#endif
		//Localization.language = "Chinese";
	}

	public void init()
	{
		initGameSetting();

		load();
	}

	bool inited = false;



    uint SDBMHash(byte[] bytes)
    {
        uint hash = 0;

        for (int i = 0; i < bytes.Length; i++)
        {
            hash = (bytes[i]) + (hash << 6) + (hash << 16) - hash;
        }

        return (hash & 0x7FFFFFFF);
    }


    void initGameSetting()
	{
		if ( !inited )
		{
#if UNITY_EDITOR
			StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
			StreamingAssetsPath = Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
			StreamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_WEBPLAYER
			StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";			
#else
			StreamingAssetsPath = Application.streamingAssetsPath + "/";
			//StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";
#endif

//#if UNITY_EDITOR
			Debug.Log( "Screen: " + Screen.width + " " + Screen.height );
			Debug.Log( "memory: setting " + System.GC.GetTotalMemory(true) );
            //#endif

            GameMath.init();

            //			#if UNITY_ANDROID && !UNITY_EDITOR
            //			try 
            //			{
            //				IntPtr ocontext = AndroidJNI.FindClass("android/content/ContextWrapper");
            //				IntPtr method_getFilesDir = AndroidJNIHelper.GetMethodID(ocontext, "getFilesDir", "()Ljava/io/File;");
            //
            //				using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
            //				{
            //					using (AndroidJavaObject oActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) 
            //					{
            //						IntPtr file = AndroidJNI.CallObjectMethod(oActivity.GetRawObject(), method_getFilesDir, new jvalue[0]);
            //						IntPtr ofile = AndroidJNI.FindClass("java/io/File");
            //						IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID(ofile, "getAbsolutePath", "()Ljava/lang/String;");   
            //
            //						PersistentDataPath = AndroidJNI.CallStringMethod(file, method_getAbsolutePath, new jvalue[0]);                    
            //
            //						if( PersistentDataPath != null) 
            //						{
            //						}
            //						else 
            //						{
            //							PersistentDataPath = "/data/data/fox.sa/files";
            //						}
            //					}
            //				}
            //			}
            //			catch(Exception e) 
            //			{
            //				Debug.Log(e.ToString());
            //			}
            //			#else
            PersistentDataPath = Application.persistentDataPath;
//			#endif

			Debug.Log( Application.persistentDataPath + " " + PersistentDataPath );
			Debug.Log( Application.streamingAssetsPath );

			inited = true;

			IsWWW = Application.platform == RuntimePlatform.Android ||
				Application.platform == RuntimePlatform.WebGLPlayer ||
			Application.platform == RuntimePlatform.WSAPlayerARM ||
			Application.platform == RuntimePlatform.WSAPlayerX64 ||
			Application.platform == RuntimePlatform.WSAPlayerX86 ;

//			IsWWW = true;

			#if UNITY_WEBPLAYER || UNITY_WEBGL
			AddonPath = StreamingAssetsPath;
			#else
			AddonPath = PersistentDataPath;
			#endif

//			Localization.language = "Chinese";
		}
	}

	public delegate void loadCallback( byte[] bytes , bool err );
	public void loadRes( string path , loadCallback cb )
	{
		if ( IsWWW )
		{
//			if ( GameTestManager.instance )
//			{
//				GameTestManager.instance.StartCoroutine( wwwLoad( path , cb ) );
//			}
//
//			if ( GameManager.instance) 
//			{
//				GameManager.instance.StartCoroutine( wwwLoad( path , cb ) );
//			}
		}
		else
		{
//			try
//			{
				#if UNITY_WEBPLAYER
				FileStream stream = File.Open( path , FileMode.Open );
				#else
				FileStream stream = File.Open( path , FileMode.Open , FileAccess.Read );
				#endif

				if ( stream == null )
				{
					cb( null , true );
					return;
				}
				
				byte[] bytes = new byte[ stream.Length ];
				stream.Read( bytes , 0 ,(int)stream.Length );
				stream.Close();
				stream.Dispose();

				cb( bytes , false );

				//System.GC.Collect();
//			}
//			catch ( Exception ex ) 
//			{
//				Debug.LogWarning( ex.Message + " " + path + " load error." );
//			}
		}
	}

	public void loadResF( string path , loadCallback cb )
	{
		try
		{
			#if UNITY_WEBPLAYER || UNITY_WEBGL

			if ( GameManager.instance) 
			{
				GameManager.instance.StartCoroutine( wwwLoad( path , cb ) );
			}
			#else
			FileStream stream = File.Open( path , FileMode.Open , FileAccess.Read );

			if ( stream == null )
			{
				cb( null , true );
				return;
			}

			byte[] bytes = new byte[ stream.Length ];
			stream.Read( bytes , 0 ,(int)stream.Length );
			stream.Close();
			stream.Dispose();

			cb( bytes , false );

			#endif

			//System.GC.Collect();
		}
		catch ( Exception ex ) 
		{
			Debug.LogWarning( ex.Message + " " + path + " load error." );
		}
	}


//	public void loadResWWW( string path , loadCallback cb )
//	{
//		if ( GameTestManager.instance )
//		{
//			GameTestManager.instance.StartCoroutine( wwwLoad( path , cb ) );
//		}
//
//		if ( GameManager.instance) 
//		{
//			GameManager.instance.StartCoroutine( wwwLoad( path , cb ) );
//		}
//	}


	IEnumerator wwwLoad( string url , loadCallback cb )
	{
		if ( Application.platform == RuntimePlatform.WindowsEditor || 
		    Application.platform == RuntimePlatform.WindowsPlayer || 
			Application.platform == RuntimePlatform.OSXPlayer ||
			Application.platform == RuntimePlatform.OSXEditor ||
			Application.platform == RuntimePlatform.IPhonePlayer )
			url = "file://" + url;

        UnityWebRequest www = new UnityWebRequest( url );
		yield return www;

		if ( www.isDone )
		{
			if ( www.error != null ) 
			{
				Debug.LogWarning( url + " load error." );
			}
			else
			{
				cb( www.downloadHandler.data , false );
				www.Dispose();
			}
		}
		else
		{
			//cb( null , true );
			Debug.LogWarning( url + " load error." );
		}
	}
	

	public delegate void loadWWWCallback( UnityWebRequest www , int i , bool err );
	public void loadWWW( string path , int i , loadWWWCallback cb )
	{
//		if ( GameManager.instance )  
//		{
//			GameManager.instance.StartCoroutine( wwwLoadW( path , i , cb ) );
//		}
//
//		if ( GameTestManager.instance ) 
//		{
//			GameTestManager.instance.StartCoroutine( wwwLoadW( path , i , cb ) );
//		}
	}
	
	IEnumerator wwwLoadW( string url , int i , loadWWWCallback cb )
	{
        //		if ( Application.platform == RuntimePlatform.WindowsEditor || 
        //		    Application.platform == RuntimePlatform.WindowsPlayer || 
        //			Application.platform == RuntimePlatform.OSXPlayer || 
        //			Application.platform == RuntimePlatform.OSXEditor || 
        //			Application.platform == RuntimePlatform.IPhonePlayer )
        //			url = "file://" + url;

        UnityWebRequest www = new UnityWebRequest( url );
		yield return www;
		
		if ( www.isDone )
		{
			if ( www.error != null ) 
			{
	
			}
			else
			{
				cb( www , i , false );
				www.Dispose();
			}
		}
		else
		{
			cb( null , i , true );
		}
	}


	public static bool		UseAsync = true;

	public static bool		IsWWW = false;


	public static string	TexturesPath = "Textures/";
	public static string	UIAtlasPath = "UIAtlas/";
	public static string	UIPath = "Prefabs/UI/";
	public static string	CreauterPath = "Prefabs/Character/";
	public static string	MonsterPath = "Prefabs/Monster/";
	public static string	EffectsPath = "Prefabs/Effects/";
	public static string	AudioPath = "Audio/";

	public static string	StreamingAssetsPath;
	public static string	PersistentDataPath;
	public static string	AddonPath;

	public static float		GameSpeed = 1.0f;
	public static float		GameScale = 0.74f;


}

