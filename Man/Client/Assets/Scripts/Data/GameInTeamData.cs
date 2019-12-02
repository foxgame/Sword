using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class GameInTeamData : Singleton<GameInTeamData>
{
    [System.Serializable]
    public class Data
    {
        public short[] data;
    }

    [SerializeField]
    Data[] data = new Data[ GameDefine.MAX_STAGE ];

    public Data getData( int i )
    {
        return data[ i ];
    }

    public override void initSingleton()
    {
        data[ 0 ] = new Data();
        data[ 0 ].data = new short[ 1 ];
        data[ 0 ].data[ 0 ] = 0;

        data[ 1 ].data = new short[ 1 ];
        data[ 1 ].data[ 0 ] = 1;
    }




}

