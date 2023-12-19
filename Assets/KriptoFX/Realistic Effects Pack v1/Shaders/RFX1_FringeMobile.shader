Shader "KriptoFX/RFX1/FringeMobile" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Speed("Distort Speed", Float) = 1
	_Scale("Distort Scale", Float) = 1
	_CutoutMap("Cuout Tex (r)", 2D) = "black" {}
	_Cutout("Cutout", Range(0, 1)) = 1

}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Blend SrcAlpha One
	//ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CutoutMap;
			half4 _TintColor;
			half _Cutout;
			half _Speed;
			half _Scale;
			float4x4 _InverseTransformMatrix;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 projPos : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			float4 _MainTex_ST;
			float4 _CutoutMap_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				o.color = v.color * _TintColor;
				
				float2 pos = mul(_InverseTransformMatrix, float4(v.vertex.xyz, 1)).xz;
				o.color.a =  saturate((-1.15 - pos.y + _Cutout)*10);
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = (pos.xy - 0.5) * _CutoutMap_ST.xy + _CutoutMap_ST.zw;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
	
			
			half4 frag (v2f i) : SV_Target
			{
				fixed2 tex1 = tex2D(_MainTex, i.texcoord + _Time.x * _Speed * half2(0, -0.5)).xy;
				fixed tex2 = tex2D(_MainTex, i.texcoord + tex1.xy * _Scale + _Time.x * _Speed * half2(-2, 3)).b;
				half4 col = i.color * tex2 * tex2 * 2;
				half cutoutAlpha = tex2D(_CutoutMap, i.texcoord.zw).r;
				half alpha = (pow(1 - cutoutAlpha + _Cutout, 50));
				col.a *= saturate(alpha * pow(_Cutout, .2));
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG 
		}
	}	
}
}
