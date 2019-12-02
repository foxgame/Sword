using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameMsgBoxTextUI : MonoBehaviour
{
    GameImageText text;
    Transform faceTrans;
    GameAnimation gameAnimation;

    RectTransform textTrans;

    public Rect rect;

    public bool IsOver { get { return text.IsOver; } }
    public bool IsStopLine { get { return text.IsStopLine; } }

    Vector2 pivot;

    void Awake()
    {
        text = transform.Find( "text" ).GetComponent<GameImageText>();
        textTrans = text.GetComponent<RectTransform>();

        faceTrans = transform.Find( "face" );
    }

    public void setPivot( Vector2 v2 )
    {
        pivot = v2;
    }

    public void onClick()
    {
        text.onClick();
    }

    public void clear()
    {
        text.clear();

        if ( gameAnimation != null )
        {
            gameAnimation.stopAnimation();
            gameAnimation.clearAnimation();

            Destroy( gameAnimation.gameObject );
            gameAnimation.transform.SetParent( null );

            gameAnimation = null;
        }
    }

    public void showText( int face , int type , string str )
    {
        clear();

        if ( face != -1 )
        {
            string path = "Prefab/Face/Face" + ( face < 10 ? "0" + face : face.ToString() );

            GameObject faceObj = Resources.Load<GameObject>( path );

            if ( faceObj == null )
            {
                return;
            }

            GameObject obj = Instantiate<GameObject>( faceObj );

            Transform trans = obj.transform;
            trans.SetParent( faceTrans );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
            trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );

            gameAnimation = obj.GetComponent<GameAnimation>();

            for ( int i = 0 ; i < gameAnimation.saf1.Length ; i++ )
            {
                for ( int j = 0 ; j < gameAnimation.saf1[ i ].saf11.Length ; j++ )
                {
                    gameAnimation.saf1[ i ].saf11[ j ].textureX = 0;
                    gameAnimation.saf1[ i ].saf11[ j ].textureY = 0;
                }
            }

            gameAnimation.UI = true;
            gameAnimation.pivot = pivot;
            gameAnimation.showFrame( type );

            if ( textTrans.sizeDelta.x == 595 )
            {
                textTrans.sizeDelta = new Vector2( 380 , 110 );
            }
        }
        else
        {
            if ( textTrans.sizeDelta.x == 380 )
            {
                textTrans.sizeDelta = new Vector2( 595 , 110 );
            }
        }

        text.showText( str );
    }
}

