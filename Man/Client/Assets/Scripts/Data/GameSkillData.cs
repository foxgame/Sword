using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public enum GameSkillType
{
    Magic,
    Physical,

    Count
}


public enum GameSkillMoveAttackType
{
    Stand,
    Move,

    Count
}

public enum GameTargetType
{
    None = -1,
    User = 0,
    Enemy,
    Summon
}

public enum GameAttackRangeType
{
    Circle = 0,
    Line = 1,

    Count
}


public enum GameSkillBattleType
{
    Normal = -1, // "Magic/magic"
    UI,
    Map, // "Sprite/man"
    UIMap,

    Count
}

public enum GameSkillBattleRanged
{
    None = -1,
    Normal,
    Ranged,

    Count
}

public enum GameSkillResutlType
{
    None = -1,
    Damage,
    Cure,
    Purge,
    Blessing,
    Curse,
    Special,

    Count
}

public enum GameSkillResutlEffect
{
    None = -1,
    StrUp,
    VitUp,
    IntUp,
    MoveUp,

    Violent,

    PhyImmunity,
    MagImmunity,

    Miss,

    Poison,
    Palsy,
    Silence,
    Fetter,
    Clear,

    SummonKiller,

    Summon,
    SummonRelive,

    Under,

    Count
}

public enum GameSkillOtherType
{
    None = -1,
    Normal,
    Target,
    Slef,
}

public enum GameSkillOtherEffect
{
    None = -1,

    AttackX2,
    AttackX3,
    AttackX4,
    AttackPalsy,
    AttackPoison,

    AttackSilence = 5,

    AttackHP,
    MP = 7,
    AttackFetter,

    KillSelf = 9,
    Invincible0 = 10,
    Invincible1 = 11,
    AbilityDown = 12,
    HealAll = 13,
    AbilityUp = 14,
    MPAdd = 15,

    Count
}

public enum GameSkillAdditionType
{
    None = -1,
    Target,
    Self,

    Count
}


[System.Serializable]
public class GameSkillSpiritAddition
{
    public GameSkillAdditionType Type;
    public short Request;
    public short Plus;
}

[System.Serializable]
public class GameSkill
{
    public short ID;

    public GameSkillType Type;

    public GameSkillMoveAttackType MoveAttackType;
    public GameTargetType TargetType;
    public GameAttackRangeType AttackRangeType;
    public short AttackRange;

    public short AttackRangeMin;
    public short AttackRangeMax;

    public short Hit;
    public short MPCost;
    public short MoveCost;

    public short BasePower;

    public short HitAdd;

    public GameAttributeType AttributeType;

    public GameSkillResutlType ResultType;
    public GameSkillResutlEffect ResultEffect;

    public GameSkillOtherType OtherType;
    public GameSkillOtherEffect OtherEffect;

    public GameSkillSpiritAddition[] addition = new GameSkillSpiritAddition[ (int)GameSpiritType.Count ];

    public short AnimationID;
    public short HasMovie;

    public GameSkillBattleType BattleType;
    public GameSkillBattleRanged BattleRanged;
    public short BattleSprite;
    public short Unknown4;

    public string NameS;
    public string DescriptionS;

    public string NameT;
    public string DescriptionT;

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


public class GameSkillData : Singleton< GameSkillData >
{
    [SerializeField]
    GameSkill[] data;
    

    public GameSkill getData( int id )
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

        data = new GameSkill[ bytes.Length / 144 ];

        int index = 0;
        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameSkill skill = new GameSkill();

