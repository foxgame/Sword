using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleAttackUI : Singleton<GameBattleAttackUI>
{
    bool startFade = false;
    bool startFadeKill = false;
    bool alphaAdd = false;

    bool kill = false;

    float alpha;
    Color color;
    float scale;
    float time;

    int jumpHP = 0;

    bool isShow = false;

    int hitCount = 0;

    List<int> hitHP = new List<int>();

    GameAnimation floorAnimation;

    Image blackImage;

    Transform floorTrans;

    Transform left;
    Transform right;

    GameAnimation addAnimation;
    GameAnimation levelUpAnimation;

    GameAnimation criticalAnimation;

    GameAnimation leftAnimation;
    GameAnimation rightAnimation;

    GameAnimation attackerAnimation = null;
    GameAnimation defencerAnimation = null;

    GameBattleInfoUI leftInfoUI;
    GameBattleInfoUI rightInfoUI;

    GameBattleAttackResultSide side;
    GameBattleAttackResultPhysical result;

    List<GameBattleAttackResultSkill> resultSkill;

    GameBattleUnit leftUnit;
    GameBattleUnit rightUnit;

    GameObject white;
    bool startWhite;
    float timeWhite;
    bool showAdd = false;

    GameSkill skill;

    GameBattleAttackMapDirection direction;

    bool rangedAttack;
    bool mapAttack;

    OnEventOver onEventOver;


    bool overUnShow = false;

    public override void initSingleton()
    {
        floorTrans = transform.Find( "floor" );
        blackImage = transform.Find( "black" ).GetComponent<Image>();

        left = transform.Find( "left" );
        right = transform.Find( "right" );

        leftInfoUI = transform.Find( "leftInfo" ).GetComponent<GameBattleInfoUI>();
        rightInfoUI = transform.Find( "rightInfo" ).GetComponent<GameBattleInfoUI>();

        white = transform.Find( "white" ).gameObject;

        gameObject.SetActive( false );
    }

    public void clear()
    {
        startWhite = false;
        startFadeKill = false;
        startFade = false;

        criticalAnimation = null;

        leftAnimation = null;
        rightAnimation = null;

        attackerAnimation = null;
        defencerAnimation = null;

        if ( floorAnimation != null )
        {
            floorAnimation.clearAnimation();

            Destroy( floorAnimation.gameObject );
            floorAnimation = null;
        }

        GameDefine.DestroyAll( left );
        GameDefine.DestroyAll( right );
    }
    

    public void showSkillMap( byte f , GameSkill m , GameBattleUnit lt , GameBattleUnit rt , GameBattleAttackMapDirection dir , GameBattleAttackResultSide s , List<GameBattleAttackResultSkill> r , OnEventOver over )
    {
        if ( m.BattleType == GameSkillBattleType.Normal ||
            m.BattleType == GameSkillBattleType.Map )
        {
            return;
        }

        clear();

        skill = m;
        direction = dir;

        onEventOver = over;
        resultSkill = r;
        result = null;
        side = s;
        isShow = true;
        mapAttack = true;
        overUnShow = true;
        showAdd = false;

        GameUserData.instance.LastSkillID = skill.ID;

        gameObject.SetActive( true );

        leftUnit = lt;
        rightUnit = rt;


        string path = "Prefab/Stage/Stage" + GameDefine.getString2( GameUserData.instance.Stage ) + "/FLOOR_";
        path += GameDefine.getString2( GameUserData.instance.Stage );

        GameObject gameObjectFloor = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        floorAnimation = gameObjectFloor.GetComponent<GameAnimation>();
        floorAnimation.UI = true;
        floorAnimation.showFrame( f );
        floorAnimation.transform.SetParent( floorTrans );
        floorAnimation.transform.localScale = Vector3.one;
        floorAnimation.transform.localPosition = Vector3.zero;


        if ( side == GameBattleAttackResultSide.Left )
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/";
            path += ( GameDefine.getString3( lt.Sprite ) + "MISC" );

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            leftAnimation = obj.GetComponent<GameAnimation>();
            leftAnimation.UI = true;
            leftAnimation.showFrame( 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] ) );
            Transform trans = obj.transform;
            trans.SetParent( left.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            leftInfoUI.gameObject.SetActive( true );
            leftInfoUI.setValue( lt.HP , lt.HPMax , lt.Name );
            rightInfoUI.gameObject.SetActive( false );


            path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/" +
                GameDefine.getString3( lt.Sprite );
            path += ( m.BattleSprite > 0 ? "-" + m.BattleSprite : "" );
            path += "s";

            obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( left.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = null;
        }
        else
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/";
            path += ( GameDefine.getString3( rt.Sprite ) + "MISC" );

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            rightAnimation = obj.GetComponent<GameAnimation>();
            rightAnimation.UI = true;
            rightAnimation.showFrame( 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] ) );
            Transform trans = obj.transform;
            trans.SetParent( right.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            rightInfoUI.gameObject.SetActive( true );
            rightInfoUI.setValue( rt.HP , rt.HPMax , rt.Name );
            leftInfoUI.gameObject.SetActive( false );


            path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/" +
                GameDefine.getString3( rt.Sprite );
            path += ( m.BattleSprite > 0 ? "-" + m.BattleSprite : "" );
            path += "s";

            obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( right.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = null;
        }

        scale = 2.0f;
        alpha = 0.1f;
        time = 0.0f;
        alphaAdd = true;
        startFade = true;

        updateStage();
    }

    void onLevelShowOver()
    {
        levelUpAnimation.stopAnimation();
        levelUpAnimation.clearAnimation();
        Destroy( levelUpAnimation.gameObject );
        levelUpAnimation.transform.SetParent( null );
        levelUpAnimation = null;
    }

    void onAddShowOver()
    {
        addAnimation.stopAnimation();
        addAnimation.clearAnimation();
        Destroy( addAnimation.gameObject );
        addAnimation.transform.SetParent( null );
        addAnimation = null;
    }

    public void showLevelUp( OnEventOver over )
    {
        if ( !isShow )
        {
            over();
            return;
        }

        if ( side == GameBattleAttackResultSide.Left )
        {
            if ( rangedAttack )
            {
                rightAnimation.stopAnimation();
                rightAnimation.clearAnimation();
            }
        }
        else
        {
            if ( rangedAttack )
            {
                leftAnimation.stopAnimation();
                leftAnimation.clearAnimation();
            }
        }

        string path = "Prefab/Misc/Ftlup_r";

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        Transform trans = obj.transform;
        trans.SetParent( right.transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        levelUpAnimation = obj.GetComponent<GameAnimation>();
        levelUpAnimation.UI = true;
        levelUpAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onLevelShowOver );

        rightAnimation.playAnimation( 6 , 6 + rightAnimation.safHead.count3[ 0 ] , false , over );
    }

    public void showSkill( byte f , GameSkill m , GameBattleUnit lt , GameBattleUnit rt , GameBattleAttackMapDirection dir , GameBattleAttackResultSide s , GameBattleAttackResultPhysical rs , OnEventOver over )
    {
        if ( m.BattleType == GameSkillBattleType.Normal ||
            m.BattleType == GameSkillBattleType.Map )
        {
            return;
        }

        clear();

        skill = m;
        direction = dir;

        onEventOver = over;
        result = rs;
        resultSkill = null;
        side = s;
        isShow = true;
        overUnShow = false;
        showAdd = false;

        GameUserData.instance.LastSkillID = skill.ID;

        gameObject.SetActive( true );

        leftUnit = lt;
        rightUnit = rt;

        string path = "Prefab/Stage/Stage" + GameDefine.getString2( GameUserData.instance.Stage ) + "/FLOOR_";
        path += GameDefine.getString2( GameUserData.instance.Stage );

        GameObject gameObjectFloor = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        floorAnimation = gameObjectFloor.GetComponent<GameAnimation>();
        floorAnimation.UI = true;
        floorAnimation.showFrame( f );
        floorAnimation.transform.SetParent( floorTrans );
        floorAnimation.transform.localScale = Vector3.one;
        floorAnimation.transform.localPosition = Vector3.zero;


        //        string ab = result.type == GameBattleAttackResultType.Block ? "ab" : "a";

        path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/";
        path += ( GameDefine.getString3( rt.Sprite ) + "MISC" );

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        rightAnimation = obj.GetComponent<GameAnimation>();
        rightAnimation.UI = true;
        rightAnimation.showFrame( 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] ) );
        Transform trans = obj.transform;
        trans.SetParent( right.transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        rightInfoUI.gameObject.SetActive( true );
        rightInfoUI.setValue( rt.HP , rt.HPMax , rt.Name );



        path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/";
        path += ( GameDefine.getString3( lt.Sprite ) + "MISC" );

        GameObject obj1 = Resources.Load<GameObject>( path );

        if ( obj1 == null )
        {
            unShow();
            return;
        }

        obj = Instantiate<GameObject>( obj1 );
        leftAnimation = obj.GetComponent<GameAnimation>();
        leftAnimation.UI = true;
        leftAnimation.showFrame( 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] ) );
        trans = obj.transform;
        trans.SetParent( left.transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        leftInfoUI.gameObject.SetActive( true );
        leftInfoUI.setValue( lt.HP , lt.HPMax , lt.Name );

        rangedAttack = ( m.BattleRanged == GameSkillBattleRanged.Ranged );
        mapAttack = ( m.BattleType == GameSkillBattleType.UIMap );

        if ( side == GameBattleAttackResultSide.Left )
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/" +
                GameDefine.getString3( lt.Sprite );
            path += ( m.BattleSprite > 0 ? "-" + m.BattleSprite : "" );
            path += ( result.type == GameBattleAttackResultPhysical.Type.Block ? "sb" : "s" );

            obj1 = Resources.Load<GameObject>( path );

            if ( obj1 == null )
            {
                unShow();
                return;
            }

            obj = Instantiate<GameObject>( obj1 );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( left.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = rightAnimation;

            RectTransform trans1 = left.GetComponent<RectTransform>();
            trans1.SetSiblingIndex( 3 );

            if ( rangedAttack ||
                mapAttack )
            {
                rightAnimation.clearAnimation();
            }
        }
        else
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/" +
                GameDefine.getString3( rt.Sprite );
            path += ( m.BattleSprite > 0 ? "-" + m.BattleSprite : "" );
            path += ( result.type == GameBattleAttackResultPhysical.Type.Block ? "sb" : "s" );

            obj1 = Resources.Load<GameObject>( path );

            if ( obj1 == null )
            {
                unShow();
                return;
            }

            obj = Instantiate<GameObject>( obj1 );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( right.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = leftAnimation;

            RectTransform trans1 = right.GetComponent<RectTransform>();
            trans1.SetSiblingIndex( 3 );

            if ( rangedAttack ||
                mapAttack )
            {
                leftAnimation.clearAnimation();
            }
        }

        attackerAnimation.otherGameAnimation = defencerAnimation;

        scale = 2.0f;
        alpha = 0.1f;
        time = 0.0f;
        alphaAdd = true;
        startFade = true;

        updateStage();
    }

    public void show( byte f , GameBattleUnit lt , GameBattleUnit rt , GameBattleAttackResultSide s , GameBattleAttackResultPhysical rs , bool o , OnEventOver over )
    {
        clear();

        skill = null;

        onEventOver = over;
        result = rs;
        resultSkill = null;
        side = s;
        isShow = true;
        overUnShow = false;
        showAdd = false;
        mapAttack = false;

        gameObject.SetActive( true );

        leftUnit = lt;
        rightUnit = rt;

        string path = "Prefab/Stage/Stage" + GameDefine.getString2( GameUserData.instance.Stage ) + "/FLOOR_";
        path += GameDefine.getString2( GameUserData.instance.Stage );

        GameObject gameObjectFloor = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        floorAnimation = gameObjectFloor.GetComponent<GameAnimation>();
        floorAnimation.UI = true;
        floorAnimation.showFrame( f );
        floorAnimation.transform.SetParent( floorTrans );
        floorAnimation.transform.localScale = Vector3.one;
        floorAnimation.transform.localPosition = Vector3.zero;


        //        string ab = result.type == GameBattleAttackResultType.Block ? "ab" : "a";

        path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/";
        path += ( GameDefine.getString3( rt.Sprite ) + "MISC" );

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        rightAnimation = obj.GetComponent<GameAnimation>();
        rightAnimation.UI = true;
        rightAnimation.showFrame( 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] ) );
        Transform trans = obj.transform;
        trans.SetParent( right.transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        rightInfoUI.gameObject.SetActive( true );
        rightInfoUI.setValue( rt.HP , rt.HPMax , rt.Name );



        path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/";
        path += ( GameDefine.getString3( lt.Sprite ) + "MISC" );

        obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        leftAnimation = obj.GetComponent<GameAnimation>();
        leftAnimation.UI = true;
        leftAnimation.showFrame( 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] ) );
        trans = obj.transform;
        trans.SetParent( left.transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        leftInfoUI.gameObject.SetActive( true );
        leftInfoUI.setValue( lt.HP , lt.HPMax , lt.Name );



        if ( side == GameBattleAttackResultSide.Left )
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( lt.Sprite ) + "/" + 
                GameDefine.getString3( lt.Sprite );
            path += ( result.type == GameBattleAttackResultPhysical.Type.Block ? "ab" : "a" );

            GameObject object1 = Resources.Load<GameObject>( path );

            if ( object1 == null )
            {
                onShowOver();
                return;
            }

            obj = Instantiate<GameObject>( object1 );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( left.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = rightAnimation;

            RectTransform trans1 = left.GetComponent<RectTransform>();
            trans1.SetSiblingIndex( 3 );

            rangedAttack = ( lt.AttackType == GameUnitAttackType.Ranged );

            if ( rangedAttack )
            {
                rightAnimation.clearAnimation();
            }
        }
        else
        {
            path = "Prefab/Sprite/man" + GameDefine.getString3( rt.Sprite ) + "/" +
                GameDefine.getString3( rt.Sprite );
            path += ( result.type == GameBattleAttackResultPhysical.Type.Block ? "ab" : "a" );

            GameObject object1 = Resources.Load<GameObject>( path );

            if ( object1 == null )
            {
                onShowOver();
                return;
            }

            obj = Instantiate<GameObject>( object1 );
            attackerAnimation = obj.GetComponent<GameAnimation>();
            attackerAnimation.UI = true;
            attackerAnimation.onAnimationEvent = onAnimationEvent;
            trans = obj.transform;
            trans.SetParent( right.transform );
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            defencerAnimation = leftAnimation;

            RectTransform trans1 = right.GetComponent<RectTransform>();
            trans1.SetSiblingIndex( 3 );

            rangedAttack = ( rt.AttackType == GameUnitAttackType.Ranged );

            if ( rangedAttack )
            {
                leftAnimation.clearAnimation();
            }
        }

        attackerAnimation.otherGameAnimation = defencerAnimation;

        if ( o )
        {
            onShowOver();
        }
        else
        {
            scale = 2.0f;
            alpha = 0.1f;
            time = 0.0f;
            alphaAdd = true;
            startFade = true;

            updateStage();
        }
    }

    public void unShow( OnEventOver over )
    {
        onEventOver = over;

        if ( !isShow )
        {
            onEventOver();
            return;
        }

        unShow();
    }

    void unShow()
    {
        
        if ( leftAnimation != null )
            leftAnimation.stopAnimation();

        if ( rightAnimation != null )
            rightAnimation.stopAnimation();

        isShow = false;

        scale = 1.0f;
        alpha = 1.0f;
        time = 0.0f;
        alphaAdd = false;
        startFade = true;

        updateStage();
    }

    void onJumpHPOver()
    {
        if ( result.AddType == GameAttackAddType.AbsorbHP &&
            result.AddValue > 0 )
        {
            if ( side == GameBattleAttackResultSide.Right )
            {
                GameBattleJumpHPUI.instance.jump( GameBattleAttackResultSide.Left , -result.AddValue , onJumpHPOver1 );
                rightInfoUI.setValue( rightUnit.HP + result.AddValue , rightUnit.HPMax , rightUnit.Name );
            }
            else
            {
                GameBattleJumpHPUI.instance.jump( GameBattleAttackResultSide.Right , -result.AddValue , onJumpHPOver1 );
                leftInfoUI.setValue( leftUnit.HP + result.AddValue , leftUnit.HPMax , leftUnit.Name );
            }

            return;
        }

        onJumpHPOver1();
    }

    void onJumpHPOver1()
    {
        if ( kill )
        {
            if ( side == GameBattleAttackResultSide.Left )
            {
                rightAnimation.stopAnimation();
                rightAnimation.setColor( new Color( 0.0f , 0.0f , 0.0f ) );
            }
            else
            {
                leftAnimation.stopAnimation();
                leftAnimation.setColor( new Color( 0.0f , 0.0f , 0.0f ) );
            }

            startFadeKill = true;
            alpha = 0.8f;
            time = -0.1f;

            updateFadeKill();
        }
        else
        {
            if ( overUnShow )
            {
                unShow();
            }
            else
            {
                if ( onEventOver != null )
                {
                    onEventOver();
                }
            }
        }
    }

    void onJumpHPOver2()
    {
        if ( result.AddType == GameAttackAddType.AbsorbHP &&
            result.AddValue > 0 )
        {
            if ( side == GameBattleAttackResultSide.Right )
            {
                GameBattleJumpHPUI.instance.jump( GameBattleAttackResultSide.Left , -result.AddValue , onJumpHPOver3 );
                rightInfoUI.setValue( rightUnit.HP + result.AddValue , rightUnit.HPMax , rightUnit.Name );
            }
            else
            {
                GameBattleJumpHPUI.instance.jump( GameBattleAttackResultSide.Right , -result.AddValue , onJumpHPOver3 );
                leftInfoUI.setValue( leftUnit.HP + result.AddValue , leftUnit.HPMax , leftUnit.Name );
            }

            return;
        }

        onJumpHPOver3();
    }

    void onJumpHPOver3()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            leftAnimation.stopAnimation();
            leftAnimation.clearAnimation();

            if ( rangedAttack )
            {
                rightAnimation.stopAnimation();
                rightAnimation.clearAnimation();
            }
        }
        else
        {
            rightAnimation.stopAnimation();
            rightAnimation.clearAnimation();

            if ( rangedAttack )
            {
                leftAnimation.stopAnimation();
                leftAnimation.clearAnimation();
            }
        }

        attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );
    }

    void updateStageKill()
    {
        if ( side == GameBattleAttackResultSide.Left )
        {
            rightAnimation.setAlpha( alpha );
        }
        else
        {
            leftAnimation.setAlpha( alpha );
        }
    }

    void updateStage()
    {
        transform.localScale = new Vector3( scale , scale , scale );

        if ( floorAnimation != null )
        {
            floorAnimation.setAlpha( alpha );
        }

        blackImage.color = new Color( 0.0f , 0.0f , 0.0f , alpha );

        if ( kill )
        {
            if ( side == GameBattleAttackResultSide.Left )
            {
                if ( leftAnimation != null )
                    leftAnimation.setAlpha( alpha );
            }
            else
            {
                if ( rightAnimation != null )
                    rightAnimation.setAlpha( alpha );
            }
        }
        else
        {
            if ( attackerAnimation != null )
            {
                attackerAnimation.setAlpha( alpha );
            }

            if ( leftAnimation != null )
                leftAnimation.setAlpha( alpha );

            if ( rightAnimation != null )
                rightAnimation.setAlpha( alpha );
        }
    }

    private void onAnimationEvent( int i )
    {
        if ( mapAttack )
        {
            return;
        }

        GameAnimation.SAF1 saf1 = attackerAnimation.saf1[ i ];

        bool hideDefencer = ( attackerAnimation.safHead.count3[ 1 ] > 0 &&
            i >= attackerAnimation.safHead.count3[ 1 ] && 
            i < attackerAnimation.safHead.count3[ 2 ] );

        if ( hideDefencer && defencerAnimation.start )
        {
            defencerAnimation.stopAnimation();
            defencerAnimation.clearAnimation();
        }

        if ( saf1.hit > 0 )
        {
            if ( !showAdd && result.AddType != GameAttackAddType.None )
            {
                showAdd = true;

                switch ( result.AddType )
                {
                    case GameAttackAddType.AbsorbHP:
                        break;
                    case GameAttackAddType.Poison:
                        {
                            string path = "Prefab/Misc/Sp003" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

                            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
                            addAnimation = obj.GetComponent<GameAnimation>();
                            addAnimation.UI = true;

                            obj.transform.SetParent( attackerAnimation.transform.parent );
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localScale = Vector3.one;

                            addAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAddShowOver );
                        }
                        break;
                    case GameAttackAddType.Palsy:
                        {
                            string path = "Prefab/Misc/Sp005" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

                            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
                            addAnimation = obj.GetComponent<GameAnimation>();
                            addAnimation.UI = true;

                            obj.transform.SetParent( attackerAnimation.transform.parent );
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localScale = Vector3.one;

                            addAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAddShowOver );
                        }
                        break;
                    case GameAttackAddType.Silence:
                        {
                            string path = "Prefab/Misc/Sp002" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

                            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
                            addAnimation = obj.GetComponent<GameAnimation>();
                            addAnimation.UI = true;

                            obj.transform.SetParent( attackerAnimation.transform.parent );
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localScale = Vector3.one;

                            addAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAddShowOver );
                        }
                        break;
                    case GameAttackAddType.Fetter:
                        break;
                    case GameAttackAddType.Clear:
                        break;
                }
            }

            if ( result.type == GameBattleAttackResultPhysical.Type.Block )
            {
                if ( defencerAnimation.startFrame != 0 && 
                    !hideDefencer )
                {
                    defencerAnimation.playAnimation( 0 , 1 );
                }
            }
            else
            {
                if ( result.type == GameBattleAttackResultPhysical.Type.Critical )
                {
                    white.SetActive( true );
                    startWhite = true;
                    timeWhite = 0.0f;
                }

                if ( defencerAnimation.startFrame != 1 &&
                    !hideDefencer )
                {
                    defencerAnimation.playAnimation( 1 , 3 );
                }
            }

            if ( side == GameBattleAttackResultSide.Left )
            {
                rightInfoUI.setValue( hitHP[ hitCount ] );
            }
            else
            {
                leftInfoUI.setValue( hitHP[ hitCount ] );
            }
            
            hitCount++;
        }
        else
        {
            if ( rangedAttack && attackerAnimation.safHead.count3[ 0 ] > i )
            {
                return;
            }

            int f = 6 + ( defencerAnimation.safHead.count3[ 0 ] == 0 ? 1 : defencerAnimation.safHead.count3[ 0 ] );

            if ( defencerAnimation.startFrame != f &&
                !hideDefencer )
            {
                defencerAnimation.playAnimation( f , f + defencerAnimation.safHead.count3[ 1 ] );
            }
        }
    }

    void onAttackOver2()
    {
        GameBattleJumpHPUI.instance.jump( side , jumpHP , onJumpHPOver2 );

        attackerAnimation.stopAnimation();
        attackerAnimation.clearAnimation();

        if ( side == GameBattleAttackResultSide.Left )
        {
            int f = 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] );

            if ( !rangedAttack )
                leftAnimation.playAnimation( f , f + leftAnimation.safHead.count3[ 1 ] );

            f = 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] );
            rightAnimation.playAnimation( f , f + rightAnimation.safHead.count3[ 1 ] );
        }
        else
        {
            int f = 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] );

            if ( !rangedAttack )
                rightAnimation.playAnimation( f , f + rightAnimation.safHead.count3[ 1 ] );

            f = 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] );
            leftAnimation.playAnimation( f , f + leftAnimation.safHead.count3[ 1 ] );
        }

        int HP = 0;

        if ( side == GameBattleAttackResultSide.Left )
        {
            HP = rightUnit.HP;
        }
        else
        {
            HP = leftUnit.HP;
        }

        kill = ( HP <= result.HP + result.HP2 );

        hitHP.Clear();

        jumpHP = result.HP2;

        hitCount = 0;
        for ( int i = 0 ; i < attackerAnimation.saf1.Length ; i++ )
        {
            if ( attackerAnimation.saf1[ i ].hit > 0 )
            {
                hitCount++;
            }
        }

        int[] hit = new int[ hitCount ];

        hitCount = 0;
        for ( int i = 0 ; i < attackerAnimation.saf1.Length ; i++ )
        {
            if ( attackerAnimation.saf1[ i ].hit > 0 )
            {
                hit[ hitCount ] = i;
                hitCount++;
            }
        }

        int hitAll = 0;
        for ( int i = 0 ; i < hitCount ; i++ )
        {
            hitAll += hit[ i ];
        }

        float per = 0.0f;
        for ( int i = 0 ; i < hitCount ; i++ )
        {
            per += hit[ i ] / (float)hitAll;

            if ( i == hitCount - 1 )
            {
                hitHP.Add( HP - result.HP - jumpHP );
            }
            else
            {
                hitHP.Add( HP - result.HP - (int)( per * jumpHP ) );
            }

        }

        hitCount = 0;
    }

    void onAttackOver()
    {
        if ( mapAttack )
        {
            unShow();
            return;
        }

        attackerAnimation.stopAnimation();
        attackerAnimation.clearAnimation();

        if ( side == GameBattleAttackResultSide.Left )
        {
            int f = 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] );

            if ( !rangedAttack )
                leftAnimation.playAnimation( f , f + leftAnimation.safHead.count3[ 1 ] );

            f = 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] );
            rightAnimation.playAnimation( f , f + rightAnimation.safHead.count3[ 1 ] );
        }
        else
        {
            int f = 6 + ( rightAnimation.safHead.count3[ 0 ] == 0 ? 1 : rightAnimation.safHead.count3[ 0 ] );

            if ( !rangedAttack )
                rightAnimation.playAnimation( f , f + rightAnimation.safHead.count3[ 1 ] );

            f = 6 + ( leftAnimation.safHead.count3[ 0 ] == 0 ? 1 : leftAnimation.safHead.count3[ 0 ] );
            leftAnimation.playAnimation( f , f + leftAnimation.safHead.count3[ 1 ] );
        }

        GameBattleJumpHPUI.instance.jump( side , jumpHP , onJumpHPOver );

