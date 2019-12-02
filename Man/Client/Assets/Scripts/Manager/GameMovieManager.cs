using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMovieManager : Singleton<GameMovieManager>
{
    RawImage rawImage;
    VideoPlayer videoPlayer;
    Text text;

    OnEventOver onEventOver;

    bool isPlaying;
    public bool IsPlaying { get { return isPlaying; } }

    public override void initSingleton()
    {
        rawImage = transform.Find( "Movie" ).GetComponent<RawImage>();
        videoPlayer = transform.Find( "Movie" ).GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += onPlayOver;

        rawImage.enabled = false;

        text = transform.Find( "Text" ).GetComponent< Text >();
        text.gameObject.SetActive( false );

        gameObject.SetActive( false );
    }

    public void playMovieCenter( string str , OnEventOver over )
    {
        RectTransform trans = GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2( 0.0f , 0.0f );

        videoPlayer.Stop();

        onEventOver = over;

        gameObject.SetActive( true );

        VideoClip clip = Resources.Load<VideoClip>( str );
        videoPlayer.clip = clip;
        videoPlayer.Play();

        rawImage.enabled = true;

        isPlaying = true;

#if UNITY_EDITOR
        Debug.Log( "playMovieCenter " + str + " " + clip );
#endif
    }

    public void playMovie( string str , OnEventOver over )
    {
        RectTransform trans = GetComponent<RectTransform>();
//        trans.anchoredPosition = new Vector2( GameCameraManager.instance.xOffset , 0.0f );

        videoPlayer.Stop();

        onEventOver = over;

        gameObject.SetActive( true );

        VideoClip clip = Resources.Load<VideoClip>( str );
        videoPlayer.clip = clip;
        videoPlayer.Play();

        rawImage.enabled = true;

        isPlaying = true;

#if UNITY_EDITOR
        Debug.Log( "playMovie " + str + " " + clip );
#endif
    }

    public void onClick()
    {
        if ( text.gameObject.activeSelf )
        {
            stopMovie();
        }
        else
        {
            text.gameObject.SetActive( true );
        }
    }

    public void stopMovie()
    {
        isPlaying = false;

        videoPlayer.Stop();
        rawImage.enabled = false;
        text.gameObject.SetActive( false );

        gameObject.SetActive( false );

        if ( onEventOver != null )
        {
            onEventOver();
        }

        if ( !isPlaying )
        {
            videoPlayer.targetTexture.Release();
        }
    }

    public void onPlayOver( VideoPlayer p )
    {
#if UNITY_EDITOR
        Debug.Log( "onPlayOver" );
#endif
        stopMovie();
    }



}
