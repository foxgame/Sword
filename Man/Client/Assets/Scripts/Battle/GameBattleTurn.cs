using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum GameBattleTurnType
{
    User = 0,
    Enemy,

    Count
}


public class GameBattleTurn : Singleton<GameBattleTurn>
{
    float time = 0.0f;
    bool startFade = false;
    float alpha = 0.0f;
    float dis = 0.0f;

    [SerializeField]
    GameBattleTurnType type;
    [SerializeField]
    int turn;

    GameAnimation gameAnimation;
    RectTransform transNumbers;

    public int Turn { get { return turn; } }

    public bool Ishow { get { return startFade; } }

    public bool IsStartEvent { get; set; }

    public bool UserTurn
    {
        get
        {
            return type == GameBattleTurnType.User;
        }
    }

    public bool EnemyTurn
    {
        get
        {
            return type == GameBattleTurnType.Enemy;
        }
    }
    public bool Pause { get; set; }

    public override void initSingleton()
    {
        gameAnimation = transform.Find( "Cha_tur" ).GetComponent<GameAnimation>();
        transNumbers = transform.Find( "Numbers" ).GetComponent<RectTransform>();

        gameObject.SetActive( false );
    }


    public void checkOver()
    {
        if ( GameBattleJudgment.instance.check() )
        {
            return;
        }

        if ( type == GameBattleTurnType.User )
        {
            if ( GameBattleUnitManager.instance.getValidUserCount() == 0 )
            {
                nextTurn();
            }
        }
        else
        {
            if ( GameBattleUnitManager.instance.getValidEnemyCount() == 0 )
            {
                nextTurn();
            }
        }
    }


    public void nextTurn()
    {
        if ( Pause )
        {
            return;
        }

        GameBattleCursor.instance.unShow();

        if ( type == GameBattleTurnType.User )
        {
            type = GameBattleTurnType.Enemy;
        }
        else
        {
            turn++;
            type = GameBattleTurnType.User;
            GameUserData.instance.TurnCountAll++;

            GameBattleJudgment.instance.Proficiency13 = 0;
        }

        GameBattleUnitManager.instance.doActed( showAnimation );

        Resources.UnloadUnusedAssets();
    }

    public void start( int t )
    {
        turn = t;
        type = GameBattleTurnType.User;
    }

//     private void Start()
//     {
//         RectTransform trans = GetComponent<RectTransform>();
//         trans.anchoredPosition = new Vector2( trans.anchoredPosition.x + GameCameraManager.instance.xOffset ,
//             trans.anchoredPosition.y );
//     }

    public void start()
    {
        turn = 1;
        type = GameBattleTurnType.User;

        GameBattleUnitManager.instance.showUnits( 0 );

        checkEvent( GameBattleTurnEventType.Start , onEventStartOver );
    }

    public void checkEvent( GameBattleTurnEventType type , OnEventOver over )
    {
        GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;

        for ( int i = 0 ; i < stage.TEVT.Length ; i++ )
        {
            if ( ( stage.TEVT[ i ].EventTurn == turn || stage.TEVT[ i ].EventTurn == GameDefine.INVALID_ID ) && 
                stage.TEVT[ i ].EventType == type )
            {
                GameBattleEventManager.instance.showEvent( stage.TEVT[ i ].EventID , 0 , over );
                return;
            }

//             if ( stage.TEVT[ i ].EventTurn < 0 &&
//                 GameBattleEventManager.instance.showedEvent( stage.TEVT[ i ].EventID - stage.TEVT[ i ].EventTurn ) )
//             {
//                 GameBattleEventManager.instance.showEvent( stage.TEVT[ i ].EventID , 0 , over );
//                 return;
//             }
        }

        over();
    }

    void onEventStartOver()
    {
        if ( GameUserData.instance.Stage == 34 )
        {
            GameBattleManager.instance.initMusic();
        }

        GameBattleManager.instance.updateTreasures();

        showAnimation();
    }

    //     void checkEvent()
    //     {
    //         GameBattleStage stage = GameBattleManager.instance.ActiveBattleStage;
    // 
    //         for ( int i = 0 ; i < stage.TEVT.Length ; i++ )
    //         {
    //             if ( stage.TEVT[ i ].EventTurn == turn &&
    //                 type == GameBattleTurnType.User )
    //             {
    //                 if ( stage.TEVT[ i ].EventType == GameBattleTurnEventType.Start )
    //                 {
    //                 }
    // 
    //                 GameBattleEventManager.instance.showEvent( stage.TEVT[ i ].EventID , 0 , onCheckEvent );
    //                 return;
    //             }
    //         }
    // 
    //         onCheckEvent();
    //     }

    void onCheckEvent()
    {
        
    }

