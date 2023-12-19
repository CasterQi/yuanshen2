Shader "KriptoFX/RFX1/Decal/DistortMaskMul" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutoff("Cutoff", Range(0, 1.1)) = 1.1
	_MainTex ("Main Texture", 2D) = "white" {}
	_DistortTex("Distort Texture", 2D) = "white" {}
	_Mask ("Mask", 2D) = "white" {}
	_Speed("Distort Speed", Float) = 1
	_Scale("Distort Scale", Float) = 1
	_MaskPow("Mask pow", Float) = 1
	_AlphaPow("Alpha pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend DstColor Zero
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
			sampler2D _DistortTex;
			half4 _TintColor;
			half _Cutoff;
			half _Speed;
			half _Scale;
			half _MaskPow;
			half _AlphaPow;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			half4 _Tex_NextFrame;
			half InterpolationValue;

			
			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 uvShadow : TEXCOORD1;
				float4 uvMainTex : TEXCOORD2;
				float4 uvMask : TEXCOORD3;
				float3 worldPos : TEXCOORD5;
				float3 normal : NORMAL;
				UNITY_FOG_COORDS(4)

			};
			
			float4 _MainTex_ST;
			float4 _DistortTex_ST;
			float4 _Mask_ST;

			half4 tex2DTriplanar(sampler2D tex, half2 offset, float3 worldPos, float3 normal)
			{
				half2 yUV = worldPos.xz * _DistortTex_ST.xy;
				half2 xUV = worldPos.zy * _DistortTex_ST.xy;
				half2 zUV = worldPos.xy * _DistortTex_ST.xy;

				half4 yDiff = tex2D(tex, yUV + offset);
				half4 xDiff = tex2D(tex, xUV + offset);
				half4 zDiff = tex2D(tex, zUV + offset);

				half3 blendWeights = pow(abs(normal), 1);

				blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

				return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
			}


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				o.color = v.color;
				o.uvMainTex = mul(unity_Projector, v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				o.texcoord.xy = TRANSFORM_TEX(o.uvMainTex.xyz, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(o.uvMainTex.xyz, _DistortTex);
				o.uvMask.xy = TRANSFORM_TEX(o.uvMainTex.xyz, _Mask);
				o.uvMask.zw = o.uvMainTex.xy * _Tex_NextFrame.xy + _Tex_NextFrame.zw;

				o.uvShadow = mul(unity_Projector, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				
				//half4 distort = tex2D(_DistortTex, i.texcoord.zw)*2-1;
				half4 distort = tex2DTriplanar(_DistortTex, 0, i.worldPos/10, i.normal) * 2 - 1;
				half4 tex = tex2D(_MainTex, i.texcoord.xy + distort.xy / 10 * _Scale + _Speed * _Time.xx);
				half4 tex2 = tex2D(_MainTex, i.texcoord.xy - distort.xy / 7 * _Scale - _Speed * _Time.xx * 1.4 + float2(0.4, 0.6));

				tex *= tex2;
				half mask = tex2D(_Mask, i.uvMask.xy).a;
				half maskNext = tex2D(_Mask, i.uvMask.zw).a;
				mask = lerp(mask, maskNext, InterpolationValue);
				mask = pow(mask, _MaskPow);
				
				half4 col = 2.0f * i.color * _TintColor * tex;

				UNITY_APPLY_FOG(i.fogCoord, col);
				
				half m = saturate(mask - _Cutoff);
				half alpha = saturate(tex.a * m * _TintColor.a * 2);

				half clampMutliplier = 1 - step(i.uvMainTex.x, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.x);
				clampMutliplier *= 1 - step(i.uvMainTex.y, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.y);
				float projectedCordZ = i.uvShadow.z;
				clampMutliplier *= step(projectedCordZ, 1);
				clampMutliplier *= step(-1, projectedCordZ);
				col.a = alpha * clampMutliplier;
				col.rgb = col.rgb + saturate(pow(dot(col.rgb, 0.3), 10)/2)*2;
				//clip(col.a - 0.02);
				return float4(lerp(col.rgb * pow(alpha, _AlphaPow), 1, (1-col.aaa)), 1);
			}
			ENDCG 
		}
	}	
}
}
