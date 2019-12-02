using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleUnitAttackSelection : Singleton<GameBattleUnitAttackSelection>
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

    List<Cell> list = new List<Cell>();

    Cell attackCell = new Cell();

    public Cell AttackCell { get { return attackCell; } }

    public bool IsShow { get { return isShow; } }

    public List<Cell> Cells { get { return list; } }


    public override void initSingleton()
    {
        gameObject.SetActive( false );
    }

    public void clear()
    {
        GameDefine.DestroyAll( transform );
    }

    public bool checkCell( int x , int y )
    {
        for ( int i = 0 ; i < list.Count ; i++ )
        {
            if ( list[ i ].x == x && list[ i ].y == y )
            {
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
        Color c = camp == GameUnitCampType.User ? new Color( 1.0f , 0.0f , 1.0f , 0.5f ) : new Color( 1.0f , 0.0f , 0.0f , 0.5f );
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

    public bool canAttack( GameBattleUnit u , GameBattleUnit t )
    {
        if ( u.BattleAIType == GameBattleAIType.AIStay )
        {
            return false;
        }

        if ( u.MoveBurst > 1 )
        {
            return false;
        }

        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = 1;
        int max = 1;

        GameAttackRangeType type = GameAttackRangeType.Circle;

        if ( u.Weapon != null )
        {
            min = ( u.Weapon.AttackRangeMin == GameDefine.INVALID_ID ? 1 : u.Weapon.AttackRangeMin + 1 );
            max = u.Weapon.AttackRangeMax;
            type = u.Weapon.AttackRangeType;

            if ( min > max )
            {
                min = max;
            }
        }
        else
        {
            return false;
        }

        int x = u.PosX;
        int y = u.PosY;

        if ( type == GameAttackRangeType.Line )
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
        else
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

        }


        for ( int i = 0 ; i < list.Count ; i++ )
        {
            if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                list[ i ].x >= width || list[ i ].y >= height )
            {
                continue;
            }

            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( list[ i ].x , list[ i ].y );

            if ( unit == t )
            {
                return true;
            }
        }

        return false;
    }

    public GameBattleUnit canAttack( int x , int y , GameItem item , GameUnitCampType camp )
    {
        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = 1;
        int max = 1;

        GameAttackRangeType type = GameAttackRangeType.Circle;

        if ( item != null )
        {
            min = ( item.AttackRangeMin == GameDefine.INVALID_ID ? 1 : item.AttackRangeMin + 1 );
            max = item.AttackRangeMax;
            type = item.AttackRangeType;
        }

        if ( type == GameAttackRangeType.Line )
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
        else
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

        }


        for ( int i = 0 ; i < list.Count ; i++ )
        {
            if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                list[ i ].x >= width || list[ i ].y >= height )
            {
                continue;
            }

            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( list[ i ].x , list[ i ].y );

            if ( unit != null && 
                unit.UnitCampType != camp )
            {
                return unit;
            }
        }

        return null;
    }

    public void show( int x , int y , GameItem item , GameUnitCampType camp , bool fade )
    {
        gameObject.SetActive( true );

        clear();

        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = 1;
        int max = 1;

        GameAttackRangeType type = GameAttackRangeType.Circle;

        if ( item != null )
        {
            min = ( item.AttackRangeMin == GameDefine.INVALID_ID ? 1 : item.AttackRangeMin + 1 );
            max = item.AttackRangeMax;
            type = item.AttackRangeType;
        }

        if ( type == GameAttackRangeType.Line )
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
        else
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
                i++;
            }
        }

        int dis = 99;
        attackCell.x = x;
        attackCell.y = y;


        for ( int i = 0 ; i < list.Count ; i++ )
        {
            addCell( list[ i ].x , list[ i ].y , camp );

            int d = Mathf.Abs( x - list[ i ].x ) + Mathf.Abs( y - list[ i ].y );

            if ( dis > d )
            {
                GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( list[ i ].x , list[ i ].y );

                if ( unit != null && 
                    unit.UnitCampType != camp )
                {
                    dis = d;
                    attackCell.x = list[ i ].x;
                    attackCell.y = list[ i ].y;
                }
            }
        }



        fadeCell( fade );

        isShow = true;

        GameBattleCursor.instance.show( 0 );
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
