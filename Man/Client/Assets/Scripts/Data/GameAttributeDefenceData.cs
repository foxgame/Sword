using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


[System.Serializable]
public class GameAttributeDefence
{
    public short ID;
    public short[] AttributeDefence = new short[ (int)GameAttributeType.Count ];
}


public class GameAttributeDefenceData : Singleton< GameAttributeDefenceData >
{
    [SerializeField]
    GameAttributeDefence[] data;
    
    public GameAttributeDefence getData( int id )
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

        data = new GameAttributeDefence[ bytes.Length / 14 ];

        int index = 0;
        for ( int i = 0 ; i < data.Length ; ++i )
        {
            GameAttributeDefence attribute = new GameAttributeDefence();
           
            for ( int j = 0 ; j <= (int)GameAttributeType.Dark ; j++ )
            {
                attribute.AttributeDefence[ j ] = BitConverter.ToInt16( bytes , index ); index += 2;
            }

            // bug ?
            attribute.ID = BitConverter.ToInt16( bytes , index ); index += 2;

            data[ i ] = attribute;
        }

        Debug.Log( "GameAttributeDefence loaded." );
    }



}
