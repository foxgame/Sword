using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public abstract class GameUI<T> : MonoBehaviour where T : GameUI<T>
{
    static private T Instance = null;
    static public T instance
    {
        get
        {
            return Instance;
        }
    }

    private void Awake()
    {
        if ( Instance == null )
        {
            Instance = this as T;
            Instance.initSingleton();
        }

        gameObject.SetActive( false );

        uiFade = GetComponentsInChildren<GameUIFade>();
    }

    public virtual void initSingleton()
    {

    }

    [SerializeField]
    protected GameUIFade[] uiFade = null;

    [SerializeField]
    protected bool isShow = false;
    [SerializeField]
    protected bool pause = false;

    public bool Pause { get { return pause; } set { pause = value; } }

    public bool IsShow { get { return isShow && !isFadeOut; } }

    [SerializeField]
    protected bool isFadeIn = false;
    [SerializeField]
    protected bool isFadeOut = false;
    [SerializeField]
    protected int fadeCount = 0;

    [SerializeField]
    protected bool autoUnshow = true;

    public bool AutoUnshow { get { return autoUnshow; } set { autoUnshow = value; } }

    public bool IsFadeIn { get { return isFadeIn; } }
    public bool IsFadeOut { get { return isFadeOut; } }


    public void show()
    {
        if ( !isShow )
        {
            gameObject.SetActive( true );
            isShow = true;

            onShow();
        }
    }

    public void unShow()
    {
        if ( isShow )
        {
            gameObject.SetActive( false );
            isShow = false;

            onUnShow();
        }
    }

    public virtual void unShowFade()
    {
        if ( !isShow || isFadeOut )
        {
            return;
        }

        isFadeOut = true;
        isFadeIn = false;
        fadeCount = 0;

        //        Debug.Log( "unShowFade" );

        for ( int i = 0 ; i < uiFade.Length ; i++ )
        {
            uiFade[ i ].startUIFade( false , onUIFadeOut );
        }
    }

    public virtual void showFade()
    {
        if ( !isShow )
        {
            return;
        }

        isFadeOut = false;
        isFadeIn = true;
        fadeCount = 0;

//        Debug.Log( "showFade" );

        for ( int i = 0 ; i < uiFade.Length ; i++ )
        {
            uiFade[ i ].startUIFade( true , onUIFadeIn );
        }
    }

    public void updateUIStageFadeOver()
    {
        for ( int i = 0 ; i < uiFade.Length ; i++ )
        {
            uiFade[ i ].updateUIStageFadeOver();
        }
    }

    protected virtual void onUIFadeOut()
    {
        fadeCount++;

        if ( fadeCount != uiFade.Length )
        {
            return;
        }

        isFadeOut = false;

        if ( autoUnshow )
        {
            updateUIStageFadeOver();
            unShow();
        }
    }

    protected virtual void onUIFadeIn()
    {
        fadeCount++;

        if ( fadeCount != uiFade.Length )
        {
            return;
        }

        isFadeIn = false;
        updateUIStageFadeOver();
    }

    public virtual void onShow()
    {

    }
    public virtual void onUnShow()
    {

    }

    protected virtual void onUpdate()
    {

    }

    private void Update()
    {
        if ( pause )
        {
            return;
        }

        onUpdate();

        if ( !( isFadeOut || isFadeIn ) || uiFade == null || uiFade.Length == 0 )
        {
            return;
        }

        for ( int i = 0 ; i < uiFade.Length ; i++ )
        {
            uiFade[ i ].updateFade();
        }
    }

}
