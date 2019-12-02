using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameBattleAttackMapDirection
{
    North = 0,
    East,
    South,
    West,

    Count
}


public class GameBattleAttackMap : Singleton<GameBattleAttackMap>
{
    OnEventOver onEventOver;

    GameSkill skill;
    GameItem item;

    GameBattleUnit unit;
    List<GameBattleAttackResultSkill> result;
    List<GameBattleAttackResultItem> resultItem;

    GameBattleAttackMapDirection direction;
    List<GameBattleJumpHPMap> jumpHPlist = new List<GameBattleJumpHPMap>();

    RectTransform jumpHPTrans;

    public override void initSingleton()
    {
        jumpHPTrans = GameObject.Find( "Canvas/UI/Center/JumpHPMap" ).GetComponent<RectTransform>();
    }

    public void clearJumpHP()
    {
        GameDefine.DestroyAll( jumpHPTrans );
    }

    public void clear()
    {
        GameDefine.DestroyAll( transform );
    }


    public void showItem( GameItem m , GameBattleUnit u , GameBattleAttackMapDirection dir , List<GameBattleAttackResultItem> ri , List<GameBattleAttackResultSkill> r , OnEventOver over )
    {
        clear();

        item = m;
        skill = null;

        unit = u;
        direction = dir;
        onEventOver = over;

        resultItem = ri;
        result = r;

        GameUserData.instance.LastItemID = item.ID;

        if ( item.UseSkillID != GameDefine.INVALID_ID )
        {
            skill = GameSkillData.instance.getData( item.UseSkillID );

            showSkill( skill , u , dir , r , over );
        }
        else
        {
            onAttackOverItem();
        }
    }


    public void showSkill( GameSkill m , GameBattleUnit u , GameBattleAttackMapDirection dir , List<GameBattleAttackResultSkill> r , OnEventOver over )
    {
        clear();

        skill = m;
        item = null;

        unit = u;
        result = r;
        direction = dir;
        onEventOver = over;

        GameUserData.instance.LastSkillID = skill.ID;

        string path = "Prefab/Misc/Mag_misc";

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        GameAnimation gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
        gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y - 18;

        unit.playAnimationBattle( GameAnimationType.Skill , u.Direction , null );

        Transform trans = obj.transform;
        trans.SetParent( transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        transform.localPosition = new Vector3( unit.PosBattleX ,
            unit.PosBattleY + GameBattleManager.instance.LayerHeight ,
            transform.localPosition.z );

        gameAnimation.playAnimation( 1 , GameDefine.INVALID_ID , false , onStartOver );
    }


    public void showSkillMap( GameSkill sk , GameBattleUnit u , GameBattleAttackMapDirection dir , List<GameBattleAttackResultSkill> r , OnEventOver over )
    {
        clear();

        skill = sk;
        unit = u;
        result = r;
        direction = dir;
        onEventOver = over;

        GameUserData.instance.LastSkillID = skill.ID;

        //         string path = "Prefab/Sprite/man" + GameDefine.getString3( u.Sprite ) + "/" +
        //     GameDefine.getString3( u.Sprite );
        //         path += ( m.BattleSprite > 0 ? "-" + m.BattleSprite : "" );
        //         path += "sm";
        // 
        //         GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        //         GameAnimation attackerAnimation = obj.GetComponent<GameAnimation>();
        //         Transform trans = obj.transform;
        //         trans.SetParent( transform );
        //         trans.localScale = Vector3.one;
        //         trans.localPosition = Vector3.zero;
        // 
        //         transform.localPosition = new Vector3( GameSceneMovement.instance.posXReal , 
        //             GameSceneMovement.instance.posYReal + GameBattleManager.instance.LayerHeight ,
        //             transform.localPosition.z );
        // 
        //         float x = GameCameraManager.instance.sceneWidth / GameDefine.SCENE_WIDTH;
        //         transform.localScale = new Vector3( x , 1.0f , 1.0f );
        // 
        //         if ( dir != GameBattleAttackMapDirection.Count )
        //         {
        //             int c = attackerAnimation.safHead.count3[ 0 ];
        //             int f = c * (int)dir;
        // 
        //             attackerAnimation.playAnimation( f , f + c , false , onAttackOver );
        //         }
        //         else
        //         {
        //             attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );
        //         }

        if ( skill.AttackRangeType == GameAttackRangeType.Line )
        {
            GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell ,
        unit.PosY - GameCameraManager.instance.yCell , 50 , 50 , onStartMoveOver );
        }
        else
        {
            GameBattleSceneMovement.instance.moveTo( GameBattleCursor.instance.PosX - GameCameraManager.instance.xCell ,
        GameBattleCursor.instance.PosY - GameCameraManager.instance.yCell , 50 , 50 , onStartMoveOver );
        }
    }

