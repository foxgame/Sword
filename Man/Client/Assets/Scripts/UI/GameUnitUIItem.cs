using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUIItem : MonoBehaviour
{
    GameUnitUIItemSlot[] slot = new GameUnitUIItemSlot[ GameDefine.MAX_SLOT ];


    void Awake()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slot[ i ] = transform.Find( "slot" + i ).GetComponent<GameUnitUIItemSlot>();
        }
    }


    public void clear()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slot[ i ].clear();
        }
    }

    public void updateData( GameBattleUnit battleUnit )
    {
        clear();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            GameItem item = GameItemData.instance.getData( battleUnit.Items[ i ] );

            bool b = ( battleUnit.AccessorySlot == i || battleUnit.ArmorSlot == i || battleUnit.WeaponSlot == i );

            slot[ i ].setData( item , b );
        }
    }

    public void updateData( GameUnitBase unitBase )
    {
        clear();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            GameItem item = GameItemData.instance.getData( unitBase.Items[ i ] );

            bool b = ( unitBase.AccessorySlot == i || unitBase.ArmorSlot == i || unitBase.WeaponSlot == i );

            slot[ i ].setData( item , b );
        }
    }

}

