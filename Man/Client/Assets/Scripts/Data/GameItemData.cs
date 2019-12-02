using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public enum GameItemType
{
    Item = 0,
    Weapon,
    Armor,
    Accessory,

    Count
}

public enum GameItemUseType
{
    None = -1,
    Recovery = 0,
    Cure,
    Limit,
    Skill,

    Count
}

public enum GameItemEffectType
{
    None = -1,

    HP = 0,
    MP = 1,

    Poison,
    Palsy,
    Silence,

    HPUp,
    MPUp,

    StrUp,
    IntUp,
    AvgUp,
    VitUp,
    LukUp,

    Count

}

public enum GameAttackAddType
{
    None = -1,
    AbsorbHP = 0,
    Poison,
    Palsy,
    Silence,
    Fetter,
    Clear,

    Count
}

[System.Serializable]
public class GameItem
{
    public short ID;

    public GameItemType ItemType;
    public short[] EquipUnit = new short[ 16 ];
    public GameAttackRangeType AttackRangeType;
    public short AttackRange;

    public short AttackRangeMin;
    public short AttackRangeMax;

    public short AttackHit;
    public short AttackMiss;
    public short AttackCritical;
    public short AttackDouble;

    public GameAttackAddType AttackAddType;
    public short AttackAddRatio;
    public GameAttributeType AttackAttributeType;
    public short AttackAddTime;
    public short AttackAddCost;
    public short Unknown3;

    public short Attack;
    public short Defence;
    public short Unknown4;

    public short[] AttributeDefence = new short[ (int)GameAttributeType.Count ];


    public short UseMPCost;
    public GameTargetType UseTargetType;
    public GameAttackRangeType UseRangeType;
    public short UseRange;
    public short UseRangeMin;
    public short UseRangeMax;
    public short Unknown5;
    public short UseBasePower;
    public short TimeCost;
    public short IsConsumable;

    public short Price;
    public GameItemUseType UseType;
    public GameItemEffectType UseEffectType;
    public short UseSkillID;


    public short SynthesizeItem1;
    public short SynthesizeItem2;

    public string NameS;
    public string DescriptionS;

    public string NameT;
    public string DescriptionT;


    public bool canEquip( int u )
    {
        if ( ItemType == GameItemType.Item )
        {
            return false;
        }

        for ( int i = 0 ; i < EquipUnit.Length ; i++ )
        {
            if ( EquipUnit[ i ] == u )
            {
                return true;
            }
        }

        return false;
    }

    public string Description
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return DescriptionS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return DescriptionT;
            }
            return "";
        }
    }

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


public class GameItemData : Singleton< GameItemData >
{
    [SerializeField]
    GameItem[] data;

    public GameItem[] Data { get { return data; } }

    public GameItem getData( int id )
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

        data = new GameItem[ bytes.Length / 182 ];

