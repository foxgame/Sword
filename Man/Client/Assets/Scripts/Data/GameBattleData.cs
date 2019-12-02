using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameBattleJudgeJudgeType
{
    None = 0,
    KillAll,
    KillOne,

    Count
}

public enum GameBattleAIType
{
    None = -1,
    AIStay = 0,
    AIPositive = 1,
    AINegative = 2,

    AIMoveToUnit = 3,
    AIMoveToPos = 4,
    AI5 = 5,
    AI6 = 6,
    AI7 = 7,
    AI8 = 8,
    AI9 = 9,
    AI10 = 10,

    Count
}

public enum GameBattleTurnEventType
{
    // turn start
    Start = 0,

    // npc end , user start
    User = 1,

    // enemy start
    Enemy = 2,

    Count
}

[System.Serializable]
public class GameBattleMan
{
    public short BattleManID;

    public GameUnitBase UnitBase = new GameUnitBase();

    public short HpAdd;
    public short HpSub;
    public short MpAdd;
    public short MpSub;

    public short EventParm1;
    public short EventParm2;

    public short KillGetType;
    public short KillGetParm1;

    public short killEvent;
    public short UnknowParm88;

    public GameBattleAIType BattleAIType;

    public short moveToX;
    public short moveToY;
    public short moveToIDUnkonw;
    public short moveToID;

    public short[] UnknowParm92 = new short[ 10 ];

    public GameBattleMan clone()
    {
        GameBattleMan man = new GameBattleMan();

        man.UnitBase.UnitID = UnitBase.UnitID;
        man.UnitBase.LV = UnitBase.LV;
        man.UnitBase.Exp = UnitBase.Exp;
        man.UnitBase.HP = UnitBase.HP;
        man.UnitBase.MP = UnitBase.MP;
        man.UnitBase.Str = UnitBase.Str;
        man.UnitBase.Int = UnitBase.Int;
        man.UnitBase.Avg = UnitBase.Avg;
        man.UnitBase.Vit = UnitBase.Vit;
        man.UnitBase.Luk = UnitBase.Luk;
        man.UnitBase.Weapon = UnitBase.Weapon;
        man.UnitBase.Armor = UnitBase.Armor;
        man.UnitBase.Accessory = UnitBase.Accessory;
        man.UnitBase.InTeam = UnitBase.InTeam;
        man.UnitBase.BaseSpiritPower = UnitBase.BaseSpiritPower;

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            man.UnitBase.SpiritPower[ i ] = UnitBase.SpiritPower[ i ];
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            man.UnitBase.Items[ i ] = UnitBase.Items[ i ];
            man.UnitBase.Skills[ i ] = UnitBase.Skills[ i ];
        }
   
        man.BattleManID = BattleManID;
        man.HpAdd = HpAdd;
        man.HpSub = HpSub;
        man.MpAdd = MpAdd;
        man.MpSub = MpSub;
        man.EventParm1 = EventParm1;
        man.EventParm2 = EventParm2;
        man.KillGetType = KillGetType;
        man.KillGetParm1 = KillGetParm1;
        man.killEvent = killEvent;
        man.UnknowParm88 = UnknowParm88;
        man.BattleAIType = BattleAIType;
        man.moveToX = moveToX;
        man.moveToY = moveToY;
        man.moveToIDUnkonw = moveToIDUnkonw;
        man.moveToID = moveToID;

        for ( int i = 0 ; i < 10 ; i++ )
        {
            man.UnknowParm92[ i ] = UnknowParm92[ i ];
        }

        return man;
    }
}


[System.Serializable]
public class GameBattleXY
{
    public short ID;
    public short X;
    public short Y;
}

public enum GameBattleMapEventType
{
    None = -1,
    Event = 0,
    Item = 1,
    Gold = 2,

    Count
}


[System.Serializable]
public class GameBattleMEVT
{
    public GameBattleMapEventType EventType;
    public short EventParm1;
    public short EventParm2;

    public GameBattleMapEventType ItemGetType;
    public short ItemGetParm1;
    public short ItemGetParm2;

    public short MapShow;
}

[System.Serializable]
public class GameBattleTEVT
{
    public short EventTurn;
    public GameBattleTurnEventType EventType;
    public short EventID;
}

[System.Serializable]
public class GameBattleEvent
{
    [System.Serializable]
    public class BattleEvent
    {
        public int Type;
        public byte[] Data;
    }

    public BattleEvent[] Event;
}

[System.Serializable]
public class GameBattleJudgeWin
{
    public GameBattleJudgeJudgeType Judge;

    public short ID;
    public short EventID;
}

[System.Serializable]
public class GameBattleJudgeLose
{
    public GameBattleJudgeJudgeType Judge;

    public short ID;
    public short EventID;
}

[System.Serializable]
public class GameBattleJudge
{
    public GameBattleJudgeWin[] JudgeWin;
    public GameBattleJudgeLose[] JudgeLose;
}

