using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;




public class GameTitleInput : Singleton<GameTitleInput>
{
    float time = 0.0f;

    bool confirm = false;
    bool cancel = false;

    


    void Update()
    {
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

        updateSLUI();

        updateCloudSLUI();

        updateHelpUI();

        updateTitleUI();
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


    void moveTitleUI()
    {
        bool bMove = false;

        int selection = GameTitleUI.instance.Selection;

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

        GameTitleUI.instance.select( selection );
    }

    void moveHelpUI()
    {
        bool bMove = false;

        int selection = GameHelpUI.instance.Selection;

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

        GameHelpUI.instance.select( selection );
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

                GameBlackUI.instance.showBlack( 1.0f , onShowBlackOverUnShowSLUI );
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

                GameBlackUI.instance.showBlack( 1.0f , onShowBlackOverCloudUnShowSLUI );
            }

            return;
        }

        moveCloudSLUI();
    }

    void onShowBlackOverUnShowSLUI()
    {
        GameSLUI.instance.unShow();
        GameSLUI.instance.updateUIStageFadeOver();

        GameTitleUI.instance.show();
        GameTitleUI.instance.showFade();
        GameTitleUI.instance.Pause = true;

        GameBlackUI.instance.unShowBlack( 0.5f , onShowBlackOverUnShowSLUI1 );
    }

    void onShowBlackOverCloudUnShowSLUI()
    {
        GameSLCloudUI.instance.unShow();
        GameSLCloudUI.instance.updateUIStageFadeOver();

        GameTitleUI.instance.show();
        GameTitleUI.instance.showFade();
        GameTitleUI.instance.Pause = true;

        GameBlackUI.instance.unShowBlack( 0.5f , onShowBlackOverUnShowSLUI1 );
    }

    void onShowBlackOverUnShowSLUI1()
    {
        GameTitleUI.instance.Pause = false;
    }

    void onShowBlackOverLoadGame()
    {
        GameTitleUI.instance.unShowFade();

        GameSLUI.instance.show( GameSLType.Load );
        GameSLUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void onNewGame()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Battle ,
                            GameSceneLoadMode.StartBattle );
    }

    void onLoadBattle()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Battle ,
                           GameSceneLoadMode.LoadBattle );
    }

    void updateTitleUI()
    {
        if ( !GameTitleUI.instance.IsShow )
        {
            return;
        }

        if ( GameSLUI.instance.IsShow || 
            GameBlackUI.instance.IsShow ||
            !GameTitleUI.instance.IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( confirm )
        {
            confirm = false;

            switch ( GameTitleUI.instance.Selection )
            {
                case 0:
                    {
                        if ( time < 1.0f )
                        {
                            return;
                        }

                        GameUserData.instance.setStage( 0 );

                        GameBlackUI.instance.showBlack( 0.5f , onNewGame );
                    }
                    break;
                case 1:
                    {
                        GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadGame );
                    }
                    break;
                case 2:
                    {
                        string path = GameSetting.PersistentDataPath + "/SaveBattle.dat";

                        if ( !File.Exists( path ) )
                        {
                            return;
                        }

                        GameBlackUI.instance.showBlack( 0.5f , onLoadBattle );
                    }
                    break;
                case 3:
                    {
                        GameHelpUI.instance.show();
                        GameHelpUI.instance.select( 0 );
                    }
                    break;
            }


            return;
        }

        if ( cancel )
        {
            cancel = false;

            return;
        }

        moveTitleUI();
    }


    void updateHelpUI()
    {
        if ( !GameHelpUI.instance.IsShow )
        {
            return;
        }

        if ( GameSLUI.instance.IsShow ||
            GameBlackUI.instance.IsShow ||
            !GameHelpUI.instance.IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( confirm )
        {
            confirm = false;

            switch ( GameHelpUI.instance.Selection )
            {
                case 0:
                    GameHelpUI.instance.onClick0();
                    break;
                case 1:
                    GameHelpUI.instance.onClick1();
                    break;
                case 2:
                    GameHelpUI.instance.onClick2();
                    break;
                case 3:
                    GameHelpUI.instance.onClick3();
                    break;
                case 4:
                    GameHelpUI.instance.onClick4();
                    break;
                case 5:
                    GameHelpUI.instance.onClick5();
                    break;
                case 6:
                    GameHelpUI.instance.onSetting0();
                    break;
                case 7:
                    GameHelpUI.instance.onSetting1();
                    break;
                case 8:
                    GameHelpUI.instance.onSetting20();
                    break;
                case 9:
                    GameHelpUI.instance.onSetting21();
                    break;
                case 10:
                    GameHelpUI.instance.onClick6();
                    break;
                case 11:
                    GameHelpUI.instance.onClick7();
                    break;

            }

            return;
        }

        if ( cancel )
        {
            cancel = false;
            GameHelpUI.instance.unShow();
            return;
        }

        moveHelpUI();
    }

}


