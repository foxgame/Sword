using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;




public enum GameBattleAttackResultSide
{
    Left = 0,
    Right,

    Count
}

public class GameBattleAttackResultPhysical
{
    public enum Type
    {
        Normal = 0,
        Critical,
        Double,
        Block,

        Count
    }

    public Type type;

    public int HP;
    public int HP2;

    public int MP;
    public int MP2;

    public GameAttackAddType AddType;
    public int AddTurn;
    public int AddValue;
}

public class GameBattleAttackResultSkill
{
    public GameBattleUnit unit;

    public GameSkillResutlType type;
    public GameSkillOtherEffect other;

    public GameSkillResutlEffect effect = GameSkillResutlEffect.None;

    public int HP;
    public int MP;
}

public class GameBattleAttackResultItem
{
    public GameBattleUnit unit;

    public GameItemUseType type;

    public GameItemEffectType effect;

    public int HP;
    public int MP;
}


public class GameBattleAttackResult : Singleton<GameBattleAttackResult>
{

    OnEventOver onEventOver;


    public void clear()
    {
    }

    GameBattleAttackResultSide side;

    GameBattleUnit unit;

    List<GameBattleAttackResultItem> resultItem = new List<GameBattleAttackResultItem>();

    List<GameBattleAttackResultSkill> resultSkill = new List<GameBattleAttackResultSkill>();
    List<GameBattleUnit> targets;

    GameBattleAttackResultPhysical resultPhysical;
    GameBattleUnit target;

    bool counter;
    bool addExp;

    GameSkill skill;

    GameItem item;

    GameAnimation maplupAnimation;

    public void itemAttack( GameItem m , GameBattleUnit u , List<GameBattleUnit> t , GameBattleAttackMapDirection dir , GameBattleAttackResultSide s , OnEventOver over )
    {
        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        onEventOver = over;

        skill = null;
        item = m;
        addExp = ( GameSetting.instance.mode == GameSetting.GameMode.Hard ? false : true );

        side = s;
        unit = u;
        targets = t;

        GameBattleUnit attacker = u;

        int power = m.UseBasePower;

        byte floor = GameBattleManager.instance.getPoint( GameBattleCursor.instance.PosX ,
GameBattleCursor.instance.PosY ).Floor;

        unit.useItemID( m.ID );

        unit.addMP( -m.UseMPCost );

        GameBattleSkillTitleUI.instance.show( m.Name );

        for ( int i = 0 ; i < t.Count ; i++ )
        {
            GameBattleUnit defencer = t[ i ];

            GameBattleAttackResultItem r = new GameBattleAttackResultItem();
            r.unit = defencer;
            r.type = m.UseType;
            r.effect = m.UseEffectType;

            switch ( m.UseType )
            {
                case GameItemUseType.Recovery:
                case GameItemUseType.Limit:
                    {
                        r.HP = power;
                    }
                    break;
                case GameItemUseType.Cure:
                    {
                        switch ( item.UseEffectType )
                        {
                            case GameItemEffectType.HP:
                                r.HP = power;
                                break;
                            case GameItemEffectType.MP:
                                r.MP = power;
                                break;
                        }
                    }
                    break;
                case GameItemUseType.None:
                case GameItemUseType.Skill:
                    {
                    }
                    break;
            }

            resultItem.Add( r );
        }

        if ( m.UseType == GameItemUseType.None ||
            m.UseType == GameItemUseType.Skill )
        {
            resultItem.Clear();

            skill = GameSkillData.instance.getData( item.UseSkillID );

            if ( skill == null )
            {
                return;
            }

            skillAttack( skill , unit , targets , dir , s , over );
            addExp = ( GameSetting.instance.mode == GameSetting.GameMode.Hard ? false : true );
        }
        else
        {
            GameBattleAttackMap.instance.showItem( m , unit , dir , resultItem , resultSkill , onShowOverItem );
        }
    }


