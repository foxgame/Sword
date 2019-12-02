using System.Runtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameSessionPlayer 
{
	public int guid;
	public GameBitArray32 name;

	public GameBitArray32BytesChat unknow0;
	public GameBitArray32BytesChat unknow1;
};



[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameBitArraySessionPlayer //: IDisposable
{
	GameSessionPlayer a0;
	GameSessionPlayer a1;
	GameSessionPlayer a2;
	GameSessionPlayer a3;
	GameSessionPlayer a4;
	GameSessionPlayer a5;
	GameSessionPlayer a6;
	GameSessionPlayer a7;
	GameSessionPlayer a8;
	GameSessionPlayer a9;

	public GameSessionPlayer this[ int i ]
	{
		get
		{
			switch ( i ) 
			{
				case 0:
					return a0;
				case 1:
					return a1;
				case 2:
					return a2;
				case 3:
					return a3;
				case 4:
					return a4;
				case 5:
					return a5;
				case 6:
					return a6;
				case 7:
					return a7;
				case 8:
					return a8;
				case 9:
					return a9;

				default:
					return a0;
			}
		}
		set
		{
			switch ( i )
			{
				case 0:
					a0 = value;
					break;
				case 1:
					a1 = value;
					break;
				case 2:
					a2 = value;
					break;
				case 3:
					a3 = value;
					break;
				case 4:
					a4 = value;
					break;
				case 5:
					a5 = value;
					break;
				case 6:
					a6 = value;
					break;
				case 7:
					a7 = value;
					break;
				case 8:
					a8 = value;
					break;
				case 9:
					a9 = value;
					break;

				default:
					a0 = value;
					break;
			}
		}
	}

}



[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameSessionListData
{
	public int ID;
	public int mapID;
	public GameBitArray32 name;

	public GameBitArray32BytesChat unknow0;
	public GameBitArray32BytesChat unknow1;

};

[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameSessionData 
{
	public int ID;
	public int mapID;
	public GameBitArray32 name;

	public GameBitArray32BytesChat unknow0;
	public GameBitArray32BytesChat unknow1;

	public int num;
	public GameBitArraySessionPlayer players;
};



[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameBitArraySessionListData //: IDisposable
{
	GameSessionListData a0;
	GameSessionListData a1;
	GameSessionListData a2;
	GameSessionListData a3;
	GameSessionListData a4;
	GameSessionListData a5;
	GameSessionListData a6;
	GameSessionListData a7;
	GameSessionListData a8;
	GameSessionListData a9;

	public GameSessionListData this[ int i ]
	{
		get
		{
			switch ( i ) 
			{
				case 0:
					return a0;
				case 1:
					return a1;
				case 2:
					return a2;
				case 3:
					return a3;
				case 4:
					return a4;
				case 5:
					return a5;
				case 6:
					return a6;
				case 7:
					return a7;
				case 8:
					return a8;
				case 9:
					return a9;

				default:
					return a0;
			}
		}
		set
		{
			switch ( i )
			{
				case 0:
					a0 = value;
					break;
				case 1:
					a1 = value;
					break;
				case 2:
					a2 = value;
					break;
				case 3:
					a3 = value;
					break;
				case 4:
					a4 = value;
					break;
				case 5:
					a5 = value;
					break;
				case 6:
					a6 = value;
					break;
				case 7:
					a7 = value;
					break;
				case 8:
					a8 = value;
					break;
				case 9:
					a9 = value;
					break;

				default:
					a0 = value;
					break;
			}
		}
	}

}
