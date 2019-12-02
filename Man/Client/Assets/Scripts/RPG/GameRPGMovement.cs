using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameRPGMovement : MonoBehaviour
{
    int originalPoxX;
    int originalPoxY;

    GameAnimationDirection originalDirection;

    int posX;
    int posY;

    float posBattleX;
    float posBattleY;

    int moveToX;
    int moveToY;

    int moveToBattleX;
    int moveToBattleY;

    float moveSpeed;

    GameAnimation gameAnimation;

    public bool isMoving = false;

    bool startMoving = false;
    bool playStartAnimation = false;
    bool sceneFollow = false;

    public bool nextMove = false;

    OnEventOver onEventOver;


    List<short> moveBuffer = new List<short>();

    public int PosX { get { return posX; } }
    public int PosY { get { return posY; } }

    public float PosBattleX { get { return posBattleX; } }
    public float PosBattleY { get { return posBattleY; } }


    void Awake()
    {
        gameAnimation = GetComponent<GameAnimation>();
        gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
        gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;

        //        canvas = gameObject.AddComponent<Canvas>();
    }


    public void updatePosition()
    {
        float z = GameRPGManager.instance.getZ( posX , posY );

        transform.localPosition = new Vector3( posBattleX ,
    posBattleY + GameRPGManager.instance.LayerHeight ,
    z );
    }
    
    public bool moveToDirection( int x , int y , bool start , bool follow , int speed , OnEventOver over )
    {
        if ( x == posX && y == posY )
        {
            return false;
        }

        if ( startMoving )
        {
            return false;
        }

        moveSpeed = GameDefine.getMoveSpeed( speed );

        sceneFollow = follow;
        playStartAnimation = start;
        onEventOver = over;

        moveBuffer.Clear();

        moveToX = x;
        moveToY = y;

        moveToBattleX = GameDefine.getBattleX( x );
        moveToBattleY = GameDefine.getBattleY( y );

        startAnimation();

        startMoving = true;

        return true;
    }


    public bool moveToEvent( int x , int y , bool start , bool follow , int speed , OnEventOver over )
    {
        if ( x == posX && y == posY )
        {
            return false;
        }

        if ( startMoving )
        {
            return false;
        }

        moveBuffer.Clear();

        int count = GameRPGPathFinder.instance.findPath( posX , posY , x , y , false , false );

        if ( count == 0 )
        {
            return false;
        }

        for ( int i = count - 3 ; i >= 0 ; i-- )
        {
            moveBuffer.Add( GameRPGPathFinder.instance.pathResult[ i ] );
        }

        moveSpeed = GameDefine.getMoveSpeed( speed );

        sceneFollow = follow;
        playStartAnimation = start;
        onEventOver = over;

        moveNext();

        startAnimation();

        startMoving = true;

        return true;
    }


    public void stopMove()
    {
        startMoving = false;
        isMoving = false;
        nextMove = false;

        gameAnimation.playAnimationRPG( GameAnimationType.Stand ,
                            gameAnimation.animationDirection , null );
    }
    
    public int moveTo( int x , int y , bool start , bool follow , bool check , int speed , OnEventOver over )
    {
        if ( x == posX && y == posY )
        {
            return GameDefine.INVALID_ID;
        }

        if ( startMoving )
        {
            return GameDefine.INVALID_ID;
        }

        moveBuffer.Clear();

        int count = GameRPGPathFinder.instance.findPath( posX , posY , x , y , check );

        if ( count == 0 )
        {
            return GameDefine.INVALID_ID;
        }

        for ( int i = count - 3 ; i >= 0 ; i-- )
        {
            moveBuffer.Add( GameRPGPathFinder.instance.pathResult[ i ] );
        }
        
        moveSpeed = GameDefine.getMoveSpeed( speed );

        sceneFollow = follow;
        playStartAnimation = start;
        onEventOver = over;

        moveNext();

        startAnimation();

        startMoving = true;

        return 0;
    }

    public void moveNext()
    {
        moveToX = moveBuffer[ 1 ];
        moveToY = moveBuffer[ 0 ];

        moveToBattleX = GameDefine.getBattleX( moveBuffer[ 1 ] );
        moveToBattleY = GameDefine.getBattleY( moveBuffer[ 0 ] );

        moveBuffer.RemoveRange( 0 , 2 );
    }

    void startAnimation()
    {
//        gameAnimation.stopAnimation();

        if ( playStartAnimation )
        {
            if ( moveToX != posX )
            {
                if ( moveToX > posX )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.MoveStart ,
                        GameAnimationDirection.East , onStartMove );
                }
                else
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.MoveStart ,
                        GameAnimationDirection.West , onStartMove );
                }
            }
            else
            {
                if ( moveToY > posY )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.MoveStart ,
                        GameAnimationDirection.South , onStartMove );
                }
                else
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.MoveStart ,
                        GameAnimationDirection.North , onStartMove );
                }
            }
        }
        else
        {
            isMoving = true;
        }
    }

    void onStartMove()
    {
        isMoving = true;

        gameAnimation.playAnimationRPG( GameAnimationType.Move ,
                        gameAnimation.animationDirection , null );
    }

    void onMoveOver()
    {
//         Debug.Log( "onMoveOver" );

        startMoving = false;

        if ( !nextMove )
        {
            gameAnimation.playAnimationRPG( GameAnimationType.Stand ,
                            gameAnimation.animationDirection , null );
        }

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public void setOriginalDirection()
    {
        originalPoxX = posX;
        originalPoxY = posY;

        originalDirection = gameAnimation.animationDirection;
    }

    public void originalPosition()
    {
        if ( originalPoxX == 0 && originalPoxY == 0 )
        {
            return;
        }

        setPos( originalPoxX , originalPoxY );

        gameAnimation.playAnimationRPG( GameAnimationType.Stand , originalDirection , null );

        originalDirection = GameAnimationDirection.Count;
        originalPoxX = 0;
        originalPoxY = 0;
    }

    public void setDirection( GameAnimationDirection d )
    {
        gameAnimation.playAnimationRPG( GameAnimationType.Stand , d , null );
    }

    public void stopAnimation()
    {
        gameAnimation.stopAnimation();
    }

    public void setColor( Color c )
    {
        gameAnimation.setColor( c );
    }

    public void clearOriginalPosition()
    {
        originalDirection = GameAnimationDirection.Count;
        originalPoxX = 0;
        originalPoxY = 0;
    }

    public void setPos( int x , int y )
    {
        posX = x;
        posY = y;

        moveToBattlePos();
    }

    public void moveToBattlePos()
    {
        posBattleX = GameDefine.getBattleX( posX );
        posBattleY = GameDefine.getBattleY( posY );

        updatePosition();
    }

    public void moveUpdate()
    {
        if ( !isMoving )
        {
            return;
        }

        float dis = Time.deltaTime * moveSpeed;

        if ( posX != moveToX ||
            posBattleX != moveToBattleX )
        {
            if ( moveToX > posX )
            {
                posBattleX += dis;

                if ( posBattleX >= moveToBattleX )
                {
                    posX = moveToX;
                    posBattleX = moveToBattleX;
                }
                else
                {
                    posX = GameDefine.getXFormBattle( (int)posBattleX );
                }

                updatePosition();

                if ( sceneFollow )
                    GameRPGSceneMovement.instance.followPos( posBattleX , posBattleY );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.East )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.Move ,
                        GameAnimationDirection.East , null );
                }
            }
            else
            {
                posBattleX -= dis;

                if ( posBattleX <= moveToBattleX )
                {
                    posX = moveToX;
                    posBattleX = moveToBattleX;
                }
                else
                {
                    posX = GameDefine.getXFormBattle( (int)posBattleX );
                }

                updatePosition();

                if ( sceneFollow )
                    GameRPGSceneMovement.instance.followPos( posBattleX , posBattleY );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.West )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.Move ,
                        GameAnimationDirection.West , null );
                }
            }
        }
        else if ( posY != moveToY ||
            posBattleY != moveToBattleY )
        {
            if ( moveToY > posY )
            {
                posBattleY -= dis;

                if ( posBattleY <= moveToBattleY )
                {
                    posY = moveToY;
                    posBattleY = moveToBattleY;
                }
                else
                {
                    posY = GameDefine.getYFormBattle( (int)posBattleY );
                }

                updatePosition();

                if ( sceneFollow )
                    GameRPGSceneMovement.instance.followPos( posBattleX , posBattleY );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.South )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.Move ,
                        GameAnimationDirection.South , null );
                }
            }
            else
            {
                posBattleY += dis;

                if ( posBattleY >= moveToBattleY )
                {
                    posY = moveToY;
                    posBattleY = moveToBattleY;
                }
                else
                {
                    posY = GameDefine.getYFormBattle( (int)posBattleY );
                }

                updatePosition();

                if ( sceneFollow )
                    GameRPGSceneMovement.instance.followPos( posBattleX , posBattleY );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.North )
                {
                    gameAnimation.playAnimationRPG( GameAnimationType.Move ,
                        GameAnimationDirection.North , null );
                }
            }

        }
        else
        {
            // move end

            updatePosition();

            if ( moveBuffer.Count > 0 )
            {
                moveNext();
                return;
            }

            isMoving = false;

            if ( playStartAnimation )
            {
                gameAnimation.playAnimationRPG( GameAnimationType.MoveEnd ,
            gameAnimation.animationDirection , onMoveOver );

            }
            else
            {
                onMoveOver();
            }
        }

    }

}

