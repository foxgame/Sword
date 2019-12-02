using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip; 
using ICSharpCode.SharpZipLib.Zip; 
using System.IO;
using System;


[System.Serializable]
public class GamePointTset : MonoBehaviour
{
    public byte Value0;
    public byte Value1;
    public short Value2;
}

public enum GameUserDataType
{
    LastAttackID = 0x64,
    LastTargetID = 0x65,
    MapEvent6 = 0x66,


    Count
}

public class GameDefine
{
    public const int MAX_SAVE = 5;

    public const int MAX_STAGE = 36;
    public const int MAX_RPG_STAGE = 27;

    public const int MAX_SLOT = 10;
    public const int MAX_SLOT_HALF = 5;

    public const int INVALID_ID = -1;

    public const int LAYER_HEIGHT = 5000;


    public const int TEXTURE_WIDTH = 30;
    public const int TEXTURE_HEIGHT = 24;

    public const int TEXTURE_WIDTH_HALF = 15;
    public const int TEXTURE_HEIGHT_HALF = 12;

    public const int MAX_EXP = 500;

    public const int SCENE_WIDTH = 320;
    public const int SCENE_HEIGHT = 200;

    public const int BATTLE_OFFSET_X = -142;
    public const int BATTLE_OFFSET_Y = 106;

    public const int MAX_USER = 10;

    public const int MAX_GAMEDATA = 256;

    public const int MAX_ITEMBAG = 512;

    public const int MAX_RPG_SHOP_SLOT = 24;

    public const int MAX_POWER = 100;

    public const int MAX_LEVEL = 99;


    public static float Difficulty
    {
        get
        {
            //             if ( GameUserData.instance.Stage == 35 )
            //             {
            //                 return 1.0f;
            //             }

            float diff = GameUserData.instance.Proficiency / 60.0f + 0.8f;

            if ( GameSetting.instance.mode == GameSetting.GameMode.Normal )
            {
                return diff;
            }
            else
            {
                diff += 0.1f;

                if ( diff < 1.0f )
                {
                    diff = 1.0f;
                }

                return diff;
            }
        }
    }


    public static Color getRGB888( short rgb555 , bool alphaminx )
    {
        int n0, n1, n2, n3, n4, n5, n6, n7;
        int n8, n9, n10, n11, n12, n13, n14, n15;

        n0 = ( rgb555 & 0x01 ) == 0x01 ? 1 : 0;
        n1 = ( rgb555 & 0x02 ) == 0x02 ? 1 : 0;
        n2 = ( rgb555 & 0x04 ) == 0x04 ? 1 : 0;
        n3 = ( rgb555 & 0x08 ) == 0x08 ? 1 : 0;
        n4 = ( rgb555 & 0x10 ) == 0x10 ? 1 : 0;
        n5 = ( rgb555 & 0x20 ) == 0x20 ? 1 : 0;
        n6 = ( rgb555 & 0x40 ) == 0x40 ? 1 : 0;
        n7 = ( rgb555 & 0x80 ) == 0x80 ? 1 : 0;

        n8 = ( rgb555 & 0x0100 ) == 0x0100 ? 1 : 0;
        n9 = ( rgb555 & 0x0200 ) == 0x0200 ? 1 : 0;
        n10 = ( rgb555 & 0x0400 ) == 0x0400 ? 1 : 0;
        n11 = ( rgb555 & 0x0800 ) == 0x0800 ? 1 : 0;
        n12 = ( rgb555 & 0x1000 ) == 0x1000 ? 1 : 0;
        n13 = ( rgb555 & 0x2000 ) == 0x2000 ? 1 : 0;
        n14 = ( rgb555 & 0x4000 ) == 0x4000 ? 1 : 0;
        n15 = ( rgb555 & 0x8000 ) == 0x8000 ? 1 : 0;

        byte r = (byte)( ( n14 << 7 ) + ( n13 << 6 ) + ( n12 << 5 ) + ( n11 << 4 ) + ( n10 << 3 ) );
        byte g = (byte)( ( n9 << 7 ) + ( n8 << 6 ) + ( n7 << 5 ) + ( n6 << 4 ) + ( n5 << 3 ) );
        byte b = (byte)( ( n4 << 7 ) + ( n3 << 6 ) + ( n2 << 5 ) + ( n1 << 4 ) + ( n0 << 3 ) );
        byte a = 0;

        if ( alphaminx )
        {
            byte max = r;
            if ( max > g )
                max = g;
            if ( max > b )
                max = b;

            a = max;
        }
        else
        {
            if ( r > 0 || g > 0 || b > 0 )
            {
                a = 255;
            }
        }

        return new Color( r / 255.0f , g / 255.0f , b / 255.0f , a / 255.0f );
    }

