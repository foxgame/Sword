using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;




public class GameRPGInput : Singleton<GameRPGInput>
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

        if ( GameBlackUI.instance.IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        updateMsgBoxChooseUI();

        updateMsgBoxUI();

        updateShopUI();

        if ( GameRPGEventManager.instance.IsShow )
        {
            return;
        }

        updateItemUI();

        updateRPG();
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


    void moveShopUI()
    {
        if ( time < 0.1f )
        {
            return;
        }

        bool bMove = false;

        if ( GameShopUI.instance.isShowAskUI() )
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

            GameShopUI.instance.showAskUI( !GameShopUI.instance.isOKAskUI() );

            return;
        }


        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            GameShopUI.instance.moveLeft();
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            GameShopUI.instance.moveRight();
            bMove = true;
        }


        int selection = GameShopUI.instance.getSelection();

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
            selection += GameItemBagUI.MAX_SLOT - 1;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameShopUI.instance.select( selection );

        time = 0.0f;
    }

    void updateShopUI()
    {
        if ( !GameShopUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            if ( GameShopUI.instance.isShowAskUI() )
            {
                GameShopUI.instance.onClickAskUI();
            }
            else
            {
                GameShopUI.instance.showAskUI( true );
            }

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameShopUI.instance.isShowAskUI() )
            {
                GameShopUI.instance.unShowAskUI();
            }
            else
            {
                GameShopUI.instance.unShowFade();
            }

            return;
        }

        moveShopUI();
    }


    void moveRPG()
    {
        bool bMove = false;
        bool up = false;
        bool down = false;
        bool left = false;
        bool right = false;

        if ( GameInputManager.getKey( GameInputCode.Up ) )
        {
            bMove = true;
            up = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Down ) )
        {
            bMove = true;
            down = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Left ) )
        {
            bMove = true;
            left = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Right ) )
        {
            bMove = true;
            right = true;
        }

        GameRPGManager.instance.moveTo( bMove );
        GameRPGManager.instance.moveUpdate();

        if ( GameBlackUI.instance.IsShow )
        {
            return;
        }

        if ( up )
        {
            GameRPGManager.instance.moveTo( GameAnimationDirection.North );
        }

        if ( down )
        {
            GameRPGManager.instance.moveTo( GameAnimationDirection.South );
        }

        if ( left )
        {
            GameRPGManager.instance.moveTo( GameAnimationDirection.West );
        }

        if ( right )
        {
            GameRPGManager.instance.moveTo( GameAnimationDirection.East );
        }
    }

    void updateRPG()
    {
        if ( GameMsgBoxUI.instance.IsShow ||
            GameMsgBoxChooseUI.instance.IsShow ||
            GameItemUI.instance.IsShow )
        {
            return;
        }

        if ( GameShopUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameRPGManager.instance.showEvent();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameItemUI.instance.show( null );
            GameItemUI.instance.showFade();

            return;
        }

        moveRPG();
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

        if ( GameInputManager.getKey( GameInputCode.Up1 ) )
        {
            selection -= GameItemBagUI.MAX_SLOT - 1;
            bMove = true;
        }

        if ( GameInputManager.getKey( GameInputCode.Down1 ) )
        {
            selection += GameItemBagUI.MAX_SLOT - 1;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameItemUI.instance.select( selection );

        time = 0.0f;
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
                }
            }

            return;
        }

        moveItemUI();
    }

}


