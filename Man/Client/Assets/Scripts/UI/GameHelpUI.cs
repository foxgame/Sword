using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameHelpUI : GameUI<GameHelpUI>
{
    Text[] text = new Text[ 12 ];
    int selection = 0;
    Color color;
    public int Selection { get { return selection; } }

    Text setting00;
    Text setting10;
    Text setting2;

    public override void initSingleton()
    {
        text[ 0 ] = transform.Find( "button0/text" ).GetComponent<Text>();
        text[ 1 ] = transform.Find( "button1/text" ).GetComponent<Text>();
        text[ 2 ] = transform.Find( "button2/text" ).GetComponent<Text>();
        text[ 3 ] = transform.Find( "button3/text" ).GetComponent<Text>();
        text[ 4 ] = transform.Find( "button4/text" ).GetComponent<Text>();
        text[ 5 ] = transform.Find( "button5/text" ).GetComponent<Text>();

        text[ 6 ] = transform.Find( "setting0/text" ).GetComponent<Text>();
        text[ 7 ] = transform.Find( "setting1/text" ).GetComponent<Text>();

        text[ 6 ] = transform.Find( "setting0/text" ).GetComponent<Text>();
        text[ 7 ] = transform.Find( "setting1/text" ).GetComponent<Text>();

        text[ 8 ] = transform.Find( "setting20" ).GetComponent<Text>();
        text[ 9 ] = transform.Find( "setting21" ).GetComponent<Text>();

        text[ 10 ] = transform.Find( "button6/text" ).GetComponent<Text>();
        text[ 11 ] = transform.Find( "button7/text" ).GetComponent<Text>();

        if ( PlayerPrefs.GetInt( "review" , 0 ) > 0 )
            text[ 10 ].text = GameStringData.instance.getString( GameStringType.Help10 );
        else
            text[ 10 ].text = GameStringData.instance.getString( GameStringType.Help00 );

        setting00 = transform.Find( "setting00" ).GetComponent<Text>();
        setting10 = transform.Find( "setting10" ).GetComponent<Text>();

        setting2 = transform.Find( "setting2/text" ).GetComponent<Text>();

        color = text[ 0 ].color;
    }

    public override void onShow()
    {
        updateSettingText();
    }

    public void select( int i )
    {
        if ( i < 0 )
        {
            i = 11;
        }

        if ( i > 11 )
        {
            i = 0;
        }

        selection = i;

        updateText();
    }

    public void onSetting0()
    {
        if ( GameSetting.instance.mode == GameSetting.GameMode.Normal )
        {
            GameSetting.instance.mode = GameSetting.GameMode.Hard;
        }
        else
        {
            GameSetting.instance.mode = GameSetting.GameMode.Normal;
        }

        updateSettingText();

        GameSetting.instance.save();
    }

    public void onSetting1()
    {
        if ( GameSetting.instance.location == GameSetting.GameLocation.SimplifiedChinese )
        {
            GameSetting.instance.location = GameSetting.GameLocation.TraditionalChinese;
        }
        else
        {
            GameSetting.instance.location = GameSetting.GameLocation.SimplifiedChinese;
        }

        GameObject objCanvas = GameObject.Find( "Canvas" );
        GameLocalization[] d = objCanvas.GetComponentsInChildren<GameLocalization>();

        for ( int i = 0 ; i < d.Length ; i++ )
        {
            d[ i ].updateText();
        }

        updateSettingText();

        GameSetting.instance.save();
    }

    public void onSetting20()
    {
        GameSetting.instance.touchScale -= 0.1f;

        if ( GameSetting.instance.touchScale < 1.0f )
        {
            GameSetting.instance.touchScale = 1.0f;
        }

        GameTouchLeftUI.instance.transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );
        GameTouchRightUI.instance.transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );

        GameSetting.instance.save();
    }

    public void onSetting21()
    {
        GameSetting.instance.touchScale += 0.1f;

        if ( GameSetting.instance.touchScale > 1.5f )
        {
            GameSetting.instance.touchScale = 1.5f;
        }

        GameTouchLeftUI.instance.transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );
        GameTouchRightUI.instance.transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );

        GameSetting.instance.save();
    }


    public void updateSettingText()
    {
        if ( GameSetting.instance.mode == GameSetting.GameMode.Normal )
        {
            setting00.text = GameStringData.instance.getString( GameStringType.Setting01 );
        }
        else
        {
            setting00.text = GameStringData.instance.getString( GameStringType.Setting02 );
        }

        if ( GameSetting.instance.location == GameSetting.GameLocation.SimplifiedChinese )
        {
            setting10.text = GameStringData.instance.getString( GameStringType.Setting11 );
        }
        else
        {
            setting10.text = GameStringData.instance.getString( GameStringType.Setting12 );
        }
    }

    public void updateText()
    {
        for ( int i = 0 ; i < 12 ; i++ )
        {
            Color c = color;

            if ( selection == i )
            {
                c.a = 1.0f;
                text[ i ].color = c;
            }
            else
            {
                c.a = 0.3f;
                text[ i ].color = c;

                setting00.color = c;
                setting10.color = c;

                setting2.color = c;
            }
        }

        if ( selection == 6 )
            setting00.color = text[ selection ].color;

        if ( selection == 7 )
            setting10.color = text[ selection ].color;

        if ( selection == 8 || selection == 9 )
            setting2.color = text[ selection ].color;
    }

    void Start()
    {
    }

    public void onClick0()
    {
        Application.OpenURL( "https://dynasty.xin/index.php?c=show&id=132" );       
    }

    public void onClick1()
    {
        Application.OpenURL( "https://dynasty.xin/index.php?c=show&id=127" );
    }

    public void onClick2()
    {
        Application.OpenURL( "https://dynasty.xin/index.php?c=show&id=129" );
    }

    public void onClick3()
    {
        Application.OpenURL( "https://dynasty.xin/index.php?c=show&id=131" );
    }

    public void onClick4()
    {
        Application.OpenURL( "https://dynasty.xin/index.php?c=show&id=130" );
    }


    public void onClick5()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        GameManager.joinGroup1();
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );

        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>( "currentActivity" );

        jo.Call( "joinQQGroup1" , "pwMD-7s71mgVc-4rMpogwzhcw6vB3_Mv" );
#else
        Application.OpenURL( "https://shang.qq.com/wpa/qunwpa?idkey=471f2c42f065e9be59b82e2d7883c2b55e3caaf6b55414cf89e486a4c5c735c5" );
#endif
    }

    public void onClick6()
    {
        if ( PlayerPrefs.GetInt( "review" , 0 ) > 0 )
        {
            return;
        }

        PlayerPrefs.SetInt( "review" , 2 );

#if UNITY_IPHONE
        const string APP_ID = "1461510167";
        var url = string.Format(
            "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review" ,
            APP_ID );
        Application.OpenURL( url );
#endif
    }

    public void onClick7()
    {
        unShow();
    }

}
