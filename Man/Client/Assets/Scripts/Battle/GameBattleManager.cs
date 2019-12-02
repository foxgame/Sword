using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleManager : Singleton< GameBattleManager >
{
    List<int> layerList = new List<int>();

    GameBattleDTL activeDTL;

    [SerializeField]
    GameBattleStage activeBattleStage;
    [SerializeField]
    GameBattleStart activeBattleStart;

    GameObject[] objsLayer1 = null;
    GameObject[] objsLayer0 = null;

    Dictionary<int , GameAnimation> animations = new Dictionary<int , GameAnimation>();

    Dictionary<int , GameBattleMEVT> mapEvent = new Dictionary<int , GameBattleMEVT>();

    Transform transBackground;
    Transform transCell;
    Transform transTreasures;

    [SerializeField]
    int layerHeight = 0;

    [SerializeField]
    int lastEffectID = GameDefine.INVALID_ID;
    OnEventOver onEventOver;

    [SerializeField]
    public int LastUnitID = GameDefine.INVALID_ID;

    public GameBattleDTL ActiveDTL { get { return activeDTL; } }

    public int Width { get { return activeDTL.Width; } }
    public int Height { get { return activeDTL.Height; } }

    public GameBattleStage ActiveBattleStage { get { return activeBattleStage; } }

    public int ActiveLayuer { get { return layerList[ layerList.Count - 1 ]; } }
    public int LayerHeight { get { return layerHeight; } }

    public Dictionary<int , GameBattleMEVT> MapEvent { get{ return mapEvent; } }

    public void clear()
    {
        clearAnimations();
        clearTreasures();
        clearStage();

        activeDTL = null;
        activeBattleStage = null;
        activeBattleStart = null;

        layerHeight = 0;

        lastEffectID = GameDefine.INVALID_ID;
        onEventOver = null;
    }

    public void clearStage()
    {
        for ( int j = 0 ; j < activeBattleStage.Layer.L0.Length ; j++ )
        {
            if ( objsLayer0[ j ] != null )
            {
                Destroy( objsLayer0[ j ] );
                objsLayer0[ j ].gameObject.transform.SetParent( null );
                objsLayer0[ j ] = null;
            }
        }

        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( objsLayer1[ j ] != null )
            {
                Destroy( objsLayer1[ j ] );
                objsLayer1[ j ].gameObject.transform.SetParent( null );
                objsLayer1[ j ] = null;
            }
        }

        objsLayer0 = null;
        objsLayer1 = null;

        layerList.Clear();

        GameDefine.DestroyAll( transBackground );
        GameDefine.DestroyAll( transCell );
    }

    public override void initSingleton()
    {
        transBackground = GameObject.Find( "BattleScene/Background" ).transform;
        transCell = GameObject.Find( "BattleScene/Cell" ).transform;
        transTreasures = GameObject.Find( "BattleScene/Treasures" ).transform;
    }

    public void active()
    {
        activeBattleStage = GameBattleData.instance.getStage( GameUserData.instance.Stage );
        activeBattleStart = GameBattleData.instance.getStartData( GameUserData.instance.Stage );

        activeDTL = activeBattleStage.DTL;

        GameCameraManager.instance.MapWidth = activeDTL.Width * GameDefine.TEXTURE_WIDTH;
        GameCameraManager.instance.MapHeight = activeDTL.Height * GameDefine.TEXTURE_HEIGHT;

        byte[] block = new byte[ activeDTL.Width * activeDTL.Height ];
        for ( int i = 0 ; i < activeDTL.Width * activeDTL.Height ; i++ )
        {
            block[ i ] = activeDTL.Points[ i ].Move;
        }

        GameBattlePathFinder.instance.initMap( activeDTL.Width ,
            activeDTL.Height , block );
    }

    public GameBattleDTL.Point getPoint( int x , int y )
    {
        if ( y < 0 || x < 0 || x >= activeDTL.Width || y >= activeDTL.Height )
        {
            return null;
        }

        return activeDTL.Points[ y * activeDTL.Width + x ];
    }


    public void initMusic()
    {
        short music = activeBattleStart.Music;

        GameMusicManager.instance.playMusic( 0 , "Music/Music_" + GameDefine.getString2( music ) );
    }


