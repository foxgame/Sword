using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUIItemSlot : MonoBehaviour
{
    Text text;

    Image[] icon = new Image[ (int)GameItemType.Count ];
    Image[] equip = new Image[ (int)GameItemType.Count - 1 ];

    void Awake()
    {
        text = transform.Find( "name" ).GetComponent<Text>();

        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ] = transform.Find( "icon" + i ).GetComponent<Image>();
        }

        for ( int i = 0 ; i < (int)GameItemType.Count - 1 ; i++ )
        {
            equip[ i ] = transform.Find( "equip" + i ).GetComponent<Image>();
        }
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

        text.text = "";
    }


    public void setData( GameItem item , bool b )
    {
        clear();

        if ( item == null )
        {
            return;
        }

        text.text = item.Name;

        icon[ (int)item.ItemType ].gameObject.SetActive( true );

        if ( b )
        {
            equip[ (int)item.ItemType - 1 ].gameObject.SetActive( b );
        }
    }



}

