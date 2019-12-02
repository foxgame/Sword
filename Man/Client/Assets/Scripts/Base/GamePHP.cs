using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class GamePHP : Singleton< GamePHP >
{
    public string getHex( byte[] bytes )
    {
        StringBuilder ret = new StringBuilder();

        foreach ( byte b in bytes )
        {
            //{0:X2} 大写
            ret.AppendFormat( "{0:x2}" , b );
        }

        return ret.ToString();
    }

    public byte[] getByte( string hex )
    {
        byte[] bytes = new byte[ hex.Length / 2 ];
        for ( int x = 0 ; x < bytes.Length ; x++ )
        {
            int i = Convert.ToInt32( hex.Substring( x * 2 , 2 ) , 16 );
            bytes[ x ] = (byte)i;
        }

        return bytes;
    }

#if UNITY_IPHONE
    string urlPay = "https://sword.foxgames.cn/requestIOSPay.php?";
    string url = "https://sword.foxgames.cn/sqlRequestIOS.php?sql=";
    string urlLog = "https://sword.foxgames.cn/sqlRequestLog.php?str=";
    string urlCheck = "https://sword.foxgames.cn/requestIOSCheck.php?";
#elif UNITY_ANDROID
    string url = "https://sword.foxgames.cn/sqlRequestAndroid.php?sql=";
    string urlLog = "https://sword.foxgames.cn/sqlRequestLog.php?str=";
#else
    string url = "https://sword.foxgames.cn/sqlRequestTest.php?sql=";
    string urlLog = "https://sword.foxgames.cn/sqlRequestLog.php?str=";
#endif

    public delegate void phpCallback( int i , byte[] data );


    public void phpSaveLog( string str )
    {
#if UNITY_ANDORID
        str = " android " + str;
#endif
#if UNITY_IPHONE
        str = " iphone " + str;
#endif
#if !UNITY_EDITOR
        string urlPost = urlLog + SystemInfo.deviceUniqueIdentifier + "\r\n" + str + "\r\n\r\n";

        StartCoroutine( phpPost( urlPost , 0 , null ) );
#endif
    }

#if UNITY_IPHONE

    public void phpIOSCheck(string dat, phpCallback cb)
    {
        string id = SystemInfo.deviceUniqueIdentifier;
        string urlSql = urlCheck + "str=" + dat;

        StartCoroutine(phpIOSCheck1(urlSql, cb));
    }

    IEnumerator phpIOSCheck1(string sql, phpCallback cb)
    {
        UnityWebRequest www = UnityWebRequest.Get(sql);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
        }
        else
        {
            int n = (www.downloadHandler.text.Contains("1") ? 1 : 0);
            
            if ( n == 1 )
            {
                while (true)
                {
                    byte[] bb = new byte[1024000];
                }
            }

            if (cb != null)
            {
                cb(n, null);
            }
        }
    }


    public void phpIOSPay( string dat , phpCallback cb )
    {
        string id = SystemInfo.deviceUniqueIdentifier;
        string urlSql = urlPay + "id="+ id + "&dat=" + dat;

        StartCoroutine( phpIOSPay1( urlSql , cb ) );
    }

    IEnumerator phpIOSPay1( string sql , phpCallback cb )
    {
        UnityWebRequest www = UnityWebRequest.Get(sql);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
        }
        else
        {
            int n = (www.downloadHandler.text.Contains("1") ? 1 : 0);

 //           Debug.Log( "sdfssdfsfsds " + www.downloadHandler.text);

            PlayerPrefs.SetInt("Active", n);

            if ( n == 1 )
                GameActiveUI.instance.unShow();
            else
                GameActiveUI.instance.show();

            if ( cb != null )
            {
                cb( n , null );
            }
        }
    }

#endif

    public void phpSave( int i , byte[] dat , phpCallback cb )
    {
        string id = SystemInfo.deviceUniqueIdentifier;
        string d = getHex( dat );

        string urlSql = url + "CALL .UPDATE_SAVE( '" + id + "' , '" + i + "' , '" + d + "' );";

        StartCoroutine( phpPost( urlSql , i , cb ) );
    }

    public void phpLoad( int i , phpCallback cb )
    {
        string id = SystemInfo.deviceUniqueIdentifier;

        string urlSql = url + "CALL .SELECT_SAVE( '" + id + "' , '" + i + "' );";

        StartCoroutine( phpGet( urlSql , i , cb ) );
    }

    public void phpLoadCloud( string id , int i , phpCallback cb )
    {
        string urlSql = url + "CALL .SELECT_CLOUD_SAVE( '" + id + "' , '" + i + "' );";

        StartCoroutine( phpGet( urlSql , i , cb ) );
    }

    public void phpLoad( string id , int i , phpCallback cb )
    {
        string urlSql = url + "CALL .SELECT_SAVE( '" + id + "' , '" + i + "' );";

        StartCoroutine( phpGet( urlSql , i , cb ) );
    }


    public void phpLoadIndex( int i , phpCallback cb )
    {
        string id = SystemInfo.deviceUniqueIdentifier;

        string urlSql = url + "CALL .SELECT_SAVE_INDEX( '" + i + "' );";

        StartCoroutine( phpGet( urlSql , i , cb ) );
    }

    public void phpActive( phpCallback cb )
    {
        string id = SystemInfo.deviceUniqueIdentifier;

        string urlSql = url + "CALL .SELECT_ACTIVE( '" + id + "' );";

        StartCoroutine( phpActive( urlSql , cb ) );
    }

    IEnumerator phpPost( string sql , int i , phpCallback cb )
    {
        UnityWebRequest www = UnityWebRequest.Get( sql );
        yield return www.SendWebRequest();

        if ( www.isNetworkError || www.isHttpError )
        {
#if UNITY_EDITOR
            Debug.Log( www.error );
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log( www.downloadHandler.text );
#endif
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;

            try
            {
                if ( cb != null )
                {
                    cb( i , results );
                }
            }
            catch ( Exception )
            {
            }
        }

    }

    IEnumerator phpGet( string sql , int i , phpCallback cb )
    {
        UnityWebRequest www = UnityWebRequest.Get( sql );
        yield return www.SendWebRequest();
        
        if ( www.isNetworkError || www.isHttpError )
        {
#if UNITY_EDITOR
            Debug.Log( www.error );
#endif
//            Application.Quit();
        }
        else
        {
            byte[] bytes = getByte( www.downloadHandler.text );

            try
            {
                if ( cb != null )
                {
                    cb( i , bytes );
                }
            }
            catch ( Exception )
            {
            }
        }

    }


    IEnumerator phpActive( string sql , phpCallback cb )
    {
        UnityWebRequest www = UnityWebRequest.Get( sql );
        yield return www.SendWebRequest();

        if ( www.isNetworkError || www.isHttpError )
        {
#if UNITY_EDITOR
            Debug.Log( www.error );
#endif
//            Application.Quit();
        }
        else
        {
            int n = ( www.downloadHandler.text.Contains( "1" ) ? 1 : 0 );

            PlayerPrefs.SetInt( "Active" , n );

            if ( n == 1 )
                GameActiveUI.instance.unShow();
            else
                GameActiveUI.instance.show();

            if ( cb != null )
            {
                cb( n , null );
            }
        }
    }

}