Shader "KriptoFX/RFX1/Decal/WorldMaskMobile" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff("Cutoff", Range(0, 1.1)) = 1.1
	_MainTex ("Main Texture", 2D) = "white" {}
	_Mask ("Mask", 2D) = "white" {}
	_MaskPow("Mask pow", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			half4 _TintColor;
			half _Cutoff;
			half _MaskPow;
			half _AlphaPow;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			
			float4 _MainTex_ST;
			float4 _Mask_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float2 worldUV = mul(unity_ObjectToWorld, v.vertex).xz / 5;
				o.uv.xy = TRANSFORM_TEX(worldUV, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _Mask);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 tex = tex2D(_MainTex,  i.uv.xy);
				half mask = tex2D(_Mask, i.uv.zw).a;
				mask = pow(mask, _MaskPow);
				
				half4 col = 2.0f * _TintColor * tex;

				UNITY_APPLY_FOG(i.fogCoord, col);
				
				half m = saturate(mask - _Cutoff);
				half alpha = saturate(tex.a * m * _TintColor.a * 2);
				col.a = alpha;
				col.rgb = col.rgb + saturate(pow(dot(col.rgb, 0.3), 10)/2)*2;
				return float4(col.rgb * pow(alpha, _AlphaPow), col.a);
			}
			ENDCG 
		}
	}	
}
}
