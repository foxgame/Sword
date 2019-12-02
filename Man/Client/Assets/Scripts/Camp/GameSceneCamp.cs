using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneCamp : Singleton<GameSceneCamp>
{
    public override void initSingleton()
    {

    }

    void Start()
    {
        GameCampScript s = GameCampData.instance.getData( GameUserData.instance.Stage );
        GameCampUI.instance.show( s );

        GameBlackUI.instance.unShowBlack( 1.0f , null );

        GameTouchCenterUI.instance.showUI();
    }


}