//     public bool isUnit( int x , int y )
//     {
//         for ( int i = 0 ; i < men.Length ; i++ )
//         {
//             GameMovement movement = men[ i ].GetComponent<GameMovement>();
// 
//             if ( movement.posX == x && movement.posY == y )
//             {
//                 return true;
//             }
//         }
// 
//         return false;
//     }

    

    void onEffectPlayOver()
    {
        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    void onEffectPlayOver1()
    {
        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public void addAnimation( int id , int name )
    {
        GameAnimation ani = null;

        if ( animations.ContainsKey( id ) )
        {
            ani = animations[ id ];
            ani.stopAnimation();
            ani.clearAnimation();
            ani.transform.SetParent( null );
            Destroy( ani.gameObject );
            animations.Remove( id );
        }

        //        else
        //        {
        string path = "Prefab/Stage/Stage" + GameDefine.getString2( GameUserData.instance.Stage ) + "/Eanm_";
        path += GameDefine.getString2( name );

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        ani = obj.GetComponent<GameAnimation>();
        obj.name = "Eanm_" + GameDefine.getString2( name );

        Transform trans = obj.transform;
        trans.SetParent( transBackground );
        trans.localPosition = new Vector3( 0.0f , layerHeight , 0.0f );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        ani.lastUnitID = LastUnitID;
        LastUnitID = GameDefine.INVALID_ID;

        animations.Add( id , ani );

        //        }
    }


    public bool getAllTreasures()
    {
        int count = 0;
        foreach ( KeyValuePair<int , GameBattleMEVT> item in mapEvent )
        {
            if ( item.Value.EventType > GameBattleMapEventType.Event )
            {
                count++;
            }
        }

        for ( int i = 0 ; i < activeBattleStage.MEVT.Length ; i++ )
        {
            if ( activeBattleStage.MEVT[ i ].EventType > GameBattleMapEventType.Event )
            {
                return count == 0;
            }
        }

        return false;
    }


    public void clearTreasures()
    {
        mapEvent.Clear();

        GameDefine.DestroyAll( transTreasures );
    }

    public void initTreasures()
    {
        mapEvent.Clear();

        for ( int i = 0 ; i < activeBattleStage.MEVT.Length ; i++ )
        {
            if ( activeBattleStage.MEVT[ i ].EventType != GameBattleMapEventType.None )
            {
                mapEvent.Add( i , activeBattleStage.MEVT[ i ] );
            }
        }
    }

    public void addTreasure( int i )
    {
        if ( activeBattleStage.MEVT[ i ].EventType != GameBattleMapEventType.None )
        {
            mapEvent.Add( i , activeBattleStage.MEVT[ i ] );
        }
    }

    public void updateTreasures()
    {
        GameBattleDTL dtl = activeBattleStage.DTL;

        string path = "Prefab/Misc/TREASURE";

        GameObject gameObject = Resources.Load<GameObject>( path ) ;

        bool[] bc = new bool[ activeBattleStage.MEVT.Length ];
        for ( int i = 0 ; i < bc.Length ; i++ )
        {
            bc[ i ] = false;
        }

        int ii = 0;
        for ( int i = 0 ; i < dtl.Height ; i++ )
        {
            for ( int j = 0 ; j < dtl.Width ; j++ )
            {
                if ( mapEvent.ContainsKey( dtl.Points[ ii ].MapEvent ) )
                {
                    bc[ dtl.Points[ ii ].MapEvent ] = true;

                    if ( mapEvent[ dtl.Points[ ii].MapEvent ].EventType == GameBattleMapEventType.Event )
                    {
                        ii++;
                        continue;
                    }

                    GameObject obj = Instantiate<GameObject>( gameObject , transTreasures );
                    obj.name = dtl.Points[ ii ].MapEvent.ToString();

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( j * GameDefine.TEXTURE_WIDTH ,
                        -i * GameDefine.TEXTURE_HEIGHT + layerHeight , 0.0f );

                    GameAnimation gameAnimation = obj.GetComponent<GameAnimation>();
                    gameAnimation.playAnimation();
                    gameAnimation.offsetX = GameDefine.BATTLE_OFFSET_X;
                    gameAnimation.offsetY = GameDefine.BATTLE_OFFSET_Y;
                }

                ii++;
            }
        }

        for ( int i = 0 ; i < bc.Length ; i++ )
        {
            if ( !bc[ i ] )
            {
                mapEvent.Remove( i );
            }
        }

    }

    public void clearAnimations()
    {
        foreach ( KeyValuePair< int , GameAnimation > kv in animations )
        {
            kv.Value.clearAnimation();
        }

        animations.Clear();
    }

    public void clearAnimation( int id )
    {
        if ( !animations.ContainsKey( id ) )
        {
            return;
        }

        GameAnimation animation = animations[ id ];
        animation.stopAnimation();
        animation.clearAnimation();
    }

    public void playAnimation( int id , int id2 )
    {
        if ( !animations.ContainsKey( id ) )
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log( "playAnimation " + animations[ id ].gameObject.name );
#endif

        GameAnimation animation = animations[ id ];
        animation.playAnimation( 0 , GameDefine.INVALID_ID , false );
    }

    public void playAnimation( int id , int id2 , int frame , int endFrame , OnEventOver over )
    {
        if ( !animations.ContainsKey( id ) )
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log( "playAnimation "  + animations[ id ].gameObject.name );
#endif

        onEventOver = over;

        GameAnimation animation = animations[ id ];

        int height = layerHeight;

        bool bUpdateHeight = false;

        if ( id2 >= 10000500 && GameUserData.instance.Stage == 19 )
        {
            // fix stage 7 19 bug
            height = 2 * 5000;
            bUpdateHeight = true;

            GameAnimation ani = objsLayer0[ 1 ].GetComponent<GameAnimation>();
            ani.stopAnimation();
            ani.clearAnimation();
        }

        if ( ( id2 >= 20000000 && GameUserData.instance.Stage == 7 ) ||
            ( id2 >= 20000000 && GameUserData.instance.Stage == 14 ) )
        {
            // fix stage 7 14 bug

            height = 2 * 5000;
            bUpdateHeight = true;
        }

        float z = GameDefine.getZ( id2 );

        if ( id2 >= 20000000 )
        {
            z -= 3000;
        }

        Transform trans = animation.transform;

        if ( animation.lastUnitID != GameDefine.INVALID_ID )
        {
            GameBattleUnit unit = GameBattleUnitManager.instance.getUnit( animation.lastUnitID );

            GameBattleSceneMovement.instance.moveToEvent( unit.PosX - GameCameraManager.instance.xCell ,
                unit.PosY - GameCameraManager.instance.yCell );

            trans.localPosition = new Vector3( GameBattleSceneMovement.instance.PosXReal ,
                GameBattleSceneMovement.instance.PosYReal + height , z );
        }
        else
        {
            trans.localPosition = new Vector3( GameBattleSceneMovement.instance.AnimationPosX ,
               GameBattleSceneMovement.instance.AnimationPosY + height ,
               z );
        }


        if ( id2 >= 20000000 &&
            GameUserData.instance.Stage == 23 )
        {
            // fix stage 23 bug

            trans.localPosition = new Vector3( GameBattleSceneMovement.instance.PosXReal ,
    GameBattleSceneMovement.instance.PosYReal + height , z );

            trans.localScale = new Vector3( GameCameraManager.instance.sceneWidth / 320.0f , 1.0f , 1.0f );
        }

        if ( id2 == 10000500 && 
            GameUserData.instance.Stage == 24 )
        {
            // fix stage 24 bug

            trans.localPosition = new Vector3( 260.0f , 
                -620.0f + height , z );
        }

        if ( frame == GameDefine.INVALID_ID )
        {
            animation.playAnimation( 0 , GameDefine.INVALID_ID , false , onEffectPlayOver );
        }
        else
        {
            if ( frame == endFrame )
            {
                animation.stopAnimation();
                animation.showFrame( endFrame );

                if ( onEventOver != null )
                {
                    onEventOver();
                }
            }
            else
            {
                animation.playAnimation( frame , endFrame + 1 , false , onEffectPlayOver );
            }

            //                animation.stopAnimation();
            //               animation.showFrame( frame );             
        }

        if ( bUpdateHeight )
            GameCameraManager.instance.setPos( GameBattleSceneMovement.instance.PosXReal ,
          GameBattleSceneMovement.instance.PosYReal + height );
    }


    public void playOnceEffect( int id , int frame , int endFrame , OnEventOver over )
    {
        onEventOver = over;

        bool b = false;

        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].Parm == id )
            {
                if ( activeBattleStage.Layer.L1[ j ].ParmEffect != GameDefine.INVALID_ID )
                {
                    if ( GameUserData.instance.getGameData( activeBattleStage.Layer.L1[ j ].ParmEffect ) != 1 )
                    {
                        continue;
                    }
                }

                lastEffectID = j;
                GameAnimation animation = objsLayer1[ j ].GetComponent<GameAnimation>();

#if UNITY_EDITOR
                Debug.Log( "playOnceEffect " + animation.gameObject.name + " " + j );
#endif

                if ( frame == GameDefine.INVALID_ID )
                {
                    if ( activeBattleStage.Layer.L1[ j ].Pause == 0 )
                    {
                        animation.stopAnimation();
                        animation.showFrame( endFrame );
                    }
                    else
                    {
                        if ( !b )
                        {
                            animation.playAnimation( 0 , GameDefine.INVALID_ID , false , onEffectPlayOver1 );
                        }
                        else
                        {
                            animation.playAnimation( 0 , GameDefine.INVALID_ID , false , null );
                        }

                        b = true;
                    }
                }
                else
                {
                    if ( frame == endFrame )
                    {
                        animation.stopAnimation();
                        animation.showFrame( endFrame );
                    }
                    else
                    {
                        if ( !b )
                        {
                            animation.playAnimation( frame , endFrame + 1 , false , onEffectPlayOver1 );
                        }
                        else
                        {
                            animation.playAnimation( frame , endFrame + 1 , false , null );
                        }

                        b = true;
                    }

                }

            }
        }


        if ( !b )
        {
            onEffectPlayOver1();
        }

    }

    public void showEffect( int s1 , int s2 , int s3 )
    {
        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].ParmEffect == s1 )
            {
                if ( objsLayer1[ j ] == null )
                {
                    continue;
                }

                GameAnimation animation = objsLayer1[ j ].GetComponent<GameAnimation>();

                if ( s2 == 0 )
                {
                    animation.stopAnimation();
                    animation.clearAnimation();
                }
                else
                {
                    if ( activeBattleStage.Layer.L1[ j ].Pause == 0 )
                    {
                        animation.playAnimation();
                    }
                    else
                    {
                        animation.stopAnimation();
                        animation.showFrame( 0 );
                    }
                }
                
            }
        }
    }

    public void showEffect( int id , int s )
    {
        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].Parm == id )
            {

                if ( objsLayer1[ j ] == null )
                {
                    string path = GameDefine.getString2( GameUserData.instance.Stage );
                    string pathDir = "Prefab/Stage/Stage" + path + "/";
                    string pathTexture = pathDir + activeBattleStage.Layer.L1[ j ].Name;

                    GameObject imgObject = Resources.Load<GameObject>( pathTexture );
                    GameObject obj = Instantiate( imgObject , transBackground );
                    obj.name = activeBattleStage.Layer.L1[ j ].Name;

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( GameBattleSceneMovement.instance.AnimationPosX ,
                        GameBattleSceneMovement.instance.AnimationPosY + layerHeight , 1.0f );

                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    GameAnimation animation = obj.GetComponent<GameAnimation>();

                    objsLayer1[ j ] = animation.gameObject;

                    if ( GameUserData.instance.Stage == 29 )
                    {
                        // fix stage 29 bug

                        trans.localPosition = new Vector3( GameBattleSceneMovement.instance.AnimationPosX ,
                        GameBattleSceneMovement.instance.AnimationPosY + layerHeight , -1005.0f );
                    }

                    if ( GameUserData.instance.Stage == 34 && id == 6 )
                    {
                        // fix stage 34 bug

                        trans.localPosition = new Vector3( 0.0f , layerHeight , 1.0f );
                    }

                    if ( activeBattleStage.Layer.L1[ j ].Pause == 0 )
                    {
                        animation.playAnimation();
                    }
                    else
                    {
                        animation.stopAnimation();
                        animation.showFrame( 0 );
                    }

                    return;
                }

                GameAnimation animation1 = objsLayer1[ j ].GetComponent<GameAnimation>();

                if ( activeBattleStage.Layer.L1[ j ].Pause == 0 )
                {
                    animation1.playAnimation();
                }
                else
                {
                    animation1.stopAnimation();
                    animation1.showFrame( 0 );
                }
            }
        }
       
    }

    public void unShowEffect( int id , int s )
    {
        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].Parm == id )
            {
                if ( objsLayer1[ j ] == null )
                {
                    continue;
                }

                GameAnimation animation = objsLayer1[ j ].GetComponent<GameAnimation>();

                animation.stopAnimation();
                animation.clearAnimation();
            }

