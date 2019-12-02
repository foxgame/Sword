using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameItemAskUIType
{
    Give = 0,
    Equip,
    UnEquip,
    Drop,

    Count
}

public class GameItemAskUI : MonoBehaviour
{
    public const int MAX_SLOT = 3;

    RectTransform trans;
    RectTransform transPos;
    GameAnimation gameAnimation;

    bool isShow = false;
    int selection = 0;

    bool[] isEnabled;
    GameItemAskUIType[] uiType;

    public bool IsShow { get { return isShow; } }
    public GameItemAskUIType Type { get { return uiType[ selection ]; } }
    public bool IsEnabled { get { return isEnabled[ selection ]; } }
    public int Selection { get { return selection; } }

    Text[] text = new Text[ MAX_SLOT ];

    void Awake()
    {
        trans = transform.GetComponent<RectTransform>();

        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            text[ i ] = transform.Find( "text" + i ).GetComponent<Text>();
        }

        transPos = transform.Find( "pos" ).GetComponent<RectTransform>();
        gameAnimation = transPos.GetComponentInChildren<GameAnimation>();
    }

    public void show( GameItemAskUIType[] t , bool[] e )
    {
        uiType = t;
        isEnabled = e;

        isShow = true;

        gameAnimation.playAnimation( 1 );

        gameObject.SetActive( true );

        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            text[ i ].text = GameStringData.instance.getString( GameStringType.ItemUI0 + (int)uiType[ i ] );

            Color c = text[ i ].color;
            c.a = isEnabled[ i ] ? 1.0f : 0.25f;
            text[ i ].color = c;
        }
    }

    public void select( int n )
    {
        selection = n;

        if ( selection < 0 )
        {
            selection = MAX_SLOT - 1;
        }

        if ( selection >= MAX_SLOT )
        {
            selection = 0;
        }

        transPos.anchoredPosition = new Vector2( -18.0f , text[ selection ].GetComponent<RectTransform>().anchoredPosition.y + 6 );
    }

    public void setPos( float x , float y )
    {
        trans.anchoredPosition = new Vector2( x , y );
    }

    public void unShow()
    {
        isShow = false;

        gameObject.SetActive( false );

        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();
    }


}

