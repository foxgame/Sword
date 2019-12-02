using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSceneMovement : Singleton<GameBattleSceneMovement>
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

    float moveToXReal;
    float moveToYReal;

    int posXSpeed;
    int posYSpeed;

    bool isMoving = false;

    Transform trans;
    GameObject obj;

    public int PosX { get { return posX; } }
    public int PosY { get { return posY; } }

    public float PosXReal { get { return posXReal; } }
    public float PosYReal { get { return posYReal; } }

//    bool canFollow = false;
    bool moveEvent = false;

    OnEventOver onEventOver;

    public float AnimationPosX
    {
        get
        {
            return posX * GameDefine.TEXTURE_WIDTH -
                GameCameraManager.instance.AnimationOffsetX;
        }
    }

    public float AnimationPosY
    {
        get
        {
            return -posY * GameDefine.TEXTURE_HEIGHT +
                GameCameraManager.instance.AnimationOffsetY;
        }
    }


    public override void initSingleton()
    {
        obj = GameObject.Find( "BattleScene" );
        trans = GameObject.Find( "MainCamera" ).transform;
    }

    public void updatePosition()
    {
        trans.position = new Vector3( posXReal + GameCameraManager.instance.sceneWidthHalf , 
            posYReal - GameCameraManager.instance.sceneHeightHalf + GameBattleManager.instance.LayerHeight , 
            -10.0f );

        if ( moveEvent )
        {
            GameCameraManager.instance.followPosEvent( posXReal , posYReal ,
                    GameBattleManager.instance.LayerHeight );
        }
        else
        {
            GameCameraManager.instance.followPos( posXReal , posYReal ,
                    GameBattleManager.instance.LayerHeight );
        }
    }


    public void moveToEvent( int x , int y )
    {
        posX = x;
        posY = y;

        posXReal = posX * GameDefine.TEXTURE_WIDTH;
        posYReal = -posY * GameDefine.TEXTURE_HEIGHT;

        posXReal -= GameCameraManager.instance.xOffsetCell;
        posYReal += GameCameraManager.instance.yOffsetCell;

        moveEvent = true;

        updatePosition();

        if ( !GameBattleCursor.instance.IsShow )
        {
            GameBattleCursor.instance.moveTo( posX + GameCameraManager.instance.xCell ,
             posY + +GameCameraManager.instance.yCell , false );
        }
    }

    public void updatePositionToCamera()
    {
        posXReal = GameCameraManager.instance.PosXReal;
        posYReal = GameCameraManager.instance.PosYReal - GameBattleManager.instance.LayerHeight;

        posX = (int)( posXReal / GameDefine.TEXTURE_WIDTH );
        posY = (int)( -posYReal / GameDefine.TEXTURE_HEIGHT );

        if ( (int)posXReal % GameDefine.TEXTURE_WIDTH > 0 )
            posX++;
        if ( (int)-posYReal % GameDefine.TEXTURE_HEIGHT > 0 )
            posY++;
    }

    public void moveToEvent( int x , int y , int sx , int sy , OnEventOver over )
    {
#if UNITY_EDITOR
        Debug.Log( "GameBattleSceneMovement moveToEvent0 " + x + " " + y + " " + posX + " " + posY + " " + posXReal + " " + posYReal + " " + moveToXReal + " " + moveToYReal );
#endif
        posXReal = GameCameraManager.instance.PosXReal;
        posYReal = GameCameraManager.instance.PosYReal - GameBattleManager.instance.LayerHeight;

        posX = (int)( posXReal / GameDefine.TEXTURE_WIDTH );
        posY = (int)( -posYReal / GameDefine.TEXTURE_HEIGHT );

        if ( (int)posXReal % GameDefine.TEXTURE_WIDTH > 0 )
            posX++;
        if ( (int)-posYReal % GameDefine.TEXTURE_HEIGHT > 0 )
            posY++;

        moveToX = x;
        moveToY = y;

        moveToXReal = moveToX * GameDefine.TEXTURE_WIDTH;
        moveToYReal = -moveToY * GameDefine.TEXTURE_HEIGHT;

        moveToXReal -= GameCameraManager.instance.xOffsetCell;
        moveToYReal += GameCameraManager.instance.yOffsetCell;

        posXSpeed = GameDefine.getSpeed( sx ) + 20;
        posYSpeed = GameDefine.getSpeed( sy ) + 20;

        if ( posXReal == moveToXReal )
        {
            posX = moveToX;
        }
        if ( posYReal == moveToYReal )
        {
            posY = moveToY;
        }

        isMoving = true;
        moveEvent = true;

        onEventOver = over;

#if UNITY_EDITOR
        Debug.Log( "GameBattleSceneMovement moveToEvent1 " + x + " " + y + " " + posX + " " + posY + " " + posXReal + " " + posYReal + " " + moveToXReal + " " + moveToYReal );
#endif
    }


    public void moveTo( int x , int y , int sx , int sy , OnEventOver over )
    {
#if UNITY_EDITOR
        Debug.Log( "GameBattleSceneMovement moveTo0 " + x + " " + y + " " + posX + " " + posY + " " + posXReal + " " + posYReal + " " + moveToXReal  + " " + moveToYReal );
#endif
        posXReal = GameCameraManager.instance.PosXReal;
        posYReal = GameCameraManager.instance.PosYReal - GameBattleManager.instance.LayerHeight;

        posX = (int)( posXReal / GameDefine.TEXTURE_WIDTH );
        posY = (int)( -posYReal / GameDefine.TEXTURE_HEIGHT );

        if ( (int)posXReal % GameDefine.TEXTURE_WIDTH > 0 )
            posX++;
        if ( (int)-posYReal % GameDefine.TEXTURE_HEIGHT > 0 )
            posY++;

        moveToX = x;
        moveToY = y;

        moveToXReal = moveToX * GameDefine.TEXTURE_WIDTH + GameDefine.TEXTURE_WIDTH_HALF;
        moveToYReal = -moveToY * GameDefine.TEXTURE_HEIGHT - GameDefine.TEXTURE_HEIGHT_HALF;

        moveToXReal -= GameCameraManager.instance.xOffsetCell;
        moveToYReal += GameCameraManager.instance.yOffsetCell;

        posXSpeed = GameDefine.getSpeed( sx ) + 20;
        posYSpeed = GameDefine.getSpeed( sy ) + 20;

        if ( posXReal == moveToXReal )
        {
            posX = moveToX;
        }
        if ( posYReal == moveToYReal )
        {
            posY = moveToY;
        }

        isMoving = true;
        moveEvent = true;

        onEventOver = over;

#if UNITY_EDITOR
        Debug.Log( "GameBattleSceneMovement moveTo1 " + x + " " + y + " " + posX + " " + posY + " " + posXReal + " " + posYReal + " " + moveToXReal + " " + moveToYReal );
#endif
    }

    void Update()
    {
        if ( !isMoving )
        {
            return;
        }

        float disX = Time.deltaTime * posXSpeed;
        float disY = Time.deltaTime * posYSpeed;

        if ( posXReal != moveToXReal )
        {
            if ( posXReal < moveToXReal )
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

        if ( posYReal != moveToYReal )
        {
            if ( posYReal > moveToYReal )
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

            if ( !GameBattleCursor.instance.IsShow )
            {
                GameBattleCursor.instance.moveTo( posX + GameCameraManager.instance.xCell ,
                 posY + GameCameraManager.instance.yCell , false );
            }

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }

    }



}

