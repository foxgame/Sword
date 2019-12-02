using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleEventManager : Singleton<GameBattleEventManager>
{
    [SerializeField]
    int activeID;
    [SerializeField]
    int activeStep;

    [SerializeField]
    GameBattleEvent.BattleEvent activeEvent;

    int movedCount = 0;
    int moveOverCount = 0;

    int moveEventCount = 0;
    int moveEventStart = 0;
    int moveEventFollow = 0;
    int moveEventStep = 0;
    int moveEventSpeed = 0;

    int showAnimationCount = 0;

    int clearAnimationCount = 0;
    int clearAnimationRemove = 0;

    int clearAnimationUnshow = 0;

    int blackTime = 0;

    bool isShow = false;

    short talkFace = 0;
    short talkType = 0;
    short talkSide = 0;
    short talkNum = 0;

    List<int> eventList = new List<int>();

    OnEventOver onEventOver;


    public bool IsShow { get { return isShow; } }

    public void clear()
    {
        isShow = false;
        eventList.Clear();
    }


    void onShowBlackOverLoadRpg()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Rpg , GameSceneLoadMode.BattleOver );
    }

    void onShowBlackOverLoadCamp()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Camp , GameSceneLoadMode.Count );
    }


    public void onMoveOver()
    {
        moveOverCount++;

        if ( moveOverCount == moveEventCount )
        {
            nextEvent();
        }
    }

    public void onMoveToTalk()
    {
        GameBattleCursor.instance.gameObject.SetActive( false );

        GameBattleEMSG[] emsg = GameBattleManager.instance.ActiveBattleStage.EMSG;

        if ( emsg.Length > activeID &&
           emsg[ activeID ].MsgT.Length > talkNum )
        {
            string str = emsg[ activeID ][ talkNum ];
            GameMsgBoxUI.instance.showText( talkFace , talkType , talkSide , str , nextEvent );
        }
        else
        {
            nextEvent();
        }
    }
    
    public void showEvent( int id , int step , OnEventOver over )
    {
        if ( GameBattleGetItemUI.instance.IsShow )
        {
            GameBattleGetItemUI.instance.unShowFade();
        }

        GameBattleInput.instance.Pause = true;

        onEventOver = over;

        if ( isShow )
        {
            eventList.Add( id );
            return;
        }

        if ( GameBattleManager.instance.ActiveBattleStage.Event.Length > id )
        {
            if ( GameBattleManager.instance.ActiveBattleStage.Event[ id ].Event.Length > step )
            {
                activeID = id;
                activeStep = step;
                activeEvent = GameBattleManager.instance.ActiveBattleStage.Event[ id ].Event[ step ];

                isShow = true;

                doEvent();
            }
        }
    }

    public void nextEvent()
    {
        if ( GameBattleManager.instance.ActiveBattleStage.Event[ activeID ].Event.Length > activeStep + 1 )
        {
            activeStep++;
            activeEvent = GameBattleManager.instance.ActiveBattleStage.Event[ activeID ].Event[ activeStep ];
            doEvent();
        }
        else
        {
            // end event

            isShow = false;

            if ( eventList.Count != 0 )
            {
                showEvent( eventList[ 0 ] , 0 , onEventOver );
                eventList.RemoveAt( 0 );
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log( " end event." );
#endif
                // check start event.

                if ( activeID != 1 )
                    GameBattleInput.instance.Pause = false;

                if ( onEventOver != null )
                {
                    onEventOver();
                }
            }
        }
    }

    void returnTitle()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Title , GameSceneLoadMode.Count );
    }

    IEnumerator nextEvent( float delay )
    {
        bool b = false;

        if ( delay >= 60 )
        {
            b = true;
            delay = 10;
        }

        yield return new WaitForSeconds( delay );

        if ( b && GameUserData.instance.Stage == 31 )
        {
            returnTitle();
        }
        else
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
                // unshow animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "show/unshow animation " + id );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    if ( showAnimationCount > 0 )
                    {
                        if ( !unit.gameObject.activeSelf )
                        {
                            unit.gameObject.SetActive( true );
                            unit.setColor( Color.white );

                            byte block = activeID == 1 ? GameBattlePathFinder.BLOCK_EVENT : unit.UnitMove.block;

                            if ( GameBattlePathFinder.instance.isBlockPos( unit.PosX , unit.PosY , block , unit ) )
                            {
                                GameBattlePathFinder.instance.findNearPos( unit.PosX , unit.PosY , block );

                                unit.setPos( GameBattlePathFinder.instance.nearPosX ,
                                    GameBattlePathFinder.instance.nearPosY );
                            }
                        }

                        unit.playAnimationBattle( null );
                        unit.IsShow = true;

                        showAnimationCount--;
                    }

                    if ( clearAnimationCount > 0 )
                    {
                        if ( clearAnimationUnshow == -1 )
                        {
                            unit.stopAnimation();
                            unit.clearAnimation();
                            unit.IsShow = false;

                            unit.updateAlive();
                        }
                        else if( clearAnimationRemove == -1 )
                        {
                            // fade out ?

                            //                             unit.stopAnimation();
                            //                             unit.clearAnimation();

                            unit.clearEffects();

                            if ( unit.IsAlive )
                            {
                                unit.fadeOut( null );
                            }

                            unit.IsShow = false;

                            //Destroy( obj );
                            //men[ id ] = null;
                        }
                        else
                        {
                            Debug.LogError( "clearAnimationS " + clearAnimationUnshow + " " + clearAnimationRemove );
                        }

                        clearAnimationCount--;
                    }

                    nextEvent();
                }
                break;
            case 0x02:
                // change scene
                // +4 short
                // +6 short
                // +8 short
                {
                    short town = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short camp = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short next = BitConverter.ToInt16( activeEvent.Data , 4 );

#if UNITY_EDITOR
                    Debug.Log( "change scene " + town + " " + camp + " " + next );
#endif
                    GameUserData.instance.setNextStage( next );

                    if ( !GameBattleJudgment.instance.IsWin && 
                        !GameBattleJudgment.instance.IsLose )
                    {
                        GameBattleJudgment.instance.UpdateProficiency();
                        GameUserData.instance.updateInTeam();

                        GameBattleTurn.instance.Pause = true;
                    }

                    if ( town != GameDefine.INVALID_ID )
                    {
                        // show rpg

                        GameUserData.instance.setTown( town );

                        GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadRpg );
                    }
                    else
                    {
                        if ( camp == GameDefine.INVALID_ID )
                        {
                            GameUserData.instance.clearBattle();

                            GameUserData.instance.setStage( next );

                            GameBattleManager.instance.active();
                            GameBattleManager.instance.showLayer( 1 , false );
                            GameBattleManager.instance.initMusic();

                            GameBattleManager.instance.initTreasures();

                            GameBattleUnitManager.instance.initUnits();

                            GameUserData.instance.clearTempData();

                            GameBattleTurn.instance.start();
                        }
                        else
                        {
                            // show camp
                            GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadCamp );
                        }
                    }

                    return;
                }
            case 0x03:
                {
                    // game over
                    //nextEvent();

#if UNITY_EDITOR
                    Debug.Log( "game over" );
#endif
                    GameMusicManager.instance.clearMusic();

                    GameMsgBoxUI.instance.unShow();
                    GameMsgBoxChooseUI.instance.unShow();

                    GameOverUI.instance.show();

                    return;
                }
            case 0x04:
                // show title

