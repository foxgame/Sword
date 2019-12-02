using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameUnitUI : GameUI<GameUnitUI>
{
    GameUnitUIInfo unitUIInfo;
    GameUnitUIItem unitUIItem;
    GameUnitUISkill unitUISkill;
    GameUnitUIStage unitUIStage;

    GameAnimation gameAnimation;

    int userID;

    public int UserID { get { return userID; } }

    public override void initSingleton()
    {
        unitUIStage = transform.Find( "unit" ).GetComponent<GameUnitUIStage>();
        unitUIInfo = transform.Find( "unitInfo" ).GetComponent<GameUnitUIInfo>();
        unitUIItem = transform.Find( "item" ).GetComponent<GameUnitUIItem>();
        unitUISkill = transform.Find( "skill" ).GetComponent<GameUnitUISkill>();
    }

    public void showNextUser( bool next )
    {
        if ( next )
        {
            userID = GameUserData.instance.nextUser( userID );

            if ( userID < 0 )
            {
                userID = 0;
            }
        }
        else
        {
            userID--;

            if ( userID < 0 )
            {
                userID = GameUserData.instance.lastUser();
            }
        }

        show( userID );
    }

    public void showNext()
    {
        if ( unitUIInfo.gameObject.activeSelf )
        {
            unitUIInfo.gameObject.SetActive( false );
            unitUIItem.gameObject.SetActive( true );
            unitUISkill.gameObject.SetActive( false );
        }
        else if ( unitUIItem.gameObject.activeSelf )
        {
            unitUIInfo.gameObject.SetActive( false );
            unitUIItem.gameObject.SetActive( false );
            unitUISkill.gameObject.SetActive( true );
        }
        else if ( unitUISkill.gameObject.activeSelf )
        {
            unitUIInfo.gameObject.SetActive( true );
            unitUIItem.gameObject.SetActive( false );
            unitUISkill.gameObject.SetActive( false );
        }
    }

    public override void onUnShow()
    {
        if ( gameAnimation != null )
        {
            gameAnimation.clearAnimation();
            Destroy( gameAnimation );
            gameAnimation = null;
        }
    }

    public void show( int id )
    {
        userID = id;

        show();

        GameUnitBase u = GameUserData.instance.getUnitBase( userID );

        updateData( u );

        unitUIInfo.gameObject.SetActive( true );
        unitUIItem.gameObject.SetActive( false );
        unitUISkill.gameObject.SetActive( false );

        updateAnimation();
    }

    public void show( GameBattleUnit battleUnit )
    {
        GameBattleUserLeftUI.instance.unShowFade();
        GameBattleUserRightUI.instance.unShowFade();

        userID = battleUnit.UnitID;

        show();

        updateData( battleUnit );

        unitUIInfo.gameObject.SetActive( true );
        unitUIItem.gameObject.SetActive( false );
        unitUISkill.gameObject.SetActive( false );

        updateAnimation();
    }

    public void updateData( GameUnitBase unitBase )
    {
        unitUIStage.updateData( unitBase );
        unitUIInfo.updateData( unitBase );
        unitUIItem.updateData( unitBase );
        unitUISkill.updateData( unitBase );
    }

    public void updateAnimation()
    {
        if ( gameAnimation != null )
        {
            gameAnimation.clearAnimation();
            Destroy( gameAnimation );
            gameAnimation = null;
        }

        GameUnit gameUnit = GameUnitData.instance.getData( userID );

        string path = "Prefab/Sprite/man" + GameDefine.getString3( gameUnit.Sprite ) + "/";
        path += ( GameDefine.getString3( gameUnit.Sprite ) + "man" );

        GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( path ) );
        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.UI = true;
        gameAnimation.playAnimationBattle( GameAnimationType.Stand , GameAnimationDirection.South , null );
        gameAnimation.transform.SetParent( unitUIStage.transform );
        gameAnimation.transform.localScale = Vector3.one;
        gameAnimation.transform.localPosition = new Vector3( -165.0f , 150.0f , 0.0f );
    }

    public void updateData( GameBattleUnit battleUnit )
    {
        unitUIStage.updateData( battleUnit );
        unitUIInfo.updateData( battleUnit );
        unitUIItem.updateData( battleUnit );
        unitUISkill.updateData( battleUnit );
    }

}


