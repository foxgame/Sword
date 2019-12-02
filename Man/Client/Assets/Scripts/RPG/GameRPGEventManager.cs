using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameRPGEventManager : Singleton<GameRPGEventManager>
{
    public class CheckData
    {
        public short id;
        public int dat;
        public short step;
        public short unknow;
    }

    List<CheckData> checkData;

    int activeID;
    int activeStep;

    int msgBoxData;
    bool checkGold;

    GameRPGEvent.RPGEvent activeEvent;

    bool isShow = false;

    OnEventOver onEventOver;

    int changeSceneID = 0;
    int changeScenePos = 0;

    public bool IsShow { get { return isShow; } }

    void onChangeScene()
    {
        GameRPGManager.instance.clear();

        GameRPGManager.instance.active( changeSceneID );
        GameRPGManager.instance.showLayer( 1 );
        GameRPGManager.instance.initPos( changeScenePos );

        GameBlackUI.instance.unShowBlack( 1 , null );

        Resources.UnloadUnusedAssets();
    }

    void onShowBlackOverLoadBattle()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Battle , GameSceneLoadMode.StartBattle );
    }

    void onShowBlackOverLoadCamp()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Camp , GameSceneLoadMode.Count );
    }


    public void nextEvent()
    {
        if ( GameRPGManager.instance.ActiveRPGStage.Event[ activeID ].Event.Length > activeStep + 1 )
        {
            activeStep++;
            activeEvent = GameRPGManager.instance.ActiveRPGStage.Event[ activeID ].Event[ activeStep ];
            doEvent();
        }
        else
        {
// end event
//             activeStep = 0;
//             activeID++;
// 
//             if ( GameRPGManager.instance.ActiveRPGStage.Event.Length > activeID )
//             {
//                 if ( GameRPGManager.instance.ActiveRPGStage.Event[ activeID ].Event.Length > activeStep )
//                 {
//                     activeEvent = GameRPGManager.instance.ActiveRPGStage.Event[ activeID ].Event[ activeStep ];
//                     doEvent();
//                     return;
//                 }
//             }

            if ( onEventOver != null )
            {
                onEventOver();
            }

            isShow = false;

#if UNITY_EDITOR
            Debug.Log( " end event." );
#endif

        }
    }

    void eventOver()
    {
        if ( onEventOver != null )
        {
            onEventOver();
        }

        isShow = false;

        GameTouchCenterUI.instance.showUI();
    }

    public void showEvent( int id , int step , OnEventOver over )
    {
        onEventOver = over;

        if ( isShow )
        {
            return;
        }

        if ( GameRPGManager.instance.ActiveRPGStage.Event.Length > id )
        {
            if ( GameRPGManager.instance.ActiveRPGStage.Event[ id ].Event.Length > step )
            {
                activeID = id;
                activeStep = step;
                activeEvent = GameRPGManager.instance.ActiveRPGStage.Event[ id ].Event[ step ];

                isShow = true;

                doEvent();
            }
        }
    }

    void onMsgBoxClick()
    {
        if ( GameMsgBoxChooseUI.instance.IsOK )
        {
            GameUserData.instance.setGameData( msgBoxData , 1 );
        }
        else
        {
            GameUserData.instance.setGameData( msgBoxData , 0 );
        }

        nextEvent();
    }

    public void doEvent()
    {
#if UNITY_EDITOR
        Debug.LogWarning( "doEvent type: " + activeID + " " + activeStep + " " + String.Format( "{0:X}" , activeEvent.Type ) );
#endif

        switch ( activeEvent.Type )
        {
            case 0x01:
                // unknow
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x01 unknow  " + id );
#endif
                    nextEvent();
                }
                break;
            case 0x02:
                // visible
                {
                    GameRPGManager.instance.visibleLayer( !GameRPGManager.instance.IsVisibleLayer );

#if UNITY_EDITOR
                    Debug.Log( "0x02 visible layer" );
#endif

                    nextEvent();
                }
                break;
            case 0x04:
                // move scene
                {
                    short posX = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short posY = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short posXSpeed = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short posYSpeed = BitConverter.ToInt16( activeEvent.Data , 6 );


#if UNITY_EDITOR
                    Debug.Log( "0x04 move scene " + posX + " " + posY + " " + posXSpeed + " " + posYSpeed );
#endif

//                     if ( GameRPGManager.instance.IsVisibleLayer )
//                     {
//                         GameRPGManager.instance.moveTo( posX , posY , nextEvent );
//                     }
//                     else
//                     {
                        GameRPGSceneMovement.instance.moveToEvent( posX , posY - 1 , posXSpeed , posYSpeed , nextEvent );
//                    }

                }
                break;
            case 0x06:
                // black
                {
                    GameBlackUI.instance.showBlack( 1 , null );

#if UNITY_EDITOR
                    Debug.Log( "0x06 black " );
#endif
                    nextEvent();
                }
                break;
            case 0x07:
                // black fade out
                {
                    GameBlackUI.instance.unShowBlack( 1 , null );

#if UNITY_EDITOR
                    Debug.Log( "0x07 black fade out " );
#endif
                    nextEvent();
                }
                break;
            case 0x09:
                // talk
                {
                    short face = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short type = BitConverter.ToInt16( activeEvent.Data , 8 );
                    short side = BitConverter.ToInt16( activeEvent.Data , 10 );
                    short num = BitConverter.ToInt16( activeEvent.Data , 12 );

#if UNITY_EDITOR
                    Debug.Log( "0x09 talk " + face + " " + type + " " + side + " " + num );
#endif
                    GameTouchCenterUI.instance.unShowUI();

                    GameMsgBoxUI.instance.showText( face , type , side ,
                        GameRPGManager.instance.ActiveRPGStage.EMSG[ activeID ].Msg[ num ] , nextEvent );
                }
                break;

            case 0x10:
                // set game data max 128 + 128
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x10 set game data " + id + " " + dat );
#endif
                    GameUserData.instance.setGameData( id , dat );

                    nextEvent();
                }
                break;

            case 0x15:
                // add animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );


