using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameTouchCenterUI : GameUI<GameTouchCenterUI>
{
    public void showUI()
    {
#if ( UNITY_ANDROID || UNITY_IPHONE )

        if ( GameTouchLeftUI.instance.IsShow || 
            GameTouchLeftUI.instance.IsFadeIn )
        {
            return;
        }

        GameTouchLeftUI.instance.showUI();
        GameTouchRightUI.instance.showUI();
        unShow();
#else
//         if ( GameTouchLeftUI.instance.IsShow ||
//             GameTouchLeftUI.instance.IsFadeIn )
//         {
//             return;
//         }
// 
//         GameTouchLeftUI.instance.showUI();
//         GameTouchRightUI.instance.showUI();
//         unShow();
#endif
    }

    public void unShowUI()
    {
#if ( UNITY_ANDROID || UNITY_IPHONE )

        if ( !GameTouchLeftUI.instance.IsShow ||
            GameTouchLeftUI.instance.IsFadeOut )
        {
            return;
        }

        GameTouchLeftUI.instance.unShowFade();
        GameTouchRightUI.instance.unShowFade();
        show();
#else
//         if ( !GameTouchLeftUI.instance.IsShow ||
//             GameTouchLeftUI.instance.IsFadeOut )
//         {
//             return;
//         }
// 
//         GameTouchLeftUI.instance.unShowFade();
//         GameTouchRightUI.instance.unShowFade();
//         show();
#endif
    }
}
