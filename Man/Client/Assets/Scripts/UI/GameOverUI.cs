using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : GameUI<GameOverUI>
{
    GameAnimation gameAnimation;

    bool isPlayOver = false;
    bool isShowMsgBox = false;

    public override void initSingleton()
    {
        gameAnimation = GetComponentInChildren<GameAnimation>();
    }

    public override void onShow()
    {
        gameAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onPlayOver );

        isPlayOver = false;
        isShowMsgBox = false;
    }

    void onPlayOver()
    {
        isPlayOver = true;
        GameTouchCenterUI.instance.showUI();
    }

    void onLoadTitle()
    {
        unShow();

        GameSceneManager.instance.loadScene( GameSceneType.Title , GameSceneLoadMode.Count );
    }

    void onReloadBattle()
    {
        unShow();

        GameUserData.instance.ReloadBattleCount += 1;
        GameUserData.instance.ReloadBattleCountAll += 1;

        GameUserData.instance.clearBattle();

        GameBattleManager.instance.active();
        GameBattleManager.instance.showLayer( 1 , false );
        GameBattleManager.instance.initMusic();

        GameBattleManager.instance.initTreasures();

        GameBattleUnitManager.instance.initUnits();

        GameBlackUI.instance.unShowBlack( 1 , null );

        GameBattleTurn.instance.start();
    }

    void onReloadBattle1()
    {
    }

    void onMsgBoxClick()
    {
        if ( GameMsgBoxChooseUI.instance.IsOK )
        {
            GameBlackUI.instance.showBlack( 1 , onReloadBattle );
        }
        else
        {
            GameBlackUI.instance.showBlack( 1 , onLoadTitle );
        }
    }

    public void onClick()
    {
        if ( !isPlayOver )
        {
            gameAnimation.playAnimation( gameAnimation.saf1.Length - 1 , GameDefine.INVALID_ID , false , null );

            isPlayOver = true;
            GameTouchCenterUI.instance.showUI();

            return;
        }

        if ( !isShowMsgBox )
        {
            GameMsgBoxChooseUI.instance.showText( GameStringData.instance.getString( GameStringType.GameOver0 ) ,
                            GameStringData.instance.getString( GameStringType.GameOver1 ) ,
                            GameStringData.instance.getString( GameStringType.GameOver2 ) ,
                            true ,
                            onMsgBoxClick );

            GameMsgBoxChooseUI.instance.showFade();

            isShowMsgBox = true;
        }

    }

}

