using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBattleUnitEffect
{
    public GameSkillResutlEffect Effect = GameSkillResutlEffect.None;
    public GameSkillOtherEffect OtherEffect = GameSkillOtherEffect.None;

    public int Turn;

    public short Str = 100;
    public short Int = 100;
    public short Vit = 100;
    public short Mov = 100;
    public short Luk = 100;

    public GameObject gameObject;
}

public partial class GameBattleUnit : MonoBehaviour
{
    OnEventOver onEventOver;

    [SerializeField]
    List<GameBattleUnitEffect> effects = new List<GameBattleUnitEffect>();

    GameObject effectsObject = null;

    bool startFadeOut = false;

    float time;
    float alpha;

    short moveBurst = 1;

    public short MoveBurst { get { return moveBurst; } }

    public void setMoveBurst( short mb )
    {
        moveBurst = mb;
    }

    public List<GameBattleUnitEffect> Effects { get { return effects; } }

    public void useItemEffect( GameItem item )
    {
        switch ( item.UseEffectType )
        {
            case GameItemEffectType.None:
                break;
            case GameItemEffectType.HP:
                break;
            case GameItemEffectType.MP:
                break;
            case GameItemEffectType.Poison:
                removeDeBuff( GameSkillResutlEffect.Poison );
                break;
            case GameItemEffectType.Palsy:
                removeDeBuff( GameSkillResutlEffect.Palsy );
                break;
            case GameItemEffectType.Silence:
                removeDeBuff( GameSkillResutlEffect.Silence );
                break;
            case GameItemEffectType.HPUp:
                gameBattleMan.UnitBase.HP += item.UseBasePower;
                break;
            case GameItemEffectType.MPUp:
                gameBattleMan.UnitBase.MP += item.UseBasePower;
                break;
            case GameItemEffectType.StrUp:
                gameBattleMan.UnitBase.Str += item.UseBasePower;
                break;
            case GameItemEffectType.VitUp:
                gameBattleMan.UnitBase.Vit += item.UseBasePower;
                break;
            case GameItemEffectType.IntUp:
                gameBattleMan.UnitBase.Int += item.UseBasePower;
                break;
            case GameItemEffectType.AvgUp:
                gameBattleMan.UnitBase.Avg += item.UseBasePower;
                break;
            case GameItemEffectType.LukUp:
                gameBattleMan.UnitBase.Luk += item.UseBasePower;
                break;
        }

        updateUnitData();
    }

    public void addEffect( GameBattleUnitEffect e )
    {
        for ( int i = 0 ; i < effects.Count ; i++ )
        {
            if ( effects[ i ].Effect == e.Effect )
            {
//                 e.gameObject = effects[ i ].gameObject;
//                 effects[ i ] = e;
                return;
            }
        }

        effects.Add( e );

        if ( effectsObject == null )
        {
            effectsObject = new GameObject();
            effectsObject.name = "effects";
            effectsObject.transform.SetParent( gameObject.transform );
            effectsObject.transform.localPosition = Vector3.zero;
        }

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Misc/Man_eff" ) );
        GameAnimation ani = obj.GetComponent<GameAnimation>();
        ani.offsetX = GameDefine.BATTLE_OFFSET_X;
        ani.offsetY = GameDefine.BATTLE_OFFSET_Y;

        Transform trans = obj.transform;
        trans.SetParent( effectsObject.transform );
        trans.localPosition = Vector3.zero;

        e.gameObject = obj;

        switch ( e.OtherEffect )
        {
            case GameSkillOtherEffect.AbilityUp:
                ani.playAnimation( 23 , 45 );
                break;
        }

        switch ( e.Effect )
        {
            case GameSkillResutlEffect.StrUp:
            case GameSkillResutlEffect.VitUp:
            case GameSkillResutlEffect.IntUp:
            case GameSkillResutlEffect.Violent:
                ani.playAnimation( 23 , 45 );
                break;
            case GameSkillResutlEffect.MoveUp:
                ani.playAnimation( 1 , 23 );
                trans.localPosition = new Vector3( 0.0f , 0.0f , 1000.0f );
                break;
            case GameSkillResutlEffect.PhyImmunity:
                ani.playAnimation( 129 , 154 );
                break;
            case GameSkillResutlEffect.MagImmunity:
                ani.playAnimation( 109 , 129 );
                break;
            case GameSkillResutlEffect.Miss:
                ani.playAnimation( 87 , 109 );
                break;
            case GameSkillResutlEffect.Poison:
//                ani.playAnimation( 154 , 187 );
                break;
            case GameSkillResutlEffect.Palsy:
//                ani.playAnimation( 154 , 187 );
                break;
            case GameSkillResutlEffect.Silence:
                ani.playAnimation( 45 , 87 );
                break;
            case GameSkillResutlEffect.Fetter:
                ani.playAnimation( 154 , 187 );
                break;
            case GameSkillResutlEffect.Clear:
                break;
            case GameSkillResutlEffect.SummonKiller:
                break;
            case GameSkillResutlEffect.Summon:
                break;
            case GameSkillResutlEffect.SummonRelive:
                break;
            case GameSkillResutlEffect.Under:
                ani.playAnimation( 1 , 23 );
                trans.localPosition = new Vector3( 0.0f , 0.0f , 1000.0f );
                break;
        }

        updateEffects();
    }

