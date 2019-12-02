using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleUnitManager : Singleton<GameBattleUnitManager>
{
    [SerializeField]
    List<GameBattleUnit> user = new List<GameBattleUnit>();
    [SerializeField]
    List<GameBattleUnit> enemy = new List<GameBattleUnit>();
    [SerializeField]
    List<GameBattleUnit> npc = new List<GameBattleUnit>();
    [SerializeField]
    GameBattleUnit[] units;

    Transform unitTransform;

    int activeActed;
    int activeEnemy;
    int activeNPC;

    int damageUserID = GameDefine.INVALID_ID;

    bool resetAI = false;

    int enemyCount = 0;

    OnEventOver onEventOver;

    public GameBattleUnit[] Units { get { return units; } }
    public List<GameBattleUnit> User { get { return user; } }
    public List<GameBattleUnit> Enemy { get { return enemy; } }
    public List<GameBattleUnit> Npc { get { return npc; } }

    public bool IsNpcAI { get; set; }
    public bool IsEnemyAI { get; set; }

    public override void initSingleton()
    {
        unitTransform = GameObject.Find( "BattleScene/Unit" ).transform;
    }

    public void updateItem()
    {
        for ( int i = 0 ; i < units.Length ; i++ )
        {
            if ( units[ i ].IsSummon && units[ i ].IsUser )
            {
                for ( int j = 2 ; j < units[ i ].Items.Length ; j++ )
                {
                    if ( units[ i ].Items[ j ] != GameDefine.INVALID_ID )
                    {
                        GameUserData.instance.addItem( units[ i ].Items[ j ] );
                    }
                }
            }
        }
    }

    public void clear()
    {
        GameDefine.DestroyAll( unitTransform );

        user.Clear();
        enemy.Clear();
        npc.Clear();

        activeEnemy = GameDefine.INVALID_ID;
        activeNPC = GameDefine.INVALID_ID;

        enemyCount = 0;

        units = null;
    }

    public void enemyAI()
    {
        GameBattleInput.instance.Pause = true;

        activeEnemy = GameDefine.INVALID_ID;

        enemyCount = getValidEnemyCount();

        if ( enemyCount > 0 )
        {
            doEnemyAI();
        }
        else
        {
            endEnemyAI();
            GameBattleTurn.instance.checkOver();
        }
    }

    public void endEnemyAI()
    {
        IsEnemyAI = false;
        activeEnemy = GameDefine.INVALID_ID;
    }

    public void resetEnemyAI()
    {
        if ( GameBattleTurn.instance.EnemyTurn )
        {
            resetAI = true;
        }
    }

    public void doEnemyAI()
    {
        IsEnemyAI = true;

        if ( resetAI )
        {
            resetAI = false;
            activeEnemy = GameDefine.INVALID_ID;
        }

        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( i > activeEnemy &&
                enemy[ i ].IsAlive &&
                !enemy[ i ].IsActed && 
                enemy[ i ].IsAI )
            {
                activeEnemy = i;
                enemy[ activeEnemy ].AI( doEnemyAI );

                if ( resetAI )
                {
                    resetAI = false;
                    i = 0;
                    activeEnemy = GameDefine.INVALID_ID;
                }

                return;
            }
        }

        // end ai

        endEnemyAI();
    }

    public void npcAI()
    {
        GameBattleInput.instance.Pause = true;

        activeEnemy = GameDefine.INVALID_ID;

        doNpcAI();
    }

    void onCheckUserEventOver()
    {
        GameBattleCursor.instance.show();

        GameBattleUnit unit = GameBattleUnitManager.instance.firstUser();

        if ( unit != null )
        {
            GameBattleInput.instance.lastUnitID = unit.UnitID;

            GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY ,
                GameBattleCursor.SpeedX , GameBattleCursor.SpeedY ,
                true ,
                onMoveCursorOver );
        }

        GameBattleInput.instance.Pause = false;
    }

    void onMoveCursorOver()
    {
        GameBattleInput.instance.Pause = false;
    }

    public void endNpcAI()
    {
        IsNpcAI = false;
        activeNPC = GameDefine.INVALID_ID;

        GameBattleTurn.instance.checkEvent( GameBattleTurnEventType.User , onCheckUserEventOver );       
    }

    public void doNpcAI()
    {
        IsNpcAI = true;

        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            if ( i > activeEnemy &&
                npc[ i ].IsAlive &&
                !npc[ i ].IsActed )
            {
                activeNPC = i;
                npc[ activeNPC ].AI( doNpcAI );
                return;
            }
        }

        // end ai

        endNpcAI();
    }


    public GameBattleXY getUnitXY( int i )
    {
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;

        return stage.XY[ i ];
    }

    public GameBattleUnit getUnit( int i )
    {
        return units[ i ];
    }

    public GameBattleUnit getUnitByID( int id )
    {
        for ( int i = 0 ; i < units.Length ; i++ )
        {
            if ( units[ i ].UnitID == id )
            {
                return units[ i ];
            }
        }

        return null;
    }

    public GameBattleUnit getKilledSummonUnit()
    {
        for ( int i = 0 ; i < units.Length ; i++ )
        {
            if ( units[ i ].IsSummon && units[ i ].IsShow && units[ i ].IsKilled )
            {
                return units[ i ];
            }
        }

        return null;
    }

    public void showUnits( int e )
    {
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;

        for ( int i = 0 ; i < stage.Man.Length ; i++ )
        {
            GameBattleMan man = stage.Man[ i ];

            if ( man.EventParm1 == e )
            {
                GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( i );

                GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( unit.MoveType );

                GameBattleUnit unit1 = GameBattleUnitManager.instance.getUnit( unit.PosX , unit.PosY );

                if ( unit != unit1 &&
                    unit1 != null )
                {
                    GameBattlePathFinder.instance.findNearPos( unit.PosX , unit.PosY , unitMove.block );

                    unit.setPos( GameBattlePathFinder.instance.nearPosX ,
                        GameBattlePathFinder.instance.nearPosY );
                }

//                 if ( GameBattlePathFinder.instance.isBlockPos( unit.PosX , unit.PosY , unitMove.block , unit ) )
//                 {
//                     GameBattlePathFinder.instance.findNearPos( unit.PosX , unit.PosY , unitMove.block );
// 
//                     unit.setPos( GameBattlePathFinder.instance.nearPosX ,
//                         GameBattlePathFinder.instance.nearPosY );
//                 }

                unit.IsShow = true;

                unit.updateAlive();
                unit.playAnimationBattle( GameAnimationType.Stand ,
                    GameAnimationDirection.South , null );
                unit.updateInteam();
                
            }
        }
    }

    public void showUnits( int parm , int check )
    {
        bool b = false;

        GameBattleMan[] Man = GameBattleManager.instance.ActiveBattleStage.Man;
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;

        for ( int i = 0 ; i < Man.Length ; i++ )
        {
            GameBattleMan man = Man[ i ];

            if ( man.EventParm1 == parm )
            {
                GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( i );

                //                 if ( unit.IsKilled )
                //                 {
                //                     unit.initHPMP();
                //                     GameBattleXY xy = stage.XY[ i ];
                //                     unit.setPos( xy.X , xy.Y );
                //                 }

                if ( unit.IsAlive )
                {
                    unit.setPos( stage.XY[ unit.BattleManID ].X , stage.XY[ unit.BattleManID ].Y );
                }

                GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( unit.MoveType );

                GameBattleUnit unit1 = GameBattleUnitManager.instance.getUnit( unit.PosX , unit.PosY );

                if ( check == 1 && 
                    unit != unit1 &&
                    unit1 != null )
                {
                    GameBattlePathFinder.instance.findNearPos( unit.PosX , unit.PosY , unitMove.block );

                    unit.setPos( GameBattlePathFinder.instance.nearPosX ,
                        GameBattlePathFinder.instance.nearPosY );
                }

                if ( check == 1 &&
                    GameBattlePathFinder.instance.isBlockPos( unit.PosX , unit.PosY , unitMove.block , unit ) )
                {
                    GameBattlePathFinder.instance.findNearPos( unit.PosX , unit.PosY , unitMove.block );

                    unit.setPos( GameBattlePathFinder.instance.nearPosX ,
                        GameBattlePathFinder.instance.nearPosY );
                }

                unit.IsShow = true;
                //                            unit.IsActed = true;

                if ( unit.IsKilled )
                {
                    unit.IsKilled = false;
                    unit.setAlpha( 1.0f );
                    unit.gameObject.SetActive( true );
                }


                unit.updateAlive();
                unit.playAnimationBattle( GameAnimationType.Stand ,
                    GameAnimationDirection.South , null );
                unit.updateInteam();

//                 if ( !b && GameBattleTurn.instance.Turn > 1 )
//                 {
//                     if ( GameUserData.instance.Stage == 12 && parm == 4 )
//                     {
//                         // stage 12 bug
//                         continue;
//                     }
// 
//                     b = true;
// 
//                     GameBattleCursor.instance.show();
//                     GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY ,
//                                     GameBattleCursor.SpeedX , GameBattleCursor.SpeedY ,
//                                      onShowUnitOver );
//                 }
            }
        }

        GameBattleUnitManager.instance.resetEnemyAI();

        if ( !b )
        {
            onShowUnitOver();
        }
    }


    void onShowUnitOver()
    {
        GameBattleCursor.instance.unShow();
        GameBattleEventManager.instance.nextEvent();
    }

    public void initUnits()
    {
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;


        units = new GameBattleUnit[ stage.Man.Length ];

        for ( int i = 0 ; i < stage.Man.Length ; i++ )
        {
            GameUnit unit = GameUnitData.instance.getData( stage.Man[ i ].UnitBase.UnitID );

            string path = "Prefab/Sprite/man" + GameDefine.getString3( unit.Sprite ) + "/";
            path += ( GameDefine.getString3( unit.Sprite ) + "man" );

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            GameAnimation ani = obj.GetComponent<GameAnimation>();
            obj.name = "man" + GameDefine.getString3( unit.Sprite ) + " " + i + " " + stage.Man[ i ].UnitBase.UnitID;

            Transform trans = obj.transform;
            trans.SetParent( unitTransform );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

            GameBattleMovement movement = obj.AddComponent<GameBattleMovement>();
            GameBattleUnit battleUnit = obj.AddComponent<GameBattleUnit>();

            battleUnit.init( unit , stage.Man[ i ].clone() );

            if ( battleUnit.UnitID < GameDefine.MAX_USER )
            {
                battleUnit.setUnitBase( GameUserData.instance.getUnitBase( battleUnit.UnitID ) );
            }

            battleUnit.updateUnitData();
            battleUnit.initHPMP();
            battleUnit.clearMove();

            if ( battleUnit.IsUser )
            {
                addUser( battleUnit );
            }
            else if( battleUnit.IsEnemy )
            {
                addEnemy( battleUnit );
            }
            else if ( battleUnit.IsNpc )
            {
                addNpc( battleUnit );
            }

            units[ i ] = battleUnit;

            if ( stage.XY.Length <= i )
            {

            }
            else
            {
                GameBattleXY xy = stage.XY[ i ];
                movement.setPos( xy.X , xy.Y );
            }
        }
    }

    public GameBattleUnit summonUnit( GameBattleUnit u , int id , int x , int y )
    {
        GameBattleUnit battleUnit = getUnitByID( id );

        if ( battleUnit == null )
        {
            return null;
        }

        if ( battleUnit.IsShow || battleUnit.IsKilled )
        {
            return null;
        }

        GameUnit unit = GameUnitData.instance.getData( battleUnit.UnitID );

        battleUnit.IsShow = true;
        battleUnit.IsSummon = true;

        GameUnitBase unitBaseU = u.getUnitBase();
        GameUnitBase unitBase = battleUnit.getUnitBase();

        unitBase.LV = unitBaseU.LV;

        unitBase.HP = (short)( unitBaseU.HP * unit.HP / 100.0f );
        unitBase.MP = (short)( unitBaseU.MP * unit.MP / 100.0f );
        unitBase.Str = (short)( unitBaseU.Str * unit.Str / 100.0f );
        unitBase.Vit = (short)( unitBaseU.Vit * unit.Vit / 100.0f );
        unitBase.Avg = (short)( unitBaseU.Avg * unit.Avg / 100.0f );
        unitBase.Int = (short)( unitBaseU.Int * unit.Int / 100.0f );
        unitBase.Luk = (short)( unitBaseU.Luk * unit.Luk / 100.0f );

        battleUnit.updateUnitData();

        battleUnit.initHPMP();
        battleUnit.clearMove();

        battleUnit.setPos( x , y );
        battleUnit.updateAnimation( GameAnimationDirection.South );
        battleUnit.updateAlive();

        return battleUnit;
    }

    public GameBattleUnit reliveSummonUnit( int x , int y )
    {
        GameBattleUnit unit = getKilledSummonUnit();

        if ( unit == null )
        {
            return null;
        }

        unit.IsShow = true;
        unit.IsKilled = false;

        unit.addHP( 1 );
        unit.clearMove();

        unit.setPos( x , y );
        unit.updateAnimation( GameAnimationDirection.South );
        unit.updateAlive();

        return unit;
    }

    public GameBattleUnit getUnit( int x , int y , GameBattleUnit u = null )
    {
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            GameBattleUnit unit = user[ i ];

            if ( unit == u )
            {
                continue;
            }

            if ( !unit.IsAlive )
            {
                continue;
            }

            if ( unit.checkPosition( x , y ) )
            {
                return unit;
            }
        }

        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            GameBattleUnit unit = enemy[ i ];

            if ( unit == u )
            {
                continue;
            }

            if ( !unit.IsAlive )
            {
                continue;
            }

            if ( unit.checkPosition( x , y ) )
            {
                return unit;
            }
        }

        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            GameBattleUnit unit = npc[ i ];

            if ( unit == u )
            {
                continue;
            }

            if ( !unit.IsAlive )
            {
                continue;
            }

            if ( unit.checkPosition( x , y ) )
            {
                return unit;
            }
        }
        
        return null;
    }


    public void addUser( GameBattleUnit unit )
    {
        user.Add( unit );
    }

    public void addEnemy( GameBattleUnit unit )
    {
        enemy.Add( unit );
    }

    public void addNpc( GameBattleUnit unit )
    {
        npc.Add( unit );
    }


    public void removeUser( GameBattleUnit unit )
    {
        user.Remove( unit );
    }

    public void removeEnemy( GameBattleUnit unit )
    {
        enemy.Remove( unit );
    }

    public void removeNpc( GameBattleUnit unit )
    {
        npc.Remove( unit );
    }

    public void clearEnemy()
    {
        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( enemy[ i ].KillEvent != GameDefine.INVALID_ID )
            {
                continue;
            }

            if ( enemy[ i ].IsAlive )
            {
                enemy[ i ].kill();
                enemy[ i ].fadeOut( null );
            }
        }
    }

    public void clearNPC()
    {
        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            if ( npc[ i ].IsAlive )
            {
                npc[ i ].kill();
                npc[ i ].fadeOut( null );
            }
        }
    }


    void onDoActedOver()
    {
        if ( GameBattleTurn.instance.UserTurn )
        {
            GameBattleUnitManager.instance.clearUserActed();
            GameBattleUnitManager.instance.clearNpcActed();
        }
        else
        {
            GameBattleUnitManager.instance.clearEnemyActed();
        }

        onEventOver();
    }


    public void damageUser( int id , OnEventOver over )
    {
        onEventOver = over;

        damageUserID = id;

        List<GameBattleAttackResultSkill> resultSkill = new List<GameBattleAttackResultSkill>();

        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].UnitID == id &&
                user[ i ].IsAlive )
            {
                GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                r.type = GameSkillResutlType.Damage;
                r.unit = user[ i ];
                r.HP = (int)( user[ i ].HPMax * 0.33f );

                resultSkill.Add( r );

                GameBattleCursor.instance.moveTo( user[ i ].PosX , user[ i ].PosY , true );
            }
        }

        GameBattleAttackMap.instance.showRes( resultSkill , onDamageUserResOver );
    }

    void onDamageUserResOver()
    {
        bool b = false;

        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive && user[ i ].UnitID == damageUserID )
            {
                user[ i ].addHP( -(int)( user[ i ].HPMax * 0.33f ) );
                user[ i ].IsHurt = true;

                if ( user[ i ].IsKilled )
                {
                    user[ i ].fadeOut( null );
                    b |= GameBattleJudgment.instance.check( user[ i ] , onDamageAllUserOver );
                }
            }
        }

        if ( !b )
        {
            onDamageAllUserOver();
        }
    }

    public void damageAllUser( OnEventOver over )
    {
        onEventOver = over;

        List<GameBattleAttackResultSkill> resultSkill = new List<GameBattleAttackResultSkill>();

        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive )
            {
                GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                r.type = GameSkillResutlType.Damage;
                r.unit = user[ i ];
                r.HP = (int)( user[ i ].HPMax * 0.33f );

                resultSkill.Add( r );
            }
        }

        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            if ( npc[ i ].IsAlive )
            {
                GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                r.type = GameSkillResutlType.Damage;
                r.unit = npc[ i ];
                r.HP = (int)( npc[ i ].HPMax * 0.33f );

                resultSkill.Add( r );
            }
        }

        GameBattleAttackMap.instance.showRes( resultSkill , onDamageAllUserResOver );
    }


    void onDamageAllUserResOver()
    {
        bool b = false;

        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive )
            {
                user[ i ].addHP( -(int)( user[ i ].HPMax * 0.33f ) );
                user[ i ].IsHurt = true;

                if ( user[ i ].IsKilled )
                {
                    user[ i ].fadeOut( null );
                    b |= GameBattleJudgment.instance.check( user[ i ] , onDamageAllUserOver );
                }
            }
        }

        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            if ( npc[ i ].IsAlive )
            {
                npc[ i ].addHP( -(int)( npc[ i ].HPMax * 0.33f ) );
                npc[ i ].IsHurt = true;

                if ( npc[ i ].IsKilled )
                {
                    npc[ i ].fadeOut( null );
                    b |= GameBattleJudgment.instance.check( npc[ i ] , onDamageAllUserOver );
                }
            }
        }

        if ( !b )
        {
            onDamageAllUserOver();
        }
    }

    void onDamageAllUserOver()
    {
        onEventOver();
    }


    void onDoActedResOver()
    {
        bool b = false;

        if ( GameBattleTurn.instance.UserTurn )
        {
            for ( int i = 0 ; i < user.Count ; i++ )
            {
                if ( user[ i ].IsAlive && 
                    user[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    user[ i ].addHP( -(int)( user[ i ].HPMax * 0.33f ) );
                    user[ i ].IsHurt = true;

                    if ( user[ i ].IsKilled )
                    {
                        user[ i ].fadeOut( null );
                        b |= GameBattleJudgment.instance.check( user[ i ] , onDoActedOver );
                    }
                }
            }

            for ( int i = 0 ; i < npc.Count ; i++ )
            {
                if ( npc[ i ].IsAlive && 
                    npc[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    npc[ i ].addHP( -(int)( npc[ i ].HPMax * 0.33f ) );
                    npc[ i ].IsHurt = true;

                    if ( npc[ i ].IsKilled )
                    {
                        npc[ i ].fadeOut( null );
                        b |= GameBattleJudgment.instance.check( npc[ i ] , onDoActedOver );
                    }
                }
            }
        }
        else
        {
            for ( int i = 0 ; i < enemy.Count ; i++ )
            {
                if ( enemy[ i ].IsAlive && 
                    enemy[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    enemy[ i ].addHP( -(int)( enemy[ i ].HPMax * 0.33f ) );
                    enemy[ i ].IsHurt = true;

                    if ( enemy[ i ].IsKilled )
                    {
                        enemy[ i ].fadeOut( null );
                        b |= GameBattleJudgment.instance.check( enemy[ i ] , onDoActedOver );
                    }
                }
            }
        }

        if ( !b )
        {
            onDoActedOver();
        }
    }

    void onDoActedMoveOver()
    {
        GameBattleCursor.instance.moveTo( GameBattleSceneMovement.instance.PosX + GameCameraManager.instance.xCell ,
            GameBattleSceneMovement.instance.PosY + GameCameraManager.instance.yCell , true );

        List<GameBattleAttackResultSkill> resultSkill = new List<GameBattleAttackResultSkill>();

        if ( GameBattleTurn.instance.UserTurn )
        {
            for ( int i = 0 ; i < user.Count ; i++ )
            {
                if ( user[ i ].IsAlive && 
                    user[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                    r.type = GameSkillResutlType.Damage;
                    r.unit = user[ i ];
                    r.HP = (int)( user[ i ].HPMax * 0.33f );

                    resultSkill.Add( r );
                }
            }

            for ( int i = 0 ; i < npc.Count ; i++ )
            {
                if ( npc[ i ].IsAlive && 
                    npc[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                    r.type = GameSkillResutlType.Damage;
                    r.unit = npc[ i ];
                    r.HP = (int)( npc[ i ].HPMax * 0.33f );

                    resultSkill.Add( r );
                }
            }
        }
        else
        {
            for ( int i = 0 ; i < enemy.Count ; i++ )
            {
                if ( enemy[ i ].IsAlive && 
                    enemy[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                    r.type = GameSkillResutlType.Damage;
                    r.unit = enemy[ i ];
                    r.HP = (int)( enemy[ i ].HPMax * 0.33f );

                    resultSkill.Add( r );
                }
            }
        }

        GameBattleAttackMap.instance.showRes( resultSkill , onDoActedResOver );
    }

    public void doActed( OnEventOver over )
    {
        onEventOver = over;

        GameBattleInput.instance.Pause = true;

        if ( GameBattleTurn.instance.UserTurn )
        {
            for ( int i = 0 ; i < user.Count ; i++ )
            {
                if ( user[ i ].IsAlive && 
                    user[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleSceneMovement.instance.moveTo( user[ i ].PosX - GameCameraManager.instance.xCell ,
                        user[ i ].PosY - GameCameraManager.instance.yCell , 50 , 50 , onDoActedMoveOver );

                    return;
                }
            }

            for ( int i = 0 ; i < npc.Count ; i++ )
            {
                if ( npc[ i ].IsAlive && 
                    npc[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleSceneMovement.instance.moveTo( npc[ i ].PosX - GameCameraManager.instance.xCell ,
                        npc[ i ].PosY - GameCameraManager.instance.yCell , 50 , 50 , onDoActedMoveOver );

                    return;
                }
            }
        }
        else
        {
            for ( int i = 0 ; i < enemy.Count ; i++ )
            {
                if ( enemy[ i ].IsAlive && 
                    enemy[ i ].checkEffect( GameSkillResutlEffect.Poison ) )
                {
                    GameBattleSceneMovement.instance.moveTo( enemy[ i ].PosX - GameCameraManager.instance.xCell ,
                        enemy[ i ].PosY - GameCameraManager.instance.yCell , 50 , 50 , onDoActedMoveOver );

                    return;
                }
            }
        }

        onDoActedOver();
    }

    public void clearUserActed()
    {
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            user[ i ].clearActed();
        }
    }

    public void clearEnemyActed()
    {
        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            enemy[ i ].clearActed();
        }
    }

    public void clearNpcActed()
    {
        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            npc[ i ].clearActed();
        }
    }


    public int getValidUserCount()
    {
        int c = 0;
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive &&
                !user[ i ].IsActed )
            {
                c++;
            }
        }

        return c;
    }

    public int getValidEnemyCount()
    {
        int c = 0;
        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( enemy[ i ].IsAlive &&
                !enemy[ i ].IsActed && 
                enemy[ i ].IsAI )
            {
                c++;
            }
        }

        return c;
    }

    public GameBattleUnit firstUser()
    {
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive &&
                !user[ i ].IsActed )
            {
                return user[ i ];
            }
        }

        return null;
    }

    public GameBattleUnit nextUser( int id )
    {
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].UnitID > id && 
                user[ i ].IsAlive &&
                !user[ i ].IsActed )
            {
                return user[ i ];
            }
        }

        return firstUser();
    }

    public int getUserCount( int id = GameDefine.INVALID_ID )
    {
        int c = 0;

        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive &&
                id != i )
            {
                c++;
            }
        }

        return c;
    }

    public int getNpcCount()
    {
        int c = 0;

        for ( int i = 0 ; i < npc.Count ; i++ )
        {
            if ( npc[ i ].IsAlive )
            {
                c++;
            }
        }

        return c;
    }

    public int getEnemyCount( int id = GameDefine.INVALID_ID )
    {
        int c = 0;

        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( enemy[ i ].IsAlive && 
                id != i &&
                enemy[ i ].IsAI )
            {
                c++;
            }
        }

        return c;
    }


    public bool userAllKilled()
    {
        for ( int i = 0 ; i < user.Count ; i++ )
        {
            if ( user[ i ].IsAlive )
            {
                return false;
            }
        }

        return true;
    }

    public bool enemyAllKilled()
    {
        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( enemy[ i ].IsAlive )
            {
                return false;
            }
        }

        return true;
    }

    public int enemyKilledCount()
    {
        int c = 0;
        for ( int i = 0 ; i < enemy.Count ; i++ )
        {
            if ( enemy[ i ].IsKilled )
            {
                c++;
            }
        }

        return c;
    }


}
