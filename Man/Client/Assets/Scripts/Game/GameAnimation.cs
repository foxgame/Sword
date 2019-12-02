using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public enum GameAnimationDirection
{
    South = 0,
    West = 1,
    North = 2,
    East = 3,

    Count = 4
}

public enum GameAnimationType
{
    Stand = 0,
    Move = 1,
    MoveStart = 2,
    MoveEnd = 3,
    Win = 4,
    Skill = 5,

    Count = 32
}


public class GameAnimation : MonoBehaviour
{
    public bool UI = false;

    public int lastUnitID = GameDefine.INVALID_ID;

    [ System.Serializable ]
    public class SAFHead
    {
        public short count1;
        public short count2;

        public short[] count3 = new short[ 32 ];

//        public SAFDirectionAnimation[] directionAnimations = new SAFDirectionAnimation[ 32 ];
    }

    [System.Serializable]
    public class SAF11
    {
        public ushort textureID;
        public short textureX;
        public short textureY;
        public byte textureType;
        public short unknow8;
        public short unknow10;
        public short unknow12;
    };

    [System.Serializable]
    public class SAF1
    {
        public short sound;
        public short time;

        public byte hit;
        public byte color;
        public byte shake;
        public byte unknow4;

        public short count;

        public SAF11[] saf11;
    };

    [System.Serializable]
    public class SAF2
    {
        public short width;
        public short height;
    };

    public SAFHead safHead;
    public SAF1[] saf1;
    public SAF2[] saf2;

    public Sprite[] sprites = null;

    public float time;
    public int frame;
    public float frameTime;

    public float delay;

    public SAF1 active;
    
    public string animationName = "";

    public int startFrame = 0;
    public int endFrame = 0;
    public bool start = false;
    public bool pause = false;
    public bool playSound = true;

    public bool loop = false;
    OnEventOver onEventOver;

    public delegate void OnAnimationEvent( int i );
    public OnAnimationEvent onAnimationEvent;

    public GameAnimationType animationType;
    public GameAnimationDirection animationDirection;

    public GameAnimation otherGameAnimation;

    public int offsetX;
    public int offsetY;

    public Vector2 pivot = new Vector2( 0.0f , 1.0f );

    [ SerializeField]
    Color color = Color.white;

    public void playAnimationBattle( GameAnimationType type , GameAnimationDirection dir , OnEventOver over )
    {
        animationType = type;
        animationDirection = dir;

        int index = 0;
        int size = safHead.count3[ (int)type ];

        switch ( type )
        {
            case GameAnimationType.Stand:
                {
                    index += 2;
                    if ( safHead.count3[ (int)GameAnimationType.Win ] > 0 )
                    {
                        index += safHead.count3[ (int)GameAnimationType.Win ];
                    }
                    else
                    {
                        index += 1;
                    }

                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = true;
                }
                break;
            case GameAnimationType.Move:
                {
                    index += 2;
                    if ( safHead.count3[ (int)GameAnimationType.Win ] > 0 )
                    {
                        index += safHead.count3[ (int)GameAnimationType.Win ];
                    }
                    else
                    {
                        index += 1;
                    }

                    index += safHead.count3[ (int)GameAnimationType.Stand ] * 4;

                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = true;
                }
                break;
            case GameAnimationType.MoveStart:
                {
                    index += 2;
                    if ( safHead.count3[ (int)GameAnimationType.Win ] > 0 )
                    {
                        index += safHead.count3[ (int)GameAnimationType.Win ];
                    }
                    else
                    {
                        index += 1;
                    }

                    index += safHead.count3[ (int)GameAnimationType.Move ] * 4;
                    index += safHead.count3[ (int)GameAnimationType.Stand ] * 4;

                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = false;
                }
                break;
            case GameAnimationType.MoveEnd:
                {
                    index += 2;
                    if ( safHead.count3[ (int)GameAnimationType.Win ] > 0 )
                    {
                        index += safHead.count3[ (int)GameAnimationType.Win ];
                    }
                    else
                    {
                        index += 1;
                    }

                    if ( safHead.count3[ (int)GameAnimationType.Move ] > 0 )
                    {
                        index += safHead.count3[ (int)GameAnimationType.Move ] * 4;
                    }
                    else
                    {
                        index += 1;
                    }

                    index += safHead.count3[ (int)GameAnimationType.Stand ] * 4;
                    index += safHead.count3[ (int)GameAnimationType.MoveStart ] * 4;

                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = false;
                }
                break;
            case GameAnimationType.Win:
                {
                    index += 2;
                    startFrame = index;
                    endFrame = index + size;
                    loop = false;
                }
                break;
            case GameAnimationType.Skill:
                {
                    size = 2;
                    startFrame = 0;
                    endFrame = 2;
                    loop = true;
                }
                break;
            default:
                break;
        }


        if ( size == 0 )
        {
            start = false;
            if ( over != null )
            {
                over();
            }
        }
        else
        {
            frame = startFrame;
            updateFrame( frame );

            onEventOver = over;
            start = true;
        }
    }

