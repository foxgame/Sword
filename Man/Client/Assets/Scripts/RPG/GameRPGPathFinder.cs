using UnityEngine;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;
using System;


public class GameRPGPathFinder : Singleton<GameRPGPathFinder>
{
    public const byte BLOCK = 1;

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


    public bool fly = false;

    public short[] pathResult;

    private PathNode resultNode = null;
    private PathNode topNode = null;

    private PathNode[] openResult = null;
    private PathNode[] open = null;

    private byte[] close;

    private byte[] mapData = null;

    private int maxX = 0;
    private int maxY = 0;

    private int step = 0;

    private int openCount;
    private int openResultCount;

    private bool checkCell;
    private bool checkUnit;

    private int startPosX;
    private int startPosY;
    private int endPosX;
    private int endPosY;

    public int nearPosX;
    public int nearPosY;


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
        if ( fly )
        {
            return false;
        }

        return c == BLOCK;
    }

    public bool isBlock( int x , int y )
    {
        int i = getIndex( x , y );

        if ( i == GameDefine.INVALID_ID )
        {
            return false;
        }

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

    public void setBlock( int x , int y , bool b )
    {
        int i = getIndex( x , y );

        if ( i == GameDefine.INVALID_ID )
        {
            return;
        }

        mapData[ getIndex( x , y ) ] = b ? BLOCK : (byte)0;
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


            if ( fly )
            {
                if ( count == 10000 )
                {
                    resultNode = null;
                    return;
                }
            }
            else
            {
                if ( count == 1000 )
                {
                    resultNode = null;
                    return;
                }
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
        // s
        int posX = parent.posX;
        int posY = parent.posY;

        ++posY;
        int find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // n
        posX = parent.posX;
        posY = parent.posY;
        --posY;
        find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // w
        posX = parent.posX;
        posY = parent.posY;
        --posX;
        find1 = find( v , parent , posX , posY , 10 );
        if ( find1 > 0 )
            return find1;

        // e
        posX = parent.posX;
        posY = parent.posY;
        ++posX;
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

    public int findPath( int sx , int sy , int ex , int ey , bool cc = false , bool cu = true )
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

        checkUnit = cu;
        checkCell = cc;

        setStartEnd( sx , sy , ex , ey );
        findPath();

        if ( resultNode == null )
        {
            return 0;
        }

        int count = 0;
        PathNode node = resultNode;

        while ( node != null )
        {
            pathResult[ count ] = node.posX; count++;
            pathResult[ count ] = node.posY; count++;

            node = node.parent;
        }

        return count;
    }
        
    public bool findNearPos( int posX , int posY )
    {
        int dis = 1;
        int range = 128;
        int d = 9999;

        int x1 = posX - 1;
        int y1 = posY - 1;
        int x2 = posX + 1;
        int y2 = posY + 1;

        nearPosX = GameDefine.INVALID_ID;
        nearPosY = GameDefine.INVALID_ID;

        checkUnit = true;
        checkCell = false;

        while ( range > dis )
        {
            int x = x1 - dis;
            int y = y1 - dis;
            int fx = x2 + dis;
            int fy = y2 + dis;

            for ( int i = 0 ; i < fx - x ; i++ )
            {
                if ( !isBlock( x + i , y ) )
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
                if ( !isBlock( x + i , fy - 1 ) )
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
                if ( !isBlock( x , y + i ) )
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
                if ( !isBlock( fx - 1 , y + i ) )
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

}

