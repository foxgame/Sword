using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameWhiteUI : GameUI<GameWhiteUI>
{
    OnEventOver onEventOver;
    float time;
    float timeAll;

    Image image;
    bool isShowBlack;
    bool alphaAdd = false;
    float alpha = 0.0f;

    public override void initSingleton()
    {
        image = GetComponent<Image>();
    }

    public void showWhite( int t , Color c , OnEventOver over )
    {
        show();

        timeAll = GameDefine.getTimeWhite( t );
        time = 0.0f;

        image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );
        alpha = 0.0f;

        isShowBlack = true;
        alphaAdd = true;

        onEventOver = over;
    }

    public void unShowWhite( int t , Color c , OnEventOver over )
    {
        show();

        timeAll = GameDefine.getTimeWhite( t );
        time = 0.0f;

        image.color = new Color( 0.0f , 0.0f , 0.0f , 1.0f );
        alpha = 1.0f;

        isShowBlack = true;
        alphaAdd = false;

        onEventOver = over;
    }

    protected override void onUpdate()
    {
        if ( !isShowBlack )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > timeAll )
        {
            isShowBlack = false;

            if ( alphaAdd )
            {
                image.color = new Color( 1.0f , 1.0f , 1.0f , 0.5f );
                unShow();
            }
            else
            {
                image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );
                unShow();
            }

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }
        else
        {
            if ( alphaAdd )
            {
                alpha = 1.0f / timeAll * time;
            }
            else
            {
                alpha = 1.0f - 1.0f / timeAll * time;
            }

            //            Debug.Log( "alpha " + alpha + " " + time );
            image.color = new Color( 1.0f , 1.0f , 1.0f , alpha );
        }
    }

}

