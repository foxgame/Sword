using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class GameManager : SingletonManager< GameManager >
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    internal extern static int joinGroup1();
    [DllImport("__Internal")]
    internal extern static int getMsg2();
    [DllImport("__Internal")]
    internal extern static string getMsg1();
#endif

    public bool getKey( GameInputCode c )
    {
        switch ( c )
        {
            case GameInputCode.Up:
                {
                    return Input.GetKey( KeyCode.W ) || Input.GetKey( KeyCode.UpArrow );
                }
            case GameInputCode.Down:
                {
                    return Input.GetKey( KeyCode.S ) || Input.GetKey( KeyCode.DownArrow );
                }
            case GameInputCode.Left:
                {
                    return Input.GetKey( KeyCode.A ) || Input.GetKey( KeyCode.LeftArrow );
                }
            case GameInputCode.Right:
                {
                    return Input.GetKey( KeyCode.D ) || Input.GetKey( KeyCode.RightArrow );
                }
            case GameInputCode.Confirm:
                {
                    return Input.GetKey( KeyCode.Return );
                }
            case GameInputCode.Cancel:
                {
                    return Input.GetKey( KeyCode.Escape );
                }
        }

        return false;
    }

    public bool getKeyDown( GameInputCode c )
    {
        switch ( c )
        {
            case GameInputCode.Up:
                {
                    return Input.GetKeyDown( KeyCode.W ) || Input.GetKeyDown( KeyCode.UpArrow );
                }
            case GameInputCode.Down:
                {
                    return Input.GetKeyDown( KeyCode.S ) || Input.GetKeyDown( KeyCode.DownArrow );
                }
            case GameInputCode.Left:
                {
                    return Input.GetKeyDown( KeyCode.A ) || Input.GetKeyDown( KeyCode.LeftArrow );
                }
            case GameInputCode.Right:
                {
                    return Input.GetKeyDown( KeyCode.D ) || Input.GetKeyDown( KeyCode.RightArrow );
                }
            case GameInputCode.Confirm:
                {
                    return Input.GetKeyDown( KeyCode.Return );
                }
            case GameInputCode.Cancel:
                {
                    return Input.GetKeyDown( KeyCode.Escape );
                }
        }

        return false;
    }

    public override void initSingleton()
    {
    }

    public void check()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        
        try
        {
            GamePHP.instance.phpIOSCheck(Application.identifier + "_" + getMsg1() + "_" + getMsg2(), null);
        }
        catch (System.Exception ex)
        {

        }
#endif
    }

    public void init()
    {
        GameUserData.instance.init();
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        GameSetting.instance.init();

        init();

#if UNITY_ANDROID && !UNITY_EDITOR
        if ( PlayerPrefs.GetInt( "Active" , 0 ) == 0 )
        {
            GameActiveUI.instance.show();
        }
#endif




#if UNITY_IPHONE && !UNITY_EDITOR
//        GameIOSPayData.instance.init();
#endif

        //       GameBattleData.instance.saveText();

        //         GameRPGManager.instance.active( 7 );
        //         GameRPGManager.instance.showLayer( 1 );
        //         GameRPGManager.instance.initPos( 1 );

        //         GameBattleManager.instance.active();
        // 
        //         GameBattleManager.instance.clearStage();
        //         GameBattleManager.instance.showLayer( 1 );
        //         GameBattleManager.instance.initMusic();
        // 
        //         GameBattleManager.instance.initTreasures();
        //         GameBattleManager.instance.updateTreasures();
        // 
        //         GameBattleUnitManager.instance.initUnits();
        // 
        //         GameBattleEventManager.instance.showEvent( 4 , 0 , null );

        //        return;





        //         bool f = GamePathFinder.instance.findNearPos( 16 , 10 );
        //         Debug.Log( GamePathFinder.instance.nearPosX + " " + GamePathFinder.instance.nearPosY );

        //         GameUserData.instance.setStage( 4 );
        // 
        //         GameBattleManager.instance.active();
        // 
        //         GameBattleManager.instance.clearStage();
        //         GameBattleManager.instance.showLayer( 1 );
        //         GameBattleManager.instance.initMusic();
        // 
        //         GameBattleManager.instance.initTreasures();
        //         GameBattleManager.instance.updateTreasures();
        // 
        //         GameBattleUnitManager.instance.initUnits();


        //         GameBattleUnitManager.instance.showUnits( 0 );
        //         GameBattleUnitManager.instance.showUnits( 1 );
        //         GameBattleUnitManager.instance.showUnits( 2 );
        //         GameBattleCursor.instance.show( 0 );
        //         GameBattleCursor.instance.moveTo( 16 , 13 );

        //         GameUserData.instance.setGameData( 102 , 1 );
        //         GameUserData.instance.setGameData( 87 , 1 );
        //         GameBattleEventManager.instance.showEvent( 8 , 0 , null );
        //         GameBattleEventManager.instance.showEvent( 2 , 0 , null );

        //        GameBattleTurn.instance.start();



        //        GameBattleEventManager.instance.showEvent( 1 , 0 , GameBattleTurn.instance.start );

        //        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit(1,1);



        //         GameBattleSkillUI.instance.show();
        //         GameBattleSkillUI.instance.setData( GameBattleUnitManager.instance.user[ 1 ].gameBattleMan.unitBase.Skill );


        //         GameBattleUnitMovement.instance.addCell( 16 , 13 , true );
        //         GameBattleUnitMovement.instance.addCell( 17 , 13 , true );
        //         GameBattleUnitMovement.instance.addCell( 18 , 13 , true );
        //         GameBattleUnitMovement.instance.addCell( 19 , 13 , true );

        //        GameBattleUnitMovement.instance.show( 16 , 13 , 25 , 0 , true , -1 );

        //        GameBattleUnitSelectionUI.instance.show( true , true );
        //        GameBattleUnitSelectionUI.instance.setPos( 16 , 13 );


        // 
        //         if ( GameBattleManager.instance.activeStage == 13 )
        //         {
        //             GameBattleEventManager.instance.showUnits();
        //        }

        //         GameMovieManager.instance.playMovie( "Movie/Movie 00" );

    }


}
