using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GameUnitMove
{
    public byte baseCost = 0;
    public byte block = 0;
    public bool fly = false;
    public bool addMove = false;
    public sbyte subMove = 0;
}


public class GameUnitMoveTypeData : Singleton<GameUnitMoveTypeData>
{
    [SerializeField]
    GameUnitMove[] data = new GameUnitMove[ (int)GameUnitMoveType.Count ];


    public void clear()
    {

    }

    public GameUnitMove getData( GameUnitMoveType id )
    {
        if ( id == GameUnitMoveType.Invalid )
        {
            return data[ (int)GameUnitMoveType.None ];
        }

        return data[ (int)id ];
    }

    public void load()
    {
        data[ (int)GameUnitMoveType.Walk0 ].baseCost = 5;
        data[ (int)GameUnitMoveType.Walk0 ].block = 7;
        data[ (int)GameUnitMoveType.Walk0 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk0 ].fly = false;
        data[ (int)GameUnitMoveType.Walk0 ].subMove = 0;

        data[ (int)GameUnitMoveType.Walk1 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Walk1 ].block = 7;
        data[ (int)GameUnitMoveType.Walk1 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk1 ].fly = false;
        data[ (int)GameUnitMoveType.Walk1 ].subMove = 0;

        data[ (int)GameUnitMoveType.Walk2 ].baseCost = 5;
        data[ (int)GameUnitMoveType.Walk2 ].block = 7;
        data[ (int)GameUnitMoveType.Walk2 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk2 ].fly = false;
        data[ (int)GameUnitMoveType.Walk2 ].subMove = 0;

        data[ (int)GameUnitMoveType.Walk3 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Walk3 ].block = 7;
        data[ (int)GameUnitMoveType.Walk3 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk3 ].fly = false;
        data[ (int)GameUnitMoveType.Walk3 ].subMove = 1;

        data[ (int)GameUnitMoveType.Fly4 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Fly4 ].block = 7;
        data[ (int)GameUnitMoveType.Fly4 ].addMove = false;
        data[ (int)GameUnitMoveType.Fly4 ].fly = true;
        data[ (int)GameUnitMoveType.Fly4 ].subMove = 0;

        data[ (int)GameUnitMoveType.Fly5 ].baseCost = 7;
        data[ (int)GameUnitMoveType.Fly5 ].block = 9;
        data[ (int)GameUnitMoveType.Fly5 ].addMove = false;
        data[ (int)GameUnitMoveType.Fly5 ].fly = true;
        data[ (int)GameUnitMoveType.Fly5 ].subMove = 0;

        data[ (int)GameUnitMoveType.Fly6 ].baseCost = 5;
        data[ (int)GameUnitMoveType.Fly6 ].block = 9;
        data[ (int)GameUnitMoveType.Fly6 ].addMove = false;
        data[ (int)GameUnitMoveType.Fly6 ].fly = true;
        data[ (int)GameUnitMoveType.Fly5 ].subMove = 0;

        data[ (int)GameUnitMoveType.Walk7 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Walk7 ].block = 7;
        data[ (int)GameUnitMoveType.Walk7 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk7 ].fly = false;
        data[ (int)GameUnitMoveType.Walk7 ].subMove = -1;

        data[ (int)GameUnitMoveType.Fly8 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Fly8 ].block = 8;
        data[ (int)GameUnitMoveType.Fly8 ].addMove = true;
        data[ (int)GameUnitMoveType.Fly8 ].fly = true;
        data[ (int)GameUnitMoveType.Fly8 ].subMove = -2;

        data[ (int)GameUnitMoveType.Fly9 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Fly9 ].block = 9;
        data[ (int)GameUnitMoveType.Fly9 ].addMove = true;
        data[ (int)GameUnitMoveType.Fly9 ].fly = true;
        data[ (int)GameUnitMoveType.Fly9 ].subMove = -2;

        data[ (int)GameUnitMoveType.Walk10 ].baseCost = 6;
        data[ (int)GameUnitMoveType.Walk10 ].block = 7;
        data[ (int)GameUnitMoveType.Walk10 ].addMove = true;
        data[ (int)GameUnitMoveType.Walk10 ].fly = false;
        data[ (int)GameUnitMoveType.Walk10 ].subMove = -2;

        data[ (int)GameUnitMoveType.None ].baseCost = 0;
        data[ (int)GameUnitMoveType.None ].block = 0;
        data[ (int)GameUnitMoveType.None ].addMove = true;
        data[ (int)GameUnitMoveType.None ].fly = false;
        data[ (int)GameUnitMoveType.None ].subMove = 0;


        data[ (int)GameUnitMoveType.Fly ].baseCost = 7;
        data[ (int)GameUnitMoveType.Fly ].block = 10;
        data[ (int)GameUnitMoveType.Fly ].addMove = false;
        data[ (int)GameUnitMoveType.Fly ].fly = true;
        data[ (int)GameUnitMoveType.Fly ].subMove = 0;

    }



}