[System.Serializable]
public class GameBattleSDES
{
    public string TitleT;
    public string TitleS;
    public string WinT;
    public string WinS;
    public string LoseT;
    public string LoseS;
    public string ProficiencyT;
    public string ProficiencyS;


    public string Proficiency
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return ProficiencyS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return ProficiencyT;
            }
            return "";
        }
    }


    public string Title
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return TitleS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return TitleT;
            }
            return "";
        }
    }

    public string Win
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return WinS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return WinT;
            }
            return "";
        }
    }

    public string Lose
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return LoseS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return LoseT;
            }
            return "";
        }
    }
}

[System.Serializable]
public class GameBattleEMSG
{
    public string[] MsgT;
    public string[] MsgS;

    public string[] Msg
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return MsgS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return MsgT;
            }

            return null;
        }
    }

    public string this[ int i ]
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return MsgS[ i ];
                case GameSetting.GameLocation.TraditionalChinese:
                    return MsgT[ i ];
            }
            return "";
        }
    }
}


[System.Serializable]
public class GameBattleLayer
{
    [System.Serializable]
    public class Layer0
    {
        public short ID;
        public int Unknow0;
        public short Unknow1;
        public short Unknow2;
        public short Unknow21;
        public int Unknow3;

        public short Unknow4;
        public short Unknow5;
        public short Lyaer;
        public short Unknow61;
        public int Unknow7;
        public short Unknow8;
        public short Unknow9;
        public short Unknow10;

        public short Width;
        public short Height;

        public string Name;
    }

    [System.Serializable]
    public class Layer1
    {
        public short ID;
        public short Parm;
        public short OffsetX;
        public short OffsetY;

        public short ParmEffect;
        public short Unknow4;
        public short Pause;
        public short Delay;
        public short VisibleBattle;

        public string Name;
    };

    public Layer0[] L0;
    public Layer1[] L1;
}

[ System.Serializable ]
public class GameBattleDTL
{
    public byte Unknow0;
    public byte Unknow1;
    public byte Unknow2;
    public byte Unknow3;

    public short Width;
    public short Height;

    public byte[] Unknow = new byte[ 18 ];

    [System.Serializable]
    public class Point
    {
        public byte Move;
        public byte Floor;
        public short MapEvent;
    }

    public Point[] Points;
}

[System.Serializable]
public class GameBattleStage
{
    public GameBattleDTL DTL = new GameBattleDTL();
    public GameBattleLayer Layer = new GameBattleLayer();
    public GameBattleSDES SDES = new GameBattleSDES();
    public GameBattleJudge Judge = new GameBattleJudge();
    public GameBattleEvent[] Event;
    public GameBattleTEVT[] TEVT;
    public GameBattleEMSG[] EMSG;
    public GameBattleXY[] XY;
    public GameBattleMan[] Man;
    public GameBattleMEVT[] MEVT;
}

[System.Serializable]
public class GameBattleStart
{
    public short Music;
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

public class GameBattleData : Singleton< GameBattleData >
{
    [SerializeField]
    GameBattleStage[] data = new GameBattleStage[ GameDefine.MAX_STAGE ];

    [SerializeField]
    GameBattleStart[] startData = new GameBattleStart[ GameDefine.MAX_STAGE ];


#if UNITY_EDITOR
    [Serializable]
    public class aaaa
    {
        [SerializeField]
        public List<int> aa = new List<int>();
    }

    [SerializeField]
    public aaaa[] aaa = new aaaa[ 255 ];
#endif


    public GameBattleStage getStage( int id )
    {
        if ( id < 0 || data.Length <= id )
        {
            return null;
        }

        return data[ id ];
    }

    public GameBattleStart getStartData( int id )
    {
        if ( id < 0 || startData.Length <= id )
        {
            return null;
        }

        return startData[ id ];
    }


    public void clear()
    {
//         GameObject cellsObj = GameObject.Find( "Cells" );
//         while ( cellsObj.transform.childCount > 0 )
//         {
//             DestroyImmediate( cellsObj.transform.GetChild( 0 ).gameObject );
//         }
    }


    public void loadStartData( string path )
    {
        FileStream fs = File.OpenRead( path );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;

        for ( int i = 0 ; i < GameDefine.MAX_STAGE ; i++ )
        {
            startData[ i ] = new GameBattleStart();
            startData[ i ].Music = BitConverter.ToInt16( bytes , index ); index += 2;
            startData[ i ].NameT = Encoding.GetEncoding( "big5" ).GetString( bytes , index , 24 ); index += 24;
            startData[ i ].NameS = ChineseStringUtility.ToSimplified( startData[ i ].NameT );
        }
    }