#if UNITY_EDITOR
                    Debug.Log( "0x15 add animation " + id + " " + id2 );
#endif

                    GameRPGManager.instance.addAnimation( id , id2 );

                    nextEvent();
                }
                break;
            case 0x16:
                // remove animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );


#if UNITY_EDITOR
                    Debug.Log( "0x16 remove animation " + id + " " + id2 );
#endif
                    GameRPGManager.instance.clearAnimation( id , id2);

                    nextEvent();
                }
                break;

            case 0x18:
                // show animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short frame = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short endFrame = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "0x18 show animation " + id + " " + id2 + " " + frame + " " + endFrame );
#endif

                    GameRPGManager.instance.playAnimation( id , id2 , frame , endFrame , nextEvent );
                }
                break;


            case 0x1A:
                // play music
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x1A play music" + id + " " + id2 );
#endif
                    GameMusicManager.instance.playMusic( id2 , "Music/Music_" + GameDefine.getString2( id ) );
                    nextEvent();
                }
                break;
            case 0x1B:
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x1C pause music" + id2  );
#endif
                    GameMusicManager.instance.pauseMusic( id2 , true );
                    nextEvent();
                }
                break;
            case 0x1C:
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short v = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x1C music volume" + id2 + " " + v );
#endif
                    GameMusicManager.instance.musicVolume( id2 , v == 1 ? 0.5f : 1.0f );
                    nextEvent();
                }
                break;
            case 0x1D:
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x1D pause music" + id2 + " " + s );
#endif
                    GameMusicManager.instance.pauseMusic( id2 , true );
                    nextEvent();
                }
                break;
            case 0x1E:
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x1E pause music" + id2 + " " + s );
#endif
                    //                    GameMusicManager.instance.musicVolume( id2 , s == 1 ? 0.2f : 1.0f );
                    GameMusicManager.instance.pauseMusic( id2 , false );
                    nextEvent();
                }
                break;

           
           
            
            case 0x20:
                // check game data
                {
//                    CheckData cd = new CheckData();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

                    //checkData.Add( cd );

#if UNITY_EDITOR
                    Debug.Log( "0x20 check game data == " + id + " " + dat + " " + step + " " + unknow + "  " + ( GameUserData.instance.getGameData( id ) == dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) == dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x21:
                // check game data >
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "0x21 check game data > " + id + " " + dat + " " + step + " " + unknow + "  " + ( GameUserData.instance.getGameData( id ) > dat ).ToString() );
#endif
                    //checkData.Add( cd );

                    if ( GameUserData.instance.getGameData( id ) > dat )
                    {
                        activeStep = step - 1;
                    }
                    else
                    {
                    }

                    nextEvent();
                }
                break;
            case 0x22:
                // check game data >=
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "0x22 check game data >= " + id + " " + dat + " " + step + " " + unknow + "  " + ( GameUserData.instance.getGameData( id ) >= dat ).ToString() );
#endif

                    //checkData.Add( cd );

                    if ( GameUserData.instance.getGameData( id ) >= dat )
                    {
                        activeStep = step - 1;
                    }
                    else
                    {
                    }

                    nextEvent();
                }
                break;
            case 0x23:
                // check game data <
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "0x22 check game data < " + id + " " + dat + " " + step + " " + unknow + "  " + ( GameUserData.instance.getGameData( id ) < dat ).ToString() );
#endif
                    //checkData.Add( cd );

                    if ( GameUserData.instance.getGameData( id ) < dat )
                    {
                        activeStep = step - 1;
                    }
                    else
                    {
                        // stage 3 bug
                        if ( checkGold && 
                            GameUserData.instance.Stage == 3 )
                        {
                            checkGold = false;
                            GameUserData.instance.addGold( -dat );
                        }
                    }

                    nextEvent();
                }
                break;
            case 0x24:
                // check game data <=
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "0x22 check game data <= " + id + " " + dat + " " + step + " " + unknow + "  " + ( GameUserData.instance.getGameData( id ) <= dat ).ToString() );
#endif
                    //checkData.Add( cd );

                    if ( GameUserData.instance.getGameData( id ) <= dat )
                    {
                        activeStep = step - 1;
                    }
                    else
                    {
                    }

                    nextEvent();
                }
                break;


            case 0x26:
                // change scene
                {
                    changeSceneID = BitConverter.ToInt16( activeEvent.Data , 0 );
                    changeScenePos = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x26 change scene " + changeSceneID + " " + changeScenePos  );
#endif
                    GameBlackUI.instance.showBlack( 1 , onChangeScene );

                    GameRPGManager.instance.stopMove();

                    nextEvent();
                }
                break;
            case 0x27:
                // shop
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x27 shop " + id );
#endif

                    GameRPGShopInfo info = GameRPGManager.instance.ActiveRPGStage.Info.Shop[ id ];

                    GameShopUI.instance.show( info , nextEvent );

                    GameRPGManager.instance.stopMove();

                    GameBlackUI.instance.unShowBlack( 1 , null );
                }
                break;
            case 0x28:
                // nextStage
                {
#if UNITY_EDITOR
                    Debug.Log( "0x28 nextStage " );
#endif
                    GameUserData.instance.setStage( GameUserData.instance.NextStage );

                    GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadBattle );
                }
                break;
            case 0x29:
                // camp
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x29 camp " + id );
#endif
                    GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadCamp );

                    GameMusicManager.instance.stopMusic( 0 );
                }
                break;


            case 0x2B:
                // clear
                {
                    GameRPGManager.instance.clearGameAnimation();

#if UNITY_EDITOR
                    Debug.Log( "0x2B clear animation " );
#endif
                    nextEvent();
                }
                break;

            case 0x2C:
                //  add game data
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x2C add game data " + id + " " + dat );
#endif
                    GameUserData.instance.addGameData( id , dat );

                    nextEvent();
                }
                break;
            case 0x2D:
                //  add game data
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x2D add game data " + id + " " + -dat );
#endif
                    GameUserData.instance.addGameData( id , -dat );

                    nextEvent();
                }
                break;

            case 0x2E:
                // show check msg box 
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short num = BitConverter.ToInt16( activeEvent.Data , 6 );
                    msgBoxData = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "0x2E show check msg box  " + id + " " + dat + " " + num + " " + msgBoxData );
