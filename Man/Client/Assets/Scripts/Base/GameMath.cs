using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;



public class GameMath
{
    static Dictionary<int , float> sqrtData = new Dictionary<int , float>();

    public static void init()
    {
        for ( int i = 0 ; i < 65535 ; i++ )
        {
            sqrtData[ i ] = Mathf.Sqrt( i );
        }
    }




    public static float sqrt( int dx , int dz )
    {
        return sqrt( dx * dx + dz * dz );
    }

    public static float sqrt( int n )
    {
        float v = 0.0f;

        if ( sqrtData.TryGetValue( n , out v ) )
        {
            return v;
        }

        return Mathf.Sqrt( n );
    }

}
