using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSkillUISlot : MonoBehaviour
{
    Image select;
    Image icon;
    Text text;
    Text sp;
    Text mv;

    bool enabled1;
    GameSkill skill;

    public bool Enabled { get { return enabled1; } }
    public GameSkill Skill { get { return skill; } }

    void Awake()
    {
        select = transform.Find( "select" ).GetComponent<Image>();
        icon = transform.Find( "icon" ).GetComponent<Image>();
        text = transform.Find( "text" ).GetComponent<Text>();
        sp = transform.Find( "sp" ).GetComponent<Text>();
        mv = transform.Find( "mv" ).GetComponent<Text>();
    }

    public void selection( bool b )
    {
        select.gameObject.SetActive( b );
    }

    public void clear()
    {
        select.gameObject.SetActive( false );
        icon.gameObject.SetActive( false );

        text.text = "";
        sp.text = "";
        mv.text = "";

        skill = null;
    }

    public void setData( GameSkill m )
    {
        clear();

        if ( m == null )
        {
            return;
        }

        skill = m;

        text.text = m.Name;
        sp.text = GameDefine.getBigInt( m.MPCost.ToString() );
        mv.text = GameDefine.getBigInt( m.MoveCost.ToString() );

        icon.gameObject.SetActive( true );
    }

    public void enable( bool b )
    {
        enabled1 = b;

        Color c0 = Color.white;
        Color c1 = new Color( 1.0f , 1.0f , 1.0f , 0.2f );

//        select.color = b ? c0 : c1;
        icon.color = b ? c0 : c1;

        Color tc0 = text.color; tc0.a = 1.0f;
        Color tc1 = text.color; tc1.a = 0.2f;
        text.color = b ? tc0 : tc1;

        Color sp0 = sp.color; sp0.a = 1.0f;
        Color sp1 = sp.color; sp1.a = 0.2f;
        sp.color = b ? sp0 : sp1;

        Color mv0 = mv.color; mv0.a = 1.0f;
        Color mv1 = mv.color; mv1.a = 0.2f;
        mv.color = b ? mv0 : mv1;
    }


}


