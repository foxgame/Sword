using UnityEngine;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;
using System;


public class GameBattlePathFinder : Singleton< GameBattlePathFinder >
{
    public const byte BLOCK = 7;
    public const byte BLOCK_EVENT = 9;

    class PathNode
    {
        public short posX = 0;
        public short posY = 0;

//         public int f = 0;
//         public int g = 0;
//         public int n = 0;

        public PathNode parent = null;

        public void clear()
        {
            parent = null;

//             f = 0;
//             g = 0;
//             n = 0;
        }

        public PathNode()
        {

        }
    };


    public short[] pathResult;

    PathNode resultNode = null;
    PathNode topNode = null;

    PathNode[] openResult = null;
    PathNode[] open = null;

    byte[] close;

    byte[] mapData = null;

    int maxX = 0;
    int maxY = 0;

    int step = 0;

    int openCount;
    int openResultCount;

    bool checkUnit;

    byte checkBlock;

    GameUnitCampType campType;

    int startPosX;
    int startPosY;
    int endPosX;
    int endPosY;

    public int nearPosX;
    public int nearPosY;

    public int cost;

    private PathNode[] nodes;


    public void release()
    {
        resultNode = null;
        topNode = null;

        openResult = null;
        open = null;

        close = null;

        mapData = null;

        nodes = null;

        pathResult = null;
    }

    public void initMap( int x , int y , byte[] d )
    {
        maxX = x;
        maxY = y;

        mapData = d;

        close = new byte[ x * y ];

        open = new PathNode[ x * y ];
        openResult = new PathNode[ x * y ];

        pathResult = new short[ x * y ];

        nodes = new PathNode[ x * y ];
        for ( int i = 0 ; i < x * y ; i++ )
        {
            nodes[ i ] = new PathNode();
        }

        int count = 0;
        for ( short i = 0 ; i < y ; i++ )
        {
            for ( short j = 0 ; j < x ; j++ )
            {
                nodes[ count ].posX = j;
                nodes[ count ].posY = i;

                count++;
            }
        }
    }

    public bool isBlock( byte c )
    {
        return c >= checkBlock;
    }

    public bool isBlock( int x , int y )
    {
        int i = getIndex( x , y );

        if ( i == GameDefine.INVALID_ID )
        {
            return true;
        }

//        if ( checkUnit )
//        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( x , y );

            if ( campType != GameUnitCampType.Count && unit != null && unit.UnitCampType != campType )
            {
                return true;
            }
//        }

        return isBlock( mapData[ getIndex( x , y ) ] );
    }


    public byte getMapData( int x , int y )
    {
        int i = getIndex( x , y );

        if ( i == GameDefine.INVALID_ID )
        {
            return 0;
        }

        return mapData[ getIndex( x , y ) ];
    }

    public void setBlock( int x , int y , byte b )
    {
        int i = getIndex( x , y );

        if ( i == GameDefine.INVALID_ID )
        {
            return;
        }

        mapData[ getIndex( x , y ) ] = b;
    }

    public void setStartEnd( int sx , int sy , int ex , int ey )
    {
        startPosX = sx;
        startPosY = sy;

        endPosX = ex;
        endPosY = ey;
    }