    void onAnimationOver()
    {
        gameObject.SetActive( false );

        if ( turn == 1 )
        {
            if ( type == GameBattleTurnType.Enemy )
            {
                GameBattleUnitManager.instance.enemyAI();
            }
            else
            {
                GameBattleUnitManager.instance.npcAI();
            }
        }
        else
        {
            if ( type == GameBattleTurnType.Enemy )
            {
                checkEvent( GameBattleTurnEventType.Enemy , GameBattleUnitManager.instance.enemyAI );
            }
            else
            {
                checkEvent( GameBattleTurnEventType.Start , GameBattleUnitManager.instance.npcAI );
            }
        }       
    }

    void clear()
    {
        GameDefine.DestroyAll( transNumbers );
    }

    void showAnimation()
    {
        clear();

        time = 0.0f;
        startFade = false;
        alpha = 0.0f;
        dis = 0.0f;

        if ( type == GameBattleTurnType.User )
        {
            gameAnimation.playAnimation( 12 , 27 , false , null );
        }
        else
        {
            gameAnimation.playAnimation( 28 , 43 , false , null );
        }
        
        int n1 = turn / 10;
        int n2 = turn % 10;
        int x = 215;
        int y = -170;

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Image" ) );

        RectTransform trans = obj.GetComponent<RectTransform>();
        trans.SetParent( transNumbers );
        trans.anchoredPosition = new Vector2( x + gameAnimation.saf1[ 0 ].saf11[ 0 ].textureX ,
            y - gameAnimation.saf1[ 0 ].saf11[ 0 ].textureY );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        Image image = obj.GetComponent<Image>();
        image.sprite = gameAnimation.sprites[ gameAnimation.saf1[ 0 ].saf11[ 0 ].textureID ];

        float scale = 0.5f;
        trans.sizeDelta = new Vector2( image.sprite.rect.width * scale , image.sprite.rect.height * scale );

        if ( n1 > 1 )
        {
            x += (int)( trans.sizeDelta.y );
            obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Image" ) );

            trans = obj.GetComponent<RectTransform>();
            trans.SetParent( transNumbers );
            trans.anchoredPosition = new Vector2( x + gameAnimation.saf1[ n1 ].saf11[ 0 ].textureX ,
                y - gameAnimation.saf1[ n1 ].saf11[ 0 ].textureY );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

            image = obj.GetComponent<Image>();
            image.sprite = gameAnimation.sprites[ gameAnimation.saf1[ n1 ].saf11[ 0 ].textureID ];
            image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );

            trans.sizeDelta = new Vector2( image.sprite.rect.width * scale , image.sprite.rect.height * scale );
        }

        if ( n1 > 0 )
        {
            x += (int)( trans.sizeDelta.y );
            obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Image" ) );

            trans = obj.GetComponent<RectTransform>();
            trans.SetParent( transNumbers );
            trans.anchoredPosition = new Vector2( x + gameAnimation.saf1[ 10 ].saf11[ 0 ].textureX ,
                y - gameAnimation.saf1[ 10 ].saf11[ 0 ].textureY );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

            image = obj.GetComponent<Image>();
            image.sprite = gameAnimation.sprites[ gameAnimation.saf1[ 10 ].saf11[ 0 ].textureID ];
            image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );

            trans.sizeDelta = new Vector2( image.sprite.rect.width * scale , image.sprite.rect.height * scale );
            x += 1;
        }

        if ( n2 > 0 )
        {
            x += (int)( trans.sizeDelta.y );
            obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Image" ) );

            trans = obj.GetComponent<RectTransform>();
            trans.SetParent( transNumbers );
            trans.anchoredPosition = new Vector2( x + gameAnimation.saf1[ n2 ].saf11[ 0 ].textureX , 
                y - gameAnimation.saf1[ n2 ].saf11[ 0 ].textureY );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

            image = obj.GetComponent<Image>();
            image.sprite = gameAnimation.sprites[ gameAnimation.saf1[ n2 ].saf11[ 0 ].textureID ];
            image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );

            trans.sizeDelta = new Vector2( image.sprite.rect.width * scale , image.sprite.rect.height * scale );
        }

        startFade = true;
        transNumbers.anchoredPosition = new Vector2( -15.0f , 0.0f );
        gameObject.SetActive( true );
    }



    void Update()
    {
        if ( !startFade )
        {
            return;
        }

        float d = 15.0f;
        float t = 1.0f;

        time += Time.deltaTime;

        if ( time < t )
        {
            dis = time * d * t;
            alpha = time * t;

            transNumbers.anchoredPosition = new Vector2( 15.0f - dis , 0.0f );

            for ( int i = 0 ; i < transNumbers.childCount ; i++ )
            {
                Image image = transNumbers.GetChild( i ).GetComponent<Image>();

                Color c = image.color;
                c.a = alpha;
                image.color = c;
            }
        }
        else if ( time >= t + 0.8f )
        {
            startFade = false;

            gameObject.SetActive( false );

            onAnimationOver();
        }

    }

}
