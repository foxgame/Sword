using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleUserLeftUI : GameUI<GameBattleUserLeftUI>
{
    Text text;
    Text hp;
    Text hpMax;
    Text mp;
    Text mpMax;
    Text move;
    Text moveMax;

    RectTransform trans;

    public override void initSingleton()
    {
        text = transform.Find( "name" ).GetComponent<Text>();
        hp = transform.Find( "hp" ).GetComponent<Text>();
        hpMax = transform.Find( "hpMax" ).GetComponent<Text>();
        mp = transform.Find( "mp" ).GetComponent<Text>();
        mpMax = transform.Find( "mpMax" ).GetComponent<Text>();
        move = transform.Find( "move" ).GetComponent<Text>();
        moveMax = transform.Find( "moveMax" ).GetComponent<Text>();

        trans = GetComponent<RectTransform>();
    }


    public void show( GameBattleUnit unit , bool top )
    {
        show();

#if ( UNITY_ANDROID || UNITY_IPHONE )
        top = true;
#endif

        trans.anchoredPosition = top ? new Vector2( 0.0f , 166.0f + GameCanvasScale.instance.Height - GameDefine.SCENE_HEIGHT ) : Vector2.zero;

        text.text = unit.Name;

        hp.text = GameDefine.getBigInt( unit.HP.ToString() , true );
        hpMax.text = GameDefine.getBigInt( unit.HPMax.ToString() , true );
        mp.text = GameDefine.getBigInt( unit.MP.ToString() , true );
        mpMax.text = GameDefine.getBigInt( unit.MPMax.ToString() , true );
        move.text = GameDefine.getBigInt( unit.Move.ToString() );
        moveMax.text = GameDefine.getBigInt( unit.MoveMax.ToString() );

        showFade();
    }



}

