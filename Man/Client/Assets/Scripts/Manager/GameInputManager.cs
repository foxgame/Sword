using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public enum GameInputCode
{
    Up = 0,
    Down,
    Left,
    Right,

    Up1,
    Down1,

    Confirm,
    Cancel,

    Info,
    Debug,

    Count
}

public class GameInputManager : MonoBehaviour
{
    static bool[] lastInputAxisState = new bool[ (int)GameInputCode.Count ];

    public static bool getKey( GameInputCode c )
    {
        if ( GameTouchManager.instance.IsShow )
        {
            return GameTouchManager.instance.getKey( c );
        }

        switch ( c )
        {
            case GameInputCode.Info:
                {
                    return Input.GetAxis( "Info" ) > 0.1f;
                }
            case GameInputCode.Up1:
                {
                    return Input.GetAxis( "PageUp" ) > 0.1f;
                }
            case GameInputCode.Down1:
                {
                    return Input.GetAxis( "PageDown" ) > 0.1f;
                }
            case GameInputCode.Up:
                {
                    return Input.GetAxis( "Vertical" ) > 0.1f;
                }
            case GameInputCode.Down:
                {
                    return Input.GetAxis( "Vertical" ) < -0.1f;
                }
            case GameInputCode.Left:
                {
                    return Input.GetAxis( "Horizontal" ) < -0.1f;
                }
            case GameInputCode.Right:
                {
                    return Input.GetAxis( "Horizontal" ) > 0.1f;
                }
            case GameInputCode.Confirm:
                {
                    return Input.GetAxis( "Submit" ) > 0.1f;
                }
            case GameInputCode.Cancel:
                {
                    return Input.GetAxis( "Cancel" ) > 0.1f;
                }
        }

        return false;
    }

    public static bool getKeyDown( GameInputCode c )
    {
        if ( GameTouchManager.instance.IsShow )
        {
            return GameTouchManager.instance.getKeyDown( c );
        }

        switch ( c )
        {
            case GameInputCode.Debug:
                {
                    bool b = Input.GetAxis( "Debug" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Info:
                {
                    bool b = Input.GetAxis( "Info" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Up1:
                {
                    bool b = Input.GetAxis( "PageUp" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Down1:
                {
                    bool b = Input.GetAxis( "PageDown" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Up:
                {
                    bool b = Input.GetAxis( "Vertical" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Down:
                {
                    bool b = Input.GetAxis( "Vertical" ) < -0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Left:
                {
                    bool b = Input.GetAxis( "Horizontal" ) < -0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Right:
                {
                    bool b = Input.GetAxis( "Horizontal" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Confirm:
                {
                    bool b = Input.GetAxis( "Submit" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
            case GameInputCode.Cancel:
                {
                    bool b = Input.GetAxis( "Cancel" ) > 0.1f;

                    if ( b && lastInputAxisState[ (int)c ] )
                    {
                        return false;
                    }

                    lastInputAxisState[ (int)c ] = b;

                    return b;
                }
        }

        return false;
    }

}

