using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneRPG : Singleton<GameSceneRPG>
{
    public override void initSingleton()
    {
    }

    void Start()
    {
        GameRPGManager.instance.active( GameUserData.instance.Town );
        GameRPGManager.instance.showLayer( 1 );
        GameRPGManager.instance.initPos( GameUserData.instance.TownPosition );

        GameBlackUI.instance.unShowBlack( 1 , null );

        GameTouchCenterUI.instance.showUI();
    }


}