    public void playAnimationRPG( GameAnimationType type , GameAnimationDirection dir , OnEventOver over )
    {
        animationType = type;
        animationDirection = dir;

        int index = 0;
        int size = safHead.count3[ (int)type ];

        switch ( type )
        {
            case GameAnimationType.Stand:
                {
                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = true;
                }
                break;
            case GameAnimationType.Move:
                {
                    index += safHead.count3[ (int)GameAnimationType.Stand ] * 4;

                    index += (int)dir * size;
                    startFrame = index;
                    endFrame = index + size;
                    loop = true;
                }
                break;
            default:
                break;
        }


        if ( size == 0 )
        {
            start = false;
            if ( over != null )
            {
                over();
            }
        }
        else
        {
            frame = startFrame;
            updateFrame( frame );

            onEventOver = over;
            start = true;
        }
    }

    public void playAnimation( int s = 0 , int e = GameDefine.INVALID_ID , bool l = true , OnEventOver over = null )
    {
        if ( s < saf1.Length && s >= 0 )
        {
            startFrame = s;
            frame = s;
        }
        else
        {
            startFrame = 0;
            frame = 0;
        }

        if ( e < 0 || e >= safHead.count1 )
        {
            endFrame = safHead.count1;
        }
        else if ( e < safHead.count1 )
        {
            endFrame = e;
        }

        loop = l;
        onEventOver = over;
        start = true;

        updateFrame( frame );
    }

    public void clearAnimation()
    {
        for ( int i = 0 ; i < objects.Count ; i++ )
        {
            Destroy( objects[ i ] );
            objects[ i ].transform.SetParent( null );
        }

        objects.Clear();
        objectsAlpha.Clear();

        otherGameAnimation = null;

        Resources.UnloadUnusedAssets();
    }

    public void stopAnimation()
    {
        start = false;
    }

    public void pauseAnimation( bool b )
    {
        start = b;
    }

    public void showFrame( int f , float scale = 0.5f )
    {
        frame = f;

        if ( f >= 0 && f < saf1.Length )
        {
            updateFrame( f , scale );
        }
    }

    public void showLastFrame( float scale = 0.5f )
    {
        updateFrame( frame , scale );
    }

    void Awake()
    {
        objects = new List< GameObject >();
        objectsAlpha = new List<float>();
    }

    List< GameObject > objects;
    List< float > objectsAlpha;

#if UNITY_EDITOR

