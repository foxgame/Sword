using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameMsgBoxUI : GameUI< GameMsgBoxUI >
{
    GameMsgBoxTextUI left;
    GameMsgBoxTextUI right;
    GameMsgBoxTextUI center;

    GameMsgBoxTextUI active = null;

    OnEventOver onEventOver;

    float time = 0.0f;

    public override void initSingleton()
    {
        left = transform.Find( "left" ).GetComponent<GameMsgBoxTextUI>();
        left.setPivot( new Vector2( 0.0f , 0.0f ) );

        right = transform.Find( "right" ).GetComponent<GameMsgBoxTextUI>();
        right.setPivot( new Vector2( 1.0f , 0.0f ) );

        center = transform.Find( "center" ).GetComponent<GameMsgBoxTextUI>();
        center.setPivot( new Vector2( 1.0f , 0.0f ) );
    }

    void clear()
    {
        active = null;

        left.clear();
        left.gameObject.SetActive( false );
        right.clear();
        right.gameObject.SetActive( false );
        center.clear();
        center.gameObject.SetActive( false );
    }

    public void showText( int face , int type , int side , string str , OnEventOver over )
    {
        bool bs = IsShow;

        clear();

        show();

        onEventOver = over;

        switch ( side )
        {
            case -1:
                active = center;
                break;
            case 0:
                active = left;
                break;
            case 1:
                active = right;
                break;
            default:
                active = left;
                break;
        }

        active.gameObject.SetActive( true );
        active.showText( face , type , str );

        if ( !bs )
        {
            showFade();
        }

        time = 0.0f;
    }

    protected override void onUpdate()
    {
        if ( !IsShow || active == null )
        {
            return;
        }

        if ( active.IsOver || active.IsStopLine )
        {
            time += Time.deltaTime;

            if ( time > 5.0f )
            {
                onClick();
                time = 0.0f;
            }
        }
    }

    public void onClick()
    {
        if ( active == null )
        {
            return;
        }

        if ( active.IsOver )
        {
            clear();

            if ( onEventOver != null )
            {
                onEventOver();
            }

            if ( active == null )
            {
                unShowFade();
            }
        }
        else
        {
            active.onClick();
        }
    }

}


