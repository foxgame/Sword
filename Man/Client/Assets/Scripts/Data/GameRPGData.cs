using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameRPGShopType
{
    Weapon = 0,
    Accessory,
    Other,

    Count
}


[System.Serializable]
public class GameRPGEvent
{
    [System.Serializable]
    public class RPGEvent
    {
        public int Type;
        public byte[] Data;
    }

    public RPGEvent[] Event;
}

[System.Serializable]
public class GameRPGEMSG
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
public class GameRPGLayer
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
        public short Unknow6;
        public short Unknow61;
        public int Unknow7;
        public short Unknow8;
        public short Unknow9;
        public short Unknow10;
        public string Name;
    }

    [System.Serializable]
    public class Layer1
    {
        public short ID;
        public short Parm;
        public short OffsetX;
        public short OffsetY;

        public short Unknow3;
        public short Unknow4;
        public short Unknow5;
        public short play;
        public short Unknow7;

        public string Name;
    };

    public Layer0[] L0;
    public Layer1[] L1;
}

[System.Serializable]
public class GameRPGDTL
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
        public byte MapEvent;
        public short RPGEvent;
    }

    public Point[] Points;
}


[System.Serializable]
public class GameRPGShopInfo
{
    public GameRPGShopType type;
    public short shop;
    public short[] item;
}

[System.Serializable]
public class GameRPGPosInfo
{
    public short Music;
    public short MapPosX;
    public short MapPosY;
    public short PosX;
    public short PosY;
    public short Direction;
}

[System.Serializable]
public class GameRPGInfo
{
    public GameRPGPosInfo[] Pos;
    public GameRPGShopInfo[] Shop;
}

[System.Serializable]
public class GameRPGStage
{
    public GameRPGDTL DTL = new GameRPGDTL();
    public GameRPGLayer Layer = new GameRPGLayer();
    public GameRPGEvent[] Event;
    public GameRPGEMSG[] EMSG;
    public GameRPGInfo Info = new GameRPGInfo();
}


public class GameRPGData : Singleton<GameRPGData>
{
    [SerializeField]
    GameRPGStage[] data = new GameRPGStage[ GameDefine.MAX_RPG_STAGE ];

    public void clear()
    {

    }

    public GameRPGStage getData( int id )
    {
        if ( id < 0 || data.Length <= id )
        {
            return null;
        }

        return data[ id ];
    }

