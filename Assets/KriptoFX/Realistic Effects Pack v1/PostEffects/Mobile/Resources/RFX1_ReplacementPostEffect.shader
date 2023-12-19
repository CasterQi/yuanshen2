// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/KriptoFX/PostEffects/RFX1_ReplacementPostEffect" {
	SubShader{
		
		Pass{
		Tags{ "PostEffect" = "Bloom" }
		Fog{ Mode Off }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f {
		float4 pos : POSITION;
#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
		float2 depth : TEXCOORD0;
#endif
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		UNITY_TRANSFER_DEPTH(o.depth);
		return o;
	}

	half4 frag(v2f i) : COLOR{
		return 1;
		//UNITY_OUTPUT_DEPTH(i.depth);
	}

		ENDCG

	}
	}

		Fallback Off

}
