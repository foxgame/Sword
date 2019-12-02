using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameCampSelectUI : GameUI<GameCampSelectUI>
{
    public const int MAX_SLOT = 7;

    GameAnimation gameAnimation;
    int selection = 0;
    Text[] campText = new Text[ MAX_SLOT ];


    public int Selection { get { return selection; } }

    RectTransform transPos;

    public override void initSingleton()
    {
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            campText[ i ] = transform.Find( "camp" + i ).GetComponent<Text>();
        }

        transPos = transform.Find( "pos" ).GetComponent<RectTransform>();
        gameAnimation = transPos.GetComponentInChildren<GameAnimation>();
        gameAnimation.UI = true;
    }

    private void Start()
    {
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            campText[ i ].text = GameStringData.instance.getString( GameStringType.Camp0 + i );
        }
    }


    public void select( int i )
    {
        selection = i;

        if ( selection < 0 )
        {
            selection = MAX_SLOT - 1;
        }

        if ( selection >= MAX_SLOT )
        {
            selection = 0;
        }

        transPos.anchoredPosition = new Vector2( 8.0f , campText[ selection ].GetComponent<RectTransform>().anchoredPosition.y + 6 );
    }

    public override void onUnShow()
    {
        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();
    }

    public void show( int i )
    {
        show();

        select( i );

        gameAnimation.playAnimation( 1 );
    }


}

