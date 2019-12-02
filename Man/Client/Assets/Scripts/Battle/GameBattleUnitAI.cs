using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class GameBattleUnit : MonoBehaviour
{
    enum GameBattleUnitAIType
    {
        None = -1,

        Move,
        Attack,
        Skill,
        Item,

        Count
    }

    GameBattleUnitAIType gameBattleUnitAIType;

    OnEventOver onEventOverAI;

    List<GameBattleUnit> listUnitsAI = new List<GameBattleUnit>();
    List<GameBattleUnitMovement.Cell> listCellsAI = new List<GameBattleUnitMovement.Cell>();
    GameSkill gameSkillAI;
    GameItem gameItemAI;

    int targetIndexAI = 0;
    int moveCostAI = 0;

    void AIrandomTarget( int targetID )
    {
        int[] r = new int[ listUnitsAI.Count + 1 ];

        int count = 0;

        for ( int i = 0 ; i < listUnitsAI.Count ; i++ )
        {
            float hp = ( 1.0f - listUnitsAI[ i ].HP / (float)listUnitsAI[ i ].HPMax );
            float ahp = ( ( PhysicalAttack - listUnitsAI[ i ].Defence ) / (float)listUnitsAI[ i ].HPMax );

            int rhp = (int)( hp * 100 * GameDefine.Difficulty );
            int ratt = (int)( ahp * 100 * GameDefine.Difficulty );

            r[ i ] = rhp + ratt;

            if ( targetID != GameDefine.INVALID_ID &&
                targetID == listUnitsAI[ i ].BattleManID )
            {
                targetIndexAI = i;
                return;
            }
        }

        r[ listUnitsAI.Count ] = 25;

        for ( int i = 0 ; i < r.Length ; i++ )
        {
            count += r[ i ];
        }

        int rr = Random.Range( 0 , count );

        count = 0;
        for ( int i = 0 ; i < r.Length - 1 ; i++ )
        {
            if ( r[ i ] + count > rr )
            {
                targetIndexAI = i;
                return;
            }

            count += r[ i ];
        }

        targetIndexAI = Random.Range( 0 , listUnitsAI.Count );
    }

    public void AI( OnEventOver over )
    {
        onEventOverAI = over;

        GameBattleCursor.instance.show();
        GameBattleCursor.instance.moveTo( PosX , PosY ,
                   GameBattleCursor.SpeedX15 , GameBattleCursor.SpeedY15 , 
                   true ,
                   AIFindNearEnemy );


//         GameBattleSceneMovement.instance.moveTo( PosX - GameCameraManager.instance.xCell ,
//             PosY - GameCameraManager.instance.yCell , 
//             GameBattleCursor.SpeedX , GameBattleCursor.SpeedY , null );
    }

    bool AIUseItem( List<GameBattleUnitMovement.Cell> cells )
    {
        gameItemAI = null;

        int itemIndex = GameDefine.INVALID_ID;

        for ( int i = 0 ; i < Items.Length ; i++ )
        {
            if ( Items[ i ] != GameDefine.INVALID_ID )
            {
                GameItem item = GameItemData.instance.getData( Items[ i ] );

                if ( item.UseType == GameItemUseType.Cure )
                {
                    itemIndex = i;
                    gameItemAI = item;
                    break;
                }
            }
        }

        if ( itemIndex == GameDefine.INVALID_ID )
        {
            return false;
        }

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( MoveType );
        bool fly = checkEffect( GameSkillResutlEffect.Under ) ? true : unitMove.fly;

        GameBattleUnit moveToUnit1 = null;
        GameBattleUnitMovement.Cell cellMove1 = null;

        for ( int j = 0 ; j < cells.Count ; j++ )
        {
            int size = 1;
            for ( int i0 = -size ; i0 <= size ; i0++ )
            {
                for ( int j0 = -size ; j0 <= size ; j0++ )
                {
                    if ( Mathf.Abs( i0 ) + Mathf.Abs( j0 ) > size )
                    {
                        continue;
                    }

                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( cells[ j ].x + i0 , cells[ j ].y + j0 );

                    if ( unit != null && unit != this && unit.UnitCampType == UnitCampType && ( unit.HP / (float)unit.HPMax < 0.4f ) )
                    {
                        if ( moveToUnit1 == null || 
                            Random.Range( 0 , 100 ) > 50 )
                        {
                            moveToUnit1 = unit;
                            cellMove1 = cells[ j ];
                        }

                    }
                }
            }

        }

        if ( moveToUnit1 != null )
        {
            GameBattleUnitItemSelection.instance.setAttackAI( moveToUnit1 , moveToUnit1.PosX , moveToUnit1.PosY );

            gameBattleUnitAIType = GameBattleUnitAIType.Item;

            if ( cellMove1.x != PosX || cellMove1.y != PosY )
            {
                bool b = moveTo( cellMove1.x , cellMove1.y , true , false , 3 , AIOnMoveOver );

                if ( !b )
                {
                    return false;
                }
                else
                {
                    GameBattleCursor.instance.show();
                    GameBattleCursor.instance.moveTo( cellMove1.x , cellMove1.y ,
                        GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 ,
                        true ,
                        null );
                }
            }
            else
            {
                AIOnMoveOver();
            }

            return true;
        }

       


        if ( HP / (float)HPMax < 0.4f )
        {
            List<GameBattleUnit> User = GameBattleUnitManager.instance.User;
            List<GameBattleUnit> Enemy = GameBattleUnitManager.instance.Enemy;
            List<GameBattleUnit> Npc = GameBattleUnitManager.instance.Npc;

            GameBattleUnit moveToUnit = null;
            GameBattleUnitMovement.Cell cellMove = null;
            int disMove = 999;
            int cost = 999;

            moveCostAI = 0;

            if ( UnitCampType == GameUnitCampType.User )
            {
                for ( int j = 0 ; j < Enemy.Count ; j++ )
                {
                    GameBattleUnit enemy = Enemy[ j ];

                    if ( !enemy.IsAlive )
                    {
                        continue;
                    }

                    enemy.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , enemy.PosX , enemy.PosY ,
                        unitMove.block , fly , UnitCampType );

                    enemy.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = enemy;
                    }
                }
            }
            else
            {
                for ( int j = 0 ; j < Npc.Count ; j++ )
                {
                    GameBattleUnit npc = Npc[ j ];

                    if ( !npc.IsAlive )
                    {
                        continue;
                    }

                    npc.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , npc.PosX , npc.PosY ,
                        unitMove.block , fly , UnitCampType );

                    npc.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = npc;
                    }
                }

                for ( int j = 0 ; j < User.Count ; j++ )
                {
                    GameBattleUnit user = User[ j ];

                    if ( !user.IsAlive )
                    {
                        continue;
                    }

                    user.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , user.PosX , user.PosY
                        , unitMove.block , fly , UnitCampType );

                    user.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = user;
                    }

                }
            }

            if ( moveToUnit != null )
            {
                int disMove1 = 0;

                for ( int i = 0 ; i < cells.Count ; i++ )
                {
                    GameBattleUnitMovement.Cell cell = cells[ i ];

                    int dis = GameBattlePathFinder.instance.findPath( moveToUnit.PosX , moveToUnit.PosY , cell.x , cell.y
                        , unitMove.block , fly , UnitCampType );

                    if ( ( dis > 0 && dis > disMove1 ) ||
                        ( dis > 0 && dis >= disMove1 && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        disMove1 = dis;
                        cellMove = cell;
                    }
                }
            }

            gameBattleUnitAIType = GameBattleUnitAIType.Item;

            if ( cellMove != null &&
                ( cellMove.x != PosX || cellMove.y != PosY ) )
            {
                GameBattleUnitItemSelection.instance.setAttackAI( this , cellMove.x , cellMove.y );

                bool b = moveTo( cellMove.x , cellMove.y , true , false , 3 , AIOnMoveOver );

                if ( !b )
                {
                    return false;
                }
                else
                {
                    GameBattleCursor.instance.show();
                    GameBattleCursor.instance.moveTo( cellMove.x , cellMove.y ,
                        GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 ,
                        true ,
                        null );
                }
            }
            else
            {
                AIOnMoveOver();
            }

            return true;
        }

        return false;
    }

    void AIFindNearEnemy()
    {
        if ( BattleAIType == GameBattleAIType.AIStay )
        {
            AIOver();
            return;
        }

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( MoveType );
        bool fly = checkEffect( GameSkillResutlEffect.Under ) ? true : unitMove.fly;

        GameBattleUnitMovement.instance.show( PosX , PosY , Move , MoveType , fly , UnitCampType , false );

        List< GameBattleUnitMovement.Cell > cells = GameBattleUnitMovement.instance.Cells;

        listUnitsAI.Clear();
        listCellsAI.Clear();

        gameSkillAI = null;

        GameBattleUnitSkillSelection.instance.AttackUnits.Clear();

        if ( AIUseItem( cells ) )
        {
            return;
        }

        int targetID = AITargetID;

        if ( BattleAIType == GameBattleAIType.AIPositive ||
            BattleAIType == GameBattleAIType.AINegative ||
            battleAICheckFight )
        {
            battleAICheckFight = false;

            // skill

            List<int> useSkills = new List<int>();
            List<GameBattleUnitMovement.Cell> useSkillsCells = new List<GameBattleUnitMovement.Cell>();

            for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
            {
                if ( Skill[ i ] == GameDefine.INVALID_ID )
                {
                    continue;
                }

                gameSkillAI = GameSkillData.instance.getData( Skill[ i ] );

                if ( canUseSkill( gameSkillAI ) )
                {
                    GameBattleUnitSkillSelection.instance.canAttackAI( this , PosX , PosY , PosX , PosY , gameSkillAI , UnitCampType , targetID );

                    if ( GameBattleUnitSkillSelection.instance.AttackUnits.Count != 0 &&
                        GameBattleUnitSkillSelection.instance.IsAttackUnits )
                    {
                        useSkills.Add( i );

                        if ( gameSkillAI.ResultType == GameSkillResutlType.Cure )
                        {
                            break;
                        }
                    }
                }
            }


            if ( useSkills.Count > 0 )
            {
                int r = Random.Range( 0 , useSkills.Count );

                gameSkillAI = GameSkillData.instance.getData( Skill[ useSkills[ r ] ] );

                GameBattleUnitSkillSelection.instance.AttackUnits.Clear();
                GameBattleUnitSkillSelection.instance.canAttackAI( this , PosX , PosY , PosX , PosY , gameSkillAI , UnitCampType , targetID );
                GameBattleUnitSkillSelection.instance.getUnits( this , gameSkillAI );

                gameBattleUnitAIType = GameBattleUnitAIType.Skill;
                AIOnMoveOver();
                return;
            }


            gameSkillAI = null;
            GameBattleUnitSkillSelection.instance.AttackUnits.Clear();

            for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
            {
                if ( Skill[ i ] == GameDefine.INVALID_ID )
                {
                    continue;
                }

                GameSkill skill = GameSkillData.instance.getData( Skill[ i ] );

                for ( int j = 0 ; j < cells.Count ; j++ )
                {
                    if ( canUseSkillMove( skill , cells[ j ].ca ) )
                    {
                        GameBattleUnitSkillSelection.instance.canAttackAI( this , PosX , PosY , cells[ j ].x , cells[ j ].y ,
                            skill , UnitCampType , AIMoveToID );

                        if ( GameBattleUnitSkillSelection.instance.IsAttackUnits )
                        {
                            gameSkillAI = skill;
                        }
                    }
                }
            }


            if ( GameBattleUnitSkillSelection.instance.AttackUnits.Count > 0 &&
                Random.Range( 1 , 100 ) > 10 )
            {
                gameBattleUnitAIType = GameBattleUnitAIType.Skill;

                GameBattleUnitSkillSelection.instance.getUnits( this , gameSkillAI );

                bool b = moveTo( GameBattleUnitSkillSelection.instance.AttackMoveCell.x ,
                    GameBattleUnitSkillSelection.instance.AttackMoveCell.y , true , false , 3 , AIOnMoveOver );

                if ( !b )
                {
                    GameBattleUnitMovement.instance.unShow();
                    AIOver();

#if UNITY_EDITOR
                    Debug.LogError( "error ai skill move " );
#endif
                }
                else
                {
                    GameBattleCursor.instance.moveTo( GameBattleUnitSkillSelection.instance.AttackMoveCell.x ,
                        GameBattleUnitSkillSelection.instance.AttackMoveCell.y ,
                        GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 , 
                        true , 
                        null );
                }

                return;
            }



            // attack

            for ( int i = 0 ; i < cells.Count ; i++ )
            {
                GameBattleUnit u = GameBattleUnitAttackSelection.instance.canAttack( cells[ i ].x , cells[ i ].y , Weapon , UnitCampType );

                if ( u != null )
                {
                    listUnitsAI.Add( u );
                    listCellsAI.Add( cells[ i ] );
                }
            }

            if ( listUnitsAI.Count != 0 )
            {
                AIrandomTarget( targetID );

                gameBattleUnitAIType = GameBattleUnitAIType.Attack;

                GameBattleUnit target = listUnitsAI[ targetIndexAI ];
                GameBattleUnitMovement.Cell cell = listCellsAI[ targetIndexAI ];

                if ( cell.x != PosX ||
                    cell.y != PosY )
                {
                    bool b = moveTo( cell.x , cell.y , true , false , 3 , AIOnMoveOver );

                    if ( !b )
                    {
#if UNITY_EDITOR
                        Debug.LogError( "error ai move " );
#endif

                        GameBattleUnitMovement.instance.unShow();
                        AIOver();
                    }
                    else
                    {
                        GameBattleCursor.instance.moveTo( cell.x , cell.y ,
                            GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 ,
                            true ,
                            null );
                    }
                }
                else
                {
                    AIOnMoveOver();
                }

                return;
            }

            if ( BattleAIType == GameBattleAIType.AINegative )
            {
                GameBattleUnitMovement.instance.unShow();
                AIOver();
                return;
            }

        }


        // move

        List<GameBattleUnit> User = GameBattleUnitManager.instance.User;
        List<GameBattleUnit> Enemy = GameBattleUnitManager.instance.Enemy;
        List<GameBattleUnit> Npc = GameBattleUnitManager.instance.Npc;

        GameBattleUnit moveToUnit = null;
        GameBattleUnitMovement.Cell cellMove = null;
        int disMove = 999;
        int cost = 999;

        moveCostAI = 0;

        if ( BattleAIType == GameBattleAIType.AIPositive )
        {
            if ( UnitCampType == GameUnitCampType.User )
            {
                for ( int j = 0 ; j < Enemy.Count ; j++ )
                {
                    GameBattleUnit enemy = Enemy[ j ];

                    if ( !enemy.IsAlive )
                    {
                        continue;
                    }

                    enemy.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , enemy.PosX , enemy.PosY ,
                        unitMove.block , fly , UnitCampType );

                    enemy.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = enemy;
                    }
                }
            }
            else
            {
                for ( int j = 0 ; j < Npc.Count ; j++ )
                {
                    GameBattleUnit npc = Npc[ j ];

                    if ( !npc.IsAlive )
                    {
                        continue;
                    }

                    npc.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , npc.PosX , npc.PosY ,
                        unitMove.block , fly , UnitCampType );

                    npc.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = npc;
                    }
                }

                for ( int j = 0 ; j < User.Count ; j++ )
                {
                    GameBattleUnit user = User[ j ];

                    if ( !user.IsAlive )
                    {
                        continue;
                    }

                    user.IsShow = false;

                    int dis = GameBattlePathFinder.instance.findPath( PosX , PosY , user.PosX , user.PosY
                        , unitMove.block , fly , UnitCampType );

                    user.IsShow = true;

                    if ( ( dis > 0 && dis < disMove ) ||
                        ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost = GameBattlePathFinder.instance.cost;
                        disMove = dis;
                        moveToUnit = user;
                    }

                }
            }

            if ( moveToUnit != null )
            {
                int disMove1 = 999;
                int cost1 = 999;
                int dis1 = 0;

                for ( int i = 0 ; i < cells.Count ; i++ )
                {
                    GameBattleUnitMovement.Cell cell = cells[ i ];

                    int dis = GameBattlePathFinder.instance.findPath( moveToUnit.PosX , moveToUnit.PosY , cell.x , cell.y
                        , unitMove.block , fly , UnitCampType );

                    for ( int i0 = 0 ; i0 < GameDefine.MAX_SLOT ; i0++ )
                    {
                        if ( Skill[ i0 ] == GameDefine.INVALID_ID )
                        {
                            continue;
                        }

                        GameSkill skill = GameSkillData.instance.getData( Skill[ i0 ] );

                        if ( canUseSkillStand( skill ) )
                        {
                            GameBattleUnitSkillSelection.instance.canAttackAI( this , cell.x , cell.y , cell.x , cell.y , skill , UnitCampType , targetID );

                            if ( GameBattleUnitSkillSelection.instance.AttackUnits.Count != 0 &&
                                GameBattleUnitSkillSelection.instance.IsAttackUnits )
                            {
                                if ( dis > dis1 )
                                {
                                    dis1 = dis;
                                    dis = 1;
                                }
                            }
                        }
                    }

                    if ( ( dis > 0 && dis < disMove1 ) ||
                        ( dis > 0 && dis <= disMove1 && cost1 > GameBattlePathFinder.instance.cost ) ||
                        ( ( dis > 0 && dis <= disMove1 && cost1 == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                    {
                        cost1 = GameBattlePathFinder.instance.cost;
                        disMove1 = dis;
                        cellMove = cell;
                    }
                }

            }

            gameBattleUnitAIType = GameBattleUnitAIType.Move;

            if ( cellMove != null &&
                ( cellMove.x != PosX || cellMove.y != PosY ) )
            {
                int dis2 = GameBattlePathFinder.instance.findPath( PosX , PosY , moveToUnit.PosX , moveToUnit.PosY
                    , unitMove.block , true , UnitCampType );

                if ( dis2 > 0 && 
                    disMove > dis2 )
                {
                    AIOnMoveOver();
                }
                else
                {
                    bool b = moveTo( cellMove.x , cellMove.y , true , false , 3 , AIOnMoveOver );

                    if ( !b )
                    {
                        AIOver();
                    }
                    else
                    {
                        GameBattleCursor.instance.show();
                        GameBattleCursor.instance.moveTo( cellMove.x , cellMove.y ,
                            GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 ,
                            true ,
                            null );
                    }
                }
            }
            else
            {
                AIOnMoveOver();
            }
        }
        else if ( BattleAIType == GameBattleAIType.AIMoveToUnit || 
            BattleAIType == GameBattleAIType.AIMoveToPos )
        {
            int mx = AIMoveToX;
            int my = AIMoveToY;

            if ( BattleAIType == GameBattleAIType.AIMoveToUnit )
            {
                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( AIMoveToID );

                if ( u == null && u.UnitID >= GameDefine.MAX_USER )
                {
                    move();
                    AIOver();
                    battleAICheckFight = true;
//                    BattleAIType = GameBattleAIType.AINegative;
                    return;
                }

                mx = u.PosX;
                my = u.PosY;
            }

            if ( GameBattlePathFinder.instance.isBlockPos( mx , my , unitMove.block ) )
            {
                if ( GameBattlePathFinder.instance.findNearPos( mx , my , PosX , PosY , UnitMove.block ) )
                {
                    mx = GameBattlePathFinder.instance.nearPosX;
                    my = GameBattlePathFinder.instance.nearPosY;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError( "ai cann't move." );
#endif

                    GameBattleUnitMovement.instance.unShow();
                    AIOver();
                    return;
                }
            }
            else
            {
            }

            for ( int i = 0 ; i < cells.Count ; i++ )
            {
                GameBattleUnitMovement.Cell cell = cells[ i ];

                if ( cell.x == mx && cell.y == my )
                {
                    disMove = 2;
                    cellMove = cell;
                    break;
                }

                int dis = GameBattlePathFinder.instance.findPathNear( cell.x , cell.y , mx , my ,
                            unitMove.block , fly , UnitCampType );

                if ( ( dis > 0 && dis < disMove ) ||
                    ( dis > 0 && dis <= disMove && cost > GameBattlePathFinder.instance.cost ) || 
                    ( ( dis > 0 && dis <= disMove && cost == GameBattlePathFinder.instance.cost ) && Random.Range( 0 , 100 ) > 50 ) )
                {
                    cost = GameBattlePathFinder.instance.cost;
                    disMove = dis;
                    cellMove = cell;
                    moveCostAI = cell.ca;
                }
            }


            gameBattleUnitAIType = GameBattleUnitAIType.Move;

            if ( cellMove != null &&
                ( cellMove.x != PosX || cellMove.y != PosY ) )
            {
                int dis2 = GameBattlePathFinder.instance.findPathNear( PosX , PosY , mx , my
                    , unitMove.block , true , UnitCampType );

                if ( dis2 > 0 && 
                    disMove > dis2 )
                {
                    GameBattleUnitMovement.instance.unShow();
                    AIOver();
                }
                else
                {
                    bool b = false;

                    b = moveTo( cellMove.x , cellMove.y , true , false , 3 , AIOnMoveAttackOver );

                    if ( !b )
                    {
                        AIOver();
                    }
                    else
                    {
                        GameBattleCursor.instance.show();
                        GameBattleCursor.instance.moveTo( cellMove.x , cellMove.y ,
                            GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 , 
                            true ,
                            null );
                    }
                }
            }
            else
            {
                AIOnMoveAttackOver();
            }

        }


    }


    void AIOnMoveAttackOver()
    {
        GameBattleUnitMovement.instance.unShow();

        if ( BattleAIType == GameBattleAIType.AIMoveToPos )
        {
            if ( GameBattlePathFinder.instance.getDistance( PosX , PosY , AIMoveToX , AIMoveToY ) < 3 )
            {
                battleAICheckFight = true;
//                BattleAIType = GameBattleAIType.AINegative;
            }
        }

        if ( BattleAIType == GameBattleAIType.AIMoveToUnit )
        {
            GameBattleUnit u = GameBattleUnitManager.instance.getUnit( AIMoveToID );

            if ( u != null &&
                GameBattlePathFinder.instance.getDistance( PosX , PosY , u.PosX , u.PosY ) < 3 )
            {
                battleAICheckFight = true;
//                BattleAIType = GameBattleAIType.AINegative;
            }
        }


        switch ( gameBattleUnitAIType )
        {
            case GameBattleUnitAIType.Move:
                {
                    List<int> useSkills = new List<int>();
                    List<GameBattleUnitMovement.Cell> useSkillsCells = new List<GameBattleUnitMovement.Cell>();

                    for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
                    {
                        if ( Skill[ i ] == GameDefine.INVALID_ID )
                        {
                            continue;
                        }

                        gameSkillAI = GameSkillData.instance.getData( Skill[ i ] );

                        if ( canUseSkillMove( gameSkillAI , moveCostAI ) )
                        {
                            GameBattleUnitSkillSelection.instance.canAttackAI( this , PosX , PosY , PosX , PosY , gameSkillAI , UnitCampType , AIMoveToID );

                            if ( GameBattleUnitSkillSelection.instance.AttackUnits.Count != 0 )
                            {
                                useSkills.Add( i );

                                if ( gameSkillAI.ResultType == GameSkillResutlType.Cure )
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if ( useSkills.Count > 0 )
                    {
                        int r = Random.Range( 0 , useSkills.Count );
                        gameSkillAI = GameSkillData.instance.getData( Skill[ useSkills[ r ] ] );

                        GameBattleUnitSkillSelection.instance.AttackUnits.Clear();
                        GameBattleUnitSkillSelection.instance.canAttackAI( this , PosX , PosY , PosX , PosY , gameSkillAI , UnitCampType , AIMoveToID );
                        GameBattleUnitSkillSelection.instance.getUnits( this , gameSkillAI );

                        gameBattleUnitAIType = GameBattleUnitAIType.Skill;
                        AIOnMoveOver();
                        return;
                    }

                    GameBattleUnit unit = GameBattleUnitAttackSelection.instance.canAttack( PosX , PosY , Weapon , UnitCampType );
                    if ( unit != null )
                    {
                        listUnitsAI.Clear();
                        listUnitsAI.Add( unit );
                        targetIndexAI = 0;

                        gameBattleUnitAIType = GameBattleUnitAIType.Attack;
                        AIOnMoveOver();
                        return;
                    }

                    move();
                    AIOver();
                }
                break;
           
        }



    }

    void AIOnMoveOver()
    {
        if ( AIMoveToID != GameDefine.INVALID_ID )
        {
            GameBattleUnit u = GameBattleUnitManager.instance.getUnit( AIMoveToID );

            if ( u != null && 
                GameBattlePathFinder.instance.getDistance( PosX , PosY , u.PosX , u.PosY ) >= 3 )
            {
                BattleAIType = GameBattleAIType.AIMoveToUnit;
            }
        }

        GameBattleUnitMovement.instance.unShow();

        switch ( gameBattleUnitAIType )
        {
            case GameBattleUnitAIType.Move:
                {
                    move();
                    AIOver();
                }
                break;
            case GameBattleUnitAIType.Attack:
                {
                    if ( listUnitsAI.Count == 0 )
                    {
                        move();
                        AIOver();
                        return;
                    }

                    if ( checkEffect( GameSkillResutlEffect.Palsy ) )
                    {
                        move();
                        AIOver();
                        return;
                    }

                    int x = listUnitsAI[ targetIndexAI ].PosX;
                    int y = listUnitsAI[ targetIndexAI ].PosY;

                    setDirection( x , y );
                    move();

                    GameBattleUnitAttackSelection.instance.show( PosX , PosY , Weapon , UnitCampType , false );

                    GameBattleCursor.instance.show();
                    GameBattleCursor.instance.moveTo( x , y ,
                        GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 ,
                        true ,
                        AIOnPhysicalAttackMoveCursorOver );
                }
                break;
            case GameBattleUnitAIType.Skill:
                {
                    if ( checkEffect( GameSkillResutlEffect.Silence ) )
                    {
                        move();
                        AIOver();
                        return;
                    }

                    GameBattleCursor.instance.show( gameSkillAI.AttackRange );
                    GameBattleCursor.instance.moveTo( GameBattleUnitSkillSelection.instance.AttackCell.x ,
                        GameBattleUnitSkillSelection.instance.AttackCell.y ,
                        GameBattleCursor.SpeedX3 ,
                        GameBattleCursor.SpeedX3,
                        true ,
                        AIOnSkillAttackMoveCursorOver , true );

                    GameBattleUnitSkillSelection.instance.show( PosX , PosY , gameSkillAI , UnitCampType , false );
                }
                break;
            case GameBattleUnitAIType.Item:
                {
                    GameBattleCursor.instance.show( gameItemAI.AttackRange );
                    GameBattleCursor.instance.moveTo( GameBattleUnitItemSelection.instance.AttackCell.x ,
                        GameBattleUnitItemSelection.instance.AttackCell.y ,
                        GameBattleCursor.SpeedX3 ,
                        GameBattleCursor.SpeedX3 ,
                        true ,
                        AIOnItemAttackMoveCursorOver , true );

                    GameBattleUnitItemSelection.instance.showUse( PosX , PosY , gameItemAI , UnitCampType , false );
                }
                break;
        }
    }

    void AIOnItemAttackMoveCursorOver()
    {
        GameBattleUnitItemSelection.instance.unShow();

        GameBattleCursor.instance.unShow();

        GameBattleAttackResult.instance.itemAttack( gameItemAI ,
            this ,
            GameBattleUnitItemSelection.instance.AttackUnits ,
            GameBattleUnitItemSelection.instance.AttackDirection ,
            UnitCampType == GameUnitCampType.User ? GameBattleAttackResultSide.Right : GameBattleAttackResultSide.Left ,
            AIOnItemAttackOver );
    }

    void AIOnSkillAttackMoveCursorOver()
    {
        GameBattleUnitSkillSelection.instance.unShow();

        GameBattleCursor.instance.unShow();

        GameBattleAttackResult.instance.skillAttack( gameSkillAI ,
            this ,
            GameBattleUnitSkillSelection.instance.AttackUnits ,
            GameBattleUnitSkillSelection.instance.AttackDirection ,
            UnitCampType == GameUnitCampType.User ? GameBattleAttackResultSide.Right : GameBattleAttackResultSide.Left ,
            AIOnSkillAttackOver );
    }

    void AIOnPhysicalAttackMoveCursorOver()
    {
        GameBattleUnitAttackSelection.instance.unShow();

        GameBattleCursor.instance.unShow();

        GameBattleAttackResult.instance.PhysicalAttack( this ,
            listUnitsAI[ targetIndexAI ] ,
            UnitCampType == GameUnitCampType.User ? GameBattleAttackResultSide.Right : GameBattleAttackResultSide.Left ,
            AIOnPhysicalAttackOver );
    }
    
    void AIOnPhysicalAttackOver()
    {
        AIOver();
    }

    void AIOnSkillAttackOver()
    {
        AIOver();
    }

    void AIOnItemAttackOver()
    {
        AIOver();
    }

    IEnumerator onAIOver()
    {
        yield return new WaitForSeconds( 0.3f );

        if ( onEventOverAI != null )
        {
            onEventOverAI();
        }
    }

    void AIOver()
    {
#if UNITY_EDITOR
        Debug.Log( "AIOver" );
#endif

        action();

        if ( GameBattleJudgment.instance.IsWin ||
            GameBattleJudgment.instance.IsLose )
        {
            // no more ai
            return;
        }

        GameBattleManager.instance.StartCoroutine( onAIOver() );
    }
}

