using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleUnitActionUI : GameUI<GameBattleUnitActionUI>
{
    bool enabledBurst = true;
    bool enabledSkill = true;
    bool enabled1 = true;

    int selection = GameDefine.INVALID_ID;
    int[] animationsFrame = new int[ 5 ];
    GameAnimation[] animations = new GameAnimation[ 5 ];


    public int Selection { get { return selection; } }


    public override void initSingleton()
    {
        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ 0 ] = obj.GetComponent<GameAnimation>();
        obj.name = "burst";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -50 - 12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ 1 ] = obj.GetComponent<GameAnimation>();
        obj.name = "skill";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -25 - 12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ 2 ] = obj.GetComponent<GameAnimation>();
        obj.name = "attack";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ 3 ] = obj.GetComponent<GameAnimation>();
        obj.name = "item";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( 12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ 4 ] = obj.GetComponent<GameAnimation>();
        obj.name = "stand";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( 25 + 12.5f + 14.0f , 11 , 0.0f );

        animationsFrame[ 0 ] = 0;
        animationsFrame[ 1 ] = 4;
        animationsFrame[ 2 ] = 8;
        animationsFrame[ 3 ] = 12;
        animationsFrame[ 4 ] = 16;
    }

    public void setPos( int x , int y )
    {
        transform.localPosition = new Vector3( GameDefine.getBattleXBound( x ) ,
        GameDefine.getBattleYBound( y ) + GameBattleManager.instance.LayerHeight , 0.0f );
    }

    public void show( bool b , bool s )
    {
        show();

        enableBurst( b );
        enableSkill( s );

        select( 2 );

        enable( true );
    }


    public void select( int i )
    {
        if ( i < 0 )
        {
            i = 0;
        }

        if ( i > 4 )
        {
            i = 4;
        }

        if ( !enabledBurst && i < 1 )
        {
            i = 1;
        }
        if ( !enabledSkill && i < 2 )
        {
            i = 2;
        }

        selection = i;

        updateAnimations();
    }

    public void updateAnimations()
    {
        for ( int i = 0 ; i < 5 ; i++ )
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

        for ( int i = 0 ; i < 5 ; i++ )
        {
            animations[ i ].setColor( b ? Color.white : Color.gray );
        }
    }

    public void enableBurst( bool b )
    {
        enabledBurst = b;

        if ( enabledBurst )
        {
            animationsFrame[ 0 ] = 0;
        }
        else
        {
            animationsFrame[ 0 ] = 20;
        }

//        updateAnimations();
    }

    public void enableSkill( bool b )
    {
        enabledSkill = b;

        if ( enabledSkill )
        {
            animationsFrame[ 1 ] = 4;
        }
        else
        {
            animationsFrame[ 1 ] = 24;
        }

//        updateAnimations();
    }



}