#if UNITY_EDITOR
                Debug.Log( "show title" );
#endif
                GameBattleTitleUI.instance.showTitle( GameUserData.instance.Stage ,
                    nextEvent );

                break;
            case 0x05:
                // delay 
                // +4 short time
                {
                    short delay = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "delay " + delay );
#endif

                    StartCoroutine( nextEvent( delay / 25.0f ) );
                }
                break;
            case 0x06:
                {
#if UNITY_EDITOR
                    Debug.Log( "unknow 0x06" );
#endif

                    nextEvent();
                }
                break;
            case 0x07:
                // move scene
                // + 4 short x
                // + 6 short y
                {
                    short posX = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short posY = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "move scene " + posX + " " + posY );
#endif

                    GameBattleSceneMovement.instance.moveToEvent( posX , posY );

                    nextEvent();
                }
                break;
            case 0x08:
                // move scene
                // + 4 short x
                // + 6 short y
                // + 8 speed x
                // + 10 speed y
                {
                    short posX = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short posY = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short posXSpeed = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short posYSpeed = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "move scene " + posX + " " + posY + " " + posXSpeed + " " + posYSpeed );
#endif

                    //                     GameSceneMovement.instance.moveTo( posX , posY );
                    //                     nextEvent();

                    GameBattleSceneMovement.instance.moveToEvent( posX , posY , posXSpeed , posYSpeed , nextEvent );
                }
                break;
            case 0x09:
                // clear animation
                {
                    clearAnimationCount = BitConverter.ToInt16( activeEvent.Data , 0 );
                    clearAnimationRemove = BitConverter.ToInt16( activeEvent.Data , 2 );
                    clearAnimationUnshow = 0;
                    showAnimationCount = 0;

#if UNITY_EDITOR
                    Debug.Log( "clear animation " + clearAnimationCount + " " + clearAnimationRemove );
#endif

                    nextEvent();
                }
                break;
            case 0x0A:
                // clear animations and with no block
                // + 4 short size
                // 0x01...
                {
                    clearAnimationCount = BitConverter.ToInt16( activeEvent.Data , 0 );
                    clearAnimationUnshow = BitConverter.ToInt16( activeEvent.Data , 2 );
                    clearAnimationRemove = 0;
                    showAnimationCount = 0;

#if UNITY_EDITOR
                    Debug.Log( "clear animations and with no block " + clearAnimationCount + " " + clearAnimationRemove );
#endif

                    nextEvent();
                }
                break;
            case 0x0B:
                // show animation 
                // + 4 short size
                // 0x01...
                {
                    showAnimationCount = BitConverter.ToInt16( activeEvent.Data , 0 );
                    clearAnimationCount = 0;
                    clearAnimationRemove = 0;

#if UNITY_EDITOR
                    Debug.Log( "show animation " + showAnimationCount );
#endif

                    nextEvent();
                }
                break;

            case 0x0C:
                // unit direction
                // +4 short
                // +6 short
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short d = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "unit direction " + id + " " + d );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    if ( !unit.IsShow )
                    {
                        // fix stage 20 bug.
                        nextEvent();
                        return;
                    }

                    unit.IsShow = true;
                    unit.IsKilled = false;

                    unit.updateAlive();
                    unit.playAnimationBattle( GameAnimationType.Stand ,
                        (GameAnimationDirection)d , null );
                    unit.setColor( Color.white );

                    GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , false );

                    nextEvent();
                }
                break;
            case 0x0D:
                // move unit
                // +4 short id
                // +6 short x
                // +8 short y
                // +10 short side
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short x = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short y = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short d = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "move unit " + id + " " + x + " " + y + " " + d );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    unit.setPos( x , y );
                    unit.setColor( Color.white );

                    unit.IsShow = true;
                    unit.IsKilled = false;

                    unit.updateAlive();
                    unit.playAnimationBattle( GameAnimationType.Stand ,
                        (GameAnimationDirection)d , null );

                    nextEvent();
                }
                break;
            case 0x0E:
                // move event
                // +4 short 
                // +6 short 
                // +8 short 
                // +10 short
                // +12 short
                {
                    moveEventCount = BitConverter.ToInt16( activeEvent.Data , 0 );
                    moveEventStart = BitConverter.ToInt16( activeEvent.Data , 2 );
                    moveEventFollow = BitConverter.ToInt16( activeEvent.Data , 4 );
                    moveEventSpeed = BitConverter.ToInt16( activeEvent.Data , 6 );
                    //                    short short5 = BitConverter.ToInt16( activeEvent.data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "move event " + moveEventCount + " " + moveEventStart + " " + moveEventFollow + " " + moveEventSpeed );
#endif

                    movedCount = 0;
                    moveOverCount = 0;

                    nextEvent();
                }
                break;
            case 0x0F:
                // move event
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short x = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short y = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short speed = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "move event " + id + " " + x + " " + y + " " + speed );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    if ( unit.UnitID < GameDefine.MAX_USER && 
                        !unit.InTeam && GameUserData.instance.Stage == 9 )
                    {
                        moveOverCount++;
                        unit.setPos( x , y );
                        return;
                    }

                    bool b = unit.moveToEvent( x , y , moveEventStart > 0 , moveEventFollow > 0 , moveEventSpeed , onMoveOver );

                    if ( !b )
                    {
                        b = unit.moveToDirection( x , y , moveEventStart > 0 , moveEventFollow > 0 , moveEventSpeed , onMoveOver );
                    }

                    moveEventFollow = 0;
                    movedCount++;

                    unit.IsShow = true;
                    unit.IsKilled = false;

                    unit.updateAlive();

                    GameBattleCursor.instance.moveTo( x , y , false );

                    if ( !b )
                    {
                        moveOverCount++;
                    }

                    if ( movedCount != moveEventCount )
                    {
                        nextEvent();
                    }
                    else
                    {
                        if ( moveOverCount == moveEventCount )
                        {
                            nextEvent();
                        }
                    }
                }
                break;
            case 0x10:
                // move event
                // +4 short 
                // +6 short 
                // +8 short 
                // +10 short
                // +12 short
                {
                    moveEventCount = BitConverter.ToInt16( activeEvent.Data , 0 );
                    moveEventStart = BitConverter.ToInt16( activeEvent.Data , 2 );
                    moveEventFollow = BitConverter.ToInt16( activeEvent.Data , 4 );
                    moveEventStep = BitConverter.ToInt16( activeEvent.Data , 6 );
                    moveEventSpeed = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "move event " + moveEventCount + " " + moveEventStart + " " + moveEventFollow + " " + moveEventStep + " " + moveEventSpeed );
