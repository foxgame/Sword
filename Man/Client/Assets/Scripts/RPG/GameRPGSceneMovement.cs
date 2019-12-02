using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameRPGSceneMovement : Singleton<GameRPGSceneMovement>
{
    [SerializeField]
    int posX;
    [SerializeField]
    int posY;

    [SerializeField]
    float posXReal;
    [SerializeField]
    float posYReal;

    int moveToX;
    int moveToY;

    int moveToXReal;
    int moveToYReal;

    int posXSpeed;
    int posYSpeed;

    bool isMoving = false;

    Transform trans;
    GameObject obj;

    OnEventOver onEventOver;

    public int PosX { get { return posX; } }
    public int PosY { get { return posY; } }

    public float PosXReal { get { return posXReal; } }
    public float PosYReal { get { return posYReal; } }

    public override void initSingleton()
    {
        obj = GameObject.Find( "RPGScene" );
        trans = GameObject.Find( "MainCamera" ).transform;
    }

    public void updatePosition()
    {
        if ( posXReal < 0 )
        {
            posXReal = 0;
        }

        if ( posXReal > ( GameRPGManager.instance.Width ) * GameDefine.TEXTURE_WIDTH - GameCameraManager.instance.sceneWidth )
        {
            posXReal = ( GameRPGManager.instance.Width ) * GameDefine.TEXTURE_WIDTH - GameCameraManager.instance.sceneWidth;
        }

        if ( posYReal > 0 )
        {
            posYReal = 0;
        }

        if ( posYReal < -( GameRPGManager.instance.Height ) * GameDefine.TEXTURE_HEIGHT + GameCameraManager.instance.sceneHeight )
        {
            posYReal = -( GameRPGManager.instance.Height ) * GameDefine.TEXTURE_HEIGHT + GameCameraManager.instance.sceneHeight;
        }

        trans.position = new Vector3( posXReal + GameCameraManager.instance.sceneWidthHalf ,
   posYReal - GameCameraManager.instance.sceneHeightHalf + GameRPGManager.instance.LayerHeight ,
   -10.0f );

    }
    
    public void followPos( float x , float y )
    {
        Vector2 xy = new Vector2( x , -y );
        Rect rect = GameDefine.getRPGSceneRect( posXReal , posYReal );

        posXReal = x - GameCameraManager.instance.sceneWidthHalf + GameCameraManager.instance.xOffset;
        posYReal = y + GameCameraManager.instance.sceneHeightHalf;

        if ( rect.Contains( xy ) ||
            !GameRPGManager.instance.checkPosition( x , y ) )
        {

        }
        else
        {
//             if ( xy.x < rect.xMin )
//             {
//                 posXReal = x;
//             }
//             else if ( xy.x > rect.xMax )
//             {
//                 posXReal = GameDefine.getRPGSceneXMax( xy.x );
//             }
// 
//             if ( xy.y < rect.yMin )
//             {
//                 posYReal = GameDefine.getRPGSceneYMin( xy.y );
//             }
//             else if ( xy.y > rect.yMax )
//             {
//                 posYReal = GameDefine.getRPGSceneYMax( xy.y );
//             }
// 
//             posX = (int)( posXReal / GameDefine.TEXTURE_WIDTH );
//             posY = (int)( -posYReal / GameDefine.TEXTURE_HEIGHT );

        }

        updatePosition();

    }

    public void moveTo( int x , int y )
    {
        posX = x;
        posY = y;

        posXReal = posX * GameDefine.TEXTURE_WIDTH;
        posYReal = -posY * GameDefine.TEXTURE_HEIGHT;

        posXReal -= (int)GameCameraManager.instance.xOffset;

        updatePosition();
    }

    public void moveTo( int x , int y , int sx , int sy , OnEventOver over )
    {
        moveToX = x;
        moveToY = y;

        moveToXReal = moveToX * GameDefine.TEXTURE_WIDTH;
        moveToYReal = -moveToY * GameDefine.TEXTURE_HEIGHT;

        moveToXReal -= (int)GameCameraManager.instance.xOffset;

        posXSpeed = GameDefine.getSpeed( sx ) + 10;
        posYSpeed = GameDefine.getSpeed( sy ) + 10;

        isMoving = true;

        onEventOver = over;
    }

//     public void updatePos()
//     {
//         posX = (int)( ( posXReal - GameCameraManager.instance.sceneWidthHlaf ) / GameDefine.TEXTURE_WIDTH );
//         posY = (int)( ( posYReal + GameCameraManager.instance.sceneHeightHlaf ) / GameDefine.TEXTURE_HEIGHT );
// 
//         posXReal -= GameCameraManager.instance.sceneWidthHlaf;
//         posYReal += GameCameraManager.instance.sceneHeightHlaf;
//     }

    public void moveToEvent( int x , int y , int sx , int sy , OnEventOver over )
    {
        moveToX = x;
        moveToY = y;

        moveToXReal = moveToX * GameDefine.TEXTURE_WIDTH;
        moveToYReal = -moveToY * GameDefine.TEXTURE_HEIGHT;

        moveToXReal -= (int)GameCameraManager.instance.xOffset;

        posXSpeed = GameDefine.getSpeed( sx ) + 10;
        posYSpeed = GameDefine.getSpeed( sy ) + 10;

        isMoving = true;

        onEventOver = over;
    }

    void Update()
    {
        if ( !isMoving )
        {
            return;
        }

        float disX = Time.deltaTime * posXSpeed;
        float disY = Time.deltaTime * posYSpeed;

        if ( posX != moveToX )
        {
            if ( moveToX > posX )
            {
                posXReal += disX;

                if ( posXReal >= moveToXReal )
                {
                    posX = moveToX;
                    posXReal = moveToXReal;
                }
            }
            else
            {
                posXReal -= disX;

                if ( posXReal <= moveToXReal )
                {
                    posX = moveToX;
                    posXReal = moveToXReal;
                }
            }
        }

        if ( posY != moveToY )
        {
            if ( moveToY > posY )
            {
                posYReal -= disY;

                if ( posYReal <= moveToYReal )
                {
                    posY = moveToY;
                    posYReal = moveToYReal;
                }
            }
            else
            {
                posYReal += disY;

                if ( posYReal >= moveToYReal )
                {
                    posY = moveToY;
                    posYReal = moveToYReal;
                }
            }
        }

        updatePosition();

        if ( posX == moveToX &&
            posY == moveToY )
        {
            // move end

            isMoving = false;

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }

    }



}

