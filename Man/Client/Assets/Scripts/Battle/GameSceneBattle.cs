using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneBattle : Singleton<GameSceneBattle>
{
    public override void initSingleton()
    {

    }

    void Start()
    {
//          GameUserData.instance.loadBattle();
//         GameBattleEventManager.instance.showEvent( 2 , 0 , null );

        //         Debug.LogError( GameBattleUnitManager.instance.enemyKilledCount() );
        //         GameUserData.instance.setGameData( 13 , 1 );
        // 
        // 13 10话击败夏侯仪
        //        GameUserData.instance.setGameData( 13 , 1 );
        //        GameBattleEventManager.instance.showEvent( 4 , 0 , null );

        //         GameUserData.instance.setGameData( 102 , 1 );
        //         GameUserData.instance.setGameData( 87 , 1 );
        //        GameUserData.instance.setGameData( 10 , 1 );

 //      loadTest();
    }


    public void loadTest()
    {
        GameUserData.instance.setStage( 11 );

        GameBattleManager.instance.clear();
        GameBattleUnitManager.instance.clear();

        GameBattleCursor.instance.unShow();

        GameBattleManager.instance.active();

        GameBattleManager.instance.showLayer( 1 , false );
        GameBattleManager.instance.initMusic();

        GameBattleUnitManager.instance.initUnits();

        GameBattleManager.instance.initTreasures();

        GameBattleTurn.instance.start();
//        GameBattleEventManager.instance.showEvent( 2 , 0 , null );
    }

}


