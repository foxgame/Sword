using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameActiveUI : GameUI< GameActiveUI >
{
    public void onClick()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
//        GameIOSPayData.instance.IAPBuyGoods( 0 );
#elif UNITY_ANDROID && !UNITY_EDITOR
        GamePHP.instance.phpActive( onActive );
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void onActive( int i , byte[] bytes )
    {
        if ( i == 1 )
        {
            return;
        }

        AndroidJavaClass jc = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>( "currentActivity" );
        jo.Call( "activeGame1" , SystemInfo.deviceUniqueIdentifier );
    }
#endif

}