    void loadEvent( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Rpg/Rpg" + path + "/";

        string pathEMSG = pathDir + "EVENT_" + path + ".COD";

        FileStream fs = File.OpenRead( pathEMSG );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();


        int index = 0;
        int count = BitConverter.ToInt32( bytes , index ); index += 4;
        int size = BitConverter.ToInt32( bytes , index ); index += 4;

        data[ stage ].Event = new GameRPGEvent[ count ];

        for ( int i = 0 ; i < count ; i++ )
        {
            data[ stage ].Event[ i ] = new GameRPGEvent();

            int pos1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int count1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int unknow1 = BitConverter.ToInt32( bytes , index ); index += 4;
            int size1 = BitConverter.ToInt32( bytes , index ); index += 4;

            data[ stage ].Event[ i ].Event = new GameRPGEvent.RPGEvent[ count1 ];

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
                data[ stage ].Event[ i ].Event[ j ] = new GameRPGEvent.RPGEvent();
                data[ stage ].Event[ i ].Event[ j ].Data = new byte[ size2[ j ] - 4 ];

                int l0 = BitConverter.ToInt32( bytes , pos1 + j * 4 );
                data[ stage ].Event[ i ].Event[ j ].Type = BitConverter.ToInt32( bytes , l0 );

                switch ( data[ stage ].Event[ i ].Event[ j ].Type )
                {
                    case 0x09:
                        // talk
                        break;
                   
                    case 0x20:
                        // check game data
                        break;
                    case 0x26:
                        // change scene
                        break;
                    case 0x27:
                        // shop
                        break;
                    case 0x28:
                        // nextStage
                        break;
                    case 0x29:
                        // camp
                        break;

                    case 0x22:
                        //  1 5 1 
                        break;
                    case 0x2E:
                        //  1 5 2 
                        break;
                    case 0x2D:
                        //  1 5 4 
                        break;
                    case 0x2C:
                        //  1 12 5 
                        break;
                    case 0x23:
                        //  1 12 18 
                        break;
                    case 0x10:
                        //  1 12 21 
                        break;
                    case 0x31:
                        //  1 12 17 
                        break;
                    case 0x21:
                        //  4 21 0
                        break;
                    case 0x24:
                        //  5 34 2    7 5 2
                        break;
                    case 0x06:
                        // 7 5 3
                        break;
                    case 0x04:
                        // 7 5 4
                        break;
                    case 0x1D:
                        // 7 5 5
                        break;
                    case 0x1A:
                        // 7 5 6
                        break;
                    case 0x1C:
                        // 7 5 7
                        break;
                    case 0x15:
                        // 7 5 8
                        break;
                    case 0x18:
                        // 7 5 10
                        break;
                    case 0x07:
                        // 7 5 11
                        break;
                    case 0x02:
                        // 7 5 12
                        break;
                    case 0x01:
                        // 7 5 14
                        break;
                    case 0x1B:
                        // 7 5 35
                        break;
                    case 0x16:
                        // 7 5 37
                        break;
                    case 0x1E:
                        // 7 5 36
                        break;
                    case 0x2B:
                        // 7 34 1
                        break;

                    case 0x2F:
                        // 17 63 5
                        break;

                    case 0x30:
                        // 23 1 6
                        break;
                    case 0x33:
                        // 23 1 14
                        break;
                    case 0x32:
                        // 23 15 20
                        break;


                    case 0:
                        // none
                        break;
                    default:
                        {
                            Debug.LogError( "event type: " + stage + " " + i + " " + j + " " + String.Format( "{0:X}" , data[ stage ].Event[ i ].Event[ j ].Type ) );
                        }
                        break;
                }

                for ( int k = 0 ; k < size2[ j ] - 4 ; k++ )
                {
                    data[ stage ].Event[ i ].Event[ j ].Data[ k ] = bytes[ l0 + k + 4 ];
                }
            }
        }

    }


    void loadEMSG( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Rpg/Rpg" + path + "/";

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

        data[ stage ].EMSG = new GameRPGEMSG[ c ];

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

            data[ stage ].EMSG[ ii ] = new GameRPGEMSG();
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
                        l1 = j + 2;
                    }
                }

                data[ stage ].EMSG[ ii ].MsgT[ i ] = str;
                data[ stage ].EMSG[ ii ].MsgS[ i ] = ChineseStringUtility.ToSimplified( str );
            }

        }
    }


    void loadDTL( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Rpg/Rpg" + path + "/";

        string pathDTL = pathDir + "map" + path + ".dtl";

        FileStream fs = File.OpenRead( pathDTL );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        GameRPGDTL dtl = data[ stage ].DTL;

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

        dtl.Points = new GameRPGDTL.Point[ dtl.Height * dtl.Width ];

        int ii = 0;
        for ( int i = 0 ; i < dtl.Height ; i++ )
        {
            for ( int j = 0 ; j < dtl.Width ; j++ )
            {
                dtl.Points[ ii ] = new GameRPGDTL.Point();
                dtl.Points[ ii ].Move = bytes[ index ]; index += 1;
                dtl.Points[ ii ].MapEvent = bytes[ index ]; index += 1;
                dtl.Points[ ii ].RPGEvent = BitConverter.ToInt16( bytes , index ); index += 2;


                ii++;
            }
        }

    }


    void loadLayer( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Rpg/Rpg" + path + "/";
        string pathLayer = pathDir + "LAYER_" + path + ".DAT";

        GameRPGLayer layer = data[ stage ].Layer;

        FileStream fs = File.OpenRead( pathLayer );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;
        short count0 = BitConverter.ToInt16( bytes , index ); index += 2;
        short count1 = BitConverter.ToInt16( bytes , index ); index += 2;

        layer.L0 = new GameRPGLayer.Layer0[ count0 - 1 ];
        layer.L1 = new GameRPGLayer.Layer1[ count1 ];

        index += 60;

        for ( int i = 0 ; i < count0 - 1 ; ++i )
        {
            layer.L0[ i ] = new GameRPGLayer.Layer0();

            layer.L0[ i ].ID = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow0 = BitConverter.ToInt32( bytes , index ); index += 4;
            layer.L0[ i ].Unknow1 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow2 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow21 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow3 = BitConverter.ToInt32( bytes , index ); index += 4;

            layer.L0[ i ].Unknow4 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow5 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow6 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow61 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow7 = BitConverter.ToInt32( bytes , index ); index += 4;
            layer.L0[ i ].Unknow8 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow9 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L0[ i ].Unknow10 = BitConverter.ToInt16( bytes , index ); index += 2;

            layer.L0[ i ].Name = Encoding.Default.GetString( bytes , index , 13 ).Replace( ".mpl" , "" ); index += 26;
        }

        if ( stage == 1 ||
            stage == 3 )
        {
            layer.L0[ 0 ].Name = "map00";
        }

        if ( stage == 4 )
        {
            layer.L0[ 0 ].Name = "map02";
        }

        if ( stage == 7 || stage == 9 )
        {
            layer.L0[ 0 ].Name = "map05";
        }

        if ( stage == 8 || stage == 10 )
        {
            layer.L0[ 0 ].Name = "map06";
        }

        if ( stage == 13 || stage == 15 )
        {
            layer.L0[ 0 ].Name = "map11";
        }

        if ( stage == 14 || stage == 16 )
        {
            layer.L0[ 0 ].Name = "map12";
        }

        if ( stage == 19 || stage == 21 || stage == 23 )
        {
            layer.L0[ 0 ].Name = "map17";
        }

        if ( stage == 22 || stage == 20 || stage == 24 || stage == 25 )
        {
            layer.L0[ 0 ].Name = "map18";
        }


        int size0 = 64 + ( count0 - 1 ) * 60;

        for ( int i = 0 ; i < count1 ; ++i )
        {
            layer.L1[ i ] = new GameRPGLayer.Layer1();

            layer.L1[ i ].ID = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Parm = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].OffsetX = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].OffsetY = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Unknow3 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Unknow4 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Unknow5 = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].play = BitConverter.ToInt16( bytes , index ); index += 2;
            layer.L1[ i ].Unknow7 = BitConverter.ToInt16( bytes , index ); index += 2;

            layer.L1[ i ].Name = Encoding.Default.GetString( bytes , index , 13 ).Replace( ".saf" , "" ); index += 13;
        }
    }

    void loadInfo( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathDir = Application.dataPath + "/Objects/DAT/Rpg/Rpg" + path + "/";
        string pathLayer = pathDir + "RPG_" + path + ".DAT";

        GameRPGInfo info = data[ stage ].Info;

        info.Shop = new GameRPGShopInfo[ (int)GameRPGShopType.Count ];
        info.Pos = new GameRPGPosInfo[ 2 ];

        FileStream fs = File.OpenRead( pathLayer );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;

        info.Pos[ 0 ] = new GameRPGPosInfo();
        info.Pos[ 0 ].Music = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 0 ].MapPosX = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 0 ].MapPosY = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 0 ].PosX = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 0 ].PosY = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 0 ].Direction = BitConverter.ToInt16( bytes , index ); index += 2;

        if ( bytes.Length <= index )
        {
            return;
        }

        info.Pos[ 1 ] = new GameRPGPosInfo();
        info.Pos[ 1 ].Music = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 1 ].MapPosX = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 1 ].MapPosY = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 1 ].PosX = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 1 ].PosY = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Pos[ 1 ].Direction = BitConverter.ToInt16( bytes , index ); index += 2;

        if ( bytes.Length <= index )
        {
            return;
        }

        info.Shop[ 0 ] = new GameRPGShopInfo();
        info.Shop[ 0 ].item = new short[ GameDefine.MAX_RPG_SHOP_SLOT ];
        for ( int j = 0 ; j < GameDefine.MAX_RPG_SHOP_SLOT ; j++ )
        {
            info.Shop[ 0 ].item[ j ] = GameDefine.INVALID_ID;
        }

        short type = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Shop[ 0 ].type = (GameRPGShopType)type;
        info.Shop[ 0 ].shop = BitConverter.ToInt16( bytes , index ); index += 2;
        for ( int i = 0 ; i < GameDefine.MAX_RPG_SHOP_SLOT ; i++ )
        {
            info.Shop[ 0 ].item[ i ] = BitConverter.ToInt16( bytes , index ); index += 2;
        }

        if ( bytes.Length <= index )
        {
            return;
        }

        info.Shop[ 1 ] = new GameRPGShopInfo();
        info.Shop[ 1 ].item = new short[ GameDefine.MAX_RPG_SHOP_SLOT ];
        for ( int j = 0 ; j < GameDefine.MAX_RPG_SHOP_SLOT ; j++ )
        {
            info.Shop[ 1 ].item[ j ] = GameDefine.INVALID_ID;
        }

        type = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Shop[ 1 ].type = (GameRPGShopType)type;
        info.Shop[ 1 ].shop = BitConverter.ToInt16( bytes , index ); index += 2;
        for ( int i = 0 ; i < GameDefine.MAX_RPG_SHOP_SLOT ; i++ )
        {
            info.Shop[ 1 ].item[ i ] = BitConverter.ToInt16( bytes , index ); index += 2;
        }

        if ( bytes.Length <= index )
        {
            return;
        }

        info.Shop[ 2 ] = new GameRPGShopInfo();
        info.Shop[ 2 ].item = new short[ GameDefine.MAX_RPG_SHOP_SLOT ];
        for ( int j = 0 ; j < GameDefine.MAX_RPG_SHOP_SLOT ; j++ )
        {
            info.Shop[ 2 ].item[ j ] = GameDefine.INVALID_ID;
        }

        type = BitConverter.ToInt16( bytes , index ); index += 2;
        info.Shop[ 2 ].type = (GameRPGShopType)type;
        info.Shop[ 2 ].shop = BitConverter.ToInt16( bytes , index ); index += 2;
        for ( int i = 0 ; i < GameDefine.MAX_RPG_SHOP_SLOT ; i++ )
        {
            info.Shop[ 2 ].item[ i ] = BitConverter.ToInt16( bytes , index ); index += 2;
        }

        if ( bytes.Length <= index )
        {
            return;
        }

        Debug.Log( stage );
    }


    public void load( int stage )
    {
        clear();

        data[ stage ] = new GameRPGStage();

        loadLayer( stage );
        loadEMSG( stage );
        loadDTL( stage );
        loadEvent( stage );
        loadInfo( stage );

        Debug.Log( "GameRPGData loaded. " + stage );
    }



}
