using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameSaveDataInfo
{
    public int Stage;
    public int LV;
    public int Proficiency;
    public int ReloadBattleCount;
    public int TurnCount;
    public int Time;
    public int TimeData;
}


public class GameUserData : Singleton<GameUserData>
{
    const int SAVEDAT_SIZE = 3000;

#if SWORD_CLOUD
    byte[][] saveDataCloud = new byte[ GameDefine.MAX_SAVE ][];
#endif

    byte[][] cloudSaveData = new byte[ GameDefine.MAX_SAVE ][];

    [SerializeField]
    GameSaveDataInfo[] saveInfo = new GameSaveDataInfo[ GameDefine.MAX_SAVE ];

    [SerializeField]
    GameSaveDataInfo[] cloudSaveInfo = new GameSaveDataInfo[ GameDefine.MAX_SAVE ];

    [SerializeField]
    int gold = 0;

    [SerializeField]
    short[] itemBag = new short[ GameDefine.MAX_ITEMBAG ];

    [ SerializeField]
    GameUnitBase[] unitBase = new GameUnitBase[ GameDefine.MAX_USER ];
    
    [SerializeField]
    int stage;

    [SerializeField]
    int town;

    [SerializeField]
    int proficiency;

    [SerializeField]
    int reloadBattleCount = 0;
    [SerializeField]
    int reloadBattleCountAll = 0;
    [SerializeField]
    int turnCountAll = 0;

    [SerializeField]
    int time;
    [SerializeField]
    int timeData;

    float timef;

    [SerializeField]
    int nextStage;

    [SerializeField]
    int[] gameData = new int[ GameDefine.MAX_GAMEDATA ];

    public GameSaveDataInfo[] SaveInfo { get { return saveInfo; } }
    public GameSaveDataInfo[] CloudSaveInfo { get { return cloudSaveInfo; } }

    public short[] ItemBag { get { return itemBag; } }

    public int Gold { get { return gold; } }
    public int Stage { get { return stage; } }
    public int NextStage { get { return nextStage; } }
    public int Town { get { return town; } }
    public int TownPosition { get; set; }
    public int Time { get { return time; } }

    public int ProficiencyStage { get; set; }
    public int Proficiency { get { return proficiency; } set { proficiency = value; } }
    public int ReloadBattleCount { get { return reloadBattleCount; } set { reloadBattleCount = value; } }
    public int ReloadBattleCountAll { get { return reloadBattleCountAll; } set { reloadBattleCountAll = value; } }
    public int TurnCountAll { get { return turnCountAll; } set { turnCountAll = value; } }
    public int LastSkillID { set; get; }
    public int LastItemID { set; get; }

    public int LastAttackID
    {
        get
        {
            return GameUserData.instance.getGameData( (int)GameUserDataType.LastAttackID );
        }
        set
        {
            GameUserData.instance.setGameData( (int)GameUserDataType.LastAttackID , value );
        }
    }

    public int LastTargetID
    {
        get
        {
            return GameUserData.instance.getGameData( (int)GameUserDataType.LastTargetID );
        }
        set
        {
            GameUserData.instance.setGameData( (int)GameUserDataType.LastTargetID , value );
        }
    }

    public void addTime( float t )
    {
        timef += t;

        if ( timef > 1.0f )
        {
            time++;
            timef -= 1.0f;
        }
    }

    public void clearTempData()
    {
        for ( int i = 0 ; i < 7 ; i++ )
        {
            gameData[ i ] = 0;
        }

        gameData[ (int)GameUserDataType.LastAttackID ] = GameDefine.INVALID_ID;
        gameData[ (int)GameUserDataType.LastTargetID ] = GameDefine.INVALID_ID;
        gameData[ (int)GameUserDataType.MapEvent6 ] = GameDefine.INVALID_ID;
    }

    public void addGameData( int n , int v )
    {
        gameData[ n ] += v;

#if UNITY_EDITOR
        Debug.Log( "addGameData " + n + " " + v + " " + gameData[ n ] );
#endif
    }
    public void setGameData( int n , int v )
    {
        gameData[ n ] = v;

#if UNITY_EDITOR
        Debug.Log( "setGameData " + n + " " + v );
#endif
    }
    public int getGameData( int n )
    {
#if UNITY_EDITOR
        Debug.Log( "getGameData " + n + " " + gameData[ n ] );
#endif

        return gameData[ n ];
    }

    public void updateInTeam()
    {
        GameInTeamData.Data d = GameInTeamData.instance.getData( stage );

        for ( int i = 0 ; i < d.data.Length ; i++ )
        {
            int ii = d.data[ i ];
            unitBase[ ii ].InTeam = 1;
        }
    }


    public void setTown( int t )
    {
        town = t;
    }

    public void setNextStage( int s )
    {
        nextStage = s;
    }

    public void setStage( int s )
    {
        stage = s;
    }

    public bool inTeam( int id )
    {
        return unitBase[ id ].InTeam > 0;
    }

    public bool itemFull()
    {
        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            if ( itemBag[ i ] == GameDefine.INVALID_ID )
            {
                return false;
            }
        }