#endif

                    movedCount = 0;
                    moveOverCount = 0;

                    nextEvent();
                }
                break;
            case 0x11:
                // move unit
                {
                    // 0 n, 1 s,2 w, 3e
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dir = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "move unit " + id + " " + dir );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    int x = 0;
                    int y = 0;

                    switch ( dir )
                    {
                        case 0:
                            x = unit.PosX;
                            y = unit.PosY - moveEventStep;
                            break;
                        case 1:
                            x = unit.PosX;
                            y = unit.PosY + moveEventStep;
                            break;
                        case 2:
                            x = unit.PosX - moveEventStep;
                            y = unit.PosY;
                            break;
                        case 3:
                            x = unit.PosX + moveEventStep;
                            y = unit.PosY;
                            break;
                    }

                    if ( unit.UnitID < GameDefine.MAX_USER && !unit.InTeam
                        && GameUserData.instance.Stage == 9 )
                    {
                        moveOverCount++;
                        unit.setPos( x , y );
                        return;
                    }

                    //                     int count = GameBattlePathFinder.instance.findPath( unit.PosX , unit.PosY , x , y , unit.UnitMove.block , unit.UnitMove.fly , GameUnitCampType.Count );
                    // 
                    //                     if ( count > 0 && count < moveEventStep * 2 )
                    //                     {
                    //                         unit.moveToEvent( x , y , moveEventStart > 0 , moveEventFollow > 0 , moveEventSpeed , onMoveOver );
                    //                     }
                    //                     else
                    //                     {
                    bool b = unit.moveToDirection( x , y , moveEventStart > 0 , moveEventFollow > 0 , moveEventSpeed , onMoveOver );
