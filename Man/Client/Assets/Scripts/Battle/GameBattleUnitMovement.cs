using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleUnitMovement : Singleton<GameBattleUnitMovement>
{
    [System.Serializable]
    public class CellTest : MonoBehaviour
    {
        public int f;
        public int c;
    }

    public class Cell
    {
        public int x;
        public int y;

        public int f;
        public int c;
        public int ca;
    }

    bool isShow = false;

    bool startFade = false;
    bool alphaAdd = false;

    float alpha;
    float time;

    List<Cell> openList = new List<Cell>();
    List<Cell> closeList = new List<Cell>();


    public bool IsShow { get { return isShow; } }

    public List<Cell> Cells { get { return closeList; } }



    public override void initSingleton()
    {
        gameObject.SetActive( false );
    }


    public void clear()
    {
        GameDefine.DestroyAll( transform );
    }

    public int getCellCost( int x , int y )
    {
        for ( int i = 0 ; i < closeList.Count ; i++ )
        {
            if ( closeList[ i ].x == x &&
                closeList[ i ].y == y )
            {
                return closeList[ i ].c;
            }
        }

        return 0;
    }

    public bool checkCell( int x , int y )
    {
        for ( int i = 0 ; i < closeList.Count ; i++ )
        {
            if ( closeList[ i ].x == x &&
                closeList[ i ].y == y )
            {
                return true;
            }
        }

        return false;
    }

    public void addCell( Cell cell , bool user )
    {
        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Cell" ) , transform );
        obj.name = cell.x + "-" + cell.y;

        Transform trans = obj.transform;
        trans.localPosition = new Vector3( GameDefine.getBattleX( cell.x ) , 
            GameDefine.getBattleY( cell.y ) + GameBattleManager.instance.LayerHeight , 
            0.0f );

        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        Color c = user ? new Color( 1.0f , 0.0f , 1.0f , 0.5f ) : new Color( 1.0f , 0.0f , 0.0f , 0.5f );
        sprite.color = c;

#if UNITY_EDITOR
        CellTest test = obj.AddComponent<CellTest>();
        test.c = cell.c;
        test.f = cell.f;
#endif

    }

    public void fadeCell( bool b )
    {
        alpha = 0.5f;
        time = 0.0f;
        alphaAdd = false;
        startFade = b;

        updateCell();
    }

    void updateCell()
    {
        for ( int i = 0 ; i < transform.childCount ; i++ )
        {
            SpriteRenderer sprite = transform.GetChild( i ).GetComponent<SpriteRenderer>();

            Color c = sprite.color;
            c.a = alpha;

            sprite.color = c;
        }
    }

    public void show( int x , int y , int move , GameUnitMoveType moveType , bool fly , GameUnitCampType camp , bool fade )
    {
        gameObject.SetActive( true );

        clear();

        isShow = true;

        openList.Clear();
        closeList.Clear();

        Cell cell = new Cell();
        cell.x = x;
        cell.y = y;
        cell.f = 0;

        openList.Add( cell );

        findCell( move , moveType , fly , camp );
        fadeCell( fade );
    }

    public List<Cell> getMoveList( int x , int y , int move , GameUnitMoveType moveType , bool fly , GameUnitCampType camp )
    {
        List<Cell> openList1 = new List<Cell>();
        List<Cell> closeList1 = new List<Cell>();

        Cell cell = new Cell();
        cell.x = x;
        cell.y = y;
        cell.f = 0;

        openList1.Add( cell );

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( moveType );

        bool user = ( camp == GameUnitCampType.User );

        GameBattleDTL dtl = GameBattleManager.instance.ActiveDTL;

        closeList1.Add( openList1[ 0 ] );

        while ( openList1.Count > 0 )
        {
            List<Cell> newOpenList = new List<Cell>();

            for ( int i = 0 ; i < openList1.Count ; i++ )
            {
                cell = openList1[ i ];
                GameBattleDTL.Point point = null;

                // right
                int index = dtl.Width * cell.y + cell.x + 1;
                if ( index >= 0 && index < dtl.Points.Length &&
                    !isClose1( closeList1 , cell.x + 1 , cell.y , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x + 1 , cell.y , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x + 1;
                        newCell.y = cell.y;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList1.Add( newCell );
                    }
                }

                // left
                index = dtl.Width * cell.y + cell.x - 1;
                if ( index >= 0 && index < dtl.Points.Length &&
                    !isClose1( closeList1 , cell.x - 1 , cell.y , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x - 1 , cell.y , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x - 1;
                        newCell.y = cell.y;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList1.Add( newCell );
                    }
                }

                // top
                index = dtl.Width * ( cell.y - 1 ) + cell.x;
                if ( index >= 0 && index < dtl.Points.Length &&
                    !isClose1( closeList1 , cell.x , cell.y - 1 , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x , cell.y - 1 , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x;
                        newCell.y = cell.y - 1;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList1.Add( newCell );
                    }
                }

                // bottom
                index = dtl.Width * ( cell.y + 1 ) + cell.x;
                if ( index >= 0 && index < dtl.Points.Length &&
                    !isClose1( closeList1 , cell.x , cell.y + 1 , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x , cell.y + 1 , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x;
                        newCell.y = cell.y + 1;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList1.Add( newCell );
                    }
                }

            }

            openList1.Clear();
            openList1 = null;
            openList1 = newOpenList;
        }

        for ( int i = 1 ; i < closeList1.Count ; )
        {
            if ( isUnit( closeList1[ i ].x , closeList1[ i ].y ) )
            {
                closeList1.RemoveAt( i );
            }
            else
            {
                i++;
            }
        }

        return closeList1;
    }

    void findCell( int move , GameUnitMoveType moveType , bool fly , GameUnitCampType camp )
    {
        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( moveType );

        bool user = ( camp == GameUnitCampType.User );

        GameBattleDTL dtl = GameBattleManager.instance.ActiveDTL;

        closeList.Add( openList[ 0 ] );

        while ( openList.Count > 0 )
        {
            List<Cell> newOpenList = new List<Cell>();

            for ( int i = 0 ; i < openList.Count ; i++ )
            {
                Cell cell = openList[ i ];
                GameBattleDTL.Point point = null;

                // right
                int index = dtl.Width * cell.y + cell.x + 1;
                if ( index >= 0 && index < dtl.Points.Length && 
                    !isClose( cell.x + 1 , cell.y , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x + 1 , cell.y , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block && 
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x + 1;
                        newCell.y = cell.y;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList.Add( newCell );
                    }
                }

                // left
                index = dtl.Width * cell.y + cell.x - 1;
                if ( index >= 0 && index < dtl.Points.Length && 
                    !isClose( cell.x - 1 , cell.y , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x - 1 , cell.y , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x - 1;
                        newCell.y = cell.y;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList.Add( newCell );
                    }
                }

                // top
                index = dtl.Width * ( cell.y - 1 ) + cell.x;
                if ( index >= 0 && index < dtl.Points.Length && 
                    !isClose( cell.x , cell.y - 1 , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x , cell.y - 1 , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x;
                        newCell.y = cell.y - 1;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList.Add( newCell );
                    }
                }

                // bottom
                index = dtl.Width * ( cell.y + 1 ) + cell.x;
                if ( index >= 0 && index < dtl.Points.Length && 
                    !isClose( cell.x , cell.y + 1 , camp , fly ) )
                {
                    point = dtl.Points[ index ];

                    bool bd = false;
                    int cost = getCost( cell.x , cell.y + 1 , unitMove , camp , out bd );

                    if ( point.Move < unitMove.block &&
                        cell.f + cost <= move )
                    {
                        Cell newCell = new Cell();
                        newCell.x = cell.x;
                        newCell.y = cell.y + 1;
                        newCell.f = cell.f + ( bd ? cost * 2 : cost );
                        newCell.c = cost;
                        newCell.ca = cell.f + cost;

                        newOpenList.Add( newCell );
                        closeList.Add( newCell );
                    }
                }

            }

            openList.Clear();
            openList = null;
            openList = newOpenList;
        }

        for ( int i = 1 ; i < closeList.Count ; )
        {
            if ( isUnit( closeList[ i ].x , closeList[ i ].y ) )
            {
                closeList.RemoveAt( i );
            }
            else
            {
                i++;
            }
        }

        for ( int i = 0 ; i < closeList.Count ; i++ )
        {
            addCell( closeList[ i ] , user );
        }
    }

    int getCost( int x , int y , GameUnitMove unitMove , GameUnitCampType camp , out bool bd )
    {
        //         byte baseCost = 0;
        //         byte block = 0;
        //         bool addMove = false;
        //         sbyte subMove = 0;
        // 
        //         switch ( moveType )
        //         {
        //             case GameUnitMoveType.Walk0:
        //                 baseCost = 5;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Walk1:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Walk2:
        //                 baseCost = 5;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Walk3:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = 1;
        //                 break;
        //             case GameUnitMoveType.Fly4:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = false;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Fly5:
        //                 baseCost = 7;
        //                 block = 9;
        //                 addMove = false;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Fly6:
        //                 baseCost = 5;
        //                 block = 9;
        //                 addMove = false;
        //                 subMove = 0;
        //                 break;
        //             case GameUnitMoveType.Walk7:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = -1;
        //                 break;
        //             case GameUnitMoveType.Fly8:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = -2;
        //                 break;
        //             case GameUnitMoveType.Fly9:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = -2;
        //                 break;
        //             case GameUnitMoveType.Walk10:
        //                 baseCost = 6;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = -2;
        //                 break;
        //             case GameUnitMoveType.None:
        //                 baseCost = 100;
        //                 block = 7;
        //                 addMove = true;
        //                 subMove = 0;
        //                 break;
        //         }

        bd = false;

        int cost = unitMove.baseCost;

        GameBattleDTL dtl = GameBattleManager.instance.ActiveDTL;

        int index1 = dtl.Width * y + x;

        GameBattleDTL.Point point = dtl.Points[ index1 ];

        int subMove = point.Move + unitMove.subMove;

        if ( subMove < 0 )
        {
            subMove = 0;
        }

        if ( unitMove.addMove )
        {
            cost = unitMove.baseCost + subMove;
        }

        // right
        int index = dtl.Width * y + x + 1;
        if ( index >= 0 && index < dtl.Points.Length )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x + 1 , y );

            if ( unit != null )
            {
                if ( unitMove.addMove && 
                    unit.IsAlive &&
                    unit.UnitCampType != camp )
                {
                    bd = true;
                }
            }
        }

        // left
        index = dtl.Width * y + x - 1;
        if ( index >= 0 && index < dtl.Points.Length )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x - 1 , y );

            if ( unit != null )
            {
                if ( unitMove.addMove && 
                    unit.IsAlive && 
                    unit.UnitCampType != camp )
                {
                    bd = true;
                }
            }
        }

        // top
        index = dtl.Width * ( y - 1 ) + x;
        if ( index >= 0 && index < dtl.Points.Length )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x  , y - 1);

            if ( unit != null )
            {
                if ( unitMove.addMove &&
                    unit.IsAlive &&
                    unit.UnitCampType != camp )
                {
                    bd = true;
                }
            }
        }

        // bottom
        index = dtl.Width * ( y + 1 ) + x;
        if ( index >= 0 && index < dtl.Points.Length )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x , y + 1 );

            if ( unit != null )
            {
                if ( unitMove.addMove && 
                    unit.IsAlive &&
                    unit.UnitCampType != camp )
                {
                    bd = true;
                }
            }
        }

        return cost;
    }

    bool isUnit( int x , int y )
    {
        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x , y );

        return unit != null;
    }

    bool isClose1( List<Cell> list , int x , int y , GameUnitCampType type , bool fly )
    {
        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x , y );

        if ( !fly && unit != null )
        {
            if ( unit.IsAlive && unit.UnitCampType != type )
            {
                return true;
            }
        }

        for ( int i = 0 ; i < list.Count ; i++ )
        {
            if ( list[ i ].x == x && list[ i ].y == y )
            {
                return true;
            }
        }

        return false;
    }

    bool isClose( int x , int y , GameUnitCampType type , bool fly )
    {
        GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x , y );

//        if ( !fly && unit != null )
        if ( unit != null )
        {
            if ( unit.IsAlive && unit.UnitCampType != type )
            {
                return true;
            }
        }

        for ( int i = 0 ; i < closeList.Count ; i++ )
        {
            if ( closeList[ i ].x == x && closeList[ i ].y == y )
            {
                return true;
            }
        }

        return false;
    }

    public void unShow()
    {
        isShow = false;

        startFade = false;

        clear();

        gameObject.SetActive( false );
    }


    void Update()
    {
        if ( !startFade )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > 1.0f )
        {
            alphaAdd = !alphaAdd;
            time = 0.0f;
        }
        else
        {
            if ( alphaAdd )
            {
                alpha = time * 0.5f;
            }
            else
            {
                alpha = 0.5f - time * 0.5f;
            }

            updateCell();
        }
    }

}