    public void showRes( List<GameBattleAttackResultSkill> r , OnEventOver over )
    {
        skill = null;
        unit = null;
        result = r;
        direction = GameBattleAttackMapDirection.Count;
        onEventOver = over;

        onAttackOver();
    }


    void onStartMoveOver()
    {
        string path = "";

        int offsetX = 0;
        int offsetY = 0;

        switch ( skill.BattleType )
        {
            case GameSkillBattleType.Normal:
                {
                    path = "Prefab/Magic/magic" + GameDefine.getString3( skill.AnimationID ) + "/";
                    path += "M";
                    path += skill.AnimationID;
                }
                break;
            case GameSkillBattleType.UI:
                {
                    return;
                }
            case GameSkillBattleType.Map:
                {
                    if ( skill.AnimationID > 0 && skill.BattleSprite == 0 )
                    {
                        path = "Prefab/Magic/magic" + GameDefine.getString3( skill.AnimationID ) + "/";
                        path += "M";
                        path += skill.AnimationID;
                    }
                    else
                    {
                        unit.stopAnimation();
                        unit.clearAnimation();

                        path = "Prefab/Sprite/man" + GameDefine.getString3( unit.Sprite ) + "/" +
    GameDefine.getString3( unit.Sprite );
                        path += ( skill.BattleSprite > 0 ? "-" + skill.BattleSprite : "" );
                        path += "s";

//                         offsetX = GameDefine.TEXTURE_WIDTH_HALF - 5;
//                         offsetY = GameDefine.TEXTURE_HEIGHT_HALF;
                    }
                }
                break;
            case GameSkillBattleType.UIMap:
                {
                    path = "Prefab/Sprite/man" + GameDefine.getString3( unit.Sprite ) + "/" +
GameDefine.getString3( unit.Sprite );
                    path += ( skill.BattleSprite > 0 ? "-" + skill.BattleSprite : "" );
                    path += "sm";
                }
                break;
        }

        clear();

        GameObject obj1 = Resources.Load<GameObject>( path );

        if ( obj1 == null )
        {
            onAttackOver();
            return;
        }

        GameObject obj = Instantiate<GameObject>( obj1 );
        GameAnimation attackerAnimation = obj.GetComponent<GameAnimation>();
        attackerAnimation.offsetX = offsetX;
        attackerAnimation.offsetY = offsetY;

        Transform trans = obj.transform;
        trans.SetParent( transform );
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;

        GameBattleSceneMovement.instance.updatePositionToCamera();

        transform.localPosition = new Vector3( GameBattleSceneMovement.instance.PosXReal ,
            GameBattleSceneMovement.instance.PosYReal + GameBattleManager.instance.LayerHeight ,
            transform.localPosition.z );

        attackerAnimation.transform.localScale = new Vector3( GameCameraManager.instance.sceneWidth / GameDefine.SCENE_WIDTH , 1.0f , 1.0f );

        if ( direction != GameBattleAttackMapDirection.Count )
        {
            int c = attackerAnimation.safHead.count3[ 0 ];
            int f = c * (int)direction;

            attackerAnimation.playAnimation( f , f + c , false , onAttackOver );
        }
        else
        {
            attackerAnimation.playAnimation( 0 , GameDefine.INVALID_ID , false , onAttackOver );
        }
    }

