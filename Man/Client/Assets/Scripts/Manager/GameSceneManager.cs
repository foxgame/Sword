using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameSceneType
{
    Title = 0,
    Rpg,
    Camp,
    Battle,

    Count
};

public enum GameSceneLoadMode
{
    New,
    Load,
    LoadBattle,

    StartBattle,

    BattleOver,
    CampBack,

    Count
}

public class GameSceneManager : Singleton<GameSceneManager>
{
    bool isLoading = false;

    public bool IsLoading { get { return isLoading; } }

    GameSceneLoadMode mode;

    public GameSceneType SceneType { set; get; }

    public void loadScene( GameSceneType l , GameSceneLoadMode m )
    {
        GameManager.instance.check();

        SceneType = l;

        gameObject.SetActive( true );

        mode = m;

        isLoading = true;

        int index = SceneManager.GetActiveScene().buildIndex;

        GameMusicManager.instance.clearMusic();

        GameUserData.instance.TownPosition = ( mode == GameSceneLoadMode.BattleOver ? 0 : 1 );

        GameTouchCenterUI.instance.unShowUI();

        // release scene
        switch ( index )
        {
            case (int)GameSceneType.Title:
                {
                }
                break;
            case (int)GameSceneType.Rpg:
                {
                }
                break;
            case (int)GameSceneType.Camp:
                {
                }
                break;
            case (int)GameSceneType.Battle:
                {
                }
                break;
        }

#if UNITY_EDITOR
        Debug.Log( "scene load " + l );
#endif

        Resources.UnloadUnusedAssets();

        loadSceneAsync( (int)l );
    }

    void sceneLoaded()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        // load scene
        switch ( index )
        {
            case (int)GameSceneType.Title:
                {
                    GameManager.instance.init();
                }
                break;
            case (int)GameSceneType.Rpg:
                {
                }
                break;
            case (int)GameSceneType.Camp:
                {
                }
                break;
            case (int)GameSceneType.Battle:
                {
                    switch ( mode )
                    {
                        case GameSceneLoadMode.LoadBattle:
                            {
                                GameUserData.instance.loadBattle();                               
                            }
                            break;
                        case GameSceneLoadMode.StartBattle:
                            {
                                GameBattleManager.instance.active();
                                GameBattleManager.instance.showLayer( 1 , false );
                                GameBattleManager.instance.initMusic();

                                GameBattleManager.instance.initTreasures();

                                GameBattleUnitManager.instance.initUnits();

                                GameUserData.instance.clearTempData();

                                GameBattleTurn.instance.start();
                            }
                            break;
                    }
                }
                break;
        }

#if UNITY_EDITOR
        Debug.Log( "scene loaded " + (GameSceneType)index );
#endif

        Resources.UnloadUnusedAssets();

        isLoading = false;
    }

    AsyncOperation async = null;

    public void loadSceneAsync( int s )
    {
        GameManager.instance.StartCoroutine( loadSceneCoroutine( s ) );
    }

    IEnumerator loadSceneCoroutine( int s )
    {
        yield return new WaitForSeconds( 0.1f );

        async = SceneManager.LoadSceneAsync( s , LoadSceneMode.Single );

        while ( !async.isDone )
        {
            yield return new WaitForSeconds( 0.1f );
        }

        async = null;
        sceneLoaded();
    }


}
