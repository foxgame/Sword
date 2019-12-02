using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



public class GameBattleInput : Singleton<GameBattleInput>
{
    public int lastUnitID = GameDefine.INVALID_ID;

    float time = 0.0f;


    int moving = 0;
    bool isMoving = false;

    bool pause = false;

    bool confirm = false;
    bool cancel = false;

    GameSkill skill;

    public void clearInput()
    {
        GameInputManager.getKeyDown( GameInputCode.Confirm );
        GameInputManager.getKeyDown( GameInputCode.Cancel );
    }

    public bool Pause
    {
        get
        {
            return pause;
        }
        set
        {
            pause = value;

            if ( pause )
            {
                GameTouchCenterUI.instance.unShowUI();
            }
            else
            {
                if ( !GameBattleTurn.instance.UserTurn || 
                    GameBattleUnitManager.instance.IsNpcAI )
                {
                    pause = true;
                    return;
                }

                GameTouchCenterUI.instance.showUI();
            }
        }
    }

    void moveMsgBoxChooseUI()
    {
        bool bMove = false;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameMsgBoxChooseUI.instance.select( !GameMsgBoxChooseUI.instance.IsOK );
    }

    void moveBattleCursor()
    {
        bool bMove = false;

        int x = GameBattleCursor.instance.PosX;
        int y = GameBattleCursor.instance.PosY;

        if ( GameInputManager.getKey( GameInputCode.Up ) )
        {
            if ( y == 0 )
            {
                return;
            }

            y--;

            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Down ) )
        {
            if ( y == GameBattleManager.instance.Height - 1 )
            {
                return;
            }

            y++;

            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Left ) )
        {
            if ( x == 0 )
            {
                return;
            }

            x--;

            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Right ) )
        {
            if ( x == GameBattleManager.instance.Width - 1 )
            {
                return;
            }

            x++;

            bMove = true;
        }


        if ( !bMove )
        {
            moving = 0;
            time = 0.0f;
            return;
        }
        else
        {
            time += Time.deltaTime;

            if ( moving == 1 &&
                time < 0.5f )
            {
                return;
            }

            moving++;
        }

        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        GameBattleCursor.instance.moveTo( x , y , 
            GameBattleCursor.SpeedX , GameBattleCursor.SpeedY ,
            true ,
            onCursorMoveOver );
    }

    void moveBattleSystemSLUI()
    {
        bool bMove = false;

        int selection = GameBattleSystemSLUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleSystemSLUI.instance.select( selection );
    }

    void moveBattleSystemSoundUI()
    {
        bool bMove = false;

        int selection = GameBattleSystemSoundUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleSystemSoundUI.instance.select( selection );
    }

    void moveBattleSystemUI()
    {
        if ( GameBattleSystemSLUI.instance.IsShow ||
            GameBattleSystemSoundUI.instance.IsShow )
        {
            return;
        }

        bool bMove = false;

        int selection = GameBattleSystemUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleSystemUI.instance.select( selection );
    }

    void moveBattleUnitActionUI()
    {
        if ( GameBattleUnitActionItemUI.instance.IsShow )
        {
            return;
        }

        bool bMove = false;

        int selection = GameBattleUnitActionUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleUnitActionUI.instance.select( selection );
    }

    void moveBattleUnitActionItemUI()
    {
        bool bMove = false;

        int selection = GameBattleUnitActionItemUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleUnitActionItemUI.instance.select( selection );
    }



    void moveSkillUI()
    {
        bool bMove = false;

        int selection = GameBattleSkillUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            if ( selection > 0 )
            {
                selection--;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            if ( selection < GameDefine.MAX_SLOT - 1 )
            {
                selection++;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            if ( selection >= GameDefine.MAX_SLOT_HALF )
            {
                selection -= GameDefine.MAX_SLOT_HALF;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            if ( selection < GameDefine.MAX_SLOT_HALF )
            {
                selection += GameDefine.MAX_SLOT_HALF;
            }

            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleSkillUI.instance.select( selection );
    }

    void moveBattleMapUI()
    {
        if ( GameInputManager.getKey( GameInputCode.Up ) )
        {
            GameBattleMapUI.instance.moveUp();
        }

        if ( GameInputManager.getKey( GameInputCode.Down ) )
        {
            GameBattleMapUI.instance.moveDown();
        }

        if ( GameInputManager.getKey( GameInputCode.Left ) )
        {
            GameBattleMapUI.instance.moveLeft();
        }

        if ( GameInputManager.getKey( GameInputCode.Right ) )
        {
            GameBattleMapUI.instance.moveRight();
        }
    }

    void moveItemUI()
    {
        bool bMove = false;

        int selection = GameBattleItemUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            if ( selection > 0 )
            {
                selection--;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            if ( selection < GameDefine.MAX_SLOT - 1 )
            {
                selection++;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            if ( selection >= GameDefine.MAX_SLOT_HALF )
            {
                selection -= GameDefine.MAX_SLOT_HALF;
            }

            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            if ( selection < GameDefine.MAX_SLOT_HALF )
            {
                selection += GameDefine.MAX_SLOT_HALF;
            }

            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameBattleItemUI.instance.select( selection );
    }

    void onMoveOver()
    {
        GameTouchCenterUI.instance.showUI();

        isMoving = false;

        GameBattleUnit a = GameBattleSelection.instance.SelectionUnit;

        GameBattleUnitActionUI.instance.show( false , a.canUseSkill() );
        GameBattleUnitActionUI.instance.setPos( GameBattleCursor.instance.PosX ,
    GameBattleCursor.instance.PosY );
        GameBattleUnitActionUI.instance.showFade();

        GameBattleCursor.instance.unShow();

        clearInput();
    }

    void onAttackOver()
    {
        GameTouchCenterUI.instance.showUI();

        GameBattleCursor.instance.show();

        GameBattleUnit a = GameBattleSelection.instance.SelectionUnit;
        a.action();

        GameBattleSelection.instance.clearSelection();

        clearInput();
    }

    void onSkillOver()
    {
        GameTouchCenterUI.instance.showUI();

        GameBattleCursor.instance.show();

        GameBattleUnit a = GameBattleSelection.instance.SelectionUnit;
        a.action();

        GameBattleSelection.instance.clearSelection();

        clearInput();
    }

    void onItemOver()
    {
        GameTouchCenterUI.instance.showUI();

        GameBattleCursor.instance.show();

        GameBattleUnit a = GameBattleSelection.instance.SelectionUnit;
        a.action();

        GameBattleSelection.instance.clearSelection();

        clearInput();
    }

    void Update()
    {
        if ( GameInputManager.getKeyDown( GameInputCode.Debug ) )
        {
            GameUserData.instance.saveBattleDebug( -99 );
        }

        GameUserData.instance.addTime( Time.deltaTime );

        if ( isMoving || 
            GameBattleTurn.instance.Ishow )
        {
            return;
        }

        confirm = false;
        cancel = false;

        if ( GameInputManager.getKeyDown( GameInputCode.Confirm ) )
        {
            confirm = true;
        }
        if ( GameInputManager.getKeyDown( GameInputCode.Cancel ) )
        {
            cancel = true;
        }

        updateMovie();

        updateMsgBoxUI();
        // msg box choose
        updateMsgBoxChooseUI();

        // get item ui
        updateGetItemUI();

        // level up ui
        updateLevelUpUI();

        updateGameOverUI();

        if ( pause )
        {
            return;
        }

        // skill selection
        updateSkillSelection();
        // skill ui
        updateSkillUI();

        // item selection
        updateItemSelection();
        // item ui
        updateItemUI();

        // map ui
        updateBattleMapUI();

        // info ui
        updateBattleInfoUI();

        // system sl ui
        updateSystemSLUI();
        // sound ui
        updateSystemSoundUI();
        // system ui
        updateSystemUI();

        // attack selection
        updateAttackSelection();

        // action item ui
        updateActionItemUI();
        // action ui
        updateActionUI();

        // unit ui
        updateUnitUI();

        // move
        updateUnitMove();

        // cursor
        updateCursor();

    }


    void updateMovie()
    {
        if ( !GameMovieManager.instance.IsPlaying )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameMovieManager.instance.onClick();

            time = 0.0f;

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameMovieManager.instance.onClick();

            time = 0.0f;

            return;
        }
    }

    void updateMsgBoxUI()
    {
        if ( !GameMsgBoxUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameMsgBoxUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameMsgBoxUI.instance.onClick();

            return;
        }

    }

    void onMsgBoxClick1()
    {
        if ( GameMsgBoxChooseUI.instance.IsOK )
        {
            GameBattleTurn.instance.nextTurn();
        }
        else
        {
            GameBattleCursor.instance.show();
        }
    }

    void onMsgBoxClick()
    {
        GameBattleCursor.instance.show();

        switch ( GameBattleSystemSLUI.instance.Selection )
        {
            case 0:
                {
                    if ( GameMsgBoxChooseUI.instance.IsOK )
                    {
                        GameUserData.instance.saveBattle();
                    }
                }
                break;
            case 1:
                {
                    if ( GameMsgBoxChooseUI.instance.IsOK )
                    {
                        GameUserData.instance.loadBattle();
                    }
                }
                break;
            case 3:
                {
                    if ( GameMsgBoxChooseUI.instance.IsOK )
                    {
                        GameSceneManager.instance.loadScene( GameSceneType.Title , GameSceneLoadMode.Count );
                        GameMusicManager.instance.stopMusic( 0 );

                        pause = true;
                    }
                }
                break;
        }
    }

    void updateMsgBoxChooseUI()
    {
        if ( !GameMsgBoxChooseUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameMsgBoxChooseUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameMsgBoxChooseUI.instance.onCancelClick();

            return;
        }

        moveMsgBoxChooseUI();
    }

    void updateSkillSelection()
    {
        if ( !GameBattleUnitSkillSelection.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            int x = GameBattleCursor.instance.PosX;
            int y = GameBattleCursor.instance.PosY;

            GameBattleAttackMapDirection dir;

            if ( !GameBattleUnitSkillSelection.instance.checkCell( x , y , out dir ) )
            {
                return;
            }

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleSelection.instance.getUnits( unit , skill , dir );

            if ( GameBattleSelection.instance.SelectionUnits.Count == 0 )
            {
                return;
            }

            GameBattleUnitSkillSelection.instance.unShow();

            GameBattleCursor.instance.unShow();

            GameTouchCenterUI.instance.unShowUI();

            GameBattleAttackResult.instance.skillAttack( skill , unit ,
                GameBattleSelection.instance.SelectionUnits , dir , GameBattleAttackResultSide.Right , onSkillOver );

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( true );
            GameBattleUnitActionUI.instance.showFade();

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleUnitSkillSelection.instance.unShow();

//            GameBattleCursor.instance.show();
            GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , true );
            GameBattleCursor.instance.unShow();

            return;
        }
    }


    void updateItemSelection()
    {
        if ( !GameBattleUnitItemSelection.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            int x = GameBattleCursor.instance.PosX;
            int y = GameBattleCursor.instance.PosY;

            GameBattleAttackMapDirection dir;

            if ( !GameBattleUnitItemSelection.instance.checkCell( x , y , out dir ) )
            {
                return;
            }

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            int itemSlot = GameBattleItemUI.instance.Selection;
            GameItem item = GameBattleItemUI.instance.getSelectionItem();

            switch ( GameBattleItemUI.instance.Mode )
            {
                case GameBattleUnitActionItemMode.Use:
                    {
                        GameBattleSelection.instance.getUnits( unit , item , dir );

                        if ( GameBattleSelection.instance.SelectionUnits.Count == 0 )
                        {
                            return;
                        }

                        GameBattleUnitSkillSelection.instance.unShow();

                        GameTouchCenterUI.instance.unShowUI();

                        GameBattleAttackResult.instance.itemAttack( item , unit ,
                            GameBattleSelection.instance.SelectionUnits , dir , GameBattleAttackResultSide.Right , onItemOver );

                        GameBattleUnitItemSelection.instance.unShow();

                        GameBattleCursor.instance.unShow();
                    }
                    break;
                case GameBattleUnitActionItemMode.Give:
                    {
                        GameBattleUnit unit2 = GameBattleUnitManager.instance.getUnit( x , y );

                        if ( unit2 == null )
                        {
                            return;
                        }

                        if ( !unit2.IsUser )
                        {
                            return;
                        }

                        if ( !unit2.canAddItem() )
                        {
                            // item full

                            return;
                        }

                        unit.giveItem( itemSlot , unit2 );

                        GameBattleUnitItemSelection.instance.unShow();

//                        GameBattleCursor.instance.show();

                        unit.action();

                        GameBattleSelection.instance.clearSelection();
                    }
                    break;
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( true );
            GameBattleUnitActionUI.instance.showFade();

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleUnitItemSelection.instance.unShow();

            GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , true );
            GameBattleCursor.instance.unShow();

            return;
        }
    }

    void updateBattleMapUI()
    {
        if ( !GameBattleMapUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleMapUI.instance.unShowFade();
            GameBattleCursor.instance.show();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleMapUI.instance.unShowFade();
            GameBattleCursor.instance.show();

            return;
        }

        moveBattleMapUI();
    }

    void updateBattleInfoUI()
    {
        if ( !GameBattleInformationUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleInformationUI.instance.unShowFade();
            GameBattleCursor.instance.show();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleInformationUI.instance.unShowFade();
            GameBattleCursor.instance.show();

            return;
        }
    }


    void updateItemUI()
    {
        if ( !GameBattleItemUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameItem item = GameBattleItemUI.instance.getSelectionItem();
            int itemSlot = GameBattleItemUI.instance.Selection;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            if ( item == null )
            {
                return;
            }

            switch ( GameBattleItemUI.instance.Mode )
            {
                case GameBattleUnitActionItemMode.Use:
                    {
                        GameBattleUnitItemSelection.instance.showUse( unit.PosX , unit.PosY , item , unit.UnitCampType , true );

                        GameBattleCursor.instance.moveTo( GameBattleUnitItemSelection.instance.AttackCell.x ,
                        GameBattleUnitItemSelection.instance.AttackCell.y ,
                        GameBattleCursor.SpeedX / 2 ,
                        GameBattleCursor.SpeedY / 2 ,
                        true ,
                        onCursorMoveOver , false );

                        GameBattleItemUI.instance.unShowFade();

                        GameBattleUnitActionItemUI.instance.unShowFade();
                        GameBattleUnitActionUI.instance.unShowFade();
                    }
                    break;
                case GameBattleUnitActionItemMode.Give:
                    {
                        GameBattleUnitItemSelection.instance.showGive( unit.PosX , unit.PosY , GameDefine.INVALID_ID , 1 , GameAttackRangeType.Circle , 0 , unit.UnitCampType , true );
                        
                        GameBattleItemUI.instance.unShowFade();

                        GameBattleUnitActionItemUI.instance.unShowFade();
                        GameBattleUnitActionUI.instance.unShowFade();
                    }
                    break;
                case GameBattleUnitActionItemMode.Equip:
                    {
                        unit.equipItem( item );

                        GameBattleItemUI.instance.setData( unit , GameBattleItemUI.instance.Mode );

                        if ( GameEquipInfoUI.instance.IsShow )
                            GameEquipInfoUI.instance.unShowFade();
                    }
                    break;
                case GameBattleUnitActionItemMode.Drop:
                    {
                        if ( GameBattleItemUI.instance.IsShowAskUI )
                        {
                            GameBattleItemUI.instance.unShowAskUI();

                            if ( GameBattleItemUI.instance.IsOKAskUI )
                            {
                                // drop item

                                unit.removeItem( itemSlot );

                                GameBattleItemUI.instance.setData( unit , GameBattleItemUI.instance.Mode );
                            }
                        }
                        else
                        {
                            GameBattleItemUI.instance.showAskUI();
                        }
                    }
                    break;
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameBattleItemUI.instance.IsShowAskUI )
            {
                GameBattleItemUI.instance.unShowAskUI();
                return;
            }

            GameEquipInfoUI.instance.unShowFade();

            GameBattleItemUI.instance.unShowFade();

            GameBattleUnitActionItemUI.instance.show();
            GameBattleUnitActionItemUI.instance.showFade();

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( false );
            GameBattleUnitActionUI.instance.showFade();

            return;
        }

        moveItemUI();
    }

    void updateSkillUI()
    {
        if ( !GameBattleSkillUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            skill = GameBattleSkillUI.instance.getSelectionSkill();

            if ( skill == null )
            {
                return;
            }

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleUnitSkillSelection.instance.show( unit.PosX , unit.PosY , skill , unit.UnitCampType , true );

            GameBattleSkillUI.instance.unShowFade();

            GameBattleCursor.instance.moveTo( GameBattleUnitSkillSelection.instance.AttackCell.x ,
                GameBattleUnitSkillSelection.instance.AttackCell.y ,
            GameBattleCursor.SpeedX , GameBattleCursor.SpeedY ,
            true ,
            onCursorMoveOver );

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleSkillUI.instance.unShowFade();

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( true );
            GameBattleUnitActionUI.instance.showFade();

            return;
        }

        moveSkillUI();
    }


    void updateGetItemUI()
    {
        if ( !GameBattleGetItemUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleCursor.instance.show();

            GameBattleGetItemUI.instance.unShowFade();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleCursor.instance.show();

            GameBattleGetItemUI.instance.unShowFade();

            return;
        }
    }

    void updateSystemSLUI()
    {
        if ( !GameBattleSystemSLUI.instance.IsShow )
        {
            return;
        }


        if ( confirm )
        {
            confirm = false;

            switch ( GameBattleSystemSLUI.instance.Selection )
            {
                case 0:
                    {
                        GameMessage msg = GameMessageData.instance.getData( GameMessageType.Common );

                        GameMsgBoxChooseUI.instance.showText( msg.message[ 0 ][ 0 ] , 
                            msg.message[ 0 ][ 1 ] ,
                            msg.message[ 0 ][ 2 ] ,
                            true ,
                            onMsgBoxClick );
                        GameMsgBoxChooseUI.instance.showFade();

                        GameBattleSystemSLUI.instance.unShowFade();
                        GameBattleSystemUI.instance.unShowFade();
                    }
                    break;
                case 1:
                    {
                        GameMessage msg = GameMessageData.instance.getData( GameMessageType.Common );

                        GameMsgBoxChooseUI.instance.showText( msg.message[ 0 ][ 3 ] ,
                            msg.message[ 0 ][ 4 ] ,
                            msg.message[ 0 ][ 5 ] ,
                            true ,
                            onMsgBoxClick );
                        GameMsgBoxChooseUI.instance.showFade();

                        GameBattleSystemSLUI.instance.unShowFade();
                        GameBattleSystemUI.instance.unShowFade();
                    }
                    break;
                case 2:
                    {
                        GameBattleInformationUI.instance.show();
                        GameBattleInformationUI.instance.updateData();
                        GameBattleInformationUI.instance.showFade();

                        GameBattleSystemSLUI.instance.unShowFade();
                        GameBattleSystemUI.instance.unShowFade();
                    }
                    break;
                case 3:
                    {
                        GameMessage msg = GameMessageData.instance.getData( GameMessageType.Common );

                        GameMsgBoxChooseUI.instance.showText( msg.message[ 0 ][ 6 ] ,
                            msg.message[ 0 ][ 7 ] ,
                            msg.message[ 0 ][ 8 ] ,
                            true ,
                            onMsgBoxClick );
                        GameMsgBoxChooseUI.instance.showFade();

                        GameBattleSystemSLUI.instance.unShowFade();
                        GameBattleSystemUI.instance.unShowFade();
                    }
                    break;
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleSystemSLUI.instance.unShowFade();

            GameBattleSystemUI.instance.enable( true );

            return;
        }

        moveBattleSystemSLUI();
    }

    void updateSystemSoundUI()
    {
        if ( !GameBattleSystemSoundUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            switch ( GameBattleSystemSoundUI.instance.Selection )
            {
                case 0:
                    {
                        GameSetting.instance.enabledMusic = true;

                        GameBattleManager.instance.initMusic();
                    }
                    break;
                case 1:
                    {
                        GameSetting.instance.enabledMusic = false;

                        GameMusicManager.instance.stopMusic( 0 );
                    }
                    break;
                case 2:
                    {
                        GameSetting.instance.enabledSound = true;
                    }
                    break;
                case 3:
                    {
                        GameSetting.instance.enabledSound = false;
                    }
                    break;
            }

            GameSetting.instance.save();

            GameBattleSystemSoundUI.instance.unShowFade();

            GameBattleSystemUI.instance.enable( true );

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleSystemSoundUI.instance.unShowFade();

            GameBattleSystemUI.instance.enable( true );

            return;
        }

        moveBattleSystemSoundUI();
    }

    void updateSystemUI()
    {
        if ( !GameBattleSystemUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            switch ( GameBattleSystemUI.instance.Selection )
            {
                case 0:
                    {
                        GameBattleSystemSLUI.instance.show( GameBattleCursor.instance.PosX ,
                                GameBattleCursor.instance.PosY );
                        GameBattleSystemSLUI.instance.showFade();

                        GameBattleSystemUI.instance.enable( false );
                    }
                    break;
                case 1:
                    {
                        GameBattleSystemSoundUI.instance.show( GameBattleCursor.instance.PosX ,
                                GameBattleCursor.instance.PosY );
                        GameBattleSystemSoundUI.instance.showFade();

                        GameBattleSystemUI.instance.enable( false );
                    }
                    break;
                case 2:
                    {
                        GameBattleMapUI.instance.show();
                        GameBattleMapUI.instance.updateData();
                        GameBattleMapUI.instance.showFade();

                        GameBattleSystemUI.instance.unShowFade();
                    }
                    break;
                case 3:
                    {
                        GameMessage msg = GameMessageData.instance.getData( GameMessageType.Common );

                        GameMsgBoxChooseUI.instance.showText( msg.message[ 0 ][ 9 ] ,
                            msg.message[ 0 ][ 10 ] ,
                            msg.message[ 0 ][ 11 ] ,
                            true ,
                            onMsgBoxClick1 );
                        GameMsgBoxChooseUI.instance.showFade();

                        GameBattleSystemUI.instance.unShowFade();

                        GameBattleCursor.instance.unShow();

//                        GameBattleTurn.instance.nextTurn();
                    }
                    break;
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleSystemUI.instance.unShowFade();

            GameBattleCursor.instance.show();

            return;
        }

        moveBattleSystemUI();
    }

    void updateActionUI()
    {
        if ( !GameBattleUnitActionUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            if ( unit != null )
            {
                switch ( GameBattleUnitActionUI.instance.Selection )
                {
                    case 0:
                        {
                            GameBattleCursor.instance.show();

                            GameBattleUnitActionUI.instance.unShowFade();

                            unit.burst();

                            GameBattleSelection.instance.clearSelection();
                        }
                        break;
                    case 1:
                        {
                            GameBattleCursor.instance.unShow();

                            GameBattleUnitActionUI.instance.unShowFade();

                            GameBattleSkillUI.instance.show();
                            GameBattleSkillUI.instance.setData( unit );
                            GameBattleSkillUI.instance.showFade();
                        }
                        break;
                    case 2:
                        {
                            if ( unit.checkEffect( GameSkillResutlEffect.Palsy ) )
                            {
                                return;
                            }

                            GameBattleCursor.instance.show();

                            GameBattleUnitActionUI.instance.unShowFade();

                            GameBattleUnitAttackSelection.instance.show( unit.PosX ,
                                unit.PosY ,
                                unit.Weapon ,
                                unit.UnitCampType ,
                                true );

                            GameBattleCursor.instance.moveTo( GameBattleUnitAttackSelection.instance.AttackCell.x ,
                                GameBattleUnitAttackSelection.instance.AttackCell.y , 
                                GameBattleCursor.SpeedX , GameBattleCursor.SpeedY , 
                                true ,
                                onCursorMoveOver );
                        }
                        break;
                    case 3:
                        {
                            GameBattleUnitActionItemUI.instance.show( GameBattleCursor.instance.PosX ,
                                GameBattleCursor.instance.PosY );
                            GameBattleUnitActionItemUI.instance.showFade();

                            GameBattleUnitActionUI.instance.enable( false );
                        }
                        break;
                    case 4:
                        {
                            GameBattleCursor.instance.show();

                            GameBattleUnitActionUI.instance.unShowFade();

                            unit.action();

                            GameBattleSelection.instance.clearSelection();
                        }
                        break;
                }

            }
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleUnitAttackSelection.instance.unShow();
            GameBattleUnitActionUI.instance.unShowFade();

            GameBattleCursor.instance.show();

            if ( unit != null )
            {
                GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , true );
            }

            GameBattleSelection.instance.clearSelection();
        }

        moveBattleUnitActionUI();
    }

    void updateActionItemUI()
    {
        if ( !GameBattleUnitActionItemUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;
            
            GameBattleItemUI.instance.show();
            GameBattleItemUI.instance.setData( unit , (GameBattleUnitActionItemMode)GameBattleUnitActionItemUI.instance.Selection );
            GameBattleItemUI.instance.showFade();

            GameBattleUnitActionUI.instance.unShowFade();
            GameBattleUnitActionItemUI.instance.unShowFade();
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( true );
            GameBattleUnitActionUI.instance.showFade();

            GameBattleUnitActionItemUI.instance.unShowFade();
        }

        moveBattleUnitActionItemUI();
    }


    void updateAttackSelection()
    {
        if ( !GameBattleUnitAttackSelection.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            //attack

            if ( !GameBattleUnitAttackSelection.instance.checkCell( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY ) )
            {
                return;
            }

            GameBattleUnit defencer = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY );

            GameBattleUnit attacker = GameBattleSelection.instance.SelectionUnit;

            if ( defencer != null && attacker != null )
            {
                if ( attacker.UnitCampType != defencer.UnitCampType )
                {
                    GameTouchCenterUI.instance.unShowUI();

                    GameBattleAttackResult.instance.PhysicalAttack( attacker , defencer , GameBattleAttackResultSide.Right , onAttackOver );

                    attacker.setDirection( defencer.PosX , defencer.PosY );
                    attacker.move();

                    GameBattleCursor.instance.unShow();
                    GameBattleUnitAttackSelection.instance.unShow();
                    GameBattleUnitActionUI.instance.unShowFade();
                }
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleCursor.instance.unShow();
            GameBattleUnitAttackSelection.instance.unShow();

            GameBattleUnitActionUI.instance.show();
            GameBattleUnitActionUI.instance.enable( true );
            GameBattleUnitActionUI.instance.showFade();

            return;
        }
    }

    void updateUnitMove()
    {
        if ( !GameBattleUnitMovement.instance.IsShow )
        {
            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnitMovement.instance.unShow();
            GameBattleSelection.instance.clearSelection();

            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameBattleUnit unit = GameBattleSelection.instance.SelectionUnit;

            if ( unit != null )
            {
                if ( unit.IsMoving )
                {
                    return;
                }

                if ( unit.IsActed )
                {
                    GameUnitUI.instance.show( GameBattleSelection.instance.SelectionUnit );
                    GameUnitUI.instance.showFade();
                    return;
                }

                if ( !unit.IsUser )
                {
                    GameUnitUI.instance.show( GameBattleSelection.instance.SelectionUnit );
                    GameUnitUI.instance.showFade();
                    return;
                }

                if ( !GameBattleUnitMovement.instance.checkCell( GameBattleCursor.instance.PosX ,
                    GameBattleCursor.instance.PosY ) )
                {
                    return;
                }

                GameBattleUnitMovement.instance.unShow();

                isMoving = true;

                unit.setOriginalDirection();

                if ( GameBattleCursor.instance.PosX == unit.PosX &&
                    GameBattleCursor.instance.PosY == unit.PosY )
                {
                    GameBattleUnitActionUI.instance.show( true , true );
                    GameBattleUnitActionUI.instance.setPos( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY );
                    GameBattleUnitActionUI.instance.showFade();

                    GameBattleCursor.instance.unShow();

                    isMoving = false;
                }
                else
                {
                    GameTouchCenterUI.instance.unShowUI();

                    unit.moveTo( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY , true , true , 2 , onMoveOver );

//                    GameBattleSceneMovement.instance.moveTo( unit , onMoveTo );
                }
            }
        }
    }

    void onMoveTo()
    {
        GameBattleSelection.instance.SelectionUnit.moveTo( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY , true , true , 2 , onMoveOver );
    }

    void updateUnitUI()
    {
        if ( !GameUnitUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameUnitUI.instance.showNext();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameUnitUI.instance.unShowFade();

            return;
        }
    }

    void updateLevelUpUI()
    {
        if ( !GameLevelUpUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameLevelUpUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameLevelUpUI.instance.onClick();

            return;
        }
    }

    void updateGameOverUI()
    {
        if ( !GameOverUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameOverUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameOverUI.instance.onClick();

            return;
        }
    }

    void updateCursor()
    {
        if ( !GameBattleCursor.instance.IsShow ||
             GameBattleGetItemUI.instance.IsShow ||
             GameUnitUI.instance.IsShow ||
             GameBattleCursor.instance.IsMoving )
        {
            return;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Info ) )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX ,
                GameBattleCursor.instance.PosY );

            if ( unit != null )
            {
                GameUnitUI.instance.show( unit );
                GameUnitUI.instance.showFade();
            }

            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameBattleSelection.instance.SelectionUnit == null )
            {
                GameBattleSelection.instance.selectUnit();

                if ( GameBattleSelection.instance.SelectionUnit == null &&
                   !GameBattleSystemUI.instance.IsShow )
                {
                    GameBattleSystemUI.instance.show( GameBattleCursor.instance.PosX ,
                        GameBattleCursor.instance.PosY );
                    GameBattleSystemUI.instance.showFade();

                    GameBattleCursor.instance.unShow();

                    return;
                }
            }
            else
            {

            }

        }

        if ( cancel )
        {
            cancel = false;

            GameBattleUnit unit = GameBattleUnitManager.instance.nextUser( lastUnitID );

            if ( unit != null )
            {
                lastUnitID = unit.UnitID;

                GameBattleUserLeftUI.instance.unShowFade();
                GameBattleUserRightUI.instance.unShowFade();

                GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY ,
                    GameBattleCursor.SpeedX , GameBattleCursor.SpeedY ,
                    true ,
                    onCursorMoveOver );

//                 GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell , 
//                     unit.PosY - GameCameraManager.instance.yCell , GameBattleCursor.SpeedX , GameBattleCursor.SpeedY , null );
            }
        }

        moveBattleCursor();
    }

    void onCursorMoveOver()
    {
        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( GameBattleCursor.instance.PosX ,
            GameBattleCursor.instance.PosY );

        if ( unit != null )
        {
            bool btop = ( GameBattleCursor.instance.PosY - GameBattleSceneMovement.instance.PosY >=
                GameCameraManager.instance.yOffsetCell );

            GameBattleUserLeftUI.instance.show( unit , btop );
            GameBattleUserRightUI.instance.show( unit , btop );
        }
        else
        {
            GameBattleUserLeftUI.instance.unShowFade();
            GameBattleUserRightUI.instance.unShowFade();
        }

        clearInput();
    }

}


