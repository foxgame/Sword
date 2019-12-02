using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


public static class ChineseStringUtility
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
    private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
    private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

    [DllImport( "kernel32" , CharSet = CharSet.Auto , SetLastError = true )]
    private static extern int LCMapString( int Locale , int dwMapFlags , string lpSrcStr , int cchSrc , [Out] string lpDestStr , int cchDest );
#endif

    public static string ToTraditional( string source )
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        String target = new String( ' ' , source.Length );
        int ret = LCMapString( LOCALE_SYSTEM_DEFAULT , LCMAP_TRADITIONAL_CHINESE , source , source.Length , target , source.Length );
        return target;
#else
        return source;
#endif
    }

    public static string ToSimplified( string source )
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        String target = new String( ' ' , source.Length );
        int ret = LCMapString( LOCALE_SYSTEM_DEFAULT , LCMAP_SIMPLIFIED_CHINESE , source , source.Length , target , source.Length );
        return target;
#else
        return source;
#endif
    }
}
