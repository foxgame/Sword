using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameItemBagUI : MonoBehaviour
{
    public enum GameItemBagUIType
    {
        Shop = 0,
        User,
        Bag,

        Count
    }


    public const int MAX_SLOT = 8;

    Image image;
    Text text;

    GameItemUISlot[] slot = new GameItemUISlot[ MAX_SLOT ];

    GameItemBagUIType type;

    short[] items = null;
    int itemCount = 0;

    int pos = 0;
    int selection = 0;
    int selectionPos = 0;
    int userID = 0;

    bool enabled1 = false;

    GameAskUI askUI;
    GameItemAskUI itemAskUI;

    GameAnimation upAnimation;
    GameAnimation downAnimation;

    public GameItemBagUIType Type { get { return type; } }
    public int UserID { get { return userID; } }
    public int Selection { get { return selection; } }
    public bool Enabled { get{ return enabled1; } }


    public int getItemSlot()
    {
        return selection;
    }

    public GameItem getItem()
    {
        if ( itemCount == 0 )
        {
            return null;
        }

        if ( pos < 0 || pos >= MAX_SLOT )
        {
            return null;
        }

        return slot[ pos ].Item;
    }

    public string getItemDes()
    {
        if ( itemCount == 0 )
        {
            return "";
        }

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
            slot[ i ] = transform.Find( "slot" + i ).GetComponent< GameItemUISlot >();
        }

        text = transform.Find( "text" ).GetComponent<Text>();

        image = GetComponent<Image>();

        askUI = GetComponentInChildren<GameAskUI>();

        itemAskUI = GetComponentInChildren<GameItemAskUI>();

        if ( itemAskUI != null )
        {
            itemAskUI.gameObject.SetActive( false );
        }

        upAnimation = transform.Find( "up" ).GetComponentInChildren<GameAnimation>();
        downAnimation = transform.Find( "down" ).GetComponentInChildren<GameAnimation>();
    }

    public void showItemAskUI( GameItemBagUI other )
    {
        GameItemAskUIType[] t = new GameItemAskUIType[ GameItemAskUI.MAX_SLOT ];
        bool[] e = new bool[ GameItemAskUI.MAX_SLOT ];

        if ( type == GameItemBagUIType.Bag )
        {
            t[ 0 ] = GameItemAskUIType.Give;
            t[ 1 ] = GameItemAskUIType.Equip;
            t[ 2 ] = GameItemAskUIType.Drop;

            e[ 0 ] = !other.itemFull();
            e[ 1 ] = false;
            e[ 2 ] = true;
        }
        else
        {
            GameUnitBase unit = GameUserData.instance.getUnitBase( userID );

            t[ 0 ] = GameItemAskUIType.Give;
            t[ 2 ] = GameItemAskUIType.Drop;

            e[ 0 ] = false;
            e[ 2 ] = false;

            if ( unit.WeaponSlot == selection )
            {
                t[ 1 ] = GameItemAskUIType.UnEquip;
                e[ 1 ] = false;
            }
            else if ( unit.ArmorSlot == selection )
            {
                t[ 1 ] = GameItemAskUIType.UnEquip;
                e[ 1 ] = false;
            }
            else if( unit.AccessorySlot == selection )
            {
                t[ 1 ] = GameItemAskUIType.UnEquip;
                e[ 1 ] = true;
            }
            else
            {
                t[ 1 ] = GameItemAskUIType.Equip;
                e[ 1 ] = getItem().canEquip( userID );

                e[ 0 ] = !other.itemFull();
                e[ 2 ] = true;
            }

        }

        itemAskUI.show( t , e );

        itemAskUI.setPos( 100.0f , slot[ pos ].GetComponent<RectTransform>().anchoredPosition.y );

        itemAskUI.select( 0 );
    }

    public void unshowItemAskUI()
    {
        itemAskUI.unShow();
    }

    public void selectItemAskUI( int n )
    {
        itemAskUI.select( n );
    }

    public bool IsEnabledItemAskUI { get { return itemAskUI.IsEnabled; } }
    public bool IsShowItemAskUI { get { return itemAskUI.IsShow; } }
    public int SelectionItemAskUI { get { return itemAskUI.Selection; } }

    public void showAskUI( bool b )
    {
        if ( pos < 0 || pos >= MAX_SLOT )
        {
            return;
        }

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

        if ( enabled1 )
        {
            image.color = new Color( 1.0f , 1.0f , 1.0f , 1.0f );
        }
        else
        {
            image.color = new Color( 1.0f , 1.0f , 1.0f , 1.0f );
        }

        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            slot[ i ].selection( false );
            slot[ i ].enable( enabled1 );
        }

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

    public void setUser( int u )
    {
        userID = u;

        if ( type == GameItemBagUIType.User )
        {
            GameUnit unit = GameUnitData.instance.getData( userID );
            text.text = unit.Name;
        }
    }

    public void setShopType( GameRPGShopType t )
    {
        switch ( t )
        {
            case GameRPGShopType.Weapon:
                text.text = GameStringData.instance.getString( GameStringType.Shop0 );
                break;
            case GameRPGShopType.Accessory:
                text.text = GameStringData.instance.getString( GameStringType.Shop1 );
                break;
            case GameRPGShopType.Other:
                text.text = GameStringData.instance.getString( GameStringType.Shop2 );
                break;
        }
    }

    public void setType( GameItemBagUIType t )
    {
        type = t;

        if ( t == GameItemBagUIType.Bag )
        {
            text.text = GameStringData.instance.getString( GameStringType.Bag0 );
            userID = GameDefine.INVALID_ID;
        }
    }

    public void select( int n )
    {
        if ( n == 0 )
        {
            clearSelection();
        }

        if ( itemCount == 0 )
        {
            return;
        }

        if ( selection != n )
        {
            pos += ( ( selection > n ) ? -1 : 1 ) * Mathf.Abs( selection - n );
        }

        selection = n;

        if ( selection >= itemCount )
        {
            selection = itemCount - 1;
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

        if ( pos >= itemCount - selectionPos )
        {
            pos = itemCount - selectionPos - 1;
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

    public bool itemFull()
    {
        switch ( type )
        {
            case GameItemBagUIType.User:
                {
                    GameUnitBase unit = GameUserData.instance.getUnitBase( userID );

                    return unit.itemFull();
                }
            case GameItemBagUIType.Bag:
                {
                    return GameUserData.instance.itemFull();
                }
        }

        return false;
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

        if ( selectionPos < itemCount - MAX_SLOT )
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

        GameUnitBase unit = null;

        if ( type == GameItemBagUIType.User )
        {
            unit = GameUserData.instance.getUnitBase( userID );
        }


        for ( int i = 0 ; i < MAX_SLOT ; i++ )
        {
            int index = selectionPos + i;

            if ( itemCount > index )
            {
                GameItem item = GameItemData.instance.getData( items[ index ] );

                if ( item != null )
                {
                    if ( item.ItemType == GameItemType.Item )
                    {
                        slot[ i ].setData( item , true , type == GameItemBagUIType.Shop );
                    }
                    else
                    {
                        slot[ i ].setData( item , item.canEquip( GameItemUI.instance.UserID ) , type == GameItemBagUIType.Shop );
                    }

                    slot[ i ].enable( true );

                    switch ( type )
                    {
                        case GameItemBagUIType.Shop:
                            {
                                slot[ i ].enable( item.Price <= GameUserData.instance.Gold );
                            }
                            break;
                        case GameItemBagUIType.User:
                            {
                                if ( unit.WeaponSlot == index )
                                {
                                    slot[ i ].setEquip( true );
                                    slot[ i ].enable( false );
                                }
                                if ( unit.ArmorSlot == index )
                                {
                                    slot[ i ].setEquip( true );
                                    slot[ i ].enable( false );
                                }
                                if ( unit.AccessorySlot == index )
                                {
                                    slot[ i ].setEquip( true );
                                    slot[ i ].enable( false );
                                }
                            }
                            break;
                        case GameItemBagUIType.Bag:
                            break;
                        case GameItemBagUIType.Count:
                            break;
                    }

                }
            }
        }
    }


    public void setItems( short[] i )
    {
        items = i;
        itemCount = 0;

        for ( int j = 0 ; j < items.Length ; j++ )
        {
            if ( items[ j ] != GameDefine.INVALID_ID )
            {
                itemCount++;
            }
        }

        updateItems();
    }

}


