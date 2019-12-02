using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleMovement : MonoBehaviour
{
    int originalPoxX;
    int originalPoxY;

    GameAnimationDirection originalDirection;

    [SerializeField]
    int posX;
    [SerializeField]
    int posY;

    [SerializeField]
    float posBattleX;
    [SerializeField]
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
    bool canFollow = false;

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
    }

    public void stopMove()
    {
        if ( isMoving )
        {
            posX = moveToX;
            posBattleY = moveToBattleX;
            posY = moveToY;
            posBattleY = moveToBattleY;
        }

        isMoving = false;
        startMoving = false;

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public void updatePosition()
    {
        float z = GameBattleManager.instance.getZ( posX , posY );

        transform.localPosition = new Vector3( posBattleX ,
    posBattleY + GameBattleManager.instance.LayerHeight ,
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
        canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                GameBattleManager.instance.LayerHeight );


        moveBuffer.Clear();

        moveToX = x;
        moveToY = y;

        moveToBattleX = GameDefine.getBattleX( x );
        moveToBattleY = GameDefine.getBattleY( y );

        startAnimation();

        startMoving = true;

        return true;
    }


    public bool moveToEvent( int x , int y , GameUnitMove unitMove , bool start , bool follow , int speed , OnEventOver over )
    {
        if ( x == posX && y == posY )
        {
            return false;
        }

        if ( startMoving )
        {
            return false;
        }

        int count = GameBattlePathFinder.instance.findPath( posX , posY , x , y ,
            unitMove.block , unitMove.fly , GameUnitCampType.Count );

        if ( count == 0 )
        {
            // try fly 

            count = GameBattlePathFinder.instance.findPath( posX , posY , x , y ,
                GameBattlePathFinder.BLOCK_EVENT , true , GameUnitCampType.Count );
        }

        if ( count == 0 )
        {
            return false;
        }

        for ( int i = count - 3 ; i >= 0 ; i-- )
        {
            moveBuffer.Add( GameBattlePathFinder.instance.pathResult[ i ] );
        }

        moveSpeed = GameDefine.getMoveSpeed( speed );

        sceneFollow = follow;
        playStartAnimation = start;
        onEventOver = over;
        canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );

        moveNext();

        startAnimation();

        startMoving = true;
        
        return true;
    }

    public int moveTo( int x , int y , byte cb , bool fly , GameUnitCampType camp , bool start , bool follow , int speed , OnEventOver over )
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

        int count = GameBattlePathFinder.instance.findPath( posX , posY , x , y , cb , fly , camp );

        if ( count == 0 )
        {
            return GameDefine.INVALID_ID;
        }

        for ( int i = count - 3 ; i >= 0 ; i-- )
        {
            moveBuffer.Add( GameBattlePathFinder.instance.pathResult[ i ] );
        }

        int cost = 0;

        for ( int i = 0 ; i < moveBuffer.Count / 2 ; i++ )
        {
            int xx = moveBuffer[ i * 2 + 1 ];
            int yy = moveBuffer[ i * 2 ];

            cost += GameBattleUnitMovement.instance.getCellCost( xx , yy );
        }

        moveSpeed = GameDefine.getMoveSpeed( speed );

        sceneFollow = follow;
        playStartAnimation = start;
        onEventOver = over;
        canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                GameBattleManager.instance.LayerHeight );


        moveNext();

        startAnimation();

        startMoving = true;

        return cost;
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
        gameAnimation.stopAnimation();

        if ( playStartAnimation )
        {
            if ( moveToX != posX )
            {
                if ( moveToX > posX )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.MoveStart ,
                        GameAnimationDirection.East , onStartMove );
                }
                else
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.MoveStart ,
                        GameAnimationDirection.West , onStartMove );
                }
            }
            else
            {
                if ( moveToY > posY )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.MoveStart ,
                        GameAnimationDirection.South , onStartMove );
                }
                else
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.MoveStart ,
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

        gameAnimation.playAnimationBattle( GameAnimationType.Move ,
                        gameAnimation.animationDirection , null );
    }

    void onMoveOver()
    {
//        Debug.Log( "onMoveOver" );

        startMoving = false;

        gameAnimation.playAnimationBattle( GameAnimationType.Stand ,
                        gameAnimation.animationDirection , null );

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

        gameAnimation.playAnimationBattle( GameAnimationType.Stand , originalDirection , null );

        originalDirection = GameAnimationDirection.Count;
        originalPoxX = 0;
        originalPoxY = 0;
    }

    public void setDirection( GameAnimationDirection d )
    {
        gameAnimation.playAnimationBattle( GameAnimationType.Stand , d , null );
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

    void Update()
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

                if ( !canFollow )
                {
                    canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );
                }

                if ( sceneFollow && canFollow )
                    GameCameraManager.instance.followPos( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.East )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.Move ,
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

                if ( !canFollow )
                {
                    canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );
                }

                if ( sceneFollow && canFollow )
                    GameCameraManager.instance.followPos( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.West )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.Move ,
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

                if ( !canFollow )
                {
                    canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );
                }

                if ( sceneFollow && canFollow )
                    GameCameraManager.instance.followPos( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.South )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.Move ,
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

                if ( !canFollow )
                {
                    canFollow = GameCameraManager.instance.canFollow( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );
                }

                if ( sceneFollow && canFollow )
                    GameCameraManager.instance.followPos( posBattleX , posBattleY ,
                        GameBattleManager.instance.LayerHeight );

                if ( gameAnimation.animationType != GameAnimationType.Move ||
                    gameAnimation.animationDirection != GameAnimationDirection.North )
                {
                    gameAnimation.playAnimationBattle( GameAnimationType.Move ,
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
                gameAnimation.playAnimationBattle( GameAnimationType.MoveEnd ,
            gameAnimation.animationDirection , onMoveOver );

            }
            else
            {
                onMoveOver();
            }

        }

    }

}
