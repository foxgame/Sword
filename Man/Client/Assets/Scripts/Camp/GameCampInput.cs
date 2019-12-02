using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;




public class GameCampInput : Singleton<GameCampInput>
{
    float time = 0.0f;

    bool confirm = false;
    bool cancel = false;

    public bool pause = false;

    void Update()
    {
        GameUserData.instance.addTime( Time.deltaTime );

        if ( pause )
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

        time += Time.deltaTime;

        updateSLUI();

        updateCloudSLUI();

        updateItemUI();

        updateAlchemyUI();

        updateUnitUI();

        updatePowerUI();

        updateQuitUI();

        updateCampUI();
    }

    void moveItemUI()
    {
        if ( time < 0.1f )
        {
            return;
        }

        bool bMove = false;

        if ( GameItemUI.instance.isShowItemAskUI() )
        {
            int selection1 = GameItemUI.instance.getSelectionItemAskUI();

            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                selection1--;
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                selection1++;
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                selection1--;
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                selection1++;
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameItemUI.instance.selectItemAskUI( selection1 );

            time = 0.0f;

            return;
        }

        if ( GameItemUI.instance.isShowAskUI() )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameItemUI.instance.showAskUI( !GameItemUI.instance.isOKAskUI() );

            return;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            GameItemUI.instance.moveLeft();
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            GameItemUI.instance.moveRight();
            bMove = true;
        }


        int selection = GameItemUI.instance.getSelection();

