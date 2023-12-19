Shader "KriptoFX/RFX1/Decal/MaskCutoutMobile" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutout("Cutout", Range(0, 1.1)) = 1.1
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Mask ("Mask", 2D) = "white" {}
	_MaskAlpha ("Mask Alpha", 2D) = "white" {}
	_AlphaMultiplier("Alpha mul", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	Offset -1, -1

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			sampler2D _MaskAlpha;
			half4 _TintColor;
			float _Cutout;
			half _AlphaPow;
			half _AlphaMultiplier;

			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR0;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR0;
				float4 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};
			
			float4 _MainTex_ST;
			float4 _Mask_ST;
			float4 _MaskAlpha_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
			
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _Mask);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord, _MaskAlpha);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			half4 frag (v2f i) : SV_Target
			{
				
				float4 tex = tex2D(_MainTex, i.texcoord.xy);
				float mask = 1 - tex2D(_Mask, i.texcoord.zw).r;
				float maskAlpha = tex2D(_MaskAlpha, i.texcoord1).a;
				half4 col = 2.0f * i.color * _TintColor * tex;
				UNITY_APPLY_FOG(i.fogCoord, col);
				float m = saturate(_Cutout - mask);

				col.a = tex.a * saturate(m*m * 100) * _TintColor.a * maskAlpha;
				
				return half4(col.rgb,  saturate(pow(col.a * _AlphaMultiplier, _AlphaPow)));
			}
			ENDCG 
		}
	}	
}
}
