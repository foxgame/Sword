using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



public enum GameBattleUnitActionItemMode
{
    Use = 0,
    Give,
    Equip,
    Drop,

    Count
}

public class GameBattleUnitActionItemUI : GameUI<GameBattleUnitActionItemUI>
{
    int selection = GameDefine.INVALID_ID;
    int[] animationsFrame = new int[ (int)GameBattleUnitActionItemMode.Count ];
    GameAnimation[] animations = new GameAnimation[ (int)GameBattleUnitActionItemMode.Count ];



    public int Selection { get{ return selection; } }


    public override void initSingleton()
    {
        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ (int)GameBattleUnitActionItemMode.Use ] = obj.GetComponent<GameAnimation>();
        obj.name = "use";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -50 - 12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ (int)GameBattleUnitActionItemMode.Give ] = obj.GetComponent<GameAnimation>();
        obj.name = "give";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -25 - 12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ (int)GameBattleUnitActionItemMode.Equip ] = obj.GetComponent<GameAnimation>();
        obj.name = "equip";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( -12.5f + 14.0f , 11 , 0.0f );

        obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Man_int" ) );
        animations[ (int)GameBattleUnitActionItemMode.Drop ] = obj.GetComponent<GameAnimation>();
        obj.name = "drop";
        obj.transform.SetParent( transform );
        obj.transform.localPosition = new Vector3( 12.5f + 14.0f , 11 , 0.0f );


        animationsFrame[ (int)GameBattleUnitActionItemMode.Use ] = 28;
        animationsFrame[ (int)GameBattleUnitActionItemMode.Give ] = 32;
        animationsFrame[ (int)GameBattleUnitActionItemMode.Equip ] = 36;
        animationsFrame[ (int)GameBattleUnitActionItemMode.Drop ] = 40;
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

        if ( i >= (int)GameBattleUnitActionItemMode.Count )
        {
            i = (int)GameBattleUnitActionItemMode.Count - 1;
        }

        selection = i;

        updateAnimations();
    }

    public void updateAnimations()
    {
        for ( int i = 0 ; i < (int)GameBattleUnitActionItemMode.Count ; i++ )
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


