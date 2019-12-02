using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameAlchemyUIBag : MonoBehaviour
{
    public class Item
    {
        public GameAlchemyUIType type;
        public int userID;
        public int itemSlot;
        public short itemID;
    }

    List<Item> items = new List<Item>();

    public const int MAX_SLOT = 8;

    Image image;
    Text text;

    GameAlchemyUISlot[] slot = new GameAlchemyUISlot[ MAX_SLOT ];

    [SerializeField]
    GameAlchemyUIBagType type;


    [SerializeField]
    int selectionPos = 0;
    [SerializeField]
    int pos = GameDefine.INVALID_ID;
    [SerializeField]
    int selection = GameDefine.INVALID_ID;
    [SerializeField]
    int alchemySelection = 0;

    bool enabled1 = false;

    GameAskUI askUI;

    GameAnimation upAnimation;
    GameAnimation downAnimation;

    public GameAlchemyUIBagType Type { get { return type; } }
    public int Selection { get { return selection; } }
    public bool Enabled { get { return enabled1; } }


    public void setAlchemySelection( int n )
    {
        alchemySelection = n;
    }

    public int getItemSlot()
    {
        return slot[ pos ].ItemSlot;
    }

    public GameItem getItem()
    {
        return slot[ pos ].Item;
    }

    public Item removeItem()
    {
        if ( items[ selection ].type == GameAlchemyUIType.User )
        {
            GameUnitBase unit = GameUserData.instance.getUnitBase( items[ selection ].userID );
            unit.removeItem( items[ selection ].itemSlot );
        }
        else
        {
            GameUserData.instance.removeItem( items[ selection ].itemSlot );
        }

        return items[ selection ];
    }

    public bool getItemEnabled()
    {
        return slot[ pos ].Enabled;
    }

    public string getItemDes()
    {
        if ( slot[ pos ].Item != null )
        {
            return slot[ pos ].Item.Description;
        }

        return "";
    }

    void Awake()
    {
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            slot[ i ] = transform.Find( "slot" + i ).GetComponent<GameAlchemyUISlot>();
        }

        text = transform.Find( "text" ).GetComponent<Text>();

        image = GetComponent<Image>();

        askUI = GetComponentInChildren<GameAskUI>();

        upAnimation = transform.Find( "up" ).GetComponentInChildren<GameAnimation>();
        downAnimation = transform.Find( "down" ).GetComponentInChildren<GameAnimation>();
    }

    public void showAskUI( bool b )
    {
        if ( !slot[ pos ].Enabled ||
            slot[ pos ].Item == null )
        {
            return;
        }

        askUI.show( b );

        RectTransform trans = slot[ pos ].GetComponent<RectTransform>();
        askUI.setPos( trans.anchoredPosition.x + 80 , trans.anchoredPosition.y + 11 );
    }

    public void unShowAskUI()
    {
        askUI.unShow();
    }

    public bool IsShowAskUI { get { return askUI.IsShow; } }
    public bool IsOKAskUI { get { return askUI.IsOK; } }

    public void enable( bool b )
    {
        enabled1 = b;

//         if ( enabled1 )
//         {
//             image.color = new Color( 1.0f , 1.0f , 1.0f , 1.0f );
//         }
//         else
//         {
//             image.color = new Color( 1.0f , 1.0f , 1.0f , 1.0f );
//         }
// 
//         for ( int i = 0 ; i < MAX_SLOT ; i++ )
//         {
//             slot[ i ].selection( false );
//             slot[ i ].enable( enabled1 );
//         }

        updateUpDonw();
    }

    public void enableAll( bool b )
    {
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            slot[ i ].enable( b );
        }
    }

    public void clear()
    {
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            slot[ i ].clear();
        }
    }

    public void clearSelection()
    {
        pos = GameDefine.INVALID_ID;
        selection = GameDefine.INVALID_ID;
        selectionPos = 0;
    }

    public void setType( GameAlchemyUIBagType t )
    {
        switch ( t )
        {
            case GameAlchemyUIBagType.Bag:
                text.text = GameStringData.instance.getString( GameStringType.AlchemyUI0 );
                break;
            case GameAlchemyUIBagType.Alchemy:
                text.text = GameStringData.instance.getString( GameStringType.AlchemyUI1 );
                break;
        }
    }

    public void select( int n )
    {
        if ( n == 0 )
        {
            clearSelection();
        }

        if ( items.Count == 0 )
        {
            return;
        }

        bool up = selection > n;

        pos += ( up ? -1 : 1 ) * Mathf.Abs( selection - n );

        selection = n;

        if ( selection >= items.Count )
        {
            selection = items.Count - 1;
        }
        if ( selection < 0 )
        {
            selection = 0;
        }


        if ( pos < 0 )
        {
            selectionPos = selection;
            pos = 0;
        }

        if ( pos >= MAX_SLOT )
        {
            selectionPos = selection - MAX_SLOT + 1;
            pos = MAX_SLOT - 1;

            if ( selectionPos < 0 )
            {
                selectionPos = selection;
                pos = 0;
            }
        }

        
        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            slot[ i ].selection( false );
        }

        updateItems();

        if ( pos < 0 || pos >= MAX_SLOT )
        {
            return;
        }

        slot[ pos ].selection( true );
    }
    
    public void updateUpDonw()
    {
        upAnimation.stopAnimation();
        upAnimation.clearAnimation();

        downAnimation.stopAnimation();
        downAnimation.clearAnimation();

        if ( selectionPos > 0 )
        {
            if ( !enabled1 )
            {
                upAnimation.showFrame( 18 );
            }
            else
            {
                upAnimation.playAnimation( 18 , 22 );
            }
        }

        if ( selectionPos < items.Count - MAX_SLOT )
        {
            if ( !enabled1 )
            {
                downAnimation.showFrame( 22 );
            }
            else
            {
                downAnimation.playAnimation( 22 , 26 );
            }
        }
    }

    public void updateItems()
    {
        clear();

        updateUpDonw();

        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            int index = selectionPos + i;

            if ( items.Count > index )
            {
                GameItem item = GameItemData.instance.getData( items[ index ].itemID );
                
                if ( item != null )
                {
                    slot[ i ].setData( item , items[ index ].itemSlot , type , items[ index ].type , items[ index ].userID );

                    short id = canAlchemy( index );

                    slot[ i ].enable( id != GameDefine.INVALID_ID );

                    if ( id != GameDefine.INVALID_ID &&
                        type == GameAlchemyUIBagType.Alchemy )
                    {
                        item = GameItemData.instance.getData( id );
                        slot[ i ].setAlchemy( item );
                    }
                }
            }
        }
    }


    short canAlchemy( int n )
    {
        if ( type == GameAlchemyUIBagType.Bag )
        {
            for ( int i = 0 ; i < items.Count ; i++ )
            {
                if ( i == n )
                {
                    continue;
                }

                short id = GameUserData.instance.getAlchemyItem( items[ n ].itemID , items[ i ].itemID );

                if ( id != GameDefine.INVALID_ID )
                {
                    return id;
                }
            }

        }
        else
        {
            short id = GameUserData.instance.getAlchemyItem( items[ n ].itemID , items[ alchemySelection ].itemID );

            if ( id != GameDefine.INVALID_ID )
            {
                return id;
            }
        }

        return GameDefine.INVALID_ID;
    }

    public void addItems()
    {
        items.Clear();

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            GameUnitBase unit = GameUserData.instance.getUnitBase( i );

            if ( unit.InTeam == 0 )
            {
                continue;
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                if ( unit.Items[ j ] != GameDefine.INVALID_ID && 
                    j != unit.WeaponSlot && 
                    j != unit.ArmorSlot && 
                    j != unit.AccessorySlot )
                {
                    Item item = new Item();
                    item.userID = i;
                    item.type = GameAlchemyUIType.User;
                    item.itemSlot = j;
                    item.itemID = unit.Items[ j ];
                    items.Add( item );
                }
            }
        }

        for ( int j = 0 ; j < GameDefine.MAX_ITEMBAG ; j++ )
        {
            if ( GameUserData.instance.ItemBag[ j ] != GameDefine.INVALID_ID )
            {
                Item item = new Item();
                item.userID = GameDefine.INVALID_ID;
                item.type = GameAlchemyUIType.Bag;
                item.itemSlot = j;
                item.itemID = GameUserData.instance.ItemBag[ j ];

                items.Add( item );
            }
        }
    }

}