//                     }

                    unit.IsShow = true;
                    unit.IsKilled = false;
                    unit.updateAlive();

                    if ( unit.UnitID < GameDefine.MAX_USER && !unit.InTeam &&
                        GameUserData.instance.Stage == 29 && 
                        GameUserData.instance.getGameData( 12 ) >= 2 )
                    {
                        // fix 29 bug

                        unit.updateInteam1();
                    }

                    movedCount++;

                    if ( !b )
                    {
                        moveOverCount++;
                    }

                    if ( movedCount != moveEventCount )
                    {
                        nextEvent();
                    }
                    else
                    {
                        if ( moveOverCount == moveEventCount )
                        {
                            nextEvent();
                        }
                    }
                }
                break;
            case 0x12:
                // white ?
                // +4 short
                // +6 short
                // +8 short
                // +10 short
                // +12 short
                // +14 short
                // +16 short
                // +18 short
                {
                    short color = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short fadeOut = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short fadeIn = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short per = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short baseColor = BitConverter.ToInt16( activeEvent.Data , 8 );
                    short unkonw6 = BitConverter.ToInt16( activeEvent.Data , 10 );
                    short unkonw7 = BitConverter.ToInt16( activeEvent.Data , 12 );
                    short time = BitConverter.ToInt16( activeEvent.Data , 14 );

#if UNITY_EDITOR
                    Debug.Log( "white " );
#endif

                    Color c = GameDefine.getRGB888( color , false );

                    if ( fadeOut > 0 )
                    {
                        GameWhiteUI.instance.unShowWhite( time * fadeOut , c , nextEvent );
                    }
                    else if( fadeIn > 0 )
                    {
                        GameWhiteUI.instance.showWhite( time * fadeIn , c , nextEvent );
                    }
                    else
                    {
                        nextEvent();
                    }
                }
                break;
            case 0x13:
                // black time
                // +4 short time
                {
                    blackTime = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "black time " + blackTime );
#endif

                    GameBlackUI.instance.showBlack( blackTime , null );

                    nextEvent();

                    //                     if ( blackTime > 0 )
                    //                         GameBlackUI.instance.setBlack( true );
                    // 
                    //                     nextEvent();
                }
                break;
            case 0x14:
                // unshow black time
                // +4 short time
                {
                    blackTime = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "unshow black time " + blackTime );
#endif

                    GameBlackUI.instance.unShowBlack( blackTime , null );

                    nextEvent();
                    //                     if ( blackTime > 0 )
                    //                         GameBlackUI.instance.setBlack( false );
                    //
                    //                    nextEvent();
                }
                break;
            case 0x15:
                // black start?
                {
#if UNITY_EDITOR
                    Debug.Log( "black start?" + blackTime );
#endif

                    if ( blackTime > 0 )
                    {
                        GameBlackUI.instance.unShowBlack( blackTime , nextEvent );
                        blackTime = 0;
                    }
                    else
                    {
                        nextEvent();
                    }
                }
                break;
            case 0x16:
                // talk
                // +4 int unknow
                // +8 short face
                // +10 short face num
                // + 12 short side left or right
                // + 14 short talk num
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    talkFace = BitConverter.ToInt16( activeEvent.Data , 4 );
                    talkType = BitConverter.ToInt16( activeEvent.Data , 6 );
                    talkSide = BitConverter.ToInt16( activeEvent.Data , 8 );
                    talkNum = BitConverter.ToInt16( activeEvent.Data , 10 );

#if UNITY_EDITOR
                    Debug.Log( "talk " + id + " " + id2 + " " + talkFace + " " + talkType + " " + talkSide + " " + talkNum );
#endif

                    if ( id != GameDefine.INVALID_ID )
                    {
                        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                        if ( unit != null )
                        {
                            GameBattleCursor.instance.gameObject.SetActive( true );
                            GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY ,
                                            GameBattleCursor.SpeedX , GameBattleCursor.SpeedY , true , 
                                            onMoveToTalk );
                        }
                        else
                        {
                            onMoveToTalk();
                        }
                    }
                    else
                    {
                        onMoveToTalk();
                    }
                    
                }
                break;
            case 0x17:
                // show unit
                // +4 short event Parm1
                // +6 check position
                {
                    short parm = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short check = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "show unit " + parm + " " + check );
#endif
                    GameBattleUnitManager.instance.showUnits( parm , check );
                }
                break;
            case 0x18:
                // add animation
                // +4 short animation name , 
                // +6 short 0x01, ID
                {
                    short name = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "add animation " + name + " " + id );
#endif

                    GameBattleManager.instance.addAnimation( id , name );

                    nextEvent();
                }
                break;
            case 0x19:
                // show animation
                // + 4 short ID
                // + 6 int
                // + 10 short
                // + 12 short 
                // 				{
                // 					unsigned char fff[] = { 0x82 , 0x96 , 0x98 };
                // 					sizeAll += 14; 
                // 				}
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int id2 = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short frame = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short endFrame = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "show animation " + id + " " + id2 + " " + frame + " " + endFrame );
#endif

                    GameBattleManager.instance.playAnimation( id , id2 , frame , endFrame , nextEvent );
                }
                break;
            case 0x1A:
                // play animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "play animation " + id + " " + s2 );
