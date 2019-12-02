using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleTitleUI : GameUI<GameBattleTitleUI>
{
    OnEventOver onEventOver;
    float time;
    float timeAll;

    Image image;
    Image black;
    bool isShowBlack;
    bool alphaAdd = false;
    float alpha = 0.0f;

    public override void initSingleton()
    {
        image = transform.Find( "Image").GetComponent<Image>();
        black = transform.Find( "Black" ).GetComponent<Image>();
    }

    public void showTitle( int t , OnEventOver over )
    {
        show();

        image.sprite = Resources.Load<Sprite>( "Texture/Map/Stage" + GameDefine.getString2( t ) + "/Tanm_" + GameDefine.getString2( t ) + "_t0" );

        timeAll = 0.5f;
        time = 0.0f;

        black.color = new Color( 0.0f , 0.0f , 0.0f , 0.0f );
        image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );

        alpha = 0.0f;

        isShowBlack = true;
        alphaAdd = true;

        onEventOver = over;
    }

    IEnumerator onShowOver( float t )
    {
        yield return new WaitForSeconds( t );

        timeAll = 0.3f;
        time = 0.0f;

        alpha = 1.0f;

        isShowBlack = true;
        alphaAdd = false;
    }

    protected override void onUpdate()
    {
        if ( !isShowBlack )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > timeAll )
        {
            isShowBlack = false;

            if ( alphaAdd )
            {
                image.color = Color.white;
                black.color = Color.black;

                StartCoroutine( onShowOver( 3.0f ) );
            }
            else
            {
                image.color = new Color( 1.0f , 1.0f , 1.0f , 0.0f );
                black.color = new Color( 0.0f , 0.0f , 0.0f , 0.0f );
                image.sprite = null;

                unShow();

#if UNITY_EDITOR
                Debug.Log( "onEventOver" );
#endif

                if ( onEventOver != null )
                {
                    onEventOver();
                }
            }
        }
        else
        {
            if ( alphaAdd )
            {
                alpha = 1.0f / timeAll * time;
            }
            else
            {
                alpha = 1.0f - 1.0f / timeAll * time;
            }

            //            Debug.Log( "alpha " + alpha + " " + time );
            image.color = new Color( 1.0f , 1.0f , 1.0f , alpha );
            black.color = new Color( 0.0f , 0.0f , 0.0f , alpha );
        }
    }

}