#endif

                    string str0 = GameRPGManager.instance.ActiveRPGStage.EMSG[ activeID ].Msg[ num ];
                    string str1 = GameRPGManager.instance.ActiveRPGStage.EMSG[ activeID ].Msg[ num + 1 ];
                    string str2 = GameRPGManager.instance.ActiveRPGStage.EMSG[ activeID ].Msg[ num + 2 ];

                    GameMsgBoxChooseUI.instance.showText( str0 , str1 , str2 , true , onMsgBoxClick );
                    GameMsgBoxChooseUI.instance.showFade();

                    GameTouchCenterUI.instance.showUI();
                }
                break;


            case 0x2F:
                // add item
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x2F add item " + id );
#endif
                    GameUserData.instance.addItem( id );

                    nextEvent();
                }
                break;

            case 0x30:
                // check item
                {
                    short item = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x30 check item " + item + " set dat " + id );
#endif
                    if ( GameUserData.instance.hasItem( item ) )
                    {
                        GameUserData.instance.setGameData( id , 1 );
                    }

                    nextEvent();
                }
                break;
            case 0x31:
                // set gold
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x31 set dat " + id + " gold " + GameUserData.instance.Gold );
#endif
                    GameUserData.instance.setGameData( id , GameUserData.instance.Gold );

                    checkGold = true;

                    nextEvent();
                }
                break;
            case 0x32:
                // gold
                {
                    short gold = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x32 add gold " + -gold );
#endif

                    GameUserData.instance.addGold( -gold );

                    nextEvent();
                }
                break;
            case 0x33:
                // next stage
                {
                    short stage = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x33 next stage " );
#endif
                    GameUserData.instance.setNextStage( stage );
                    GameUserData.instance.setStage( GameUserData.instance.NextStage );

                    nextEvent();
                }
                break;


            case 0:
                // none
                {

#if UNITY_EDITOR
                    Debug.Log( "0x00 none." );
#endif
                    eventOver();
                }
                break;
            default:
                {
                    Debug.LogError( "doEvent type: " + activeStep + " " + String.Format( "{0:X}" , activeEvent.Type ) );
                    nextEvent();
                }
                break;
        }
    }

}