        return true;
    }

    public void addItem( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            if ( itemBag[ i ] == GameDefine.INVALID_ID )
            {
                itemBag[ i ] = id;
                return;
            }
        }
    }

    public void removeItem( int slot )
    {
        itemBag[ slot ] = GameDefine.INVALID_ID;

        for ( int i = slot ; i < GameDefine.MAX_ITEMBAG - 1 ; i++ )
        {
            itemBag[ i ] = itemBag[ i + 1 ];
        }

        itemBag[ GameDefine.MAX_ITEMBAG - 1 ] = GameDefine.INVALID_ID;
    }


    public void addGold( int g )
    {
        gold += g;
    }

    public GameUnitBase getUnitBase( int i )
    {
        return unitBase[ i ];
    }


    void copyBytes( ref byte[] des , ref byte[] ori , ref int index )
    {
        for ( int i = 0 ; i < ori.Length ; i++ )
        {
            des[ index ] = ori[ i ]; index++;
        }
    }

    short getShort( ref byte[] b , ref int index )
    {
        short s = BitConverter.ToInt16( b , index );
        index += 2;
        return s;
    }

    int getInt( ref byte[] b , ref int index )
    {
        int n = BitConverter.ToInt32( b , index );
        index += 4;
        return n;
    }

    byte getByte( ref byte[] b , ref int index )
    {
        byte bb = b[ index ];
        index += 1;
        return bb;
    }


    public void save( int n , bool cloud )
    {
#if ( UNITY_ANDROID ) && !UNITY_EDITOR
        if ( PlayerPrefs.GetInt( "Active" , 0 ) == 0 )
        {
            return;
        }
#endif

        timeData = Convert.ToInt32( DateTime.Now.Subtract( DateTime.Parse( "1970-01-01 00:00:00" ).ToLocalTime() ).TotalSeconds );

        byte[] bytes;
        int index = 0;

        if ( cloud )
        {
            cloudSaveData[ n ] = new byte[ SAVEDAT_SIZE ];
            bytes = cloudSaveData[ n ];
        }
        else
        {
#if SWORD_CLOUD
            saveDataCloud[ n ] = new byte[ SAVEDAT_SIZE ];
            bytes = saveDataCloud[ n ];
#else
            bytes = new byte[ SAVEDAT_SIZE ];
#endif
        }


        byte[] b = BitConverter.GetBytes( stage );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( unitBase[ 0 ].LV );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( proficiency );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( reloadBattleCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( turnCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( time );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( timeData );
        copyBytes( ref bytes , ref b , ref index );

        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = 0;
            b = BitConverter.GetBytes( nnn );
            copyBytes( ref bytes , ref b , ref index );
        }


        b = BitConverter.GetBytes( nextStage );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( gold );
        copyBytes( ref bytes , ref b , ref index );

        if ( cloud )
        {
            cloudSaveInfo[ n ].Stage = stage;
            cloudSaveInfo[ n ].Proficiency = proficiency;
            cloudSaveInfo[ n ].ReloadBattleCount = reloadBattleCountAll;
            cloudSaveInfo[ n ].TurnCount = turnCountAll;
            cloudSaveInfo[ n ].LV = unitBase[ 0 ].LV;
            cloudSaveInfo[ n ].Time = time;
            cloudSaveInfo[ n ].TimeData = timeData;
        }
        else
        {
            saveInfo[ n ].Stage = stage;
            saveInfo[ n ].Proficiency = proficiency;
            saveInfo[ n ].ReloadBattleCount = reloadBattleCountAll;
            saveInfo[ n ].TurnCount = turnCountAll;
            saveInfo[ n ].LV = unitBase[ 0 ].LV;
            saveInfo[ n ].Time = time;
            saveInfo[ n ].TimeData = timeData;
        }


        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            b = BitConverter.GetBytes( gameData[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            b = BitConverter.GetBytes( unitBase[ i ].UnitID );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].LV );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Exp );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].HP );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].MP );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].Str );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Int );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Avg );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Vit );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Luk );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].SpiritPower[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            b = BitConverter.GetBytes( unitBase[ i ].Weapon );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Armor );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Accessory );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].InTeam );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].BaseSpiritPower );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Items[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Skills[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }
        }

        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            b = BitConverter.GetBytes( itemBag[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }


        if ( cloud )
        {
            GameSLCloudUI.instance.showLoading();
            GamePHP.instance.phpSave( n + GameDefine.MAX_SAVE , bytes , onCloudSave );
        }
        else
        {
#if SWORD_CLOUD
            GameSLUI.instance.showLoading();
            GamePHP.instance.phpSave( n , bytes , onSaveCloud );
#else
            string path = GameSetting.PersistentDataPath + "/Save" + n + ".dat";

#if UNITY_EDITOR
            Debug.Log( path );
#endif

            FileStream fs = new FileStream( path , FileMode.Create );

            fs.Write( bytes , 0 , bytes.Length );
            fs.Close();
            fs.Dispose();

            bytes = null;
#endif
        }

    }


    public void load( int n , bool cloud )
    {
#if ( UNITY_ANDROID ) && !UNITY_EDITOR
        if ( PlayerPrefs.GetInt( "Active" , 0 ) == 0 )
        {
            return;
        }
#endif

        byte[] bytes = new byte[ SAVEDAT_SIZE ];

        if ( cloud )
        {
            bytes = cloudSaveData[ n ];

            if ( bytes == null )
            {
                return;
            }
        }
        else
        {
#if SWORD_CLOUD
            bytes = saveDataCloud[ n ];

            if ( bytes == null )
            {
                return;
            }
#else
            string path = GameSetting.PersistentDataPath + "/Save" + n + ".dat";

            if ( !File.Exists( path ) )
            {
                return;
            }

            FileStream fs = new FileStream( path , FileMode.Open );

            fs.Read( bytes , 0 , SAVEDAT_SIZE );
            fs.Close();
            fs.Dispose();
#endif
        }



        int index = 0;

        stage = getInt( ref bytes , ref index );
        short lv = getShort( ref bytes , ref index );

        proficiency = getInt( ref bytes , ref index );
        ReloadBattleCount = 0;
        reloadBattleCountAll = getInt( ref bytes , ref index );
        turnCountAll = getInt( ref bytes , ref index );
        time = getInt( ref bytes , ref index );
        timeData = getInt( ref bytes , ref index );
        timef = 0.0f;

        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = getInt( ref bytes , ref index );
        }

        nextStage = getInt( ref bytes , ref index );
        gold = getInt( ref bytes , ref index );

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            gameData[ i ] = getInt( ref bytes , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            unitBase[ i ].UnitID = getShort( ref bytes , ref index );
            unitBase[ i ].LV = getShort( ref bytes , ref index );
            unitBase[ i ].Exp = getShort( ref bytes , ref index );
            unitBase[ i ].HP = getShort( ref bytes , ref index );
            unitBase[ i ].MP = getShort( ref bytes , ref index );

            unitBase[ i ].Str = getShort( ref bytes , ref index );
            unitBase[ i ].Int = getShort( ref bytes , ref index );
            unitBase[ i ].Avg = getShort( ref bytes , ref index );
            unitBase[ i ].Vit = getShort( ref bytes , ref index );
            unitBase[ i ].Luk = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                unitBase[ i ].SpiritPower[ j ] = getShort( ref bytes , ref index );
            }

            unitBase[ i ].Weapon = getShort( ref bytes , ref index );
            unitBase[ i ].Armor = getShort( ref bytes , ref index );
            unitBase[ i ].Accessory = getShort( ref bytes , ref index );

            unitBase[ i ].InTeam = getShort( ref bytes , ref index );
            unitBase[ i ].BaseSpiritPower = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Items[ j ] = getShort( ref bytes , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Skills[ j ] = getShort( ref bytes , ref index );
            }
        }


        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            itemBag[ i ] = getShort( ref bytes , ref index );
        }

        GameSceneManager.instance.loadScene( GameSceneType.Camp , GameSceneLoadMode.Load );
    }



    public void saveBattle()
    {
#if ( UNITY_ANDROID ) && !UNITY_EDITOR
        if ( PlayerPrefs.GetInt( "Active" , 0 ) == 0 )
        {
            return;
        }
#endif

        byte[] bytes = new byte[ 102400 ];
        int index = 0;

        byte[] b = BitConverter.GetBytes( stage );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( unitBase[ 0 ].LV );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( proficiency );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( ProficiencyStage );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( reloadBattleCount );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( reloadBattleCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( turnCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( time );
        copyBytes( ref bytes , ref b , ref index );


        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = 0;
            b = BitConverter.GetBytes( nnn );
            copyBytes( ref bytes , ref b , ref index );
        }


        b = BitConverter.GetBytes( nextStage );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( gold );
        copyBytes( ref bytes , ref b , ref index );

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            b = BitConverter.GetBytes( gameData[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            b = BitConverter.GetBytes( unitBase[ i ].UnitID );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].LV );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Exp );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].HP );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].MP );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].Str );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Int );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Avg );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Vit );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Luk );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].SpiritPower[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            b = BitConverter.GetBytes( unitBase[ i ].Weapon );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Armor );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Accessory );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].InTeam );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].BaseSpiritPower );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Items[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Skills[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }
        }

        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            b = BitConverter.GetBytes( itemBag[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }


        b = BitConverter.GetBytes( (short)GameBattleTurn.instance.Turn );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( (short)GameBattleCursor.instance.PosX );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( (short)GameBattleCursor.instance.PosY );
        copyBytes( ref bytes , ref b , ref index );

        int px = (int)GameCameraManager.instance.PosXReal;
        int py = (int)( GameCameraManager.instance.PosYReal - GameBattleManager.instance.LayerHeight );

        b = BitConverter.GetBytes( px );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( py );
        copyBytes( ref bytes , ref b , ref index );


        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;

        for ( int i = 0 ; i < units.Length ; i++ )
        {
            b = BitConverter.GetBytes( units[ i ].IsShow ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsMoved ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsActed ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsKilled ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsSummon ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsCopy ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsHurt ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );


            b = BitConverter.GetBytes( (short)units[ i ].BattleAIType );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].AIMoveToX );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].AIMoveToY );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].Direction );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].PosX );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].PosY );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( units[ i ].HP );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].MP );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( units[ i ].WeaponID );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].ArmorID );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].AccessoryID );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].MoveBurst );
            copyBytes( ref bytes , ref b , ref index );

            GameUnitBase unitBase = units[ i ].getUnitBase();

            if ( units[ i ].IsSummon || 
                units[ i ].IsCopy )
            {
                b = BitConverter.GetBytes( unitBase.LV );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.HP );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.MP );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Str );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Int );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Avg );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Vit );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Luk );
                copyBytes( ref bytes , ref b , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase.Items[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            List<GameBattleUnitEffect> effects = units[ i ].Effects;

            b = BitConverter.GetBytes( (short)effects.Count );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < effects.Count ; j++ )
            {
                b = BitConverter.GetBytes( (short)effects[ j ].Effect );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( (short)effects[ j ].OtherEffect );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( (short)effects[ j ].Turn );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( effects[ j ].Str );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Int );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Mov );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Vit );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Luk );
                copyBytes( ref bytes , ref b , ref index );
            }
        }



        Dictionary<int , GameBattleMEVT> mapEvent = GameBattleManager.instance.MapEvent;

        b = BitConverter.GetBytes( (short)mapEvent.Count );
        copyBytes( ref bytes , ref b , ref index );

        foreach ( KeyValuePair< int , GameBattleMEVT > kv in mapEvent )
        {
            b = BitConverter.GetBytes( (short)kv.Key );
            copyBytes( ref bytes , ref b , ref index );
        }