//             if ( activeBattleStage.Layer.L1[ j ].ParmEffect != GameDefine.INVALID_ID )
//             {
//                 if ( GameUserData.instance.getGameData( activeBattleStage.Layer.L1[ j ].ParmEffect ) != 1 )
//                 {
//                     GameAnimation animation = objsLayer1[ j ].GetComponent<GameAnimation>();
//                     animation.stopAnimation();
//                     animation.clearAnimation();
//                 }
//             }
        }
    }

    public void unShowLayer( int id )
    {
        layerList.Remove( id );

        for ( int j = 0 ; j < activeBattleStage.Layer.L0.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L0[ j ].ID == id )
            {
                if ( objsLayer0[ j ] != null )
                {
                    Destroy( objsLayer0[ j ] );
                    objsLayer0[ j ].gameObject.transform.SetParent( null );
                    objsLayer0[ j ] = null;
                }

            }
        }
        
        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].ID == id )
            {
                if ( objsLayer1[ j ] != null )
                {
                    Destroy( objsLayer1[ j ] );
                    objsLayer1[ j ].gameObject.transform.SetParent( null );
                    objsLayer1[ j ] = null;
                }
            }
        }

        if ( layerList.Count > 0 )
            layerHeight = layerList[ layerList.Count - 1 ] * 5000;
    }

    public void showLayer( int id , bool b )
    {
//        clearAnimations();

        layerList.Add( id );

        string path = GameDefine.getString2( GameUserData.instance.Stage );

        if ( objsLayer0 == null ||
            objsLayer0.Length != activeBattleStage.Layer.L0.Length )
            objsLayer0 = new GameObject[ activeBattleStage.Layer.L0.Length ];

        int lastLayer = 0;

        float lastWidth = 0.0f;
        float lastHeight = 0.0f;

        for ( int j = 0 ; j < activeBattleStage.Layer.L0.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L0[ j ].ID == id )
            {
                if ( objsLayer0[ j ] != null )
                {
                    continue;
                }

                lastLayer = activeBattleStage.Layer.L0[ j ].Lyaer;
                layerHeight = id * 5000;

                if ( id > 1 && GameUserData.instance.Stage == 34 )
                {
                    // fix stage 34 bug.
                    layerHeight = 5000;
                }

                if ( layerList.Count == 1 )
                {
                    GameBattleSceneMovement.instance.moveToEvent( 0 , 0 );
                }

                if ( activeBattleStage.Layer.L0[ j ].Name.Contains( ".saf" ) )
                {
                    string pathDir = "Prefab/Stage/Stage" + path + "/";
                    string pathTexture = pathDir + activeBattleStage.Layer.L0[ j ].Name.Replace( ".saf" , "" );

                    GameObject imgObject = Resources.Load<GameObject>( pathTexture );
                    GameObject obj = Instantiate( imgObject , GameCameraManager.instance.transform );
                    obj.name = activeBattleStage.Layer.L0[ j ].Name;

                    float z = GameDefine.getZ( activeBattleStage.Layer.L0[ j ].Unknow0 );

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( -GameCameraManager.instance.sceneWidthHalf ,
                        GameCameraManager.instance.sceneHeightHalf
                         , z + 20 );
                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    GameAnimation animation = obj.GetComponent<GameAnimation>();

                    animation.playAnimation();

                    objsLayer0[ j ] = obj;
                }
                else
                {
                    string pathDir = "Texture/Map/Stage" + path + "/";
                    string pathTexture = pathDir + activeBattleStage.Layer.L0[ j ].Name;

                    GameObject imgObject = Resources.Load<GameObject>( "Prefab/Map" );
                    GameObject obj = Instantiate( imgObject , transBackground );
                    obj.name = activeBattleStage.Layer.L0[ j ].Name;

                    float z = GameDefine.getZ( activeBattleStage.Layer.L0[ j ].Unknow0 );

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( 0.0f , layerHeight , z );
                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    sprite.sprite = Resources.Load<Sprite>( pathTexture );

                    float w = activeBattleStage.Layer.L0[ j ].Width * GameDefine.TEXTURE_WIDTH;
                    float h = activeBattleStage.Layer.L0[ j ].Height * GameDefine.TEXTURE_HEIGHT;

                    if ( w > 4096 || h > 4096 )
                    {
                        w = w / activeBattleStage.DTL.Width * GameDefine.TEXTURE_WIDTH;
                        h = w / activeBattleStage.DTL.Height * GameDefine.TEXTURE_HEIGHT;

                        trans.localScale = new Vector3( w , h , 1.0f );
                    }

                    if ( activeBattleStage.Layer.L0[ j ].Unknow4 != 10 &&
                        activeBattleStage.Layer.L0[ j ].Unknow5 != 8 )
                    {
                        if ( GameUserData.instance.Stage == 18 )
                        {
                            // fix stage 18 bug
                            trans.localPosition = new Vector3( 0.0f , layerHeight , 10.0f );
                        }

                        trans.localScale = new Vector3( lastWidth / w , lastHeight / h , 1.0f );
                    }
                    else
                    {
                        lastWidth = w;
                        lastHeight = h;
                    }

                    objsLayer0[ j ] = obj;
                }
                
            }
        }


        if ( objsLayer1 == null || 
            objsLayer1.Length != activeBattleStage.Layer.L1.Length )
            objsLayer1 = new GameObject[ activeBattleStage.Layer.L1.Length ];

        for ( int j = 0 ; j < activeBattleStage.Layer.L1.Length ; j++ )
        {
            if ( activeBattleStage.Layer.L1[ j ].ID != id )
            {
                continue;
            }

            string pathDir = "Prefab/Stage/Stage" + path + "/";
            string pathTexture = pathDir + activeBattleStage.Layer.L1[ j ].Name;

            GameObject imgObject = Resources.Load<GameObject>( pathTexture );
            GameObject obj = Instantiate( imgObject , transBackground );
            obj.name = activeBattleStage.Layer.L1[ j ].Name;

            Transform trans = obj.transform;
            trans.localPosition = new Vector3( activeBattleStage.Layer.L1[ j ].OffsetX ,
                -activeBattleStage.Layer.L1[ j ].OffsetY + layerHeight ,
                -1000 - j );

            
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

            GameAnimation animation = obj.GetComponent<GameAnimation>();

            if ( activeBattleStage.Layer.L1[ j ].VisibleBattle == 0 )
            {
                bool bVisible = true;

                if ( activeBattleStage.Layer.L1[ j ].ParmEffect != GameDefine.INVALID_ID )
                {
                    bVisible = GameUserData.instance.getGameData( activeBattleStage.Layer.L1[ j ].ParmEffect ) > 0;
                }

                if ( bVisible )
                {
                    switch ( activeBattleStage.Layer.L1[ j ].Pause )
                    {
                        case 0:
                            animation.playAnimation();
                            break;
                        case 1:
                            animation.playAnimation();
                            //                                Debug.LogError( "activeBattleStage.Layer.L1[ j ].Pause" + j );
                            break;
                        case 2:
                            animation.playSound = false;
                            animation.showFrame( 0 );
                            animation.playSound = true;
                            break;
                    }
                }

            }



//             // 夏侯婴加入？ 
//             if ( GameUserData.instance.Stage == 28 &&
//                 activeBattleStage.Layer.L1[ j ].Parm == 7 ) // 8
//             {
//                 continue;
//             }
//             if ( GameUserData.instance.Stage == 29 &&
//                 activeBattleStage.Layer.L1[ j ].Parm == 1 ) // 3
//             {
//                 continue;
//             }

            objsLayer1[ j ] = obj;
        }


