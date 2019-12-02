using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class GameBattleUnitData
{
    public int HP;
    public int HPMax;

    public int MP;
    public int MPMax;

    public int Move;
    public int MoveMax;

    public int Attack;
    public int Defence;
    public int Hit;
    public int Miss;
    public int Critical;
    public int Double;

    public int Str;
    public int Int;
    public int Avg;
    public int Vit;
    public int Luk;

    public GameAttributeType AttributeType;

    public short[] AttributeDefence = new short[ (int)GameAttributeType.Count ];


    public void clear()
    {
        HPMax = 0;
        MPMax = 0;
        MoveMax = 0;

        Attack = 0;
        Defence = 0;
        Hit = 0;
        Miss = 0;
        Critical = 0;
        Double = 0;

        Str = 0;
        Int = 0;
        Avg = 0;
        Vit = 0;
        Luk = 0;

        AttributeType = GameAttributeType.None;

        for ( int i = 0 ; i < (int)GameAttributeType.Count ; i++ )
        {
            AttributeDefence[ i ] = 0;
        }
    }
}

public partial class GameBattleUnit : MonoBehaviour
{
    [SerializeField]
    GameBattleUnitData unitData = new GameBattleUnitData();

    [SerializeField]
    GameUnit gameUnit;

    [SerializeField]
    GameBattleMan gameBattleMan;

    GameAnimation gameAnimation;
    GameBattleMovement gameMovement;

    SpriteRenderer shadowSprite;

    short tempExp = 0;

    [SerializeField]
    bool isShow = false;
    [SerializeField]
    bool isMoved = false;
    [SerializeField]
    bool isKilled = false;

    [SerializeField]
    bool isActed = false;
    [SerializeField]
    bool isSummon = false;
    [SerializeField]
    bool isCopy = false;

    [SerializeField]
    bool isHurt = false;

    [SerializeField]
    bool isBoss = false;

    [SerializeField]
    bool battleAICheckFight = false;


    int lastMove = 0;

    public void Awake()
    {
        gameAnimation = GetComponent<GameAnimation>();
        gameMovement = GetComponent<GameBattleMovement>();
    }

    public short TempExp { get { return tempExp; } }
    public bool IsLevelUp
    {
        get
        {
            if ( LV == GameDefine.MAX_LEVEL )
            {
                tempExp = 0;
                return false;
            }

            return tempExp + gameBattleMan.UnitBase.Exp >= GameDefine.MAX_EXP;
        }
    }

    public int WeaponSlot
    {
        get
        {
            return gameBattleMan.UnitBase.WeaponSlot;
        }
    }

    public int ArmorSlot
    {
        get
        {
            return gameBattleMan.UnitBase.ArmorSlot;
        }
    }

    public int AccessorySlot
    {
        get
        {
            return gameBattleMan.UnitBase.AccessorySlot;
        }
    }

    public GameItem Weapon
    {
        get
        {
            if ( gameBattleMan.UnitBase.Weapon != GameDefine.INVALID_ID )
            {
                return GameItemData.instance.getData( gameBattleMan.UnitBase.Weapon );
            }

            return null;
        }
    }

    public short WeaponID
    {
        get
        {
            return gameBattleMan.UnitBase.Weapon;
        }
        set
        {
            gameBattleMan.UnitBase.Weapon = value;
        }
    }

    public short ArmorID
    {
        get
        {
            return gameBattleMan.UnitBase.Armor;
        }
        set
        {
            gameBattleMan.UnitBase.Armor = value;
        }
    }

    public short AccessoryID
    {
        get
        {
            return gameBattleMan.UnitBase.Accessory;
        }
        set
        {
            gameBattleMan.UnitBase.Accessory = value;
        }
    }

    

    public GameItem Armor
    {
        get
        {
            if ( gameBattleMan.UnitBase.Armor != GameDefine.INVALID_ID )
            {
                return GameItemData.instance.getData( gameBattleMan.UnitBase.Armor );
            }

            return null;
        }
    }