//        unShow();
    }

    void onShowOver()
    {
        if ( result != null )
        {
            int HP = 0;

            if ( side == GameBattleAttackResultSide.Left )
            {
                HP = rightUnit.HP;
            }
            else
            {
                HP = leftUnit.HP;
            }

            kill = ( HP <= result.HP );

            hitHP.Clear();

            jumpHP = result.HP;

            hitCount = 0;
            for ( int i = 0 ; i < attackerAnimation.saf1.Length ; i++ )
            {
                if ( attackerAnimation.saf1[ i ].hit > 0 )
                {
                    hitCount++;
                }
            }

            int[] hit = new int[ hitCount ];

            hitCount = 0;
            for ( int i = 0 ; i < attackerAnimation.saf1.Length ; i++ )
            {
                if ( attackerAnimation.saf1[ i ].hit > 0 )
                {
                    hit[ hitCount ] = i;
                    hitCount++;
                }
            }

            int hitAll = 0;
            for ( int i = 0 ; i < hitCount ; i++ )
            {
                hitAll += hit[ i ];
            }

            float per = 0.0f;
            for ( int i = 0 ; i < hitCount ; i++ )
            {
                per += hit[ i ] / (float)hitAll;

                if ( i == hitCount - 1 )
                {
                    hitHP.Add( HP - jumpHP );
                }
                else
                {
                    hitHP.Add( HP - (int)( per * jumpHP ) );
                }
            }

            hitCount = 0;
        }


        if ( skill != null )
        {
            GameBattleSkillTitleUI.instance.show( skill.Name );

            string path = "Prefab/Misc/Sp000" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
            criticalAnimation = obj.GetComponent<GameAnimation>();
            criticalAnimation.UI = true;

            obj.transform.SetParent( attackerAnimation.transform.parent );
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            criticalAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onSkill );
        }
        else if ( result.type == GameBattleAttackResultPhysical.Type.Critical )
        {
            string path = "Prefab/Misc/Sp000" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
            criticalAnimation = obj.GetComponent<GameAnimation>();
            criticalAnimation.UI = true;

            obj.transform.SetParent( attackerAnimation.transform.parent );
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            criticalAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onCritical );
        }
        else if ( result.type == GameBattleAttackResultPhysical.Type.Double )
        {
            string path = "Prefab/Misc/Sp000" + ( side == GameBattleAttackResultSide.Left ? "l" : "r" );

            GameObject obj = Instantiate( Resources.Load<GameObject>( path ) );
            criticalAnimation = obj.GetComponent<GameAnimation>();
            criticalAnimation.UI = true;

            obj.transform.SetParent( attackerAnimation.transform.parent );
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            criticalAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onDouble );
        }
        else
        {
            attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );

            if ( side == GameBattleAttackResultSide.Left )
            {
                leftAnimation.stopAnimation();
                leftAnimation.clearAnimation();
            }
            else
            {
                rightAnimation.stopAnimation();
                rightAnimation.clearAnimation();
            }
        }

        if ( !rangedAttack && !mapAttack )
        {
            int f = 6 + ( defencerAnimation.safHead.count3[ 0 ] == 0 ? 1 : defencerAnimation.safHead.count3[ 0 ] );
            defencerAnimation.playAnimation( f , f + defencerAnimation.safHead.count3[ 1 ] );
        }
    }

    void onDouble()
    {
        criticalAnimation.clearAnimation();

        if ( side == GameBattleAttackResultSide.Left )
        {
            leftAnimation.stopAnimation();
            leftAnimation.clearAnimation();
        }
        else
        {
            rightAnimation.stopAnimation();
            rightAnimation.clearAnimation();
        }

        attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver2 );
    }

    void onSkill()
    {
        criticalAnimation.clearAnimation();

        if ( side == GameBattleAttackResultSide.Left )
        {
            leftAnimation.stopAnimation();
            leftAnimation.clearAnimation();
        }
        else
        {
            rightAnimation.stopAnimation();
            rightAnimation.clearAnimation();
        }

        attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );
    }

    void onCritical()
    {
        criticalAnimation.clearAnimation();

        if ( side == GameBattleAttackResultSide.Left )
        {
            leftAnimation.stopAnimation();
            leftAnimation.clearAnimation();
        }
        else
        {
            rightAnimation.stopAnimation();
            rightAnimation.clearAnimation();
        }

        attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );
    }

    void onUnShowOver()
    {
        gameObject.SetActive( false );

        if ( mapAttack )
        {
            GameBattleAttackMap.instance.showSkillMap( skill ,
                (side == GameBattleAttackResultSide.Left ? leftUnit : rightUnit ) , direction , resultSkill , onEventOver );
        }
        else
        {
            if ( onEventOver != null )
            {
                onEventOver();
            }
        }

        clear();
    }


    void showEffect()
    {
//        bool b = false;

//         for ( int i = 0 ; i < result.Count ; i++ )
//         {
//             string path = "Prefab/Misc/MAN_RES";
// 
//             GameBattleUnit u = result[ i ].unit;
// 
//             if ( u == null )
//             {
//                 onMoveOver();
//                 return;
//             }
// 
//             GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
//             GameAnimation gameAnimation = obj.GetComponent<GameAnimation>();
//             gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
//             gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;
// 
//             Transform trans = obj.transform;
//             trans.SetParent( transform );
//             trans.localScale = Vector3.one;
//             trans.position = new Vector3( u.PosBattleX ,
//                 u.PosBattleY + GameBattleManager.instance.LayerHeight ,
//                 transform.localPosition.z );
// 
// 
// 
//             string path1 = "Prefab/JumpHPMap";
// 
//             GameObject obj1 = Instantiate<GameObject>( Resources.Load<GameObject>( path1 ) );
//             GameBattleJumpHPMap jumpHP = obj1.GetComponent<GameBattleJumpHPMap>();
// 
//             Vector3 screenPos = GameCameraManager.instance.mainCamera.WorldToScreenPoint( trans.position );
//             screenPos.z = 0;
// 
//             RectTransform trans1 = obj1.GetComponent<RectTransform>();
//             trans1.SetParent( GameObject.Find( "Canvas/UI/Center/JumpHPMap" ).transform );
//             trans1.localScale = Vector3.one;
//             trans1.position = screenPos;
//             trans1.anchoredPosition = new Vector2( GameDefine.TEXTURE_WIDTH_HALF + trans1.anchoredPosition.x ,
//                 trans1.anchoredPosition.y - GameDefine.TEXTURE_HEIGHT );
// 
//             jumpHPlist.Add( jumpHP );
// 
//             int animationStart = 2;
//             int animationEnd = 10;
// 
//             switch ( result[ i ].type )
//             {
//                 case GameSkillResutlType.Damage:
//                     animationStart = 2;
//                     animationEnd = 10;
//                     break;
//                 case GameSkillResutlType.Cure:
//                     animationStart = 46;
//                     animationEnd = 60;
//                     break;
//                 case GameSkillResutlType.Purge:
//                     animationStart = 60;
//                     animationEnd = 81;
//                     break;
//                 case GameSkillResutlType.Blessing:
//                     animationStart = 18;
//                     animationEnd = 32;
//                     break;
//                 case GameSkillResutlType.Curse:
//                     animationStart = 32;
//                     animationEnd = 46;
//                     break;
//                 case GameSkillResutlType.None:
//                 case GameSkillResutlType.Special:
//                     {
//                         animationStart = 81;
//                         animationEnd = 99;
// 
//                         if ( result[ i ].HP > 0 || result[ i ].MP > 0 )
//                         {
//                             if ( skill.OtherEffect == GameSkillOtherEffect.HealAll ||
//                                 skill.OtherEffect == GameSkillOtherEffect.MPAdd )
//                             {
//                                 animationStart = 46;
//                                 animationEnd = 60;
//                             }
//                             else
//                             {
//                                 animationStart = 2;
//                                 animationEnd = 10;
//                             }
//                         }
//                     }
//                     break;
//             }
// 
//             OnEventOver over = null;
// 
//             if ( result[ i ].HP != 0 || result[ i ].MP != 0 )
//             {
//                 over = onResOver;
//             }
//             else
//             {
//                 over = onJumpHPOver;
//             }
// 
//             b = true;
// 
//             gameAnimation.playSound = ( i == 0 );
//             gameAnimation.playAnimation( animationStart , animationEnd , false , over );
//         }
// 
//         if ( !b )
//         {
//             onJumpHPOver();
//         }
    }


    void updateFade()
    {
        float t = 0.5f;

        if ( !startFade )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time >= t )
        {
            if ( alphaAdd )
            {
                scale = 1.0f;
                alpha = 1.0f;

                onShowOver();
            }
            else
            {
                scale = 2.0f;
                alpha = 0.0f;

                onUnShowOver();
            }

            time = 0.0f;
            startFade = false;

            updateStage();
        }
        else
        {
            if ( alphaAdd )
            {
                if ( time > t - 0.5f )
                {
                    alpha = 0.1f + ( time - ( t - 0.5f ) ) * 2.0f;
                }

                scale = 2.0f - time * ( 1.0f / t );
            }
            else
            {
                alpha = 1.0f - time * ( 1.0f / t );
                scale = 1.0f + time * ( 1.0f / t );
            }

            updateStage();
        }
    }

    void updateFadeKill()
    {
        float t = 0.8f;

        if ( !startFadeKill )
        {
            return;
        }

        time += Time.deltaTime;
       
        if ( time >= t )
        {
            alpha = 0.0f;
            time = 0.0f;
            startFadeKill = false;

            if ( side == GameBattleAttackResultSide.Left )
            {
                rightAnimation.setColor( Color.white );
            }
            else
            {
                leftAnimation.setColor( Color.white );
            }

            updateStageKill();

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }
        else
        {
            alpha = 0.8f - time * ( 0.8f / t );

            updateStageKill();
        }
    }


    void updateWhite()
    {
        if ( !startWhite )
        {
            return;
        }

        timeWhite += Time.deltaTime;

        if ( timeWhite > 0.05f )
        {
            white.SetActive( false );
            startWhite = false;
            timeWhite = 0.0f;
        }
    }

    void Update()
    {
        updateWhite();
        updateFade();
        updateFadeKill();
    }

}

