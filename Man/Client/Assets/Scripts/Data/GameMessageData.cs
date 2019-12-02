using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;




[System.Serializable]
public class GameMessageString
{
    public string[] MsgT;
    public string[] MsgS;

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
public class GameMessage
{
    public GameMessageString[] message;
}

public enum GameMessageType
{
    Common = 0,
    Get0,
    LevelUp,
    Get1,
    Get2,
}

public class GameMessageData : Singleton< GameMessageData >
{
    [SerializeField]
    List<GameMessage> data;
    
    public string getExp( int e , bool lv )
    {
        string str = data[ (int)GameMessageType.LevelUp ].message[ lv ? 1 : 0 ][ 0 ];

        str = str.Replace( "0" , e.ToString() );

        return str;
    }

    public GameMessage getData( GameMessageType t )
    {
        return data[ (int)t ];
    }

    public void clear()
    {
        data.Clear();
    }

    public void load( string path )
    {
        FileStream fs = File.OpenRead( path );
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

        GameMessage message = new GameMessage();
        data.Add( message );

        message.message = new GameMessageString[ c ];

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

            message.message[ ii ] = new GameMessageString();
            message.message[ ii ].MsgT = new string[ count[ ii ] ];
            message.message[ ii ].MsgS = new string[ count[ ii ] ];

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

                        if ( l2 == 1 )
                        {
                            int cccc = 0;

                            while ( bytes[ j + cccc ] == 0 )
                            {
                                cccc++;
                            }

                            j += cccc;
                            l1 = j;
                            str += "0";
                            continue;
                        }

                        string m = Encoding.GetEncoding( "big5" ).GetString( bytes , l1 , l2 );
                        str += m;
//                        l1 = j + 2;

                        int n = 1;
                        while ( true )
                        {
                            if ( bytes[ j + n ] == 0 )
                            {
                                n++;
                            }
                            else if( bytes[ j + n ] == 1 )
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

                str = str.Replace( "00" , "0" );

                message.message[ ii ].MsgT[ i ] = str;
                message.message[ ii ].MsgS[ i ] = ChineseStringUtility.ToSimplified( str );
            }

        }


        Debug.Log( "GameMessageData loaded." );
    }



}
