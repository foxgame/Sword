using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameRPGManager : Singleton<GameRPGManager>
{
    [SerializeField]
    int activeID = GameDefine.INVALID_ID;

    [SerializeField]
    GameRPGDTL activeDTL = null;
    [SerializeField]
    GameRPGStage activeRPGStage = null;

    [SerializeField]
    GameRPGInfo activeInfo = null;

    [SerializeField]
    int layerHeight = 0;

    Transform transEvent;
    Transform transBackground;
    Transform transUnit;
    Transform transCell;

    GameObject[] objsLayer1 = null;
    GameObject[] objsLayer0 = null;

    bool isVisibleLayer = true;

    GameRPGMovement movement = null;
    GameAnimation gameAnimation = null;

    public int LayerHeight { get { return layerHeight; } }

    public int ActiveID { get { return activeID; } }

    public GameRPGStage ActiveRPGStage { get { return activeRPGStage; } }

    public int Width { get{ return activeDTL.Width; } }
    public int Height { get { return activeDTL.Height; } }
    public bool IsVisibleLayer { get { return isVisibleLayer; } }

    public int PosX { get { return movement.PosX; } }
    public int PosY { get { return movement.PosY; } }

    Dictionary<string , GameAnimation> animations = new Dictionary<string , GameAnimation>();


    public override void initSingleton()
    {
        transBackground = GameObject.Find( "RPGScene/Background" ).transform;
        transUnit = GameObject.Find( "RPGScene/Unit" ).transform;
        transCell = GameObject.Find( "RPGScene/Cell" ).transform;
        transEvent = GameObject.Find( "RPGScene/Event" ).transform;
    }

    public void clear()
    {
        visibleLayer( true );

        clearStage();

        activeID = GameDefine.INVALID_ID;
        activeDTL = null;
        activeRPGStage = null;
        activeInfo = null;

        objsLayer1 = null;
        objsLayer0 = null;

        movement = null;
        gameAnimation = null;

        animations.Clear();
    }


    public void visibleLayer( bool v )
    {
        isVisibleLayer = v;

        transBackground.gameObject.SetActive( v );
        transUnit.gameObject.SetActive( v );
        transCell.gameObject.SetActive( v );
    }


    void clearStage()
    {
        GameDefine.DestroyAll( transBackground );
        GameDefine.DestroyAll( transUnit );
        GameDefine.DestroyAll( transCell );
        GameDefine.DestroyAll( transEvent );
    }



    public void active( int id )
    {
        activeID = id;
        activeRPGStage = GameRPGData.instance.getData( activeID );
        activeDTL = activeRPGStage.DTL;
        activeInfo = activeRPGStage.Info;

        byte[] block = new byte[ activeDTL.Width * activeDTL.Height ];
        for ( int i = 0 ; i < activeDTL.Width * activeDTL.Height ; i++ )
        {
            block[ i ] = ( activeDTL.Points[ i ].Move == 0 ? (byte)0 : GameRPGPathFinder.BLOCK );
        }

        GameRPGPathFinder.instance.initMap( activeDTL.Width ,
            activeDTL.Height , block );

        isVisibleLayer = true;
    }

    public void initPos( int i )
    {
        GameRPGPosInfo info = activeInfo.Pos[ i ];

//        GameRPGSceneMovement.instance.moveTo( info.MapPosX , info.MapPosY );

        GameMusicManager.instance.playMusic( 0 , "Music/Music_" + GameDefine.getString2( info.Music ) );

        string path = "Prefab/RPG/Rpgman";

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        gameAnimation = obj.GetComponent<GameAnimation>();
        obj.name = "Rpgman";

        gameAnimation.playAnimationRPG( GameAnimationType.Stand , GameAnimationDirection.South , null );

        Transform trans = obj.transform;
        trans.SetParent( transUnit );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        movement = obj.AddComponent<GameRPGMovement>();
        movement.setPos( info.PosX , info.PosY );
        movement.setDirection( (GameAnimationDirection)info.Direction );

        GameRPGSceneMovement.instance.followPos( movement.PosBattleX , movement.PosBattleY );
    }

    public void moveUpdate()
    {
        movement.moveUpdate();
    }

    public void moveTo( bool b )
    {
        movement.nextMove = b;
    }

    public void showEvent()
    {
        int posX = movement.PosX;
        int posY = movement.PosY;

        switch ( gameAnimation.animationDirection )
        {
            case GameAnimationDirection.South:
                {
                    if ( posY + 1 == activeDTL.Height )
                    {
                        return;
                    }

                    posY++;
                }
                break;
            case GameAnimationDirection.West:
                {
                    if ( posX == 0 )
                    {
                        return;
                    }

                    posX--;
                }
                break;
            case GameAnimationDirection.North:
                {
                    if ( posY == 0 )
                    {
                        return;
                    }

                    posY--;
                }
                break;
            case GameAnimationDirection.East:
                {
                    if ( posX + 1 == activeDTL.Width )
                    {
                        return;
                    }

                    posX++;
                }
                break;
        }


        GameRPGDTL.Point point = activeDTL.Points[ posY * Width + posX ];

        if ( point.MapEvent <= 0 )
        {
            return;
        }

        GameRPGEventManager.instance.showEvent( point.MapEvent , 0 , null );
    }

    public void moveTo( GameAnimationDirection dir )
    {
        if ( movement.isMoving )
        {
            return;
        }

        int posX = movement.PosX;
        int posY = movement.PosY;

        bool exit = false;

        switch ( dir )
        {
            case GameAnimationDirection.South:
                {
                    if ( posY + 1 == activeDTL.Height )
                    {
                        exit = true;
                        break;
                    }

                    posY++;
                }
                break;
            case GameAnimationDirection.West:
                {
                    if ( posX == 0 )
                    {
                        exit = true;
                        break;
                    }

                    posX--;
                }
                break;
            case GameAnimationDirection.North:
                {
                    if ( posY == 0 )
                    {
                        exit = true;
                        break;
                    }

                    posY--;
                }
                break;
            case GameAnimationDirection.East:
                {
                    if ( posX + 1 == activeDTL.Width )
                    {
                        exit = true;
                        break;
                    }

                    posX++;
                }
                break;
        }

        if ( exit || GameRPGPathFinder.instance.isBlock( posX , posY ) )
        {
            gameAnimation.playAnimationRPG( GameAnimationType.Stand ,
                            dir , null );

            return;
        }

        movement.moveTo( posX , posY , false , true , false , 2 , onMoveOver );
    }

    public void moveTo( int x , int y , OnEventOver over )
    {
        if ( movement.isMoving )
        {
            return;
        }
        
        movement.moveTo( x , y , false , true , false , 2 , over );
    }

    public void stopMove()
    {
        movement.stopMove();
    }

    void onMoveOver()
    {
        GameRPGDTL.Point point = activeDTL.Points[ movement.PosY * Width + movement.PosX ];

        if ( point.RPGEvent <= 0 )
        {
            return;
        }

        moveTo( false );

        GameRPGEventManager.instance.showEvent( point.RPGEvent , 0 , null );
    }

    public void unShowLayer( int id )
    {
        for ( int j = 0 ; j < activeRPGStage.Layer.L0.Length ; j++ )
        {
            if ( activeRPGStage.Layer.L0[ j ].ID == id )
            {
            }
        }
    }

    public void showUnits()
    {

    }

    public void showLayer( int id )
    {
        string path = GameDefine.getString2( activeID );

        if ( objsLayer0 == null ||
            objsLayer0.Length != activeRPGStage.Layer.L0.Length )
            objsLayer0 = new GameObject[ activeRPGStage.Layer.L0.Length ];

        for ( int j = 0 ; j < activeRPGStage.Layer.L0.Length ; j++ )
        {
            if ( activeRPGStage.Layer.L0[ j ].ID == id )
            {
                if ( objsLayer0[ j ] != null )
                {
                    continue;
                }

                if ( activeRPGStage.Layer.L0[ j ].Unknow4 == 10 &&
                        activeRPGStage.Layer.L0[ j ].Unknow5 == 8 )
                {
                    layerHeight = id * 5000;
                }

                if ( activeRPGStage.Layer.L0[ j ].Name.Contains( ".saf" ) )
                {
                    string pathDir = "Prefab/RPG/RPG" + path + "/";
                    string pathTexture = pathDir + activeRPGStage.Layer.L0[ j ].Name.Replace( ".saf" , "" );

                    GameObject imgObject = Resources.Load<GameObject>( pathTexture );
                    GameObject obj = Instantiate( imgObject , transBackground );
                    obj.name = activeRPGStage.Layer.L0[ j ].Name;

                    float z = GameDefine.getZ( activeRPGStage.Layer.L0[ j ].Unknow0 );

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( 0.0f , layerHeight , z );
                    trans.localScale = new Vector3( 5.0f , 5.0f , 5.0f );

                    GameAnimation animation = obj.GetComponent<GameAnimation>();

                    animation.playAnimation();

                    objsLayer0[ j ] = obj;
                }
                else
                {
                    string pathDir = "Texture/RPG/";
                    string pathTexture = pathDir + activeRPGStage.Layer.L0[ j ].Name;

                    GameObject imgObject = Resources.Load<GameObject>( "Prefab/Sprite" );
                    GameObject obj = Instantiate( imgObject , transBackground );
                    obj.name = activeRPGStage.Layer.L0[ j ].Name;

                    float z = GameDefine.getZ( activeRPGStage.Layer.L0[ j ].Unknow0 );

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( 0.0f , layerHeight , z );
                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    if ( activeRPGStage.Layer.L0[ j ].Unknow4 != 10 &&
                        activeRPGStage.Layer.L0[ j ].Unknow5 != 8 )
                    {
                        trans.localScale = new Vector3( 2.0f , 2.0f , 1.0f );
                    }

                    transUnit.localPosition = new Vector3( 0.0f , 0.0f , -1100 );

                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    sprite.sprite = Resources.Load<Sprite>( pathTexture );

                    float w = activeRPGStage.DTL.Width * GameDefine.TEXTURE_WIDTH;
                    float h = activeRPGStage.DTL.Height * GameDefine.TEXTURE_HEIGHT;

                    if ( w == 4096 || h == 4096 )
                    {
                        w = w / activeRPGStage.DTL.Width * GameDefine.TEXTURE_WIDTH;
                        h = w / activeRPGStage.DTL.Height * GameDefine.TEXTURE_HEIGHT;

                        trans.localScale = new Vector3( w , h , 1.0f );
                    }

                    objsLayer0[ j ] = obj;
                }

            }
        }


        if ( objsLayer1 == null ||
            objsLayer1.Length != activeRPGStage.Layer.L1.Length )
            objsLayer1 = new GameObject[ activeRPGStage.Layer.L1.Length ];

        for ( int j = 0 ; j < activeRPGStage.Layer.L1.Length ; j++ )
        {
            if ( activeRPGStage.Layer.L1[ j ].ID == id )
            {
                string pathDir = "Prefab/RPG/RPG" + path + "/";
                string pathTexture = pathDir + activeRPGStage.Layer.L1[ j ].Name;

                GameObject imgObject = Resources.Load<GameObject>( pathTexture );
                GameObject obj = Instantiate( imgObject , transBackground );
                obj.name = activeRPGStage.Layer.L1[ j ].Name;

                Transform trans = obj.transform;
                trans.localPosition = new Vector3( activeRPGStage.Layer.L1[ j ].OffsetX ,
                    -activeRPGStage.Layer.L1[ j ].OffsetY + layerHeight ,
                    -1000 - j );

                trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                GameAnimation animation = obj.GetComponent<GameAnimation>();

                if ( activeRPGStage.Layer.L1[ j ].Unknow5 == 0 )
                {
                    if ( activeRPGStage.Layer.L1[ j ].Unknow3 == GameDefine.INVALID_ID )
                    {
                        animation.playAnimation();
                    }
                    else
                    {
                    }
                }
                else
                {
                }

                objsLayer1[ j ] = obj;
            }
        }



#if UNITY_EDITOR

        if ( id == 1 )
        {
            GameRPGDTL dtl = activeRPGStage.DTL;
            int ii = 0;

            Sprite spriteObject = Resources.Load<Sprite>( "White" );

            for ( int i = 0 ; i < dtl.Height ; i++ )
            {
                for ( int j = 0 ; j < dtl.Width ; j++ )
                {
                    // editor test

                    GameObject imgObject = Resources.Load<GameObject>( "Prefab/Sprite" );
                    GameObject obj = Instantiate( imgObject , transCell );
                    obj.name = i + "-" + j;

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( j * 30 , -i * 24 + layerHeight , 0.0f );
                    //                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    sprite.color = new Color( 1.0f , 1.0f , 1.0f , ( dtl.Points[ ii ].Move == 1 ? 0.2f : 0.0f ) );
                    sprite.sprite = spriteObject;

                    GamePointTset test = obj.AddComponent<GamePointTset>();
                    test.Value0 = dtl.Points[ ii ].Move;
                    test.Value1 = dtl.Points[ ii ].MapEvent;
                    test.Value2 = dtl.Points[ ii ].RPGEvent;

                    if ( dtl.Points[ ii ].MapEvent > 0 )
                    {
                        sprite.color = new Color( 0.0f , 1.0f , 0.0f , 0.25f );
                    }
                    else if ( dtl.Points[ ii ].RPGEvent > 0 )
                    {
                        sprite.color = new Color( 1.0f , 0.0f , 0.0f , 0.25f );
                    }

                    ii++;
                }
            }

        }

#endif

    }

    public bool checkPosition( float x , float y )
    {
        if ( x < 0 || y > 0 ||
            x + GameCameraManager.instance.sceneWidth > activeDTL.Width * GameDefine.TEXTURE_WIDTH ||
            y - GameCameraManager.instance.sceneHeight < -activeDTL.Height * GameDefine.TEXTURE_HEIGHT )
        {
            return false;
        }

        return true;
    }

    public int getZ( int x , int y )
    {
        return -y;
    }


    public void addAnimation( int id , int id2 )
    {
        GameAnimation ani = null;

        string str = id + " " + id2;

        if ( animations.ContainsKey( str ) )
        {
            ani = animations[ str ];
            ani.stopAnimation();
            ani.clearAnimation();
            Destroy( ani.gameObject );
            ani.transform.SetParent( null );
            animations.Remove( str );
        }

        for ( int i = 0 ; i < activeRPGStage.Layer.L1.Length ; i++ )
        {
            if ( activeRPGStage.Layer.L1[ i ].ID == id &&
                activeRPGStage.Layer.L1[ i ].Parm == id2 )
            {
                string path = "Prefab/RPG/RPG" + GameDefine.getString2( activeID ) + "/";
                path += activeRPGStage.Layer.L1[ i ].Name;

                GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
                ani = obj.GetComponent<GameAnimation>();

                Transform trans = obj.transform;
                trans.SetParent( transEvent );
                trans.localPosition = new Vector3( 0.0f , layerHeight , 0.0f );
                trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                animations.Add( str , ani );

                return;
            }
           
        }

    }

    public void playAnimation( int id , int id2 )
    {
        string str = id + " " + id2;

        if ( !animations.ContainsKey( str ) )
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log( "playAnimation " + animations[ str ].gameObject.name );
#endif

        GameAnimation animation = animations[ str ];
        animation.playAnimation( 0 , GameDefine.INVALID_ID , false );
    }

    public void clearGameAnimation()
    {
        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();
    }

    public void playAnimation( int id , int id2 , int frame , int endFrame , OnEventOver over )
    {
        string str = id + " " + id2;

        if ( !animations.ContainsKey( str ) )
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log( "playAnimation " + animations[ str ].gameObject.name );
#endif

        GameAnimation animation = animations[ str ];

        Transform trans = animation.transform;
        trans.localPosition = new Vector3( GameRPGSceneMovement.instance.PosXReal ,
            GameRPGSceneMovement.instance.PosYReal + layerHeight ,
            -id2 * 20 );

        if ( frame == GameDefine.INVALID_ID )
        {
            animation.playAnimation( 0 , GameDefine.INVALID_ID , false , over );
        }
        else
        {
            if ( frame == endFrame )
            {
                animation.stopAnimation();
                animation.showFrame( endFrame );

                if ( over != null )
                {
                    over();
                }
            }
            else
            {
                animation.playAnimation( frame , endFrame + 1 , false , over );
            }

            //                animation.stopAnimation();
            //               animation.showFrame( frame );             
        }
    }

    public void clearAnimation( int id , int id2 )
    {
        string str = id + " " + id2;

        if ( !animations.ContainsKey( str ) )
        {
            return;
        }

        GameAnimation animation = animations[ str ];
        animation.stopAnimation();
        animation.clearAnimation();
    }

}


