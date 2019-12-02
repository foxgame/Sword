using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleUnitSkillSelection : Singleton<GameBattleUnitSkillSelection>
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

    int targetID = GameDefine.INVALID_ID;

    List<GameBattleUnit> attackUnits = new List<GameBattleUnit>();
    GameBattleAttackMapDirection attackDirection;
    Cell attackCell = new Cell();
    Cell attackMoveCell = new Cell();

    public bool IsShow { get { return isShow; } }
    public List<GameBattleUnit> AttackUnits { get { return attackUnits; } }
    public GameBattleAttackMapDirection AttackDirection { get { return attackDirection; } }
    public Cell AttackCell { get{ return attackCell; } }
    public Cell AttackMoveCell { get { return attackMoveCell; } }

    public bool IsAttackUnits { get; set; }

    bool checkUnitAI = false;

    public override void initSingleton()
    {
        gameObject.SetActive( false );
    }

    public void clear()
    {
        GameDefine.DestroyAll( transform );
    }

    public void getUnits( GameBattleUnit unit , GameSkill skill )
    {
        attackUnits.Clear();

        switch ( skill.AttackRangeType )
        {
            case GameAttackRangeType.Circle:
                {
                    int size = skill.AttackRange;

                    for ( int i = -size ; i <= size ; i++ )
                    {
                        for ( int j = -size ; j <= size ; j++ )
                        {
                            int xx = Mathf.Abs( j );
                            int yy = Mathf.Abs( i );

                            if ( xx + yy <= size )
                            {
                                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( attackCell.x + j ,
                                    attackCell.y + i );

                                switch ( skill.TargetType )
                                {
                                    case GameTargetType.User:
                                        {
                                            if ( u != null &&
                                                u.UnitCampType == unit.UnitCampType )
                                            {
                                                attackUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Enemy:
                                        {
                                            if ( u != null &&
                                                u.UnitCampType != unit.UnitCampType )
                                            {
                                                attackUnits.Add( u );
                                            }
                                        }
                                        break;
                                    case GameTargetType.Summon:
                                        {
                                            if ( u == null )
                                            {
                                                attackUnits.Add( u );
                                            }
                                        }
                                        break;
                                }

                            }
                        }
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    int x = 0;
                    int y = 0;

                    switch ( attackDirection )
                    {
                        case GameBattleAttackMapDirection.North:
                            y = -1;
                            break;
                        case GameBattleAttackMapDirection.East:
                            x = 1;
                            break;
                        case GameBattleAttackMapDirection.South:
                            y = 1;
                            break;
                        case GameBattleAttackMapDirection.West:
                            x = -1;
                            break;
                    }

                    for ( int i = 1 ; i <= skill.AttackRangeMax ; i++ )
                    {
                        GameBattleUnit u = GameBattleUnitManager.instance.getUnit( unit.PosX + i * x , unit.PosY + i * y );

                        switch ( skill.TargetType )
                        {
                            case GameTargetType.User:
                                {
                                    if ( u != null &&
                                        u.UnitCampType == unit.UnitCampType )
                                    {
                                        attackUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Enemy:
                                {
                                    if ( u != null &&
                                        u.UnitCampType != unit.UnitCampType )
                                    {
                                        attackUnits.Add( u );
                                    }
                                }
                                break;
                            case GameTargetType.Summon:
                                {
                                    if ( u == null )
                                    {
                                        attackUnits.Add( u );
                                    }
                                }
                                break;
                        }
                    }
                }
                break;
        }

    }


    public void canAttackAI( GameBattleUnit unit1 , int ux , int uy , int x , int y , GameSkill sk , GameUnitCampType camp , int id )
    {
        targetID = id;

        IsAttackUnits = false;
        checkUnitAI = false;

        list.Clear();

        int width = GameBattleManager.instance.Width;
        int height = GameBattleManager.instance.Height;

        int min = ( sk.AttackRangeMin == GameDefine.INVALID_ID ? 1 : sk.AttackRangeMin + 1 );
        int max = sk.AttackRangeMax;

        if ( min > max )
        {
            min = max;
        }

        rangeType = sk.AttackRangeType;
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

                    if ( sk.AttackRangeMin == 0 )
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

                        getUnitsCircleAI( unit1 , ux , uy , x , y , list[ i ].x , list[ i ].y , sk , camp );
                    }
                }
                break;
            case GameAttackRangeType.Line:
                {
                    getUnitsLineAI( unit1 , ux , uy , sk , camp );
                }
                break;
        }

        if ( checkUnitAI )
        {
            for ( int i = 0 ; i < attackUnits.Count ; i++ )
            {
                GameBattleUnit unit = attackUnits[ i ];

                GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( unit.MoveType );
                bool fly = unit.checkEffect( GameSkillResutlEffect.Under ) ? true : unitMove.fly;

                List<GameBattleUnitMovement.Cell> cells = GameBattleUnitMovement.instance.getMoveList(
                    unit.PosX , unit.PosY , unit.Move , unit.MoveType , fly , unit.UnitCampType );

                for ( int j = 0 ; j < cells.Count ; j++ )
                {
                    GameBattleUnit u = GameBattleUnitAttackSelection.instance.canAttack( 
                        cells[ j ].x , cells[ j ].y , unit.Weapon , unit.UnitCampType );

                    if ( u != null )
                    {
                        return;
                    }
                }
            }

            list.Clear();
            attackUnits.Clear();
        }
    }

    public bool checkSkillAI( GameBattleUnit unit1 , GameBattleUnit unit , GameSkill sk , GameUnitCampType camp )
    {
        switch ( sk.TargetType )
        {
            case GameTargetType.User:
                {
                    if ( unit == null ||
                        unit.UnitCampType != camp )
                    {
                        return false;
                    }

                    switch ( sk.ResultType )
                    {
                        case GameSkillResutlType.Damage:
                            break;
                        case GameSkillResutlType.Cure:
                            {
                                if ( unit.HPMax - unit.HP > sk.BasePower || 
                                    ( unit.HP / (float)unit.HPMax ) < 0.4f )
                                {
                                    return true;
                                }

                                return false;
                            }
                        case GameSkillResutlType.Purge:
                            {
                                if ( unit.checkEffect( GameSkillResutlEffect.Poison ) ||
                                    unit.checkEffect( GameSkillResutlEffect.Palsy ) ||
                                    unit.checkEffect( GameSkillResutlEffect.Silence ) ||
                                    unit.checkEffect( GameSkillResutlEffect.Fetter ) )
                                {
                                    return true;
                                }

                                return false;
                            }
                        case GameSkillResutlType.Blessing:
                            {
                                if ( unit.checkEffect( sk.ResultEffect ) )
                                {
                                    return false;
                                }

                                checkUnitAI = true;

                                return true;
                            }
                        case GameSkillResutlType.Curse:
                            break;
                        case GameSkillResutlType.None:
                        case GameSkillResutlType.Special:
                            {
                                switch ( sk.OtherEffect )
                                {
                                    case GameSkillOtherEffect.Invincible0:
                                        {
                                            if ( unit.checkEffect( GameSkillResutlEffect.PhyImmunity ) )
                                            {
                                                return false;
                                            }

                                            checkUnitAI = true;

                                            return true;
                                        }
                                    case GameSkillOtherEffect.Invincible1:
                                        {
                                            if ( unit.checkEffect( GameSkillResutlEffect.MagImmunity ) )
                                            {
                                                return false;
                                            }

                                            checkUnitAI = true;

                                            return true;
                                        }
                                    case GameSkillOtherEffect.HealAll:
                                        {
                                            if ( unit.HPMax - unit.HP > sk.BasePower || 
                                                ( unit.HP / (float)unit.HPMax ) < 0.4f )
                                            {
                                                return true;
                                            }

                                            return false;
                                        }
                                    case GameSkillOtherEffect.MPAdd:
                                        {
                                            if ( unit.MPMax - unit.MP > sk.BasePower ||
                                                ( unit.MP / (float)unit.MPMax ) < 0.4f )
                                            {
                                                return true;
                                            }

                                            return false;
                                        }
                                    case GameSkillOtherEffect.AbilityUp:
                                        {
                                            if ( unit.checkEffectOther( GameSkillOtherEffect.AbilityUp ) )
                                            {
                                                return false;
                                            }

                                            checkUnitAI = true;

                                            return true;
                                        }
                                }


                                switch ( sk.ResultEffect )
                                {
                                    case GameSkillResutlEffect.Clear:
                                        break;
                                    case GameSkillResutlEffect.PhyImmunity:
                                    case GameSkillResutlEffect.MagImmunity:
                                    case GameSkillResutlEffect.Miss:
                                    case GameSkillResutlEffect.SummonKiller:
                                    case GameSkillResutlEffect.Under:
                                        {
                                            if ( unit.checkEffect( sk.ResultEffect ) )
                                            {
                                                return false;
                                            }

                                            checkUnitAI = true;

                                            return true;
                                        }
                                }
                            }
                            break;
                    }

                    

                }
                break;
            case GameTargetType.Enemy:
                {
                    if ( unit == null ||
                        unit.UnitCampType == camp )
                    {
                        return false;
                    }

                    if ( sk.Type == GameSkillType.Physical )
                    {
                        return true;
                    }

                    switch ( sk.ResultType )
                    {
                        case GameSkillResutlType.Damage:
                            {
                                return true;
                            }
                        case GameSkillResutlType.Cure:
                            break;
                        case GameSkillResutlType.Purge:
                            break;
                        case GameSkillResutlType.Blessing:
                            break;
                        case GameSkillResutlType.Curse:
                            {
                                if ( unit.checkEffect( sk.ResultEffect ) )
                                {
                                    return false;
                                }

                                return true;
                            }
                        case GameSkillResutlType.None:
                        case GameSkillResutlType.Special:
                            {

                                switch ( sk.OtherEffect )
                                {
                                    case GameSkillOtherEffect.AttackX2:
                                    case GameSkillOtherEffect.AttackX3:
                                    case GameSkillOtherEffect.AttackX4:
                                    case GameSkillOtherEffect.AttackPalsy:
                                    case GameSkillOtherEffect.AttackPoison:
                                    case GameSkillOtherEffect.AttackHP:
                                    case GameSkillOtherEffect.AttackFetter:
                                    case GameSkillOtherEffect.MP:
                                        {
                                            return true;
                                        }
                                    case GameSkillOtherEffect.KillSelf:
                                        {
                                            return ( (float)unit1.HP / unit1.HPMax ) <= 0.5f;
                                        }
                                    case GameSkillOtherEffect.AttackSilence:
                                        {
                                            if ( unit.checkEffect( GameSkillResutlEffect.Silence ) )
                                            {
                                                return false;
                                            }

                                            return true;
                                        }
                                    case GameSkillOtherEffect.AbilityDown:
                                        {
                                            if ( unit.checkEffect( GameSkillResutlEffect.StrUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.VitUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.IntUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.MoveUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.Violent ) )
                                            {
                                                return true;
                                            }

                                            return false;
                                        }
                                    case GameSkillOtherEffect.HealAll:
                                    case GameSkillOtherEffect.AbilityUp:
                                    case GameSkillOtherEffect.Invincible0:
                                    case GameSkillOtherEffect.Invincible1:
                                        break;
                                }

                                switch ( sk.ResultEffect )
                                {
                                    case GameSkillResutlEffect.Clear:
                                        {
                                            if ( unit.checkEffect( GameSkillResutlEffect.StrUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.VitUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.IntUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.MoveUp ) ||
                                                unit.checkEffect( GameSkillResutlEffect.Violent ) )
                                            {
                                                return true;
                                            }

                                            return false;
                                        }
                                    case GameSkillResutlEffect.PhyImmunity:
                                    case GameSkillResutlEffect.MagImmunity:
                                    case GameSkillResutlEffect.Miss:
                                    case GameSkillResutlEffect.SummonKiller:
                                    case GameSkillResutlEffect.Under:
                                        {
                                        }
                                        break;
                                }


                            }
                            break;
                    }
                }
                break;
            case GameTargetType.Summon:
                {
                    if ( unit != null )
                    {
                        return false;
                    }

                    return true;
                }
        }

        return false;
    }

    public void getUnitsLineAI( GameBattleUnit unit , int ux , int uy , GameSkill sk , GameUnitCampType camp )
    {
        if ( targetID != GameDefine.INVALID_ID )
        {
            for ( int j = 0 ; j < attackUnits.Count ; j++ )
            {
                if ( attackUnits[ j ].BattleManID == targetID )
                {
                    return;
                }
            }
        }

        List<GameBattleUnit> list1 = new List<GameBattleUnit>();

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


            for ( int j = 0 ; j < sk.AttackRangeMax ; j++ )
            {
                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( ux + i * x1 , uy + i * y1 );

                if ( checkSkillAI( unit , u , sk , camp ) )
                {
                    list1.Add( u );
                }
            }

            bool bTarget = false;

            if ( targetID != GameDefine.INVALID_ID )
            {
                for ( int j = 0 ; j < list1.Count ; j++ )
                {
                    if ( list1[ j ].BattleManID == targetID )
                    {
                        bTarget = true;
                    }
                }
            }


            if ( list1.Count > attackUnits.Count || bTarget )
            {
                attackCell.x = ux + x1;
                attackCell.y = uy + y1;

                attackMoveCell.x = ux;
                attackMoveCell.y = uy;

                attackDirection = (GameBattleAttackMapDirection)i;

                attackUnits.Clear();

                IsAttackUnits = true;

                for ( int k = 0 ; k < list1.Count ; k++ )
                {
                    attackUnits.Add( list1[ k ] );
                }
            }

            list1.Clear();

        }
        
    }

    public void getUnitsCircleAI( GameBattleUnit unit , int ux , int uy , int mx , int my , int x , int y , GameSkill sk , GameUnitCampType camp )
    {
        if ( targetID != GameDefine.INVALID_ID )
        {
            for ( int j = 0 ; j < attackUnits.Count ; j++ )
            {
                if ( attackUnits[ j ].BattleManID == targetID )
                {
                    return;
                }
            }
        }

        attackDirection = GameBattleAttackMapDirection.Count;

        List<GameBattleUnit> list1 = new List<GameBattleUnit>();

        int size = sk.AttackRange;

        for ( int i = -size ; i <= size ; i++ )
        {
            for ( int j = -size ; j <= size ; j++ )
            {
                int xx = Mathf.Abs( j );
                int yy = Mathf.Abs( i );

                if ( xx + yy <= size )
                {
                    GameBattleUnit u = GameBattleUnitManager.instance.getUnit( x + j , y + i );

                    if ( checkSkillAI( unit , u , sk , camp ) )
                    {
                        list1.Add( u );
                    }
                }
            }
        }

        bool bTarget = false;

        if ( targetID != GameDefine.INVALID_ID )
        {
            for ( int j = 0 ; j < list1.Count ; j++ )
            {
                if ( list1[ j ].BattleManID == targetID )
                {
                    bTarget = true;
                }
            }
        }

        if ( bTarget || list1.Count > attackUnits.Count ||
           ( list1.Count > 0 && list1.Count == attackUnits.Count &&
           Mathf.Abs( ux - x ) + Mathf.Abs( uy - y ) < Mathf.Abs( ux - attackCell.x ) + Mathf.Abs( uy - attackCell.y ) ) ||
           
           ( list1.Count > 0 && list1.Count == attackUnits.Count && Random.Range( 0 , 100 ) > 50 &&
           Mathf.Abs( ux - x ) + Mathf.Abs( uy - y ) == Mathf.Abs( ux - attackCell.x ) + Mathf.Abs( uy - attackCell.y ) )
           )
        {
            attackCell.x = x;
            attackCell.y = y;

            attackMoveCell.x = mx;
            attackMoveCell.y = my;

            attackUnits.Clear();

            IsAttackUnits = true;

            for ( int i = 0 ; i < list1.Count ; i++ )
            {
                attackUnits.Add( list1[ i ] );
            }
        }

        list1.Clear();
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

    public void show( int x , int y , GameSkill m , GameUnitCampType camp , bool fade )
    {
        gameObject.SetActive( true );

        clear();

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
                    if ( m.AttackRangeMin == 0 )
                    {
                        Cell cell = new Cell();
                        cell.x = x;
                        cell.y = y;
                        list.Add( cell );
                    }

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

        int size = m.AttackRange;

        for ( int i = 0 ; i < list.Count ; )
        {
            if ( list[ i ].x < 0 || list[ i ].y < 0 ||
                list[ i ].x >= width || list[ i ].y >= height )
            {
                list.RemoveAt( i );
                continue;
            }

            if ( m.ResultEffect == GameSkillResutlEffect.Summon &&
                GameBattlePathFinder.instance.isBlockPos( list[ i ].x , list[ i ].y , GameBattlePathFinder.BLOCK ) )
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
                    switch ( m.TargetType )
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
                    switch ( m.TargetType )
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
                                    switch ( m.TargetType )
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
                                    switch ( m.TargetType )
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

        GameBattleCursor.instance.show( m.AttackRange );
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
