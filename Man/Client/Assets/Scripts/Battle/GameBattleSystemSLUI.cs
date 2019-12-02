using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSystemSLUI : GameUI<GameBattleSystemSLUI>
{

    int selection = GameDefine.INVALID_ID;

    public int Selection { get { return selection; } }

    int[] animationsFrame = new int[ 4 ];
    GameAnimation[] animations = new GameAnimation[ 4 ];

    public override void initSingleton()
    {
        //        gameObject.SetActive( false );


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

        animationsFrame[ 0 ] = 16;
        animationsFrame[ 1 ] = 20;
        animationsFrame[ 2 ] = 24;
        animationsFrame[ 3 ] = 28;
    }
    
    public void setPos( int x , int y )
    {
        transform.localPosition = new Vector3( GameDefine.getBattleXBound( x ) - 5 ,
        GameDefine.getBattleYBound( y ) + GameBattleManager.instance.LayerHeight + 5 , -1.0f );
    }

    public void show( int x , int y )
    {
        show();
        select( 0 );

        setPos( x , y );
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

