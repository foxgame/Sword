using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


[System.Serializable]
public class GameUnitLevelUpSkillLearn
{
    public short LV;
    public short[] SpiritRequest = new short[ (int)GameSpiritType.Count ];
    public short SkillID;
}

[System.Serializable]
public class GameUnitLevelUp
{
    public short ID;

    public int HP;
    public int MP;
    public short StrBase;
    public short StrRand;
    public short IntBase;
    public short IntRand;
    public short AvgBase;
    public short AvgRand;
    public short VitBase;
    public short VitRand;
    public short LukBase;
    public short LukRand;

    public GameUnitLevelUpSkillLearn[] Skill = new GameUnitLevelUpSkillLearn[ GameDefine.MAX_SLOT ];
}



public class GameUnitLevelUpData : Singleton<GameUnitLevelUpData>
{
    [SerializeField]
    GameUnitLevelUp[] data;

    GameUnitLevelUp lastData;

    public GameUnitLevelUp LastData { get{ return lastData; } }

    public GameUnitLevelUp getData( int id )
    {
        if ( id < 0 || data.Length <= id )
        {
            return null;
        }

        return data[ id ];
    }

    
    public void levelUp( GameBattleUnit unit )
    {
        GameUnit gameUnit = unit.getGameUnit();
        GameUnitBase unitBase = unit.getUnitBase();

        GameUnitLevelUp d = getData( unitBase.UnitID );

        if ( d == null )
        {
            return;
        }

        if ( unitBase.Exp < GameDefine.MAX_EXP )
        {
            return;
        }

        unitBase.Exp -= GameDefine.MAX_EXP;
        unitBase.LV++;

        if ( unitBase.LV == GameDefine.MAX_LEVEL )
        {
            unitBase.Exp = 0;
        }

        lastData = new GameUnitLevelUp();

        lastData.HP = gameUnit.HPGrow;
        lastData.MP = gameUnit.MPGrow;

        lastData.StrBase = d.StrBase;
        lastData.StrRand = (short)UnityEngine.Random.Range( 1 , d.StrRand );
        lastData.VitBase = d.VitBase;
        lastData.VitRand = (short)UnityEngine.Random.Range( 1 , d.VitRand );
        lastData.IntBase = d.IntBase;
        lastData.IntRand = (short)UnityEngine.Random.Range( 1 , d.IntRand );
        lastData.AvgBase = d.AvgBase;
        lastData.AvgRand = (short)UnityEngine.Random.Range( 1 , d.AvgRand );
        lastData.LukBase = d.LukBase;
        lastData.LukRand = (short)UnityEngine.Random.Range( 1 , d.LukRand );

        lastData.HP += (short)( unitBase.Vit / 35.0f + UnityEngine.Random.Range( 1 , d.HP + 1 ) );
        lastData.MP += (short)( unitBase.Int / 35.0f + UnityEngine.Random.Range( 1 , d.MP + 1 ) );

        unitBase.HP += lastData.HP;
        unitBase.MP += lastData.MP;
        unitBase.Str += lastData.StrBase;
        unitBase.Str += lastData.StrRand;
        unitBase.Vit += lastData.VitBase;
        unitBase.Vit += lastData.VitRand;
        unitBase.Int += lastData.IntBase;
        unitBase.Int += lastData.IntRand;
        unitBase.Avg += lastData.AvgBase;
        unitBase.Avg += lastData.AvgRand;
        unitBase.Luk += lastData.LukBase;
        unitBase.Luk += lastData.LukRand;

        unitBase.BaseSpiritPower += 2;

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( d.Skill[ i ].SkillID == GameDefine.INVALID_ID )
            {
                continue;
            }

            if ( unitBase.hasSkill( d.Skill[ i ].SkillID ) )
            {
                continue;
            }

            if ( unit.SpiritPower[ 0 ] < d.Skill[ i ].SpiritRequest[ 0 ] ||
                unit.SpiritPower[ 1 ] < d.Skill[ i ].SpiritRequest[ 1 ] ||
                unit.SpiritPower[ 2 ] < d.Skill[ i ].SpiritRequest[ 2 ] ||
                unit.SpiritPower[ 3 ] < d.Skill[ i ].SpiritRequest[ 3 ] ||
                unit.SpiritPower[ 4 ] < d.Skill[ i ].SpiritRequest[ 4 ] )
            {
                continue;
            }

            if ( unit.LV < d.Skill[ i ].LV )
            {
                continue;
            }

            unitBase.addSkill( d.Skill[ i ].SkillID );

            lastData.Skill[ 0 ] = new GameUnitLevelUpSkillLearn();
            lastData.Skill[ 0 ].SkillID = d.Skill[ i ].SkillID;
        }

