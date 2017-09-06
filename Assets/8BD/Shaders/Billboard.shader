Shader "LemonSpawn/Billboard" {
	Properties{
		_MainTex("Texture Image", 2D) = "white" {}
	_ScaleX("Scale X", Float) = 1.0
		_ScaleY("Scale Y", Float) = 1.0
	}
		SubShader{
		Pass{
		Blend srcalpha oneminussrcalpha
		//cull off
		Zwrite off
		Ztest off

		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag

		// User-specified uniforms            
		uniform sampler2D _MainTex;
	uniform float _ScaleX;
	uniform float _ScaleY;

	struct vertexInput {
		float4 vertex : POSITION;
		float4 tex : TEXCOORD0;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 tex : TEXCOORD0;
	};

/*	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;

		output.pos = mul(UNITY_MATRIX_P,
			mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
			+ float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
			* float4(_ScaleX, _ScaleY, 1.0, 1.0));

		output.tex = input.tex;

		return output;
	}
	*/
/*	vertexOutput vert(vertexInput input)
	{
		vertexOutput o;
		float4 ori = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1));
		float4 vt = input.vertex;
		vt.y = vt.z;
		vt.z = 0;
		vt.xyz += ori.xyz;//result is vt.z==ori.z ,so the distance to camera keeped ,and screen size keeped
		o.pos = mul(UNITY_MATRIX_P, vt);

		o.tex = input.tex;
		return o;
	}
	*/
	vertexOutput vert(vertexInput v)
	{
		vertexOutput o;
		float4 vv = v.vertex;
		vv.w /= 2;
		o.pos = UnityObjectToClipPos(vv);
		//            o.pos = mul(UNITY_MATRIX_P, 
		//            mul(UNITY_MATRIX_MV, float4(1.0, 0.0, 0.0, 2))
		//          - float4(v.vertex.x, v.vertex.y, 0.0, 0.0));

		o.tex = v.tex;


		return o;
	}


	float4 frag(vertexOutput input) : COLOR
	{
		half4 c = tex2D(_MainTex, float2(input.tex.xy));
		return c;
	}

		ENDCG
	}
	}
}