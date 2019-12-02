using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelUpUI : GameUI<GameLevelUpUI>
{
    Text level0;
    Text level1;
    Text hp0;
    Text hp1;
    Text mp0;
    Text mp1;
    Text str0;
    Text str1;
    Text vit0;
    Text vit1;
    Text int0;
    Text int1;
    Text avg0;
    Text avg1;
    Text luk0;
    Text luk1;
    Text skill;

    float time;

    OnEventOver onEventOver;


    public override void initSingleton()
    {
        level0 = transform.Find( "level0" ).GetComponent<Text>();
        level1 = transform.Find( "level1" ).GetComponent<Text>();
        hp0 = transform.Find( "hp0" ).GetComponent<Text>();
        hp1 = transform.Find( "hp1" ).GetComponent<Text>();
        mp0 = transform.Find( "mp0" ).GetComponent<Text>();
        mp1 = transform.Find( "mp1" ).GetComponent<Text>();
        str0 = transform.Find( "str0" ).GetComponent<Text>();
        str1 = transform.Find( "str1" ).GetComponent<Text>();
        vit0 = transform.Find( "vit0" ).GetComponent<Text>();
        vit1 = transform.Find( "vit1" ).GetComponent<Text>();
        int0 = transform.Find( "int0" ).GetComponent<Text>();
        int1 = transform.Find( "int1" ).GetComponent<Text>();
        avg0 = transform.Find( "avg0" ).GetComponent<Text>();
        avg1 = transform.Find( "avg1" ).GetComponent<Text>();
        luk0 = transform.Find( "luk0" ).GetComponent<Text>();
        luk1 = transform.Find( "luk1" ).GetComponent<Text>();

        skill = transform.Find( "skill" ).GetComponent<Text>();
    }

    public void show( OnEventOver over , GameBattleUnit unit , GameUnitLevelUp lv )
    {
        onEventOver = over;

        show();

        level0.text = GameDefine.getBigInt( ( unit.LV - 1 ).ToString() );
        level1.text = GameDefine.getBigInt( ( unit.LV ).ToString() );

        hp0.text = GameDefine.getBigInt( unit.HPMax.ToString() );
        hp1.text = GameDefine.getBigInt( ( unit.HPMax + lv.HP ).ToString() );

        mp0.text = GameDefine.getBigInt( unit.MPMax.ToString() );
        mp1.text = GameDefine.getBigInt( ( unit.MPMax + lv.MP ).ToString() );

        str0.text = GameDefine.getBigInt( unit.Str.ToString() );
        str1.text = GameDefine.getBigInt( ( unit.Str + lv.StrBase + lv.StrRand ).ToString() );

        vit0.text = GameDefine.getBigInt( unit.Vit.ToString() );
        vit1.text = GameDefine.getBigInt( ( unit.Vit + lv.VitBase + lv.VitRand ).ToString() );

        int0.text = GameDefine.getBigInt( unit.Int.ToString() );
        int1.text = GameDefine.getBigInt( ( unit.Int + lv.IntBase + lv.IntRand ).ToString() );

        avg0.text = GameDefine.getBigInt( unit.Avg.ToString() );
        avg1.text = GameDefine.getBigInt( ( unit.Avg + lv.AvgBase + lv.AvgRand ).ToString() );

        luk0.text = GameDefine.getBigInt( unit.Luk.ToString() );
        luk1.text = GameDefine.getBigInt( ( unit.Luk + lv.LukBase + lv.LukRand ).ToString() );

        if ( lv.Skill[ 0 ] != null )
        {
            GameSkill m = GameSkillData.instance.getData( lv.Skill[ 0 ].SkillID );
            skill.text = m.Name;
        }
        else
        {
            skill.text = "";
        }

        time = 0.0f;
    }

    protected override void onUpdate()
    {
        if ( !IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > 5.0f )
        {
            onClick();
            time = 0.0f;
        }
    }

    public void onClick()
    {
        unShow();

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

}

