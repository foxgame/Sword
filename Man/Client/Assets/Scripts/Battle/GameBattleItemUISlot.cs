using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleItemUISlot : MonoBehaviour
{
    Image select;
    Text text;
    Image[] icon = new Image[ (int)GameItemType.Count ];
    Image[] equip = new Image[ (int)GameItemType.Count - 1 ];

    bool enabled1;
    GameItem item;

    public bool Enabled { get { return enabled1; } }
    public GameItem Item { get { return item; } }

    void Awake()
    {
        select = transform.Find( "select" ).GetComponent<Image>();
        text = transform.Find( "text" ).GetComponent<Text>();

        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ] = transform.Find( "icon" + i ).GetComponent<Image>();
        }

        for ( int i = 0 ; i < (int)GameItemType.Count - 1 ; i++ )
        {
            equip[ i ] = transform.Find( "equip" + i ).GetComponent<Image>();
        }
    }

    public void selection( bool b )
    {
        select.gameObject.SetActive( b );
    }

    public void clear()
    {
        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ].gameObject.SetActive( false );
        }

        for ( int i = 0 ; i < (int)GameItemType.Count - 1 ; i++ )
        {
            equip[ i ].gameObject.SetActive( false );
        }

        select.gameObject.SetActive( false );

        text.text = "";
        item = null;
        enabled1 = false;
    }

    public void setEquip( bool b )
    {
        equip[ (int)item.ItemType - 1 ].gameObject.SetActive( b );      
    }

    public void setData( GameItem i )
    {
        clear();

        if ( i == null )
        {
            return;
        }

        item = i;

        text.text = i.Name;

        icon[ (int)item.ItemType ].gameObject.SetActive( true );
    }

    public void enable( bool b )
    {
        enabled1 = b;

        Color c0 = Color.white;
        Color c1 = new Color( 1.0f , 1.0f , 1.0f , 0.2f );

//        select.color = b ? c0 : c1;

        Color tc0 = text.color; tc0.a = 1.0f;
        Color tc1 = text.color; tc1.a = 0.2f;
        text.color = b ? tc0 : tc1;

        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ].color = b ? c0 : c1;
        }

        for ( int i = 0 ; i < (int)GameItemType.Count - 1 ; i++ )
        {
            equip[ i ].color = b ? c0 : c1;
        }
    }


}