    public GameItem Accessory
    {
        get
        {
            if ( gameBattleMan.UnitBase.Accessory != GameDefine.INVALID_ID )
            {
                return GameItemData.instance.getData( gameBattleMan.UnitBase.Accessory );
            }

            return null;
        }
    }


    public GameAnimationDirection Direction { get { return gameAnimation.animationDirection; } }

    public short[] Skill { get { return gameBattleMan.UnitBase.Skills; } }
    public short[] Items { get { return gameBattleMan.UnitBase.Items; } }

    public int LV { get{ return gameBattleMan.UnitBase.LV + gameUnit.BaseLv; } }
    public int EXPBase { get { return gameUnit.BaseExp; } }
    public int EXP { get { return gameBattleMan.UnitBase.Exp; } }

    public int Str { get { return unitData.Str; } }
    public int Vit { get { return unitData.Vit; } }
    public int Int { get { return unitData.Int; } }
    public int Avg { get { return unitData.Avg; } }
    public int Luk { get { return unitData.Luk; } }

    public bool InTeam { get { return gameBattleMan.UnitBase.InTeam == 1; } }
    public bool IsMoving { get { return gameMovement.isMoving; } }

    public int PosX { get { return gameMovement.PosX; } }
    public int PosY { get { return gameMovement.PosY; } }

    public float PosBattleX { get { return gameMovement.PosBattleX; } }
    public float PosBattleY { get { return gameMovement.PosBattleY; } }

    public bool IsSummon { get { return isSummon; } set { isSummon = value; } }
    public bool IsActed { get { return isActed; } set { isActed = value; } }
    public bool IsCopy { get { return isCopy; } set { isCopy = value; } }
    public bool IsHurt { get { return isHurt; } set { isHurt = value; } }
    public bool IsBoss { get { return isBoss; } }

    public bool BattleAICheckFight { get { return battleAICheckFight; } set { battleAICheckFight = value; } }

    public bool IsAI
    {
        get
        {
            return BattleAIType != GameBattleAIType.None;
        }
    }

    public bool IsAlive
    {
        get
        {
            return isShow && !isKilled;
        }
    }

    public bool IsShow
    {
        get
        {
            return isShow;
        }
        set
        {
            isShow = value;

            if ( GameUserData.instance.Stage > 15 && 
                UnitID == 8 && 
                GameUserData.instance.getUnitBase( 8 ).InTeam == 0 )
            {
                // fix 夏侯仪 bug

                isShow = false;
            }
        }
    }

    public bool IsKilled
    {
        get
        {
            return isKilled;
        }
        set
        {
            isKilled = value;
        }
    }


    public bool IsMoved { get { return isMoved; } set { isMoved = value; } }


    public bool IsUser { get { return gameBattleMan.BattleAIType == GameBattleAIType.None && gameUnit.UnitCampType == GameUnitCampType.User; } }
    public bool IsEnemy { get { return gameUnit.UnitCampType == GameUnitCampType.Enemy; } }
    public bool IsNpc { get { return gameBattleMan.BattleAIType != GameBattleAIType.None && gameUnit.UnitCampType == GameUnitCampType.User; } }

    public GameUnitCampType UnitCampType { get { return gameUnit.UnitCampType; } }
    public GameBattleAIType BattleAIType { get { return gameBattleMan.BattleAIType; } set { gameBattleMan.BattleAIType = value; } }

    public GameUnitAttackType AttackType { get { return gameUnit.AttackType; } }
    public int Sprite { get { return gameUnit.Sprite; } }
    public string Name { get { return gameUnit.Name; } }

    public int UnitID { get { return gameUnit.UnitID; } }
    public int BattleManID { get { return gameBattleMan.BattleManID; } }

    public int KillEvent { get { return gameBattleMan.killEvent; } }

    public short KillGetType { get { return gameBattleMan.KillGetType; } }
    public short KillGetParm1 { get { return gameBattleMan.KillGetParm1; } }

    public short AIMoveToX { get { return gameBattleMan.moveToX; } set { gameBattleMan.moveToX = value; } }
    public short AIMoveToY { get { return gameBattleMan.moveToY; } set { gameBattleMan.moveToY = value; } }
    public short AIMoveToID { get { return gameBattleMan.moveToID; } set { gameBattleMan.moveToID = value; } }
    public short AIMoveToIDUnkonw { get { return gameBattleMan.moveToIDUnkonw; } set { gameBattleMan.moveToIDUnkonw = value; } }

