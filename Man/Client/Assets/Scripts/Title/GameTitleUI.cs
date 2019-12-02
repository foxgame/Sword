using UnityEngine;
using UnityEngine.UI;

public class GameTitleUI : GameUI<GameTitleUI>
{
    Text[] text = new Text[ 4 ];
    Text agreement;
    Text version;

    int selection = 0;

    Color color;
    
    public int Selection { get { return selection; } }



    public override void initSingleton()
    {
        text[ 0 ] = transform.Find( "title/new" ).GetComponent<Text>();
        text[ 1 ] = transform.Find( "title/load" ).GetComponent<Text>();
        text[ 2 ] = transform.Find( "title/loadBattle" ).GetComponent<Text>();
        text[ 3 ] = transform.Find( "title/help" ).GetComponent<Text>();

        agreement = transform.Find( "agreement" ).GetComponent<Text>();
        version = transform.Find( "version" ).GetComponent<Text>();

        color = text[ 0 ].color;
    }

    public override void onShow()
    {
        version.text = GameSetting.instance.getVersion();
        agreement.text = GameStringData.instance.getString( GameStringType.Agreement0 ) + "\n" + SystemInfo.deviceUniqueIdentifier;
    }

    public void select( int i )
    {
        if ( i < 0 )
        {
            i = 3;
        }

        if ( i > 3 )
        {
            i = 0;
        }

        selection = i;

        updateText();
    }

    public void updateText()
    {
        for ( int i = 0 ; i < 4 ; i++ )
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
            }
        }
    }

}