    void loadEvent( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathEMSG = pathDir + "EVENT_" + path + ".COD";

        FileStream fs = File.OpenRead( pathEMSG );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();


        int index = 0;
        int count = BitConverter.ToInt32( bytes , index ); index += 4;
        int size = BitConverter.ToInt32( bytes , index ); index += 4;

        data[ stage ].Event = new GameBattleEvent[ count ];

        for ( int i = 0 ; i < count ; i++ )
        {
            data[ stage ].Event[ i ] = new GameBattleEvent();

            int pos1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int count1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int unknow1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int size1 = BitConverter.ToInt32( bytes , index ); index += 4;

            data[ stage ].Event[ i ].Event = new GameBattleEvent.BattleEvent[ count1 ];

            int[] size2 = new int[ count1 ];
            size1 -= count1 * 4;
            for ( int j = 0 ; j < count1 ; ++j )
            {
                if ( j == count1 - 1 )
                {
                    size2[ j ] = size1;
                }
                else
                {
                    int l0 = BitConverter.ToInt32( bytes , pos1 + j * 4 );
                    int l1 = BitConverter.ToInt32( bytes , pos1 + ( j + 1 ) * 4 );

                    size2[ j ] = l1 - l0;
                }

                size1 -= size2[ j ];
            }

            for ( int j = 0 ; j < count1 ; ++j )
            {
                data[ stage ].Event[ i ].Event[ j ] = new GameBattleEvent.BattleEvent();
                data[ stage ].Event[ i ].Event[ j ].Data = new byte[ size2[ j ] - 4 ];

                int l0 = BitConverter.ToInt32( bytes , pos1 + j * 4 );
                data[ stage ].Event[ i ].Event[ j ].Type = BitConverter.ToInt32( bytes , l0 );

                switch ( data[ stage ].Event[ i ].Event[ j ].Type )
                {
                    case 0x4E:
                       // Debug.Log( " battle event get item " + stage + " " + i + " " + j );
                        break;
                }

                for ( int k = 0 ; k < size2[ j ] - 4 ; k++ )
                {
                    data[ stage ].Event[ i ].Event[ j ].Data[ k ] = bytes[ l0 + k + 4 ];
                }

                switch ( data[ stage ].Event[ i ].Event[ j ].Type )
                {
                    case 0x17:
                        {
                            short parm = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            short check = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            Debug.Log( "0x17 show unit " + i + " " + j + " " + parm + " " + check );
                        }
                        break;
                    case 0x1E:
                        {
                            short s1 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            short s2 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            short s3 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 4 );
                            Debug.Log( "0x1E show effect parm event " + i + " " + j + " " + s1 + " " + s2 + " " + s3 );
                        }
                        break;
                    case 0x2F:
                        {
                            short id = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            short ai = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            short moveTo0 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 4 );
                            short moveTo1 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 6 );
                            short unkonw2 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 8 );
                            short unkonw3 = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 10 );

                            Debug.Log( "0x2F unit ai " + i + " " + j + " " + id + " " + (GameBattleAIType)ai + " " + moveTo0 + " " + moveTo1 + " " + unkonw2 + " " + unkonw3 );
                        }
                        break;
                    case 0x31:
                        {
                            // check game dat 
                            short id = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            int dat = BitConverter.ToInt32( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            short step = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 6 );
                            short unknow = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 8 );

