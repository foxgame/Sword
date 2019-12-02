using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GamePowerAskUI : GameUI<GamePowerAskUI>
{
    GameAskUI askUI;
    GameObject okObject;

    public bool IsOKAskUI { get { return askUI.IsOK; } }

    public override void initSingleton()
    {
        okObject = transform.Find( "ok" ).gameObject;
        askUI = transform.Find( "ok" ).GetComponentInChildren<GameAskUI>();
    }

    public void showAskUI( bool b )
    {
        bool bs = isShow;

        show();

        askUI.show( b );

        if ( !bs )
        {
            showFade();
        }
    }

    public void unShowAskUI()
    {
        unShowFade();
    }

}
