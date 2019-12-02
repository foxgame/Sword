using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleUserRightUI : GameUI<GameBattleUserRightUI>
{
    Text attack;
    Text defence;
    Text lv;
    Text exp;

    GameObject effect0;
    GameObject effect1;
    GameObject effect2;
    GameObject effect3;

    RectTransform trans;

    public override void initSingleton()
    {
        attack = transform.Find( "attack" ).GetComponent<Text>();
        defence = transform.Find( "defence" ).GetComponent<Text>();
        lv = transform.Find( "lv" ).GetComponent<Text>();
        exp = transform.Find( "exp" ).GetComponent<Text>();

        effect0 = transform.Find( "effect0" ).gameObject;
        effect1 = transform.Find( "effect1" ).gameObject;
        effect2 = transform.Find( "effect2" ).gameObject;
        effect3 = transform.Find( "effect3" ).gameObject;

        trans = GetComponent<RectTransform>();
    }

    public void show( GameBattleUnit unit , bool top )
    {
        show();

#if ( UNITY_ANDROID || UNITY_IPHONE )
        top = true;
#endif

        trans.anchoredPosition = top ? new Vector2( 0.0f , 166.0f + GameCanvasScale.instance.Height - GameDefine.SCENE_HEIGHT ) : Vector2.zero;

        attack.text = GameDefine.getBigInt( unit.PhysicalAttack.ToString() );
        defence.text = GameDefine.getBigInt( unit.Defence.ToString() );
        lv.text = GameDefine.getBigInt( unit.LV.ToString() );
        exp.text = GameDefine.getBigInt( unit.EXP.ToString() );

        effect0.SetActive( unit.checkEffect( GameSkillResutlEffect.StrUp ) );
        effect1.SetActive( unit.checkEffect( GameSkillResutlEffect.VitUp ) );
        effect2.SetActive( unit.checkEffect( GameSkillResutlEffect.IntUp ) );
        effect3.SetActive( unit.checkEffect( GameSkillResutlEffect.MoveUp ) );

        showFade();
    }



}

