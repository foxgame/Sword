using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleSelection : Singleton<GameBattleSelection>
{
    public override void initSingleton()
    {
        
    }

    [SerializeField]
    GameBattleUnit selectionUnit;
    [SerializeField]
    List<GameBattleUnit> selectionUnits = new List<GameBattleUnit>();

    public GameBattleUnit SelectionUnit { get { return selectionUnit; } }
    public List<GameBattleUnit> SelectionUnits { get { return selectionUnits; } }


    public void getUnits( GameBattleUnit unit , GameItem item , GameBattleAttackMapDirection dir )
    {
        selectionUnits.Clear();

        switch ( item.UseRangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    int size = GameBattleCursor.instance.Size;

                    for ( int i = -size ; i <= size ; i++ )
                    {
                        for ( int j = -size ; j <= size ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= size )
                            {
                                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX + j ,
                                    GameBattleCursor.instance.PosY + i );

                                switch ( item.UseTargetType )
                                {
                                    case GameTargetType.User:
                                        {
                                            if ( u != null &&
                                                u.UnitCampType == unit.UnitCampType )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Enemy:
                                        {
                                            if ( u != null &&
                                                u.UnitCampType != unit.UnitCampType )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Summon:
                                        {
                                            if ( u == null )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                }

                            }
                        }
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    int x = 0;
                    int y = 0;

                    switch ( dir )
                    {
                        case GameBattleAttackMapDirection.North:
                            y = -1;
                            break;
                        case GameBattleAttackMapDirection.East:
                            x = 1;
                            break;
                        case GameBattleAttackMapDirection.South:
                            y = 1;
                            break;
                        case GameBattleAttackMapDirection.West:
                            x = -1;
                            break;
                    }

                    for ( int i = 1 ; i <= item.AttackRangeMax ; i++ )
                    {
                        GameBattleUnit u = GameBattleUnitManager.instance.getUnit( unit.PosX + i * x , unit.PosY + i * y );

                        switch ( item.UseTargetType )
                        {
                            case GameTargetType.User:
                                {
                                    if ( u != null && 
                                        u.UnitCampType == unit.UnitCampType )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Enemy:
                                {
                                    if ( u != null &&
                                        u.UnitCampType != unit.UnitCampType )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Summon:
                                {
                                    if ( u == null )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                        }
                    }
                }
                break;
        }

    }


    public void getUnits( GameBattleUnit unit , GameSkill skill , GameBattleAttackMapDirection dir )
    {
        selectionUnits.Clear();

        switch ( skill.AttackRangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    int size = GameBattleCursor.instance.Size;

                    for ( int i = -size ; i <= size ; i++ )
                    {
                        for ( int j = -size ; j <= size ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= size )
                            {
                                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX + j ,
                                    GameBattleCursor.instance.PosY + i );

                                switch ( skill.TargetType )
                                {
                                    case GameTargetType.User:
                                        {
                                            if ( u != null && 
                                                u.UnitCampType == unit.UnitCampType )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Enemy:
                                        {
                                            if ( u != null && 
                                                u.UnitCampType != unit.UnitCampType )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Summon:
                                        {
                                            if ( u == null )
                                            {
                                                selectionUnits.Add( u );
                                            }
                                        }
                                        break;
                                }
                                
                            }
                        }
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    int x = 0;
                    int y = 0;

                    switch ( dir )
                    {
                        case GameBattleAttackMapDirection.North:
                            y = -1;
                            break;
                        case GameBattleAttackMapDirection.East:
                            x = 1;
                            break;
                        case GameBattleAttackMapDirection.South:
                            y = 1;
                            break;
                        case GameBattleAttackMapDirection.West:
                            x = -1;
                            break;
                    }

                    for ( int i = 1 ; i <= skill.AttackRangeMax ; i++ )
                    {
                        GameBattleUnit u = GameBattleUnitManager.instance.getUnit( unit.PosX + i * x , unit.PosY + i * y );

                        switch ( skill.TargetType )
                        {
                            case GameTargetType.User:
                                {
                                    if ( u != null && 
                                        u.UnitCampType == unit.UnitCampType )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Enemy:
                                {
                                    if ( u != null &&
                                        u.UnitCampType != unit.UnitCampType )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Summon:
                                {
                                    if ( u == null )
                                    {
                                        selectionUnits.Add( u );
                                    }
                                }
                                break;
                        }
                    }
                }
                break;
        }
        
    }



    public void clearSelection()
    {
        if ( selectionUnit != null )
        {
            selectionUnit.clearSelection();
            selectionUnit = null;
        }
    }


    public bool selectUnit()
    {
        selectionUnit = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX , 
            GameBattleCursor.instance.PosY );

        if ( selectionUnit == null )
        {
            return false;
        }

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( selectionUnit.MoveType );
        bool fly = selectionUnit.checkEffect( GameSkillResutlEffect.Under ) ? true : unitMove.fly;

        if ( selectionUnit.IsUser ||
            selectionUnit.IsNpc )
        {
            // user

            GameBattleUnitMovement.instance.show( (short)selectionUnit.PosX , (short)selectionUnit.PosY ,
    selectionUnit.IsNpc || selectionUnit.IsActed ? selectionUnit.MoveMax : selectionUnit.Move , selectionUnit.MoveType , fly , selectionUnit.UnitCampType , true );

            if ( selectionUnit.IsActed )
            {
                // show info ui

//                selectionUnit = null;
            }
        }
        else
        {
            GameBattleUnitMovement.instance.show( (short)selectionUnit.PosX , (short)selectionUnit.PosY ,
                selectionUnit.MoveMax , selectionUnit.MoveType , fly , selectionUnit.UnitCampType , false );
        }

        return true;
    }



}

