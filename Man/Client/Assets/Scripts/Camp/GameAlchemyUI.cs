using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameAlchemyUI : GameUI<GameAlchemyUI>
{
    OnEventOver onEventOver;

    GameAlchemyUIBag bagUI0;
    GameAlchemyUIBag bagUI1;

    Text itemText;

    public override void initSingleton()
    {
        bagUI0 = transform.Find( "bag0" ).GetComponent<GameAlchemyUIBag>();
        bagUI1 = transform.Find( "bag1" ).GetComponent<GameAlchemyUIBag>();

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
    }

    public override void onUnShow()
    {
        clear();

        if ( onEventOver != null )
        {
            onEventOver();
        }
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

        updateText();
    }


    public bool isShowAskUI()
    {
        return bagUI1.IsShowAskUI;
    }

    public void onClickAskUI()
    {
        unShowAskUI();

        if ( bagUI1.IsOKAskUI )
        {
            GameItem item1 = bagUI0.getItem();
            GameItem item2 = bagUI1.getItem();

            short id = GameUserData.instance.getAlchemyItem( item1.ID , item2.ID );

            GameAlchemyUIBag.Item item = null;

            if ( bagUI0.Selection > bagUI1.Selection )
            {
                item = bagUI0.removeItem();
                bagUI1.removeItem();
            }
            else
            {
                bagUI1.removeItem();
                item = bagUI0.removeItem();
            }

            if ( item.type == GameAlchemyUIType.User )
            {
                GameUnitBase unit = GameUserData.instance.getUnitBase( item.userID );
                unit.addItem( id );
            }
            else
            {
                GameUserData.instance.addItem( id );
            }

            bagUI0.addItems();
            bagUI1.addItems();

            bagUI0.select( 0 );

            showAlchemy( false );
        }
        else
        {

        }
    }
    
    public void unShowAskUI()
    {
        bagUI1.unShowAskUI();
    }

    public void showAskUI( bool b )
    {
        bagUI1.showAskUI( b );
    }

    public bool isOKAskUI()
    {
        return bagUI1.IsOKAskUI;
    }

    public void onClick()
    {
        if ( bagUI0.Enabled )
        {
            if ( bagUI0.getItemEnabled() )
            {
                showAlchemy( true );
            }
        }
    }

    public void onClickAlchemy()
    {
        if ( bagUI1.Enabled )
        {
            bagUI1.showAskUI( true );
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

    public bool isEnabledAlchemy()
    {
        return bagUI1.Enabled;
    }

    public void showAlchemy( bool b )
    {
        if ( b )
        {
            bagUI1.enable( true );
            bagUI0.enable( false );

            bagUI1.setAlchemySelection( bagUI0.Selection );
            bagUI1.select( 0 );

            bagUI1.gameObject.SetActive( true );
        }
        else
        {
            bagUI1.enable( false );
            bagUI0.enable( true );

            bagUI1.gameObject.SetActive( false );
        }
    }

    public void show( OnEventOver over )
    {
        onEventOver = over;

        show();

        bagUI0.addItems();
        bagUI0.enable( true );
        bagUI0.select( 0 );

        bagUI1.addItems();
        bagUI1.enable( false );
        bagUI1.gameObject.SetActive( false );

        updateText();
    }


}