    void findPath()
    {
        resultNode = null;

        if ( startPosX == endPosX && startPosY == endPosY )
        {
            return;
        }

        if ( isBlock( endPosX , endPosY ) )
        {
            return;
        }

        clear();

        open[ openCount ] = nodes[ getIndex( startPosX , startPosY ) ];
        open[ openCount ].parent = null;

        openCount++;

        close[ getIndex( startPosX , startPosY ) ] = 1;
        topNode = open[ 0 ];
        topNode.clear();

        step = 1;
        int count = 0;

        while ( true )
        {
            count++;
            PathNode[] v1 = open;
            PathNode[] v2 = openResult;
            int c = openCount;


            if ( step > 0 )
            {
                step = 0;
            }
            else
            {
                v1 = openResult;
                v2 = open;

                step = 1;

                c = openResultCount;
            }

            for ( int i = 0 ; i < c ; ++i )
            {
                int n = find( v2 , v1[ i ] );

                if ( n > 0 )
                {
                    return;
                }
            }


            if ( step > 0 )
            {
                openResultCount = 0;

                if ( openCount == 0 )
                {
                    return;
                }
            }
            else
            {
                openCount = 0;

                if ( openResultCount == 0 )
                {
                    return;
                }
            }

            if ( count >= 5000 )
            {
                resultNode = null;
                return;
            }

            //        if ( step )
            //        {
            //            qsort( v2 , mOpenCount, sizeof( PathNode* ) , PathNodeLess );
            //        }
            //        else
            //        {
            //            qsort( v2 , mOpenResultCount, sizeof( PathNode* ) , PathNodeLess );
            //        }


            //qsort( v2  , v2->size() , sizeof( PathNode* ) , PathNodeLess );
        }
    }

    int find( PathNode[] v , PathNode parent )
    {
        // w
        int posX = parent.posX;
        int posY = parent.posY;
        --posX;
        int find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // e
        posX = parent.posX;
        posY = parent.posY;
        ++posX;
        find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // s
        posX = parent.posX;
        posY = parent.posY;

        ++posY;
        find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // n
        posX = parent.posX;
        posY = parent.posY;
        --posY;
        find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        return 0;
    }


    int find( PathNode[] v , PathNode parent , int posX , int posY , int f , int n = 0 )
    {
        int index = getIndex( posX , posY );
        if ( index != GameDefine.INVALID_ID )
        {
            if ( close[ index ] != 1 )
            {
                if ( !isBlock( posX , posY ) )
                {                    
                    nodes[ index ].parent = parent;

                    if ( step != 0 )
                    {
                        v[ openCount ] = nodes[ index ];
                        openCount++;
                    }
                    else
                    {
                        v[ openResultCount ] = nodes[ index ];
                        openResultCount++;
                    }
                    //v->push_back( mNodes + index );

                    if ( posX == endPosX && posY == endPosY )
                    {
                        resultNode = nodes[ index ];
                        return 1;
                    }
                }

                close[ index ] = 1;
            }

        }

        return 0;
    }


    void clear()
    {
        resultNode = null;

        if ( topNode != null )
        {
            topNode.clear();
            topNode = null;
        }

        openCount = 0;
        openResultCount = 0;

        for ( int i = 0 ; i < maxX * maxY ; i++ )
        {
            close[ i ] = 0;
        }
    }


    public int getX( int index )
    {
        return index % maxX;
    }
    public int getY( int index )
    {
        return index / maxX;
    }

    public int getIndex( int x , int y )
    {
        if ( x < 0 || y < 0 || x >= maxX || y >= maxY )
        {
            return GameDefine.INVALID_ID;
        }

        return y * maxX + x;
    }

    public int findPath( int sx , int sy , int ex , int ey , byte cb , bool fly , GameUnitCampType ct )
    {
        if ( maxX == 0 || maxY == 0 )
        {
            return 0;
        }

        if ( ex >= maxX || ey >= maxY ||
            sx >= maxX || sy >= maxY ||
            sx < 0 || sy < 0 ||
            ex < 0 || ey < 0 )
        {
            return 0;
        }

        checkBlock = cb;
        checkUnit = !fly;
        campType = ct;

        setStartEnd( sx , sy , ex , ey );
        findPath();

        if ( resultNode == null )
        {
            return 0;
        }

        cost = 0;

        int count = 0;
        PathNode node = resultNode;

        int lastX = 0;
        int lastY = 0;

        int lineX = 0;
        int lineY = 0;

        while ( node != null )
        {
            pathResult[ count ] = node.posX; count++;
            pathResult[ count ] = node.posY; count++;

            if ( lastX == node.posX )
            {
                lineY++;
            }

            if ( lastY == node.posY )
            {
                lineX++;
            }

            lastX = node.posX;
            lastY = node.posY;

            cost += GameBattleManager.instance.getPoint( node.posX , node.posY ).Move;

            if ( GameBattleUnitManager.instance.getUnit( node.posX , node.posY ) != null )
            {
                cost += GameBattleManager.instance.getPoint( node.posX , node.posY ).Move;
            }

            node = node.parent;
        }

//         if ( lineX == 0 || lineY == 0 )
//         {
//             cost++;
//         }
        cost += Mathf.Abs( lineX - lineY );

        return count;
    }

