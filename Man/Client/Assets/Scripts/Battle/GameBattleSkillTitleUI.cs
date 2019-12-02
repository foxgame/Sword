using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class GameBattleSkillTitleUI : GameUI< GameBattleSkillTitleUI >
{
    float time = 0.0f;
    bool start = false;
    float alpha = 0.0f;
    float dis = 0.0f;
    bool fadeIn = false;

    float d = 20.0f;
    float t = 0.3f;

    Image image;
    Text text;
    RectTransform trans;

    Vector2 position;

    public override void initSingleton()
    {
        trans = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();

        position = trans.anchoredPosition;
    }


    public void show( string str )
    {
        text.text = str;
        fadeIn = true;
        alpha = 0.0f;
        start = true;
        dis = d;

        updateStage();

        gameObject.SetActive( true );
    }

    void updateStage()
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;

        c = image.color;
        c.a = alpha;
        image.color = c;

        trans.anchoredPosition = new Vector2( position.x , position.y + dis );
    }

    protected override void onUpdate()
    {
        if ( !start )
        {
            return;
        }

        if ( fadeIn )
        {
            time += Time.deltaTime;

            dis = d - time * d / t;
            alpha = time * ( 1.0f / t );

            if ( time > t )
            {
                alpha = 1.0f;
                fadeIn = false;
                time = -2.0f;
            }
        }
        else
        {
            time += Time.deltaTime;

            if ( time < 0.0f )
            {
                return;
            }

            dis = -time * d / t;
            alpha = 1.0f - time * ( 1.0f / t );

            if ( time > t )
            {
                start = false;
                time = 0.0f;
                gameObject.SetActive( false );
            }
        }

        updateStage();
    }

}