#endif
                    GameBattleManager.instance.playAnimation( id , s2 );

                    nextEvent();
                }
                break;
            case 0x1C:
                // unshow animation
                // +4 short id ?
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "unshow animation " + id );
#endif

                    GameBattleManager.instance.clearAnimation( id );

                    nextEvent();
                }
                break;
            case 0x1D:
                // unshow animation
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "unshow animation " + id );
#endif

                    GameBattleManager.instance.clearAnimation( id );

                    nextEvent();
                }
                break;
            case 0x1E:
                // show effect
                {
                    //                     short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    //                     int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    // 
                    // 
                    //                     Debug.Log( "set dat " + id + " " + dat );
                    // 
                    //                     GameBattleManager.instance.showEffect( id , dat );

                    short s1 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s2 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short s3 = BitConverter.ToInt16( activeEvent.Data , 4 );

#if UNITY_EDITOR
                    Debug.Log( "show effect parm event " + s1 + " " + s2 + " " + s3 );
#endif

                    GameUserData.instance.setGameData( s1 , s2 );

                    GameBattleManager.instance.showEffect( s1 , s2 , s3 );

                    nextEvent();
                }
                break;
            case 0x1F:
                // set unknow balck ?
                {
#if UNITY_EDITOR
                    Debug.Log( "unknow 0x1F" );
#endif

                    nextEvent();
//                     GameBlackUI.instance.unShowBlack( blackTime , nextEvent );
//                     blackTime = 0;
                }
                break;
            case 0x20:
                // clear unknow balck ?
                {
#if UNITY_EDITOR
                    Debug.Log( "unknow 0x20" );
#endif

                    nextEvent();
//                     short time = BitConverter.ToInt16( activeEvent.data , 0 );
//                     GameBlackUI.instance.unShowBlack( time , nextEvent );
                }
                break;
            case 0x21:
                // show Cursor
                {
                    //                    GameBlackUI.instance.unShowBlack( 1 , nextEvent );

#if UNITY_EDITOR
                    Debug.Log( "show cursor" );
#endif

                    GameBattleSceneMovement.instance.updatePositionToCamera();

                    GameBattleCursor.instance.moveTo( GameBattleSceneMovement.instance.PosX + GameCameraManager.instance.xCell ,
                        GameBattleSceneMovement.instance.PosY + GameCameraManager.instance.yCell , true );
                    GameBattleCursor.instance.show();

                    if ( activeID == 1 )
                    {
                        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( GameUserData.instance.firstUser() );

                        if ( unit != null )
                        {
                            GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , true );
                        }
                    }

                    nextEvent();
                }
                break;
            case 0x22:
                // unshow Cursor
                {
#if UNITY_EDITOR
                    Debug.Log( "unshow cursor" );
#endif

                    GameBattleCursor.instance.unShow();
                    nextEvent();
                }
                break;
            case 0x23:
                // load map layer
                // +4 short layer
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "load map layer " + id );
#endif

                    GameBattleManager.instance.showLayer( id , false );
                    nextEvent();
                }
                break;
            case 0x24:
                // unload map layer
                // +4 short layer
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "unload map layer " + id );
#endif

                    GameBattleManager.instance.unShowLayer( id );
                    nextEvent();
                }
                break;
            case 0x25:
                // show map effect
                {
                    short s = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short parm = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "show map effect " + s + " " + parm );
#endif

                    GameBattleManager.instance.showEffect( parm , s );

                    nextEvent();
                }
                break;
            case 0x26:
                // unshow map effect
                {
                    short s = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short parm = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "unshow map effect" + s + " " + parm );
#endif

                    GameBattleManager.instance.unShowEffect( parm , s );

                    nextEvent();
                }
                break;
            case 0x28:
                // play map effect
                {
                    short s = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short parm = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short frame = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short endFrame = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "play map effect " + s + "  " + parm + " " + frame + " " + endFrame );
#endif

                    GameBattleManager.instance.playOnceEffect( parm , frame , endFrame , nextEvent );
                }
                break;
            case 0x29:
                // play movie
                // + 4 short movie id
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    string path = "Movie/Movie_" + ( id < 10 ? "0" + id : id.ToString() );

#if UNITY_EDITOR
                    Debug.Log( "play movie " + id );
#endif

                    GameMusicManager.instance.stopMusic();

                    GameMovieManager.instance.playMovie( path , nextEvent );
                }
                break;

            case 0x2A:
                // play music
                // + 4 short music id
                // + 6 short music id2
                // music Music_AB
                {
                    // 					char buff[ 32 ] = {};
                    // 					sprintf( buff , "MUSIC\MUSIC_%02d.MP3" , 0x32 );
                    // 					type = type;

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "play music " + id + " " + id2 );
#endif
                    
                    GameMusicManager.instance.playMusic( id2 , "Music/Music_" + GameDefine.getString2( id ) );
                    nextEvent();
                }
                break;
            case 0x2B:
                // stop music
                // + 4 short music id2
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "stop music " + id2 );
#endif

                    GameMusicManager.instance.pauseMusic( id2 , true );
                    nextEvent();
                }
                break;
            case 0x2C:
                // music volume
                // + 4 short id2
                // + 6 1small 0big 
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short v = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "music volume " + id2 + " " + v );
#endif

                    GameMusicManager.instance.musicVolume( id2 , v == 1 ? 0.5f : 1.0f );
                    nextEvent();
                }
                break;
            case 0x2D:
                // pause music
                // + 4 short id2
                // + 6 short 1 big , 0 stop
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "music pause " + id2 + " " + s );
#endif

                    GameMusicManager.instance.pauseMusic( id2 , true );
                    nextEvent();
                }
                break;
            case 0x2E:
                // un pause music
                // + 4 short id2
                // + 6 short 1 small , 2 big
                {
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "music unpause " + id2 + " " + s );
#endif

                    //                    GameMusicManager.instance.musicVolume( id2 , s == 1 ? 0.2f : 1.0f );
                    GameMusicManager.instance.pauseMusic( id2 , false );
                    nextEvent();
                }
                break;
            case 0x2F:
                // unit ai
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short ai = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short moveTo0 = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short moveTo1 = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unkonw2 = BitConverter.ToInt16( activeEvent.Data , 8 );
                    short unkonw3 = BitConverter.ToInt16( activeEvent.Data , 10 );

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );
                    unit.BattleAIType = (GameBattleAIType)ai;

                    if ( unit.BattleAIType == GameBattleAIType.AIMoveToUnit )
                    {
                        unit.AIMoveToIDUnkonw = moveTo0;
                        unit.AIMoveToID = moveTo1;
                    }

                    if ( unit.BattleAIType == GameBattleAIType.AIMoveToPos )
                    {
                        unit.AIMoveToX = moveTo0;
                        unit.AIMoveToY = moveTo1;
                    }

                    unit.BattleAICheckFight = false;