    public static float getZ( int z )
    {
        int fz = 10000000 - z;

        while ( fz < -1000 || fz > 1000 )
        {
            fz /= 10;
        }

        return fz;
    }

    public static float getTime( int s )
    {
        // use 24fps ?

        if ( s <= 0 )
        {
            s = 1;
        }

        return s / 25.0f;
    }

    public static float getTimeBlack( int s )
    {
        // use 24fps ?

        if ( s <= 0 )
        {
            s = 1;
        }
        else if ( s > 10 )
        {
            s = 10;
        }

#if UNITY_EDITOR
        Debug.Log( "getTimeBlack" + s );
#endif

        return s;
    }

    public static float getTimeWhite( int s )
    {
        // use 24fps ?

        if ( s <= 0 )
        {
            s = 1;
        }
        else if ( s > 10 )
        {
            s = 10;
        }

#if UNITY_EDITOR
        Debug.Log( "getTimeWhite" + s );
#endif

        return s;
    }

    public static void DestroyAll( Transform trans )
    {
        while ( trans.childCount > 0 )
        {
            GameObject obj = trans.GetChild( 0 ).gameObject;

            GameObject.Destroy( obj );
            obj.transform.SetParent( null );
        }
    }

    public static GameAnimationDirection getDirection( int x1 , int y1 , int x2 , int y2 )
    {
        if ( x1 > x2 )
        {
            return GameAnimationDirection.West;
        }
        else if ( x1 < x2 )
        {
            return GameAnimationDirection.East;
        }
        else if ( y1 > y2 )
        {
            return GameAnimationDirection.North;
        }
        else if ( y1 < y2 )
        {
            return GameAnimationDirection.South;
        }

        return GameAnimationDirection.Count;
    }

    public static GameBattleAttackMapDirection getDirectionMap( int x1 , int y1 , int x2 , int y2 )
    {
        if ( x1 > x2 )
        {
            return GameBattleAttackMapDirection.West;
        }
        else if ( x1 < x2 )
        {
            return GameBattleAttackMapDirection.East;
        }
        else if ( y1 > y2 )
        {
            return GameBattleAttackMapDirection.North;
        }
        else if ( y1 < y2 )
        {
            return GameBattleAttackMapDirection.South;
        }

        return GameBattleAttackMapDirection.Count;
    }


    public static string getBigInt( string str , bool b = false )
    {
        str = str.Replace( "-" , "" );
        str = str.Replace( "+" , "" );

        str = str.Replace( "1" , "一" );
        str = str.Replace( "2" , "二" );
        str = str.Replace( "3" , "三" );
        str = str.Replace( "4" , "四" );
        str = str.Replace( "5" , "五" );
        str = str.Replace( "6" , "六" );
        str = str.Replace( "7" , "七" );
        str = str.Replace( "8" , "八" );
        str = str.Replace( "9" , "九" );
        str = str.Replace( "0" , "〇" );

        if ( b && str.Length > 4 )
        {
            str = "？？？？";
        }

        return str;
    }

    public static int getMoveSpeed( int s )
    {
        s = 150 - s * 25;

        if ( s < 35 )
        {
            s = 35;
        }

        return s;
    }

    public static int getSpeed( int s )
    {
        return s * 5;
    }

    public static int getBattleX( int x )
    {
        return x * TEXTURE_WIDTH;
    }
    public static int getBattleY( int y )
    {
        return -y * TEXTURE_HEIGHT;
    }

