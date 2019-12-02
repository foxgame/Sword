using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

public class GameToolsMenu
{



    [MenuItem( "GameTools/Data/LoadConfig" )]
    static void GameToolsDataLoadConfig()
    {
        GameItemData itemData = UnityEngine.Object.FindObjectOfType<GameItemData>();
        itemData.load( Application.dataPath + "/Objects/DAT/Misc/Item_dat.dat" );

        GameSkillData skillData = UnityEngine.Object.FindObjectOfType<GameSkillData>();
        skillData.load( Application.dataPath + "/Objects/DAT/Misc/Mag_dat.dat" );

        GameAttributeDefenceData attributeDefenceData = UnityEngine.Object.FindObjectOfType<GameAttributeDefenceData>();
        attributeDefenceData.load( Application.dataPath + "/Objects/DAT/Misc/MAG_OPP.dat" );

        GameUnitData unitData = UnityEngine.Object.FindObjectOfType<GameUnitData>();
        unitData.load( Application.dataPath + "/Objects/DAT/Misc/man_base.dat" );

        GameUnitLevelUpData unitLevelUpData = UnityEngine.Object.FindObjectOfType<GameUnitLevelUpData>();
        unitLevelUpData.load( Application.dataPath + "/Objects/DAT/Misc/KMAN_LEV.dat" );

        GameUnitInitData unitInitData = UnityEngine.Object.FindObjectOfType<GameUnitInitData>();
        unitInitData.load( Application.dataPath + "/Objects/DAT/Misc/KMAN_STA.dat" );

        GameMessageData messageData = UnityEngine.Object.FindObjectOfType<GameMessageData>();
        messageData.clear();
        
        messageData.load( Application.dataPath + "/Objects/DAT/Misc/CSSCON.MSG" );
        messageData.load( Application.dataPath + "/Objects/DAT/Misc/GITEPRO.MSG" );
        messageData.load( Application.dataPath + "/Objects/DAT/Misc/MLEVPRO.MSG" );
        messageData.load( Application.dataPath + "/Objects/DAT/Misc/Respro.MSG" );
        messageData.load( Application.dataPath + "/Objects/DAT/Misc/TREPRO.MSG" );


        GameBattleData battleData = UnityEngine.Object.FindObjectOfType<GameBattleData>();
        battleData.loadStartData( Application.dataPath + "/Objects/DAT/Misc/STA_SCR.DAT" );


        GameStringData stringData = UnityEngine.Object.FindObjectOfType<GameStringData>();
        stringData.load();

        GameUnitMoveTypeData unitMoveTypeData = UnityEngine.Object.FindObjectOfType<GameUnitMoveTypeData>();
        unitMoveTypeData.load();

        GameCampData campData = UnityEngine.Object.FindObjectOfType<GameCampData>();

        for ( int i = 0 ; i < GameDefine.MAX_STAGE ; i++ )
        {
            campData.loadScript( i );
        }
    }

    [MenuItem( "GameTools/Data/LoadBattleConfig" )]
    static void GameToolsDataLoadBattleConfig()
    {
        GameBattleData battleData = UnityEngine.Object.FindObjectOfType<GameBattleData>();

        for ( int i = 0 ; i < GameDefine.MAX_STAGE ; i++ )
        {
            battleData.load( i );
        }
    }

    [MenuItem( "GameTools/Data/LoadRPGConfig" )]
    static void GameToolsDataLoadRPGConfig()
    {
        GameRPGData rpgData = UnityEngine.Object.FindObjectOfType<GameRPGData>();

        for ( int i = 0 ; i < GameDefine.MAX_RPG_STAGE ; i++ )
        {
            rpgData.load( i );
        }
    }

