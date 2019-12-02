using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneTitle : Singleton<GameSceneTitle>
{
    public override void initSingleton()
    {

    }

    private void Start()
    {
#if UNITY_IPHONE
        onTitleMovieOver();
#else
        GameMovieManager.instance.playMovieCenter( "Movie/Title" , onTitleMovieOver );
#endif
    }

    void onTitleMovieOver()
    {
        GameTitleUI.instance.show();
        GameTitleUI.instance.select( 0 );
        GameTitleUI.instance.showFade();

        GameTouchCenterUI.instance.showUI();
    }
}

