Shader "LemonSpawn/BubbleShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _bgcolor;
			float4 _fgcolor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				float2 dir = i.uv - float2(0.5, 0.5);
				float n = 6;
				float d = pow(pow(dir.x, n) + pow(dir.y, n), 1.0 / n);

				if (d > 0.5)
					discard;

				float colScale = 1;
				if (d < 0.45)
					colScale = 0.5;

				fixed4 col = _bgcolor;
				col.xyz *= colScale;
				// apply fog
				return col;
			}
			ENDCG
		}
	}
}
