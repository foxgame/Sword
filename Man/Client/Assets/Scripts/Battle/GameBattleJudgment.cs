using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleJudgment : Singleton<GameBattleJudgment>
{
    bool isWin = false;
    bool isLose = false;
    int eventID = GameDefine.INVALID_ID;

    public int EventID { get { return eventID; } }

    public bool IsWin { get{ return isWin; } }
    public bool IsLose { get { return isLose; } }

    public bool Proficiency8 = true;
    public int Proficiency13 = 0;

    public void clear()
    {
        isWin = false;
        isLose = false;
    }

    public bool check( GameBattleUnit unit , OnEventOver over )
    {
        GameBattleJudge judge = GameBattleManager.instance.ActiveBattleStage.Judge;

        //         GameBattleEventManager.instance.showEvent( 0 , 0 , over );
        // 
        //         return true;

        if ( unit.KillEvent != GameDefine.INVALID_ID )
        {
            GameBattleEventManager.instance.showEvent( unit.KillEvent , 0 , over );
            return true;
        }

        return false;
    }

    public bool check()
    {
        GameBattleJudge judge = GameBattleManager.instance.ActiveBattleStage.Judge;

        for ( int i = 0 ; i < judge.JudgeWin.Length ; i++ )
        {
            GameBattleJudgeWin win = judge.JudgeWin[ i ];

            switch ( win.Judge )
            {
                case GameBattleJudgeJudgeType.KillAll:
                    {
                        if ( GameBattleUnitManager.instance.getEnemyCount( win.ID ) == 0 )
                        {
                            isWin = true;
                            eventID = win.EventID;
                        }
                    }
                    break;
                case GameBattleJudgeJudgeType.KillOne:
                    {
                        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( win.ID );

                        if ( unit == null ||
                            unit.IsKilled )
                        {
                            isWin = true;
                            eventID = win.EventID;
                        }
                    }
                    break;
            }
        }


        for ( int i = 0 ; i < judge.JudgeLose.Length ; i++ )
        {
            GameBattleJudgeLose lose = judge.JudgeLose[ i ];

            switch ( lose.Judge )
            {
                case GameBattleJudgeJudgeType.KillAll:
                    {
                        if ( GameBattleUnitManager.instance.getUserCount( lose.ID ) == 0 )
                        {
                            isLose = true;
                            eventID = lose.EventID;
                        }
                    }
                    break;
                case GameBattleJudgeJudgeType.KillOne:
                    {
                        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( lose.ID );

                        if ( unit == null ||
                            unit.IsKilled )
                        {
                            isLose = true;
                            eventID = lose.EventID;
                        }
                    }
                    break;
            }
        }

        if ( isLose )
        {
            GameBattleEventManager.instance.showEvent( eventID , 0 , null );
            return true;
        }

        if ( isWin )
        {
            UpdateProficiency();
            GameUserData.instance.updateInTeam();

            GameBattleEventManager.instance.showEvent( eventID , 0 , null );

            return true;
        }

        return false;
    }

    public void UpdateProficiency()
    {
        switch ( GameUserData.instance.Stage )
        {
            case 0:
                {
                    if ( GameUserData.instance.hasItem( 178 ) )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 1:
            case 2:
            case 3:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 4:
                {
                    if ( GameBattleUnitManager.instance.enemyKilledCount() == 16 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 5:
            case 6:
            case 7:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 8:
                {
                    if ( Proficiency8 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 9:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 10:
                {
                    // 13 1
                    if ( GameUserData.instance.getGameData( 13 ) == 1 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 11:
                {
                    if ( GameUserData.instance.getGameData( 10 ) == 0 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 12:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 13:
                {
                    if ( Proficiency13 == 2 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 14:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 15:
                {
                    if ( GameUserData.instance.getGameData( 9 ) < 5 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 16:
                {
                    if ( GameUserData.instance.getGameData( 10 ) < 4 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 17:
                {
                    if ( GameBattleTurn.instance.Turn <= 35 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 19:
                {
                    if ( GameBattleUnitManager.instance.enemyKilledCount() <= 8 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 20:
                {
                    if ( GameBattleUnitManager.instance.enemyKilledCount() >= 44 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 21:
                {
                    if ( GameBattleTurn.instance.Turn <= 20 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 22:
                {
                    if ( GameUserData.instance.getGameData( 1 ) == 0 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 23:
            case 24:
            case 25:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 26:
                {
                    if ( GameBattleTurn.instance.Turn >= 15 )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
            case 27:
            case 28:
            case 29:
            case 30:
            case 31:
            case 32:
            case 33:
                {
                    if ( GameBattleManager.instance.getAllTreasures() )
                        GameUserData.instance.ProficiencyStage++;
                }
                break;
        }


        if ( GameUserData.instance.ReloadBattleCount == 0 )
        {
            GameUserData.instance.Proficiency += GameUserData.instance.ProficiencyStage;
        }

#if UNITY_EDITOR
        Debug.LogError( GameUserData.instance.Proficiency );
#endif

        Proficiency8 = true;
        GameUserData.instance.ProficiencyStage = 0;
        GameUserData.instance.ReloadBattleCount = 0;

        GameBattleUnitManager.instance.updateItem();
    }


}


