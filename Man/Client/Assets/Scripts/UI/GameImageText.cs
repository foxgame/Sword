using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameImageText : Text
{
    [SerializeField]
    UIVertex uiVertex;

    GameAnimation gameAnimation;

    float speed = 0.0f;
    bool start = false;
    string text1 = "";
    int showIndex = 0;
    bool isOver = false;
    float time = 0.0f;
    int line = 0;

    bool isStopLine = false;

    public bool IsStopLine { get { return isStopLine; } }
    public bool IsOver { get { return isOver; } }

    protected override void Awake()
    {
        base.Awake();

        gameAnimation = GetComponentInChildren<GameAnimation>();
    }

    public void clear()
    {
        text = "";

        gameAnimation.stopAnimation();
        gameAnimation.clearAnimation();
    }

    public void showText( string str )
    {
        text = "";
        isOver = false;
        start = true;
        text1 = str;
        speed = 0.1f;
        time = 0.0f;
        showIndex = 0;
        line = 0;
    }

    protected override void OnPopulateMesh( VertexHelper toFill )
    {
        base.OnPopulateMesh( toFill );

        if ( toFill.currentVertCount == 0 )
            return;

        toFill.PopulateUIVertex( ref uiVertex , toFill.currentVertCount - 1 );
    }

    void updateText()
    {
        showIndex++;

        if ( showIndex > text1.Length )
        {
            start = false;
            text = text1;
            isOver = true;

            gameAnimation.offsetX = (int)( ( uiVertex.position.x + 20 ) / 2.0f );
            gameAnimation.offsetY = (int)( ( uiVertex.position.y + 20 ) / 2.0f );
            gameAnimation.playAnimation( 27 , 41 );

            return;
        }

        if ( showIndex > 20 &&
            uiVertex.position.y < 20 && uiVertex.position.x > 500 )
        {
            line = 4;
            showIndex--;
        }

        text = text1.Substring( 0 , showIndex );

        if ( text[ text.Length - 1 ] == '\n' )
        {
            line++;
        }

        if ( line > 3 )
        {
            isStopLine = true;
            speed = 0.1f;
            text1 = text1.Remove( 0 , showIndex );
            showIndex = 0;
            line = 0;

            gameAnimation.offsetX = (int)( ( uiVertex.position.x + 20 ) / 2.0f );
            gameAnimation.offsetY = (int)( ( uiVertex.position.y + 20 ) / 2.0f );
            gameAnimation.playAnimation( 27 , 41 );
        }

    }

    public void onClick()
    {
        if ( isStopLine )
        {
            isStopLine = false;
            gameAnimation.stopAnimation();
            gameAnimation.clearAnimation();
            return;
        }

        speed = 0.0f;
    }

    void Update()
    {
        if ( !start ||
            isStopLine )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time < speed )
        {
            return;
        }

        time = 0.0f;

        updateText();
    }
}