    public short AITargetID
    {
        get
        {
            if ( AIMoveToID != GameDefine.INVALID_ID )
            {
                GameBattleUnit u = GameBattleUnitManager.instance.getUnit( AIMoveToID );

                if ( u != null && u.UnitCampType != UnitCampType )
                {
                    return AIMoveToID;
                }
            }

            return GameDefine.INVALID_ID;
        }
    }

    public void giveItem( int slot , GameBattleUnit unit )
    {
        gameBattleMan.UnitBase.giveItem( slot , unit.gameBattleMan.UnitBase );
    }

    public bool canAddItem()
    {
//         if ( isSummon )
//         {
//             return false;
//         }

        return gameBattleMan.UnitBase.canAddItem();
    }

    public bool canUseItem( GameItem item )
    {
        if ( item.UseTargetType == GameTargetType.None )
        {
            return false;
        }

        if ( MP < item.UseMPCost )
        {
            return false;
        }

        return true;
    }


    public bool canEquipItem( GameItem item )
    {
        return item.canEquip( UnitID );
    }

    public void equipItem( GameItem item )
    {
        if ( !canEquipItem( item ) )
        {
            return;
        }

        switch ( item.ItemType )
        {
            case GameItemType.Weapon:
                gameBattleMan.UnitBase.Weapon = item.ID;
                break;
            case GameItemType.Armor:
                gameBattleMan.UnitBase.Armor = item.ID;
                break;
            case GameItemType.Accessory:
                {
                    if ( gameBattleMan.UnitBase.Accessory == item.ID )
                    {
                        gameBattleMan.UnitBase.Accessory = GameDefine.INVALID_ID;
                    }
                    else
                    {
                        gameBattleMan.UnitBase.Accessory = item.ID;
                    }
                }
                break;
        }

        updateUnitData();
    }

    public void addItem( short id )
    {
        gameBattleMan.UnitBase.addItem( id );
    }

    public void removeItem( int slot )
    {
        gameBattleMan.UnitBase.removeItem( slot );
    }

    public void removeItemID( int id )
    {
        gameBattleMan.UnitBase.removeItemID( id );
    }

    public void useItemID( int id )
    {
        gameBattleMan.UnitBase.useItemID( id );
    }

    public void useItem( int slot )
    {
        gameBattleMan.UnitBase.useItem( slot );
    }

    public void init( GameUnit u , GameBattleMan m )
    {
        gameUnit = u;
        gameBattleMan = m;
    }

    public GameBattleUnitData getUnitData()
    {
        return unitData;
    }

    public void setGameUnit( GameUnit unit )
    {
        gameUnit = unit;
    }
    public GameUnit getGameUnit()
    {
        return gameUnit;
    }

    public void setUnitBase( GameUnitBase b )
    {
        gameBattleMan.UnitBase = b;
    }
    public GameUnitBase getUnitBase()
    {
        return gameBattleMan.UnitBase;
    }

