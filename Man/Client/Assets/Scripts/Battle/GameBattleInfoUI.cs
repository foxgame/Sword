using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleInfoUI : MonoBehaviour
{
    public bool right = false;

    Image imageRed;
    Image imageYellow;

    RectTransform transRed;
    RectTransform transYellow;

    RectTransform white;

    Text textName;
    Text textHP;
    Text textHPMax;

    bool start = false;

    int maxHP = 0;


    float pos;
    float movePos;
    float dis;
    int times;

    void Awake()
    {
        imageRed = transform.Find( "hpBar/hpRed" ).GetComponent<Image>();
        imageYellow = transform.Find( "hpBar/hpYellow" ).GetComponent<Image>();

        transRed = transform.Find( "hpBar/hpRed" ).GetComponent<RectTransform>();
        transYellow = transform.Find( "hpBar/hpYellow" ).GetComponent<RectTransform>();

        white = transform.Find( "hpBar/white" ).GetComponent<RectTransform>();
        white.gameObject.SetActive( false );

        textName = transform.Find( "name" ).GetComponent< Text >();
        textHP = transform.Find( "hp" ).GetComponent<Text>();
        textHPMax = transform.Find( "maxHP" ).GetComponent<Text>();
    }
    

    public void setValue( int hp , int hm , string name )
    {
        if ( hp > hm )
        {
            hp = hm;
        }

        maxHP = hm;

        float v = hp / (float)maxHP;
        pos = ( 101.0f - v * 101.0f );

        textName.text = name;
        textHP.text = GameDefine.getBigInt( hp.ToString() , true );
        textHPMax.text = GameDefine.getBigInt( maxHP.ToString() , true );

        updatePosition();

        start = false;
    }

    void updatePosition()
    {
        if ( right )
        {
            transRed.anchoredPosition = new Vector2( 2 + pos , 0.0f );
            transYellow.anchoredPosition = new Vector2( 2 + pos , 0.0f );
        }
        else
        {
            transRed.anchoredPosition = new Vector2( -2 - pos , 0.0f );
            transYellow.anchoredPosition = new Vector2( -2 - pos , 0.0f );
        }
    }


    public void setValue( int hp )
    {
        if ( start )
        {
            pos = movePos;
            updatePosition();
        }

        if ( hp < 0 )
        {
            hp = 0;
        }

        float v = hp / (float)maxHP;
        movePos = ( 101.0f - v * 101.0f );
        dis = 0.0f;

        textHP.text = GameDefine.getBigInt( hp.ToString() , true );

        if ( right )
        {
            transYellow.anchoredPosition = new Vector2( 2 + movePos , 0.0f );
        }
        else
        {
            transYellow.anchoredPosition = new Vector2( -2 - movePos , 0.0f );
        }

        white.gameObject.SetActive( true );

        times = 10;

        start = true;
    }

    void Update()
    {
        if ( !start )
        {
            return;
        }

        dis += Time.deltaTime * 50.0f;

        if ( right )
        {
            if ( pos + dis > movePos )
            {
                start = false;
                pos = movePos;
                transRed.anchoredPosition = new Vector2( 2 + pos , 0.0f );

                white.gameObject.SetActive( false );
            }
            else
            {
                transRed.anchoredPosition = new Vector2( 2 + pos + dis , 0.0f );

                times--;

                if ( times > 0 && times % 2 == 1 )
                {
                    white.gameObject.SetActive( !white.gameObject.activeSelf );
                }
            }
        }
        else
        {
            if ( pos - dis > movePos )
            {
                start = false;
                pos = movePos;
                transRed.anchoredPosition = new Vector2( -2 - pos , 0.0f );

                white.gameObject.SetActive( false );
            }
            else
            {
                transRed.anchoredPosition = new Vector2( -2 - pos - dis , 0.0f );

                times--;

                if ( times > 0 && times % 2 == 1 )
                {
                    white.gameObject.SetActive( !white.gameObject.activeSelf );
                }
            }
        }

    }

}