    [MenuItem( "GameTools/Animation/CreatePrefab" ) ]
	static void GameToolsAnimationCreatePrefab()
	{
//         if ( GameObject.Find( "testAnimations" ) != null )
//             GameObject.DestroyImmediate( GameObject.Find( "testAnimations" ) );
// 
//         GameObject objTest = new GameObject( "testAnimations" );
//         RectTransform trans = objTest.AddComponent<RectTransform>();
//         trans.parent = GameObject.Find( "Canvas" ).transform;
//         trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );
//         trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );


//        List<string> objTest2 = new List<string>();

        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        GameObject[] objs = new GameObject[ arr.Length ];

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = Path.GetDirectoryName( AssetDatabase.GetAssetPath( arr[ i ] ) );
            pathDir = pathDir.Replace( '\\' , '/' );

            int n1 = pathDir.IndexOf( "Objects/" );

//             string pathDir2 = pathDir.Substring( n1 + 8 , pathDir.Length - n1 - 8 );
// 
//             if ( objTest2.Find( o => o == pathDir2 ) == null )
//             {
//                 GameObject obj2 = new GameObject( pathDir2 );
//                 trans = obj2.AddComponent<RectTransform>();
//                 trans.parent = objTest.transform;
//                 trans.localPosition = new Vector3( 0.0f , 400 + objTest2.Count * 15 , 0.0f );
//                 trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
// 
//                 objTest2.Add( pathDir2 );
//            }

            string path = Path.ChangeExtension( AssetDatabase.GetAssetPath( arr[ i ] ) , ".prefab" );
            path = path.Replace( '\\' , '/' );
            path = path.Replace( "Objects/SAF" , "Resources/Prefab" );
            //             if ( File.Exists( path ) )
            //             {
            //                 continue;
            //             }

            if ( !Directory.Exists( Path.GetDirectoryName( path ) ) )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            }

            GameObject obj1 = new GameObject();

//             SpriteRenderer sprite = obj1.AddComponent<SpriteRenderer>();
//             sprite.spriteSortPoint = SpriteSortPoint.Pivot;

            GameAnimation ani = obj1.AddComponent< GameAnimation >();
            ani.load( AssetDatabase.GetAssetPath( arr[ i ] ) );

            PrefabUtility.SaveAsPrefabAsset( obj1 , path );

            GameObject.DestroyImmediate( obj1 );

            AssetDatabase.SaveAssets();
        }

        AssetDatabase.Refresh();

//         for ( int i = 0 ; i < objs.Length ; i++ )
//         {
//             for ( int j = 0 ; j < objTest2.Count ; j++ )
//             {
//                 if ( AssetDatabase.GetAssetPath( arr[ i ] ).Contains( objTest2[ j ] ) )
//                 {
//                     trans = objs[ i ].GetComponent<RectTransform>();
//                     trans.parent = GameObject.Find( objTest2[ j ] ).GetComponent< RectTransform >();
//                     trans.localPosition = new Vector3( GameObject.Find( objTest2[ j ] ).transform.childCount * 300.0f , 0 , 0 );
//                     break;
//                 }
//             }
//         }

    }

    static public T GetImporter<T>( UnityEngine.Object asset ) where T : AssetImporter
    {
        return (T)AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( asset ) );
    }


    [MenuItem( "GameTools/Animation/Faf2ETCTextures" )]
    static void GameToolsAnimationFaf2ETCTextures()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.GameObject ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = AssetDatabase.GetAssetPath( arr[ i ] );
