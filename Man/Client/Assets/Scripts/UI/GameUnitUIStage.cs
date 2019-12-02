using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUIStage : MonoBehaviour
{
    Text nameText;
    Text lvText;
    Text moveText;
    Text moveMaxText;
    Text expText;
    Text hpText;
    Text hpMaxText;
    Text mpText;
    Text mpMaxText;
    Text strText;
    Text intText;
    Text vitText;
    Text avgText;
    Text lukText;

    RectTransform transHP;
    RectTransform transMP;

    GameAnimation[] power = new GameAnimation[ (int)GameSpiritType.Count ];
    Text[] powerText = new Text[ (int)GameSpiritType.Count ];

    Image[] image = new Image[ 13 ];

    void Awake()
    {
        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            power[ i ] = transform.Find( "power" + i ).GetComponentInChildren<GameAnimation>();
            powerText[ i ] = transform.Find( "power" + i + "Text" ).GetComponentInChildren<Text>();
        }

        for ( int i = 0 ; i < 13 ; i++ )
        {
            image[ i ] = transform.Find( "stage" + i ).GetComponent<Image>();
        }

        transHP = transform.Find( "hpBar/hp" ).GetComponent<RectTransform>();
        transMP = transform.Find( "mpBar/mp" ).GetComponent<RectTransform>();

        nameText = transform.Find( "name" ).GetComponent<Text>();
        lvText = transform.Find( "lv" ).GetComponent<Text>();
        moveText = transform.Find( "move" ).GetComponent<Text>();
        moveMaxText = transform.Find( "moveMax" ).GetComponent<Text>();
        expText = transform.Find( "exp" ).GetComponent<Text>();
        hpText = transform.Find( "hp" ).GetComponent<Text>();
        hpMaxText = transform.Find( "hpMax" ).GetComponent<Text>();
        mpText = transform.Find( "mp" ).GetComponent<Text>();
        mpMaxText = transform.Find( "mpMax" ).GetComponent<Text>();

        strText = transform.Find( "str" ).GetComponent<Text>();
        intText = transform.Find( "int" ).GetComponent<Text>();
        vitText = transform.Find( "vit" ).GetComponent<Text>();
        avgText = transform.Find( "avg" ).GetComponent<Text>();
        lukText = transform.Find( "luk" ).GetComponent<Text>();
    }

    public void clear()
    {
        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            power[ i ].stopAnimation();
            power[ i ].clearAnimation();
        }

        for ( int i = 0 ; i < 13 ; i++ )
        {
            image[ i ].gameObject.SetActive( false );
        }
    }

    public void updateData( GameBattleUnit battleUnit )
    {
        clear();

        GameUnit gameUnit = GameUnitData.instance.getData( battleUnit.UnitID );

        transHP.anchoredPosition = new Vector2( -2.0f , 0.0f );
        transMP.anchoredPosition = new Vector2( -2.0f , 0.0f );

        transHP.anchoredPosition = new Vector2( -2.0f - 75.0f * ( 1.0f - (float)battleUnit.HP / battleUnit.HPMax ) , 0.0f );
        if ( battleUnit.MPMax > 0 )
        {
            transMP.anchoredPosition = new Vector2( -2.0f - 75.0f * ( 1.0f - (float)battleUnit.MP / battleUnit.MPMax ) , 0.0f );
        }

        nameText.text = gameUnit.Name;

        lvText.text = GameDefine.getBigInt( battleUnit.LV.ToString() );

        moveText.text = GameDefine.getBigInt( battleUnit.Move.ToString() );
        moveMaxText.text = GameDefine.getBigInt( battleUnit.MoveMax.ToString() );

        hpText.text = GameDefine.getBigInt( battleUnit.HP.ToString() , true );
        hpMaxText.text = GameDefine.getBigInt( battleUnit.HPMax.ToString() , true );

        mpText.text = GameDefine.getBigInt( battleUnit.MP.ToString() , true );
        mpMaxText.text = GameDefine.getBigInt( battleUnit.MPMax.ToString() , true );

        strText.text = GameDefine.getBigInt( battleUnit.Str.ToString() );
        intText.text = GameDefine.getBigInt( battleUnit.Int.ToString() );
        vitText.text = GameDefine.getBigInt( battleUnit.Vit.ToString() );
        avgText.text = GameDefine.getBigInt( battleUnit.Avg.ToString() );
        lukText.text = GameDefine.getBigInt( battleUnit.Luk.ToString() );

        image[ 0 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.StrUp ) );
        image[ 1 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.VitUp ) );
        image[ 2 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.IntUp ) );
        image[ 3 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.MoveUp ) );

        image[ 4 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.PhyImmunity ) );
        image[ 5 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.MagImmunity ) );

        image[ 6 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Violent ) );

        image[ 7 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Palsy ) );
        image[ 8 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Fetter ) );
        image[ 9 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Silence ) );
        image[ 10 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Miss ) );
        image[ 11 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.Poison ) );
        image[ 12 ].gameObject.SetActive( battleUnit.checkEffect( GameSkillResutlEffect.SummonKiller ) );

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            powerText[ i ].text = GameDefine.getBigInt( battleUnit.SpiritPower[ i ].ToString() );

            int f = (int)( battleUnit.SpiritPower[ i ] / 10.0f );

            if ( battleUnit.SpiritPower[ i ] % 10 > 0 )
            {
                f += 1;
            }

            if ( f > 10 )
            {
                f = 10;
            }

            if ( battleUnit.SpiritPower[ i ] > 0 )
            {
                power[ i ].showFrame( 3 + f );
            }
        }
    }

    public void updateData( GameUnitBase unitBase )
    {
        clear();

        GameUnit gameUnit = GameUnitData.instance.getData( unitBase.UnitID );

        short Str = unitBase.Str;
        short Vit = unitBase.Vit;
        short Avg = unitBase.Avg;
        short Int = unitBase.Int;
        short Luk = unitBase.Luk;

        int hp = unitBase.HP;
        int mp = unitBase.MP;

        short move = (short)( Avg / 25 + gameUnit.Move );

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            powerText[ i ].text = GameDefine.getBigInt( unitBase.SpiritPower[ i ].ToString() );

            int f = (int)( unitBase.SpiritPower[ i ] / 10.0f );

            if ( unitBase.SpiritPower[ i ] % 10 > 0 )
            {
                f += 1;
            }

            if ( f > 10 )
            {
                f = 10;
            }

            if ( unitBase.SpiritPower[ i ] > 0 )
            {
                power[ i ].showFrame( 3 + f );
            }
        }

        nameText.text = gameUnit.Name;

        lvText.text = GameDefine.getBigInt( unitBase.LV.ToString() );

        moveText.text = GameDefine.getBigInt( move.ToString() );
        moveMaxText.text = GameDefine.getBigInt( move.ToString() );

        hpText.text = GameDefine.getBigInt( hp.ToString() );
        hpMaxText.text = GameDefine.getBigInt( hp.ToString() );

        mpText.text = GameDefine.getBigInt( mp.ToString() );
        mpMaxText.text = GameDefine.getBigInt( mp.ToString() );

        strText.text = GameDefine.getBigInt( Str.ToString() );
        intText.text = GameDefine.getBigInt( Int.ToString() );
        vitText.text = GameDefine.getBigInt( Vit.ToString() );
        avgText.text = GameDefine.getBigInt( Avg.ToString() );
        lukText.text = GameDefine.getBigInt( Luk.ToString() );

//         transHP.anchoredPosition = new Vector2( -2.0f , 0.0f );
//         transMP.anchoredPosition = new Vector2( -2.0f , 0.0f );
    }


}