#if UNITY_EDITOR
                    Debug.Log( "unit ai " + id + " " + (GameBattleAIType)ai + " " + moveTo0 + " " + moveTo1 + " " + unkonw2 + " " + unkonw3 );
#endif

                    nextEvent();
                }
                break;
            case 0x30:
                {
                    // go step?
                    short s1 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short s2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "go step " + s1 + " " + s2 );
#endif

                    activeStep = s1 - 1;

                    // unknow
                    nextEvent();
                }
                break;
            case 0x31:
                {
                    // check game dat 
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "check game dat == " + id + " " + dat + " " + step + " " + unknow + " " + ( GameUserData.instance.getGameData( id ) == dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) == dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x32:
                {
                    // check game dat 
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "check game dat > " + id + " " + dat + " " + step + " " + unknow + " " + ( GameUserData.instance.getGameData( id ) > dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) > dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x33:
                // check game dat >= 
                {
                    // check game dat 
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "check game dat >= " + id + " " + dat + " " + step + " " + unknow + " " + ( GameUserData.instance.getGameData( id ) >= dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) >= dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x34:
                // unkonw
                {
                    //nextEvent();

                    // check game dat 
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "check game dat < " + id + " " + dat + " " + step + " " + unknow + " " + ( GameUserData.instance.getGameData( id ) < dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) < dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x35:
                // unkonw
                {
                    // check game dat 
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int dat = BitConverter.ToInt32( activeEvent.Data , 2 );
                    short step = BitConverter.ToInt16( activeEvent.Data , 6 );
                    short unknow = BitConverter.ToInt16( activeEvent.Data , 8 );

#if UNITY_EDITOR
                    Debug.Log( "check game dat <= " + id + " " + dat + " " + step + " " + unknow + " " + ( GameUserData.instance.getGameData( id ) <= dat ).ToString() );
#endif

                    if ( GameUserData.instance.getGameData( id ) <= dat )
                    {
                        activeStep = step - 1;
                    }

                    nextEvent();
                }
                break;
            case 0x36:
                // clear ??
                {
                    // battle over 
                    // clear enemy ?
                    nextEvent();

#if UNITY_EDITOR
                    Debug.Log( "clear " );
#endif

                    GameBattleUnitManager.instance.clearEnemy();
                }
                break;
            case 0x37:
                // set game data enemy count
                {
                    //nextEvent();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x37 game data enemy count " + id + " " + GameBattleUnitManager.instance.getEnemyCount() );
#endif

                    GameUserData.instance.setGameData( id , GameBattleUnitManager.instance.getEnemyCount() );

                    nextEvent();
                }
                break;
            case 0x3A:
                // add game data
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    int value = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x3A add game data " + id + " " + value );
#endif

                    GameUserData.instance.addGameData( id , value );

                    nextEvent();
                }
                break;

            case 0x3B:
                // 
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short value1 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short value2 = BitConverter.ToInt16( activeEvent.Data , 4 );

#if UNITY_EDITOR
                    Debug.Log( "0x3b data " + id + " " + value1 + " " + value2 );
#endif

                    nextEvent();
                }
                break;
            case 0x3C:
                // 
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short value1 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short value2 = BitConverter.ToInt16( activeEvent.Data , 4 );

#if UNITY_EDITOR
                    Debug.Log( "0x3C maybe set pos for play animation ? " + id + " " + value1 + " " + value2 );
#endif

                    GameBattleManager.instance.LastUnitID = id;

                    nextEvent();
                }
                break;
            case 0x3D:
                // 
                {
                    short value1 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short value2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x3D data " + value1 + " " + value2 );
#endif

                    nextEvent();
                }
                break;
            case 0x3E:
                // 
                {
                    short value1 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short value2 = BitConverter.ToInt16( activeEvent.Data , 2 );
                    short value3 = BitConverter.ToInt16( activeEvent.Data , 4 );
                    short value4 = BitConverter.ToInt16( activeEvent.Data , 6 );

#if UNITY_EDITOR
                    Debug.Log( "0x3E data " + value1 + " " + value2 + " " + value3 + " " + value4 );
#endif

                    nextEvent();
                }
                break;
            case 0x3F:
                // 
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x3F set data turn " + id + " " + GameBattleTurn.instance.Turn );
#endif

                    GameUserData.instance.setGameData( id , GameBattleTurn.instance.Turn );

                    nextEvent();
                }
                break;
            case 0x40:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short iid = BitConverter.ToInt16( activeEvent.Data , 2 );

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    GameItem item = GameItemData.instance.getData( iid );

