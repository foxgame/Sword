using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


class GameSLUISlot : MonoBehaviour
{
    Image imageBackground;
    Image image;
    Text text;
    Text lvText;

    Text Proficiency0;
    Text Proficiency1;
    Text Turn0;
    Text Turn1;
    Text time;

    [ SerializeField]
    bool selection;

    [SerializeField]
    float color = 1.0f;
    [SerializeField]
    bool fade = false;

    private void Awake()
    {
        image = transform.Find( "image" ).GetComponent<Image>();
        text = transform.Find( "text" ).GetComponent<Text>();
        lvText = transform.Find( "lv" ).GetComponent<Text>();

        Proficiency0 = transform.Find( "Proficiency0" ).GetComponent<Text>();
        Proficiency1 = transform.Find( "Proficiency1" ).GetComponent<Text>();
        Turn0 = transform.Find( "Turn0" ).GetComponent<Text>();
        Turn1 = transform.Find( "Turn1" ).GetComponent<Text>();
        time = transform.Find( "time" ).GetComponent<Text>();

        imageBackground = GetComponent<Image>();
    }


    public void clear()
    {
        imageBackground.color = Color.white;
        image.color = Color.white;

        selection = false;
        fade = true;
        color = 1.0f;
    }

    public void select()
    {
        selection = true;
        fade = true;
        color = 1.0f;
    }

    public void setData( GameSaveDataInfo info )
    {
        if ( info.Stage == 0 )
        {
            image.gameObject.SetActive( false );
            text.text = "";
            lvText.text = "";
            Proficiency0.text = "";
            Proficiency1.text = "";
            Turn0.text = "";
            Turn1.text = "";
            time.text = "";
            return;
        }

        selection = false;

        image.gameObject.SetActive( true );

        GameBattleStage stage = GameBattleData.instance.getStage( info.Stage );
        text.text = stage.SDES.Title;
        lvText.text = GameDefine.getBigInt( info.LV.ToString() );

        Proficiency0.text = GameStringData.instance.getString( GameStringType.SL0 );
        Proficiency1.text = GameDefine.getBigInt( info.Proficiency.ToString() ) + " ";

        if ( info.Proficiency < info.Stage - info.Stage / 5 )
        {
            Proficiency1.text += GameStringData.instance.getString( GameStringType.SL2 );
        }
        else
        {
            Proficiency1.text += GameStringData.instance.getString( GameStringType.SL3 );
        }

        Turn0.text = GameStringData.instance.getString( GameStringType.SL4 );
        Turn1.text = GameDefine.getBigInt( info.TurnCount.ToString() );

        DateTime dt = DateTime.Parse( "1970-01-01 00:00:00" ).AddSeconds( info.TimeData );
        time.text = dt.ToLocalTime().ToString( "yyyy-MM-dd HH:mm:ss" ) + " ";

        dt = DateTime.Parse( "1970-01-01 00:00:00" ).AddSeconds( info.Time );
        time.text += GameStringData.instance.getString( GameStringType.Time0 ) + string.Format( "{0:T}" , dt );
    }

    void Update()
    {
        if ( !selection )
        {
            return;
        }

        float f = 0.3f;

        if ( fade )
        {
            color -= Time.deltaTime * f;

            if ( color < 0.7f )
            {
                color = 0.7f;
                fade = false;
            }
        }
        else
        {
            color += Time.deltaTime * f;

            if ( color > 0.9f )
            {
                color = 0.9f;
                fade = true;
            }
        }

        Color c = new Color( color , 1.0f , 1.0f );
        imageBackground.color = c;
        image.color = c;
    }

}