    void onStartOver()
    {
//         if ( skill.BattleType == GameSkillBattleType.Map )
//         {
//             GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell ,
//                 unit.PosY - GameCameraManager.instance.yCell , 50 , 50 , onStartMoveOver );
//         }
//         else
//         {
            GameBattleSceneMovement.instance.moveTo( GameBattleCursor.instance.PosX - GameCameraManager.instance.xCell ,
    GameBattleCursor.instance.PosY - GameCameraManager.instance.yCell , 50 , 50 , onStartMoveOver );
//        }
    }

    void onMoveOver()
    {
        clearJumpHP();
        clear();

        jumpHPlist.Clear();

        if ( onEventOver != null )
        {
            onEventOver();
        }

        onEventOver = null;
        skill = null;
        item = null;
        unit = null;
        result = null;
        direction = GameBattleAttackMapDirection.Count;
    }

    void onJumpHPOver()
    {
        onMoveOver();

//         GameBattleCursor.instance.moveTo( unit.PosX , unit.PosY );
// 
//         GameBattleSceneMovement.instance.moveTo( unit.PosX - GameCameraManager.instance.xCell ,
//             unit.PosY - GameCameraManager.instance.yCell , 50 , 50 , onMoveOver );
    }

    void onResOver()
    {
        clear();

        for ( int i = 0 ; i < result.Count ; i++ )
        {
            if ( i == 0 )
            {
                jumpHPlist[ i ].jump( result[ i ].HP , result[ i ].MP , result[ i ].type , result[ i ].other , onJumpHPOver );
            }
            else
            {
                jumpHPlist[ i ].jump( result[ i ].HP , result[ i ].MP , result[ i ].type , result[ i ].other , null );
            }
        }


    }

    void onResOverItem()
    {
        clear();

        for ( int i = 0 ; i < resultItem.Count ; i++ )
        {
            if ( i == 0 )
            {
                jumpHPlist[ i ].jump( resultItem[ i ].HP , resultItem[ i ].MP , resultItem[ i ].type , onJumpHPOver );
            }
            else
            {
                jumpHPlist[ i ].jump( resultItem[ i ].HP , resultItem[ i ].MP , resultItem[ i ].type , null );
            }
        }
    }

    void onAttackOver()
    {
        clear();

        if ( skill != null &&
            ( skill.BattleType == GameSkillBattleType.Map ||
           skill.BattleType == GameSkillBattleType.Normal ) )
        {
            unit.playAnimationBattle( GameAnimationType.Stand , unit.Direction , null );
        }

        bool b = false;

        for ( int i = 0 ; i < result.Count ; i++ )
        {
            string path = "Prefab/Misc/MAN_RES";

            GameBattleUnit u = result[ i ].unit;

            if ( u == null )
            {
                onMoveOver();
                return;
            }

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            GameAnimation gameAnimation = obj.GetComponent<GameAnimation>();
            gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
            gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;

            Transform trans = obj.transform;
            trans.SetParent( transform );
            trans.localScale = Vector3.one;
            trans.position = new Vector3( u.PosBattleX ,
                u.PosBattleY + GameBattleManager.instance.LayerHeight ,
                transform.localPosition.z );



            string path1 = "Prefab/JumpHPMap";

            GameObject obj1 = Instantiate<GameObject>( Resources.Load<GameObject>( path1 ) );
            GameBattleJumpHPMap jumpHP = obj1.GetComponent<GameBattleJumpHPMap>();

            Vector3 screenPos = GameCameraManager.instance.mainCamera.WorldToScreenPoint( trans.position );
            screenPos.z = 0;

            RectTransform trans1 = obj1.GetComponent<RectTransform>();
            trans1.SetParent( GameObject.Find( "Canvas/UI/Center/JumpHPMap").transform );
            trans1.localScale = Vector3.one;
            trans1.position = screenPos;
            trans1.anchoredPosition = new Vector2( GameDefine.TEXTURE_WIDTH_HALF + trans1.anchoredPosition.x ,
                trans1.anchoredPosition.y - GameDefine.TEXTURE_HEIGHT );

            jumpHPlist.Add( jumpHP );

            int animationStart = 2;
            int animationEnd = 10;

            switch ( result[ i ].type )
            {
                case GameSkillResutlType.Damage:
                    animationStart = 2;
                    animationEnd = 10;
                    break;
                case GameSkillResutlType.Cure:
                    animationStart = 46;
                    animationEnd = 60;
                    break;
                case GameSkillResutlType.Purge:
                    animationStart = 60;
                    animationEnd = 81;
                    break;
                case GameSkillResutlType.Blessing:
                    animationStart = 18;
                    animationEnd = 32;
                    break;
                case GameSkillResutlType.Curse:
                    animationStart = 32;
                    animationEnd = 46;
                    break;
                case GameSkillResutlType.None:
                case GameSkillResutlType.Special:
                    {
                        animationStart = 81;
                        animationEnd = 99;

                        if ( result[ i ].HP > 0 || result[ i ].MP > 0 )
                        {
                            if ( skill != null && 
                                ( skill.OtherEffect == GameSkillOtherEffect.HealAll ||
                                skill.OtherEffect == GameSkillOtherEffect.MPAdd ) )
                            {
                                animationStart = 46;
                                animationEnd = 60;
                            }
                            else
                            {
                                animationStart = 2;
                                animationEnd = 10;
                            }
                        }
                    }
                    break;
            }

            OnEventOver over = null;

            if ( result[ i ].type == GameSkillResutlType.Damage || result[ i ].HP != 0 || result[ i ].MP != 0 )
            {
                over = onResOver;
            }
            else
            {
                over = onJumpHPOver;
            }

            b = true;

            gameAnimation.playSound = ( i == 0 );

            if ( i == 0 )
            {
                gameAnimation.playAnimation( animationStart , animationEnd , false , over );
            }
            else
            {
                gameAnimation.playAnimation( animationStart , animationEnd , false , null );
            }
        }

        if ( !b )
        {
            onJumpHPOver();
        }
    }



