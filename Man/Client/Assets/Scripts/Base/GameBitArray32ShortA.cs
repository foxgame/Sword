using System.Runtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameBitArray32ShortA //: IDisposable
{
	GameBitArray32Short a0;
	GameBitArray32Short a1;
	GameBitArray32Short a2;
	GameBitArray32Short a3;
	GameBitArray32Short a4;
	GameBitArray32Short a5;
	GameBitArray32Short a6;
	GameBitArray32Short a7;
	GameBitArray32Short a8;
	GameBitArray32Short a9;
	GameBitArray32Short a10;
	GameBitArray32Short a11;
	GameBitArray32Short a12;
	GameBitArray32Short a13;
	GameBitArray32Short a14;
	GameBitArray32Short a15;
	GameBitArray32Short a16;
	GameBitArray32Short a17;
	GameBitArray32Short a18;
	GameBitArray32Short a19;
	GameBitArray32Short a20;
	GameBitArray32Short a21;
	GameBitArray32Short a22;
	GameBitArray32Short a23;
	GameBitArray32Short a24;
	GameBitArray32Short a25;
	GameBitArray32Short a26;
	GameBitArray32Short a27;
	GameBitArray32Short a28;
	GameBitArray32Short a29;
	GameBitArray32Short a30;
	GameBitArray32Short a31;
	
	
	
	
	
	public short this[ int i ]
	{
		get
		{
			int i0 = i / 32;
			int i1 = i % 32;
			
			switch ( i0 ) 
			{
			case 0:
				return a0[ i1 ];
			case 1:
				return a1[ i1 ];
			case 2:
				return a2[ i1 ];
			case 3:
				return a3[ i1 ];
			case 4:
				return a4[ i1 ];
			case 5:
				return a5[ i1 ];
			case 6:
				return a6[ i1 ];
			case 7:
				return a7[ i1 ];
			case 8:
				return a8[ i1 ];
			case 9:
				return a9[ i1 ];
			case 10:
				return a10[ i1 ];
			case 11:
				return a11[ i1 ];
			case 12:
				return a12[ i1 ];
			case 13:
				return a13[ i1 ];
			case 14:
				return a14[ i1 ];
			case 15:
				return a15[ i1 ];
			case 16:
				return a16[ i1 ];
			case 17:
				return a17[ i1 ];
			case 18:
				return a18[ i1 ];
			case 19:
				return a19[ i1 ];
			case 20:
				return a20[ i1 ];
			case 21:
				return a21[ i1 ];
			case 22:
				return a22[ i1 ];
			case 23:
				return a23[ i1 ];
			case 24:
				return a24[ i1 ];
			case 25:
				return a25[ i1 ];
			case 26:
				return a26[ i1 ];
			case 27:
				return a27[ i1 ];
			case 28:
				return a28[ i1 ];
			case 29:
				return a29[ i1 ];
			case 30:
				return a30[ i1 ];
			case 31:
				return a31[ i1 ];
				
			default:
				return a0[ i1 ];
			}
		}
		set
		{
			int i0 = i / 32;
			int i1 = i % 32;
			
			switch ( i0 )
			{
			case 0:
				a0[ i1 ] = value;
				break;
			case 1:
				a1[ i1 ] = value;
				break;
			case 2:
				a2[ i1 ] = value;
				break;
			case 3:
				a3[ i1 ] = value;
				break;
			case 4:
				a4[ i1 ] = value;
				break;
			case 5:
				a5[ i1 ] = value;
				break;
			case 6:
				a6[ i1 ] = value;
				break;
			case 7:
				a7[ i1 ] = value;
				break;
			case 8:
				a8[ i1 ] = value;
				break;
			case 9:
				a9[ i1 ] = value;
				break;
			case 10:
				a10[ i1 ] = value;
				break;
			case 11:
				a11[ i1 ] = value;
				break;
			case 12:
				a12[ i1 ] = value;
				break;
			case 13:
				a13[ i1 ] = value;
				break;
			case 14:
				a14[ i1 ] = value;
				break;
			case 15:
				a15[ i1 ] = value;
				break;
			case 16:
				a16[ i1 ] = value;
				break;
			case 17:
				a17[ i1 ] = value;
				break;
			case 18:
				a18[ i1 ] = value;
				break;
			case 19:
				a19[ i1 ] = value;
				break;
			case 20:
				a20[ i1 ] = value;
				break;
			case 21:
				a21[ i1 ] = value;
				break;
			case 22:
				a22[ i1 ] = value;
				break;
			case 23:
				a23[ i1 ] = value;
				break;
			case 24:
				a24[ i1 ] = value;
				break;
			case 25:
				a25[ i1 ] = value;
				break;
			case 26:
				a26[ i1 ] = value;
				break;
			case 27:
				a27[ i1 ] = value;
				break;
			case 28:
				a28[ i1 ] = value;
				break;
			case 29:
				a29[ i1 ] = value;
				break;
			case 30:
				a30[ i1 ] = value;
				break;
			case 31:
				a31[ i1 ] = value;
				break;
			default:
				a0[ i1 ] = value;
				break;
			}
		}
	}
	
}

