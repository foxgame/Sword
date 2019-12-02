using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class GameBattleJumpHPUIText : MonoBehaviour
{
    RectTransform trans;
    float time = 0.0f;
    bool start = false;
    Vector2 pos;
    float dis = 0.0f;

    OnEventOver onEventOver;

    void Awake()
    {
        trans = GetComponent<RectTransform>();
    }

    public void jump( float t , OnEventOver over )
    {
        onEventOver = over;
        time = t;
        start = true;

        pos = trans.anchoredPosition;
    }

    void Update()
    {
        if ( !start )
        {
            return;
        }

        float d = 30.0f;
        float t = 0.2f;

        time += Time.deltaTime;

        if ( time >= 0.0f && time < t * 0.5f )
        {
            dis = time * d;
        }
        else if ( time >= t * 0.5f && time <= t )
        {
            dis = t * d - time * d;
        }

        trans.anchoredPosition = new Vector2( pos.x , pos.y + dis );

        if ( time > t )
        {
            trans.anchoredPosition = pos;

            if ( time > 0.5f )
            {
                start = false;
                onEventOver();
            }
        }
    }
}