                            Debug.Log( "0x31 unit ai " + i + " " + j + " " + id + " " + dat + " " + step + " " + unknow );
                        }
                        break;
                    case 0x3A:
                        {
                            short id = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            int value = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            Debug.Log( "0x3A add game data " + i + " " + j + " " + id + " " + value );
                        }
                        break;
                    case 0x37:
                        {
                            short id = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            Debug.Log( "0x37 game data enemy count " + i + " " + j + " " + id );
                        }
                        break;
                    case 0x41:
                        {
                            short id = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 0 );
                            short dat = BitConverter.ToInt16( data[ stage ].Event[ i ].Event[ j ].Data , 2 );
                            Debug.Log( "0x41 set game data " + i + " " + j + " " + id + " " + dat );
                        }
                        break;
                }
            }


            
            
        }

    }


    public void loadJudgeData( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathSDES = pathDir + "JUDGE_" + path + ".COD";

        FileStream fs = File.OpenRead( pathSDES );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        int count0 = BitConverter.ToInt32( bytes , index );
        index = 8;
        int pos1 = BitConverter.ToInt32( bytes , index );
        index = 0x0C;
        int count1 = BitConverter.ToInt32( bytes , index );
        index = 0x18;
        int pos2 = BitConverter.ToInt32( bytes , index );
        index = 0x1C;
        int count2 = BitConverter.ToInt32( bytes , index );

        data[ stage ].Judge.JudgeWin = new GameBattleJudgeWin[ count1 ];
        data[ stage ].Judge.JudgeLose = new GameBattleJudgeLose[ count2 ];

        for ( int i = 0 ; i < count2 ; i++ )
        {
            int p = BitConverter.ToInt32( bytes , pos2 + i * 4 );

            data[ stage ].Judge.JudgeLose[ i ] = new GameBattleJudgeLose();
            data[ stage ].Judge.JudgeLose[ i ].Judge = (GameBattleJudgeJudgeType)BitConverter.ToInt32( bytes , p );

            if ( data[ stage ].Judge.JudgeLose[ i ].Judge == 0 )
            {
            }
            else if ( data[ stage ].Judge.JudgeLose[ i ].Judge == GameBattleJudgeJudgeType.KillOne )
            {
                data[ stage ].Judge.JudgeLose[ i ].ID = BitConverter.ToInt16( bytes , p + 4 );
                data[ stage ].Judge.JudgeLose[ i ].EventID = BitConverter.ToInt16( bytes , p + 6 );
            }
            else
            {
                Debug.LogError( data[ stage ].Judge.JudgeLose[ i ].Judge + " " + stage );
            }
        }


        for ( int i = 0 ; i < count1 ; i++ )
        {
            int p = BitConverter.ToInt32( bytes , pos1 + i * 4 );

            data[ stage ].Judge.JudgeWin[ i ] = new GameBattleJudgeWin();
            data[ stage ].Judge.JudgeWin[ i ].Judge = (GameBattleJudgeJudgeType)BitConverter.ToInt32( bytes , p );

            if ( data[ stage ].Judge.JudgeWin[ i ].Judge == 0 )
            {
            }
            else if ( data[ stage ].Judge.JudgeWin[ i ].Judge == GameBattleJudgeJudgeType.KillAll )
            {
                data[ stage ].Judge.JudgeWin[ i ].ID = BitConverter.ToInt16( bytes , p + 4 );
                data[ stage ].Judge.JudgeWin[ i ].EventID = BitConverter.ToInt16( bytes , p + 6 );
            }
            else if ( data[ stage ].Judge.JudgeWin[ i ].Judge == GameBattleJudgeJudgeType.KillOne )
            {
                data[ stage ].Judge.JudgeWin[ i ].ID = BitConverter.ToInt16( bytes , p + 4 );
                data[ stage ].Judge.JudgeWin[ i ].EventID = BitConverter.ToInt16( bytes , p + 6 );
            }
            else
            {
                Debug.LogError( data[ stage ].Judge.JudgeWin[ i ].Judge + " " + stage );
            }
        }

    }


    public void loadSDESData( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathSDES = pathDir + "SDES_" + path + ".DAT";

        FileStream fs = File.OpenRead( pathSDES );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        data[ stage ].SDES.TitleT = Encoding.GetEncoding( "big5" ).GetString( bytes , 0 , 23 );
        data[ stage ].SDES.WinT = Encoding.GetEncoding( "big5" ).GetString( bytes , 0x18 , 52 );
        data[ stage ].SDES.LoseT = Encoding.GetEncoding( "big5" ).GetString( bytes , 0x4C , 52 );

        if ( stage == 11 )
        {
            data[ stage ].SDES.TitleT = "第十一幕  故人";
        }

        data[ stage ].SDES.TitleS = ChineseStringUtility.ToSimplified( data[ stage ].SDES.TitleT );
        data[ stage ].SDES.WinS = ChineseStringUtility.ToSimplified( data[ stage ].SDES.WinT );
        data[ stage ].SDES.LoseS = ChineseStringUtility.ToSimplified( data[ stage ].SDES.LoseT );

        switch ( stage )
        {
            case 0:
                data[ stage ].SDES.ProficiencyS = "获得燧石";
                break;
            case 1:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 2:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 3:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 4:
                data[ stage ].SDES.ProficiencyS = "敌全灭+1";
                break;
            case 5:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 6:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 7:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 8:
                data[ stage ].SDES.ProficiencyS = "不让不净散人攻击朱慎";
                break;
            case 9:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 10:
                data[ stage ].SDES.ProficiencyS = "击败夏候仪";
                break;
            case 11:
                data[ stage ].SDES.ProficiencyS = "不让天玄门弟子阵亡";
                break;
            case 12:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 13:
                data[ stage ].SDES.ProficiencyS = "同时击败高世津和灵山老人";
                break;
            case 14:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物";
                break;
            case 15:
                data[ stage ].SDES.ProficiencyS = "二十回合内击倒夏侯仪并四回合内击倒鬼魄尸王";
                break;
            case 16:
                data[ stage ].SDES.ProficiencyS = "救下至少九名禁军士兵";
                break;
            case 17:
                data[ stage ].SDES.ProficiencyS = "三十五回合内敌全灭";
                break;
            case 18:
                data[ stage ].SDES.ProficiencyS = "无";
                break;
            case 19:
                data[ stage ].SDES.ProficiencyS = "击杀敌人数小于8";
                break;
            case 20:
                data[ stage ].SDES.ProficiencyS = "除韩无砂外的敌全灭";
                break;
            case 21:
                data[ stage ].SDES.ProficiencyS = "二十回合内结束本关";
                break;
            case 22:
                data[ stage ].SDES.ProficiencyS = "不让封寒月攻击韩无砂+1";
                break;
            case 23:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 24:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 25:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 26:
                data[ stage ].SDES.ProficiencyS = "十五回合以上结束本关+1";
                break;
            case 27:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 28:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 29:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 30:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 31:
                data[ stage ].SDES.ProficiencyS = "无";
                break;
            case 32:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 33:
                data[ stage ].SDES.ProficiencyS = "获得全部宝物+1";
                break;
            case 34:
                data[ stage ].SDES.ProficiencyS = "无";
                break;
            case 35:
                data[ stage ].SDES.ProficiencyS = "无";
                break;
        }

        data[ stage ].SDES.ProficiencyT = ChineseStringUtility.ToTraditional( data[ stage ].SDES.ProficiencyS );

    }

    void loadEMSG( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathEMSG = pathDir + "EMSG_" + path + ".MSG";

        FileStream fs = File.OpenRead( pathEMSG );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();


        int index = 0;
        int c = BitConverter.ToInt32( bytes , index ); index += 4;
        int size = BitConverter.ToInt32( bytes , index ); index += 4;

        int[] pos = new int[ c ];
        int[] count = new int[ c ];
        int[] unknow3 = new int[ c ];
        int[] len = new int[ c ];

        data[ stage ].EMSG = new GameBattleEMSG[ c ];

        for ( int ii = 0 ; ii < c ; ii++ )
        {
            pos[ ii ] = BitConverter.ToInt32( bytes , index ); index += 4;
            count[ ii ] = BitConverter.ToInt32( bytes , index ); index += 4;
            unknow3[ ii ] = BitConverter.ToInt32( bytes , index ); index += 4;
            len[ ii ] = BitConverter.ToInt32( bytes , index ); index += 4;
        }

        for ( int ii = 0 ; ii < c ; ii++ )
        {
            index = pos[ ii ] + 4;

            int[] pos1 = new int[ count[ ii ] ];
            int[] pos2 = new int[ count[ ii ] ];

            for ( int i = 0 ; i < count[ ii ] ; i++ )
            {
                pos1[ i ] = BitConverter.ToInt32( bytes , index ); index += 4;
            }

            len[ ii ] -= 4 + count[ ii ] * 4;

            for ( int i = 0 ; i < count[ ii ] ; i++ )
            {
                if ( i == count[ ii ] - 1 )
                {
                    pos2[ i ] = len[ ii ];
                }
                else
                {
                    pos2[ i ] = pos1[ i + 1 ] - pos1[ i ];
                    len[ ii ] -= pos2[ i ];
                }
            }

            data[ stage ].EMSG[ ii ] = new GameBattleEMSG();
            data[ stage ].EMSG[ ii ].MsgT = new string[ count[ ii ] ];
            data[ stage ].EMSG[ ii ].MsgS = new string[ count[ ii ] ];

            for ( int i = 0 ; i < count[ ii ] ; i++ )
            {
                string str = "";

                int l1 = pos1[ i ];
                int l2 = 0;
                for ( int j = pos1[ i ] ; j < pos1[ i ] + pos2[ i ] ; j++ )
                {
                    if ( bytes[ j ] == 0 )
                    {
                        l2 = j - l1;

                        string m = Encoding.GetEncoding( "big5" ).GetString( bytes , l1 , l2 );
                        str += m;

                        int n = 1;
                        while ( true )
                        {
                            if ( bytes[ j + n ] == 0 )
                            {
                                n++;
                            }
                            else if ( bytes[ j + n ] == 1 )
                            {
                                l1 = j + n + 1;
                                j = l1;
                                str += "\n";
                                break;
                            }
                            else if ( bytes[ j + n ] == 2 )
                            {
                                l1 = j + n + 1;
                                j = l1;
                                break;
                            }
                            else
                            {
                                Debug.LogError( "eeeeeeeeeeee " + bytes[ j + n ] );
                                break;
                            }
                        }

                    }
                }

                data[ stage ].EMSG[ ii ].MsgT[ i ] = str;
                data[ stage ].EMSG[ ii ].MsgS[ i ] = ChineseStringUtility.ToSimplified( str );

            }

        }
    }

    void loadXY( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathXY = pathDir + "xy_" + path + ".COD";

        FileStream fs = File.OpenRead( pathXY );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        
        int index = 7;
        int count = BitConverter.ToInt16( bytes , index ); index += 2;
        data[ stage ].XY = new GameBattleXY[ count ];

        for ( int i = 0 ; i < count ; i++ )
        {
            data[ stage ].XY[ i ] = new GameBattleXY();
            data[ stage ].XY[ i ].ID = BitConverter.ToInt16( bytes , index ); index += 2;
            data[ stage ].XY[ i ].X = BitConverter.ToInt16( bytes , index ); index += 2;
            data[ stage ].XY[ i ].Y = BitConverter.ToInt16( bytes , index ); index += 2;


            // editor test
//             GameObject imgObject = Resources.Load<GameObject>( "Prefab/Image" );
//             GameObject obj = Instantiate( imgObject , GameObject.Find( "Cells" ).transform );
//             obj.name = "xy id: " + data[ stage ].XY[ i ].ID + " " + data[ stage ].XY[ i ].Y + "-" + data[ stage ].XY[ i ].X;
// 
//             RectTransform trans = obj.GetComponent<RectTransform>();
//             trans.localPosition = new Vector3( data[ stage ].XY[ i ].X * 30 , -data[ stage ].XY[ i ].Y * 24 , 0.0f );
//             trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
//             trans.sizeDelta = new Vector2( 30 , 24 );
// 
//             Image image = obj.GetComponent<Image>();
//             image.color = new Color( 1.0f , 0 , 0 , 0.7f );
        }

        if ( stage == 19 )
        {
            // fix 19 bug.

            for ( int i = 0 ; i < data[ stage ].Man.Length ; i++ )
            {
                short y = data[ stage ].Man[ i ].moveToY;

                if ( data[ stage ].Man[ i ].moveToY > 0 )
                {
                    data[ stage ].Man[ i ].moveToID = y;
                    data[ stage ].Man[ i ].moveToX = 0;
                    data[ stage ].Man[ i ].moveToY = 0;
                }
            }

        }

    }

    void loadDTL( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathDTL = pathDir + "map" + path + ".dtl";

        FileStream fs = File.OpenRead( pathDTL );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        GameBattleDTL dtl = data[ stage ].DTL;

        int index = 3;
        dtl.Unknow0 = bytes[ index ]; index += 1;
        dtl.Unknow1 = bytes[ index ]; index += 1;
        dtl.Unknow2 = bytes[ index ]; index += 1;
        dtl.Unknow3 = bytes[ index ]; index += 1;

        dtl.Width = BitConverter.ToInt16( bytes , index ); index += 2;
        dtl.Height = BitConverter.ToInt16( bytes , index ); index += 2;

        for ( int i = 0 ; i < dtl.Unknow.Length ; i++ )
        {
            dtl.Unknow[ i ] = bytes[ index ]; index += 1;
        }

        dtl.Points = new GameBattleDTL.Point[ dtl.Height * dtl.Width ];

        int ii = 0;
        for ( int i = 0 ; i < dtl.Height ; i++ )
        {
            for ( int j = 0 ; j < dtl.Width ; j++ )
            {
                dtl.Points[ ii ] = new GameBattleDTL.Point();
                dtl.Points[ ii ].Move = bytes[ index ]; index += 1;
                dtl.Points[ ii ].Floor = bytes[ index ]; index += 1;
                dtl.Points[ ii ].MapEvent = BitConverter.ToInt16( bytes , index ); index += 2;


                // editor test
//                 GameObject imgObject = Resources.Load<GameObject>( "Prefab/Image" );
//                 GameObject obj = Instantiate( imgObject , GameObject.Find( "Cells" ).transform );
//                 obj.name = i + "-" + j;
// 
//                 RectTransform trans = obj.GetComponent<RectTransform>();
//                 trans.localPosition = new Vector3( j * 30 , -i * 24 , 0.0f );
//                 trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
//                 trans.sizeDelta = new Vector2( 30 , 24 );
// 
//                 Image image = obj.GetComponent<Image>();
//                 float c = ( dtl.Points[ ii ].Value0 * 25 ) / 255.0f;
//                 image.color = new Color( c , c , c , ( c > 0.5f ? 0.5f : c ) );
// 
//                 if ( dtl.Points[ ii ].MapEvent != -1 &&
//                     data[ stage ].MEVT.Length > dtl.Points[ ii ].MapEvent )
//                 {
//                     if ( data[ stage ].MEVT[ dtl.Points[ ii ].MapEvent ].EventType == 0 )
//                     {
//                         image.color = new Color( 0.0f , 1.0f , 0.0f , 0.7f );
//                     }
//                     else if ( data[ stage ].MEVT[ dtl.Points[ ii ].MapEvent ].EventType == 1 )
//                     {
//                         image.color = new Color( 1.0f , 1.0f , 0.0f , 0.7f );
//                     }
//                 }

                ii++;
            }
        }

    }

    void loadMan( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathMan = pathDir + "MAN_" + path + ".DAT";

        FileStream fs = File.OpenRead( pathMan );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        int count = BitConverter.ToInt16( bytes , index ); index += 2;
        data[ stage ].Man = new GameBattleMan[ count ];


        for ( int i = 0 ; i < count ; i++ )
        {
            GameBattleMan p = new GameBattleMan();
            p.BattleManID = (short)i;

            p.UnitBase.UnitID = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.LV = BitConverter.ToInt16( bytes , index ); index += 2;
            p.HpAdd = BitConverter.ToInt16( bytes , index ); index += 2;
            p.HpSub = BitConverter.ToInt16( bytes , index ); index += 2;
            p.MpAdd = BitConverter.ToInt16( bytes , index ); index += 2;
            p.MpSub = BitConverter.ToInt16( bytes , index ); index += 2;

            p.UnitBase.Str = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Int = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Avg = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Vit = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Luk = BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                p.UnitBase.SpiritPower[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }
            p.UnitBase.Weapon = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Armor = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnitBase.Accessory = BitConverter.ToInt16( bytes , index ); index += 2;

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                p.UnitBase.Items[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            if ( p.UnitBase.WeaponSlot == GameDefine.INVALID_ID )
                p.UnitBase.Weapon = GameDefine.INVALID_ID;
            if ( p.UnitBase.ArmorSlot == GameDefine.INVALID_ID )
                p.UnitBase.Armor = GameDefine.INVALID_ID;
            if ( p.UnitBase.AccessorySlot == GameDefine.INVALID_ID )
                p.UnitBase.Accessory = GameDefine.INVALID_ID;

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                p.UnitBase.Skills[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;

#if UNITY_EDITOR
//                 if ( p.UnitBase.Skills[ j ] >= 0 )
//                 {
//                     if ( aaa[ p.UnitBase.Skills[ j ] ] == null )
//                     {
//                         aaa[ p.UnitBase.Skills[ j ] ] = new aaaa();
//                     }
// 
//                     GameUnit unit = GameUnitData.instance.getData( p.UnitBase.UnitID );
//                     //                     Debug.LogError( p.UnitBase.Skills[ j ] + " " + p.UnitBase.UnitID + " " + unit.Sprite );
// 
//                     aaa[ p.UnitBase.Skills[ j ] ].aa.Remove( unit.Sprite );
//                     aaa[ p.UnitBase.Skills[ j ] ].aa.Add( unit.Sprite );
//                 }
#endif
            }

            

            p.EventParm1 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.EventParm2 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.KillGetType = BitConverter.ToInt16( bytes , index ); index += 2;
            p.KillGetParm1 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.killEvent = BitConverter.ToInt16( bytes , index ); index += 2;
            p.UnknowParm88 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.BattleAIType = (GameBattleAIType)BitConverter.ToInt16( bytes , index ); index += 2;

            p.moveToID = GameDefine.INVALID_ID;
            p.moveToIDUnkonw = GameDefine.INVALID_ID;
            p.moveToX = GameDefine.INVALID_ID;
            p.moveToY = GameDefine.INVALID_ID;

            short moveTo0 = BitConverter.ToInt16( bytes , index ); index += 2;
            short moveTo1 = BitConverter.ToInt16( bytes , index ); index += 2;

            if ( p.BattleAIType == GameBattleAIType.AIMoveToUnit )
            {
                p.moveToID = moveTo1;
                p.moveToIDUnkonw = moveTo0;

                Debug.LogError( "AIMoveToUnit " + stage + " " + i + " " + p.moveToIDUnkonw + " " + p.moveToID );
            }

            if ( p.BattleAIType == GameBattleAIType.AIMoveToPos )
            {
                p.moveToX = moveTo0;
                p.moveToY = moveTo1;

                Debug.LogError( "AIMoveToPos " + stage + " " + i + " " + p.moveToX + " " + p.moveToY );
            }


            for ( int j = 0 ; j < 10 ; j++ )
            {
                p.UnknowParm92[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            data[ stage ].Man[ i ] = p;

        }


    }

    void loadTEVT( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathTEVT = pathDir + "TEVT_" + path + ".DAT";

        FileStream fs = File.OpenRead( pathTEVT );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        int count = bytes.Length / 6;
        data[ stage ].TEVT = new GameBattleTEVT[ count ];

        for ( int i = 0 ; i < count ; i++ )
        {
            GameBattleTEVT p = new GameBattleTEVT();
            p.EventTurn = BitConverter.ToInt16( bytes , index ); index += 2;
            p.EventType = (GameBattleTurnEventType)BitConverter.ToInt16( bytes , index ); index += 2;
            p.EventID = BitConverter.ToInt16( bytes , index ); index += 2;

            if ( p.EventTurn < 0 )
            {
                Debug.Log( "EventTurn " + stage + " " + p.EventTurn + " " + p.EventID );
            }

            data[ stage ].TEVT[ i ] = p;
        }
    }

    void loadMEVT( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";

        string pathMEVT = pathDir + "MEVT_" + path + ".DAT";

        FileStream fs = File.OpenRead( pathMEVT );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        int count = bytes.Length / 14;
        data[ stage ].MEVT = new GameBattleMEVT[ count ];

        for ( int i = 0 ; i < count ; i++ )
        {
            GameBattleMEVT p = new GameBattleMEVT();
            p.EventType = (GameBattleMapEventType)BitConverter.ToInt16( bytes , index ); index += 2;
            p.EventParm1 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.EventParm2 = BitConverter.ToInt16( bytes , index ); index += 2;

            p.ItemGetType = (GameBattleMapEventType)BitConverter.ToInt16( bytes , index ); index += 2;
            p.ItemGetParm1 = BitConverter.ToInt16( bytes , index ); index += 2;
            p.ItemGetParm2 = BitConverter.ToInt16( bytes , index ); index += 2;

            p.MapShow = BitConverter.ToInt16( bytes , index ); index += 2;

            data[ stage ].MEVT[ i ] = p;

            if ( p.EventType == GameBattleMapEventType.Event )
            {
                Debug.LogWarning( "stage EventType " + stage  + " " + p.EventParm1 + " " + p.EventParm2 );
            }
        }
    }


    void loadLayer( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Stage/Stage" + path + "/";
        string pathLayer = pathDir + "LAYER_" + path + ".DAT";

        GameBattleLayer layer = data[ stage ].Layer;

        FileStream fs = File.OpenRead( pathLayer );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        short count0 = BitConverter.ToInt16( bytes , index ); index += 2;
        short count1 = BitConverter.ToInt16( bytes , index ); index += 2;

        layer.L0 = new GameBattleLayer.Layer0[ count0 - 1 ];
        layer.L1 = new GameBattleLayer.Layer1[ count1 ];

        index += 60;

        string log = "stage l0 " + stage + "  " ;

        for ( int i = 0 ; i < count0 - 1 ; ++i )
        {
            layer.L0[ i ] = new GameBattleLayer.Layer0();

            layer.L0[ i ].ID = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow0 = BitConverter.ToInt32( bytes , index ); index += 4;
            layer.L0[ i ].Unknow1 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow2 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow21 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow3 = BitConverter.ToInt32( bytes , index ); index += 4;

            layer.L0[ i ].Unknow4 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow5 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Lyaer = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow61 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow7 = BitConverter.ToInt32( bytes , index ); index += 4;
            layer.L0[ i ].Unknow8 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow9 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow10 = BitConverter.ToInt16( bytes , index ); index += 2;

            layer.L0[ i ].Name = Encoding.Default.GetString( bytes , index , 13 ).Replace( ".mpl" , "" ).TrimEnd( '\0' ); index += 26;

            if ( layer.L0[ i ].Unknow4 != 10 && layer.L0[ i ].Unknow5 != 8 )
            {
                layer.L0[ i ].ID = layer.L0[ i - 1 ].ID;
            }

            log += layer.L0[ i ].ID + " ";


            string pathDir1 = "E:/Swordmancn/Stage/Stage" + path + "/";
            string pathTexture1 = pathDir1 + layer.L0[ i ].Name + ".mpl";

            try
            {
                FileStream fs1 = File.OpenRead( pathTexture1 );

                byte[] bytes1 = new byte[ 16 ];

                fs1.Read( bytes1 , 0 , 16 );
                fs1.Close();

                layer.L0[ i ].Width = BitConverter.ToInt16( bytes1 , 7 );
                layer.L0[ i ].Height = BitConverter.ToInt16( bytes1 , 9 );
            }
            catch ( Exception e )
            {
            }
        }

        if ( count0 - 1 > 1 )
        {
            Debug.Log( log );
        }

        int size0 = 64 + ( count0 - 1 ) * 60;

        for ( int i = 0 ; i < count1 ; ++i )
        {
            layer.L1[ i ] = new GameBattleLayer.Layer1();

            layer.L1[ i ].ID = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Parm = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].OffsetX = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].OffsetY = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].ParmEffect = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Unknow4 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Pause = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Delay = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].VisibleBattle = BitConverter.ToInt16( bytes , index ); index += 2;
            
            layer.L1[ i ].Name = Encoding.Default.GetString( bytes , index , 13 ).Replace( ".saf" , "" ); index += 13;
        }

    }

    public void load( int stage )
    {
        clear();

        data[ stage ] = new GameBattleStage();

        loadLayer( stage );
        loadEMSG( stage );
        loadSDESData( stage );
        loadJudgeData( stage );
        loadTEVT( stage );
        loadMEVT( stage );
        loadMan( stage );
        loadXY( stage );
        loadDTL( stage );
        loadEvent( stage );


        Debug.Log( "GameBattleData loaded. " + stage );
    }


    public void saveText()
    {
        string strHelp = "";

        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameBattleStage stage = data[ i ];

            strHelp += stage.SDES.Title + "\r\n";
            strHelp += "胜利条件：" + stage.SDES.Win + "\r\n";
            strHelp += "失败条件：" + stage.SDES.Lose + "\r\n";
            strHelp += "熟练度：\r\n";
            strHelp += "过关要点：\r\n";
            strHelp += "城镇场景：\r\n\r\n";
        }

        File.WriteAllText( Application.dataPath + "/Objects/Help/Help40.txt" , strHelp , Encoding.UTF8 );
    }


}
