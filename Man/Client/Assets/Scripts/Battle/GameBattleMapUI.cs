using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleMapUI : GameUI<GameBattleMapUI>
{
    Image image;
    GameAnimation gameAnimation;
    RectTransform transImage;

    Vector2 size;

    public override void initSingleton()
    {
        gameAnimation = transform.Find( "map" ).GetComponentInChildren<GameAnimation>();
        image = transform.Find( "map/image" ).GetComponent<Image>();
        transImage = image.GetComponent<RectTransform>();

        size = transform.Find( "map" ).GetComponent<RectTransform>().sizeDelta;
    }

    public void clear()
    {
        GameDefine.DestroyAll( transImage );
    }

    public void moveLeft()
    {
        if ( transImage.sizeDelta.x < size.x )
        {
            return;
        }

        Vector2 v2 = transImage.anchoredPosition;
        v2.x--;

        if ( v2.x < -( transImage.sizeDelta.x - size.x ) * 0.5f )
        {
            return;
        }

        transImage.anchoredPosition = v2;
    }

    public void moveRight()
    {
        if ( transImage.sizeDelta.x < size.x )
        {
            return;
        }

        Vector2 v2 = transImage.anchoredPosition;
        v2.x++;

        if ( v2.x > ( transImage.sizeDelta.x - size.x ) * 0.5f )
        {
            return;
        }

        transImage.anchoredPosition = v2;
    }

    public void moveUp()
    {
        if ( transImage.sizeDelta.y < size.y )
        {
            return;
        }

        Vector2 v2 = transImage.anchoredPosition;
        v2.y++;

        if ( v2.y > ( transImage.sizeDelta.y - size.y ) * 0.5f )
        {
            return;
        }

        transImage.anchoredPosition = v2;
    }

    public void moveDown()
    {
        if ( transImage.sizeDelta.y < size.y )
        {
            return;
        }

        Vector2 v2 = transImage.anchoredPosition;
        v2.y--;

        if ( v2.y < -( transImage.sizeDelta.y - size.y ) * 0.5f )
        {
            return;
        }

        transImage.anchoredPosition = v2;
    }

    public void updateData()
    {
        int stage = GameUserData.instance.Stage;
        image.sprite = gameAnimation.sprites[ stage + 1 ];

        float w = image.sprite.rect.width;
        float h = image.sprite.rect.height;

        transImage.sizeDelta = new Vector2( w * 0.5f , h * 0.5f );
        transImage.anchoredPosition = new Vector2( 0.0f , 0.0f );

        clear();


        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;

        for ( int i = 0 ; i < units.Length ; i++ )
        {
            if ( !units[ i ].IsAlive )
            {
                continue;
            }

            GameObject obj = Instantiate<GameObject>( Resources.Load<GameObject>( "Prefab/Image" ) );

            RectTransform trans = obj.GetComponent<RectTransform>();
            trans.SetParent( image.transform );
            trans.anchoredPosition = new Vector2( units[ i ].PosX * 3 , -units[ i ].PosY * 3 );
            trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );
            trans.sizeDelta = new Vector2( 2.0f , 2.0f );

            Image image1 = obj.GetComponent<Image>();

            if ( units[ i ].IsEnemy )
            {
                image1.color = new Color( 1.0f , 0.0f , 0.0f , 1.0f );
            }
            else if ( units[ i ].IsUser )
            {
                image1.color = new Color( 0.0f , 0.0f , 1.0f , 1.0f );
            }
            else
            {
                image1.color = new Color( 0.0f , 1.0f , 0.0f , 1.0f );
            }

        }

    }

}
