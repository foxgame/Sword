using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameCampUI : GameUI<GameCampUI>
{
    GameAnimation gameAnimation;
    RectTransform transBackground;

    GameCampScript script;

    public override void initSingleton()
    {
        transBackground = transform.Find( "Background" ).GetComponent<RectTransform>();
    }

    void clear()
    {
        GameDefine.DestroyAll( transBackground );
    }
    
    public override void onUnShow()
    {
        clear();
    }

    public void show( GameCampScript s )
    {
        script = s;

        show();

        string path = "Prefab/Camp/Image/camp" + GameDefine.getString2( script.Camp );

        GameObject gameOjbect = Resources.Load<GameObject>( path );
        GameObject obj = Instantiate( gameOjbect , transBackground );
        Transform trans = obj.transform;
        trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.UI = true;
        gameAnimation.offsetX = -(int)( GameCameraManager.instance.sceneWidthHalf - GameCameraManager.instance.xOffset );
        gameAnimation.offsetY = (int)( GameCameraManager.instance.sceneHeightHalf );
        gameAnimation.playAnimation();

        GameCampSelectUI.instance.show( 0 );
        GameCampSelectUI.instance.showFade();
    }

    void onLeave()
    {
        if ( script.Town != GameDefine.INVALID_ID )
        {
            GameUserData.instance.setTown( script.Town );
            GameSceneManager.instance.loadScene( GameSceneType.Rpg , GameSceneLoadMode.CampBack );
        }
        else
        {
            GameUserData.instance.setStage( GameUserData.instance.NextStage );
            GameSceneManager.instance.loadScene( GameSceneType.Battle , GameSceneLoadMode.StartBattle );
        }
    }

    public void leave()
    {
        GameBlackUI.instance.showBlack( 1.0f , onLeave );
    }


}