    public void load( string path )
    {
        int n1 = path.IndexOf( "Objects/" );
        animationName = path.Substring( n1 + 8 , path.Length - n1 - 8 - 4 ).Replace( "SAF/" , "" );

        safHead = new SAFHead();

        FileStream fs = File.OpenRead( path );

        byte[] bytes = new byte[ fs.Length ];
        fs.Read( bytes , 0 , (int)fs.Length );

        int index = 0;

        safHead.count1 = (short)BitConverter.ToInt32( bytes , index ); index += 2;
        safHead.count2 = (short)BitConverter.ToInt32( bytes , index ); index += 2;

        saf1 = new SAF1[ safHead.count1 ];
        saf2 = new SAF2[ safHead.count2 ];

        for ( int i = 0 ; i < 32 ; i++ )
        {
            short count = (short)BitConverter.ToInt16( bytes , index ); index += 2;
            safHead.count3[ i ] = count;
        }

//        Dictionary<int , int> dic = new Dictionary<int , int>();

        for ( int i = 0 ; i < safHead.count1 ; i++ )
        {
            saf1[ i ] = new SAF1();

            saf1[ i ].sound = (short)BitConverter.ToInt16( bytes , index ); index += 2;
            saf1[ i ].time = (short)BitConverter.ToInt16( bytes , index ); index += 2;
            saf1[ i ].hit = bytes[ index ]; index += 1;
            saf1[ i ].color = bytes[ index ]; index += 1;
            saf1[ i ].shake = bytes[ index ]; index += 1;
            saf1[ i ].unknow4 = bytes[ index ]; index += 1;
            saf1[ i ].count = (short)BitConverter.ToInt16( bytes , index ); index += 2;

            saf1[ i ].saf11 = new SAF11[ saf1[ i ].count ];

            for ( int j = 0 ; j < saf1[ i ].count ; j++ )
            {
                saf1[ i ].saf11[ j ] = new SAF11();

                saf1[ i ].saf11[ j ].textureID = (ushort)BitConverter.ToUInt16( bytes , index ); index += 2;
                saf1[ i ].saf11[ j ].textureX = (short)BitConverter.ToInt16( bytes , index ); index += 2;
                saf1[ i ].saf11[ j ].textureY = (short)BitConverter.ToInt16( bytes , index ); index += 2;
                saf1[ i ].saf11[ j ].textureType = bytes[ index ]; index += 1;
                saf1[ i ].saf11[ j ].unknow8 = (short)BitConverter.ToInt16( bytes , index ); index += 2;
                saf1[ i ].saf11[ j ].unknow10 = (short)BitConverter.ToInt16( bytes , index ); index += 2;
                saf1[ i ].saf11[ j ].unknow12 = (short)BitConverter.ToInt16( bytes , index ); index += 2;

//                 if ( dic.ContainsKey( saf1[ i ].saf11[ j ].textureID ) )
//                 {
//                     if ( dic[ saf1[ i ].saf11[ j ].textureID ] != saf1[ i ].saf11[ j ].unknow8 )
//                     {
//                         Debug.Log( i + " " + j + " " + saf1[ i ].saf11[ j ].textureID + " " + dic[ saf1[ i ].saf11[ j ].textureID ]  + " " + saf1[ i ].saf11[ j ].unknow8 );
//                     }
//                 }
//                 else
//                 {
//                     dic.Add( saf1[ i ].saf11[ j ].textureID , saf1[ i ].saf11[ j ].unknow8 );
//                 }
            }
        }

        for ( int i = 0 ; i < safHead.count2 ; i++ )
        {
            saf2[ i ] = new SAF2();

            saf2[ i ].width = (short)BitConverter.ToInt16( bytes , index ); index += 2;
            saf2[ i ].height = (short)BitConverter.ToInt16( bytes , index ); index += 2;
        }

        if ( safHead.count2 > 0 )
        {
            string[] str = animationName.Split( '/' );

            //        Debug.Log( "Assets/Objects/Texture/" + animationName + "/" + str[ str.Length - 1 ] + ".png" );
            UnityEngine.Object[] objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath( "Assets/Objects/Texture/" + animationName + "/" + str[ str.Length - 1 ] + ".png" );

            if ( objs.Length > 0 )
            {
                sprites = new Sprite[ safHead.count2 ];

                for ( int i = 0 ; i < sprites.Length ; i++ )
                {
                    sprites[ i ] = (Sprite)objs[ i + 1 ];
//                    sprites[ i ].p
                }
            }
            else
            {
//                 for ( int i = 0 ; i < sprites.Length ; i++ )
//                 {
//                     sprites[ i ] = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>( "Assets/Objects/Texture/" + animationName + "/" + str[ str.Length - 1 ] + "_t" + i + ".png" );
//                 }
            }
        }

        //updateFrame( 0 );
    }

#endif

    public void setColor( Color c )
    {
        color = c;

        updateColor();
    }

    public Color getColor()
    {
        return color;
    }

    void updateColor()
    {
        if ( UI )
        {
            for ( int i = 0 ; i < objects.Count ; i++ )
            {
                Image image = objects[ i ].GetComponent<Image>();
                Color c = color;
                c.a *= objectsAlpha[ i ];
                image.color = c;
            }
        }
        else
        {
            for ( int i = 0 ; i < objects.Count ; i++ )
            {
                SpriteRenderer sprite = objects[ i ].GetComponent<SpriteRenderer>();
                Color c = color;
                c.a *= objectsAlpha[ i ];
                sprite.color = c;
            }
        }
    }

    public void setAlpha( float a )
    {
        color.a = a;

        updateColor();
    }

