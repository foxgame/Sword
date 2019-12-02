using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSkillUI : GameUI<GameBattleSkillUI>
{
    GameBattleSkillUISlot[] slots = new GameBattleSkillUISlot[ GameDefine.MAX_SLOT ];
    Text description;
    int selection = 0;


    public int Selection { get { return selection; } }

    public override void initSingleton()
    {
        description = transform.Find( "description/text" ).GetComponent<Text>();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ] = transform.Find( "slot" + i ).GetComponent< GameBattleSkillUISlot >();
        }
    }

    
    public GameSkill getSelectionSkill()
    {
        if ( selection == GameDefine.INVALID_ID )
        {
            return null;
        }

        if ( !slots[ selection ].Enabled )
        {
            return null;
        }

        return slots[ selection ].Skill;
    }

    public void select( int s )
    {
        if ( s < 0 || s >= GameDefine.MAX_SLOT )
        {
            return;
        }

        if ( slots[ s ].Skill == null )
        {
            return;
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].selection( false );
        }

        selection = s;
        slots[ selection ].selection( true );

        description.text = slots[ s ].Skill.Description;
    }

    public void setData( GameBattleUnit unit )
    {
        description.text = "";

        selection = GameDefine.INVALID_ID;

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].clear();

            if ( unit.Skill[ i ] == GameDefine.INVALID_ID )
            {
                continue;
            }

            GameSkill m = GameSkillData.instance.getData( unit.Skill[ i ] );

            slots[ i ].setData( m );
            slots[ i ].enable( unit.canUseSkill( m ) );

            if ( i == 0 )
            {
                select( 0 );
            }
        }
    }


}