#if SWORD_CLOUD
//         byte[] bb = new byte[ index + 4 ];
//         for ( int i = 0 ; i < bb.Length ; i++ )
//             bb[ i ] = bytes[ i ];
//         GamePHP.instance.phpSave( -1 , bb , null );
#endif
        string path = GameSetting.PersistentDataPath + "/SaveBattle.dat";

#if UNITY_EDITOR
        Debug.Log( path );
#endif

        FileStream fs = new FileStream( path , FileMode.Create );

        fs.Write( bytes , 0 , bytes.Length );
        fs.Close();
        fs.Dispose();

        bytes = null;
    }


    void onLoadBattleDebug( int iii , byte[] bytes )
    {
        if ( bytes.Length < 10 )
        {
            return;
        }

        int index = 0;

        stage = getInt( ref bytes , ref index );
        short lv = getShort( ref bytes , ref index );

        proficiency = getInt( ref bytes , ref index );
        ProficiencyStage = getInt( ref bytes , ref index );
        reloadBattleCount = getInt( ref bytes , ref index );
        reloadBattleCountAll = getInt( ref bytes , ref index );
        turnCountAll = getInt( ref bytes , ref index );
        time = getInt( ref bytes , ref index );

        timef = 0.0f;


        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = getInt( ref bytes , ref index );
        }


        nextStage = getInt( ref bytes , ref index );
        gold = getInt( ref bytes , ref index );

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            gameData[ i ] = getInt( ref bytes , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            unitBase[ i ].UnitID = getShort( ref bytes , ref index );
            unitBase[ i ].LV = getShort( ref bytes , ref index );
            unitBase[ i ].Exp = getShort( ref bytes , ref index );
            unitBase[ i ].HP = getShort( ref bytes , ref index );
            unitBase[ i ].MP = getShort( ref bytes , ref index );

            unitBase[ i ].Str = getShort( ref bytes , ref index );
            unitBase[ i ].Int = getShort( ref bytes , ref index );
            unitBase[ i ].Avg = getShort( ref bytes , ref index );
            unitBase[ i ].Vit = getShort( ref bytes , ref index );
            unitBase[ i ].Luk = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                unitBase[ i ].SpiritPower[ j ] = getShort( ref bytes , ref index );
            }

            unitBase[ i ].Weapon = getShort( ref bytes , ref index );
            unitBase[ i ].Armor = getShort( ref bytes , ref index );
            unitBase[ i ].Accessory = getShort( ref bytes , ref index );

            unitBase[ i ].InTeam = getShort( ref bytes , ref index );
            unitBase[ i ].BaseSpiritPower = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Items[ j ] = getShort( ref bytes , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Skills[ j ] = getShort( ref bytes , ref index );
                //                unitBase[ i ].Skills[ j ] = GameUnitInitData.instance.getData( unitBase[ i ].UnitID ).Skills[ j ];
            }
        }


        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            itemBag[ i ] = getShort( ref bytes , ref index );
        }

        clearBattle();
        reloadBattle();

        short turn = getShort( ref bytes , ref index );
        short cx = getShort( ref bytes , ref index );
        short cy = getShort( ref bytes , ref index );
        int rx = getInt( ref bytes , ref index );
        int ry = getInt( ref bytes , ref index );

        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;

        for ( int i = 0 ; i < units.Length ; i++ )
        {
            short isShow = getShort( ref bytes , ref index );
            short isMoved = getShort( ref bytes , ref index );
            short isActed = getShort( ref bytes , ref index );
            short isKilled = getShort( ref bytes , ref index );
            short isSummon = getShort( ref bytes , ref index );
            short isCopy = getShort( ref bytes , ref index );
            short isHurt = getShort( ref bytes , ref index );

            short aiType = getShort( ref bytes , ref index );
            short aiMoveX = getShort( ref bytes , ref index );
            short aiMoveY = getShort( ref bytes , ref index );

            short direction = getShort( ref bytes , ref index );

            short x = getShort( ref bytes , ref index );
            short y = getShort( ref bytes , ref index );

            int hp = getInt( ref bytes , ref index );
            int mp = getInt( ref bytes , ref index );

            short weaponID = getShort( ref bytes , ref index );
            short armorID = getShort( ref bytes , ref index );
            short accessoryID = getShort( ref bytes , ref index );

            short mb = getShort( ref bytes , ref index );


            if ( weaponID != units[ i ].WeaponID )
            {
                if ( units[ i ].WeaponSlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].WeaponSlot );
                }

                units[ i ].addItem( weaponID );
                units[ i ].WeaponID = weaponID;
            }
            if ( armorID != units[ i ].ArmorID )
            {
                if ( units[ i ].ArmorSlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].ArmorSlot );
                }

                units[ i ].addItem( armorID );
                units[ i ].ArmorID = armorID;
            }
            if ( accessoryID != units[ i ].AccessoryID )
            {
                if ( units[ i ].AccessorySlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].AccessorySlot );
                }

                units[ i ].addItem( accessoryID );
                units[ i ].AccessoryID = accessoryID;
            }

            GameUnitBase unitBase = units[ i ].getUnitBase();

            if ( isSummon == 1 || isCopy == 1 )
            {
                unitBase.LV = getShort( ref bytes , ref index );
                unitBase.HP = getInt( ref bytes , ref index );
                unitBase.MP = getInt( ref bytes , ref index );
                unitBase.Str = getShort( ref bytes , ref index );
                unitBase.Int = getShort( ref bytes , ref index );
                unitBase.Avg = getShort( ref bytes , ref index );
                unitBase.Vit = getShort( ref bytes , ref index );
                unitBase.Luk = getShort( ref bytes , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase.Items[ j ] = getShort( ref bytes , ref index );
            }

            short ec = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < ec ; j++ )
            {
                GameBattleUnitEffect effect = new GameBattleUnitEffect();

                effect.Effect = (GameSkillResutlEffect)getShort( ref bytes , ref index );
                effect.OtherEffect = (GameSkillOtherEffect)getShort( ref bytes , ref index );

                effect.Turn = getShort( ref bytes , ref index );

                effect.Str = getShort( ref bytes , ref index );
                effect.Int = getShort( ref bytes , ref index );
                effect.Mov = getShort( ref bytes , ref index );
                effect.Vit = getShort( ref bytes , ref index );
                effect.Luk = getShort( ref bytes , ref index );

                units[ i ].addEffect( effect );
            }

            if ( units[ i ].IsUser && (GameBattleAIType)aiType != GameBattleAIType.None )
            {
                GameBattleUnitManager.instance.removeUser( units[ i ] );
                GameBattleUnitManager.instance.addNpc( units[ i ] );
            }

            units[ i ].BattleAIType = (GameBattleAIType)aiType;
            units[ i ].AIMoveToX = aiMoveX;
            units[ i ].AIMoveToY = aiMoveY;

            units[ i ].IsShow = ( isShow == 1 );
            units[ i ].IsMoved = ( isMoved == 1 );
            units[ i ].IsActed = ( isActed == 1 );
            units[ i ].IsKilled = ( isKilled == 1 );
            units[ i ].IsSummon = ( isSummon == 1 );
            units[ i ].IsCopy = ( isCopy == 1 );
            units[ i ].IsHurt = ( isHurt == 1 );


            units[ i ].updateUnitData();

            units[ i ].initHPMP( hp , mp );
            units[ i ].setPos( x , y );
            units[ i ].setMoveBurst( mb );
            units[ i ].updateAnimation( (GameAnimationDirection)direction );
            units[ i ].updateAlive();

        }

        short tc = getShort( ref bytes , ref index );
        for ( int i = 0 ; i < tc ; i++ )
        {
            short id = getShort( ref bytes , ref index );

            GameBattleManager.instance.addTreasure( id );
        }

        GameBattleManager.instance.updateTreasures();

        GameBattleTurn.instance.start( turn );

        GameCameraManager.instance.setPos( rx , ry + GameBattleManager.instance.LayerHeight );

        GameBattleCursor.instance.show( 0 );
        GameBattleCursor.instance.moveTo( cx , cy , true );

        GameBattleCursor.instance.updatePosition();

        GameBlackUI.instance.unShowBlack( 1 , null );

        GameTouchCenterUI.instance.showUI();

        Debug.Log( "GameBattleUnitManager.instance.IsEnemyAI " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitManager.instance.IsNpcAI " + getByte( ref bytes , ref index ) );

        Debug.Log( "GameBattleJudgment.instance.IsLose " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleJudgment.instance.IsWin " + getByte( ref bytes , ref index ) );

        Debug.Log( "GameBattleEventManager.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleInput.instance.Pause " + getByte( ref bytes , ref index ) );

        Debug.Log( "GameBattleTurn.instance.UserTurn " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleTurn.instance.EnemyTurn " + getByte( ref bytes , ref index ) );

        Debug.Log( "GameBattleCursor.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleGetItemUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleInformationUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleItemUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleSkillTitleUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleSkillUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleSystemSLUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleSystemUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleSystemSoundUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleTitleUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitMovement.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitActionItemUI.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitSkillSelection.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitItemSelection.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitItemSelection.instance.IsShow " + getByte( ref bytes , ref index ) );
        Debug.Log( "GameBattleUnitAttackSelection.instance.IsShow " + getByte( ref bytes , ref index ) );

        Debug.Log( "LastItemID " + getInt( ref bytes , ref index ) );
        Debug.Log( "LastSkillID " + getInt( ref bytes , ref index ) );

        bytes = null;

    }

    public void loadBattleDebug( int i )
    {
        GamePHP.instance.phpLoadIndex( i , onLoadBattleDebug );
    }

    public void saveBattleDebug( int bug )
    {
        if ( GameSceneManager.instance.SceneType != GameSceneType.Battle )
        {
            return;
        }

        byte[] bytes = new byte[ 102400 ];
        int index = 0;

        byte[] b = BitConverter.GetBytes( stage );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( unitBase[ 0 ].LV );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( proficiency );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( ProficiencyStage );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( reloadBattleCount );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( reloadBattleCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( turnCountAll );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( time );
        copyBytes( ref bytes , ref b , ref index );


        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = 0;
            b = BitConverter.GetBytes( nnn );
            copyBytes( ref bytes , ref b , ref index );
        }


        b = BitConverter.GetBytes( nextStage );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( gold );
        copyBytes( ref bytes , ref b , ref index );

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            b = BitConverter.GetBytes( gameData[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            b = BitConverter.GetBytes( unitBase[ i ].UnitID );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].LV );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Exp );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].HP );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)unitBase[ i ].MP );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].Str );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Int );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Avg );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Vit );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Luk );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].SpiritPower[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            b = BitConverter.GetBytes( unitBase[ i ].Weapon );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Armor );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].Accessory );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( unitBase[ i ].InTeam );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( unitBase[ i ].BaseSpiritPower );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Items[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase[ i ].Skills[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }
        }

        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            b = BitConverter.GetBytes( itemBag[ i ] );
            copyBytes( ref bytes , ref b , ref index );
        }


        b = BitConverter.GetBytes( (short)GameBattleTurn.instance.Turn );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( (short)GameBattleCursor.instance.PosX );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( (short)GameBattleCursor.instance.PosY );
        copyBytes( ref bytes , ref b , ref index );

        int px = (int)GameCameraManager.instance.PosXReal;
        int py = (int)( GameCameraManager.instance.PosYReal - GameBattleManager.instance.LayerHeight );

        b = BitConverter.GetBytes( px );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( py );
        copyBytes( ref bytes , ref b , ref index );


        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;

        for ( int i = 0 ; i < units.Length ; i++ )
        {
            b = BitConverter.GetBytes( units[ i ].IsShow ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsMoved ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsActed ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsKilled ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsSummon ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsCopy ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].IsHurt ? (short)1 : (short)0 );
            copyBytes( ref bytes , ref b , ref index );


            b = BitConverter.GetBytes( (short)units[ i ].BattleAIType );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].AIMoveToX );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].AIMoveToY );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].Direction );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].PosX );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( (short)units[ i ].PosY );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( units[ i ].HP );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].MP );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( units[ i ].WeaponID );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].ArmorID );
            copyBytes( ref bytes , ref b , ref index );
            b = BitConverter.GetBytes( units[ i ].AccessoryID );
            copyBytes( ref bytes , ref b , ref index );

            b = BitConverter.GetBytes( (short)units[ i ].MoveBurst );
            copyBytes( ref bytes , ref b , ref index );

            GameUnitBase unitBase = units[ i ].getUnitBase();

            if ( units[ i ].IsSummon ||
                units[ i ].IsCopy )
            {
                b = BitConverter.GetBytes( unitBase.LV );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.HP );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.MP );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Str );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Int );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Avg );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Vit );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( unitBase.Luk );
                copyBytes( ref bytes , ref b , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                b = BitConverter.GetBytes( unitBase.Items[ j ] );
                copyBytes( ref bytes , ref b , ref index );
            }

            List<GameBattleUnitEffect> effects = units[ i ].Effects;

            b = BitConverter.GetBytes( (short)effects.Count );
            copyBytes( ref bytes , ref b , ref index );

            for ( int j = 0 ; j < effects.Count ; j++ )
            {
                b = BitConverter.GetBytes( (short)effects[ j ].Effect );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( (short)effects[ j ].OtherEffect );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( (short)effects[ j ].Turn );
                copyBytes( ref bytes , ref b , ref index );

                b = BitConverter.GetBytes( effects[ j ].Str );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Int );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Mov );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Vit );
                copyBytes( ref bytes , ref b , ref index );
                b = BitConverter.GetBytes( effects[ j ].Luk );
                copyBytes( ref bytes , ref b , ref index );
            }
        }

        Dictionary<int , GameBattleMEVT> mapEvent = GameBattleManager.instance.MapEvent;

        b = BitConverter.GetBytes( (short)mapEvent.Count );
        copyBytes( ref bytes , ref b , ref index );

        foreach ( KeyValuePair<int , GameBattleMEVT> kv in mapEvent )
        {
            b = BitConverter.GetBytes( (short)kv.Key );
            copyBytes( ref bytes , ref b , ref index );
        }

        b = BitConverter.GetBytes( GameBattleUnitManager.instance.IsEnemyAI );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitManager.instance.IsNpcAI );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( GameBattleJudgment.instance.IsLose );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleJudgment.instance.IsWin );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( GameBattleEventManager.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleInput.instance.Pause );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( GameBattleTurn.instance.UserTurn );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleTurn.instance.EnemyTurn );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( GameBattleCursor.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleGetItemUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleInformationUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleItemUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleSkillTitleUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleSkillUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleSystemSLUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleSystemUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleSystemSoundUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleTitleUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitMovement.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitActionItemUI.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitSkillSelection.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitItemSelection.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( GameBattleUnitAttackSelection.instance.IsShow );
        copyBytes( ref bytes , ref b , ref index );

        b = BitConverter.GetBytes( LastItemID );
        copyBytes( ref bytes , ref b , ref index );
        b = BitConverter.GetBytes( LastSkillID );
        copyBytes( ref bytes , ref b , ref index );


        byte[] bb = new byte[ index + 4 ];
        for ( int i = 0 ; i < bb.Length ; i++ )
            bb[ i ] = bytes[ i ];
        GamePHP.instance.phpSave( bug , bb , null );

        bytes = null;
    }

    public void loadBattle()
    {
#if UNITY_EDITOR
//         loadBattleDebug( 79 );
//         return;
#endif
#if ( UNITY_ANDROID ) && !UNITY_EDITOR
        if ( PlayerPrefs.GetInt( "Active" , 0 ) == 0 )
        {
            return;
        }
#endif

        string path = GameSetting.PersistentDataPath + "/SaveBattle.dat";

        if ( !File.Exists( path ) )
        {
            return;
        }

        FileStream fs = new FileStream( path , FileMode.Open );

        byte[] bytes = new byte[ 102400 ];

        fs.Read( bytes , 0 , 102400 );
        fs.Close();
        fs.Dispose();


        int index = 0;

        stage = getInt( ref bytes , ref index );
        short lv = getShort( ref bytes , ref index );

        proficiency = getInt( ref bytes , ref index );
        ProficiencyStage = getInt( ref bytes , ref index );
        reloadBattleCount = getInt( ref bytes , ref index );
        reloadBattleCountAll = getInt( ref bytes , ref index );
        turnCountAll = getInt( ref bytes , ref index );
        time = getInt( ref bytes , ref index );

        timef = 0.0f;


        for ( int i = 0 ; i < 10 ; i++ )
        {
            int nnn = getInt( ref bytes , ref index );
        }


        nextStage = getInt( ref bytes , ref index );
        gold = getInt( ref bytes , ref index );

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            gameData[ i ] = getInt( ref bytes , ref index );
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            unitBase[ i ].UnitID = getShort( ref bytes , ref index );
            unitBase[ i ].LV = getShort( ref bytes , ref index );
            unitBase[ i ].Exp = getShort( ref bytes , ref index );
            unitBase[ i ].HP = getShort( ref bytes , ref index );
            unitBase[ i ].MP = getShort( ref bytes , ref index );

            unitBase[ i ].Str = getShort( ref bytes , ref index );
            unitBase[ i ].Int = getShort( ref bytes , ref index );
            unitBase[ i ].Avg = getShort( ref bytes , ref index );
            unitBase[ i ].Vit = getShort( ref bytes , ref index );
            unitBase[ i ].Luk = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                unitBase[ i ].SpiritPower[ j ] = getShort( ref bytes , ref index );
            }

            unitBase[ i ].Weapon = getShort( ref bytes , ref index );
            unitBase[ i ].Armor = getShort( ref bytes , ref index );
            unitBase[ i ].Accessory = getShort( ref bytes , ref index );

            unitBase[ i ].InTeam = getShort( ref bytes , ref index );
            unitBase[ i ].BaseSpiritPower = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Items[ j ] = getShort( ref bytes , ref index );
            }
            
            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Skills[ j ] = getShort( ref bytes , ref index );
