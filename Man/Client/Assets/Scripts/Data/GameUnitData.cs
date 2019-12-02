using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public enum GameSpiritType
{
    Xun,
    Lie,
    Shen,
    Mo,
    Hun,

    Count
}

public enum GameUnitMoveType
{
    Invalid = -1,

    Walk0 = 0,
    Walk1,
    Walk2,
    Walk3,
    Fly4,
    Fly5,
    Fly6,
    Walk7,
    Fly8,
    Fly9,
    Walk10,

    None,

    Fly,

    Count
}

public enum GameUnitDefenceType
{
    Normal = 0,
    Invincible,

    Count
}

public enum GameUnitAttackType
{
    Normal,
    Ranged,

    Count
}

public enum GameAttributeType
{
    None = -1,
    Thunder,
    Fire,
    Ice,
    Illusion,
    Light,
    Dark,

    Cure,

    Count
}

public enum GameUnitCampType
{
    User = 0,
    Enemy = 1,

    Count
}


[System.Serializable]
public class GameUnit
{
    public short UnitID;

    public short AttributeDefenceID;
    public GameUnitMoveType MoveType;
    public GameUnitCampType UnitCampType;
    public int HP;
    public int HPGrow;
    public int MP;
    public int MPGrow;
    public short Move;
    public short Str;
    public short StrGrow;
    public short Int;
    public short IntGrow;
    public short Avg;
    public short AvgGrow;
    public short Vit;
    public short VitGrow;
    public short Luk;
    public short LukGrow;

    public short Sprite;
    public short unknow1;
    public GameUnitDefenceType DefenceType;
    public GameUnitAttackType AttackType;
    public short BaseLv;
    public short BaseExp;

    public string NameS;
    public string NameT;

    public string Name
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return NameS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return NameT;
            }
            return "";
        }
    }

}



public class GameUnitData : Singleton< GameUnitData >
{
    [SerializeField]
    GameUnit[] data;
    
    public GameUnit getData( int id )
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

        data = new GameUnit[ bytes.Length / 58 ];

        int index = 0;
        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameUnit unit = new GameUnit();

            unit.UnitID = (short)i;

            unit.AttributeDefenceID = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.MoveType = (GameUnitMoveType)BitConverter.ToInt16( bytes , index ); index += 2;
            unit.UnitCampType = (GameUnitCampType)BitConverter.ToInt16( bytes , index ); index += 2;
            unit.HP = BitConverter.ToUInt16( bytes , index ); index += 2;
            unit.HPGrow = BitConverter.ToUInt16( bytes , index ); index += 2;
            unit.MP = BitConverter.ToUInt16( bytes , index ); index += 2;
            unit.MPGrow = BitConverter.ToUInt16( bytes , index ); index += 2;
            unit.Move = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Str = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.StrGrow = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Int = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.IntGrow = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Avg = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.AvgGrow = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Vit = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.VitGrow = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.Luk = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.LukGrow = BitConverter.ToInt16( bytes , index ); index += 2;

            unit.Sprite = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.unknow1 = BitConverter.ToInt16( bytes , index ); index += 2;

            if ( unit.unknow1 != 2 )
            {
                UnityEngine.Debug.Log( "i " + i );
            }

            unit.DefenceType = (GameUnitDefenceType)BitConverter.ToInt16( bytes , index ); index += 2;
            unit.AttackType = (GameUnitAttackType)BitConverter.ToInt16( bytes , index ); index += 2;
            unit.BaseLv = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.BaseExp = BitConverter.ToInt16( bytes , index ); index += 2;

            unit.NameT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 10 ); index += 10;

            unit.NameS = ChineseStringUtility.ToSimplified( unit.NameT );

            data[ i ] = unit;
        }

        // fix hp grow bug.

        data[ 0 ].HPGrow = 4;
        data[ 1 ].HPGrow = 3;
        data[ 2 ].HPGrow = 4;
        data[ 3 ].HPGrow = 2;
        data[ 4 ].HPGrow = 5;
        data[ 5 ].HPGrow = 4;
        data[ 6 ].HPGrow = 3;
        data[ 7 ].HPGrow = 3;
        data[ 8 ].HPGrow = 3;
        data[ 9 ].HPGrow = 3;

        Debug.Log( "GameUnitData loaded." );
    }



}
