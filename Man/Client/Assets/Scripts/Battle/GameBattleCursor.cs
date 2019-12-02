using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleCursor : Singleton<GameBattleCursor>
{
    public const int SpeedX15 = GameDefine.TEXTURE_WIDTH * 15 + 15;
    public const int SpeedY15 = GameDefine.TEXTURE_HEIGHT * 15 + 15;

    public const int SpeedX = GameDefine.TEXTURE_WIDTH * 10 + 10;
    public const int SpeedY = GameDefine.TEXTURE_HEIGHT * 10 + 10;

    public const int SpeedX3 = GameDefine.TEXTURE_WIDTH * 8 + 7;
    public const int SpeedY3 = GameDefine.TEXTURE_HEIGHT * 8 + 7;

    public const int SpeedXHalf = GameDefine.TEXTURE_WIDTH * 5 + 5;
    public const int SpeedYHalf = GameDefine.TEXTURE_HEIGHT * 5 + 5;

    bool isShow = false;

    GameAnimation gameAnimation;

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
    bool isFollow = false;

    Transform trans;

    int size;

    OnEventOver onEventOver;

    float time;

    public int Size { get { return size; } }

    public bool IsShow { get { return isShow; } }
    public bool IsMoving { get { return isMoving; } }

    public int PosX { get { return posX; } }
    public int PosY { get { return posY; } }


    public override void initSingleton()
    {
        trans = transform;

        GameObject obj = Instantiate( Resources.Load<GameObject>( "Prefab/Misc/Cursor" ) );

        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.offsetX = -145;
        gameAnimation.offsetY = 88;

        obj.transform.SetParent( transform );
        obj.transform.localPosition = Vector3.zero;
    }


    public void updatePosition()
    {
        trans.localPosition = new Vector3( posXReal ,
    posYReal + GameBattleManager.instance.LayerHeight , trans.localPosition.z );

        if ( isFollow )
        {
            GameCameraManager.instance.followPos( posXReal , posYReal , GameBattleManager.instance.LayerHeight );
        }
    }

    public void moveTo( int x , int y , bool follow )
    {
        posX = x;
        posY = y;

        posXReal = GameDefine.getBattleX( x );
        posYReal = GameDefine.getBattleY( y );

        time = 0.0f;

        isFollow = follow;

        updatePosition();
    }


    public void moveTo( int x , int y , int sx , int sy , bool follow , OnEventOver over , bool delay = false )
    {
#if UNITY_EDITOR
        Debug.Log( "GameBattleCursor moveTo " + x + " " + y );
#endif

        moveToX = x;
        moveToY = y;

        moveToXReal = GameDefine.getBattleX( x );
        moveToYReal = GameDefine.getBattleY( y );

        posXSpeed = sx;
        posYSpeed = sy;

        isMoving = true;

        isFollow = follow;

        onEventOver = over;

        if ( delay && posX == x && posY == y )
        {
            time = -0.5f;
        }
        else
        {
            time = 0.0f;
        }
    }

    void Update()
    {
        if ( !isMoving )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time < 0 )
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
                else
                {
                    //posX = GameDefine.getXFormBattle( (int)posXReal );
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
                else
                {
                    //posX = GameDefine.getXFormBattle( (int)posXReal );
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
                else
                {
                    //posY = GameDefine.getYFormBattle( (int)posYReal );
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
                else
                {
                    //posY = GameDefine.getYFormBattle( (int)posYReal );
                }
            }

        }

        updatePosition();

        if ( posX == moveToX &&
            posY == moveToY )
        {
            // move end

            isMoving = false;

            time = 0.0f;

            if ( onEventOver != null )
            {
                onEventOver();
            }
        }

    }

    public void show( int s = 0 )
    {
        gameObject.SetActive( true );

        isShow = true;

        size = s;

        gameAnimation.playAnimation( s * 36 , s * 36 + 36 );
    }

    public void unShow()
    {
        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        isShow = false;

        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();

        gameObject.SetActive( false );
    }




}
