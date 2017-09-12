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
			float _pixelsColorX, _pixelsColorY;
			float _gamma;
			float _correction;


			float3 Tint(float3 c) {
//				return _tintStrength*float3(c.x*_tint.x, c.y*_tint.y, c.z*_tint.z) + ((1 - _tintStrength)*c);
//				return float3(c.x + (_tint.x - c.x)*_tintStrength, c.y + (_tint.y - c.y)*_tintStrength, c.z + (_tint.z - c.z)*_tintStrength);
				float i = (c.r - c.g + c.b)/1;
				float3 tintColor = _tint*i;
				float3 newC = _tintStrength*tintColor + ((1 - _tintStrength)*c);
				return newC;
				
			}

			float3 Gamma(float3 c) {
				c.x = pow(c.x + _correction, _gamma);
				c.y = pow(c.y + _correction, _gamma);
				c.z = pow(c.z + _correction, _gamma);
				return c;
			}


			float2 pixelize(float2 uv) {
				uv.x = ((int)(uv.x*_pixels)) / _pixels;
				uv.y = ((int)(uv.y*_pixels)) / _pixels;
				return uv;
			}
			float3 pixelsColor(float2 uv, float3 color) 
			{
				float ss = 1;// 0.25;// 0.25;// 0.25;
				int x = ((int)(uv.x*_pixelsColorX*ss));
				int y = ((int)(uv.y*_pixelsColorY*ss));
				float3 retCol =  float3(0, 0, 0);

				float s = 2;

				if (y % 2 == 0) {
					if (x % 2 == 0)
						retCol.r = color.r*s;
					if (x % 2 == 1)
						retCol.g = color.g*s;
				}
				else
				{
					
					if (x % 2 == 0)
						retCol = float3(0, 0, color.b*s);
					else retCol = color;

				}
				retCol *= 1;

				return retCol;
			}

			float2 radialDistort(float2 uv, float strength, out float vignette) {
				float2 dir = (float2(0.5, 0.5) - uv);
				float dist = clamp(length(dir)*1.0-0.35,0,1.0);
				float distKeep = dist;
				dir = normalize(dir);
				dist = pow(dist, 4);
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
				col.xyz = _pixelBlend*col.xyz + (1-_pixelBlend)*pixelsColor(i.uv, col.xyz)*1;
				col.xyz *= vignette;
			
				// just invert the colors
//				col = 1 - col;
				col.xyz = Gamma(Tint(col.xyz));
				return col;
			}
			ENDCG
		}
	}
}