    public static int getBattleXBound( int x )
    {
        int rx = x * TEXTURE_WIDTH;

        if ( rx <= TEXTURE_WIDTH )
        {
            rx = TEXTURE_WIDTH;
        }

        if ( rx >= GameCameraManager.instance.MapWidth - TEXTURE_WIDTH * 2 )
        {
            rx = (int)( GameCameraManager.instance.MapWidth - TEXTURE_WIDTH * 2 );
        }

        return rx;
    }
    public static int getBattleYBound( int y )
    {
        int ry = -y * TEXTURE_HEIGHT;

        if ( ry >= -TEXTURE_HEIGHT )
        {
            ry = -TEXTURE_HEIGHT;
        }

        if ( -ry >= GameCameraManager.instance.MapHeight + TEXTURE_HEIGHT * 2 )
        {
            ry = (int)-( GameCameraManager.instance.MapHeight + TEXTURE_HEIGHT * 2 );
        }

        return ry;
    }

    public static Rect getSceneRect( float x , float y )
    {
        return new Rect( x + TEXTURE_WIDTH ,
            -y + TEXTURE_HEIGHT ,
            SCENE_WIDTH - TEXTURE_WIDTH * 2 ,
            SCENE_HEIGHT - TEXTURE_HEIGHT * 2 );
    }

    public static Rect getRPGSceneRect( float x , float y )
    {
        return new Rect( x + TEXTURE_WIDTH ,
            -y + TEXTURE_HEIGHT ,
            TEXTURE_WIDTH * 2 ,
            TEXTURE_HEIGHT * 2 );
    }

    public static float getRPGSceneXMin( float x )
    {
        return ( x - TEXTURE_WIDTH );
    }

    public static float getRPGSceneXMax( float x )
    {
        return ( x + TEXTURE_WIDTH );
    }

    public static float getRPGSceneYMin( float y )
    {
        return -( y - TEXTURE_HEIGHT );
    }

    public static float getRPGSceneYMax( float y )
    {
        return -( y + TEXTURE_HEIGHT );
    }

    public static float getSceneXMin( float x )
    {
        return ( x - TEXTURE_WIDTH );
    }

    public static float getSceneXMax( float x )
    {
        return ( x - SCENE_WIDTH + TEXTURE_WIDTH );
    }

    public static float getSceneYMin( float y )
    {
        return -( y - TEXTURE_HEIGHT );
    }

    public static float getSceneYMax( float y )
    {
        return -( y - SCENE_HEIGHT + TEXTURE_HEIGHT + TEXTURE_HEIGHT );
    }

    public static int getXFormBattle( int x )
    {
        return x / TEXTURE_WIDTH;
    }
    public static int getYFormBattle( int y )
    {
        return -y / TEXTURE_HEIGHT;
    }

    public static int getX( int x )
    {
        return x * TEXTURE_WIDTH;
    }
    public static int getY( int y )
    {
        return y * TEXTURE_HEIGHT;
    }

    public static string getString2( int id )
    {
        return String.Format( "{0:D2}", id );
    }

    public static string getString3( int id )
    {
        return String.Format( "{0:D3}" , id );
    }

    public static byte[] Compress( byte[] bytesToCompress )
    {
        byte[] rebyte = null;
        MemoryStream ms = new MemoryStream();

        GZipOutputStream s = new GZipOutputStream( ms );

        try
        {
            s.Write( bytesToCompress , 0 , bytesToCompress.Length );
            s.Flush();
            s.Finish();
        }
        catch ( System.Exception ex )
        {
#if UNITY_EDITOR
            Debug.Log( ex );
#endif
        }

        ms.Seek( 0 , SeekOrigin.Begin );

        rebyte = ms.ToArray();

        s.Close();
        ms.Close();

        s.Dispose();
        ms.Dispose();

        return rebyte;
    }

    public static byte[] DeCompress( byte[] bytesToDeCompress )
    {
        byte[] rebyte = new byte[ bytesToDeCompress.Length * 20 ];

        MemoryStream ms = new MemoryStream( bytesToDeCompress );
        MemoryStream outStream = new MemoryStream();

        GZipInputStream s = new GZipInputStream( ms );

        int read = s.Read( rebyte , 0 , rebyte.Length );
        while ( read > 0 )
        {
            outStream.Write( rebyte , 0 , read );
            read = s.Read( rebyte , 0 , rebyte.Length );
        }

        byte[] rebyte1 = outStream.ToArray();

        ms.Close();
        s.Close();
        outStream.Close();

        ms.Dispose();
        s.Dispose();
        outStream.Dispose();

        bytesToDeCompress = null;
        rebyte = null;

        return rebyte1;
    }



}