    void updateFrame( int f , float scale = 0.5f )
    {
        if ( pause )
        {
            return;
        }

        for ( int i = 0 ; i < objects.Count ; i++ )
        {
            Destroy( objects[ i ] );
            objects[ i ].transform.SetParent( null );
        }

        objects.Clear();
        objectsAlpha.Clear();

        if ( onAnimationEvent != null )
        {
            onAnimationEvent( f );
        }

        frameTime = 0.04f + saf1[ f ].time * 0.04f + delay;

        for ( int i = 0 ; i < saf1[ f ].count ; i++ )
        {
            if ( UI )
            {
                GameObject objImage = Resources.Load<GameObject>( "Prefab/Image" );

                GameObject img = Instantiate( objImage );
                img.name = animationName + "_t" + ( saf1[ f ].saf11[ i ].textureID );

                RectTransform trans = img.GetComponent<RectTransform>();

                trans.SetParent( transform );
                trans.pivot = pivot;
                trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
                trans.anchoredPosition = new Vector2( saf1[ f ].saf11[ i ].textureX + offsetX
                    , -saf1[ f ].saf11[ i ].textureY + offsetY );


                Image image = img.GetComponent<Image>();

                image.color = color;

                switch ( saf1[ f ].saf11[ i ].unknow10 )
                {
                    case 16:
                        {
                            float alpha = Mathf.Min( saf1[ f ].saf11[ i ].unknow8 * 0.05f , 1.0f );
                            image.color = new Color( 1.0f , 1.0f , 1.0f , alpha );
                        }
                        break;
                    default:
                        break;
                }

                int otherTextureID = GameDefine.INVALID_ID;

                if ( saf1[ f ].saf11[ i ].textureID >= 0x8000 )
                {
                    otherTextureID = saf1[ f ].saf11[ i ].textureID - 0x8000 + 1;
                }

                if ( otherTextureID != GameDefine.INVALID_ID )
                {
                    if ( otherGameAnimation == null )
                    {
                        objects.Add( img );
                        objectsAlpha.Add( image.color.a );
                        continue;
                    }

                    if ( otherGameAnimation.sprites != null &&
                    otherGameAnimation.sprites.Length > 0 )
                    {
                        if ( otherGameAnimation.saf1[ otherTextureID ].saf11.Length == 0 )
                        {
                            otherTextureID = 1;
                        }
                    
                        image.sprite = otherGameAnimation.sprites[ otherGameAnimation.saf1[ otherTextureID ].saf11[ 0 ].textureID ];
                    }
                    else
                    {
                        image.sprite = Resources.Load<Sprite>( "Texture/" + otherGameAnimation.animationName + "_t" + ( otherGameAnimation.saf1[ otherTextureID ].saf11[ 0 ].textureID ) );

                        if ( image.sprite == null )
                        {
                            string[] nn = otherGameAnimation.animationName.Split( '/' );
                            image.sprite = Resources.Load<Sprite>( "Texture/" + otherGameAnimation.animationName + "/" + nn[ nn.Length - 1 ] + "_t" + ( otherGameAnimation.saf1[ otherTextureID ].saf11[ 0 ].textureID ) );
                        }
                    }

                    image.material = GameMaterialConfig.instance.getMaterial( otherGameAnimation.saf1[ otherTextureID ].saf11[ 0 ].textureType );

                    //#if UNITY_EDITOR
                    if ( image.sprite == null )
                    {
                        //                Debug.Log( "animationName" + animationName );
                        objects.Add( img );
                        objectsAlpha.Add( image.color.a );
                        continue;
                    }
                    //#endif
                    scale = 0.5f;
                    float w1 = image.sprite.rect.width * scale;
                    float h1 = image.sprite.rect.height * scale;

                    trans.sizeDelta = new Vector2( w1 , h1 );

                    trans.SetAsFirstSibling();
                    //            image.SetNativeSize();


                    objects.Add( img );
                    objectsAlpha.Add( image.color.a );

                    continue;
                }

                if ( sprites != null &&
                    sprites.Length > 0 )
                {
                    image.sprite = sprites[ saf1[ f ].saf11[ i ].textureID ];
                }
                else
                {
                    image.sprite = Resources.Load<Sprite>( "Texture/" + animationName + "_t" + ( saf1[ f ].saf11[ i ].textureID ) );

                    if ( image.sprite == null )
                    {
                        string[] nn = animationName.Split( '/' );
                        image.sprite = Resources.Load<Sprite>( "Texture/" + animationName + "/" + nn[ nn.Length - 1 ] + "_t" + ( saf1[ f ].saf11[ i ].textureID ) );
                    }
                }

                int type = saf1[ f ].saf11[ i ].textureType;

                if ( type == 7 )
                {
                    trans.SetAsFirstSibling();
                }

                if ( saf1[ f ].saf11[ i ].textureType == 2 && 
                    saf1[ f ].saf11[ i ].unknow10 == 0 )
                {
                    type = 0;
                }

                image.material = GameMaterialConfig.instance.getMaterial( type );

                //#if UNITY_EDITOR
                if ( image.sprite == null )
                {
                    //                Debug.Log( "animationName" + animationName );
                    objects.Add( img );
                    objectsAlpha.Add( image.color.a );
                    continue;
                }
                //#endif
                scale = 0.5f;
                float w = image.sprite.rect.width * scale;
                float h = image.sprite.rect.height * scale;

                trans.sizeDelta = new Vector2( w , h );

                if ( pivot.x == 0.0f && pivot.y == 0.0f )
                {
                    trans.localScale = new Vector3( -1.0f , 1.0f , 1.0f );
                    trans.anchoredPosition = new Vector2( w + saf1[ f ].saf11[ i ].textureX + offsetX
                    , -saf1[ f ].saf11[ i ].textureY + offsetY );
                }

                //            image.SetNativeSize();

                objects.Add( img );
                objectsAlpha.Add( image.color.a );
            }
            else
            {
                GameObject objSprite = Resources.Load<GameObject>( "Prefab/Sprite" );

                GameObject sprite = Instantiate( objSprite );
                sprite.name = animationName + "_t" + ( saf1[ f ].saf11[ i ].textureID );

                Transform trans = sprite.transform;

                trans.SetParent( transform );
                trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
                trans.localPosition = new Vector3( saf1[ f ].saf11[ i ].textureX + offsetX ,
                    -saf1[ f ].saf11[ i ].textureY + offsetY ,
                    -i );

                SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
                spriteRenderer.color = color;

                switch ( saf1[ f ].saf11[ i ].unknow10 )
                {
                    case 16:
                        {
                            float alpha = Mathf.Min( saf1[ f ].saf11[ i ].unknow8 * 0.05f , 1.0f );
                            spriteRenderer.color = new Color( color.r , color.g , color.b , color.a * alpha );
                        }
                        break;
                    default:
                        break;
                }

                if ( sprites != null &&
                    sprites.Length > 0 )
                {
                    spriteRenderer.sprite = sprites[ saf1[ f ].saf11[ i ].textureID ];
                }
                else
                {
                    spriteRenderer.sprite = Resources.Load<Sprite>( "Texture/" + animationName + "_t" + ( saf1[ f ].saf11[ i ].textureID ) );

                    if ( spriteRenderer.sprite == null )
                    {
                        string[] nn = animationName.Split( '/' );
                        spriteRenderer.sprite = Resources.Load<Sprite>( "Texture/" + animationName + "/" + nn[ nn.Length - 1 ] + "_t" + ( saf1[ f ].saf11[ i ].textureID ) );
                    }
                }

                int type = saf1[ f ].saf11[ i ].textureType;

                if ( type == 7 )
                {
                    trans.localPosition = new Vector3( saf1[ f ].saf11[ i ].textureX + offsetX ,
                    -saf1[ f ].saf11[ i ].textureY + offsetY ,
                    i );
                }

                if ( saf1[ f ].saf11[ i ].textureType == 2 &&
                    saf1[ f ].saf11[ i ].unknow10 == 0 )
                {
                    type = 0;
                }

                spriteRenderer.material = GameMaterialConfig.instance.getMaterial( type );

                //#if UNITY_EDITOR
                if ( spriteRenderer.sprite == null )
                {
                    //                Debug.Log( "animationName" + animationName );
                    objects.Add( sprite );
                    objectsAlpha.Add( spriteRenderer.color.a );
                    continue;
                }
                //#endif
                //            scale = 0.5f;
                //            float w = sprie.sprite.rect.width * scale;
                //            float h = sprie.sprite.rect.height * scale;

                //trans.sizeDelta = new Vector2( w , h );

                //            image.SetNativeSize();

                objects.Add( sprite );
                objectsAlpha.Add( spriteRenderer.color.a );
            }


        }

        active = saf1[ f ];

        if ( saf1[ f ].sound != -1 && 
            playSound )
        {
            //            Debug.Log( "Sound/" + animationName + "_s" + saf1[ f ].sound );
            GameMusicManager.instance.playSound( "Sound/" + saf1[ f ].sound );
        }
    }

    void Update()
    {
        if ( pause )
        {
            return;
        }

        if ( !start )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time >= frameTime )
        {
            time = 0.0f;

            frame++;

            if ( frame >= endFrame )
            {
                if ( loop )
                {
                    frame = startFrame;

                    if ( GameUserData.instance.Stage == 26 )
                    {
                        // fix stage bug.
                        playSound = false;
                    }
                }
                else
                {
                    start = false;

                    if ( onEventOver != null )
                    {
                        onEventOver();
                    }

                    return;
                }
            }

            updateFrame( frame );
        }
    }


}
