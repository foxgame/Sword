using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleInformationUI : GameUI<GameBattleInformationUI>
{
    Text title;
    Text win;
    Text lose;
    Text enemy;
    Text user;
    Text npc;
    Text turn;
    Text proficiency;

    public override void initSingleton()
    {
        proficiency = transform.Find( "proficiency" ).GetComponent<Text>();
        title = transform.Find( "title" ).GetComponent<Text>();
        win = transform.Find( "win" ).GetComponent<Text>();
        lose = transform.Find( "lose" ).GetComponent<Text>();
        enemy = transform.Find( "enemy" ).GetComponent<Text>();
        user = transform.Find( "user" ).GetComponent<Text>();
        npc = transform.Find( "npc" ).GetComponent<Text>();
        turn = transform.Find( "turn" ).GetComponent<Text>();
    }

    public void updateData()
    {
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;

        title.text = stage.SDES.Title;
        win.text = stage.SDES.Win;
        lose.text = stage.SDES.Lose;
        proficiency.text = stage.SDES.Proficiency;

        int enemyCount = GameBattleUnitManager.instance.getEnemyCount();
        string enemyCountStr = enemyCount.ToString();
        if ( enemyCount < 10 )
        {
            enemyCountStr = enemyCountStr.Insert( 0 , "0" );
        }

        int userCount = GameBattleUnitManager.instance.getUserCount();
        string userCountStr = userCount.ToString();
        if ( userCount < 10 )
        {
            userCountStr = userCountStr.Insert( 0 , "0" );
        }

        int npcCount = GameBattleUnitManager.instance.getNpcCount();
        string npcCountStr = npcCount.ToString();
        if ( npcCount < 10 )
        {
            npcCountStr = npcCountStr.Insert( 0 , "0" );
        }

        int turnCount = GameBattleTurn.instance.Turn;
        string turnCountStr = turnCount.ToString();
        if ( turnCount < 10 )
        {
            turnCountStr = turnCountStr.Insert( 0 , "00" );
        }
        else if ( turnCount < 100 )
        {
            turnCountStr = turnCountStr.Insert( 0 , "0" );
        }


        enemy.text = GameDefine.getBigInt( enemyCountStr );
        user.text = GameDefine.getBigInt( userCountStr );
        npc.text = GameDefine.getBigInt( npcCountStr );
        turn.text = GameDefine.getBigInt( turnCountStr );


    }

}
