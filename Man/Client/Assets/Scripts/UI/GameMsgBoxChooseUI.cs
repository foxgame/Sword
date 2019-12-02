using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameMsgBoxChooseUI : GameUI<GameMsgBoxChooseUI>
{
    Text text;
    Text okText;
    Text cancelText;

    GameAnimation okGameAnimation;
    GameAnimation cancelGameAnimation;

    bool bOK = false;

    public bool IsOK { get { return bOK; } }

    OnEventOver onEventOver;

    Color color;

    public void onClick()
    {
        unShowFade();

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public void onCancelClick()
    {
        bOK = false;

        unShowFade();

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public override void initSingleton()
    {
        text = transform.Find( "text" ).GetComponent<Text>();
        okText = transform.Find( "okText" ).GetComponent<Text>();
        cancelText = transform.Find( "cancelText" ).GetComponent<Text>();

        color = text.color;

        okGameAnimation = transform.Find( "ok" ).GetComponent<GameAnimation>();
        cancelGameAnimation = transform.Find( "cancel" ).GetComponent<GameAnimation>();
    }

    void clear()
    {
        text.text = "";
        okText.text = "";
        cancelText.text = "";

        okGameAnimation.stopAnimation();
        okGameAnimation.clearAnimation();

        cancelGameAnimation.stopAnimation();
        cancelGameAnimation.clearAnimation();
    }
    
    public void select( bool b )
    {
        bOK = b;

        updateAnimations();
    }

    public void updateAnimations()
    {
        if ( bOK )
        {
            okGameAnimation.playAnimation( 1 , 12 );

            cancelGameAnimation.stopAnimation();
            cancelGameAnimation.showFrame( 12 );

            color.a = 1.0f;
            okText.color = color;
            color.a = 0.2f;
            cancelText.color = color;
        }
        else
        {
            okGameAnimation.stopAnimation();
            okGameAnimation.showFrame( 1 );

            cancelGameAnimation.playAnimation( 12 , 23 );

            color.a = 1.0f;
            cancelText.color = color;
            color.a = 0.2f;
            okText.color = color;
        }
    }


    public void showText( string str , string os , string cs , bool b , OnEventOver over )
    {
        show();

        text.text = str;
        okText.text = os;
        cancelText.text = cs;

        select( b );
        
        onEventOver = over;
    }

    public override void onUnShow()
    {
        clear();
    }

}


