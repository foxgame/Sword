using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleUnitItemSelection : Singleton<GameBattleUnitItemSelection>
{
    public class Cell
    {
        public int x;
        public int y;
    }

    bool isShow = false;

    bool startFade = false;
    bool alphaAdd = false;

    float alpha;
    float time;

    GameAttackRangeType rangeType;
    int posX = 0;
    int posY = 0;

    List<Cell> list = new List<Cell>();


    List<GameBattleUnit> attackUnits = new List<GameBattleUnit>();
    GameBattleAttackMapDirection attackDirection;
    Cell attackCell = new Cell();
    Cell attackMoveCell = new Cell();

    public bool IsShow { get { return isShow; } }
    public List<GameBattleUnit> AttackUnits { get { return attackUnits; } }
    public GameBattleAttackMapDirection AttackDirection { get { return attackDirection; } }
    public Cell AttackCell { get { return attackCell; } }
    public Cell AttackMoveCell { get { return attackMoveCell; } }



    public override void initSingleton()
    {
        gameObject.SetActive( false );
    }

    public void clear()
    {
        GameDefine.DestroyAll( transform );
    }

    public void setAttackAI( GameBattleUnit unit , int x , int y )
    {
        attackDirection = GameBattleAttackMapDirection.Count;
        attackUnits.Clear();

        attackUnits.Add( unit );

        attackCell.x = x;
        attackCell.y = y;
    }

    public void canAttack( int x , int y , GameItem m , GameUnitCampType camp )
    {
        attackUnits.Clear();
        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = ( m.AttackRangeMin == GameDefine.INVALID_ID ? 1 : m.AttackRangeMin + 1 );
        int max = m.AttackRangeMax;

        if ( min > max )
        {
            min = max;
        }

        rangeType = m.AttackRangeType;
        posX = x;
        posY = y;

        switch ( rangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    for ( int i = -max ; i <= max ; i++ )
                    {
                        for ( int j = -max ; j <= max ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= max && xx + yy >= min )
                            {
                                Cell cell = new Cell();
                                cell.x = x + j;
                                cell.y = y + i;
                                list.Add( cell );
                            }
                        }
                    }

                    if ( m.AttackRangeMin == 0 )
                    {
                        Cell cell = new Cell();
                        cell.x = x;
                        cell.y = y;
                        list.Add( cell );
                    }

                    for ( int i = 0 ; i < list.Count ; i++ )
                    {
                        if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                            list[ i ].x >= width || list[ i ].y >= height )
                        {
                            continue;
                        }

                        getUnitsCircle( x , y , list[ i ].x , list[ i ].y , m , camp );
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    getUnitsLine( x , y , m , camp );
                }
                break;
        }
    }

    public void getUnitsLine( int x , int y , GameItem m , GameUnitCampType camp )
    {
        List<GameBattleUnit> list = new List<GameBattleUnit>();

        for ( int i = 0 ; i < (int)GameBattleAttackMapDirection.Count ; i++ )
        {
            int x1 = 0;
            int y1 = 0;

            switch ( i )
            {
                case (int)GameBattleAttackMapDirection.North:
                    y1 = -1;
                    break;
                case (int)GameBattleAttackMapDirection.East:
                    x1 = 1;
                    break;
                case (int)GameBattleAttackMapDirection.South:
                    y1 = 1;
                    break;
                case (int)GameBattleAttackMapDirection.West:
                    x1 = -1;
                    break;
            }


            for ( int j = 0 ; j < m.AttackRangeMax ; j++ )
            {
                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( x + i * x1 , y + i * y1 );

                switch ( m.UseTargetType )
                {
                    case GameTargetType.User:
                        {
                            if ( u != null && 
                                u.UnitCampType == camp )
                            {
                                list.Add( u );
                            }
                        }
                        break;
                    case GameTargetType.Enemy:
                        {
                            if ( u != null &&
                                u.UnitCampType != camp )
                            {
                                list.Add( u );
                            }
                        }
                        break;
                    case GameTargetType.Summon:
                        {
                            if ( u == null )
                            {
                                list.Add( u );
                            }
                        }
                        break;
                }

                if ( list.Count > attackUnits.Count )
                {
                    attackCell.x = x + x1;
                    attackCell.y = y + y1;

                    attackDirection = (GameBattleAttackMapDirection)i;

                    attackUnits.Clear();

                    for ( int k = 0 ; k < list.Count ; k++ )
                    {
                        attackUnits.Add( list[ k ] );
                    }
                }

                list.Clear();
            }

        }

    }

    public void getUnitsCircle( int ux , int uy , int x , int y , GameItem m , GameUnitCampType camp )
    {
        attackDirection = GameBattleAttackMapDirection.Count;

        List<GameBattleUnit> list = new List<GameBattleUnit>();

        int size = m.AttackRange;

        for ( int i = -size ; i <= size ; i++ )
        {
            for ( int j = -size ; j <= size ; j++ )
            {
                int xx = Mathf.Abs( j );
                int yy = Mathf.Abs( i );

                if ( xx + yy <= size )
                {
                    GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x + j , y + i );

                    if ( unit != null )
                    {
                        switch ( m.UseTargetType )
                        {
                            case GameTargetType.User:
                                {
                                    if ( unit.UnitCampType == camp )
                                    {
                                        list.Add( unit );
                                    }
                                }
                                break;
                            case GameTargetType.Enemy:
                                {
                                    if ( unit.UnitCampType != camp )
                                    {
                                        list.Add( unit );
                                    }
                                }
                                break;
                            case GameTargetType.Summon:
                                {
                                    if ( unit.IsSummon &&
                                        unit.UnitCampType != camp )
                                    {
                                        list.Add( unit );
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch ( m.UseTargetType )
                        {
                            case GameTargetType.Summon:
                                {
                                    list.Add( unit );
                                }
                                break;
                        }
                    }
                }
            }
        }

        if ( list.Count > attackUnits.Count ||
           ( list.Count > 0 && list.Count == attackUnits.Count &&
           Mathf.Abs( ux - x ) + Mathf.Abs( uy - y ) < Mathf.Abs( ux - attackCell.x ) + Mathf.Abs( uy - attackCell.y ) ) )
        {
            attackCell.x = x;
            attackCell.y = y;

            attackUnits.Clear();

            for ( int i = 0 ; i < list.Count ; i++ )
            {
                attackUnits.Add( list[ i ] );
            }
        }

        list.Clear();
    }

    public bool checkCell( int x , int y , out GameBattleAttackMapDirection dir )
    {
        dir = GameBattleAttackMapDirection.Count;

        for ( int i = 0 ; i < list.Count ; i++ )
        {
            if ( list[ i ].x == x && list[ i ].y == y )
            {
                if ( rangeType == GameAttackRangeType.Line )
                {
                    dir = GameDefine.getDirectionMap( posX , posY , x , y );
                }

                return true;
            }
        }

        return false;
    }

    public void addCell( int x , int y , GameUnitCampType camp )
    {
        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Cell" ) , transform );
        obj.name = x + "-" + y;

        Transform trans = obj.transform;
        trans.localPosition = new Vector3( GameDefine.getBattleX( x ) ,
            GameDefine.getBattleY( y ) + GameBattleManager.instance.LayerHeight ,
            0.0f );

        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        Color c = ( camp == GameUnitCampType.User ? new Color( 1.0f , 0.0f , 1.0f , 0.5f ) : new Color( 1.0f , 0.0f , 0.0f , 0.5f ) );
        sprite.color = c;
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


    public void showGive( int x , int y , int rmin , int rmax , GameAttackRangeType type , int range , GameUnitCampType camp , bool fade )
    {
        gameObject.SetActive( true );

        clear();

        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = ( rmin == GameDefine.INVALID_ID ? 1 : rmin + 1 );
        int max = rmax;

        if ( min > max )
        {
            min = max;
        }

        rangeType = type;
        posX = x;
        posY = y;


        switch ( rangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    for ( int i = -max ; i <= max ; i++ )
                    {
                        for ( int j = -max ; j <= max ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= max && xx + yy >= min )
                            {
                                Cell cell = new Cell();
                                cell.x = x + j;
                                cell.y = y + i;
                                list.Add( cell );
                            }
                        }
                    }

                    if ( rmin == 0 )
                    {
                        Cell cell = new Cell();
                        cell.x = x;
                        cell.y = y;
                        list.Add( cell );
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    for ( int i = min ; i <= max ; i++ )
                    {
                        Cell cell = new Cell();
                        cell.x = x + i;
                        cell.y = y;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x - i;
                        cell.y = y;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x;
                        cell.y = y - i;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x;
                        cell.y = y + i;
                        list.Add( cell );
                    }
                }
                break;
        }

        for ( int i = 0 ; i < list.Count ; )
        {
            if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                list[ i ].x >= width || list[ i ].y >= height )
            {
                list.RemoveAt( i );
            }
            else
            {
                addCell( list[ i ].x , list[ i ].y , camp );
                i++;
            }
        }

        fadeCell( fade );

        isShow = true;

        GameBattleCursor.instance.show( range );
    }

    public void showUse( int x , int y , GameItem item , GameUnitCampType camp , bool fade )
    {
        gameObject.SetActive( true );

        clear();

        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = ( item.UseRangeMin == GameDefine.INVALID_ID ? 1 : item.UseRangeMin + 1 );
        int max = item.UseRangeMax;

        if ( min > max )
        {
            min = max;
        }

        rangeType = item.UseRangeType;
        posX = x;
        posY = y;

        switch ( rangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    for ( int i = -max ; i <= max ; i++ )
                    {
                        for ( int j = -max ; j <= max ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= max && xx + yy >= min )
                            {
                                Cell cell = new Cell();
                                cell.x = x + j;
                                cell.y = y + i;
                                list.Add( cell );
                            }
                        }
                    }

                    if ( item.UseRangeMin == 0 )
                    {
                        Cell cell = new Cell();
                        cell.x = x;
                        cell.y = y;
                        list.Add( cell );
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    for ( int i = min ; i <= max ; i++ )
                    {
                        Cell cell = new Cell();
                        cell.x = x + i;
                        cell.y = y;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x - i;
                        cell.y = y;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x;
                        cell.y = y - i;
                        list.Add( cell );

                        cell = new Cell();
                        cell.x = x;
                        cell.y = y + i;
                        list.Add( cell );
                    }
                }
                break;
        }


        bool b = false;
        int dis = 99;
        attackCell.x = x;
        attackCell.y = y;

        int size = item.UseRange;

        for ( int i = 0 ; i < list.Count ; )
        {
            if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                list[ i ].x >= width || list[ i ].y >= height )
            {
                list.RemoveAt( i );
                continue;
            }

            if ( item.UseTargetType == GameTargetType.Summon &&
                GameBattlePathFinder.instance.isBlockPos( list[ i ].x , list[ i ].y , GameBattlePathFinder.BLOCK_EVENT ) )
            {
                list.RemoveAt( i );
                continue;
            }

            i++;
        }

        for ( int i = 0 ; i < list.Count ; i++ )
        {
            addCell( list[ i ].x , list[ i ].y , camp );

            int d = Mathf.Abs( x - list[ i ].x ) + Mathf.Abs( y - list[ i ].y );

            if ( dis > d )
            {
                GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( list[ i ].x , list[ i ].y );

                if ( unit != null )
                {
                    switch ( item.UseTargetType )
                    {
                        case GameTargetType.User:
                            {
                                if ( unit.UnitCampType == camp )
                                {
                                    dis = d;
                                    b = true;
                                    attackCell.x = list[ i ].x;
                                    attackCell.y = list[ i ].y;
                                }
                            }
                            break;
                        case GameTargetType.Enemy:
                            {
                                if ( unit.UnitCampType != camp )
                                {
                                    dis = d;
                                    b = true;
                                    attackCell.x = list[ i ].x;
                                    attackCell.y = list[ i ].y;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch ( item.UseTargetType )
                    {
                        case GameTargetType.Summon:
                            {
                                dis = d;
                                b = true;
                                attackCell.x = list[ i ].x;
                                attackCell.y = list[ i ].y;
                            }
                            break;
                    }
                }
            }
        }


        if ( !b )
        {
            for ( int i = 0 ; i < list.Count ; i++ )
            {
                if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                    list[ i ].x >= width || list[ i ].y >= height )
                {
                    continue;
                }

                int d = Mathf.Abs( x - list[ i ].x ) + Mathf.Abs( y - list[ i ].y );

                if ( dis > d )
                {
                    for ( int i0 = -size ; i0 <= size ; i0++ )
                    {
                        for ( int j0 = -size ; j0 <= size ; j0++ )
                        {
                            int xx = Mathf.Abs( j0 );
                            int yy = Mathf.Abs( i0 );

                            if ( xx + yy <= size )
                            {
                                GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( list[ i ].x + j0 , list[ i ].y + i0 );

                                if ( unit != null )
                                {
                                    switch ( item.UseTargetType )
                                    {
                                        case GameTargetType.User:
                                            {
                                                if ( unit.UnitCampType == camp )
                                                {
                                                    dis = d;
                                                    attackCell.x = list[ i ].x;
                                                    attackCell.y = list[ i ].y;
                                                }
                                            }
                                            break;
                                        case GameTargetType.Enemy:
                                            {
                                                if ( unit.UnitCampType != camp )
                                                {
                                                    dis = d;
                                                    attackCell.x = list[ i ].x;
                                                    attackCell.y = list[ i ].y;
                                                }
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch ( item.UseTargetType )
                                    {
                                        case GameTargetType.Summon:
                                            {
                                                dis = d;
                                                attackCell.x = list[ i ].x;
                                                attackCell.y = list[ i ].y;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        fadeCell( fade );

        isShow = true;

        GameBattleCursor.instance.show( item.UseRange );

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

