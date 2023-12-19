Shader "KriptoFX/RFX1/Decal/WorldMaskIceMobile" {
Properties {
	[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Main Texture", 2D) = "white" {}
	_BumpMap("Normalmap", 2D) = "bump" {}
	_Mask ("Mask", 2D) = "white" {}
	_MaskPow("Mask pow", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
	_BumpAmtTex("Distortion tex", Float) = 10
	[Toggle(USE_TRIPLANAR)] _UseTriplanar("Use Triplanar Mapping", Int) = 0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_TRIPLANAR
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			half4 _Color;
			half4 _TintColor;
			half _MaskPow;
			half _AlphaPow;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			sampler2D _GrabTexture2;
			float4 _GrabTexture2_TexelSize;
			float _BumpAmt;
			float _BumpAmtTex;
			sampler2D _BumpMap;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
#ifdef USE_TRIPLANAR
				float3 worldPos : TEXCOORD0;
				float3 normal : NORMAL;
#else
				float4 uv : TEXCOORD0;
#endif
				float2 uv2 : TEXCOORD1;
				UNITY_FOG_COORDS(2)

			};
			
			float4 _MainTex_ST;
			float4 _Mask_ST;
			float4 _BumpMap_ST;

			half4 tex2DTriplanar(sampler2D tex, float3 worldPos, float3 normal)
			{
				half2 yUV = worldPos.xz * _MainTex_ST.xy;
				half2 xUV = worldPos.zy * _MainTex_ST.xy;
				half2 zUV = worldPos.xy * _MainTex_ST.xy;

				half4 yDiff = tex2D(tex, yUV);
				half4 xDiff = tex2D(tex, xUV);
				half4 zDiff = tex2D(tex, zUV);

				half3 blendWeights = pow(abs(normal), 1);

				blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

				return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
			}
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef USE_TRIPLANAR
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
#else
				float2 worldUV = mul(unity_ObjectToWorld, v.vertex).xz / 5;
				o.uv.xy = TRANSFORM_TEX(worldUV, _MainTex);
				o.uv.zw = TRANSFORM_TEX(worldUV, _BumpMap);
#endif
				o.uv2 = TRANSFORM_TEX(v.texcoord, _Mask);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
#ifdef USE_TRIPLANAR
				half2 bump = UnpackNormal(tex2DTriplanar(_BumpMap, i.worldPos/5, i.normal)).rg;
				half4 tex = tex2DTriplanar(_MainTex, i.worldPos/5 + bump.xyy * _BumpAmtTex / 100,  i.normal);
#else
				half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw)).rg;
				half4 tex = tex2D(_MainTex, i.uv.xy + bump * _BumpAmtTex / 100);
#endif

				half mask = 1-tex2D(_Mask, i.uv2).a;
				half maskTex =  tex.a * pow(1-mask, _AlphaPow);
				mask = pow(mask, -_MaskPow);
				
				half4 texCol = _TintColor * tex;
				half m = saturate(mask * maskTex);
				UNITY_APPLY_FOG(i.fogCoord, texCol);
				
				return half4(texCol.rgb * tex.a * _TintColor.a, m);
			}
			ENDCG 
		}
	}	
}
}
