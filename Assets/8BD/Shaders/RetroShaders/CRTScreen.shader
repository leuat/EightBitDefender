Shader "Hidden/CRTScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _pixels;
			float _radialDistort;
			float _vignetteDistance;
			float _tintStrength;
			float4 _tint;
			float _pixelBlend;
			float _pixelsColor;



			float3 Tint(float3 c) {
//				return _tintStrength*float3(c.x*_tint.x, c.y*_tint.y, c.z*_tint.z) + ((1 - _tintStrength)*c);
//				return float3(c.x + (_tint.x - c.x)*_tintStrength, c.y + (_tint.y - c.y)*_tintStrength, c.z + (_tint.z - c.z)*_tintStrength);
				float i = (c.r - c.g + c.b)/1;
				float3 tintColor = _tint*i;
				float3 newC = _tintStrength*tintColor + ((1 - _tintStrength)*c);
				return newC;
				
			}

			float2 pixelize(float2 uv) {
				uv.x = ((int)(uv.x*_pixels)) / _pixels;
				uv.y = ((int)(uv.y*_pixels)) / _pixels;
				return uv;
			}
			float3 pixelsColor(float2 uv, float3 color) {
				int x = ((uv.x*_pixelsColor));
				int y = ((uv.y*_pixelsColor));
				float3 retCol = float3(0, 0, 0);

				float i = 0.4*(color.r + color.g + color.b);


				if (y % 2 == 0) {
					if (x % 2 == 0)
						retCol.r = i;
					if (x % 2 == 1)
						retCol.g = i;
				}
				else
				{
					
					if (x % 2 == 0)
						retCol = float3(0, 0, i);
					else retCol = color;

				}
					
				return retCol;
			}

			float2 radialDistort(float2 uv, float strength, out float vignette) {
				float2 dir = (float2(0.5, 0.5) - uv);
				float dist = clamp(length(dir)-0.2,0,1);
				float distKeep = dist;
				dir = normalize(dir);
				dist = pow(dist, 6);
				uv -= dir*dist*strength;

				vignette = clamp(clamp(1.0-distKeep*distKeep*_vignetteDistance*10,0,1),0,1);



				return uv;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = pixelize(i.uv);

				float vignette;
				uv = radialDistort(uv, _radialDistort*10, vignette);
				fixed4 col = tex2D(_MainTex, uv);
				col.xyz = _pixelBlend*col.xyz + (1-_pixelBlend)*pixelsColor(i.uv, col.xyz);
				col.xyz *= vignette;
			
				// just invert the colors
//				col = 1 - col;
				col.xyz = Tint(col.xyz);
				return col;
			}
			ENDCG
		}
	}
}