    public void skillAttack( GameSkill sk , GameBattleUnit u , List<GameBattleUnit> t , GameBattleAttackMapDirection dir , GameBattleAttackResultSide s , OnEventOver over )
    {
        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        onEventOver = over;

        skill = sk;
        addExp = true;

        side = s;
        unit = u;
        targets = t;

        GameUserData.instance.LastAttackID = unit.BattleManID;

        GameBattleUnit attacker = u;

        int power = ( skill.Type == GameSkillType.Magic ? (int)( attacker.MagicAttack + skill.BasePower ) : attacker.PhysicalAttack );

        if ( skill.Type == GameSkillType.Magic )
        {
            power = (int)( power * attacker.spiritAddition( skill , GameSkillAdditionType.Self ) );
        }
        else
        {
//             power = (int)( power * ( attacker.spiritAddition( skill , GameSkillAdditionType.Self ) + 1.0f ) / 2.0f ) 
//                 + (int)( attacker.PhysicalAttack * skill.BasePower / 1000.0f );
        }

        if ( item != null )
            power += ( item.UseBasePower - skill.BasePower );
        else
        {
            unit.addMP( -skill.MPCost );
            unit.addMove( -skill.MoveCost );

            if ( skill.BattleType != GameSkillBattleType.UI &&
                skill.BattleType != GameSkillBattleType.UIMap )
                GameBattleSkillTitleUI.instance.show( skill.Name );
        }

        byte floor = GameBattleManager.instance.getPoint( GameBattleCursor.instance.PosX ,
GameBattleCursor.instance.PosY ).Floor;


        switch ( skill.BattleType )
        {
            case GameSkillBattleType.Normal:
            case GameSkillBattleType.Map:
                {
                    if ( skill.Hit > 0 )
                    {
                        for ( int i = 0 ; i < targets.Count ; )
                        {
                            if ( targets[ i ] == null )
                            {
                                i++;
                                continue;
                            }

                            if ( GameUserData.instance.Stage == 8 && 
                                unit.UnitID == 9 && targets[ i ].UnitID == 52 )
                            {
                                GameBattleJudgment.instance.Proficiency8 = false;
                            }

                            if ( UnityEngine.Random.Range( 0 , 100 ) < skill.Hit + unit.RandomHit - targets[ i ].RandomHit )
                            {
                                if ( skill.ResultType == GameSkillResutlType.Curse && targets[ i ].IsBoss )
                                    targets.RemoveAt( i );
                                else
                                    i++;
                            }
                            else
                            {
                                targets.RemoveAt( i );
                            }
                        }
                    }

                    if ( skill.OtherEffect == GameSkillOtherEffect.KillSelf )
                    {
                        targets.Add( unit );
                    }

                    for ( int i = 0 ; i < targets.Count ; i++ )
                    {
                        GameBattleUnit defencer = targets[ i ];

                        GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                        r.unit = defencer;
                        r.type = skill.ResultType;
                        r.other = skill.OtherEffect;

                        int power1 = 0;

                        if ( defencer != null )
                        {
                            power1 = (int)( power * defencer.spiritAddition( skill , GameSkillAdditionType.Target ) );

                            if ( defencer.IsSummon && unit.checkEffect( GameSkillResutlEffect.SummonKiller ) )
                            {
                                power1 *= 2;
                            }
                        }

                        switch ( skill.ResultType )
                        {
                            case GameSkillResutlType.None:
                            case GameSkillResutlType.Special:
                                {
                                    switch ( skill.OtherEffect )
                                    {
                                        case GameSkillOtherEffect.AttackX2:
                                            {
                                                r.HP = (int)( ( power1 + unit.MagicAttack ) * defencer.getAttributeDefence( skill.AttributeType ) );
                                            }
                                            break;
                                        case GameSkillOtherEffect.AttackX3:
                                        case GameSkillOtherEffect.AttackX4:
                                        case GameSkillOtherEffect.AttackPalsy:
                                        case GameSkillOtherEffect.AttackPoison:
                                        case GameSkillOtherEffect.AttackHP:
                                        case GameSkillOtherEffect.AttackFetter:
                                            {
                                                r.HP = (int)( power1 * defencer.getAttributeDefence( skill.AttributeType ) );
                                            }
                                            break;
                                        case GameSkillOtherEffect.MP:
                                            {
                                                r.MP = (int)( skill.BasePower * defencer.getAttributeDefence( skill.AttributeType ) );

                                                if ( defencer.checkEffect( GameSkillResutlEffect.MagImmunity ) )
                                                {
                                                    r.MP = 0;
                                                }
                                            }
                                            break;
                                        case GameSkillOtherEffect.HealAll:
                                            {
                                                r.HP = defencer.HPMax - defencer.HP;
                                            }
                                            break;
                                        case GameSkillOtherEffect.KillSelf:
                                            {
                                                r.HP = unit.HP;
                                            }
                                            break;
                                    }

                                    resultSkill.Add( r );
                                }
                                break;
                            case GameSkillResutlType.Damage:
                                {
                                    r.HP = (int)( power1 * defencer.getAttributeDefence( skill.AttributeType ) );

                                    if ( defencer.checkEffect( GameSkillResutlEffect.MagImmunity ) )
                                    {
                                        r.HP = 0;
                                    }

                                    resultSkill.Add( r );
                                }
                                break;
                            case GameSkillResutlType.Cure:
                                {
                                    r.HP = power1;
                                    resultSkill.Add( r );
                                }
                                break;
                            case GameSkillResutlType.Purge:
                                {
                                    resultSkill.Add( r );
                                }
                                break;
                            case GameSkillResutlType.Blessing:
                                {
                                    resultSkill.Add( r );
                                }
                                break;
                            case GameSkillResutlType.Curse:
                                {
                                    resultSkill.Add( r );
                                }
                                break;
                        }


                    }

                    GameBattleAttackMap.instance.showSkill( skill , unit , dir , resultSkill , onShowOverSkill );
                }
                break;
            case GameSkillBattleType.UI:
                {
                    target = targets[ 0 ];

                    GameUserData.instance.LastTargetID = target.BattleManID;

                    int power1 = (int)( power * target.spiritAddition( skill , GameSkillAdditionType.Target ) );

                    if ( target.IsSummon && unit.checkEffect( GameSkillResutlEffect.SummonKiller ) )
                    {
                        power1 *= 2;
                    }

                    resultPhysical = new GameBattleAttackResultPhysical();                   
                    resultPhysical.HP = power1 - target.Defence;
                    resultPhysical.HP += unit.RandomLucky;

                    switch ( skill.OtherEffect )
                    {
                        case GameSkillOtherEffect.AttackX2:
                            resultPhysical.HP *= 2;
                            break;
                        case GameSkillOtherEffect.AttackX3:
                            resultPhysical.HP *= 3;
                            break;
                        case GameSkillOtherEffect.AttackX4:
                            resultPhysical.HP *= 4;
                            break;
                    }

                    if ( skill.OtherType == GameSkillOtherType.Normal )
                    {
                        resultPhysical.HP += (int)( power1 * 0.1f );

                        if ( resultPhysical.HP < power1 * 0.1f )
                        {
                            resultPhysical.HP = (int)( power1 * 0.1f );
                        }
                    }

                    int rand = Random.Range( 1 , 100 );

                    int hit = skill.Hit <= 0 ? unit.Hit : skill.Hit;

                    if ( rand <= hit - target.Miss )
                    {
                        resultPhysical.type = GameBattleAttackResultPhysical.Type.Normal;
                    }
                    else
                    {
                        resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;

                        rand = Random.Range( 1 , 100 );

                        if ( rand < 30 )
                            resultPhysical.HP = (int)(resultPhysical.HP * 0.5f );
                        else
                            resultPhysical.HP = 0;
                    }

                    if ( target.checkEffect( GameSkillResutlEffect.Miss ) )
                    {
                        resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;
                        resultPhysical.HP = 0;
                        resultPhysical.HP2 = 0;
                    }

                    if ( resultPhysical.HP < 0 )
                    {
                        resultPhysical.HP = 0;
                    }

                    if ( target.checkEffect( GameSkillResutlEffect.PhyImmunity ) )
                    {
                        resultPhysical.HP = 0;
                    }

                    GameItem weapon = unit.Weapon;

                    if ( resultPhysical.type != GameBattleAttackResultPhysical.Type.Block &&
                        weapon.AttackAddType != GameAttackAddType.None )
                    {
                        if ( Random.Range( 1 , 100 ) < weapon.AttackAddRatio )
                        {
                            resultPhysical.AddType = weapon.AttackAddType;
                            resultPhysical.AddTurn = weapon.AttackAddTime / 100;
                        }

                        if ( weapon.AttackAddType == GameAttackAddType.AbsorbHP )
                        {
                            resultPhysical.AddValue = target.getAbsorbHP( resultPhysical.HP + resultPhysical.HP2 );
                        }
                    }

                    bool skHit = false;

                    if ( skill.HitAdd <= 0 )
                    {
                        skHit = Random.Range( 1 , 100 ) < 34 + unit.RandomHit - target.RandomHit;
                    }
                    else
                    {
                        skHit = Random.Range( 1 , 100 ) < skill.HitAdd + unit.RandomHit - target.RandomHit;
                    }

                    if ( target.IsBoss )
                    {
                        skHit = false;
                    }

                    if ( resultPhysical.type != GameBattleAttackResultPhysical.Type.Block && skHit )
                    {
                        switch ( skill.OtherEffect )
                        {
                            case GameSkillOtherEffect.AttackPalsy:
                                resultPhysical.AddType = GameAttackAddType.Palsy;
                                resultPhysical.AddTurn = 2;
                                break;
                            case GameSkillOtherEffect.AttackPoison:
                                resultPhysical.AddType = GameAttackAddType.Poison;
                                resultPhysical.AddTurn = 2;
                                break;
                            case GameSkillOtherEffect.AttackSilence:
                                resultPhysical.AddType = GameAttackAddType.Silence;
                                resultPhysical.AddTurn = 2;
                                break;
                            case GameSkillOtherEffect.AttackFetter:
                                resultPhysical.AddType = GameAttackAddType.Fetter;
                                resultPhysical.AddTurn = 2;
                                break;
                            case GameSkillOtherEffect.AttackHP:
                                resultPhysical.AddType = GameAttackAddType.AbsorbHP;
                                resultPhysical.AddValue = target.getAbsorbHP( resultPhysical.HP + resultPhysical.HP2 );
                                break;
                        }
                    }
                    

                    counter = false;

                    if ( s == GameBattleAttackResultSide.Left )
                    {
                        GameBattleAttackUI.instance.showSkill( floor , skill , unit , target , dir , s , resultPhysical , onShowOverPhysical );
                    }
                    else
                    {
                        GameBattleAttackUI.instance.showSkill( floor , skill , target , unit , dir , s , resultPhysical , onShowOverPhysical );
                    }
                }
                break;
//             case GameSkillBattleType.Single:
//                 {
//                     for ( int i = 0 ; i < t.Count ; i++ )
//                     {
//                         GameBattleUnit defencer = t[ i ];
// 
//                         int power1 = (int)( power * defencer.spiritAddition( skill , GameSkillAdditionType.Target ) );
// 
//                         if ( skill.AttributeType != GameAttributeType.None )
//                         {
//                             result = (int)( power1 * ( defencer.AttributeDefence[ i ] / 100.0f ) );
//                         }
//                         else
//                         {
//                             result = power1;
//                         }
// 
//                         GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
//                         r.unit = t[ i ];
//                         r.HP = result;
//                         r.type = GameSkillResutlType.Damage;
// 
//                         resultSkill.Add( r );
//                     }
// 
//                     GameBattleAttackMap.instance.showSkill( skill , unit , dir , resultSkill , onShowOverSkill );
//                 }
//                 break;
            case GameSkillBattleType.UIMap:
                {
                    for ( int i = 0 ; i < targets.Count ; i++ )
                    {
                        GameBattleUnit defencer = targets[ i ];

                        int power1 = (int)( power * defencer.spiritAddition( skill , GameSkillAdditionType.Target ) );

                        if ( defencer.IsSummon && unit.checkEffect( GameSkillResutlEffect.SummonKiller ) )
                        {
                            power1 *= 2;
                        }
                        
                        GameBattleAttackResultSkill r = new GameBattleAttackResultSkill();
                        r.unit = targets[ i ];
                        r.HP = power1 - defencer.Defence;
                        r.type = GameSkillResutlType.Damage;

                        switch ( skill.OtherEffect )
                        {
                            case GameSkillOtherEffect.AttackX2:
                                r.HP *= 2;
                                break;
                            case GameSkillOtherEffect.AttackX3:
                                r.HP *= 3;
                                break;
                            case GameSkillOtherEffect.AttackX4:
                                r.HP *= 4;
                                break;
                        }

                        if ( r.HP < power1 * 0.1f )
                        {
                            r.HP = (int)( power1 * 0.1f );
                        }

                        resultSkill.Add( r );
                    }

                    if ( s == GameBattleAttackResultSide.Left )
                    {
                        GameBattleAttackUI.instance.showSkillMap( floor , skill , unit , targets[ 0 ] , dir , s , resultSkill , onShowOverSkill );
                    }
                    else
                    {
                        GameBattleAttackUI.instance.showSkillMap( floor , skill , targets[ 0 ] , unit , dir , s , resultSkill , onShowOverSkill );
                    }
                }
                break;
        }


    }