    public bool checkEffect( GameSkillResutlEffect e )
    {
        for ( int i = 0 ; i < effects.Count ; i++ )
        {
            if ( effects[ i ].Effect == e )
            {
                return true;
            }
        }

        return false;
    }

    public bool checkEffectOther( GameSkillOtherEffect e )
    {
        for ( int i = 0 ; i < effects.Count ; i++ )
        {
            if ( effects[ i ].OtherEffect == e )
            {
                return true;
            }
        }

        return false;
    }

    public void clearEffects()
    {
        for ( int i = 0 ; i < effects.Count ; )
        {
            destroyEffect( i );
        }
    }

    void destroyEffect( int i )
    {
        effects[ i ].gameObject.SetActive( false );
        Destroy( effects[ i ].gameObject );
        effects[ i ].gameObject.transform.SetParent( null );
        effects.RemoveAt( i );
    }

    public void removeBuff( GameSkillResutlEffect e = GameSkillResutlEffect.None )
    {
        if ( e == GameSkillResutlEffect.None )
        {
            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect == GameSkillResutlEffect.StrUp ||
                    effects[ i ].Effect == GameSkillResutlEffect.VitUp ||
                    effects[ i ].Effect == GameSkillResutlEffect.IntUp ||
                    effects[ i ].Effect == GameSkillResutlEffect.MoveUp )
                {
                    destroyEffect( i );
                }
                else
                {
                    i++;
                }
            }
        }
        else
        {
            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect == e )
                {
                    destroyEffect( i );
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public void removeDeBuff( GameSkillResutlEffect e = GameSkillResutlEffect.None )
    {
        if ( e == GameSkillResutlEffect.None )
        {
            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect == GameSkillResutlEffect.Poison ||
                    effects[ i ].Effect == GameSkillResutlEffect.Palsy ||
                    effects[ i ].Effect == GameSkillResutlEffect.Silence ||
                    effects[ i ].Effect == GameSkillResutlEffect.Fetter )
                {
                    destroyEffect( i );
                }
                else
                {
                    i++;
                }
            }
        }
        else
        {
            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect == e )
                {
                    destroyEffect( i );
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public int PhysicalAttack
    {
        get
        {
            float str = 1.0f;
            bool b = false;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect != GameSkillResutlEffect.Violent )
                    str *= effects[ i ].Str / 100.0f;
                else
                    b = true;
            }

            int pow = (int)( unitData.Str * str + unitData.Attack );

            if ( b )
                pow *= 2;

            return pow;
        }
    }

    public int Defence
    {
        get
        {
            float vit = 1.0f;
            bool b = false;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                if ( effects[ i ].Effect != GameSkillResutlEffect.Violent )
                    vit *= effects[ i ].Vit / 100.0f;
                else
                    b = true;
            }

            int pow = (int)( unitData.Vit * vit / 2.0f + unitData.Defence );

            if ( b )
                pow /= 2;

            return pow;
        }
    }

    public int MagicAttack
    {
        get
        {
            float Int = 1.0f;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                Int *= effects[ i ].Int / 100.0f;
            }

            return (int)( unitData.Int * Int / 2.0f );
        }
    }

    public int Speed
    {
        get
        {
            float spd = 1.0f;

            return (int)( unitData.Avg * spd );
        }
    }

    public int Lucky
    {
        get
        {
            float luk = 1.0f;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                luk *= effects[ i ].Luk / 100.0f;
            }

            return (int)( unitData.Luk * luk );
        }
    }

    public int RandomLucky
    {
        get
        {
            return Random.Range( 0 , (int)( unitData.Luk / 10.0f ) );
        }
    }

    public int RandomHit
    {
        get
        {
            return Random.Range( 0 , (int)( unitData.Luk / 15.0f ) );
        }
    }

    public int SkillMiss
    {
        get
        {
            return unitData.Miss;
        }
    }

    public int Hit
    {
        get
        {
            return unitData.Hit + (int)( unitData.Luk / 25.0f );
        }
    }
    public int Miss
    {
        get
        {
            return unitData.Miss + (int)( unitData.Luk / 25.0f );
        }
    }

    public int Critical
    {
        get
        {
            return unitData.Critical;
        }
    }
    public int Double
    {
        get
        {
            return unitData.Double;
        }
    }

    public int HP { get { return unitData.HP; } }
    public int MP { get { return unitData.MP; } }

    public int HPMax { get { return unitData.HPMax; } }
    public int MPMax { get { return unitData.MPMax; } }

    public int Move
    {
        get
        {
            if ( checkEffect( GameSkillResutlEffect.Fetter ) )
            {
                return 0;
            }

            if ( gameUnit.MoveType == GameUnitMoveType.None )
            {
                return 0;
            }

            float mov = 1.0f;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                mov *= effects[ i ].Mov / 100.0f;
            }

            return (int)( unitData.Move * mov * moveBurst );
        }
    }

    public GameUnitMove UnitMove
    {
        get
        {
            return GameUnitMoveTypeData.instance.getData( gameUnit.MoveType );
        }
    }

    public int MoveMax
    {
        get
        {
            if ( checkEffect( GameSkillResutlEffect.Fetter ) )
            {
                return 0;
            }

            if ( gameUnit.MoveType == GameUnitMoveType.None )
            {
                return 0;
            }

            float mov = 1.0f;

            for ( int i = 0 ; i < effects.Count ; i++ )
            {
                mov *= effects[ i ].Mov / 100.0f;
            }

            return (int)( unitData.MoveMax * mov * moveBurst );
        }
    }

    public GameUnitMoveType MoveType
    {
        get
        {
            if ( GameBattlePathFinder.instance.getMapData( PosX , PosY ) == GameBattlePathFinder.BLOCK_EVENT )
            {
                return GameUnitMoveType.Fly;
            }

            return gameUnit.MoveType;
        }
    }

    public short[] AttributeDefence { get { return unitData.AttributeDefence; } }
    public float getAttributeDefence( GameAttributeType type )
    {
        if ( type != GameAttributeType.None )
        {
            return AttributeDefence[ (int)type ] / 100.0f;
        }

        return 1.0f;
    }

    public short[] SpiritPower { get { return gameBattleMan.UnitBase.SpiritPower; } }

    public void addMove( int mv )
    {
        unitData.Move += mv;

        if ( unitData.Move > unitData.MoveMax )
        {
            unitData.Move = unitData.MoveMax;
        }

        if ( unitData.Move < 0 )
        {
            unitData.Move = 0;
        }
    }

    public void addHP( int hp )
    {
        unitData.HP += hp;

        if ( hp < 0 )
        {
            if ( BattleAIType == GameBattleAIType.AINegative ||
                BattleAIType == GameBattleAIType.AIStay )
            {
                if ( GameUserData.instance.Stage != 19 )
                {
                    BattleAIType = GameBattleAIType.AIPositive;
                }
            }
        }

        if ( unitData.HP > unitData.HPMax )
        {
            unitData.HP = unitData.HPMax;
        }

        if ( unitData.HP <= 0 )
        {
            unitData.HP = 0;

            kill();
        }
    }

    public void addMP( int mp )
    {
        unitData.MP += mp;

        if ( unitData.MP > unitData.MPMax )
        {
            unitData.MP = unitData.MPMax;
        }

        if ( unitData.MP <= 0 )
        {
            unitData.MP = 0;
        }
    }

    public float spiritAddition( GameSkill m , GameSkillAdditionType type )
    {
        float power = 1.0f;

        for ( int i = 0 ; i < (int)GameSpiritType.Count ; i++ )
        {
            if ( m.addition[ i ].Type == type )
            {
                if ( SpiritPower[ i ] < m.addition[ i ].Request )
                {
                    power *= ( 100.0f + SpiritPower[ i ] - m.addition[ i ].Request ) / 100.0f;
                }

                if ( SpiritPower[ i ] > m.addition[ i ].Plus )
                {
                    power *= ( 100.0f + SpiritPower[ i ] - m.addition[ i ].Plus ) / 100.0f;
                }
            }
        }

        if ( power < 0.1f )
        {
            power = 0.1f;
        }

        return power;
    }


    public void attackEffect( GameAttackAddType addType , int turn )
    {
        switch ( addType )
        {
            case GameAttackAddType.None:
            case GameAttackAddType.AbsorbHP:
                break;
            case GameAttackAddType.Poison:
                {
                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = GameSkillResutlEffect.Poison;
                    e.Turn = turn;

                    addEffect( e );
                }
                break;
            case GameAttackAddType.Palsy:
                {
                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = GameSkillResutlEffect.Palsy;
                    e.Turn = turn;

                    addEffect( e );
                }
                break;
            case GameAttackAddType.Silence:
                {
                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = GameSkillResutlEffect.Silence;
                    e.Turn = turn;

                    addEffect( e );
                }
                break;
            case GameAttackAddType.Fetter:
                {
                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = GameSkillResutlEffect.Fetter;
                    e.Turn = turn;

                    addEffect( e );
                }
                break;
            case GameAttackAddType.Clear:
                {
                    for ( int i = 0 ; i < effects.Count ; )
                    {
                        if ( effects[ i ].Effect == GameSkillResutlEffect.StrUp ||
                            effects[ i ].Effect == GameSkillResutlEffect.VitUp ||
                            effects[ i ].Effect == GameSkillResutlEffect.IntUp ||
                            effects[ i ].Effect == GameSkillResutlEffect.MoveUp ||
                            effects[ i ].Effect == GameSkillResutlEffect.Violent )
                        {
                            destroyEffect( i );
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                break;
        }
    }

    public void skillEffect( GameBattleUnit unit , GameSkill sk )
    {
        int hitAdd = sk.HitAdd <= 0 ? 100 : sk.HitAdd;

        if ( Random.Range( 0 , 100 ) > hitAdd )
        {
            return;
        }
        
        switch ( sk.ResultType )
        {
            case GameSkillResutlType.Purge:
                {
                    for ( int i = 0 ; i < effects.Count ; )
                    {
                        if ( effects[ i ].Effect == GameSkillResutlEffect.Poison ||
                            effects[ i ].Effect == GameSkillResutlEffect.Palsy ||
                            effects[ i ].Effect == GameSkillResutlEffect.Silence ||
                            effects[ i ].Effect == GameSkillResutlEffect.Fetter )
                        {
                            destroyEffect( i );
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                break;
            case GameSkillResutlType.Blessing:
                {
                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = sk.ResultEffect;
                    e.Turn = 3;

                    switch ( e.Effect )
                    {
                        case GameSkillResutlEffect.StrUp:
                            e.Str = 140;
                            break;
                        case GameSkillResutlEffect.VitUp:
                            e.Vit = 200;
                            break;
                        case GameSkillResutlEffect.IntUp:
                            e.Int = 140;
                            break;
                        case GameSkillResutlEffect.MoveUp:
                            e.Mov = 140;
                            break;
                        case GameSkillResutlEffect.Violent:
                            e.Str = 200;
                            e.Vit = 50;
                            break;
                    }

                    addEffect( e );
                }
                break;
            case GameSkillResutlType.Curse:
                {
//                     if ( checkEffect( GameSkillResutlEffect.MagImmunity ) )
//                     {
//                         return;
//                     }

                    GameBattleUnitEffect e = new GameBattleUnitEffect();
                    e.Effect = sk.ResultEffect;
                    e.Turn = 3;

                    addEffect( e );
                }
                break;
            case GameSkillResutlType.None:
            case GameSkillResutlType.Special:
                {
                    //                     if ( checkEffect( GameSkillResutlEffect.MagImmunity ) )
                    //                     {
                    //                         return;
                    //                     }

                    switch ( sk.OtherEffect )
                    {
                        case GameSkillOtherEffect.AttackPoison:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.Poison;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.AttackPalsy:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.Palsy;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.AttackSilence:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.Silence;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.AttackFetter:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.Fetter;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.Invincible0:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.PhyImmunity;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.Invincible1:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = GameSkillResutlEffect.MagImmunity;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                        case GameSkillOtherEffect.AbilityDown:
                            {
                                for ( int i = 0 ; i < effects.Count ; )
                                {
                                    if ( effects[ i ].Effect == GameSkillResutlEffect.StrUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.VitUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.IntUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.MoveUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.Violent )
                                    {
                                        destroyEffect( i );
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                            }
                            break;

                        case GameSkillOtherEffect.HealAll:
                            {
                                for ( int i = 0 ; i < effects.Count ; )
                                {
                                    if ( effects[ i ].Effect == GameSkillResutlEffect.Poison ||
                                        effects[ i ].Effect == GameSkillResutlEffect.Palsy ||
                                        effects[ i ].Effect == GameSkillResutlEffect.Silence ||
                                        effects[ i ].Effect == GameSkillResutlEffect.Fetter )
                                    {
                                        destroyEffect( i );
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                            }
                            break;
                        case GameSkillOtherEffect.AbilityUp:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.OtherEffect = GameSkillOtherEffect.AbilityUp;
                                e.Turn = 1;

                                e.Str = 140;
                                e.Vit = 200;
                                e.Int = 140;
                                e.Mov = 100;

                                addEffect( e );
                            }
                            break;
                    }

                    switch ( sk.ResultEffect )
                    {
                        case GameSkillResutlEffect.Clear:
                            {
                                for ( int i = 0 ; i < effects.Count ; )
                                {
                                    if ( effects[ i ].Effect == GameSkillResutlEffect.StrUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.VitUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.IntUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.MoveUp ||
                                        effects[ i ].Effect == GameSkillResutlEffect.Violent )
                                    {
                                        destroyEffect( i );
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                            }
                            break;
                        case GameSkillResutlEffect.PhyImmunity:
                        case GameSkillResutlEffect.MagImmunity:
                        case GameSkillResutlEffect.Miss:
                        case GameSkillResutlEffect.SummonKiller:
                        case GameSkillResutlEffect.Under:
                            {
                                GameBattleUnitEffect e = new GameBattleUnitEffect();
                                e.Effect = sk.ResultEffect;
                                e.Turn = 3;

                                addEffect( e );
                            }
                            break;
                    }
                    
                }
                break;
        }
    }

    public void updateEffect()
    {
        for ( int i = 0 ; i < effects.Count ; )
        {
            effects[ i ].Turn--;

            if ( effects[ i ].Turn <= 0 )
            {
                destroyEffect( i );
            }
            else
            {
                i++;
            }
        }
    }
    
    public void kill()
    {
        IsKilled = true;

        if ( GameUserData.instance.Stage == 13 )
        {
            if ( UnitID == 55 || UnitID == 56 )
            {
                GameBattleJudgment.instance.Proficiency13++;
            }
        }

        clearEffects();
    }

    public void fadeOut( OnEventOver over )
    {
        onEventOver = over;

        startFadeOut = true;
        alpha = 1.0f;
        time = -0.1f;

        alpha = 0.8f;
        gameAnimation.setColor( new Color( 0.0f , 0.0f , 0.0f ) );
        gameAnimation.setAlpha( alpha );

        gameAnimation.stopAnimation();
    }


    void updateFadeOut()
    {
        if ( !startFadeOut )
        {
            return;
        }

        time += Time.deltaTime;

        float t = 0.8f;

        if ( time >= t )
        {
            alpha = 0.0f;
            time = 0.0f;
            startFadeOut = false;

            if ( shadowSprite != null )
            {
                Color c = shadowSprite.color; c.a = alpha;
                shadowSprite.color = c;

                Destroy( shadowSprite.gameObject );
                shadowSprite.gameObject.transform.SetParent( null );
                shadowSprite = null;
            }

            gameAnimation.setColor( Color.white );
            gameAnimation.setAlpha( alpha );

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }
        else
        {
            alpha = 0.8f - time * ( 0.8f / t );

            if ( shadowSprite != null )
            {
                Color c = shadowSprite.color; c.a = alpha;
                shadowSprite.color = c;
            }

            gameAnimation.setAlpha( alpha );
        }
    }

    void Update()
    {
        updateFadeOut();
    }

}

