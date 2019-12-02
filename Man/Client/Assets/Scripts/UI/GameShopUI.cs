using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameShopUI : GameUI<GameShopUI>
{
    GameAnimation gameAnimation;

    RectTransform transBackground;

    OnEventOver onEventOver;

    GameItemBagUI shopUI;
    GameItemBagUI bagUI;

    GameRPGShopInfo info;

    Text moneyText;
    Text itemText;

    Image Background1;
    Image Background2;

    float time;

    public override void initSingleton()
    {
        transBackground = transform.Find( "Background" ).GetComponent<RectTransform>();

        Background1 = transform.Find( "Background1" ).GetComponent<Image>();
        Background2 = transform.Find( "Background2" ).GetComponent<Image>();

        shopUI = transform.Find( "shop" ).GetComponent<GameItemBagUI>();
        bagUI = transform.Find( "bag" ).GetComponent<GameItemBagUI>();

        moneyText = transform.Find( "money" ).GetComponent<Text>();
        itemText = transform.Find( "item" ).GetComponent<Text>();
    }

    void clear()
    {
        GameDefine.DestroyAll( transBackground );
    }

    public void updateText()
    {
        if ( shopUI.Enabled )
        {
            itemText.text = shopUI.getItemDes();
        }
        else
        {
            itemText.text = bagUI.getItemDes();
        }

        moneyText.text = GameDefine.getBigInt( GameUserData.instance.Gold.ToString() );

        if ( bagUI.itemFull() )
        {
            shopUI.enableAll( false );
        }
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

        GameBlackUI.instance.unShowBlack( 1 , null );
    }


    public void moveRight()
    {
        if ( bagUI.Enabled )
        {
            if ( bagUI.Type == GameItemBagUI.GameItemBagUIType.User )
            {
                int userID = GameUserData.instance.nextUser( bagUI.UserID );

                if ( userID == GameDefine.INVALID_ID )
                {
                    bagUI.setType( GameItemBagUI.GameItemBagUIType.Bag );
                    bagUI.setItems( GameUserData.instance.ItemBag );
                }
                else
                {
                    bagUI.setType( GameItemBagUI.GameItemBagUIType.User );
                    bagUI.setUser( userID );
                    shopUI.setUser( userID );
                    bagUI.setItems( GameUserData.instance.getUnitBase( userID ).Items );
                }
            }
            else if ( bagUI.Type == GameItemBagUI.GameItemBagUIType.Bag )
            {
                bagUI.setType( GameItemBagUI.GameItemBagUIType.User );
                bagUI.setUser( 0 );
                shopUI.setUser( 0 );
                bagUI.setItems( GameUserData.instance.getUnitBase( 0 ).Items );
            }
        }
        else
        {
            shopUI.enable( false );
            bagUI.enable( true );
        }

        bagUI.select( 0 );

        updateText();
    }

    public void moveLeft()
    {
        bagUI.enable( false );
        shopUI.enable( true );
        shopUI.select( 0 );

        updateText();
    }

    public void select( int n )
    {
        if ( shopUI.Enabled )
        {
            shopUI.select( n );
        }
        else
        {
            bagUI.select( n );
        }

        time = 0.0f;

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();

        updateText();
    }

    public void onClickAskUI()
    {
        if ( shopUI.Enabled )
        {
            shopUI.unShowAskUI();

            if ( shopUI.IsOKAskUI )
            {
                // buy

                GameItem item = shopUI.getItem();

                if ( item == null )
                {
                    return;
                }

                switch ( bagUI.Type )
                {
                    case GameItemBagUI.GameItemBagUIType.User:
                        {
                            GameUnitBase unit = GameUserData.instance.getUnitBase( bagUI.UserID );

                            unit.addItem( item.ID );

                            GameUserData.instance.addGold( -item.Price );

                            bagUI.setItems( unit.Items );
                            bagUI.updateItems();
                            bagUI.enable( false );

                            shopUI.updateItems();
                            shopUI.select( shopUI.Selection );

                            updateText();
                        }
                        break;
                    case GameItemBagUI.GameItemBagUIType.Bag:
                        {
                            GameUserData.instance.addItem( item.ID );

                            GameUserData.instance.addGold( -item.Price );

                            bagUI.setItems( GameUserData.instance.ItemBag );
                            bagUI.updateItems();
                            bagUI.enable( false );

                            shopUI.updateItems();
                            shopUI.select( shopUI.Selection );

                            updateText();
                        }
                        break;
                }
            }
        }
        else
        {
            bagUI.unShowAskUI();

            if ( bagUI.IsOKAskUI )
            {
                // sell

                GameItem item = bagUI.getItem();
                int itemSlot = bagUI.getItemSlot();

                switch ( bagUI.Type )
                {
                    case GameItemBagUI.GameItemBagUIType.User:
                        {
                            GameUnitBase unit = GameUserData.instance.getUnitBase( bagUI.UserID );

                            unit.removeItem( itemSlot );

                            GameUserData.instance.addGold( item.Price / 2 );

                            bagUI.setItems( unit.Items );
                            bagUI.updateItems();
                            bagUI.select( bagUI.Selection - 1 );

                            updateText();
                        }
                        break;
                    case GameItemBagUI.GameItemBagUIType.Bag:
                        {
                            GameUserData.instance.removeItem( itemSlot );

                            GameUserData.instance.addGold( item.Price / 2 );

                            bagUI.setItems( GameUserData.instance.ItemBag );
                            bagUI.updateItems();
                            bagUI.select( bagUI.Selection - 1 );

                            updateText();
                        }
                        break;
                }

            }
        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public bool isShowAskUI()
    {
        if ( shopUI.Enabled )
        {
            return shopUI.IsShowAskUI;
        }
        else
        {
            return bagUI.IsShowAskUI;
        }
    }

    public void unShowAskUI()
    {
        if ( shopUI.Enabled )
        {
            shopUI.unShowAskUI();
        }
        else
        {
            bagUI.unShowAskUI();
        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public void showAskUI( bool b )
    {
        if ( shopUI.Enabled )
        {
            shopUI.showAskUI( b );
        }
        else
        {
            bagUI.showAskUI( b );
        }

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public bool isOKAskUI()
    {
        if ( shopUI.Enabled )
        {
            return shopUI.IsOKAskUI;
        }
        else
        {
            return bagUI.IsOKAskUI;
        }
    }

    public int getSelection()
    {
        if ( shopUI.Enabled )
        {
            return shopUI.Selection;
        }
        else
        {
            return bagUI.Selection;
        }
    }


    public void show( GameRPGShopInfo i , OnEventOver over )
    {
        info = i;

        onEventOver = over;

        show();

        shopUI.gameObject.SetActive( false );
        bagUI.gameObject.SetActive( false );
        moneyText.gameObject.SetActive( false );
        itemText.gameObject.SetActive( false );
        Background1.gameObject.SetActive( false );
        Background2.gameObject.SetActive( false );


        string path = "Prefab/Shop/Shop" + GameDefine.getString2( info.shop );

        GameObject gameOjbect = Resources.Load<GameObject>( path );
        GameObject obj = Instantiate( gameOjbect , transBackground );
        Transform trans = obj.transform;
        trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.UI = true;
        gameAnimation.offsetX = -(int)( GameCameraManager.instance.sceneWidthHalf - GameCameraManager.instance.xOffset );
        gameAnimation.offsetY = (int)( GameCameraManager.instance.sceneHeightHalf );
        gameAnimation.showFrame( 0 );

        obj = Instantiate( gameOjbect , transBackground );
        trans = obj.transform;
        trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.UI = true;
        gameAnimation.offsetX = -(int)( GameCameraManager.instance.sceneWidthHalf - GameCameraManager.instance.xOffset );
        gameAnimation.offsetY = (int)( GameCameraManager.instance.sceneHeightHalf );
        gameAnimation.playAnimation( 0 , gameAnimation.safHead.count3[ 0 ] , false , onShowOver );

        shopUI.setType( GameItemBagUI.GameItemBagUIType.Shop );
        shopUI.setShopType( info.type );

        bagUI.setType( GameItemBagUI.GameItemBagUIType.User );
        bagUI.setUser( 0 );
        shopUI.setUser( 0 );

        shopUI.setItems( info.item );

        bagUI.setItems( GameUserData.instance.getUnitBase( 0 ).Items );

        bagUI.select( 0 );

        shopUI.enable( true );
        bagUI.enable( false );

        shopUI.select( 0 );

        updateText();
    }

    void onShowOver()
    {
        gameAnimation.playAnimation( gameAnimation.safHead.count3[ 0 ] + 2 ,
            gameAnimation.safHead.count3[ 0 ] + gameAnimation.safHead.count3[ 1 ] + 2 );

        shopUI.gameObject.SetActive( true );
        bagUI.gameObject.SetActive( true );
        moneyText.gameObject.SetActive( true );
        itemText.gameObject.SetActive( true );
        Background1.gameObject.SetActive( true );
        Background2.gameObject.SetActive( true );

        showFade();
    }


    protected override void onUpdate()
    {
        if ( !shopUI.gameObject.activeSelf && 
            !bagUI.gameObject.activeSelf )
        {
            return;
        }

        GameUnitBase unitBase = null;
        GameItem item = null;

        if ( shopUI.Enabled )
        {
            item = shopUI.getItem();

            if ( shopUI.UserID != GameDefine.INVALID_ID )
                unitBase = GameUserData.instance.getUnitBase( shopUI.UserID );
            else
                unitBase = GameUserData.instance.getUnitBase( bagUI.UserID );
        }
        else
        {
            item = bagUI.getItem();

            if ( bagUI.UserID != GameDefine.INVALID_ID )
                unitBase = GameUserData.instance.getUnitBase( bagUI.UserID );
            else
                unitBase = GameUserData.instance.getUnitBase( shopUI.UserID );
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

                if ( shopUI.Enabled )
                    GameEquipInfoUI.instance.setPos( 57.0f , -18.0f );
                else
                    GameEquipInfoUI.instance.setPos( -57.0f , -18.0f );

                GameEquipInfoUI.instance.show();
                GameEquipInfoUI.instance.showFade();
            }
        }

    }



}

