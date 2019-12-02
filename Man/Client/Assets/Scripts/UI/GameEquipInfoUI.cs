using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameEquipInfoUI : GameUI< GameEquipInfoUI >
{
    RectTransform trans;

    Text attack;
    Text attack1;
    Text defence;
    Text defence1;
    Text hit;
    Text hit1;
    Text speed;
    Text speed1;
    Text fire;
    Text fire1;
    Text thunder;
    Text thunder1;
    Text ice;
    Text ice1;
    Text illusion;
    Text illusion1;
    Text light0;
    Text light1;
    Text dark;
    Text dark1;


    Color defaultColor;

    public override void initSingleton()
    {
        trans = transform.GetComponent<RectTransform>();

        attack = trans.Find( "info/attack" ).GetComponent<Text>();
        attack1 = trans.Find( "info/attack1" ).GetComponent<Text>();
        defence = trans.Find( "info/defence" ).GetComponent<Text>();
        defence1 = trans.Find( "info/defence1" ).GetComponent<Text>();
        hit = trans.Find( "info/hit" ).GetComponent<Text>();
        hit1 = trans.Find( "info/hit1" ).GetComponent<Text>();
        speed = trans.Find( "info/speed" ).GetComponent<Text>();
        speed1 = trans.Find( "info/speed1" ).GetComponent<Text>();
        fire = trans.Find( "info/fire" ).GetComponent<Text>();
        fire1 = trans.Find( "info/fire1" ).GetComponent<Text>();
        thunder = trans.Find( "info/thunder" ).GetComponent<Text>();
        thunder1 = trans.Find( "info/thunder1" ).GetComponent<Text>();
        ice = trans.Find( "info/ice" ).GetComponent<Text>();
        ice1 = trans.Find( "info/ice1" ).GetComponent<Text>();
        illusion = trans.Find( "info/illusion" ).GetComponent<Text>();
        illusion1 = trans.Find( "info/illusion1" ).GetComponent<Text>();
        light0 = trans.Find( "info/light" ).GetComponent<Text>();
        light1 = trans.Find( "info/light1" ).GetComponent<Text>();
        dark = trans.Find( "info/dark" ).GetComponent<Text>();
        dark1 = trans.Find( "info/dark1" ).GetComponent<Text>();

        defaultColor = attack.color;
    }

    public void setPos( float x , float y )
    {
        trans.anchoredPosition = new Vector2( x , y );
    }

    public void showItem( GameUnitBase unitBase , GameItem item )
    {
        GameUnit gameUnit = GameUnitData.instance.getData( unitBase.UnitID );

        GameBattleUnitData unitData = new GameBattleUnitData();
        GameBattleUnitData unitData1 = new GameBattleUnitData();

        GameItem weapon = GameItemData.instance.getData( unitBase.Weapon );
        GameItem armor = GameItemData.instance.getData( unitBase.Armor ); ;
        GameItem accessory = GameItemData.instance.getData( unitBase.Accessory ); ;

        GameAttributeDefence md = GameAttributeDefenceData.instance.getData( gameUnit.AttributeDefenceID );

        for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
        {
            if ( md != null )
                unitData.AttributeDefence[ i ] = md.AttributeDefence[ i ];
            else
                unitData.AttributeDefence[ i ] = 100;
        }

        unitData.Attack += unitBase.Str;
        unitData.Defence += unitBase.Vit / 2;

        if ( weapon != null )
        {
            unitData.Attack += weapon.Attack;
            unitData.Defence += weapon.Defence;
            unitData.Hit += weapon.AttackHit;
            unitData.Miss += weapon.AttackMiss;
            unitData.Critical += weapon.AttackCritical;
            unitData.Double += weapon.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( weapon.AttributeDefence[ i ] * unitData.AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( armor != null )
        {
            unitData.Attack += armor.Attack;
            unitData.Defence += armor.Defence;
            unitData.Hit += armor.AttackHit;
            unitData.Miss += armor.AttackMiss;
            unitData.Critical += armor.AttackCritical;
            unitData.Double += armor.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( armor.AttributeDefence[ i ] * unitData.AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( accessory != null )
        {
            unitData.Attack += accessory.Attack;
            unitData.Defence += accessory.Defence;
            unitData.Hit += accessory.AttackHit;
            unitData.Miss += accessory.AttackMiss;
            unitData.Critical += accessory.AttackCritical;
            unitData.Double += accessory.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( accessory.AttributeDefence[ i ] * unitData.AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( item != null )
        {
            unitData1.Attack = unitData.Attack;
            unitData1.Defence = unitData.Defence;
            unitData1.Hit = unitData.Hit;
            unitData1.Miss = unitData.Miss;
            unitData1.Critical = unitData.Critical;
            unitData1.Double = unitData.Double;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData1.AttributeDefence[ i ] = unitData.AttributeDefence[ i ];
            }

            if ( weapon != null && item.ItemType == GameItemType.Weapon )
            {
                unitData1.Attack -= weapon.Attack;
                unitData1.Defence -= weapon.Defence;
                unitData1.Hit -= weapon.AttackHit;
                unitData1.Miss -= weapon.AttackMiss;
                unitData1.Critical -= weapon.AttackCritical;
                unitData1.Double -= weapon.AttackDouble;

                for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
                {
                    unitData1.AttributeDefence[ i ] += (short)( weapon.AttributeDefence[ i ] * unitData1.AttributeDefence[ i ] / 100.0f );
                }
            }

            if ( armor != null && item.ItemType == GameItemType.Armor )
            {
                unitData1.Attack -= armor.Attack;
                unitData1.Defence -= armor.Defence;
                unitData1.Hit -= armor.AttackHit;
                unitData1.Miss -= armor.AttackMiss;
                unitData1.Critical -= armor.AttackCritical;
                unitData1.Double -= armor.AttackDouble;

                for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
                {
                    unitData1.AttributeDefence[ i ] += (short)( armor.AttributeDefence[ i ] * unitData1.AttributeDefence[ i ] / 100.0f );
                }
            }

            if ( accessory != null && item.ItemType == GameItemType.Accessory )
            {
                unitData1.Attack -= accessory.Attack;
                unitData1.Defence -= accessory.Defence;
                unitData1.Hit -= accessory.AttackHit;
                unitData1.Miss -= accessory.AttackMiss;
                unitData1.Critical -= accessory.AttackCritical;
                unitData1.Double -= accessory.AttackDouble;

                for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
                {
                    unitData1.AttributeDefence[ i ] += (short)( accessory.AttributeDefence[ i ] * unitData1.AttributeDefence[ i ] / 100.0f );
                }
            }

            unitData1.Attack += item.Attack;
            unitData1.Defence += item.Defence;
            unitData1.Hit += item.AttackHit;
            unitData1.Miss += item.AttackMiss;
            unitData1.Critical += item.AttackCritical;
            unitData1.Double += item.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData1.AttributeDefence[ i ] -= (short)( item.AttributeDefence[ i ] * unitData1.AttributeDefence[ i ] / 100.0f );
            }
        }

        attack.text = GameDefine.getBigInt( unitData.Attack.ToString() );
        defence.text = GameDefine.getBigInt( unitData.Defence.ToString() );
        hit.text = GameDefine.getBigInt( unitData.Hit.ToString() );
        speed.text = GameDefine.getBigInt( unitData.Miss.ToString() );
        fire.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Fire ].ToString() );
        thunder.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Thunder ].ToString() );
        ice.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Ice ].ToString() );
        illusion.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Illusion ].ToString() );
        light0.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Light ].ToString() );
        dark.text = GameDefine.getBigInt( unitData.AttributeDefence[ (int)GameAttributeType.Dark ].ToString() );

        attack1.text = GameDefine.getBigInt( unitData1.Attack.ToString() );
        defence1.text = GameDefine.getBigInt( unitData1.Defence.ToString() );
        hit1.text = GameDefine.getBigInt( unitData1.Hit.ToString() );
        speed1.text = GameDefine.getBigInt( unitData1.Miss.ToString() );
        fire1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Fire ].ToString() );
        thunder1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Thunder ].ToString() );
        ice1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Ice ].ToString() );
        illusion1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Illusion ].ToString() );
        light1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Light ].ToString() );
        dark1.text = GameDefine.getBigInt( unitData1.AttributeDefence[ (int)GameAttributeType.Dark ].ToString() );


        attack1.color = defaultColor;
        defence1.color = defaultColor;
        hit1.color = defaultColor;
        speed1.color = defaultColor;
        fire1.color = defaultColor;
        thunder1.color = defaultColor;
        ice1.color = defaultColor;
        illusion1.color = defaultColor;
        light1.color = defaultColor;
        dark1.color = defaultColor;

        Color colorRed = Color.red;
        Color colorGreen = new Color( 0.0f , 0.39f , 0.0f );

        if ( unitData1.Attack > unitData.Attack )
            attack1.color = Color.red;
        if ( unitData1.Defence > unitData.Defence )
            defence1.color = Color.red;
        if ( unitData1.Hit > unitData.Hit )
            hit1.color = Color.red;
        if ( unitData1.Miss > unitData.Miss )
            speed1.color = Color.red;

        if ( unitData1.Attack < unitData.Attack )
            attack1.color = colorGreen;
        if ( unitData1.Defence < unitData.Defence )
            defence1.color = colorGreen;
        if ( unitData1.Hit < unitData.Hit )
            hit1.color = colorGreen;
        if ( unitData1.Miss < unitData.Miss )
            speed1.color = colorGreen;


        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Fire ] < unitData.AttributeDefence[ (int)GameAttributeType.Fire ] )
            fire1.color = Color.red;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Thunder ] < unitData.AttributeDefence[ (int)GameAttributeType.Thunder ] )
            thunder1.color = Color.red;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Ice ] < unitData.AttributeDefence[ (int)GameAttributeType.Ice ] )
            ice1.color = Color.red;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Illusion ] < unitData.AttributeDefence[ (int)GameAttributeType.Illusion ] )
            illusion1.color = Color.red;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Light ] < unitData.AttributeDefence[ (int)GameAttributeType.Light ] )
            light1.color = Color.red;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Dark ] < unitData.AttributeDefence[ (int)GameAttributeType.Dark ] )
            dark1.color = Color.red;

        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Fire ] > unitData.AttributeDefence[ (int)GameAttributeType.Fire ] )
            fire1.color = colorGreen;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Thunder ] > unitData.AttributeDefence[ (int)GameAttributeType.Thunder ] )
            thunder1.color = colorGreen;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Ice ] > unitData.AttributeDefence[ (int)GameAttributeType.Ice ] )
            ice1.color = colorGreen;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Illusion ] > unitData.AttributeDefence[ (int)GameAttributeType.Illusion ] )
            illusion1.color = colorGreen;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Light ] > unitData.AttributeDefence[ (int)GameAttributeType.Light ] )
            light1.color = colorGreen;
        if ( unitData1.AttributeDefence[ (int)GameAttributeType.Dark ] > unitData.AttributeDefence[ (int)GameAttributeType.Dark ] )
            dark1.color = colorGreen;

    }

}


