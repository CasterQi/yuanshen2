Shader "KriptoFX/RFX1/Decal/WorldMaskIce" {
Properties {
	[HDR]_Color ("Color Color", Color) = (0.5,0.5,0.5,0.5)
	[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff("Cutoff", Range(0, 1.1)) = 1.1
	_MainTex ("Main Texture", 2D) = "white" {}
	_BumpMap("Normalmap", 2D) = "bump" {}
	_Mask ("Mask", 2D) = "white" {}
	_MaskPow("Mask pow", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
	_BumpAmtTex("Distortion tex", Float) = 10
	_BumpAmt("Distortion", Float) = 100
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	Offset -1, -1

	SubShader {
		GrabPass{ "_GrabTexture2" }
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			half4 _Color;
			half4 _TintColor;
			half _Cutoff;
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
				float3 uvMainTex : TEXCOORD0;
				float2 uvMask : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 normal : NORMAL;
				float4 uvgrab : TEXCOORD3;
				UNITY_FOG_COORDS(4)

			};
			
			float4 _MainTex_ST;
			float4 _Mask_ST;
			float4 _BumpMap_ST;

			half4 tex2DTriplanar(sampler2D tex, float3 worldPos, float3 normal, float2 scale, float2 offset)
			{
				half2 yUV = worldPos.xz * scale + offset;
				half2 xUV = worldPos.zy * scale + offset;
				half2 zUV = worldPos.xy * scale + offset;

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
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				o.uvMainTex = mul(unity_Projector, v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				o.uvMask = TRANSFORM_TEX(o.uvMainTex.xyz, _Mask);

#if UNITY_UV_STARTS_AT_TOP
				half scale = -1.0;
#else
				half scale = 1.0;
#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.w;
#if UNITY_SINGLE_PASS_STEREO
				o.uvgrab.xy = TransformStereoScreenSpaceTex(o.uvgrab.xy, o.uvgrab.w);
#endif
				o.uvgrab.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));


				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half3 bump = UnpackNormal(tex2DTriplanar(_BumpMap, i.worldPos / 10, i.normal, _BumpMap_ST.xy, 0));
				half2 offset = bump.rg * _BumpAmt * _GrabTexture2_TexelSize.xy;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
				half4 grabCol = saturate(tex2Dproj(_GrabTexture2, UNITY_PROJ_COORD(i.uvgrab)));
				
				half4 tex = tex2DTriplanar(_MainTex, i.worldPos / 10, i.normal, _MainTex_ST.xy, offset/_BumpAmtTex);
				half mask = 1-tex2D(_Mask, i.uvMask).a;
				half maskTex =  tex.a * pow(1-mask, _AlphaPow);
				mask = pow(mask, -_MaskPow);
				
				half4 col = lerp(grabCol, _Color * grabCol, 1);
				half4 texCol = _TintColor * tex * dot(grabCol, 0.3);
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				half m = saturate(mask * maskTex);
				//return m;
				//return m/2;
				half alpha = saturate(tex.a * m * _TintColor.a * 2);

				half clampMutliplier = 1 - step(i.uvMainTex.x, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.x);
				clampMutliplier *= 1 - step(i.uvMainTex.y, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.y);
				float projectedCordZ = i.uvMainTex.z;
				clampMutliplier *= step(projectedCordZ, 1);
				clampMutliplier *= step(-1, projectedCordZ);


				col.a = m * clampMutliplier;
				//col.rgb = col.rgb + saturate(pow(dot(col.rgb, 0.3), 10)/2)*2;
				//clip(col.a - 0.02);
				//return float4(lerp(col.rgb * pow(alpha, _AlphaPow), 1, (1-col.a)), 1);

				return float4(col.rgb + texCol * tex.a * _Color.a, col.a);
			}
			ENDCG 
		}
	}	
}
}
