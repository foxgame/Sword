using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;



[System.Serializable]
public class GameUnitBase
{
    public short UnitID = GameDefine.INVALID_ID;

    public short LV;
    public short Exp;

    public int HP;
    public int MP;

    public short Str;
    public short Int;
    public short Avg;
    public short Vit;
    public short Luk;

    public short[] SpiritPower = new short[ (int)GameSpiritType.Count ];

    public short Weapon = GameDefine.INVALID_ID;
    public short Armor = GameDefine.INVALID_ID;
    public short Accessory = GameDefine.INVALID_ID;

    public short InTeam;
    public short BaseSpiritPower;

    public short[] Items = new short[ GameDefine.MAX_SLOT ];
    public short[] Skills = new short[ GameDefine.MAX_SLOT ];


    public int WeaponSlot
    {
        get
        {
            if ( Weapon != GameDefine.INVALID_ID )
            {
                for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
                {
                    if ( Items[ i ] == Weapon )
                    {
                        return i;
                    }
                }
            }

            return GameDefine.INVALID_ID;
        }
    }

    public int ArmorSlot
    {
        get
        {
            if ( Armor != GameDefine.INVALID_ID )
            {
                for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
                {
                    if ( Items[ i ] == Armor )
                    {
                        return i;
                    }
                }
            }

            return GameDefine.INVALID_ID;
        }
    }

    public int AccessorySlot
    {
        get
        {
            if ( Accessory != GameDefine.INVALID_ID )
            {
                for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
                {
                    if ( Items[ i ] == Accessory )
                    {
                        return i;
                    }
                }
            }

            return GameDefine.INVALID_ID;
        }
    }

    public void addSkill( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Skills[ i ] == GameDefine.INVALID_ID )
            {
                Skills[ i ] = id;
                return;
            }
        }
    }

    public bool hasSkill( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Skills[ i ] == id )
            {
                return true;
            }
        }

        return false;
    }

    public void addItem( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Items[ i ] == GameDefine.INVALID_ID )
            {
                Items[ i ] = id;
                return;
            }
        }
    }

    public void useItemID( int id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Items[ i ] == id )
            {
                useItem( i );
                return;
            }
        }
    }

    public void useItem( int slot )
    {
        GameItem item = GameItemData.instance.getData( Items[ slot ] );

        if ( item == null )
        {
            return;
        }

        if ( item.IsConsumable != 0 )
        {
            removeItem( slot );
        }
    }

    public void removeItem( int slot )
    {
        Items[ slot ] = GameDefine.INVALID_ID;

        for ( int i = slot ; i < GameDefine.MAX_SLOT - 1 ; i++ )
        {
            Items[ i ] = Items[ i + 1 ];
        }

        Items[ GameDefine.MAX_SLOT - 1 ] = GameDefine.INVALID_ID;
    }

    public void removeItemID( int id )
    {
        for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
        {
            if ( Items[ j ] == id )
            {
                Items[ j ] = GameDefine.INVALID_ID;

                for ( int i = j ; i < GameDefine.MAX_SLOT - 1 ; i++ )
                {
                    Items[ i ] = Items[ i + 1 ];
                }

                Items[ GameDefine.MAX_SLOT - 1 ] = GameDefine.INVALID_ID;

                if ( Weapon == id )
                    Weapon = GameDefine.INVALID_ID;

                if ( Armor == id )
                    Armor = GameDefine.INVALID_ID;

                if ( Accessory == id )
                    Accessory = GameDefine.INVALID_ID;

                return;
            }
        }

        
    }


    public void giveItem( int slot , GameUnitBase unit )
    {
        unit.addItem( Items[ slot ] );

        removeItem( slot );
    }

    public bool hasItem( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Items[ i ] == id )
            {
                return true;
            }
        }

        return false;
    }

    public bool itemFull()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Items[ i ] == GameDefine.INVALID_ID )
            {
                return false;
            }
        }

        return true;
    }


    public bool canAddItem()
    {
        for ( int i = 0 ; i < Items.Length ; i++ )
        {
            if ( Items[ i ] == GameDefine.INVALID_ID )
            {
                return true;
            }
        }

        return false;
    }
}


public class GameUnitInitData : Singleton< GameUnitInitData >
{
    [SerializeField]
    public GameUnitBase[] data;

    public GameUnitBase getData( int id )
    {
        if ( id < 0 || data.Length <= id )
        {
            return null;
        }

        return data[ id ];
    }

    public void load( string path )
    {
        FileStream fs = File.OpenRead( path );

        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );

        data = new GameUnitBase[ GameDefine.MAX_USER ];

        int index = 0;
        for ( int i = 0 ; i < GameDefine.MAX_USER ; ++i )
        {
            GameUnitBase unit = new GameUnitBase();

            unit.UnitID = BitConverter.ToInt16( bytes , index ); index += 2;

            unit.LV = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Exp = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.HP = BitConverter.ToUInt16( bytes , index ); index += 2;
            unit.MP = BitConverter.ToUInt16( bytes , index ); index += 2;

            unit.Str = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Int = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Avg = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Vit = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Luk = BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int k = 0 ; k < (int)GameSpiritType.Count ; k++ )
            {
                unit.SpiritPower[ k ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            unit.Weapon = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Armor = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Accessory = BitConverter.ToInt16( bytes , index ); index += 2;

            unit.InTeam = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.BaseSpiritPower = BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int k = 0 ; k < GameDefine.MAX_SLOT ; k++ )
            {
                unit.Items[ k ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }
            for ( int k = 0 ; k < GameDefine.MAX_SLOT ; k++ )
            {
                unit.Skills[ k ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            data[ i ] = unit;
        }


        Debug.Log( "GameUnitInitData loaded." );
    }



}
