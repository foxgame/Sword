using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



public class GameBattleJumpHPUI : Singleton<GameBattleJumpHPUI>
{

    float time = 0.0f;
    bool start = false;
    float alpha = 0.0f;
    float dis = 0.0f;

    Vector2 position;

    int jumpCount = 0;
    List<GameObject> objs = new List<GameObject>();

    OnEventOver onEventOver;

    void clear()
    {
        objs.Clear();

        GameDefine.DestroyAll( transform );
    }
    
    public void onJumpOver()
    {
        jumpCount++;

        if ( objs.Count == jumpCount )
        {
            time = 0.0f;
            start = true;
            alpha = 1.0f;
            dis = 0.0f;
        }
    }

    public void jump( GameBattleAttackResultSide side , int hp , OnEventOver over )
    {
        clear();

        gameObject.SetActive( true );

        RectTransform trans = GetComponent<RectTransform>();

        if ( side == GameBattleAttackResultSide.Right )
        {
            trans.anchoredPosition = new Vector2( -40.0f , 30.0f );
        }
        else
        {
            trans.anchoredPosition = new Vector2( 54.0f , -24.0f );
        }

        position = trans.anchoredPosition;

        onEventOver = over;

        jumpCount = 0;


        string str = GameDefine.getBigInt( hp.ToString() );

        float ox = 6.0f * str.Length / 2.0f;

        for ( int i = 0 ; i < str.Length ; i++ )
        {
            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/TextUI" ) );

            trans = obj.GetComponent<RectTransform>();
            trans.SetParent( transform );
            trans.anchoredPosition = new Vector2( -ox + i * 6.0f , 0.0f );
            trans.localScale = new Vector3( 0.5f , 0.5f , 0.5f );

            GameBattleJumpHPUIText jumpText = obj.GetComponent<GameBattleJumpHPUIText>();
            jumpText.jump( -i * 0.1f , onJumpOver );

            Text text = obj.GetComponent<Text>();
            text.text = str.Substring( i , 1 );

            if ( hp < 0 )
                text.color = new Color( 0.0f , 1.0f , 0.0f , 1.0f );

            objs.Add( obj );
        }
    }


    void Update()
    {
        if ( !start )
        {
            return;
        }
        
        float d = 30.0f;
        float t = 1.0f;

        time += Time.deltaTime;

        dis = time * d * t;
        alpha = 1.0f - time * t;

        RectTransform trans = GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2( position.x , position.y + dis );

        for ( int i = 0 ; i < objs.Count ; i++ )
        {
            Text text = objs[ i ].GetComponent<Text>();

            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }

        if ( time > t )
        {
            start = false;

            gameObject.SetActive( false );

            if ( onEventOver != null )
                onEventOver();
        }

    }

}
