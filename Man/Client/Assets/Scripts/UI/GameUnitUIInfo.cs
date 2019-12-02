using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUIInfo : MonoBehaviour
{
    RectTransform[] red = new RectTransform[ (int)GameAttributeType.Cure ];
    RectTransform[] blue = new RectTransform[ (int)GameAttributeType.Cure ];
    Text[] textTypes = new Text[ (int)GameAttributeType.Cure ];

    Text atkText;
    Text defText;
    Text magText;
    Text spdText;

    float width;

    void Awake()
    {
        for ( int i = 0 ; i < (int)GameAttributeType.Cure ; i++ )
        {
            red[ i ] = transform.Find( "type" + i + "/red" ).GetComponent<RectTransform>();
            blue[ i ] = transform.Find( "type" + i + "/blue" ).GetComponent<RectTransform>();
            textTypes[ i ] = transform.Find( "type" + i + "Text" ).GetComponent<Text>();
        }

        atkText = transform.Find( "atk" ).GetComponent<Text>();
        defText = transform.Find( "def" ).GetComponent<Text>();
        magText = transform.Find( "mag" ).GetComponent<Text>();
        spdText = transform.Find( "spd" ).GetComponent<Text>();

        width = red[ 0 ].sizeDelta.x;
    }

    public void clear()
    {
        
    }

    public void updateData( GameBattleUnit battleUnit )
    {
        clear();

        GameUnit gameUnit = GameUnitData.instance.getData( battleUnit.UnitID );

        atkText.text = GameDefine.getBigInt( battleUnit.PhysicalAttack.ToString() );
        defText.text = GameDefine.getBigInt( battleUnit.Defence.ToString() );
        magText.text = GameDefine.getBigInt( battleUnit.MagicAttack.ToString() );
        spdText.text = GameDefine.getBigInt( battleUnit.Speed.ToString() );

        for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
        {
            int ad = battleUnit.AttributeDefence[ i ];

            if ( ad > 100 )
                ad = 100;

            if ( ad < -100 )
                ad = -100;

            if ( ad > 0 )
            {
                red[ i ].gameObject.SetActive( false );
                blue[ i ].gameObject.SetActive( true );

                red[ i ].anchoredPosition = new Vector2( -1.5f - width , 0.0f );
                blue[ i ].anchoredPosition = new Vector2( -1.5f - width * ( 1.0f - ad / 100.0f ) , 0.0f );
            }
            else
            {
                red[ i ].gameObject.SetActive( true );
                blue[ i ].gameObject.SetActive( false );

                blue[ i ].anchoredPosition = new Vector2( -1.5f + width , 0.0f );
                red[ i ].anchoredPosition = new Vector2( -1.5f - width * ( 1.0f + ad / 100.0f ) , 0.0f );
            }

            textTypes[ i ].text = GameDefine.getBigInt( Mathf.Abs( battleUnit.AttributeDefence[ i ] ).ToString() );
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

        short[] AttributeDefence = new short[ (int)GameAttributeType.Cure ];

        GameAttributeDefence md = GameAttributeDefenceData.instance.getData( gameUnit.AttributeDefenceID );
        for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
        {
            if ( md != null )
                AttributeDefence[ i ] = md.AttributeDefence[ i ];
            else
                AttributeDefence[ i ] = 100;
        }

        int atk = 0;
        int def = 0;
        int mag = Int;
        int spd = Avg;

        GameItem weapon = GameItemData.instance.getData( unitBase.Weapon );
        GameItem armor = GameItemData.instance.getData( unitBase.Armor );
        GameItem accessory = GameItemData.instance.getData( unitBase.Accessory );

        if ( weapon != null )
        {
            atk += weapon.Attack;
            def += weapon.Defence;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                AttributeDefence[ i ] -= (short)( weapon.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( armor != null )
        {
            atk += armor.Attack;
            def += armor.Defence;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                AttributeDefence[ i ] -= (short)( armor.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( accessory != null )
        {
            atk += accessory.Attack;
            def += accessory.Defence;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                AttributeDefence[ i ] -= (short)( accessory.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }

        atk += Str;
        def += (short)( Vit / 2.0f );

        atkText.text = GameDefine.getBigInt( atk.ToString() );
        defText.text = GameDefine.getBigInt( def.ToString() );
        magText.text = GameDefine.getBigInt( mag.ToString() );
        spdText.text = GameDefine.getBigInt( spd.ToString() );

        for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
        {
            int ad = AttributeDefence[ i ];

            if ( ad > 100 )
                ad = 100;

            if ( ad < -100 )
                ad = -100;

            if ( ad > 0 )
            {
                red[ i ].gameObject.SetActive( false );
                blue[ i ].gameObject.SetActive( true );

                red[ i ].anchoredPosition = new Vector2( -1.5f - width , 0.0f );
                blue[ i ].anchoredPosition = new Vector2( -1.5f - width * ( 1.0f - ad / 100.0f ) , 0.0f );
            }
            else
            {
                red[ i ].gameObject.SetActive( true );
                blue[ i ].gameObject.SetActive( false );

                blue[ i ].anchoredPosition = new Vector2( -1.5f + width , 0.0f );
                red[ i ].anchoredPosition = new Vector2( -1.5f - width * ( 1.0f + ad / 100.0f ) , 0.0f );
            }

            textTypes[ i ].text = GameDefine.getBigInt( Mathf.Abs( AttributeDefence[ i ] ).ToString() );
        }


    }



}

