using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GameCampScript
{
    public short Camp = GameDefine.INVALID_ID;
    public short Town = GameDefine.INVALID_ID;
    public short TownPos = 0;
}


public class GameCampData : Singleton<GameCampData>
{
    [SerializeField]
    GameCampScript[] data = new GameCampScript[ GameDefine.MAX_STAGE ];


    public void clear()
    {

    }

    public GameCampScript getData( int id )
    {
        if ( id < 0 || data.Length <= id )
        {
            return null;
        }

        return data[ id ];
    }

    public void loadScript( int stage )
    {
        string path = stage < 10 ? "0" + stage.ToString() : stage.ToString();
        string pathName = Application.dataPath + "/Objects/DAT/Camp/Script/CAMP" + path + ".dat";

        if ( !File.Exists( pathName ) )
        {
            return;
        }

        FileStream fs = File.OpenRead( pathName );
        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );
        fs.Close();

        int index = 0;

        data[ stage ] = new GameCampScript();
        data[ stage ].Camp = BitConverter.ToInt16( bytes , index ); index += 2;
        data[ stage ].Town = BitConverter.ToInt16( bytes , index ); index += 2;
        data[ stage ].TownPos = BitConverter.ToInt16( bytes , index ); index += 2;
    }



}
