using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameItemUI : GameUI<GameItemUI>
{

    OnEventOver onEventOver;

    GameItemBagUI bagUI0;
    GameItemBagUI bagUI1;

    Text moneyText;
    Text itemText;

    Image Background1;
    Image Background2;

    float time = 0.0f;

    public int UserID
    {
        get
        {
            if ( bagUI0.Enabled )
            {
                if ( bagUI0.UserID == GameDefine.INVALID_ID )
                    return bagUI1.UserID;
                else
                    return bagUI0.UserID;
            }
            else
            {
                if ( bagUI1.UserID == GameDefine.INVALID_ID )
                    return bagUI0.UserID;
                else
                    return bagUI1.UserID;
            }
        }
    }


    public override void initSingleton()
    {
        Background1 = transform.Find( "Background1" ).GetComponent<Image>();
        Background2 = transform.Find( "Background2" ).GetComponent<Image>();

        bagUI0 = transform.Find( "bag0" ).GetComponent<GameItemBagUI>();
        bagUI1 = transform.Find( "bag1" ).GetComponent<GameItemBagUI>();

        moneyText = transform.Find( "money" ).GetComponent<Text>();
        itemText = transform.Find( "item" ).GetComponent<Text>();
    }

    void clear()
    {
    }

    public void updateText()
    {
        if ( bagUI0.Enabled )
        {
            itemText.text = bagUI0.getItemDes();
        }
        else
        {
            itemText.text = bagUI1.getItemDes();
        }

        moneyText.text = GameDefine.getBigInt( GameUserData.instance.Gold.ToString() );
    }

    public override void onUnShow()
    {
        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();

        clear();

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }


    public void moveRight()
    {
        if ( bagUI1.Enabled )
        {
            if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.Bag )
            {
                int userID = GameUserData.instance.nextUser( bagUI1.UserID );

                if ( userID == GameDefine.INVALID_ID )
                {
                    bagUI1.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI1.setUser( 0 );
                    bagUI1.setItems( GameUserData.instance.getUnitBase( 0 ).Items );
                }
                else
                {
                    bagUI1.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI1.setUser( userID );
                    bagUI1.setItems( GameUserData.instance.getUnitBase( userID ).Items );
                }
            }
            else
            {
                int userID = GameUserData.instance.nextUser( bagUI1.UserID );

                if ( bagUI0.UserID == userID )
                {
                    userID = GameUserData.instance.nextUser( userID );
                }

                if ( userID == GameDefine.INVALID_ID )
                {
                    bagUI1.setType( GameItemBagUI.GameItemBagUIType.Bag );
                    bagUI1.setItems( GameUserData.instance.ItemBag );
                }
                else
                {
                    bagUI1.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI1.setUser( userID );
                    bagUI1.setItems( GameUserData.instance.getUnitBase( userID ).Items );
                }
            }

            bagUI1.select( 0 );
        }
        else
        {
            int selection = bagUI1.Selection;

            bagUI0.enable( false );
            bagUI1.enable( true );
            bagUI1.select( selection );
        }

        updateText();
    }

    public void moveLeft()
    {
        if ( bagUI0.Enabled )
        {
            if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.Bag )
            {
                int userID = GameUserData.instance.nextUser( bagUI0.UserID );

                if ( userID == GameDefine.INVALID_ID )
                {
                    bagUI0.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI0.setUser( 0 );
                    bagUI0.setItems( GameUserData.instance.getUnitBase( 0 ).Items );
                }
                else
                {
                    bagUI0.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI0.setUser( userID );
                    bagUI0.setItems( GameUserData.instance.getUnitBase( userID ).Items );
                }
            }
            else
            {
                int userID = GameUserData.instance.nextUser( bagUI0.UserID );

                if ( bagUI1.UserID == userID )
                {
                    userID = GameUserData.instance.nextUser( userID );
                }

                if ( userID == GameDefine.INVALID_ID )
                {
                    bagUI0.setType( GameItemBagUI.GameItemBagUIType.Bag );
                    bagUI0.setItems( GameUserData.instance.ItemBag );
                }
                else
                {
                    bagUI0.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI0.setUser( userID );
                    bagUI0.setItems( GameUserData.instance.getUnitBase( userID ).Items );
                }
            }

            bagUI0.select( 0 );
        }
        else
        {
            int selection = bagUI0.Selection;

            bagUI1.enable( false );
            bagUI0.enable( true );
            bagUI0.select( selection );
        }

        updateText();
    }

    public void select( int n )
    {
        if ( bagUI0.Enabled )
        {
            bagUI0.select( n );
        }
        else
        {
            bagUI1.select( n );
        }

        time = 0.0f;

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();

        updateText();
    }
    

    public bool isShowAskUI()
    {
        if ( bagUI0.Enabled )
        {
            return bagUI0.IsShowAskUI;
        }
        else
        {
            return bagUI1.IsShowAskUI;
        }
    }

    public bool isShowItemAskUI()
    {
        if ( bagUI0.Enabled )
        {
            return bagUI0.IsShowItemAskUI;
        }
        else
        {
            return bagUI1.IsShowItemAskUI;
        }
    }


    public void onClickItemAskUI()
    {      
        int selection = GameDefine.INVALID_ID;
        GameItemBagUI.GameItemBagUIType uiType = GameItemBagUI.GameItemBagUIType.Count;
        int itemSlot = GameDefine.INVALID_ID;

        if ( bagUI0.Enabled )
        {
            if ( bagUI0.IsEnabledItemAskUI )
            {
                selection = bagUI0.SelectionItemAskUI;
                uiType = bagUI0.Type;
                itemSlot = bagUI0.getItemSlot();

                if ( uiType == GameItemBagUI.GameItemBagUIType.User )
                {
                    GameUnitBase unit0 = GameUserData.instance.getUnitBase( bagUI0.UserID );

                    switch ( selection )
                    {
                        case 0:
                            {
                                if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.Bag )
                                {
                                    short itemID = unit0.Items[ itemSlot ];
                                    unit0.removeItem( itemSlot );

                                    GameUserData.instance.addItem( itemID );
                                }
                                else
                                {
                                    GameUnitBase unit1 = GameUserData.instance.getUnitBase( bagUI1.UserID );
                                    short itemID = unit0.Items[ itemSlot ];
                                    unit0.removeItem( itemSlot );
                                    unit1.addItem( itemID );
                                }
                            }
                            break;
                        case 1:
                            {
                                if ( unit0.AccessorySlot == itemSlot )
                                {
                                    unit0.Accessory = GameDefine.INVALID_ID;
                                }
                                else
                                {
                                    short itemID = unit0.Items[ itemSlot ];
                                    GameItem item = GameItemData.instance.getData( unit0.Items[ itemSlot ] );

                                    switch ( item.ItemType )
                                    {
                                        case GameItemType.Weapon:
                                            unit0.Weapon = itemID;
                                            break;
                                        case GameItemType.Armor:
                                            unit0.Armor = itemID;
                                            break;
                                        case GameItemType.Accessory:
                                            unit0.Accessory = itemID;
                                            break;
                                    }
                                }
                            }
                            break;
                        case 2:
                            {
                                showAskUI( false );
                            }
                            break;
                    }
                }
                else
                {
                    switch ( selection )
                    {
                        case 0:
                            {
                                GameUnitBase unit1 = GameUserData.instance.getUnitBase( bagUI1.UserID );
                                short itemID = GameUserData.instance.ItemBag[ itemSlot ];
                                GameUserData.instance.removeItem( itemSlot );
                                unit1.addItem( itemID );
                            }
                            break;
                        case 2:
                            {
                                showAskUI( false );
                            }
                            break;
                    }
                }

                if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI0.setItems( GameUserData.instance.getUnitBase( bagUI0.UserID ).Items );
                }
                else
                {
                    bagUI0.setItems( GameUserData.instance.ItemBag );
                }
                if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI1.setItems( GameUserData.instance.getUnitBase( bagUI1.UserID ).Items );
                }
                else
                {
                    bagUI1.setItems( GameUserData.instance.ItemBag );
                }

                bagUI0.unshowItemAskUI();
                bagUI0.updateItems();
                bagUI1.updateItems();
                bagUI1.enable( false );

                bagUI0.select( bagUI0.Selection );
            }
        }
        else
        {
            if ( bagUI1.IsEnabledItemAskUI )
            {
                selection = bagUI1.SelectionItemAskUI;
                uiType = bagUI1.Type;
                itemSlot = bagUI1.getItemSlot();

                if ( uiType == GameItemBagUI.GameItemBagUIType.User )
                {
                    GameUnitBase unit1 = GameUserData.instance.getUnitBase( bagUI1.UserID );

                    switch ( selection )
                    {
                        case 0:
                            {
                                if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.Bag )
                                {
                                    short itemID = unit1.Items[ itemSlot ];
                                    unit1.removeItem( itemSlot );

                                    GameUserData.instance.addItem( itemID );
                                }
                                else
                                {
                                    GameUnitBase unit0 = GameUserData.instance.getUnitBase( bagUI0.UserID );
                                    short itemID = unit1.Items[ itemSlot ];
                                    unit1.removeItem( itemSlot );
                                    unit0.addItem( itemID );
                                }
                            }
                            break;
                        case 1:
                            {
                                if ( unit1.AccessorySlot == itemSlot )
                                {
                                    unit1.Accessory = GameDefine.INVALID_ID;
                                }
                                else
                                {
                                    short itemID = unit1.Items[ itemSlot ];
                                    GameItem item = GameItemData.instance.getData( unit1.Items[ itemSlot ] );

                                    switch ( item.ItemType )
                                    {
                                        case GameItemType.Weapon:
                                            unit1.Weapon = itemID;
                                            break;
                                        case GameItemType.Armor:
                                            unit1.Armor = itemID;
                                            break;
                                        case GameItemType.Accessory:
                                            unit1.Accessory = itemID;
                                            break;
                                    }

                                }
                            }
                            break;
                        case 2:
                            {
                                showAskUI( false );
                            }
                            break;
                    }
                }
                else
                {
                    switch ( selection )
                    {
                        case 0:
                            {
                                GameUnitBase unit0 = GameUserData.instance.getUnitBase( bagUI0.UserID );
                                short itemID = GameUserData.instance.ItemBag[ itemSlot ];
                                GameUserData.instance.removeItem( itemSlot );
                                unit0.addItem( itemID );
                            }
                            break;
                        case 2:
                            {
                                showAskUI( false );
                            }
                            break;
                    }
                }

                if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI0.setItems( GameUserData.instance.getUnitBase( bagUI0.UserID ).Items );
                }
                else
                {
                    bagUI0.setItems( GameUserData.instance.ItemBag );
                }
                if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI1.setItems( GameUserData.instance.getUnitBase( bagUI1.UserID ).Items );
                }
                else
                {
                    bagUI1.setItems( GameUserData.instance.ItemBag );
                }

                bagUI1.unshowItemAskUI();
                bagUI1.updateItems();
                bagUI0.updateItems();
                bagUI0.enable( false );

                bagUI1.select( bagUI1.Selection );
            }

        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public void onClickAskUI()
    {
        unShowAskUI();

        int selection = GameDefine.INVALID_ID;
        GameItemBagUI.GameItemBagUIType uiType = GameItemBagUI.GameItemBagUIType.Count;
        int itemSlot = GameDefine.INVALID_ID;

        if ( bagUI0.Enabled )
        {
            if ( bagUI0.IsEnabledItemAskUI )
            {
                selection = bagUI0.SelectionItemAskUI;
                uiType = bagUI0.Type;
                itemSlot = bagUI0.getItemSlot();

                if ( uiType == GameItemBagUI.GameItemBagUIType.User )
                {
                    GameUnitBase unit0 = GameUserData.instance.getUnitBase( bagUI0.UserID );
                    unit0.removeItem( itemSlot );
                }
                else
                {
                    GameUserData.instance.removeItem( itemSlot );
                }

                if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI0.setItems( GameUserData.instance.getUnitBase( bagUI0.UserID ).Items );
                }
                else
                {
                    bagUI0.setItems( GameUserData.instance.ItemBag );
                }
                if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI1.setItems( GameUserData.instance.getUnitBase( bagUI1.UserID ).Items );
                }
                else
                {
                    bagUI1.setItems( GameUserData.instance.ItemBag );
                }

                bagUI0.unshowItemAskUI();
                bagUI0.updateItems();
                bagUI1.updateItems();
                bagUI1.enable( false );

                bagUI0.select( bagUI0.Selection );
            }
        }
        else
        {
            if ( bagUI1.IsEnabledItemAskUI )
            {
                selection = bagUI1.SelectionItemAskUI;
                uiType = bagUI1.Type;
                itemSlot = bagUI1.getItemSlot();

                if ( uiType == GameItemBagUI.GameItemBagUIType.User )
                {
                    GameUnitBase unit1 = GameUserData.instance.getUnitBase( bagUI1.UserID );
                    unit1.removeItem( itemSlot );
                }
                else
                {
                    GameUserData.instance.removeItem( itemSlot );
                }

                if ( bagUI0.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI0.setItems( GameUserData.instance.getUnitBase( bagUI0.UserID ).Items );
                }
                else
                {
                    bagUI0.setItems( GameUserData.instance.ItemBag );
                }
                if ( bagUI1.Type == GameItemBagUI.GameItemBagUIType.User )
                {
                    bagUI1.setItems( GameUserData.instance.getUnitBase( bagUI1.UserID ).Items );
                }
                else
                {
                    bagUI1.setItems( GameUserData.instance.ItemBag );
                }

                bagUI1.unshowItemAskUI();
                bagUI1.updateItems();
                bagUI0.updateItems();
                bagUI0.enable( false );

                bagUI1.select( bagUI1.Selection );
            }

        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public void unShowItemAskUI()
    {
        if ( bagUI0.Enabled )
        {
            bagUI0.unshowItemAskUI();
        }
        else
        {
            bagUI1.unshowItemAskUI();
        }
    }

    public void selectItemAskUI( int n )
    {
        if ( bagUI0.Enabled )
        {
            bagUI0.selectItemAskUI( n );
        }
        else
        {
            bagUI1.selectItemAskUI( n );
        }
    }

    public int getSelectionItemAskUI()
    {
        if ( bagUI0.Enabled )
        {
            return bagUI0.SelectionItemAskUI;
        }
        else
        {
            return bagUI1.SelectionItemAskUI;
        }
    }

    public void showItemAskUI()
    {
        if ( bagUI0.Enabled )
        {
            if ( bagUI0.getItem() != null )
            {
                bagUI0.showItemAskUI( bagUI1 );
            }
        }
        else
        {
            if ( bagUI1.getItem() != null )
            {
                bagUI1.showItemAskUI( bagUI0 );
            }
        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public void unShowAskUI()
    {
        if ( bagUI0.Enabled )
        {
            bagUI0.unShowAskUI();
        }
        else
        {
            bagUI1.unShowAskUI();
        }
    }

    public void showAskUI( bool b )
    {
        if ( bagUI0.Enabled )
        {
            bagUI0.showAskUI( b );
        }
        else
        {
            bagUI1.showAskUI( b );
        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public bool isOKAskUI()
    {
        if ( bagUI0.Enabled )
        {
            return bagUI0.IsOKAskUI;
        }
        else
        {
            return bagUI1.IsOKAskUI;
        }
    }

    public int getSelection()
    {
        if ( bagUI0.Enabled )
        {
            return bagUI0.Selection;
        }
        else
        {
            return bagUI1.Selection;
        }
    }


    public void show( OnEventOver over )
    {
        onEventOver = over;

        show();
        
        bagUI0.setType( GameItemBagUI.GameItemBagUIType.Bag );
        bagUI0.setItems( GameUserData.instance.ItemBag );

        bagUI1.setType( GameItemBagUI.GameItemBagUIType.User );
        bagUI1.setUser( 0 );
        bagUI1.setItems( GameUserData.instance.getUnitBase( 0 ).Items );

        bagUI1.select( 0 );

        bagUI0.enable( true );
        bagUI1.enable( false );

        bagUI0.select( 0 );

        updateText();
    }



    protected override void onUpdate()
    {
        GameUnitBase unitBase = null;
        GameItem item = null;

        if ( bagUI0.Enabled )
        {
            item = bagUI0.getItem();

            if ( bagUI0.UserID != GameDefine.INVALID_ID )
                unitBase = GameUserData.instance.getUnitBase( bagUI0.UserID );
            else
                unitBase = GameUserData.instance.getUnitBase( bagUI1.UserID );
        }
        else
        {
            item = bagUI1.getItem();

            if ( bagUI1.UserID != GameDefine.INVALID_ID )
                unitBase = GameUserData.instance.getUnitBase( bagUI1.UserID );
            else
                unitBase = GameUserData.instance.getUnitBase( bagUI0.UserID );
        }

        if ( unitBase == null || item == null )
        {
            return;
        }

        if ( !item.canEquip( unitBase.UnitID ) )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > 1.0f )
        {
            time = -9999;
            if ( !GameEquipInfoUI.instance.IsShow )
            {
                GameEquipInfoUI.instance.showItem( unitBase , item );

                if ( bagUI0.Enabled )
                    GameEquipInfoUI.instance.setPos( 57.0f , -18.0f );
                else
                    GameEquipInfoUI.instance.setPos( -57.0f , -18.0f );

                GameEquipInfoUI.instance.show();
                GameEquipInfoUI.instance.showFade();
            }
        }

    }


}


