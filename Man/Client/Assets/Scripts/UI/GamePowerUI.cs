using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GamePowerUI : GameUI<GamePowerUI>
{
    public class TempData
    {
        public short[] power = new short[ (int)GameSpiritType.Count ];
    }

    TempData[] tempData = new TempData[ GameDefine.MAX_USER ];

    GameAnimation[] powerUp = new GameAnimation[ (int)GameSpiritType.Count ];
    GameAnimation[] powerDown = new GameAnimation[ (int)GameSpiritType.Count ];
    GameAnimation[] power = new GameAnimation[ (int)GameSpiritType.Count ];
    Text[] powerText = new Text[ (int)GameSpiritType.Count ];

    Text nameText;

    Text text;
    Text textMax;

    GameAnimation left;
    GameAnimation right;

    GameObject powerObject;

    int userID;

    int selection;
    bool isShowUser;

    Color textColor;

    public bool IsShowUser { get { return isShowUser; } }

    public int Selection { get { return selection; } }

    public int UserID { get { return userID; } }


    public override void initSingleton()
    {
        powerObject = transform.Find( "power" ).gameObject;

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            power[ i ] = transform.Find( "power/power" + i ).GetComponentInChildren<GameAnimation>();
            powerText[ i ] = transform.Find( "power/power" + i + "Text" ).GetComponentInChildren<Text>();

            powerUp[ i ] = transform.Find( "power/power" + i + "Up" ).GetComponentInChildren<GameAnimation>();
            powerDown[ i ] = transform.Find( "power/power" + i + "Down" ).GetComponentInChildren<GameAnimation>();
        }

        left = transform.Find( "power/left" ).GetComponentInChildren<GameAnimation>();
        right = transform.Find( "power/right" ).GetComponentInChildren<GameAnimation>();

        nameText = transform.Find( "power/name" ).GetComponent<Text>();

        text = transform.Find( "power/power" ).GetComponent< Text >();
        textMax = transform.Find( "power/powerMax" ).GetComponent<Text>();

        textColor = text.color;
    }

    void clearTempData()
    {
        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            tempData[ i ] = new TempData();
        }
    }

    void clearSelection()
    {
        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            powerUp[ i ].stopAnimation();
            powerUp[ i ].clearAnimation();

            powerDown[ i ].stopAnimation();
            powerDown[ i ].clearAnimation();
        }
    }

    public void clear()
    {
        clearSelection();

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            power[ i ].stopAnimation();
            power[ i ].clearAnimation();
       }
    }

    public void onAskUIClick()
    {
        for ( int j = 0 ; j < GameDefine.MAX_USER ; j++ )
        {
            GameUnitBase u = GameUserData.instance.getUnitBase( j );

            short count = 0;

            for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
            {
                count += tempData[ j ].power[ i ];
            }

            if ( u.BaseSpiritPower >= count )
            {
                u.BaseSpiritPower -= count;

                for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
                {
                    u.SpiritPower[ i ] += tempData[ j ].power[ i ];
                }
            }

        }
    }

    public void showUser( bool b )
    {
        isShowUser = b;

        if ( b )
        {
            left.stopAnimation();
            left.clearAnimation();

            right.stopAnimation();
            right.clearAnimation();

            select( 0 );
        }
        else
        {
            clearSelection();

            left.playAnimation( 14 , 18 );
            right.playAnimation( 18 , 22 );
        }
    }

    public void onUpClick()
    {
        GameUnitBase u = GameUserData.instance.getUnitBase( userID );

        short count = 0;

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            count += tempData[ userID ].power[ i ];
        }

        if ( u.BaseSpiritPower > count )
        {
            if ( u.SpiritPower[ selection ] + tempData[ userID ].power[ selection ] == GameDefine.MAX_POWER )
            {
                return;
            }

            tempData[ userID ].power[ selection ]++;
        }

        updateData( u );
    }

    public void onDownClick()
    {
        GameUnitBase u = GameUserData.instance.getUnitBase( userID );

        if ( tempData[ userID ].power[ selection ] > 0 )
        {
            tempData[ userID ].power[ selection ]--;
        }

        updateData( u );
    }

    public void select( int n )
    {
        clearSelection();

        selection = n;

        if ( selection < 0 )
        {
            selection = (int)GameSpiritType.Count - 1;
        }

        if ( selection >= (int)GameSpiritType.Count )
        {
            selection = 0;
        }

        powerUp[ selection ].playAnimation( 22 , 25 );
        powerDown[ selection ].playAnimation( 25 , 28 );
    }

    public void showNextUser( bool next )
    {
        if ( next )
        {
            userID = GameUserData.instance.nextUser( userID );

            if ( userID < 0 )
            {
                userID = 0;
            }
        }
        else
        {
            userID--;

            if ( userID < 0 )
            {
                userID = GameUserData.instance.lastUser();
            }
        }

        show( userID , false );
    }


    public void show( int id , bool c )
    {
        if ( c )
        {
            clearTempData();
        }

        isShowUser = false;

        powerObject.SetActive( true );

        userID = id;

        show();

        GameUnitBase u = GameUserData.instance.getUnitBase( id );

        updateData( u );

        left.playAnimation( 14 , 18 );
        right.playAnimation( 18 , 22 );
    }

    public void updateData( GameUnitBase unitBase )
    {
        GameUnit unit = GameUnitData.instance.getData( unitBase.UnitID );

        nameText.text = unit.Name;

        short count = 0;

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            count += tempData[ userID ].power[ i ];
        }

        text.text = GameDefine.getBigInt( ( unitBase.BaseSpiritPower - count ).ToString() );
        textMax.text = GameDefine.getBigInt( unitBase.BaseSpiritPower.ToString() );

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            int p = unitBase.SpiritPower[ i ] + tempData[ userID ].power[ i ];

            powerText[ i ].text = GameDefine.getBigInt( p.ToString() );

            if ( tempData[ userID ].power[ i ] > 0 )
            {
                powerText[ i ].color = new Color( 0.0f , 0.5f , 1.0f );
            }
            else
            {
                powerText[ i ].color = textColor;
            }

            int f = (int)( p / 10.0f );

            if ( p % 10 > 0 )
            {
                f += 1;
            }

            if ( f > 10 )
            {
                f = 10;
            }

            if ( p > 0 )
            {
                power[ i ].showFrame( 3 + f );
            }
        }

    }

//     protected override void onUIFadeOut()
//     {
//         fadeCount++;
// 
//         if ( fadeCount != uiFade.Length )
//         {
//             return;
//         }
// 
//         isFadeOut = false;
// 
//         unShow();
//         unShowAskUI();
//         updateUIStageFadeOver();
//     }

//     public override void unShowFade()
//     {
//         if ( !isShow )
//         {
//             return;
//         }
// 
//         isFadeOut = true;
//         isFadeIn = false;
// 
//         //        Debug.Log( "unShowFade" );
// 
//         for ( int i = 0 ; i < uiFade.Length - 1 ; i++ )
//         {
//             uiFade[ i ].startUIFade( false , onUIFadeOut );
//         }
//     }

//     public override void showFade()
//     {
//         if ( !isShow )
//         {
//             return;
//         }
// 
//         isFadeOut = false;
//         isFadeIn = true;
// 
//         //        Debug.Log( "showFade" );
// 
//         for ( int i = uiFade.Length - 1 ; i < uiFade.Length ; i++ )
//         {
//             uiFade[ i ].startUIFade( true , onUIFadeIn );
//         }
//     }


}


