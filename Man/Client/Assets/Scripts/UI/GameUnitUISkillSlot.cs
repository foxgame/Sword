using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUISkillSlot : MonoBehaviour
{
    Text text;
    Text mp;
    Text move;

    Image image;

    void Awake()
    {
        image = transform.Find( "image" ).GetComponent<Image>();
        text = transform.Find( "name" ).GetComponent<Text>();
        mp = transform.Find( "mp" ).GetComponent<Text>();
        move = transform.Find( "move" ).GetComponent<Text>();
    }

    public void clear()
    {
        image.gameObject.SetActive( false );
        text.text = "";
        mp.text = "";
        move.text = "";
    }

    public void setData( GameSkill skill )
    {
        clear();

        if ( skill == null )
        {
            return;
        }

        text.text = skill.Name;

        mp.text = GameDefine.getBigInt( skill.MPCost.ToString() );
        move.text = GameDefine.getBigInt( skill.MoveCost.ToString() );

        image.gameObject.SetActive( true );
    }



}

