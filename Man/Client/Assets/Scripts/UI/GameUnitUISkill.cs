using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUISkill : MonoBehaviour
{
    GameUnitUISkillSlot[] slot = new GameUnitUISkillSlot[ GameDefine.MAX_SLOT ];


    void Awake()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slot[ i ] = transform.Find( "slot" + i ).GetComponent<GameUnitUISkillSlot>();
        }
    }

    public void clear()
    {
        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slot[ i ].clear();
        }
    }


    public void updateData( GameBattleUnit battleUnit )
    {
        clear();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            GameSkill m = GameSkillData.instance.getData( battleUnit.Skill[ i ] );

            slot[ i ].setData( m );
        }
    }


    public void updateData( GameUnitBase unitBase )
    {
        clear();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            GameSkill m = GameSkillData.instance.getData( unitBase.Skills[ i ] );

            slot[ i ].setData( m );
        }
    }

}


