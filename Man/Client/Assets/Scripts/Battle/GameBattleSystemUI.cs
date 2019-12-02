using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSystemUI : GameUI<GameBattleSystemUI>
{
    bool enabled1 = true;

    int selection = GameDefine.INVALID_ID;
    int[] animationsFrame = new int[ 4 ];
    GameAnimation[] animations = new GameAnimation[ 4 ];

    public int Selection { get { return selection; } }

    public override void initSingleton()
    {
        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Sys_int" ) );
        animations[ 0 ] = obj.GetComponent<GameAnimation>();
        obj.name = "sl";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -50 - 12.5f + 28.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Sys_int" ) );
        animations[ 1 ] = obj.GetComponent<GameAnimation>();
        obj.name = "setting";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -25 - 12.5f + 28.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Sys_int" ) );
        animations[ 2 ] = obj.GetComponent<GameAnimation>();
        obj.name = "map";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -12.5f + 28.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Sys_int" ) );
        animations[ 3 ] = obj.GetComponent<GameAnimation>();
        obj.name = "turn";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( 12.5f + 28.0f , 11 , 0.0f );

        animationsFrame[ 0 ] = 0;
        animationsFrame[ 1 ] = 4;
        animationsFrame[ 2 ] = 8;
        animationsFrame[ 3 ] = 12;
    }

    public void show( int x , int y )
    {
        show();
        select( 0 );
        enable( true );

        transform.localPosition = new Vector3( GameDefine.getBattleXBound( x ) ,
        GameDefine.getBattleYBound( y ) + GameBattleManager.instance.LayerHeight , 0.0f );
    }

    public void enable( bool b )
    {
        enabled1 = b;

        if ( b )
        {
            animations[ selection ].playAnimation( animationsFrame[ selection ] , animationsFrame[ selection ] + 4 );
        }
        else
        {
            animations[ selection ].stopAnimation();
            animations[ selection ].showFrame( animationsFrame[ selection ] + 1 );
        }

        for ( int i = 0 ; i < 4 ; i++ )
        {
            animations[ i ].setColor( b ? Color.white : Color.gray );
        }
    }

    public void select( int i )
    {
        if ( i < 0 )
        {
            i = 0;
        }

        if ( i > 3 )
        {
            i = 3;
        }
        
        selection = i;

        updateAnimations();
    }

    public void updateAnimations()
    {
        for ( int i = 0 ; i < 4 ; i++ )
        {
            if ( selection == i )
            {
                animations[ i ].playAnimation( animationsFrame[ i ] , animationsFrame[ i ] + 4 );
            }
            else
            {
                animations[ i ].stopAnimation();
                animations[ i ].showFrame( animationsFrame[ i ] );
            }
        }
    }
    


}

