using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMusicManager : Singleton< GameMusicManager >
{
    AudioSource audioSource;
    List<AudioClip> soundList = new List<AudioClip>(); 

    Dictionary<int , string> musicDic = new Dictionary<int , string>();

    int activeID = 0;

    float maxValue = 0.8f;

    public void clearMusic()
    {
        stopMusic();

        musicDic.Clear();
        soundList.Clear();
    }

    public override void initSingleton()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playSound( string str )
    {
        if ( !GameSetting.instance.enabledSound )
        {
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>( str );

        if ( clip == null )
        {
            return;
        }

        soundList.Add( clip );

        if ( soundList.Count > 10 )
        {
            soundList.RemoveAt( 0 );
        }

        audioSource.PlayOneShot( clip );
    }

    public void musicVolume( int id , float v )
    {
        if ( activeID == id )
        {
            audioSource.volume = v;
        }
    }

    public void playMusic( int id , string str )
    {
        if ( !GameSetting.instance.enabledMusic )
        {
            return;
        }

        audioSource.Stop();

        AudioClip clip = Resources.Load<AudioClip>( str );

        audioSource.clip = clip;
        audioSource.Play();

        if ( !musicDic.ContainsKey( id ) )
        {
            musicDic.Add( id , str );
        }

        activeID = id;
    }

    public void stopMusic( int id )
    {
        if ( activeID == id )
        {
            audioSource.Stop();
        }
    }

    public void stopMusic()
    {
        audioSource.Stop();
        audioSource.volume = maxValue;
    }

    public void pauseMusic( int id , bool b )
    {
        if ( b )
        {
            if ( activeID == id )
            {
                audioSource.Stop();
            }
        }
        else
        {
            if ( !GameSetting.instance.enabledMusic )
            {
                return;
            }

            if ( activeID != id )
            {
                audioSource.Stop();
                AudioClip clip = Resources.Load<AudioClip>( musicDic[ id ] );
                audioSource.clip = clip;

                activeID = id;
            }

            audioSource.Play();
            audioSource.volume = maxValue;
        }
    }


}
