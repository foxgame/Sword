using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleGetItemUI : GameUI<GameBattleGetItemUI>
{
    Text text;
    OnEventOver onEventOver;
    float time = 0.0f;

    public override void initSingleton()
    {
        text = transform.Find( "text" ).GetComponent<Text>();
    }

    public override void onUnShow()
    {
        if ( onEventOver != null )
        {
            onEventOver();
        }
    }

    protected override void onUpdate()
    {
        if ( !IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( time > 5.0f )
        {
            unShowFade();
            time = 0.0f;
        }
    }

    public void show( int itemID , int gold , OnEventOver over )
    {
        onEventOver = over;

        show();

        string str;

        if ( itemID != GameDefine.INVALID_ID )
        {
            str = GameMessageData.instance.getData( GameMessageType.Get1 ).message[ 0 ][ 1 ];

            GameItem item = GameItemData.instance.getData( itemID );

            str = str.Replace( "0" , item.Name );
        }
        else
        {
            str = GameMessageData.instance.getData( GameMessageType.Get1 ).message[ 0 ][ 0 ];

            str = str.Replace( "0" , GameDefine.getBigInt( gold.ToString() ) );
        }

        text.text = str;

        time = 0.0f;
    }



}
