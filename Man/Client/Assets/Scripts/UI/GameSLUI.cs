using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum GameSLType
{
    Save = 0,
    Load,

    Count
}

public class GameSLUI : GameUI<GameSLUI>
{
    GameSLUISlot[] slot = new GameSLUISlot[ GameDefine.MAX_SAVE ];

    GameSLType slType;

    Text text;
    Text loading;

    int selection = 0;
    GameAskUI askUI;

    public int Selection { get { return selection; } }
    public GameSLType Type { get { return slType; } }


    public override void initSingleton()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            slot[ i ] = transform.Find( "item" + i ).GetComponent<GameSLUISlot>();
        }

        text = transform.Find( "text" ).GetComponent<Text>();
        loading = transform.Find( "loading" ).GetComponent<Text>();

        askUI = GetComponentInChildren<GameAskUI>();
    }

    void onLoadGame()
    {
        GameUserData.instance.load( selection , false );
    }

    public void showLoading()
    {
        loading.text = GameStringData.instance.getString( GameStringType.Cloud0 );
    }
    public void showLoadingComplete()
    {
        loading.text = GameStringData.instance.getString( GameStringType.Cloud1 );
    }

    public void onClick()
    {
        if ( IsShowAskUI )
        {
            if ( IsOKAskUI )
            {
                switch ( slType )
                {
                    case GameSLType.Save:
                        {
                            GameUserData.instance.save( selection , false );

                            updateData();
                        }
                        break;
                    case GameSLType.Load:
                        {
                            GameBlackUI.instance.showBlack( 0.5f , onLoadGame );
                        }
                        break;
                }
            }
            else
            {
            }

            unShowAskUI();
        }
        else
        {
            switch ( slType )
            {
                case GameSLType.Save:
                        showAskUI( true );
                    break;
                case GameSLType.Load:
                    {
                        if ( GameUserData.instance.SaveInfo[ selection ].Stage != 0 )
                        {
                            showAskUI( true );
                        }
                    }
                    break;
            }

        }
    }

    public void select( int n )
    {
        selection = n;

        if ( selection < 0 )
        {
            selection = 0;
        }

        if ( selection >= GameDefine.MAX_SAVE - 1 )
        {
            selection = GameDefine.MAX_SAVE - 1;
        }

        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            slot[ i ].clear();
        }

        slot[ selection ].select();
    }

    public void updateData()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            slot[ i ].setData( GameUserData.instance.SaveInfo[ i ] );
        }
    }

    public void clear()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            slot[ i ].clear();
        }
    }

    public void show( GameSLType t )
    {
        slType = t;

        show();

        text.text = GameStringData.instance.getString( GameStringType.CloudSL8 + (int)t );
        loading.text = "";

        updateData();
    }

    protected override void onUIFadeIn()
    {
        base.onUIFadeIn();

        select( 0 );
    }

    public void showAskUI( bool b )
    {
        askUI.show( b );

        RectTransform trans = slot[ selection ].GetComponent<RectTransform>();
        askUI.setPos( trans.anchoredPosition.x + 80 , trans.anchoredPosition.y + 11 );
    }

    public void unShowAskUI()
    {
        askUI.unShow();
    }

    public bool IsShowAskUI { get { return askUI.IsShow; } }
    public bool IsOKAskUI { get { return askUI.IsOK; } }
}


