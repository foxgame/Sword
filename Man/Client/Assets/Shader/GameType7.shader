Shader "Game/Type7"
{
	Properties
	{
		_MainTex("Tex (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Lighting Off

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv0 : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 color : COLOR;
			};

			sampler2D _MainTex;


			v2f vert( a2v v )
			{
				v2f o;
				o.vertex = UnityObjectToClipPos( v.vertex );
				o.uv0 = v.uv0;
				o.color = v.color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex , i.uv0);
				//col.a = ( col.r * 0.33f + col.g * 0.33f + col.b * 0.33f );
			
				col.a = col.rgb;
				col.a = 1 - col.rgb;

//				float a = col.rgb;

				if ( col.r == 0 && col.g == 0 && col.b == 0 )
				{
					col.a = 0;
				}

//				col.a *= 1;

				return col;
			}

			ENDCG
		}

	}

}