//            Debug.Log( i + " " + pathDir );

            GameObject obj = arr[ i ] as GameObject;

            GameAnimation ani = obj.GetComponent<GameAnimation>();

            if ( ani == null )
            {
                continue;
            }

            bool b = false;
            bool b7 = false;

            Dictionary<int , bool> dicb = new Dictionary<int , bool>();

            for ( int j = 0 ; j < ani.saf1.Length ; j++ )
            {
                for ( int k = 0 ; k < ani.saf1[ j ].saf11.Length ; k++ )
                {
                    if ( ani.sprites != null && ani.sprites.Length > 0 )
                    {
                        if ( ani.saf1[ j ].saf11[ k ].textureType < 2 ||
                            ( ani.saf1[ j ].saf11[ k ].textureType == 2 &&
                            ani.saf1[ j ].saf11[ k ].unknow10 == 0 ) )
                        {
                            b = true;
                        }
                    }
                    else
                    {
                        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath( "Assets/Resources/Texture/" + ani.animationName + "_t" + ( ani.saf1[ j ].saf11[ k ].textureID ) + ".png" );

                        if ( ti == null )
                        {
                            string[] nn = ani.animationName.Split( '/' );

                            ti = (TextureImporter)AssetImporter.GetAtPath( "Assets/Resources/Texture/" + ani.animationName + "/" + nn[ nn.Length - 1 ] + "_t" + ( ani.saf1[ j ].saf11[ k ].textureID ) + ".png" );
                        }

                        if ( ti == null )
                        {
//                            Debug.LogError( ani.animationName );
                            continue;
                        }

                        if ( ani.saf1[ j ].saf11[ k ].textureType < 2 ||
                            ( ani.saf1[ j ].saf11[ k ].textureType == 2 && 
                            ani.saf1[ j ].saf11[ k ].unknow10 == 0 ) )
                        {
                            if ( !dicb.ContainsKey( ani.saf1[ j ].saf11[ k ].textureID ) )
                            {
                                dicb.Add( ani.saf1[ j ].saf11[ k ].textureID , true );
                            }


                            if ( ti.GetPlatformTextureSettings( "iPhone" ) == null || 
                                ti.GetPlatformTextureSettings( "Android" ) == null ||
                                ti.GetPlatformTextureSettings( "Standalone" ) == null ||

                                ti.GetPlatformTextureSettings( "iPhone" ).format != TextureImporterFormat.ETC2_RGBA8Crunched ||
                                ti.GetPlatformTextureSettings( "Android" ).format != TextureImporterFormat.ETC2_RGBA8Crunched ||
                                ti.GetPlatformTextureSettings( "Standalone" ).format != TextureImporterFormat.DXT5Crunched )
                            {
                                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "iPhone";
                                settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.SetPlatformTextureSettings( settings );

                                settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "Android";
                                settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.SetPlatformTextureSettings( settings );

                                settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "Standalone";
                                settings.format = TextureImporterFormat.DXT5Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.SetPlatformTextureSettings( settings );

                                ti.SaveAndReimport();

                            }

                        }
                        else
                        {
                            if ( ti.GetPlatformTextureSettings( "iPhone" ) == null || 
                                ti.GetPlatformTextureSettings( "Android" ) == null ||
                                ti.GetPlatformTextureSettings( "Standalone" ) == null ||

                                ti.GetPlatformTextureSettings( "iPhone" ).format != TextureImporterFormat.ETC_RGB4Crunched ||
                                ti.GetPlatformTextureSettings( "Android" ).format != TextureImporterFormat.ETC_RGB4Crunched ||
                                ti.GetPlatformTextureSettings( "Standalone" ).format != TextureImporterFormat.DXT1Crunched )
                            {
                                if ( dicb.ContainsKey( ani.saf1[ j ].saf11[ k ].textureID ) )
                                {
                                    continue;
                                }

                                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "Android";
                                settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.alphaSource = TextureImporterAlphaSource.None;
                                ti.SetPlatformTextureSettings( settings );

                                settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "iPhone";
                                settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.alphaSource = TextureImporterAlphaSource.None;
                                ti.SetPlatformTextureSettings( settings );

                                settings = new TextureImporterPlatformSettings();
                                settings.overridden = true;
                                settings.name = "Standalone";
                                settings.format = TextureImporterFormat.DXT1Crunched;
                                settings.maxTextureSize = 4096;
                                settings.compressionQuality = 100;
                                settings.crunchedCompression = true;
                                ti.alphaSource = TextureImporterAlphaSource.None;
                                ti.SetPlatformTextureSettings( settings );


                                ti.SaveAndReimport();
                            }
                            
                        }
                    }
                }
            }


            if ( ani.sprites != null && ani.sprites.Length > 0 )
            {
                TextureImporter ti = GetImporter<TextureImporter>( ani.sprites[ 0 ].texture );

                if ( ti == null )
                {
                    Debug.LogError( ani.animationName );
                    continue;
                }

                if ( b )
                {

                    if ( ti.GetPlatformTextureSettings( "iPhone" ) == null || 
                        ti.GetPlatformTextureSettings( "Android" ) == null ||
                        ti.GetPlatformTextureSettings( "Standalone" ) == null ||

                        ti.GetPlatformTextureSettings( "iPhone" ).format != TextureImporterFormat.ETC2_RGBA8Crunched ||
                        ti.GetPlatformTextureSettings( "Android" ).format != TextureImporterFormat.ETC2_RGBA8Crunched ||
                        ti.GetPlatformTextureSettings( "Standalone" ).format != TextureImporterFormat.DXT5Crunched )
                    {
                        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                        settings.overridden = true;
                        settings.name = "iPhone";
                        settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.SetPlatformTextureSettings( settings );

                        settings = new TextureImporterPlatformSettings();
                        settings.overridden = true;
                        settings.name = "Android";
                        settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.SetPlatformTextureSettings( settings );

                        settings = new TextureImporterPlatformSettings();
                        settings.overridden = true;
                        settings.name = "Standalone";
                        settings.format = TextureImporterFormat.DXT5Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.SetPlatformTextureSettings( settings );

                        ti.SaveAndReimport();
                    }

                }
                else
                {
                    if ( ti.GetPlatformTextureSettings( "iPhone" ) == null || 
                        ti.GetPlatformTextureSettings( "Android" ) == null ||
                        ti.GetPlatformTextureSettings( "Standalone" ) == null ||

                        ti.GetPlatformTextureSettings( "iPhone" ).format != TextureImporterFormat.ETC_RGB4Crunched ||
                        ti.GetPlatformTextureSettings( "Android" ).format != TextureImporterFormat.ETC_RGB4Crunched || 
                        ti.GetPlatformTextureSettings( "Standalone" ).format != TextureImporterFormat.DXT1Crunched )
                    {
                        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                        settings.overridden = true;
                        settings.name = "Android";
                        settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.alphaSource = TextureImporterAlphaSource.None;
                        ti.SetPlatformTextureSettings( settings );

                        settings.overridden = true;
                        settings.name = "iPhone";
                        settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.alphaSource = TextureImporterAlphaSource.None;
                        ti.SetPlatformTextureSettings( settings );

                        settings.overridden = true;
                        settings.name = "Standalone";
                        settings.format = TextureImporterFormat.DXT1Crunched;
                        settings.maxTextureSize = 4096;
                        settings.compressionQuality = 100;
                        settings.crunchedCompression = true;
                        ti.alphaSource = TextureImporterAlphaSource.None;
                        ti.SetPlatformTextureSettings( settings );

                        ti.SaveAndReimport();
                    }
                    
                }
            }

            if ( b )
            {
            }

        }

    }


    [MenuItem( "GameTools/Animation/RGB16Textures" )]
    static void GameToolsAnimationRGB16Textures()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = AssetDatabase.GetAssetPath( arr[ i ] );
            Debug.Log( i + " " + pathDir );

            TextureImporter ti = GetImporter<TextureImporter>( arr[ i ] );

            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "iPhone";
            settings.format = TextureImporterFormat.RGB16;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "Android";
            settings.format = TextureImporterFormat.RGB16;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings.overridden = true;
            settings.name = "Standalone";
            settings.format = TextureImporterFormat.RGB16;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.alphaSource = TextureImporterAlphaSource.None;
            ti.SetPlatformTextureSettings( settings );

            ti.SaveAndReimport();


        }

    }


    [MenuItem( "GameTools/Animation/RGBTextures" )]
    static void GameToolsAnimationRGBTextures()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = AssetDatabase.GetAssetPath( arr[ i ] );
            Debug.Log( i + " " + pathDir );

            TextureImporter ti = GetImporter<TextureImporter>( arr[ i ] );

            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "iPhone";
            settings.format = TextureImporterFormat.ETC_RGB4Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "Android";
            settings.format = TextureImporterFormat.ETC_RGB4Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings.overridden = true;
            settings.name = "Standalone";
            settings.format = TextureImporterFormat.DXT1Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.alphaSource = TextureImporterAlphaSource.None;
            ti.SetPlatformTextureSettings( settings );

            ti.SaveAndReimport();


        }

    }


    [MenuItem( "GameTools/Animation/RGBATextures" )]
    static void GameToolsAnimationRGBATextures()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = AssetDatabase.GetAssetPath( arr[ i ] );
            Debug.Log( i + " " + pathDir );

            TextureImporter ti = GetImporter<TextureImporter>( arr[ i ] );

            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "iPhone";
            settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings = new TextureImporterPlatformSettings();
            settings.overridden = true;
            settings.name = "Android";
            settings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.SetPlatformTextureSettings( settings );

            settings.overridden = true;
            settings.name = "Standalone";
            settings.format = TextureImporterFormat.DXT5Crunched;
            settings.maxTextureSize = 4096;
            settings.compressionQuality = 100;
            settings.crunchedCompression = true;
            ti.alphaSource = TextureImporterAlphaSource.None;
            ti.SetPlatformTextureSettings( settings );

            ti.SaveAndReimport();

        }

    }


    [MenuItem( "GameTools/Animation/FixTextures" )]
    static void GameToolsAnimationFixTextures()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.TopLevel );

        if ( arr.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < arr.Length ; i++ )
        {
            string pathDir = AssetDatabase.GetAssetPath( arr[ i ] );

            TextureImporter ti = GetImporter<TextureImporter>( arr[ i ] );

            if ( ti.alphaIsTransparency )
            {
                Debug.Log( i + " " + pathDir );

                ti.alphaIsTransparency = false;
                ti.SaveAndReimport();
            }

            if ( ti.textureType != TextureImporterType.Sprite )
            {
                Debug.Log( i + " " + pathDir );

                ti.textureType = TextureImporterType.Sprite;
                ti.spritePixelsPerUnit = 2;
                ti.alphaIsTransparency = false;
                ti.spritePivot = new Vector2( 0.0f , 1.0f );
                ti.SaveAndReimport();
            }

            if ( ti.textureType == TextureImporterType.Sprite )
            {
//                 if ( ti.spritePivot.x != 0.0f || ti.spritePivot.y != 1.0f || ti.spritePixelsPerUnit != 2 )
//                 {
//                     ti.spritePixelsPerUnit = 2;
//                     ti.spritePivot = new Vector2( 0.0f , 1.0f );
//                     ti.SaveAndReimport();
//                 }
            }

        }

    }

    [MenuItem( "GameTools/Set Pivot(s)" )]
    static void SetPivots()
    {
        UnityEngine.Object[] textures = GetSelectedTextures();

        Selection.objects = GetSelectedTextures();

        foreach ( Texture2D texture in textures )
        {
            string path = AssetDatabase.GetAssetPath( texture );
            TextureImporter ti = AssetImporter.GetAtPath( path ) as TextureImporter;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            for ( int i = 0 ; i < ti.spritesheet.Length ; i++ )
            {
                SpriteMetaData d = ti.spritesheet[ i ];
                d.pivot = new Vector2( 0.0f , d.rect.height );

                newData.Add( d );
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset( path , ImportAssetOptions.ForceUpdate );
        }
    }
     
    static UnityEngine.Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered( typeof( Texture2D ) , SelectionMode.DeepAssets );
    }


  

}


