using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleItemUI : GameUI<GameBattleItemUI>
{
    GameBattleUnit unit;

    GameBattleUnitActionItemMode mode;

    GameBattleItemUISlot[] slots = new GameBattleItemUISlot[ GameDefine.MAX_SLOT ];
    Text description;
    int selection = 0;

    GameAskUI askUI;

    public int Selection { get { return selection; } }

    public bool IsShowAskUI { get { return askUI.IsShow; } }

    public bool IsOKAskUI { get { return askUI.IsOK; } }

    public GameBattleUnitActionItemMode Mode { get { return mode; } }

    float time = 0.0f;

    public override void initSingleton()
    {
        description = transform.Find( "description/text" ).GetComponent<Text>();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ] = transform.Find( "slot" + i ).GetComponent<GameBattleItemUISlot>();
        }

        askUI = GetComponentInChildren<GameAskUI>();
    }

    public void showAskUI()
    {
        askUI.show( false );

        RectTransform trans = slots[ selection ].GetComponent<RectTransform>();
        askUI.setPos( trans.anchoredPosition.x + 80 , trans.anchoredPosition.y + 11 );

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public void unShowAskUI()
    {
        askUI.unShow();
    }

    public override void onUnShow()
    {
        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    public GameItem getSelectionItem()
    {
        if ( selection == GameDefine.INVALID_ID )
        {
            return null;
        }

        if ( !slots[ selection ].Enabled )
        {
            return null;
        }

        return slots[ selection ].Item;
    }

    public void select( int s )
    {
        if ( s < 0 || s >= GameDefine.MAX_SLOT )
        {
            return;
        }

        if ( askUI.IsShow )
        {
            askUI.show( !askUI.IsOK );
            return;
        }

        if ( slots[ s ].Item == null )
        {
            return;
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].selection( false );
        }

        selection = s;
        slots[ selection ].selection( true );

        description.text = slots[ s ].Item.Description;

        time = 0.0f;

        if ( GameEquipInfoUI.instance.IsShow )
            GameEquipInfoUI.instance.unShowFade();
    }

    void enableWeapon( bool b )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( unit.Weapon != null &&
                unit.Weapon.ID == unit.Items[ i ] )
            {
                slots[ i ].enable( b );
                break;
            }
        }
    }

    void enableArmor( bool b )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( unit.Armor != null &&
                unit.Armor.ID == unit.Items[ i ] )
            {
                slots[ i ].enable( b );
                break;
            }
        }
    }

    void enableAccessory( bool b )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( unit.Accessory != null &&
                unit.Accessory.ID == unit.Items[ i ] )
            {
                slots[ i ].enable( b );
                break;
            }
        }
    }

    public void setData( GameBattleUnit u , GameBattleUnitActionItemMode m )
    {
        unit = u;
        mode = m;
        selection = 0;

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].clear();

            if ( unit.Items[ i ] == GameDefine.INVALID_ID )
            {
                continue;
            }

            GameItem item = GameItemData.instance.getData( unit.Items[ i ] );

            slots[ i ].setData( item );
            slots[ i ].enable( true );

            switch ( m )
            {
                case GameBattleUnitActionItemMode.Use:
                    {
                        if ( !u.canUseItem( item ) )
                        {
                            slots[ i ].enable( false );
                        }
                    }
                    break;
                case GameBattleUnitActionItemMode.Give:
                    break;
                case GameBattleUnitActionItemMode.Equip:
                    {
                        if ( !u.canEquipItem( item ) )
                        {
                            slots[ i ].enable( false );
                        }
                    }
                    break;
                case GameBattleUnitActionItemMode.Drop:
                    break;
            }
        }

        switch ( m )
        {
            case GameBattleUnitActionItemMode.Give:
            case GameBattleUnitActionItemMode.Drop:
                {
                    enableWeapon( false );
                    enableArmor( false );
                    enableAccessory( false );
                }
                break;
            case GameBattleUnitActionItemMode.Equip:
                {
                    enableWeapon( false );
                    enableArmor( false );
                }
                break;
        }

        int WeaponSlot = unit.WeaponSlot;
        int ArmorSlot = unit.ArmorSlot;
        int AccessorySlot = unit.AccessorySlot;

        if ( WeaponSlot != GameDefine.INVALID_ID )
        {
            slots[ WeaponSlot ].setEquip( true );
        }
        if ( ArmorSlot != GameDefine.INVALID_ID )
        {
            slots[ ArmorSlot ].setEquip( true );
        }
        if ( AccessorySlot != GameDefine.INVALID_ID )
        {
            slots[ AccessorySlot ].setEquip( true );
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( slots[ i ].Enabled )
            {
                select( i );
                return;
            }
        }

        select( selection );
    }

    protected override void onUpdate()
    {
        if ( mode != GameBattleUnitActionItemMode.Equip )
        {
            return;
        }

        if ( slots[ selection ].Item == null )
        {
            return;
        }

        if ( !slots[ selection ].Enabled )
        {
            return;
        }

        if ( slots[ selection ].Item.ItemType == GameItemType.Item )
        {
            return;
        }

        time += Time.deltaTime;
        
        if ( time > 1.0f )
        {
            time = -9999;
            if ( !GameEquipInfoUI.instance.IsShow )
            {
                GameEquipInfoUI.instance.showItem( unit.getUnitBase() , slots[ selection ].Item );

                RectTransform trans = slots[ selection ].GetComponent<RectTransform>();

                if ( selection > 4 )
                    GameEquipInfoUI.instance.setPos( trans.anchoredPosition.x - 37.0f , trans.anchoredPosition.y - 71.0f );
                else
                    GameEquipInfoUI.instance.setPos( trans.anchoredPosition.x + 27.0f , trans.anchoredPosition.y - 71.0f );

                GameEquipInfoUI.instance.show();
                GameEquipInfoUI.instance.showFade();
            }
        }

    }

}