        int index = 0;
        for ( int i = 0 ; i < data.Length ; ++ i )
        {
            GameItem item = new GameItem();
            item.ID = (short)i;

            item.ItemType = (GameItemType)BitConverter.ToInt16( bytes , index ); index += 2;
            for ( int j = 0 ; j < 16 ; j++ )
            {
                item.EquipUnit[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            item.AttackRangeType = (GameAttackRangeType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackRange = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackRangeMin = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackRangeMax = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackHit = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackMiss = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackCritical = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackDouble = BitConverter.ToInt16( bytes , index ); index += 2;

            item.AttackAddType = (GameAttackAddType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackAddRatio = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackAttributeType = (GameAttributeType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackAddTime = BitConverter.ToInt16( bytes , index ); index += 2;
            item.AttackAddCost = BitConverter.ToInt16( bytes , index ); index += 2;
            item.Unknown3 = BitConverter.ToInt16( bytes , index ); index += 2;

            item.Attack = BitConverter.ToInt16( bytes , index ); index += 2;
            item.Defence = BitConverter.ToInt16( bytes , index ); index += 2;
            item.Unknown4 = BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int j = 0 ; j <= (int)GameAttributeType.Dark ; j++ )
            {
                item.AttributeDefence[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            item.UseMPCost = BitConverter.ToInt16( bytes , index ); index += 2;

            int tt = BitConverter.ToInt16( bytes , index ); index += 2;

            if ( tt > 0 )
            {
                tt--;
            }

            item.UseTargetType = (GameTargetType)tt;

            item.UseRangeType = (GameAttackRangeType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseRange = BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseRangeMin = BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseRangeMax = BitConverter.ToInt16( bytes , index ); index += 2;
            item.Unknown5 = BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseBasePower = BitConverter.ToInt16( bytes , index ); index += 2;
            item.TimeCost = BitConverter.ToInt16( bytes , index ); index += 2;
            item.IsConsumable = BitConverter.ToInt16( bytes , index ); index += 2;

            item.Price = BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseType = (GameItemUseType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseEffectType = (GameItemEffectType)BitConverter.ToInt16( bytes , index ); index += 2;
            item.UseSkillID = BitConverter.ToInt16( bytes , index ); index += 2;

            item.SynthesizeItem1 = BitConverter.ToInt16( bytes , index ); index += 2;
            item.SynthesizeItem2 = BitConverter.ToInt16( bytes , index ); index += 2;

            item.NameT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 10 ).Replace( " " , "" ); index += 10;
            item.DescriptionT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 60 ); index += 60;

            item.NameS = ChineseStringUtility.ToSimplified( item.NameT );
            item.DescriptionS = ChineseStringUtility.ToSimplified( item.DescriptionT );

            data[ i ] = item;


        }

        Debug.Log( "GameItemData loaded." );

    }

    public void saveText()
    {
        string strHelp = "<table interlaced=\"enabled\" align=\"center\" ><tbody>";

        strHelp += "<tr>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"100\" align=\"center\">";
        strHelp += "<strong>名称：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"200\" align=\"center\">";
        strHelp += "<strong>合成：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"300\" align=\"center\">";
        strHelp += "<strong>属性：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"220\" align=\"center\">";
        strHelp += "<strong>附加：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"80\" align=\"center\">";
        strHelp += "<strong>技能：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"150\" align=\"center\">";
        strHelp += "<strong>装备：</strong>";
        strHelp += "</td>";
        strHelp += "</tr>";

        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameItem item = data[ i ];

            strHelp += "<tr>";

            strHelp += "<td width=\"100\" align=\"center\">";
            strHelp += item.NameS;
            strHelp += "</td>";

            strHelp += "<td width=\"200\" align=\"center\">";

            if ( item.SynthesizeItem1 != GameDefine.INVALID_ID )
            {
                strHelp += data[ item.SynthesizeItem1 ].NameS + "+" + data[ item.SynthesizeItem2 ].NameS;
            }

            strHelp += "</td>";

            strHelp += "<td width=\"300\" align=\"center\">";

            if ( item.Attack > 0 )
                strHelp += "攻击+" + item.Attack + " ";
            if ( item.Attack < 0 )
                strHelp += "攻击" + item.Attack + " ";

            if ( item.Defence > 0 )
                strHelp += "防御+" + item.Defence + " ";
            if ( item.Defence < 0 )
                strHelp += "防御" + item.Defence + " ";

            if ( item.AttackHit > 0 )
                strHelp += "命中+" + item.AttackHit + " ";
            if ( item.AttackHit < 0 )
                strHelp += "命中" + item.AttackHit + " ";

            if ( item.AttackMiss > 0 )
                strHelp += "回避+" + item.AttackMiss + " ";
            if ( item.AttackMiss < 0 )
                strHelp += "回避" + item.AttackMiss + " ";

            if ( item.AttackCritical > 0 )
                strHelp += "会心+" + item.AttackCritical + " ";
            if ( item.AttackCritical < 0 )
                strHelp += "会心" + item.AttackCritical + " ";

            if ( item.AttackDouble > 0 )
                strHelp += "双击+" + item.AttackDouble + " ";
            if ( item.AttackDouble < 0 )
                strHelp += "双击" + item.AttackDouble + " ";


            switch ( item.UseEffectType )
            {
                case GameItemEffectType.HP:
                    strHelp += "恢复体力" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.MP:
                    strHelp += "恢复术力" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.Poison:
                    strHelp += "解除中毒" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.Palsy:
                    strHelp += "解除麻痹" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.Silence:
                    strHelp += "解除封魔" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.HPUp:
                    strHelp += "提升体力上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.MPUp:
                    strHelp += "提升术力上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.StrUp:
                    strHelp += "提升膂力上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.IntUp:
                    strHelp += "提升靈智上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.AvgUp:
                    strHelp += "提升行动力上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.VitUp:
                    strHelp += "提升筋力上限" + item.UseBasePower + " ";
                    break;
                case GameItemEffectType.LukUp:
                    strHelp += "提升运气上限" + item.UseBasePower + " ";
                    break;
            }

            strHelp += "</td>";

            strHelp += "<td width=\"220\" align=\"center\">";

            if ( item.AttackAddType != GameAttackAddType.None )
            {
                switch ( item.AttackAddType )
                {
                    case GameAttackAddType.AbsorbHP:
                        strHelp += "吸血 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        break;
                    case GameAttackAddType.Poison:
                        strHelp += "中毒 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        strHelp += "持续" + ( item.AttackAddTime >= 100 ? item.AttackAddTime / 100 : 1 ) + "回合";
                        break;
                    case GameAttackAddType.Palsy:
                        strHelp += "麻痹 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        strHelp += "持续" + ( item.AttackAddTime >= 100 ? item.AttackAddTime / 100 : 1 ) + "回合";
                        break;
                    case GameAttackAddType.Silence:
                        strHelp += "封魔 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        strHelp += "持续" + ( item.AttackAddTime >= 100 ? item.AttackAddTime / 100 : 1 ) + "回合";
                        break;
                    case GameAttackAddType.Fetter:
                        strHelp += "地缚 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        strHelp += "持续" + ( item.AttackAddTime >= 100 ? item.AttackAddTime / 100 : 1 ) + "回合";
                        break;
                    case GameAttackAddType.Clear:
                        strHelp += "净化 ";
                        strHelp += "几率" + item.AttackAddRatio + "% ";
                        break;
                }

            }

            strHelp += "</td>";

            strHelp += "<td width=\"80\" align=\"center\">";

            if ( item.UseSkillID != GameDefine.INVALID_ID )
            {
                strHelp += "" + GameSkillData.instance.getData( item.UseSkillID ).Name;
            }

            strHelp += "</td>";


            strHelp += "<td width=\"150\" align=\"center\">";

            int c = 0;
            for ( int j = 0 ; j < item.EquipUnit.Length ; j++ )
            {
                if ( item.EquipUnit[ j ] != GameDefine.INVALID_ID )
                    c++;
            }

            if ( c == 10 )
            {
                strHelp += "全";
            }
            else
            {
                for ( int j = 0 ; j < item.EquipUnit.Length ; j++ )
                {
                    if ( item.EquipUnit[ j ] == 0 )
                        strHelp += "殷 ";

                    if ( item.EquipUnit[ j ] == 1 )
                        strHelp += "封 ";

                    if ( item.EquipUnit[ j ] == 2 )
                        strHelp += "上官 ";

                    if ( item.EquipUnit[ j ] == 3 )
                        strHelp += "紫 ";

                    if ( item.EquipUnit[ j ] == 4 )
                        strHelp += "鲜 ";

                    if ( item.EquipUnit[ j ] == 5 )
                        strHelp += "真 ";

                    if ( item.EquipUnit[ j ] == 6 )
                        strHelp += "韩 ";

                    if ( item.EquipUnit[ j ] == 7 )
                        strHelp += "燕 ";

                    if ( item.EquipUnit[ j ] == 8 )
                        strHelp += "夏侯 ";

                    if ( item.EquipUnit[ j ] == 9 )
                        strHelp += "不净 ";
                }               
            }

            strHelp += "</td>";


            strHelp += "</tr>";
        }

        strHelp += "</tbody></table>";

        File.WriteAllText( Application.dataPath + "/Objects/Help/Help10.txt" , strHelp , Encoding.UTF8 );

    }


}