    public void PhysicalAttack( GameBattleUnit u , GameBattleUnit e , GameBattleAttackResultSide s , OnEventOver over )
    {
        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        onEventOver = over;

        side = s;
        addExp = true;
        unit = u;
        target = e;

        if ( GameUserData.instance.Stage == 8 &&
            unit.UnitID == 9 && target.UnitID == 52 )
        {
            GameBattleJudgment.instance.Proficiency8 = false;
        }

        GameUserData.instance.LastAttackID = unit.BattleManID;
        GameUserData.instance.LastTargetID = target.BattleManID;

        byte floor = GameBattleManager.instance.getPoint( GameBattleCursor.instance.PosX ,
    GameBattleCursor.instance.PosY ).Floor;

        resultPhysical = new GameBattleAttackResultPhysical();
       
        int rand = Random.Range( 1 , 100 );

        int hp = unit.PhysicalAttack - target.Defence;
        hp += unit.RandomLucky;

        if ( target.IsSummon && unit.checkEffect( GameSkillResutlEffect.SummonKiller ) )
        {
            hp *= 2;
        }

        if ( rand <= unit.Critical )
        {
            resultPhysical.type = GameBattleAttackResultPhysical.Type.Critical;
            resultPhysical.HP = (int)( unit.PhysicalAttack * 1.5f - target.Defence );
        }
        else
        {
            rand = UnityEngine.Random.Range( 1 , 100 );

            if ( rand <= unit.Double )
            {
                resultPhysical.type = GameBattleAttackResultPhysical.Type.Double;
                resultPhysical.HP = hp;
                resultPhysical.HP2 = hp;
            }
            else
            {
                rand = UnityEngine.Random.Range( 1 , 100 );

                if ( rand <= unit.Hit - target.Miss )
                {
                    resultPhysical.type = GameBattleAttackResultPhysical.Type.Normal;
                    resultPhysical.HP = hp;
                }
                else
                {
                    resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;

                    if ( rand < 30 )
                        resultPhysical.HP = (int)( resultPhysical.HP * 0.5f );
                    else
                        resultPhysical.HP = 0;
                }
            }
        }

        if ( target.checkEffect( GameSkillResutlEffect.Miss ) )
        {
            resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;
            resultPhysical.HP = 0;
            resultPhysical.HP2 = 0;
        }

        if ( resultPhysical.HP < 0 )
        {
            resultPhysical.HP = 0;
        }
        if ( resultPhysical.HP2 < 0 )
        {
            resultPhysical.HP2 = 0;
        }

        if ( target.checkEffect( GameSkillResutlEffect.PhyImmunity ) )
        {
            resultPhysical.HP = 0;
        }

        GameItem weapon = unit.Weapon;

        if ( resultPhysical.type != GameBattleAttackResultPhysical.Type.Block &&
            weapon.AttackAddType != GameAttackAddType.None )
        {
            if ( Random.Range( 1 , 100 ) < weapon.AttackAddRatio )
            {
                resultPhysical.AddType = weapon.AttackAddType;
                resultPhysical.AddTurn = weapon.AttackAddTime / 100;
            }

            if ( weapon.AttackAddType == GameAttackAddType.AbsorbHP )
            {
                resultPhysical.AddValue = target.getAbsorbHP( resultPhysical.HP + resultPhysical.HP2 );
            }
        }



        counter = GameBattleUnitAttackSelection.instance.canAttack( target , unit );

        if ( target.checkEffect( GameSkillResutlEffect.Palsy ) )
        {
            counter = false;
        }

        if ( s == GameBattleAttackResultSide.Left )
        {
            GameBattleAttackUI.instance.show( floor , unit , target , s , resultPhysical , false ,
   onShowOverPhysical );
        }
        else
        {
            GameBattleAttackUI.instance.show( floor , target , unit , s , resultPhysical , false ,
   onShowOverPhysical );
        }

    }