    public void updateUnitData()
    {
        unitData.clear();

        unitData.HPMax = gameBattleMan.UnitBase.HP + gameBattleMan.HpAdd - gameBattleMan.HpSub;
        unitData.MPMax = gameBattleMan.UnitBase.MP + gameBattleMan.MpAdd - gameBattleMan.MpSub;

        unitData.Str = gameBattleMan.UnitBase.Str;
        unitData.Vit = gameBattleMan.UnitBase.Vit;
        unitData.Avg = gameBattleMan.UnitBase.Avg;
        unitData.Int = gameBattleMan.UnitBase.Int;
        unitData.Luk = gameBattleMan.UnitBase.Luk;

        GameAttributeDefence md = GameAttributeDefenceData.instance.getData( gameUnit.AttributeDefenceID );

        for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
        {
            if ( md != null )
                unitData.AttributeDefence[ i ] = md.AttributeDefence[ i ];
            else
                unitData.AttributeDefence[ i ] = 100;
        }

        if ( gameUnit.UnitID >= GameDefine.MAX_USER && !isCopy )
        {
            float difficulty = GameDefine.Difficulty;

            if ( !IsEnemy )
            {
                difficulty = 1.0f;
            }

            if ( !IsSummon )
            {
                unitData.HPMax += gameUnit.HP;
                unitData.MPMax += gameUnit.MP;
                unitData.Str += gameUnit.Str;
                unitData.Vit += gameUnit.Vit;
                unitData.Avg += gameUnit.Avg;
                unitData.Int += gameUnit.Int;
                unitData.Luk += gameUnit.Luk;
            }

            int lv = gameBattleMan.UnitBase.LV;

            if ( lv > 0 )
            {
                unitData.HPMax += gameUnit.HPGrow * lv;
                unitData.MPMax += gameUnit.MPGrow * lv;
                unitData.Str += gameUnit.StrGrow * lv;
                unitData.Vit += gameUnit.VitGrow * lv;
                unitData.Avg += gameUnit.AvgGrow * lv;
                unitData.Int += gameUnit.IntGrow * lv;
                unitData.Luk += gameUnit.LukGrow * lv;
            }

            unitData.HPMax = (int)( difficulty * unitData.HPMax );
            unitData.MPMax = (int)( difficulty * unitData.MPMax );
            unitData.Str = (int)( difficulty * unitData.Str );
            unitData.Vit = (int)( difficulty * unitData.Vit );
            unitData.Avg = (int)( difficulty * unitData.Avg );
            unitData.Int = (int)( difficulty * unitData.Int );
//            unitData.Luk = (int)( difficulty * unitData.Luk );
        }

        if ( unitData.HPMax >= 2500 )
        {
            isBoss = true;
        }

        unitData.AttributeType = GameAttributeType.None;

        unitData.MoveMax = unitData.Avg / 25 + gameUnit.Move;

        unitData.Attack = 0;
        unitData.Defence = 0;
        unitData.Hit = 0;
        unitData.Miss = 0;
        unitData.Critical = 0;
        unitData.Double = 0;


        GameItem weapon = Weapon;
        GameItem armor = Armor;
        GameItem accessory = Accessory;

        if ( weapon != null )
        {
            unitData.Attack += weapon.Attack;
            unitData.Defence += weapon.Defence;
            unitData.Hit += weapon.AttackHit;
            unitData.Miss += weapon.AttackMiss;
            unitData.Critical += weapon.AttackCritical;
            unitData.Double += weapon.AttackDouble;

            unitData.AttributeType = weapon.AttackAttributeType;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( weapon.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( armor != null )
        {
            unitData.Attack += armor.Attack;
            unitData.Defence += armor.Defence;
            unitData.Hit += armor.AttackHit;
            unitData.Miss += armor.AttackMiss;
            unitData.Critical += armor.AttackCritical;
            unitData.Double += armor.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( armor.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }

        if ( accessory != null )
        {
            unitData.Attack += accessory.Attack;
            unitData.Defence += accessory.Defence;
            unitData.Hit += accessory.AttackHit;
            unitData.Miss += accessory.AttackMiss;
            unitData.Critical += accessory.AttackCritical;
            unitData.Double += accessory.AttackDouble;

            for ( int i = 0 ; i <= (int)GameAttributeType.Dark ; i++ )
            {
                unitData.AttributeDefence[ i ] -= (short)( accessory.AttributeDefence[ i ] * AttributeDefence[ i ] / 100.0f );
            }
        }
    }


    public void initHPMP()
    {
        unitData.HP = unitData.HPMax;
        unitData.MP = unitData.MPMax;
    }

    public void initHPMP( int hp , int mp )
    {
        unitData.HP = hp;
        unitData.MP = mp;
    }

    public void clearMove()
    {
        unitData.Move = unitData.MoveMax;
    }

    public void clearSelection()
    {
        if ( IsActed || 
            IsKilled )
        {
            return;
        }

        if ( IsMoved )
        {
            IsMoved = false;
            gameMovement.originalPosition();

            unitData.Move = lastMove;
        }
    }

    public void setColor( Color c )
    {
        gameAnimation.setColor( c );
    }

    public void setAlpha( float a )
    {
        gameAnimation.setAlpha( a );
    }

    public void setPos( int x , int y )
    {
        gameMovement.setPos( x , y );
    }

    public void stopAnimation()
    {
        gameAnimation.stopAnimation();
    }

    public void clearAnimation()
    {
        gameAnimation.clearAnimation();
    }

    public void updateAnimation( GameAnimationDirection direction )
    {
        if ( !IsAlive )
        {
            gameAnimation.stopAnimation();
            gameAnimation.clearAnimation();

            return;
        }

        gameAnimation.playAnimationBattle( GameAnimationType.Stand , direction , null );

        if ( IsActed )
        {
            gameAnimation.stopAnimation();
        }

        updateEffects();
    }

    public void updateInteam()
    {
        if ( IsUser && UnitID < GameDefine.MAX_USER )
        {
            gameBattleMan.UnitBase.InTeam = 1;
        }
    }

    public void updateInteam1()
    {
        gameBattleMan.UnitBase.InTeam = 1;
    }

    public void updateAlive()
    {
        gameObject.SetActive( IsAlive );

        if ( IsAlive )
        {
            if ( shadowSprite == null )
            {
                GameObject obj1 = Resources.Load<GameObject>( "Prefab/Misc/Shadow" );
                GameObject obj = Instantiate<GameObject>( obj1 );

                Transform trans = obj.transform;
                trans.SetParent( transform );
                trans.localPosition = new Vector3( 15.0f , -21.0f , 1000.0f );
                trans.localScale = new Vector3( 0.6f , 0.6f , 0.6f );

                shadowSprite = obj.GetComponent<SpriteRenderer>();
            }
        }
        else
        {
            if ( shadowSprite != null )
            {
                shadowSprite.transform.SetParent( null );
                Destroy( shadowSprite.gameObject );
                shadowSprite = null;
            }
        }       
    }

    public void playAnimationBattle( OnEventOver over )
    {
        gameAnimation.playAnimationBattle( gameAnimation.animationType , gameAnimation.animationDirection , over );
    }

    public void playAnimationBattle( GameAnimationType type , GameAnimationDirection dir , OnEventOver over )
    {
        gameAnimation.playAnimationBattle( type , dir , over );
    }

    public void playAnimation( int s = 0 , int e = GameDefine.INVALID_ID , bool l = true , OnEventOver over = null )
    {
        gameAnimation.playAnimation( s , e , l , over );
    }

    public bool checkPosition( int x , int y )
    {
        return gameMovement.PosX == x &&
            gameMovement.PosY == y;
    }

    public bool moveToEvent( int x , int y , bool start , bool follow , int speed , OnEventOver over )
    {
        if ( GameUserData.instance.Stage > 15 &&
            UnitID == 8 &&
            GameUserData.instance.getUnitBase( 8 ).InTeam == 0 )
        {
            return false;
        }

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( MoveType );

        return gameMovement.moveToEvent( x , y , unitMove , start , follow , speed , over );
    }

    public bool moveTo( int x , int y , bool start , bool follow , int speed , OnEventOver over )
    {
        lastMove = unitData.Move;

        GameUnitMove unitMove = GameUnitMoveTypeData.instance.getData( MoveType );
        bool fly = checkEffect( GameSkillResutlEffect.Under ) ? true : unitMove.fly;

        int cost = gameMovement.moveTo( x , y , unitMove.block , fly , UnitCampType , start , follow , speed , over );

        if ( cost >= 0 )
        {
            unitData.Move -= cost;
        }

        IsMoved = true;

        return cost != GameDefine.INVALID_ID;
    }

    public void stopMove()
    {
        gameMovement.stopMove();
    }

    public bool moveToDirection( int x , int y , bool start , bool follow , int speed , OnEventOver over )
    {
        if ( GameUserData.instance.Stage > 15 &&
            UnitID == 8 &&
            GameUserData.instance.getUnitBase( 8 ).InTeam == 0 )
        {
            return false;
        }

        return gameMovement.moveToDirection( x , y , start , follow , speed , over );
    }

    public void setOriginalDirection()
    {
        gameMovement.setOriginalDirection();
    }

    public void updateEffects()
    {
        if ( !IsAlive )
        {
            return;
        }

        if ( isActed )
        {
            if ( checkEffect( GameSkillResutlEffect.Palsy ) )
                gameAnimation.setColor( new Color( 0.4f , 0.4f , 0.2f , 1.0f ) );
            else if ( checkEffect( GameSkillResutlEffect.Poison ) )
                gameAnimation.setColor( new Color( 0.2f , 0.4f , 0.2f , 1.0f ) );
            else
                gameAnimation.setColor( new Color( 0.5f , 0.5f , 0.5f , 1.0f ) );
        }
        else
        {
            if ( checkEffect( GameSkillResutlEffect.Palsy ) )
                gameAnimation.setColor( new Color( 1.0f , 1.0f , 0.0f , 1.0f ) );
            else if ( checkEffect( GameSkillResutlEffect.Poison ) )
                gameAnimation.setColor( new Color( 0.0f , 1.0f , 0.0f , 1.0f ) );
            else
                gameAnimation.setColor( new Color( 1.0f , 1.0f , 1.0f , 1.0f ) );
        }
    }

    public void clearActed()
    {
        if ( !IsAlive )
        {
            return;
        }

        IsMoved = false;
        IsActed = false;

        clearMove();

        gameAnimation.playAnimationBattle( GameAnimationType.Stand , gameAnimation.animationDirection , null );

        updateEffect();

        updateEffects();
    }

    public void setDirection( int x , int y )
    {
        GameAnimationDirection dir = GameDefine.getDirection( gameMovement.PosX , gameMovement.PosY ,
            x , y );

        gameMovement.setDirection( dir );
    }

    public void move()
    {
        gameMovement.stopAnimation();
        gameMovement.clearOriginalPosition();
    }

    public void burst()
    {
        action();

        moveBurst = 2;
    }

    public void action()
    {
        if ( IsActed )
        {
            return;
        }

        IsActed = true;

        moveBurst = 1;

        if ( IsUser )
        {
            GameBattleManager.instance.checkMapEvent( this , onCheckMapEventOver );
        }
        else
        {
            GameBattleManager.instance.checkMapEvent1( this , onCheckMapEventOver );
        }

        GameBattleInput.instance.lastUnitID = UnitID;
    }

    void onCheckMapEventOver()
    {
        if ( !IsKilled )
        {
            gameAnimation.playAnimationBattle( GameAnimationType.Stand , gameAnimation.animationDirection , null );
            gameAnimation.stopAnimation();
        }

        updateEffects();

        GameBattleTurn.instance.checkOver();
    }

    public bool canUseSkill( GameSkill m )
    {
        if ( checkEffect( GameSkillResutlEffect.Silence ) )
        {
            return false;
        }

        if ( m.MoveAttackType == GameSkillMoveAttackType.Stand 
            && IsMoved )
        {
            return false;
        }

        return ( m.MoveCost <= Move && m.MPCost <= MP );
    }

    public bool canUseSkillStand( GameSkill m )
    {
        if ( checkEffect( GameSkillResutlEffect.Silence ) )
        {
            return false;
        }

        if ( m.MoveAttackType != GameSkillMoveAttackType.Stand )
        {
            return false;
        }

        return ( m.MoveCost <= Move && m.MPCost <= MP );
    }

    public bool canUseSkillMove( GameSkill m , int cost )
    {
        if ( checkEffect( GameSkillResutlEffect.Silence ) )
        {
            return false;
        }

        if ( m.MoveAttackType == GameSkillMoveAttackType.Stand )
        {
            return false;
        }

        return ( m.MoveCost <= Move - cost && m.MPCost <= MP );
    }

    public bool canUseSkill()
    {
        if ( checkEffect( GameSkillResutlEffect.Silence ) )
        {
            return false;
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            if ( Skill[ i ] == GameDefine.INVALID_ID )
            {
                continue;
            }

            GameSkill m = GameSkillData.instance.getData( Skill[ i ] );

            if ( canUseSkill( m ) )
            {
                return true;
            }
        }

        return false;
    }


    public void addExpSkill( GameBattleUnit unit )
    {
        if ( !IsUser || 
            IsKilled || 
            isSummon ||
            unit == null )
        {
            return;
        }

        int l = LV - unit.LV;

        if ( l > 10 )
        {
            l = 10;
        }

        if ( l < -10 )
        {
            l = -10;
        }

        float exp = 50 * ( 1.0f - l * 0.1f ) * 1.0f;

        if ( exp < 1.0f )
            exp = 1.0f;

        //        exp = 500;

        tempExp += (short)exp;
    }

    public void addExpSkill( GameBattleUnit unit , int hp )
    {
        if ( !IsUser || 
            IsKilled ||
            isSummon ||
            unit == null )
        {
            return;
        }

        int l = LV - unit.LV;

        if ( l > 10 )
        {
            l = 10;
        }

        if ( l < -10 )
        {
            l = -10;
        }

        float f = ( (float)hp / unit.HPMax );

        float exp = 200 * ( 1.0f - l * 0.1f ) * f;

        if ( exp < 1.0f )
            exp = 1.0f;

//        exp = 500;

        tempExp += (short)exp;
    }


    public void addExpCure( GameBattleUnit unit , int hp , int mp )
    {
        if ( !IsUser ||
            IsKilled ||
            isSummon ||
            unit == null )
        {
            return;
        }

        int hp1 = 0;
        int mp1 = 0;

        if ( hp > ( unit.HPMax - unit.HP ) )
        {
            hp1 = ( unit.HPMax - unit.HP );
        }
        else
        {
            hp1 = hp;
        }

        if ( mp > ( unit.MPMax - unit.MP ) )
        {
            mp1 = ( unit.MPMax - unit.MP );
        }
        else
        {
            mp1 = mp;
        }

        int l = LV - unit.LV;
        
        if ( l > 10 )
        {
            l = 10;
        }

        if ( l < -10 )
        {
            l = -10;
        }

        float exp = 200.0f * ( 1.0f - l * 0.1f );

        if ( hp > 0 )
            exp *= ( (float)hp1 / unit.HPMax );

        if ( mp > 0 )
            exp *= ( (float)mp1 / unit.MPMax );

        if ( hp == 0 && mp == 0 )
            exp = 1;

        if ( exp < 1.0f )
            exp = 1.0f;

//        exp = 500;

        tempExp += (short)exp;
    }

    public void addExpDamage( GameBattleUnit unit , int hp , int mp )
    {
        if ( !IsUser ||
            IsKilled ||
            isSummon ||
            unit == null )
        {
            return;
        }

        int hp1 = 0;
        int mp1 = 0;

        float killed = 1.0f;

        if ( hp >= unit.HP )
        {
            hp1 = unit.HP;
        }
        else
        {
            hp1 = hp;
            killed = (float)hp1 / unit.HPMax;
        }

        if ( mp > unit.MP )
        {
            mp1 = unit.MP;
        }
        else
        {
            mp1 = mp;
        }

        int l = LV - unit.LV;

        if ( l > 10 )
        {
            l = 10;
        }

        if ( l < -10 )
        {
            l = -10;
        }

        float exp = unit.EXPBase * ( 1.0f - l * 0.1f );

        if ( hp > 0 )
            exp *= killed;

        if ( mp > 0 )
            exp *= ( (float)mp1 / unit.MPMax );

        if ( hp == 0 && mp == 0 )
            exp = 1;

        if ( exp < 1.0f )
            exp = 1.0f;

        //        exp = 500;

        tempExp += (short)exp;
    }

    public void addExp()
    {
        gameBattleMan.UnitBase.Exp += tempExp;
        tempExp = 0;
    }


    public int getAbsorbHP( int hp )
    {
        int hp1 = 0;

        if ( hp > HP )
        {
            hp1 = HP;
        }
        else
        {
            hp1 = hp;
        }

        return (int)( hp1 * 0.3f );
    }

}