#if UNITY_EDITOR
                    Debug.Log( "change equip " + id + " " + iid );
#endif

                    if ( unit != null )
                    {
                        switch ( item.ItemType )
                        {
                            case GameItemType.Item:
                                break;
                            case GameItemType.Weapon:
                                {
//                                     if ( unit.IsUser )
//                                     {
//                                         if ( unit.canAddItem() )
//                                         {
//                                             unit.addItem( iid );
//                                             unit.WeaponID = iid;
//                                             unit.updateUnitData();
//                                         }
//                                         else
//                                         {
//                                             GameUserData.instance.addItem( iid );
//                                         }
//                                     }
//                                     else
//                                    {
                                        if ( unit.WeaponSlot != GameDefine.INVALID_ID )
                                        {
                                            unit.removeItem( unit.WeaponSlot );
                                        }

                                        unit.addItem( iid );
                                        unit.WeaponID = iid;
                                        unit.updateUnitData();
//                                    }
                                    
                                }
                                break;
                            case GameItemType.Armor:
                                {
                                    if ( unit.ArmorSlot != GameDefine.INVALID_ID )
                                    {
                                        unit.removeItem( unit.ArmorSlot );
                                    }

                                    unit.addItem( iid );
                                    unit.ArmorID = iid;
                                    unit.updateUnitData();
                                }
                                break;
                            case GameItemType.Accessory:
                                {
                                    if ( unit.AccessorySlot != GameDefine.INVALID_ID )
                                    {
                                        unit.removeItem( unit.AccessorySlot );
                                    }

                                    unit.addItem( iid );
                                    unit.AccessoryID = iid;
                                    unit.updateUnitData();
                                }
                                break;
                        }
                        if ( item.ItemType == GameItemType.Weapon )
                        {
                            
                        }

                        
                    }

                    nextEvent();
                }
                break;
            case 0x41:
                {
                    // check unit
                    //nextEvent();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dat = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x41 set game data " + id + " " + dat );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    if ( unit.IsAlive && unit.IsHurt )
                    {
                        GameUserData.instance.setGameData( dat , 1 );
                    }
                    else
                    {
                        GameUserData.instance.setGameData( dat , 0 );
                    }

                    nextEvent();
                }
                break;
            case 0x42:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x42 fade out " + id + " " );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( GameUserData.instance.getGameData( id ) );

                    if ( unit != null )
                    {
                        unit.IsShow = false;
                        unit.fadeOut( null );
                    }

                    nextEvent();
                }
                break;
            case 0x43:
                {
                    // copy unit 

                    short id1 = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );

                    GameBattleUnit unit1 = GameBattleUnitManager.instance.getUnit( id1 );
                    GameBattleUnit unit2 = GameBattleUnitManager.instance.getUnit( id2 );

                    GameUnitBase base1 = unit1.getUnitBase();
                    GameUnitBase base2 = unit2.getUnitBase();

                    base2.HP = base1.HP;
                    base2.MP = base1.MP;
                    base2.LV = base1.LV;

                    base2.Str = base1.Str;
                    base2.Int = base1.Int;
                    base2.Avg = base1.Avg;
                    base2.Vit = base1.Vit;
                    base2.Luk = base1.Luk;

//                     for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
//                     {
//                         base2.SpiritPower[ i ] = base1.SpiritPower[ i ];
//                     }

                    unit2.IsCopy = true;
                    unit2.updateUnitData();
                    unit2.initHPMP();

#if UNITY_EDITOR
                    Debug.Log( "0x43 copy unit data " + id1  + " " + id2 );
#endif

                    nextEvent();
                }
                break;
            case 0x44:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short eventID = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x44 move to event." + id + " " + eventID );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    if ( eventID == 0 )
                    {
                        if ( unit.IsNpc )
                        {
                            GameBattleUnitManager.instance.removeNpc( unit );
                            GameBattleUnitManager.instance.addUser( unit );
                        }
                        else
                        {
                            Debug.LogError( "0x44 error" );
                            return;
                        }

                        unit.BattleAIType = GameBattleAIType.None;
                        unit.AIMoveToX = GameDefine.INVALID_ID;
                        unit.AIMoveToY = GameDefine.INVALID_ID;
                    }
                    else
                    {
                        if ( unit.IsUser )
                        {
                            GameBattleUnitManager.instance.removeUser( unit );
                            GameBattleUnitManager.instance.addNpc( unit );
                        }
                        else
                        {
                            Debug.LogError( "0x44 error" );
                            return;
                        }

                        short x = 0;
                        short y = 0;

                        GameBattleManager.instance.getMapEvnetPoint( eventID , ref x , ref y );
                        unit.BattleAIType = GameBattleAIType.AIMoveToPos;
                        unit.AIMoveToX = x;
                        unit.AIMoveToY = y;
                    }

                    nextEvent();
                }
                break;
            case 0x45:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dat = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x45 set weapon" + id + " " + dat );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( id );

                    GameUserData.instance.setGameData( dat , unit.WeaponID );

                    nextEvent();
                }
                break;
            case 0x46:
                {
#if UNITY_EDITOR
                    Debug.Log( "0x46 damage all users" );
#endif

                    GameBattleUnitManager.instance.damageAllUser( nextEvent );
                }
                break;
            case 0x47:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dat = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x47 set map event " + id + " " + dat );