            skill.ID = (short)i;
            skill.MoveAttackType = (GameSkillMoveAttackType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.TargetType = (GameTargetType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.AttackRangeType = (GameAttackRangeType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.AttackRange = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.AttackRangeMin = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.AttackRangeMax = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.HitAdd = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.MPCost = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.MoveCost = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.BasePower = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.Hit = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.AttributeType = (GameAttributeType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.ResultType = (GameSkillResutlType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.ResultEffect = (GameSkillResutlEffect)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.OtherType = (GameSkillOtherType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.OtherEffect = (GameSkillOtherEffect)BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                skill.addition[ j ] = new GameSkillSpiritAddition();
                skill.addition[ j ].Type = (GameSkillAdditionType)BitConverter.ToInt16( bytes , index ); index += 2;
                skill.addition[ j ].Request = BitConverter.ToInt16( bytes , index ); index += 2;
                skill.addition[ j ].Plus = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            skill.AnimationID = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.HasMovie = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.BattleType = (GameSkillBattleType)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.BattleRanged = (GameSkillBattleRanged)BitConverter.ToInt16( bytes , index ); index += 2;
            skill.BattleSprite = BitConverter.ToInt16( bytes , index ); index += 2;
            skill.Unknown4 = BitConverter.ToInt16( bytes , index ); index += 2;

            //             if ( skill.BattleType != GameSkillBattleType.None )
            //             {
            //                 if ( skill.ResultEffect != GameSkillResutlEffect.None )
            //                 {
            //                     Debug.Log( i );
            //                 }
            //             }


//            Debug.Log( "hit " + skill.ID + " " + skill.Hit + " " + skill.HitAdd );

            skill.Type = GameSkillType.Magic;

            if ( skill.BattleType == GameSkillBattleType.UI )
            {
                skill.Type = GameSkillType.Physical;
            }

            if ( skill.OtherEffect == GameSkillOtherEffect.AttackX2 ||
                skill.OtherEffect == GameSkillOtherEffect.AttackX3 || 
                skill.OtherEffect == GameSkillOtherEffect.AttackX4 ||
                skill.OtherEffect == GameSkillOtherEffect.AttackPalsy ||
                skill.OtherEffect == GameSkillOtherEffect.AttackPoison || 
                skill.OtherEffect == GameSkillOtherEffect.AttackHP ||
                skill.OtherEffect == GameSkillOtherEffect.AttackFetter )
            {
                skill.Type = GameSkillType.Physical;
            }

            skill.NameT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 10 ); index += 10;
            skill.DescriptionT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 60 ); index += 60;

            skill.NameS = ChineseStringUtility.ToSimplified( skill.NameT );
            skill.DescriptionS = ChineseStringUtility.ToSimplified( skill.DescriptionT );

            data[ i ] = skill;
        }


        Debug.Log( "GameSkillData loaded." );
    }


    public void saveText()
    {
        string strHelp = "<table interlaced=\"enabled\" align=\"center\" ><tbody>";

        strHelp += "<tr>";

        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"100\" align=\"center\">";
        strHelp += "<strong>名称：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"50\" align=\"center\">";
        strHelp += "<strong>术力：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"60\" align=\"center\">";
        strHelp += "<strong>类型：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"60\" align=\"center\">";
        strHelp += "<strong>效果：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"100\" align=\"center\">";
        strHelp += "<strong>基础威力：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"50\" align=\"center\">";
        strHelp += "<strong>命中：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"120\" align=\"center\">";
        strHelp += "<strong>附加：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"100\" align=\"center\">";
        strHelp += "<strong>附加命中：</strong>";
        strHelp += "</td>";
        strHelp += "<td style=\"background-color: rgb(235, 241, 221);\" width=\"120\" align=\"center\">";
        strHelp += "<strong>其他效果：</strong>";
        strHelp += "</td>";
        strHelp += "</tr>";

        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameSkill skill = data[ i ];

            strHelp += "<tr>";

            strHelp += "<td width=\"100\" align=\"center\">";
            strHelp += skill.NameS;
            strHelp += "</td>";

            strHelp += "<td width=\"50\" align=\"center\">";
            strHelp += skill.MPCost;
            strHelp += "</td>";

            strHelp += "<td width=\"60\" align=\"center\">";

            switch ( skill.Type )
            {
                case GameSkillType.Magic:
                    strHelp += "咒术";
                    break;
                case GameSkillType.Physical:
                    strHelp += "物理";
                    break;
            }

            strHelp += "</td>";

            strHelp += "<td width=\"60\" align=\"center\">";

            switch ( skill.ResultType )
            {
                case GameSkillResutlType.None:
                    break;
                case GameSkillResutlType.Damage:
                    strHelp += "伤害";
                    break;
                case GameSkillResutlType.Cure:
                    strHelp += "治愈";
                    break;
                case GameSkillResutlType.Purge:
                    strHelp += "净化";
                    break;
                case GameSkillResutlType.Blessing:
                    strHelp += "祝福";
                    break;
                case GameSkillResutlType.Curse:
                    strHelp += "诅咒";
                    break;
                case GameSkillResutlType.Special:
                    strHelp += "特殊";
                    break;
            }

            strHelp += "</td>";

            strHelp += "<td width=\"100\" align=\"center\">";
            strHelp += skill.BasePower;
            strHelp += "</td>";

            strHelp += "<td width=\"50\" align=\"center\">";
            strHelp += ( skill.Hit > 0 ? skill.Hit.ToString() : "" );
            strHelp += "</td>";


            strHelp += "<td width=\"120\" align=\"center\">";
            switch ( skill.ResultEffect )
            {
                case GameSkillResutlEffect.StrUp:
                    strHelp += "提升攻击力";
                    break;
                case GameSkillResutlEffect.VitUp:
                    strHelp += "提升防御力";
                    break;
                case GameSkillResutlEffect.IntUp:
                    strHelp += "提升咒术攻击力";
                    break;
                case GameSkillResutlEffect.MoveUp:
                    strHelp += "提升移动力";
                    break;
                case GameSkillResutlEffect.Violent:
                    strHelp += "提升攻击力";
                    break;
                case GameSkillResutlEffect.PhyImmunity:
                    strHelp += "物理免疫";
                    break;
                case GameSkillResutlEffect.MagImmunity:
                    strHelp += "咒术免疫";
                    break;
                case GameSkillResutlEffect.Miss:
                    strHelp += "闪避";
                    break;
                case GameSkillResutlEffect.Poison:
                    strHelp += "中毒";
                    break;
                case GameSkillResutlEffect.Palsy:
                    strHelp += "麻痹";
                    break;
                case GameSkillResutlEffect.Silence:
                    strHelp += "封魔";
                    break;
                case GameSkillResutlEffect.Fetter:
                    strHelp += "地缚";
                    break;
                case GameSkillResutlEffect.Clear:
                    strHelp += "净化";
                    break;
                case GameSkillResutlEffect.SummonKiller:
                    strHelp += "反召唤";
                    break;
                case GameSkillResutlEffect.Summon:
                    strHelp += "召唤";
                    break;
                case GameSkillResutlEffect.SummonRelive:
                    strHelp += "召唤复活";
                    break;
                case GameSkillResutlEffect.Under:
                    strHelp += "";
                    break;
                case GameSkillResutlEffect.Count:
                    break;
            }

            strHelp += "</td>";

            strHelp += "<td width=\"100\" align=\"center\">";
            strHelp += ( skill.HitAdd > 0 ? skill.HitAdd.ToString() : "" );
            strHelp += "</td>";



            strHelp += "<td width=\"120\" align=\"center\">";
            switch ( skill.OtherEffect )
            {
                case GameSkillOtherEffect.AttackX2:
                    strHelp += "伤害*2";
                    break;
                case GameSkillOtherEffect.AttackX3:
                    strHelp += "伤害*3";
                    break;
                case GameSkillOtherEffect.AttackX4:
                    strHelp += "伤害*4";
                    break;
                case GameSkillOtherEffect.AttackPalsy:
                    strHelp += "伤害+麻痹";
                    break;
                case GameSkillOtherEffect.AttackPoison:
                    strHelp += "伤害+中毒";
                    break;
                case GameSkillOtherEffect.AttackSilence:
                    strHelp += "伤害+封魔";
                    break;
                case GameSkillOtherEffect.AttackHP:
                    strHelp += "伤害+吸血";
                    break;
                case GameSkillOtherEffect.MP:
                    strHelp += "伤害+吸魔";
                    break;
                case GameSkillOtherEffect.AttackFetter:
                    strHelp += "伤害+地缚";
                    break;
                case GameSkillOtherEffect.KillSelf:
                    strHelp += "自爆";
                    break;
                case GameSkillOtherEffect.Invincible0:
                    strHelp += "物理免疫";
                    break;
                case GameSkillOtherEffect.Invincible1:
                    strHelp += "咒术免疫";
                    break;
                case GameSkillOtherEffect.AbilityDown:
                    strHelp += "驱散";
                    break;
                case GameSkillOtherEffect.HealAll:
                    strHelp += "治疗+全净化";
                    break;
                case GameSkillOtherEffect.AbilityUp:
                    strHelp += "能力全上升";
                    break;
                case GameSkillOtherEffect.MPAdd:
                    strHelp += "术力恢复";
                    break;
            }
            strHelp += "</td>";

            strHelp += "</tr>";
        }

        strHelp += "</tbody></table>";

        File.WriteAllText( Application.dataPath + "/Objects/Help/Help20.txt" , strHelp , Encoding.UTF8 );
    }


}
