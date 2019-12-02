using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameTouchLeftUI : GameUI<GameTouchLeftUI>
{

    public void showUI()
    {
        show();
        showFade();

        transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );
    }

}

