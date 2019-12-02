using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameCampExitUI : GameUI<GameCampExitUI>
{
    GameAskUI askUI;


    public override void initSingleton()
    {
        askUI = transform.GetComponentInChildren<GameAskUI>();
    }

    public void showAskUI( bool b )
    {
        bool bs = IsShow;

        askUI.show( b );

        show();

        if ( !bs )
            showFade();
    }

    public void unShowAskUI()
    {
        askUI.unShow();

        unShowFade();
    }

    public bool IsShowAskUI { get { return askUI.IsShow; } }
    public bool IsOKAskUI { get { return askUI.IsOK; } }

}