    void onBattleEventOverItemStep0()
    {
        bool over = false;

        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];

            if ( u.IsKilled )
            {
                if ( over )
                {
                    u.fadeOut( null );
                }
                else
                {
                    u.fadeOut( onBattleEventOverItemStep1 );
                }

                over = true;
            }
        }

        if ( !over )
        {
            onBattleEventOverItemStep1();
        }
    }

    void onBattleEventOverItemStep1()
    {
//         GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY , 
//             GameBattleCursor.SpeedX3 , GameBattleCursor.SpeedY3 , null );
// 
//         GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell ,
//             unit.PosY - GameCameraManager.instance.yCell , 50 , 50 , onBattleEventOverItemStep2 );

        onBattleEventOverItemStep2();
    }

    void onBattleEventOverItemStep2()
    {
        onEventOver();

        resultItem.Clear();
        resultSkill.Clear();

        unit = null;
        target = null;
        targets = null;

        item = null;
        skill = null;
    }

    void onTempShowOver()
    {
        maplupAnimation.clearAnimation();
        Destroy( maplupAnimation.gameObject );
        maplupAnimation.transform.SetParent( null );
        maplupAnimation = null;
    }

    void onShowOverSkillStep0()
    {
        if ( unit.IsLevelUp )
        {
            string path = "Prefab/Misc/Maplup";

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            Transform trans = obj.transform;
            trans.SetParent( unit.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            maplupAnimation = obj.GetComponent<GameAnimation>();
            maplupAnimation.playAnimation( 2 , GameDefine.INVALID_ID , false , onTempShowOver );
            maplupAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
            maplupAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;

            unit.playAnimationBattle( GameAnimationType.Win , unit.Direction , onShowOverSkillStep1 );
        }
        else
        {
            onShowOverSkillStep1();
        }
    }

    void onShowOverSkillStep1()
    {
        if ( unit.TempExp > 0 )
        {
            GameMsgBoxUI.instance.showText( unit.Sprite , 0 , 1 ,
                GameMessageData.instance.getExp( unit.TempExp , unit.IsLevelUp ) , onShowOverSkillStep2 );

            unit.addExp();
        }
        else
        {
            onShowOverSkillStep2();
        }
    }

    void onShowOverSkillStep2()
    {
        if ( unit.IsLevelUp )
        {
            GameUnitLevelUpData.instance.levelUp( unit );

            GameLevelUpUI.instance.show( onShowOverSkillStep3 , unit ,
                GameUnitLevelUpData.instance.LastData );
        }
        else
        {
            onShowOverSkillStep3();
        }
    }

    void onShowOverSkillStep3()
    {
        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];

            if ( u == null )
            {
                continue;
            }

            if ( u.IsKilled )
            {
                if ( u.KillGetType != GameDefine.INVALID_ID && unit.IsUser )
                {
                    switch ( u.KillGetType )
                    {
                        case (int)GameBattleMapEventType.Item:
                            {
                                if ( unit.canAddItem() )
                                {
                                    unit.addItem( u.KillGetParm1 );
                                }
                                else
                                {
                                    GameUserData.instance.addItem( u.KillGetParm1 );
                                }

                                GameBattleGetItemUI.instance.show( u.KillGetParm1 , 0 , null );
                            }
                            break;
                        case (int)GameBattleMapEventType.Event:
                            {
                                GameUserData.instance.addGold( u.KillGetParm1 );

                                GameBattleGetItemUI.instance.show( GameDefine.INVALID_ID , u.KillGetParm1 , null );
                            }
                            break;
                    }

                }
            }
        }

        onShowOverSkillStep4();
    }

    void onShowOverSkillStep4()
    {
        bool over = false;

        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];

            if ( u == null )
            {
                continue;
            }

            if ( u.IsKilled )
            {
                if ( over )
                {
                    u.fadeOut( null );
                }
                else
                {
                    u.fadeOut( onShowOverSkillStep5 );
                }

                over = true;
            }
        }

        if ( !over )
        {
            onShowOverSkillStep5();
        }
    }

    void onShowOverSkillStep5()
    {
        bool b = false;

        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];

            if ( u == null )
            {
                continue;
            }

            if ( u.IsKilled )
            {
                b |= GameBattleJudgment.instance.check( u , onShowOverSkillStep6 );
            }
        }

        if ( !b )
        {
            onShowOverSkillStep6();
        }
    }

    void onShowOverSkillStep6()
    {
//         GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY ,
//             GameBattleCursor.SpeedX , GameBattleCursor.SpeedY , null );
// 
//         GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell ,
//             unit.PosY - GameCameraManager.instance.yCell , 50 , 50 , onShowOverSkillStep7 );

        onShowOverSkillStep7();
    }

    void onShowOverSkillStep7()
    {
        onEventOver();

        resultSkill.Clear();

        unit = null;
        target = null;
        targets = null;

        item = null;
        skill = null;
    }

    void onShowOverPhysicalStep0()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.IsLevelUp )
            {
                GameBattleAttackUI.instance.showLevelUp( onShowOverPhysicalStep1 );
            }
            else
            {
                onShowOverPhysicalStep1();
            }
        }
        else
        {
            if ( unit.IsLevelUp )
            {
                GameBattleAttackUI.instance.showLevelUp( onShowOverPhysicalStep1 );
            }
            else
            {
                onShowOverPhysicalStep1();
            }
        }
    }

    void onShowOverPhysicalStep1()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.TempExp > 0 )
            {
                GameMsgBoxUI.instance.showText( target.Sprite , 0 , 1 ,
                    GameMessageData.instance.getExp( target.TempExp , target.IsLevelUp ) , onShowOverPhysicalStep2 );

                target.addExp();
            }
            else
            {
                onShowOverPhysicalStep2();
            }
        }
        else
        {
            if ( unit.TempExp > 0 )
            {
                GameMsgBoxUI.instance.showText( unit.Sprite , 0 , 1 ,
                    GameMessageData.instance.getExp( unit.TempExp , unit.IsLevelUp ) , onShowOverPhysicalStep2 );

                unit.addExp();
            }
            else
            {
                onShowOverPhysicalStep2();
            }
        }
    }

    void onShowOverPhysicalStep2()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.IsLevelUp )
            {
                GameUnitLevelUpData.instance.levelUp( target );

                GameLevelUpUI.instance.show( onShowOverPhysicalStep3 , target ,
                    GameUnitLevelUpData.instance.LastData );
            }
            else
            {
                onShowOverPhysicalStep3();
            }
        }
        else
        {
            if ( unit.IsLevelUp )
            {
                GameUnitLevelUpData.instance.levelUp( unit );

                GameLevelUpUI.instance.show( onShowOverPhysicalStep3 , unit ,
                    GameUnitLevelUpData.instance.LastData );
            }
            else
            {
                onShowOverPhysicalStep3();
            }
        }

    }


    void onShowOverPhysicalStep3()
    {
        GameBattleAttackUI.instance.unShow( onShowOverPhysicalStep4 );
    }

    void onShowOverPhysicalStep4()
    {
        if ( target.IsKilled )
        {
            if ( target.KillGetType != GameDefine.INVALID_ID && unit.IsUser )
            {
                switch ( target.KillGetType )
                {
                    case (int)GameBattleMapEventType.Item:
                        {
                            if ( unit.canAddItem() )
                            {
                                unit.addItem( target.KillGetParm1 );
                            }
                            else
                            {
                                GameUserData.instance.addItem( target.KillGetParm1 );
                            }

                            GameBattleGetItemUI.instance.show( target.KillGetParm1 , 0 , null );
                        }
                        break;
                    case (int)GameBattleMapEventType.Event:
                        {
                            GameUserData.instance.addGold( target.KillGetParm1 );

                            GameBattleGetItemUI.instance.show( GameDefine.INVALID_ID , target.KillGetParm1 , null );
                        }
                        break;
                }

            }
        }

        onShowOverPhysicalStep5();
    }

    void onShowOverPhysicalStep5()
    {
        if ( target.IsKilled )
        {
            target.fadeOut( onShowOverPhysicalStep6 );
        }
        else
        {
            onShowOverPhysicalStep6();
        }

    }

    void onShowOverPhysicalStep6()
    {
        bool b = false;

        if ( target.IsKilled )
        {
            b = GameBattleJudgment.instance.check( target , onEventOver );
        }

        if ( !b )
        {
            onEventOver();
        }

        resultPhysical = null;

        unit = null;
        target = null;
        targets = null;

        skill = null;
    }



    void onShowOverPhysicalCounterStep0()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.IsLevelUp )
            {
                GameBattleAttackUI.instance.showLevelUp( onShowOverPhysicalCounterStep1 );
            }
            else
            {
                onShowOverPhysicalCounterStep1();
            }
        }
        else
        {
            if ( unit.IsLevelUp )
            {
                GameBattleAttackUI.instance.showLevelUp( onShowOverPhysicalCounterStep1 );
            }
            else
            {
                onShowOverPhysicalCounterStep1();
            }
        }
    }

    void onShowOverPhysicalCounterStep1()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.TempExp > 0 )
            {
                GameMsgBoxUI.instance.showText( target.Sprite , 0 , 1 ,
                    GameMessageData.instance.getExp( target.TempExp , target.IsLevelUp ) , onShowOverPhysicalCounterStep2 );

                target.addExp();
            }
            else
            {
                onShowOverPhysicalCounterStep2();
            }
        }
        else
        {
            if ( unit.TempExp > 0 )
            {
                GameMsgBoxUI.instance.showText( unit.Sprite , 0 , 1 ,
                    GameMessageData.instance.getExp( unit.TempExp , unit.IsLevelUp ) , onShowOverPhysicalCounterStep2 );

                unit.addExp();
            }
            else
            {
                onShowOverPhysicalCounterStep2();
            }
        }
    }

    void onShowOverPhysicalCounterStep2()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( target.IsLevelUp )
            {
                GameUnitLevelUpData.instance.levelUp( target );

                GameLevelUpUI.instance.show( onShowOverPhysicalCounterStep3 , target ,
                    GameUnitLevelUpData.instance.LastData );
            }
            else
            {
                onShowOverPhysicalCounterStep3();
            }
        }
        else
        {
            if ( unit.IsLevelUp )
            {
                GameUnitLevelUpData.instance.levelUp( unit );

                GameLevelUpUI.instance.show( onShowOverPhysicalCounterStep3 , unit ,
                    GameUnitLevelUpData.instance.LastData );
            }
            else
            {
                onShowOverPhysicalCounterStep3();
            }
        }
    }

    void onShowOverPhysicalCounterStep3()
    {
        GameBattleAttackUI.instance.unShow( onShowOverPhysicalCounterStep4 );
    }

    void onShowOverPhysicalCounterStep4()
    {
        if ( unit.IsKilled )
        {
            if ( unit.KillGetType != GameDefine.INVALID_ID && target.IsUser )
            {
                switch ( unit.KillGetType )
                {
                    case (int)GameBattleMapEventType.Item:
                        {
                            if ( target.canAddItem() )
                            {
                                target.addItem( unit.KillGetParm1 );
                            }
                            else
                            {
                                GameUserData.instance.addItem( unit.KillGetParm1 );
                            }

                            GameBattleGetItemUI.instance.show( unit.KillGetParm1 , 0 , null );
                        }
                        break;
                    case (int)GameBattleMapEventType.Event:
                        {
                            GameUserData.instance.addGold( unit.KillGetParm1 );

                            GameBattleGetItemUI.instance.show( GameDefine.INVALID_ID , unit.KillGetParm1 , null );
                        }
                        break;
                }

            }
        }

        onShowOverPhysicalCounterStep5();
    }

    void onShowOverPhysicalCounterStep5()
    {
        if ( unit.IsKilled )
        {
            unit.fadeOut( onShowOverPhysicalCounterStep6 );
        }
        else
        {
            onShowOverPhysicalCounterStep6();
        }
    }

    void onShowOverPhysicalCounterStep6()
    {
        bool b = false;

        if ( unit.IsKilled )
        {
            b = GameBattleJudgment.instance.check( unit , onEventOver );
        }

        if ( !b )
        {
            onEventOver();
        }

        resultPhysical = null;

        unit = null;
        target = null;
        targets = null;

        skill = null;
    }


    void onShowOverPhysicalCounter()
    {
        target.addExpDamage( unit , resultPhysical.HP + resultPhysical.HP2 , 0 );

        unit.attackEffect( resultPhysical.AddType , resultPhysical.AddTurn );

        unit.addHP( -resultPhysical.HP );
        unit.addHP( -resultPhysical.HP2 );
        unit.IsHurt = true;

        if ( resultPhysical.AddType == GameAttackAddType.AbsorbHP &&
           resultPhysical.AddValue > 0 )
        {
            target.addHP( resultPhysical.AddValue );
        }

        onShowOverPhysicalCounterStep0();
    }

    void onShowOverPhysical()
    {
        unit.addExpDamage( target , resultPhysical.HP + resultPhysical.HP2 , 0 );

        target.attackEffect( resultPhysical.AddType , resultPhysical.AddTurn );

        target.addHP( -resultPhysical.HP );
        target.addHP( -resultPhysical.HP2 );
        target.IsHurt = true;

        if ( resultPhysical.AddType == GameAttackAddType.AbsorbHP && 
            resultPhysical.AddValue > 0 )
        {
            unit.addHP( resultPhysical.AddValue );
        }

        if ( target.IsKilled )
        {
            onShowOverPhysicalStep0();
            return;
        }

        if ( target.checkEffect( GameSkillResutlEffect.Palsy ) )
        {
            counter = false;
        }

        // counter
        if ( !counter )
        {
            onShowOverPhysicalStep0();
            return;
        }


        byte floor = GameBattleManager.instance.getPoint( GameBattleCursor.instance.PosX ,
    GameBattleCursor.instance.PosY ).Floor;

        resultPhysical = new GameBattleAttackResultPhysical();

        int rand = UnityEngine.Random.Range( 1 , 100 );

        int hp = target.PhysicalAttack - unit.Defence;
        hp += target.RandomLucky;

        if ( unit.IsSummon && target.checkEffect( GameSkillResutlEffect.SummonKiller ) )
        {
            hp *= 2;
        }

        if ( rand <= target.Double )
        {
            resultPhysical.type = GameBattleAttackResultPhysical.Type.Double;
            resultPhysical.HP = hp;
            resultPhysical.HP2 = hp;
        }
        else
        {
            rand = UnityEngine.Random.Range( 1 , 100 );

            if ( rand <= target.Critical )
            {
                resultPhysical.type = GameBattleAttackResultPhysical.Type.Critical;
                resultPhysical.HP = (int)( hp * 1.5f );
            }
            else
            {
                rand = UnityEngine.Random.Range( 1 , 100 );

                if ( rand <= target.Hit - unit.Miss )
                {
                    resultPhysical.type = GameBattleAttackResultPhysical.Type.Normal;
                    resultPhysical.HP = hp;
                }
                else
                {
                    resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;

                    if ( rand < 30 )
                        resultPhysical.HP = (int)( resultPhysical.HP * 0.5f );
                    else
                        resultPhysical.HP = 0;
                }
            }
        }

        if ( unit.checkEffect( GameSkillResutlEffect.Miss ) )
        {
            resultPhysical.type = GameBattleAttackResultPhysical.Type.Block;
            resultPhysical.HP = 0;
            resultPhysical.HP2 = 0;
        }

        if ( resultPhysical.HP < 0 )
        {
            resultPhysical.HP = 0;
        }
        if ( resultPhysical.HP2 < 0 )
        {
            resultPhysical.HP2 = 0;
        }

        if ( unit.checkEffect( GameSkillResutlEffect.PhyImmunity ) )
        {
            resultPhysical.HP = 0;
        }


        GameItem weapon = target.Weapon;

        if ( resultPhysical.type != GameBattleAttackResultPhysical.Type.Block &&
            weapon.AttackAddType != GameAttackAddType.None )
        {
            if ( Random.Range( 1 , 100 ) < weapon.AttackAddRatio )
            {
                resultPhysical.AddType = weapon.AttackAddType;
                resultPhysical.AddTurn = weapon.AttackAddTime / 100;
            }

            if ( weapon.AttackAddType == GameAttackAddType.AbsorbHP )
            {
                resultPhysical.AddValue = unit.getAbsorbHP( resultPhysical.HP + resultPhysical.HP2 );
            }
        }


        if ( side == GameBattleAttackResultSide.Left )
        {
            GameBattleAttackUI.instance.show( floor , unit , target , GameBattleAttackResultSide.Right , resultPhysical , true ,
   onShowOverPhysicalCounter );
        }
        else
        {
            GameBattleAttackUI.instance.show( floor , target , unit , GameBattleAttackResultSide.Left , resultPhysical , true ,
   onShowOverPhysicalCounter );
        }
    }

    void onShowOverSkill()
    {
        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];
            GameBattleAttackResultSkill r = resultSkill[ i ];

            if ( u != null )
            {
                u.skillEffect( unit , skill );
            }

            switch ( r.type )
            {
                case GameSkillResutlType.Damage:
                    {
                        if ( addExp )
                        {
                            unit.addExpDamage( u , r.HP , 0 );
                        }

                        if ( u != null )
                        {
                            u.addHP( -r.HP );
                            u.IsHurt = true;
                        }
                    }
                    break;
                case GameSkillResutlType.Cure:
                    {
                        if ( addExp )
                        {
                            unit.addExpCure( u , r.HP , 0 );
                        }

                        if ( u != null )
                        {
                            u.addHP( r.HP );
                        }
                    }
                    break;
                case GameSkillResutlType.Purge:
                case GameSkillResutlType.Blessing:
                case GameSkillResutlType.Curse:
                    {
                        if ( addExp )
                        {
                            unit.addExpSkill( u );
                        }
                    }
                    break;
                case GameSkillResutlType.None:
                case GameSkillResutlType.Special:
                    {
                        switch ( skill.OtherEffect )
                        {
                            case GameSkillOtherEffect.AttackX2:
                            case GameSkillOtherEffect.AttackX3:
                            case GameSkillOtherEffect.AttackX4:
                            case GameSkillOtherEffect.AttackPalsy:
                            case GameSkillOtherEffect.AttackPoison:
                            case GameSkillOtherEffect.AttackHP:
                            case GameSkillOtherEffect.AttackFetter:
                                {
                                    if ( addExp )
                                    {
                                        unit.addExpDamage( u , r.HP , 0 );
                                    }

                                    if ( u != null )
                                    {
                                        u.addHP( -r.HP );
                                        u.IsHurt = true;
                                    }
                                }
                                break;
                            case GameSkillOtherEffect.MP:
                                {
                                    if ( u != null )
                                    {
                                        int mp = u.MP > r.MP ? r.MP : u.MP;

                                        if ( addExp )
                                            unit.addExpDamage( u , 0 , mp );

                                        unit.addMP( mp );
                                        u.addMP( -mp );
                                    }
                                }
                                break;
                            case GameSkillOtherEffect.HealAll:
                                {
                                    if ( addExp )
                                    {
                                        unit.addExpCure( u , r.HP , 0 );
                                    }

                                    if ( u != null )
                                    {
                                        u.addHP( r.HP );
                                    }
                                }
                                break;
                            case GameSkillOtherEffect.MPAdd:
                                {
                                    if ( addExp )
                                    {
                                        unit.addExpDamage( u , 0 , skill.BasePower );
                                    }

                                    if ( u != null )
                                    {
                                        u.addMP( skill.BasePower );
                                    }
                                }
                                break;
                            case GameSkillOtherEffect.KillSelf:
                                {
                                    if ( u != null )
                                    {
                                        u.addHP( -r.HP );
                                        u.IsHurt = true;
                                    }
                                }
                                break;
                            default:
                                if ( addExp )
                                {
                                    unit.addExpSkill( u );
                                }
                                break;
                        }

                        switch ( skill.ResultEffect )
                        {
                            case GameSkillResutlEffect.Summon:
                                {
                                    GameBattleUnitManager.instance.summonUnit( unit , skill.BasePower , 
                                        GameBattleCursor.instance.PosX , GameBattleCursor.instance.PosY );
                                }
                                break;
                            case GameSkillResutlEffect.SummonRelive:
                                {
                                    GameBattleUnitManager.instance.reliveSummonUnit( GameBattleCursor.instance.PosX ,
                                        GameBattleCursor.instance.PosY );
                                }
                                break;
                            default:
                                {
                                }
                                break;
                        }


                        

                    }
                    break;
            }
        }

        onShowOverSkillStep0();      
    }


    void onShowOverItem()
    {
        bool b = false;

        for ( int i = 0 ; i < targets.Count ; i++ )
        {
            GameBattleUnit u = targets[ i ];

            switch ( item.UseType )
            {
                case GameItemUseType.None:
                case GameItemUseType.Recovery:
                case GameItemUseType.Limit:
                    {
                        u.useItemEffect( item );
                    }
                    break;
                case GameItemUseType.Cure:
                    {
                        GameBattleAttackResultItem r = resultItem[ i ];
                        u.addHP( r.HP );
                        u.addMP( r.MP );
                    }
                    break;
                case GameItemUseType.Skill:
                    {
                        GameBattleAttackResultSkill r = resultSkill[ i ];
                        u.addHP( -r.HP );
                        u.IsHurt = true;

                        if ( u.IsKilled )
                        {
                            b |= GameBattleJudgment.instance.check( u , onBattleEventOverItemStep0 );
                        }
                    }
                    break;
            }

        }

        if ( !b )
        {
            onBattleEventOverItemStep0();
        }
    }


}


