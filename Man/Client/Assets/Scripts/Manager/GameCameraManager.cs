using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameCameraManager : Singleton<GameCameraManager>
{
    public Camera mainCamera;

    public float xOffset;
    public float yOffset;

    public float xOffsetCell;
    public float yOffsetCell;

    public float xOffsetReal;
    public float yOffsetReal;

    public int animationOffsetX = 10;
    public int animationOffsetY = 4;

    public float sceneWidth;
    public float sceneHeight;

    public float sceneWidthHalf;
    public float sceneHeightHalf;

    public int xCell = 0;
    public int yCell = 0;

    // 200 pixels 
    public float minSize = 100;

    public int MapWidth { set; get; }
    public int MapHeight { set; get; }

    public float AnimationOffsetX
    {
        get
        {
            return animationOffsetX;
        }
    }
    public float AnimationOffsetY
    {
        get
        {
            return animationOffsetY;
        }
    }


    public override void initSingleton()
    {
        mainCamera = GetComponent<Camera>();

        float s = Screen.height / minSize;

        mainCamera.orthographicSize = minSize;

        // camera height is real size 200 pix
        sceneHeight = minSize * 2;
        sceneWidth = minSize * 2 * Screen.width / Screen.height;

        sceneWidthHalf = sceneWidth / 2.0f;
        sceneHeightHalf = sceneHeight / 2.0f;

        xCell = (int)( sceneWidthHalf / GameDefine.TEXTURE_WIDTH );
        yCell = (int)( sceneHeightHalf / GameDefine.TEXTURE_HEIGHT );

        xOffset = ( sceneWidth - GameDefine.SCENE_WIDTH ) / 2.0f;
        yOffset = ( sceneHeight - GameDefine.SCENE_HEIGHT ) / 2.0f;

        xOffsetReal = ( Screen.width - Screen.height / (float)GameDefine.SCENE_HEIGHT * GameDefine.SCENE_WIDTH ) / 2.0f;
        yOffsetReal = 0.0f;

        xOffsetCell = sceneWidth - (int)( sceneWidth / GameDefine.TEXTURE_WIDTH ) * GameDefine.TEXTURE_WIDTH;
        yOffsetCell = sceneHeight - (int)( sceneHeight / GameDefine.TEXTURE_HEIGHT ) * GameDefine.TEXTURE_HEIGHT;

        xOffsetCell /= 2.0f;
        yOffsetCell /= 2.0f;
    }

    public float PosXReal
    {
        get
        {
            return transform.localPosition.x - sceneWidthHalf;
        }
    }

    public float PosYReal
    {
        get
        {
            return transform.localPosition.y + sceneHeightHalf;
        }
    }

    public void setPos( float x , float y )
    {
        transform.position = new Vector3( x + sceneWidthHalf , y - sceneHeightHalf , -10.0f );

#if UNITY_EDITOR
//        Debug.Log(" transform.position " + transform.position.x);
#endif
    }

    public void setPos( int x , int y )
    {
        transform.position = new Vector3( x + sceneWidthHalf , y - sceneHeightHalf , -10.0f );

#if UNITY_EDITOR
//        Debug.Log(" transform.position " + transform.position.x);
#endif
    }

    public bool canFollow( float x , float y , float lh )
    {
        float rx = PosXReal;
        float ry = PosYReal - lh;

        Vector2 xy = new Vector2( x , -y );
        Rect rect = new Rect( rx + GameDefine.TEXTURE_WIDTH - 1 ,
            -ry + GameDefine.TEXTURE_HEIGHT - 1 ,
            sceneWidth - GameDefine.TEXTURE_WIDTH * 3 + 2 ,
            sceneHeight - GameDefine.TEXTURE_HEIGHT * 3 + 2 );

        return rect.Contains( xy );
    }

    public void followPos( float x , float y , float lh )
    {
        float rx = PosXReal;
        float ry = PosYReal - lh;

        Vector2 xy = new Vector2( x , -y );
        Rect rect = new Rect( rx + GameDefine.TEXTURE_WIDTH ,
            -ry + GameDefine.TEXTURE_HEIGHT ,
            sceneWidth - GameDefine.TEXTURE_WIDTH * 3 ,
            sceneHeight - GameDefine.TEXTURE_HEIGHT * 3 );

        if ( !rect.Contains( xy ) )
        {
            if ( xy.x < rect.xMin )
            {
                rx = xy.x - GameDefine.TEXTURE_WIDTH;
            }
            else if ( xy.x > rect.xMax )
            {
                rx = xy.x - sceneWidth + GameDefine.TEXTURE_WIDTH * 2;
            }

            if ( xy.y < rect.yMin )
            {
                ry = -( xy.y - GameDefine.TEXTURE_HEIGHT );
            }
            else if ( xy.y > rect.yMax )
            {
                ry = -( xy.y - sceneHeight + GameDefine.TEXTURE_HEIGHT * 2 );
            }

            if ( rx < 0 )
            {
                rx = 0;
            }

            if ( rx > MapWidth - sceneWidth )
            {
                rx = MapWidth - sceneWidth;
            }
            if ( ry > 0 )
            {
                ry = 0;
            }
            if ( -ry > MapHeight - sceneHeight )
            {
                ry = -( MapHeight - sceneHeight );
            }

            if ( Mathf.Abs( rx + sceneWidthHalf - transform.position.x ) > 30.0f ||
                Mathf.Abs( ry - sceneHeightHalf + lh - transform.position.y ) > 30.0f )
            {
#if UNITY_EDITOR
                Debug.LogError( " sfsf " + Mathf.Abs( rx + sceneWidthHalf - transform.position.x )  + " " + Mathf.Abs( ry - sceneHeightHalf + lh - transform.position.y ) );
#endif
            }

            transform.position = new Vector3( rx + sceneWidthHalf ,
                ry - sceneHeightHalf + lh , 
                -10.0f );

#if UNITY_EDITOR
//            Debug.Log(" transform.position " + transform.position.x );
#endif
        }
    }


    public void followPosEvent( float x , float y , float lh )
    {
        float rx = PosXReal;
        float ry = PosYReal - lh;

        Vector2 xy = new Vector2( x , -y );
        Rect rect = new Rect( rx ,
            -ry ,
            sceneWidth - GameDefine.TEXTURE_WIDTH * 2 ,
            sceneHeight - GameDefine.TEXTURE_HEIGHT * 2 );

        if ( rx < 0 )
        {
            rx = 0;
        }

        if ( rx > MapWidth - sceneWidth )
        {
            rx = MapWidth - sceneWidth;
        }
        if ( ry > 0 )
        {
            ry = 0;
        }
        if ( -ry > MapHeight - sceneHeight )
        {
            ry = -( MapHeight - sceneHeight );
        }

        transform.position = new Vector3( rx + sceneWidthHalf ,
            ry - sceneHeightHalf + lh ,
            -10.0f );

#if UNITY_EDITOR
//        Debug.Log(" transform.position " + transform.position.x);
#endif
    }

//     int battleType;
//     int batttleSide = 1;
//     string batttleHP = "30";
//     string batttleHP2 = "15";
//     string batttleLeft = "17";

//     string batttleRight = "4";
//     string battleSkill = "54";

//     string batttleRight = "4";
//     string battleSkill = "53";
//     string battleDirection = "4";

//     string batttleRight = "0";
//     string battleSkill = "46";

    void OnGUI()
    {
//         string[] str = { "left" , "right" };
//         batttleSide = GUI.Toolbar( new Rect( 50 , 10 , str.Length * 70 , 30 ) , batttleSide , str );
// 
//         string[] str1 = { "normal" , "critical" , "double" , "block" };
//         battleType = GUI.Toolbar( new Rect( 50 , 50 , str1.Length * 70 , 30 ) , battleType , str1 );
// 
//         batttleHP = GUI.TextField( new Rect( 50 , 90 , 100 , 20 ) , batttleHP );
//         batttleHP2 = GUI.TextField( new Rect( 200 , 90 , 100 , 20 ) , batttleHP2 );
// 
//         batttleRight = GUI.TextField( new Rect( 50 , 120 , 100 , 20 ) , batttleRight );
//         batttleLeft = GUI.TextField( new Rect( 200 , 120 , 100 , 20 ) , batttleLeft );
// 
//         battleDirection = GUI.TextField( new Rect( 50 , 150 , 100 , 20 ) , battleDirection );
//         battleSkill = GUI.TextField( new Rect( 200 , 150 , 100 , 20 ) , battleSkill );
// 
//         if ( GUI.Button( new Rect( 50 , 180 , 200 , 30 ) , "attack" ) )
//         {
//             GameBattleUnit right1 = new GameBattleUnit();
//             right1.init( GameUnitData.instance.getData( int.Parse( batttleRight ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[ 0 ] );
// 
//             GameBattleUnit left1 = new GameBattleUnit();
//             left1.init( GameUnitData.instance.getData( int.Parse( batttleLeft ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[ 0 ] );
// 
// 
//             GameBattleAttackResultPhysical r = new GameBattleAttackResultPhysical();
//             r.HP = int.Parse( batttleHP );
//             r.HP2 = int.Parse( batttleHP2 );
//             r.type = (GameBattleAttackResultPhysical.Type)( battleType );
// 
//             GameBattleAttackUI.instance.show( 1 , left1 , right1 , (GameBattleAttackResultSide)(batttleSide) , r , true , null );
//         }
// 
//         if ( GUI.Button( new Rect( 350 , 180 , 200 , 30 ) , "unShow attack" ) )
//         {
//             GameBattleAttackUI.instance.unShow();
//         }
// 
//         if ( GUI.Button( new Rect( 50 , 220 , 200 , 30 ) , "maigc" ) )
//         {
//             GameBattleUnit right1 = new GameBattleUnit();
//             right1.init( GameUnitData.instance.getData( int.Parse( batttleRight ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[ 0 ] );
// 
//             GameBattleUnit left1 = new GameBattleUnit();
//             left1.init( GameUnitData.instance.getData( int.Parse( batttleLeft ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[ 0 ] );
// 
// 
//             GameBattleAttackResultPhysical r = new GameBattleAttackResultPhysical();
//             r.HP = int.Parse( batttleHP );
//             r.HP2 = int.Parse( batttleHP2 );
//             r.type = (GameBattleAttackResultPhysical.Type)( battleType );
// 
//             GameSkill m = GameSkillData.instance.getData( int.Parse( battleSkill ) );
// //            GameBattleAttackUI.instance.showSkill( 1 , m , (GameBattleAttackMapDirection)int.Parse(battleDirection), left1 , right1 , (GameBattleAttackResultSide)( batttleSide ) , r , null );
//         }
// 
//         if ( GUI.Button( new Rect( 350 , 220 , 200 , 30 ) , "maigc map" ) )
//         {
//             GameBattleUnit right1 = new GameBattleUnit();
//             right1.init( GameUnitData.instance.getData( int.Parse( batttleRight ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[0] );
// 
//             GameBattleUnit left1 = new GameBattleUnit();
//             left1.init( GameUnitData.instance.getData( int.Parse( batttleLeft ) ) ,
//                 GameBattleManager.instance.ActiveBattleStage.Man[ 0 ] );
// 
// 
//             GameBattleAttackResultPhysical r = new GameBattleAttackResultPhysical();
//             r.HP = int.Parse( batttleHP );
//             r.HP2 = int.Parse( batttleHP2 );
//             r.type = (GameBattleAttackResultPhysical.Type)( battleType );
// 
//             GameSkill m = GameSkillData.instance.getData( int.Parse( battleSkill ) );
// //             GameBattleAttackMap.instance.showSkill( m , right1 , (GameBattleAttackMapDirection)int.Parse( battleDirection ) , null );
//         }
// 
//         if ( GUI.Button( new Rect( 50 , 260 , 200 , 30 ) , "maigc selection" ) )
//         {
//             GameSkill m = GameSkillData.instance.getData( int.Parse( battleSkill ) );
// 
//             GameBattleUnitSkillSelection.instance.show( 16 , 13 , m , true );
//         }
// 
//         if ( GUI.Button( new Rect( 350 , 260 , 200 , 30 ) , "unshow maigc selection" ) )
//         {
//             GameBattleUnitSkillSelection.instance.unShow();
//         }




    }
}