//                unitBase[ i ].Skills[ j ] = GameUnitInitData.instance.getData( unitBase[ i ].UnitID ).Skills[ j ];
            }
        }


        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            itemBag[ i ] = getShort( ref bytes , ref index );
        }

        clearBattle();
        reloadBattle();

        short turn = getShort( ref bytes , ref index );
        short cx = getShort( ref bytes , ref index );
        short cy = getShort( ref bytes , ref index );
        int rx = getInt( ref bytes , ref index );
        int ry = getInt( ref bytes , ref index );

        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;

        for ( int i = 0 ; i < units.Length ; i++ )
        {
            short isShow = getShort( ref bytes , ref index );
            short isMoved = getShort( ref bytes , ref index );
            short isActed = getShort( ref bytes , ref index );
            short isKilled = getShort( ref bytes , ref index );
            short isSummon = getShort( ref bytes , ref index );
            short isCopy = getShort( ref bytes , ref index );
            short isHurt = getShort( ref bytes , ref index );

            short aiType = getShort( ref bytes , ref index );
            short aiMoveX = getShort( ref bytes , ref index );
            short aiMoveY = getShort( ref bytes , ref index );

            short direction = getShort( ref bytes , ref index );

            short x = getShort( ref bytes , ref index );
            short y = getShort( ref bytes , ref index );

            int hp = getInt( ref bytes , ref index );
            int mp = getInt( ref bytes , ref index );

            short weaponID = getShort( ref bytes , ref index );
            short armorID = getShort( ref bytes , ref index );
            short accessoryID = getShort( ref bytes , ref index );

            short mb = getShort( ref bytes , ref index );


            if ( weaponID != units[ i ].WeaponID )
            {
                if ( units[ i ].WeaponSlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].WeaponSlot );
                }

                units[ i ].addItem( weaponID );
                units[ i ].WeaponID = weaponID;
            }
            if ( armorID != units[ i ].ArmorID )
            {
                if ( units[ i ].ArmorSlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].ArmorSlot );
                }

                units[ i ].addItem( armorID );
                units[ i ].ArmorID = armorID;
            }
            if ( accessoryID != units[ i ].AccessoryID )
            {
                if ( units[ i ].AccessorySlot != GameDefine.INVALID_ID )
                {
                    units[ i ].removeItem( units[ i ].AccessorySlot );
                }

                units[ i ].addItem( accessoryID );
                units[ i ].AccessoryID = accessoryID;
            }

            GameUnitBase unitBase = units[ i ].getUnitBase();

            if ( isSummon == 1 || isCopy == 1 )
            {
                unitBase.LV = getShort( ref bytes , ref index );
                unitBase.HP = getInt( ref bytes , ref index );
                unitBase.MP = getInt( ref bytes , ref index );
                unitBase.Str = getShort( ref bytes , ref index );
                unitBase.Int = getShort( ref bytes , ref index );
                unitBase.Avg = getShort( ref bytes , ref index );
                unitBase.Vit = getShort( ref bytes , ref index );
                unitBase.Luk = getShort( ref bytes , ref index );
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase.Items[ j ] = getShort( ref bytes , ref index );
            }

            short ec = getShort( ref bytes , ref index );

            for ( int j = 0 ; j < ec ; j++ )
            {
                GameBattleUnitEffect effect = new GameBattleUnitEffect();

                effect.Effect = (GameSkillResutlEffect)getShort( ref bytes , ref index );
                effect.OtherEffect = (GameSkillOtherEffect)getShort( ref bytes , ref index );

                effect.Turn = getShort( ref bytes , ref index );

                effect.Str = getShort( ref bytes , ref index );
                effect.Int = getShort( ref bytes , ref index );
                effect.Mov = getShort( ref bytes , ref index );
                effect.Vit = getShort( ref bytes , ref index );
                effect.Luk = getShort( ref bytes , ref index );

                units[ i ].addEffect( effect );
            }

            if ( units[ i ].IsUser && (GameBattleAIType)aiType != GameBattleAIType.None )
            {
                GameBattleUnitManager.instance.removeUser( units[ i ] );
                GameBattleUnitManager.instance.addNpc( units[ i ] );
            }

            units[ i ].BattleAIType = (GameBattleAIType)aiType;
            units[ i ].AIMoveToX = aiMoveX;
            units[ i ].AIMoveToY = aiMoveY;

            units[ i ].IsShow = ( isShow == 1 );
            units[ i ].IsMoved = ( isMoved == 1 );
            units[ i ].IsActed = ( isActed == 1 );
            units[ i ].IsKilled = ( isKilled == 1 );
            units[ i ].IsSummon = ( isSummon == 1 );
            units[ i ].IsCopy = ( isCopy == 1 );
            units[ i ].IsHurt = ( isHurt == 1 );

            units[ i ].updateUnitData();

            if ( isSummon == 1 )
            {
                units[ i ].clearMove();
            }

            units[ i ].initHPMP( hp , mp );
            units[ i ].setPos( x , y );
            units[ i ].setMoveBurst( mb );
            units[ i ].updateAnimation( (GameAnimationDirection)direction );
            units[ i ].updateAlive();

        }

        short tc = getShort( ref bytes , ref index );
        for ( int i = 0 ; i < tc ; i++ )
        {
            short id = getShort( ref bytes , ref index );

            GameBattleManager.instance.addTreasure( id );
        }

        bytes = null;

        GameBattleManager.instance.updateTreasures();

        GameBattleTurn.instance.start( turn );

        GameCameraManager.instance.setPos( rx , ry + GameBattleManager.instance.LayerHeight );

        GameBattleCursor.instance.show( 0 );
        GameBattleCursor.instance.moveTo( cx , cy , true );

        GameBattleCursor.instance.updatePosition();

        GameBlackUI.instance.unShowBlack( 1 , null );

        GameTouchCenterUI.instance.showUI();
    }

    public bool hasItem( short id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            if ( itemBag[ i ] == id )
            {
                return true;
            }
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            if ( unitBase[ i ].hasItem( id ) )
            {
                return true;
            }
        }

        return false;
    }

    public bool haveItem( short item )
    {
        for ( int i = 0 ; i < itemBag.Length ; i++ )
        {
            if ( ItemBag[ i ] == item )
            {
                return true;
            }
        }

        for ( int i = 0 ; i < unitBase.Length ; i++ )
        {
            if ( unitBase[ i ].hasItem( item ) )
            {
                return true;
            }
        }

        return false;
    }


    public short getAlchemyItem( short item1 , short item2 )
    {
        GameItem[] data = GameItemData.instance.Data;

        for ( int i = 0 ; i < data.Length ; i++ )
        {
            if ( ( data[ i ].SynthesizeItem1 == item1 && data[ i ].SynthesizeItem2 == item2 ) ||
                ( data[ i ].SynthesizeItem2 == item1 && data[ i ].SynthesizeItem1 == item2 ) )
            {
                return (short)i;
            }
        }

        return (short)GameDefine.INVALID_ID;
    }