    public int getDistance( int sx , int sy , int ex , int ey )
    {
        return Mathf.Abs( sx - ex ) + Mathf.Abs( sy - ey );
    }

    public int findPathNear( int sx , int sy , int ex , int ey , byte cb , bool fly , GameUnitCampType ct )
    {
        if ( maxX == 0 || maxY == 0 )
        {
            return 0;
        }

        if ( ex >= maxX || ey >= maxY ||
            sx >= maxX || sy >= maxY ||
            sx < 0 || sy < 0 ||
            ex < 0 || ey < 0 )
        {
            return 0;
        }

        checkBlock = cb;
        checkUnit = !fly;
        campType = ct;

        setStartEnd( sx , sy , ex , ey );
        findPath();

        if ( resultNode == null )
        {
            return 0;
        }

        cost = 0;

        int count = 0;
        PathNode node = resultNode;

        int lastX = 0;
        int lastY = 0;

        int lineX = 0;
        int lineY = 0;

        while ( node != null )
        {
            pathResult[ count ] = node.posX; count++;
            pathResult[ count ] = node.posY; count++;

            if ( lastX == node.posX )
            {
                lineY++;
            }

            if ( lastY == node.posY )
            {
                lineX++;
            }

            lastX = node.posX;
            lastY = node.posY;

            cost += GameBattleManager.instance.getPoint( node.posX , node.posY ).Move;

            if ( GameBattleUnitManager.instance.getUnit( node.posX , node.posY ) != null )
            {
                cost += GameBattleManager.instance.getPoint( node.posX , node.posY ).Move;
            }

            node = node.parent;
        }

        cost += Mathf.Abs( lineX - lineY );

        return count;
    }


    public bool isBlockPos( int x , int y , byte bc )
    {
        checkBlock = bc;

        if ( x < 0 || y < 0 || x >= maxX || y >= maxY )
        {
            return true;
        }

        return isBlock( mapData[ getIndex( x , y ) ] );
    }

    public bool isBlockPos( int x , int y , byte bc , GameBattleUnit unit )
    {
        checkBlock = bc;

        if ( x < 0 || y < 0 || x >= maxX || y >= maxY )
        {
            return true;
        }

        if ( GameBattleUnitManager.instance.getUnit( x , y , unit ) != null )
        {
            return true;
        }

        return isBlock( mapData[ getIndex( x , y ) ] );
    }

