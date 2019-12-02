using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameAskUI : MonoBehaviour
{
    RectTransform trans;
    GameAnimation gameAnimation;

    bool isOK = true;
    bool isShow = false;

    public bool IsOK { get { return isOK; } }
    public bool IsShow { get { return isShow; } }


    void Awake()
    {
        trans = transform.parent.GetComponent<RectTransform>();

        gameAnimation = GetComponent<GameAnimation>();
        gameAnimation.UI = true;
    }

    public void show( bool b )
    {
        isShow = true;

        isOK = b;

        if ( isOK )
        {
            gameAnimation.playAnimation( 0 , 11 );
        }
        else
        {
            gameAnimation.playAnimation( 11 , 22 );
        }
    }

    public void setPos( float x , float y )
    {
        trans.anchoredPosition = new Vector2( x , y );
    }

    public void unShow()
    {
        isShow = false;

        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();
    }


}