#if UNITY_EDITOR

        if ( id == 1 )
        {
            GameBattleDTL dtl = activeBattleStage.DTL;
            int ii = 0;

            Sprite spriteObject = Resources.Load<Sprite>( "White" );

            for ( int i = 0 ; i < dtl.Height ; i++ )
            {
                for ( int j = 0 ; j < dtl.Width ; j++ )
                {
                    // editor test

                    //                    return;

                    GameObject imgObject = Resources.Load<GameObject>( "Prefab/Sprite" );
                    GameObject obj = Instantiate( imgObject , transCell );
                    obj.name = i + "-" + j;

                    Transform trans = obj.transform;
                    trans.localPosition = new Vector3( j * 30 , -i * 24 + layerHeight , 0.0f );
//                    trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

                    SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                    float c = ( dtl.Points[ ii ].Move * 25 ) / 255.0f;
                    sprite.color = new Color( c , c , c , ( c > 0.25f ? 0.25f : c ) );
                    sprite.sprite = spriteObject;

                    if ( dtl.Points[ ii ].MapEvent != -1 &&
                        activeBattleStage.MEVT.Length > dtl.Points[ ii ].MapEvent )
                    {
                        if ( activeBattleStage.MEVT[ dtl.Points[ ii ].MapEvent ].EventType == GameBattleMapEventType.Event )
                        {
                            sprite.color = new Color( 0.0f , 1.0f , 0.0f , 0.25f );
                        }
                        else if ( activeBattleStage.MEVT[ dtl.Points[ ii ].MapEvent ].EventType == GameBattleMapEventType.Item )
                        {
                            sprite.color = new Color( 1.0f , 1.0f , 0.0f , 0.25f );
                        }
                    }

                    GamePointTset test = obj.AddComponent<GamePointTset>();
                    test.Value0 = dtl.Points[ ii ].Move;
                    test.Value1 = dtl.Points[ ii ].Floor;
                    test.Value2 = dtl.Points[ ii ].MapEvent;

//                     obj = Instantiate( imgObject , transUnit );
//                     obj.name = i + "-" + j;
// 
//                     trans = obj.GetComponent<RectTransform>();
//                     trans.localPosition = new Vector3( j * 30 , -i * 24 , 0.0f );
//                     trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
//                     trans.sizeDelta = new Vector2( 30 , 24 );
// 
//                     image = obj.GetComponent<Image>();
//                     image.enabled = false;

                    ii++;
                }
            }

        }