    public bool findNearPos( int posX , int posY , byte bc )
    {
        int dis = 1;
        int range = 128;
        int d = 9999;
        checkBlock = bc;

        int x1 = posX;
        int y1 = posY;
        int x2 = posX + 1;
        int y2 = posY + 1;

        nearPosX = GameDefine.INVALID_ID;
        nearPosY = GameDefine.INVALID_ID;

        checkUnit = true;

        while ( range > dis )
        {
            int x = x1 - dis;
            int y = y1 - dis;
            int fx = x2 + dis;
            int fy = y2 + dis;

            for ( int i = 0 ; i < fx - x ; i++ )
            {
                if ( !isBlockPos( x + i , y , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x + i - posX ) + Mathf.Abs( y - posY );
                    if ( dd < d )
                    {
                        d = dd;
                        nearPosX = x + i;
                        nearPosY = y;
                    }
                }
            }


            for ( int i = 0 ; i < fx - x ; i++ )
            {
                if ( !isBlockPos( x + i , fy - 1 , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x + i - posX ) + Mathf.Abs( fy - 1 - posY );
                    if ( dd < d )
                    {
                        d = dd;
                        nearPosX = x + i;
                        nearPosY = fy - 1;
                    }
                }
            }


            for ( int i = 1 ; i < fy - y - 1 ; i++ )
            {
                if ( !isBlockPos( x , y + i , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x - posX ) + Mathf.Abs( y + i - posY );
                    if ( dd < d )
                    {
                        d = dd;
                        nearPosX = x;
                        nearPosY = y + i;
                    }
                }
            }


            for ( int i = 1 ; i < fy - y - 1 ; i++ )
            {
                if ( !isBlockPos( fx - 1 , y + i , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( fx - 1 - posX ) + Mathf.Abs( y + i - posY );
                    if ( dd < d )
                    {
                        d = dd;
                        nearPosX = fx - 1;
                        nearPosY = y + i;
                    }
                }
            }

            if ( nearPosX > 0 )
                return true;

            dis++;
        }

        return false;
    }


    public bool findNearPos(int posX , int posY , int px , int py , byte bc )
    {
        int dis = 1;
        int range = 128;
        int d = 9999;
        int d1 = 9999;
        checkBlock = bc;

        int x1 = posX;
        int y1 = posY;
        int x2 = posX + 1;
        int y2 = posY + 1;

        nearPosX = GameDefine.INVALID_ID;
        nearPosY = GameDefine.INVALID_ID;

        checkUnit = true;

        while ( range > dis )
        {
            int x = x1 - dis;
            int y = y1 - dis;
            int fx = x2 + dis;
            int fy = y2 + dis;

            for ( int i = 0 ; i < fx - x ; i++ )
            {
                if ( !isBlockPos( x + i , y , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x + i - posX ) + Mathf.Abs( y - posY );
                    int dd1 = Mathf.Abs( x + i - px ) + Mathf.Abs( y - py );

                    if ( ( dd < d && dd1 <= d1 ) || ( dd == d && dd1 < d1 ) )
                    {
                        d = dd;
                        d1 = dd1;
                        nearPosX = x + i;
                        nearPosY = y;
                    }
                }
            }


            for ( int i = 0 ; i < fx - x ; i++ )
            {
                if ( !isBlockPos( x + i , fy - 1 , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x + i - posX ) + Mathf.Abs( fy - 1 - posY );
                    int dd1 = Mathf.Abs( x + i - px ) + Mathf.Abs( fy - 1 - py );

                    if ( ( dd < d && dd1 <= d1 ) || ( dd == d && dd1 < d1 ) )
                    {
                        d = dd;
                        d1 = dd1;
                        nearPosX = x + i;
                        nearPosY = fy - 1;
                    }
                }
            }


            for ( int i = 1 ; i < fy - y - 1 ; i++ )
            {
                if ( !isBlockPos( x , y + i , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( x - posX ) + Mathf.Abs( y + i - posY );
                    int dd1 = Mathf.Abs( x - px ) + Mathf.Abs( y + i - py );

                    if ( ( dd < d && dd1 <= d1 ) || ( dd == d && dd1 < d1 ) )
                    {
                        d = dd;
                        d1 = dd1;
                        nearPosX = x;
                        nearPosY = y + i;
                    }
                }
            }


            for ( int i = 1 ; i < fy - y - 1 ; i++ )
            {
                if ( !isBlockPos( fx - 1 , y + i , checkBlock , null ) )
                {
                    int dd = Mathf.Abs( fx - 1 - posX ) + Mathf.Abs( y + i - posY );
                    int dd1 = Mathf.Abs( fx - 1 - px ) + Mathf.Abs( y + i - py );

                    if ( ( dd < d && dd1 <= d1 ) || ( dd == d && dd1 < d1 ) )
                    {
                        d = dd;
                        d1 = dd1;
                        nearPosX = fx - 1;
                        nearPosY = y + i;
                    }
                }
            }

            if ( nearPosX > 0 )
                return true;

            dis++;
        }

        return false;
    }


}