    void onAttackOverItem()
    {
        clearJumpHP();

        for ( int i = 0 ; i < resultItem.Count ; i++ )
        {
            string path = "Prefab/Misc/MAN_RES";

            GameBattleUnit u = resultItem[ i ].unit;

            if ( u == null )
            {
                onMoveOver();
                return;
            }

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
            GameAnimation gameAnimation = obj.GetComponent<GameAnimation>();
            gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
            gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;

            Transform trans = obj.transform;
            trans.SetParent( transform );
            trans.localScale = Vector3.one;
            trans.position = new Vector3( u.PosBattleX ,
                u.PosBattleY + GameBattleManager.instance.LayerHeight ,
                transform.localPosition.z );



            string path1 = "Prefab/JumpHPMap";

            GameObject obj1 = Instantiate<GameObject>( Resources.Load<GameObject>( path1 ) );
            GameBattleJumpHPMap jumpHP = obj1.GetComponent<GameBattleJumpHPMap>();

            Vector3 screenPos = GameCameraManager.instance.mainCamera.WorldToScreenPoint( trans.position );
            screenPos.z = 0;

            RectTransform trans1 = obj1.GetComponent<RectTransform>();
            trans1.SetParent( GameObject.Find( "Canvas/UI/Center/JumpHPMap" ).transform );
            trans1.localScale = Vector3.one;
            trans1.position = screenPos;
            trans1.anchoredPosition = new Vector2( GameDefine.TEXTURE_WIDTH_HALF + trans1.anchoredPosition.x ,
                trans1.anchoredPosition.y - GameDefine.TEXTURE_HEIGHT );

            jumpHPlist.Add( jumpHP );

            int animationStart = 2;
            int animationEnd = 10;

            OnEventOver over = null;

            switch ( resultItem[ i ].type )
            {
                case GameItemUseType.Cure:
                    animationStart = 46;
                    animationEnd = 60;
                    over = onResOverItem;
                    break;
                case GameItemUseType.Recovery:
                    animationStart = 60;
                    animationEnd = 81;
                    over = onJumpHPOver;
                    break;
                case GameItemUseType.Limit:
                    animationStart = 46;
                    animationEnd = 60;
                    over = onJumpHPOver;
                    break;
            }

            if ( i == 0 )
            {
                gameAnimation.playSound = true;
                gameAnimation.playAnimation( animationStart , animationEnd , false , over );
            }
            else
            {
                gameAnimation.playSound = false;
                gameAnimation.playAnimation( animationStart , animationEnd , false , null );
            }
        }


    }

}