        unit.updateUnitData();
    }

    public void load( string path )
    {
        FileStream fs = File.OpenRead( path );

        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );

        data = new GameUnitLevelUp[ GameDefine.MAX_USER ];

        int[] id1 = new int[ GameDefine.MAX_USER ];

        int index = 0;
        for ( int i = 0 ; i < GameDefine.MAX_USER ; ++i )
        {
            GameUnitLevelUp unit = new GameUnitLevelUp();

            unit.ID = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.HP = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.MP = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.StrBase = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.StrRand = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.IntBase = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.IntRand = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.AvgBase = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.AvgRand = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.VitBase = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.VitRand = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.LukBase = BitConverter.ToInt16( bytes , index ); index += 2;
            unit.LukRand = BitConverter.ToInt16( bytes , index ); index += 2;

            data[ unit.ID ] = unit;
            id1[ i ] = unit.ID;
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; ++i )
        {
            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                GameUnitLevelUpSkillLearn unit = new GameUnitLevelUpSkillLearn();

                unit.LV = BitConverter.ToInt16( bytes , index ); index += 2;
                for ( int k = 0 ; k < (int)GameSpiritType.Count ; k++ )
                {
                    unit.SpiritRequest[ k ] = BitConverter.ToInt16( bytes , index ); index += 2;
                }
                unit.SkillID = BitConverter.ToInt16( bytes , index ); index += 2;

                data[ id1[ i ] ].Skill[ j ] = unit;
            }
            
        }


        Debug.Log( "GameUnitLevelUpData loaded." );
    }


    public void saveText()
    {
        string strHelp = "";

        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameUnitLevelUp unit = data[ i ];

            strHelp += "<table interlaced=\"enabled\" align=\"center\" ><tbody>";

            strHelp += "<tr>";
            strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"100\" align=\"center\">";
            strHelp += "<strong>技能名称：</strong>";
            strHelp += "</td>";
            strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"60\" align=\"center\">";
            strHelp += "<strong>等级：</strong>";
            strHelp += "</td>";
            strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"200\" align=\"center\">";
            strHelp += "<strong>需求：</strong>";
            strHelp += "</td>";
            strHelp += "</tr>";


            for ( int j = 0 ; j < unit.Skill.Length ; j++ )
            {
                GameUnitLevelUpSkillLearn learn = unit.Skill[ j ];

                if ( learn.SkillID == GameDefine.INVALID_ID )
                {
                    continue;
                }

                strHelp += "<tr>";

                strHelp += "<td width=\"100\" align=\"center\">";
                strHelp += GameSkillData.instance.getData( learn.SkillID ).NameS;
                strHelp += "</td>";

                strHelp += "<td width=\"60\" align=\"center\">";
                strHelp += learn.LV;
                strHelp += "</td>";


                strHelp += "<td width=\"200\" align=\"center\">";
                if ( learn.SpiritRequest[ 0 ] > 0 )
                    strHelp += "迅" + learn.SpiritRequest[ 0 ];
                if ( learn.SpiritRequest[ 1 ] > 0 )
                    strHelp += "烈" + learn.SpiritRequest[ 1 ];
                if ( learn.SpiritRequest[ 2 ] > 0 )
                    strHelp += "神" + learn.SpiritRequest[ 2 ];
                if ( learn.SpiritRequest[ 3 ] > 0 )
                    strHelp += "魔" + learn.SpiritRequest[ 3 ];
                if ( learn.SpiritRequest[ 4 ] > 0 )
                    strHelp += "魂" + learn.SpiritRequest[ 4 ];
                strHelp += "</td>";

                strHelp += "</tr>";

            }


            strHelp += "</tbody></table>";
        }


        File.WriteAllText( Application.dataPath + "/Objects/Help/Help30.txt" , strHelp , Encoding.UTF8 );

    }

}