#endif

                    int d = GameUserData.instance.getGameData( (int)GameUserDataType.MapEvent6 );

                    if ( d != GameDefine.INVALID_ID && d < GameDefine.MAX_USER )
                    {
                        GameUserData.instance.setGameData( dat , d );
                    }

                    nextEvent();
                }
                break;
            case 0x48:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dat = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.Log( "0x48 set map event " + id + " " + dat );
#endif

                    bool all = true;
                    for ( int i0 = 0 ; i0 < GameBattleUnitManager.instance.User.Count ; i0++ )
                    {
                        if ( GameBattleUnitManager.instance.User[ i0 ].IsAlive )
                        {
                            GameBattleDTL.Point point = GameBattleManager.instance.getPoint( GameBattleUnitManager.instance.User[ i0 ].PosX , 
                                GameBattleUnitManager.instance.User[ i0 ].PosY );

                            if ( !GameBattleManager.instance.MapEvent.ContainsKey( point.MapEvent ) )
                            {
                                all = false;
                            }
                        }
                    }

                    if ( all )
                    {
                        GameUserData.instance.setGameData( dat , 1 );
                    }
                    else
                    {
                        GameUserData.instance.setGameData( dat , 0 );
                    }

                    nextEvent();
                }
                break;
            case 0x49:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short dat = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.LogError( "0x49 remove item ? " + id + " " + dat );
#endif

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( 0 );

                    unit.removeItemID( 14 );
                    unit.addItem( 0 );
                    unit.WeaponID = 0;

                    nextEvent();
                }
                break;
            case 0x4A:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x4A damage user" );
#endif

                    GameBattleUnitManager.instance.damageUser( id , nextEvent );
                }
                break;
            case 0x4B:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.Log( "0x4B cure user ?" );
#endif

                    nextEvent();
//                    GameBattleUnitManager.instance.damageUser( id , nextEvent );
                }
                break;
            case 0x4C:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short check = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.LogError( "0x4C show unit " + id + " " + check );
#endif

                    GameBattleUnitManager.instance.showUnits( GameUserData.instance.getGameData( id ) , check );
                }
                break;
            case 0x4D:
                {
                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.LogError( "0x4D check % 2 ? " + id + " " + id2 + "  " + GameUserData.instance.getGameData( id ) % 2 );
#endif

                    GameUserData.instance.setGameData( id2 , GameUserData.instance.getGameData( id ) % 2 );

                    nextEvent();
                }
                break;
            case 0x4E:
                {
                    // get item

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );

                    GameUserData.instance.addItem( id );

#if UNITY_EDITOR
                    Debug.LogError( "get item " + id );
#endif

                    nextEvent();
                }
                break;
            case 0x4F:
                {
                    // unkonw
                    //nextEvent();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short unkonw = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    // stage8 9,0 stage26 1,1
                    Debug.LogError( "unknow 4f " + id + " " + unkonw );
#endif

                    nextEvent();
                }
                break;
            case 0x50:
                {
                    // unkonw
                    //nextEvent();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short unkonw = BitConverter.ToInt16( activeEvent.Data , 0 );

#if UNITY_EDITOR
                    Debug.LogError( "unknow 0x50 " + id + " " + unkonw );
#endif

                    nextEvent();
                }
                break;
            case 0x51:
                {
                    // unkonw
                    //nextEvent();

                    GameBattleManager.instance.unShowLayer( 1 );

#if UNITY_EDITOR
                    Debug.LogError( "unknow 0x51 " );
#endif

                    nextEvent();
                }
                break;
            case 0x53:
                {
                    // unkonw
                    //nextEvent();

                    short id = BitConverter.ToInt16( activeEvent.Data , 0 );
                    short id2 = BitConverter.ToInt16( activeEvent.Data , 2 );

#if UNITY_EDITOR
                    Debug.LogError( "check item " + id + " " + id2 + " " + GameUserData.instance.haveItem( id ) );
#endif

                    if ( GameUserData.instance.haveItem( id ) )
                    {
                        GameUserData.instance.setGameData( id2 , 1 );
                    }
                    else
                    {
                        GameUserData.instance.setGameData( id2 , 0 );
                    }

                    nextEvent();
                }
                break;
            case 0x56:
                {
                    nextEvent();
                }
                break;
            case 0x54:
                {
                    string code = "\n";
                    code += GameUserData.instance.Proficiency;
                    code += GameUserData.instance.Time;
                    code += GameUserData.instance.Proficiency * GameUserData.instance.Time;
                    code += "           ";

                    GameMsgBoxUI.instance.showText( GameDefine.INVALID_ID , GameDefine.INVALID_ID , GameDefine.INVALID_ID ,
                        GameStringData.instance.getString( GameStringType.GameOver3 ) + code , returnTitle );
                }
                break;
            case 0:
                // none
                {
                    activeStep = 9999;
                    StartCoroutine( nextEvent( 0.25f ) );
                }
                break;
            default:
                {
                    Debug.LogError( "doEvent type: " + GameUserData.instance.Stage + " " + activeID + " " + activeStep + " " + String.Format( "{0:X}" , activeEvent.Type ) );
                    nextEvent();
                }
                break;
        }
    }

}