#endif

    }


    void onCheckMapEvent()
    {
        GameBattleInput.instance.Pause = false;

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    void onCheckMapEvent1()
    {
        GameBattleInput.instance.Pause = false;

        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    public void checkMapEvent( GameBattleUnit unit , OnEventOver over )
    {
        onEventOver = over;

        GameBattleDTL.Point point = getPoint( unit.PosX , unit.PosY );

        if ( !mapEvent.ContainsKey( point.MapEvent ) )
        {
            onCheckMapEvent();
            return;
        }

        GameBattleMEVT evt = mapEvent[ point.MapEvent ];

        switch ( evt.EventType )
        {
            case GameBattleMapEventType.Event:
                {
                    switch ( evt.EventParm1 )
                    {
                        // what ghost !?

                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            onCheckMapEvent();
                            return;
                        case 4:
                            if ( GameUserData.instance.Stage == 31 )
                            {
                                // fix stage 30 bug. so many bugs.

                                if ( GameUserData.instance.getGameData( 10 ) == 0 )
                                {
                                    onCheckMapEvent();
                                    return;
                                }

                                if ( unit.UnitID > 1 )
                                {
                                    onCheckMapEvent();
                                    return;
                                }

                                if ( unit.UnitID == 0 && unit.WeaponID != 14 )
                                {
                                    onCheckMapEvent();
                                    return;
                                }

                                if ( unit.UnitID == 0 && GameUserData.instance.Proficiency < 25 - PlayerPrefs.GetInt( "review" , 0 ) )
                                {
                                    onCheckMapEvent();
                                    return;
                                }

                                if ( unit.UnitID == 1 && unit.BattleAIType == GameBattleAIType.None )
                                {
                                    onCheckMapEvent();
                                    return;
                                }
                            }
                            break;
                        case 5:
                            onCheckMapEvent();
                            return;
                        case 6:
                            break;
                    }

                    GameUserData.instance.setGameData( (int)GameUserDataType.MapEvent6 , unit.BattleManID );

                    GameBattleEventManager.instance.showEvent( evt.EventParm2 , 0 , onCheckMapEvent );
                }
                break;
            case GameBattleMapEventType.Item:
                {
                    switch ( evt.ItemGetType )
                    {
                        case GameBattleMapEventType.Item:
                            {
                                if ( unit.canAddItem() )
                                {
                                    unit.addItem( evt.ItemGetParm1 );
                                }
                                else
                                {
                                    GameUserData.instance.addItem( evt.ItemGetParm1 );
                                }

                                GameBattleGetItemUI.instance.show( evt.ItemGetParm1 , 0 , null );
                                onCheckMapEvent();
                            }
                            break;
                        case GameBattleMapEventType.Gold:
                            {
                                GameUserData.instance.addGold( evt.ItemGetParm1 );

                                GameBattleGetItemUI.instance.show( GameDefine.INVALID_ID , evt.ItemGetParm1 , null );
                                onCheckMapEvent();
                            }
                            break;
                    }

                    mapEvent.Remove( point.MapEvent );
                }
                break;
        }

        

        for ( int i = 0 ; i < transTreasures.childCount ; ++i )
        {
            if ( transTreasures.GetChild( i ).gameObject.name == point.MapEvent.ToString() )
            {
                Destroy( transTreasures.transform.GetChild( i ).gameObject );
            }
        }

        //         clearTreasures();
        //         updateTreasures();
    }

    public GameBattleDTL.Point getMapEvnetPoint( int id , ref short x , ref short y )
    {
        for ( int i = 0 ; i < activeDTL.Points.Length ; i++ )
        {
            if ( activeDTL.Points[ i ].MapEvent == id )
            {
                y = (short)( i / activeDTL.Width );
                x = (short)( i % activeDTL.Width );

                return activeDTL.Points[ i ];
            }
        }

        return null;
    }

    public bool checkMapEvent( int x , int y , short id )
    {
        GameBattleDTL.Point point = getPoint( x , y );

        if ( point != null )
        {
            return point.MapEvent == id;
        }

        return false;
    }

    public void checkMapEvent1( GameBattleUnit unit , OnEventOver over )
    {
        onEventOver = over;

        GameBattleDTL.Point point = getPoint( unit.PosX , unit.PosY );

        if ( !mapEvent.ContainsKey( point.MapEvent ) )
        {
            onCheckMapEvent();
            return;
        }

        GameBattleMEVT evt = mapEvent[ point.MapEvent ];

        switch ( evt.EventType )
        {
            case GameBattleMapEventType.Event:
                {
                    switch ( evt.EventParm1 )
                    {
                        // what ghost !?

                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            if ( unit.IsEnemy )
                            {
                                onCheckMapEvent();
                                return;
                            }
                            break;
                        case 4:
                            if ( GameUserData.instance.Stage == 31 )
                            {
                                if ( unit.UnitID > 1 )
                                {
                                    onCheckMapEvent();
                                    return;
                                }
                            }
                            break;
                        case 3:
                        case 5:
                            {
                                if ( unit.BattleAIType != GameBattleAIType.AIMoveToPos )
                                {
                                    onCheckMapEvent();
                                    return;
                                }

                                if ( !checkMapEvent( unit.AIMoveToX , unit.AIMoveToY , point.MapEvent ) )
                                {
                                    onCheckMapEvent();
                                    return;
                                }
                            }
                            break;
                        case 6:
                            break;
                    }

                    GameUserData.instance.setGameData( (int)GameUserDataType.MapEvent6 , unit.BattleManID );

                    GameBattleEventManager.instance.showEvent( evt.EventParm2 , 0 , onCheckMapEvent1 );
                }
                break;
            default:
                onCheckMapEvent1();
                break;
        }

    }




    public bool checkPosition( float x , float y )
    {
        if ( x < 0 || y > 0 ||
            x > activeDTL.Width * GameDefine.TEXTURE_WIDTH ||
            y < -activeDTL.Height * GameDefine.TEXTURE_HEIGHT )
        {
            return false;
        }

        return true;
    }

    public int getZ( int x , int y )
    {
        return -y * activeDTL.Width - x;
    }

}
