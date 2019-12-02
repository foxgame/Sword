using System.Runtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[ StructLayout( LayoutKind.Sequential , Pack = 1 ) ]
public struct GameBitArray32BytesChat //: IDisposable
{
	GameBitArray32 a0;
	GameBitArray32 a1;
	GameBitArray32 a2;
	GameBitArray32 a3;

	public byte this[ int i ]
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
			}
		}
	}

}

