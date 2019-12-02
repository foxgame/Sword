using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScale : Singleton<GameCanvasScale>
{
    public int Height
    {
        get
        {
            return (int)( trans.sizeDelta.y );
        }
    }

    public int Width
    {
        get
        {
            return (int)( trans.sizeDelta.x );
        }
    }

    RectTransform trans;

    private void Start()
    {
        trans = GetComponent<RectTransform>();

        RectTransform transParent = trans.parent.GetComponent<RectTransform>();

        trans.sizeDelta = new Vector2( transParent.sizeDelta.x / trans.localScale.x ,
            transParent.sizeDelta.y / trans.localScale.y );
    }
}

