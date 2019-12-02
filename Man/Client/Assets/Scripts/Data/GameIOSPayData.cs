using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;


#if UNITY_IPHONE

public class GameIOSPayData : SingletonManager< GameIOSPayData > 
{

	public void release()
	{
		inited = false;
		isload = false;
	}

    public void init()
    {
#if UNITY_EDITOR
        return;
#endif
        if (inited)
        {
            return;
        }

        inited = true;

        IAPClearList();

        IAPGetList();
    }

	[ DllImport("__Internal") ]
	private static extern void iapBuyGoods( int n );

	[ DllImport("__Internal") ]
	private static extern void iapFinishBuyGoods();

	[ DllImport("__Internal") ]
	private static extern void iapClearList();

	[ DllImport("__Internal") ]
	private static extern void iapGetList();

	string strLast = "";

	public bool isload = false;
	public bool inited = false;

	public void IAPRequest( string s )
	{
		isload = true;

		if ( strLast.Length > 1 )
		{
            GamePHP.instance.phpIOSPay( strLast , onPayOver);
   		}
	}
	public void IAPComplete( string s )
	{
		strLast = s;

		if ( !isload ) 
		{
			return;
		}

        GamePHP.instance.phpIOSPay( strLast , onPayOver );
    }
    public void IAPFailed( string s )
	{
		strLast = "";
	}
    public void onPayOver( int n , byte[] data )
    {
        if ( n == 1 )
        {
            IAPFinishBuyGoods();
            GameActiveUI.instance.unShow();
        }
    }

    public void IAPBuyGoods( int ii )
	{
		strLast = ii.ToString();

		if ( !isload ) 
		{
			return;
		}

		if ( strLast.Length > 1 ) 
		{
			return;
		}

		iapBuyGoods( ii );
		strLast = ii.ToString();
	}

	public void IAPClearList()
	{
		iapClearList();
	}

	public void IAPGetList()
	{
		iapGetList();
	}

	public void IAPFinishBuyGoods()
	{
		strLast = "";
		iapFinishBuyGoods();
	}

}

#endif