        if ( GameInputManager.getKey( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Up1 ) )
        {
            selection -= GameItemBagUI.MAX_SLOT - 1;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down1 ) )
        {
            selection += GameItemBagUI.MAX_SLOT - 1 ;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameItemUI.instance.select( selection );

        time = 0.0f;
    }

    void moveAlchemyUI()
    {
        if ( time < 0.1f )
        {
            return;
        }

        bool bMove = false;

        
        if ( GameAlchemyUI.instance.isShowAskUI() )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameAlchemyUI.instance.showAskUI( !GameAlchemyUI.instance.isOKAskUI() );

            return;
        }
        

        int selection = GameAlchemyUI.instance.getSelection();

        if ( GameInputManager.getKey( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Up1 ) )
        {
            selection -= GameAlchemyUIBag.MAX_SLOT - 1;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down1 ) )
        {
            selection += GameAlchemyUIBag.MAX_SLOT - 1;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameAlchemyUI.instance.select( selection );

        time = 0.0f;
    }

    void moveSLUI()
    {      
        bool bMove = false;

        if ( GameSLUI.instance.IsShowAskUI )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameSLUI.instance.showAskUI( !GameSLUI.instance.IsOKAskUI );

            return;
        }


        int selection = GameSLUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            return;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            GameSLUI.instance.unShow();
            GameSLCloudUI.instance.show( GameSLUI.instance.Type );
            GameSLCloudUI.instance.show();
            return;
        }

        if ( !bMove )
        {
            return;
        }

        GameSLUI.instance.select( selection );
    }

    void moveCloudSLUI()
    {
        bool bMove = false;

        if ( GameSLCloudUI.instance.IsShowAskUI )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameSLCloudUI.instance.showAskUI( !GameSLCloudUI.instance.IsOKAskUI );

            return;
        }


        int selection = GameSLCloudUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            GameSLCloudUI.instance.unShow();
            GameSLUI.instance.show();
            return;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            return;
        }

        if ( !bMove )
        {
            return;
        }

        GameSLCloudUI.instance.select( selection );
    }

    void moveUnitUI()
    {
        bool bMove = false;

        bool b = false;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            b = true;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            b = true;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameUnitUI.instance.showNextUser( b );
    }

    void movePowerUI()
    {
        bool bMove = false;

        bool b = false;

        if ( GamePowerUI.instance.IsShowUser )
        {
            int selection1 = GamePowerUI.instance.Selection;

            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                selection1--;
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                selection1++;
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                GamePowerUI.instance.onUpClick();
                return;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                GamePowerUI.instance.onDownClick();
                return;
            }

            if ( !bMove )
            {
                return;
            }

            GamePowerUI.instance.select( selection1 );

            time = 0.0f;

            return;
        }


        if ( GamePowerAskUI.instance.IsShow )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GamePowerAskUI.instance.showAskUI( !GamePowerAskUI.instance.IsOKAskUI );
            GamePowerUI.instance.unShowFade();

            return;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            b = true;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            b = true;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GamePowerUI.instance.showNextUser( b );
    }

    void moveCampQuitUI()
    {
        bool bMove = false;

        if ( GameCampQuitUI.instance.IsShow )
        {
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

            GameCampQuitUI.instance.showAskUI( !GameCampQuitUI.instance.IsOKAskUI );

            return;
        }
    }

    void moveCampUI()
    {
        bool bMove = false;

        if ( GameCampExitUI.instance.IsShow )
        {
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

            GameCampExitUI.instance.showAskUI( !GameCampExitUI.instance.IsOKAskUI );

            return;
        }

        int selection = GameCampSelectUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection--;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameCampSelectUI.instance.select( selection );
    }

    void updateItemUI()
    {
        if ( !GameItemUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameItemUI.instance.isShowAskUI() )
            {
                if ( GameItemUI.instance.isOKAskUI() )
                {
                    GameItemUI.instance.onClickAskUI();
                }
                else
                {
                    GameItemUI.instance.unShowAskUI();
                }
            }
            else
            {
                if ( GameItemUI.instance.isShowItemAskUI() )
                {
                    GameItemUI.instance.onClickItemAskUI();
                }
                else
                {
                    GameItemUI.instance.showItemAskUI();
                }
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameItemUI.instance.isShowAskUI() )
            {
                GameItemUI.instance.unShowAskUI();
            }
            else
            {
                if ( GameItemUI.instance.isShowItemAskUI() )
                {
                    if ( GameEquipInfoUI.instance.IsShow )
                        GameEquipInfoUI.instance.unShowFade();

                    GameItemUI.instance.unShowItemAskUI();
                }
                else
                {
                    if ( GameEquipInfoUI.instance.IsShow )
                        GameEquipInfoUI.instance.unShowFade();

                    GameItemUI.instance.unShowFade();

                    GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                    GameCampSelectUI.instance.showFade();
                }
            }

            return;
        }

        moveItemUI();
    }


    void updateAlchemyUI()
    {
        if ( !GameAlchemyUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameAlchemyUI.instance.isShowAskUI() )
            {
                GameAlchemyUI.instance.onClickAskUI();
            }
            else
            {
                if ( GameAlchemyUI.instance.isEnabledAlchemy() )
                {
                    GameAlchemyUI.instance.onClickAlchemy();
                }
                else
                {
                    GameAlchemyUI.instance.onClick();
                }
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameAlchemyUI.instance.isShowAskUI() )
            {
                GameAlchemyUI.instance.unShowAskUI();
            }
            else
            {
                if ( GameAlchemyUI.instance.isEnabledAlchemy() )
                {
                    GameAlchemyUI.instance.showAlchemy( false );
                }
                else
                {
                    GameAlchemyUI.instance.unShowFade();

                    GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                    GameCampSelectUI.instance.showFade();
                }
            }

            return;
        }

        moveAlchemyUI();
    }


    void onUnShowSLUI()
    {
        GameSLUI.instance.unShow();
        GameSLUI.instance.updateUIStageFadeOver();

        GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
        GameCampSelectUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void onUnShowCloudSLUI()
    {
        GameSLCloudUI.instance.unShow();
        GameSLCloudUI.instance.updateUIStageFadeOver();

        GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
        GameCampSelectUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }


    void updateSLUI()
    {
        if ( !GameSLUI.instance.IsShow ||
            GameBlackUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameSLUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameSLUI.instance.IsShowAskUI )
            {
                GameSLUI.instance.unShowAskUI();
            }
            else
            {
                GameSLUI.instance.clear();
                GameSLUI.instance.unShowFade();
                GameSLUI.instance.AutoUnshow = false;

                GameBlackUI.instance.showBlack( 1.0f , onUnShowSLUI );
            }

            return;
        }

        moveSLUI();
    }

    void updateCloudSLUI()
    {
        if ( !GameSLCloudUI.instance.IsShow ||
            GameBlackUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameSLCloudUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameSLCloudUI.instance.IsShowAskUI )
            {
                GameSLCloudUI.instance.unShowAskUI();
            }
            else
            {
                GameSLCloudUI.instance.clear();
                GameSLCloudUI.instance.unShowFade();
                GameSLCloudUI.instance.AutoUnshow = false;

                GameBlackUI.instance.showBlack( 1.0f , onUnShowCloudSLUI );
            }

            return;
        }

        moveCloudSLUI();
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

            GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
            GameCampSelectUI.instance.showFade();

            return;
        }

        moveUnitUI();
    }

    void updatePowerUI()
    {
        if ( !GamePowerUI.instance.IsShow &&
            !GamePowerAskUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GamePowerAskUI.instance.IsShow )
            {
                if ( GamePowerAskUI.instance.IsOKAskUI )
                {
                    GamePowerUI.instance.onAskUIClick();
                }
                else
                {
                }

                GamePowerAskUI.instance.unShowAskUI();
//                GamePowerUI.instance.unShowFade();

                GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                GameCampSelectUI.instance.showFade();
            }
            else
            {
                GamePowerUI.instance.showUser( true );
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GamePowerAskUI.instance.IsShow )
            {
                GamePowerAskUI.instance.unShowAskUI();

                GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                GameCampSelectUI.instance.showFade();
            }
            else
            {
                if ( GamePowerUI.instance.IsShowUser )
                {
                    GamePowerUI.instance.showUser( false );
                }
                else
                {
                    GamePowerAskUI.instance.showAskUI( false );
                    GamePowerUI.instance.unShowFade();
                }
            }

            return;
        }

        movePowerUI();
    }

    void onTitle()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Title , GameSceneLoadMode.Count );
    }

    void updateQuitUI()
    {
        if ( !GameCampQuitUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameCampQuitUI.instance.IsOKAskUI )
            {
                GameBlackUI.instance.showBlack( 1.0f , onTitle );
            }
            else
            {
                GameCampQuitUI.instance.unShowAskUI();

                GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                GameCampSelectUI.instance.showFade();
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameCampQuitUI.instance.unShowAskUI();

            GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
            GameCampSelectUI.instance.showFade();

            return;
        }

        moveCampQuitUI();
    }


    void onShowSave()
    {
        GameSLUI.instance.show( GameSLType.Save );
        GameSLUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void onShowLoad()
    {
        GameSLUI.instance.show( GameSLType.Load );
        GameSLUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void updateCampUI()
    {
        if ( GameItemUI.instance.IsShow ||
            GameAlchemyUI.instance.IsShow ||
            GameSLUI.instance.IsShow ||
            GameUnitUI.instance.IsShow ||
            GamePowerUI.instance.IsShow || 
            GameCampQuitUI.instance.IsShow ||
            GamePowerAskUI.instance.IsShow )
        {
            return;
        }

        if ( !GameCampUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameCampExitUI.instance.IsShow )
            {
                if ( GameCampExitUI.instance.IsOKAskUI )
                {
                    GameCampUI.instance.leave();
                    pause = true;
                }
                else
                {
                    GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                    GameCampSelectUI.instance.showFade();
                }

                GameCampExitUI.instance.unShowAskUI();

                return;
            }

            int selection = GameCampSelectUI.instance.Selection;

            switch ( selection )
            {
                case 0:
                    GameItemUI.instance.show( null );
                    GameItemUI.instance.showFade();
                    break;
                case 1:
                    GameAlchemyUI.instance.show( null );
                    GameAlchemyUI.instance.showFade();
                    break;
                case 2:
                    GameUnitUI.instance.show( 0 );
                    GameUnitUI.instance.showFade();
                    break;
                case 3:
                    GamePowerUI.instance.show( 0 , true );
                    GamePowerUI.instance.showFade();
                    break;
                case 4:
                    GameBlackUI.instance.showBlack( 1 , onShowSave );
                    break;
                case 5:
                    GameBlackUI.instance.showBlack( 1 , onShowLoad );
                    break;
                case 6:
                    GameCampQuitUI.instance.showAskUI( false );
                    break;
            }

            GameCampSelectUI.instance.unShowFade();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameCampExitUI.instance.IsShow )
            {
                GameCampExitUI.instance.unShowAskUI();

                GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
                GameCampSelectUI.instance.showFade();

                return;
            }

            if ( GameCampSelectUI.instance.IsShow )
            {
                GameCampExitUI.instance.showAskUI( false );

                GameCampSelectUI.instance.unShowFade();
            }
            else
            {
//                 GameCampSelectUI.instance.show( GameCampSelectUI.instance.Selection );
//                 GameCampSelectUI.instance.showFade();
            }

            return;
        }

        moveCampUI();
    }


}

