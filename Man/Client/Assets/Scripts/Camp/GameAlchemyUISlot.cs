using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameAlchemyUIType
{
    User = 0,
    Bag,

    Count
}

public enum GameAlchemyUIBagType
{
    Bag = 0,
    Alchemy,

    Count
}

public class GameAlchemyUISlot : MonoBehaviour
{
    GameAlchemyUIBagType bagType;
    GameAlchemyUIType type;
    int userID;

    Image select;
    Text text;
    Text userText;

    Image image;
    Text alchemyText;

    Image[] icon = new Image[ (int)GameItemType.Count ];

    bool enabled1;
    GameItem item;

    int itemSlot;

    public int ItemSlot { get { return itemSlot; } }
    public bool Enabled { get { return enabled1; } }
    public GameItem Item { get { return item; } }

    public GameAlchemyUIBagType BagType { get { return bagType; } }
    public GameAlchemyUIType Type { get { return type; } }
    public int UserID { get { return userID; } }
    


    void Awake()
    {
        select = transform.Find( "select" ).GetComponent<Image>();

        text = transform.Find( "text" ).GetComponent<Text>();
        userText = transform.Find( "userText" ).GetComponent<Text>();

        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ] = transform.Find( "icon" + i ).GetComponent<Image>();
        }

        if ( transform.Find( "image" ) != null )
        {
            image = transform.Find( "image" ).GetComponent<Image>();
            alchemyText = transform.Find( "alchemyText" ).GetComponent<Text>();
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

        select.gameObject.SetActive( false );

        userText.text = "";
        text.text = "";
        item = null;

        if ( image != null )
        {
            image.gameObject.SetActive( false );
            alchemyText.text = "";
        }
    }

    public void setAlchemy( GameItem i )
    {
        image.gameObject.SetActive( true );
        alchemyText.text = i.Name;
    }
    
    public void setData( GameItem i , int slot , GameAlchemyUIBagType bt , GameAlchemyUIType t , int uid )
    {
        itemSlot = slot;
        bagType = bt;
        type = t;
        userID = uid;

        clear();

        if ( i == null )
        {
            return;
        }

        item = i;

        text.text = i.Name;

        if ( type == GameAlchemyUIType.Bag )
        {
            userText.text = GameStringData.instance.getString( GameStringType.Bag0 );
        }
        else
        {
            GameUnit unit = GameUnitData.instance.getData( userID );

            userText.text = unit.Name;
        }

        icon[ (int)item.ItemType ].gameObject.SetActive( true );
    }

    public void enable( bool b )
    {
        if ( item == null )
        {
            return;
        }

        enabled1 = b;

        Color c0 = Color.white;
        Color c1 = new Color( 1.0f , 1.0f , 1.0f , 0.2f );

        //        select.color = b ? c0 : c1;

        Color tc0 = text.color; tc0.a = 1.0f;
        Color tc1 = text.color; tc1.a = 0.2f;
        text.color = b ? tc0 : tc1;
        userText.color = b ? tc0 : tc1;

        for ( int i = 0 ; i < (int)GameItemType.Count ; i++ )
        {
            icon[ i ].color = b ? c0 : c1;
        }
    }

}
