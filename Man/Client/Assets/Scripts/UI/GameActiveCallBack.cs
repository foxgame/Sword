using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameActiveCallBack : MonoBehaviour
{
    public void onActive( string str )
    {
        if ( str.Contains( "1" ) )
        {
            PlayerPrefs.SetInt( "Active" , 1 );

            GameActiveUI.instance.unShow();
        }
    }
}