#if SWORD_CLOUD

    void onSaveCloud( int i , byte[] bytes )
    {
        GameSLUI.instance.showLoadingComplete();
    }

    void onLoadCloud( int i , byte[] bytes )
    {
        if ( bytes.Length != SAVEDAT_SIZE )
        {
            return;
        }

        saveDataCloud[ i ] = bytes;

        int index = 0;

        saveInfo[ i ] = new GameSaveDataInfo();
        saveInfo[ i ].Stage = getInt( ref bytes , ref index );
        saveInfo[ i ].LV = getShort( ref bytes , ref index );
        saveInfo[ i ].Proficiency = getInt( ref bytes , ref index );
        saveInfo[ i ].ReloadBattleCount = getInt( ref bytes , ref index );
        saveInfo[ i ].TurnCount = getInt( ref bytes , ref index );
        saveInfo[ i ].Time = getInt( ref bytes , ref index );
        saveInfo[ i ].TimeData = getInt( ref bytes , ref index );

        if ( GameSLUI.instance.IsShow )
            GameSLUI.instance.updateData();
    }
#endif

    void onCloudSave( int i , byte[] bytes )
    {
        GameSLCloudUI.instance.showLoadingComplete();
    }

    void onCloudLoad( int i , byte[] bytes )
    {
        if ( i == 9 )
        {
            GameSLCloudUI.instance.showLoadingComplete();
        }

        if ( bytes.Length != SAVEDAT_SIZE )
        {
            return;
        }

        i -= GameDefine.MAX_SAVE;

        cloudSaveData[ i ] = bytes;

        int index = 0;

        cloudSaveInfo[ i ] = new GameSaveDataInfo();
        cloudSaveInfo[ i ].Stage = getInt( ref bytes , ref index );
        cloudSaveInfo[ i ].LV = getShort( ref bytes , ref index );
        cloudSaveInfo[ i ].Proficiency = getInt( ref bytes , ref index );
        cloudSaveInfo[ i ].ReloadBattleCount = getInt( ref bytes , ref index );
        cloudSaveInfo[ i ].TurnCount = getInt( ref bytes , ref index );
        cloudSaveInfo[ i ].Time = getInt( ref bytes , ref index );
        cloudSaveInfo[ i ].TimeData = getInt( ref bytes , ref index );

        if ( GameSLCloudUI.instance.IsShow )
            GameSLCloudUI.instance.updateData();
    }

    public void loadCloudSaveInfo( string id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            cloudSaveInfo[ i ].Stage = 0;
            cloudSaveInfo[ i ].LV = 0;
            cloudSaveInfo[ i ].Proficiency = 0;
            cloudSaveInfo[ i ].ReloadBattleCount = 0;
            cloudSaveInfo[ i ].TurnCount = 0;
            cloudSaveInfo[ i ].Time = 0;
            cloudSaveInfo[ i ].TimeData = 0;

#if ( UNITY_ANDROID ) && !UNITY_EDITOR
            GamePHP.instance.phpActive( null );
#endif
            GameSLCloudUI.instance.showLoading();
            GamePHP.instance.phpLoadCloud( id , i + GameDefine.MAX_SAVE , onCloudLoad );
        }

    }

    public void loadSaveInfo()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SAVE ; i++ )
        {
            saveInfo[ i ].Stage = 0;
            saveInfo[ i ].LV = 0;
            saveInfo[ i ].Proficiency = 0;
            saveInfo[ i ].ReloadBattleCount = 0;
            saveInfo[ i ].TurnCount = 0;
            saveInfo[ i ].Time = 0;
            saveInfo[ i ].TimeData = 0;

#if ( UNITY_ANDROID ) && !UNITY_EDITOR
            GamePHP.instance.phpActive( null );
#endif

#if SWORD_CLOUD
            GamePHP.instance.phpLoad( i , onLoadCloud );
#else

            string path = GameSetting.PersistentDataPath + "/Save" + i + ".dat";

            if ( !File.Exists( path ) )
            {
                

                continue;
            }

            FileStream fs = new FileStream( path , FileMode.Open );

            byte[] bytes = new byte[ 32 ];

            fs.Read( bytes , 0 , 32 );
            fs.Close();
            fs.Dispose();

            int index = 0;

            saveInfo[ i ] = new GameSaveDataInfo();
            saveInfo[ i ].Stage = getInt( ref bytes , ref index );
            saveInfo[ i ].LV = getShort( ref bytes , ref index );
            saveInfo[ i ].Proficiency = getInt( ref bytes , ref index );
            saveInfo[ i ].ReloadBattleCount = getInt( ref bytes , ref index );
            saveInfo[ i ].TurnCount = getInt( ref bytes , ref index );
            saveInfo[ i ].Time = getInt( ref bytes , ref index );
            saveInfo[ i ].TimeData = getInt( ref bytes , ref index );
#endif


        }  

    }


    public void clearBattle()
    {
        ProficiencyStage = 0;

        GameBattleJudgment.instance.clear();

        GameBattleUnitAttackSelection.instance.clear();
        GameBattleUnitItemSelection.instance.clear();
        GameBattleUnitSkillSelection.instance.clear();
        GameBattleUnitMovement.instance.clear();

        GameBattleUnitManager.instance.clear();
        GameBattleManager.instance.clear();
        GameBattleEventManager.instance.clear();

        GameBattleCursor.instance.unShow();

        Resources.UnloadUnusedAssets();
    }

    void reloadBattle()
    {
        GameBattleManager.instance.active();

        GameBattleManager.instance.showLayer( 1 , true );
        GameBattleManager.instance.initMusic();

        GameBattleUnitManager.instance.initUnits();
    }


    public int firstUser()
    {
        return unitBase[ 0 ].UnitID;
    }

    public int nextUser( int id )
    {
        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            if ( unitBase[ i ].UnitID > id && unitBase[ i ].InTeam > 0 )
            {
                return unitBase[ i ].UnitID;
            }
        }

        return GameDefine.INVALID_ID;
    }

    public int lastUser()
    {
        for ( int i = GameDefine.MAX_USER - 1 ; i >= 0 ; i-- )
        {
            if ( unitBase[ i ].InTeam > 0 )
            {
                return unitBase[ i ].UnitID;
            }
        }

        return GameDefine.INVALID_ID;
    }

    public void init()
    {
//        stage = 1;

        loadSaveInfo();

        gold = 0;
        stage = 0;
        town = 0;
        proficiency = 0;
        reloadBattleCount = 0;
        reloadBattleCountAll = 0;
        turnCountAll = 0;
        nextStage = 0;
        time = 0;
        timeData = 0;
        timef = 0.0f;

        ProficiencyStage = 0;

        for ( int i = 0 ; i < GameDefine.MAX_ITEMBAG ; i++ )
        {
            itemBag[ i ] = GameDefine.INVALID_ID;
        }

        for ( int i = 0 ; i < GameDefine.MAX_GAMEDATA ; i++ )
        {
            gameData[ i ] = 0;
        }

        for ( int i = 0 ; i < GameDefine.MAX_USER ; i++ )
        {
            unitBase[ i ] = new GameUnitBase();

            GameUnitBase b = GameUnitInitData.instance.getData( i );

            unitBase[ i ].UnitID = b.UnitID;
            unitBase[ i ].LV = b.LV;
            unitBase[ i ].Exp = b.Exp;
            unitBase[ i ].HP = b.HP;
            unitBase[ i ].MP = b.MP;
            unitBase[ i ].Str = b.Str;
            unitBase[ i ].Int = b.Int;
            unitBase[ i ].Avg = b.Avg;
            unitBase[ i ].Vit = b.Vit;
            unitBase[ i ].Luk = b.Luk;

            unitBase[ i ].Weapon = b.Weapon;
            unitBase[ i ].Armor = b.Armor;
            unitBase[ i ].Accessory = b.Accessory;

            unitBase[ i ].InTeam = b.InTeam;
            unitBase[ i ].BaseSpiritPower = b.BaseSpiritPower;

            for ( int j = 0 ; j < (int)GameSpiritType.Count ; j++ )
            {
                unitBase[ i ].SpiritPower[ j ] = b.SpiritPower[ j ];
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Items[ j ] = b.Items[ j ];
            }

            for ( int j = 0 ; j < GameDefine.MAX_SLOT ; j++ )
            {
                unitBase[ i ].Skills[ j ] = b.Skills[ j ];
            }

    //             int jj = -1;
    //             for ( int j = 0 ; j < GameDefine.MAX_USER ; j++ )
    //             {
    //                 if ( unitBase[ i ].Skills[ j ] != GameDefine.INVALID_ID )
    //                 {
    //                     jj = j;
    //                 }
    //             }
    // 
    //             for ( int j = 0 ; j < GameDefine.MAX_USER ; j++ )
    //             {
    //                 if ( GameUnitLevelUpData.instance.getData( i ).Skill[ j ].SkillID != GameDefine.INVALID_ID )
    //                 {
    //                     jj++;
    //                     if ( jj >= GameDefine.MAX_USER )
    //                     {
    //                         break;
    //                     }
    //                     unitBase[ i ].Skills[ jj ] = GameUnitLevelUpData.instance.getData( i ).Skill[ j ].SkillID;
    //                 }
    //             }
        }

        gold = 5000;

        unitBase[ 0 ].InTeam = 1;
//         unitBase[ 0 ].HP = 9999;
//         unitBase[ 0 ].MP = 9999;
//         unitBase[ 0 ].Str = 999;
//         unitBase[ 0 ].Vit = 999;
//         unitBase[ 0 ].Avg = 999;
//         unitBase[ 0 ].Luk = 999;
//         unitBase[ 0 ].Int = 999;

    }


}


