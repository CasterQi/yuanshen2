Shader "KriptoFX/RFX1/BlendCutout" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_CutoutMap("Cuout Tex (r)", 2D) = "black" {}
		_Cutout("Cutout", Range(0, 1)) = 1
		_InvFade("Soft Particles Factor", Float) = 3

	}

		Category{
				Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
			//ColorMask RGB
			Cull Off Lighting Off ZWrite Off

			SubShader{
			Pass{

			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 2.0
	#pragma multi_compile_particles
	#pragma multi_compile_fog
	float4x4 _InverseTransformMatrix;

	#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _CutoutMap;
		half4 _TintColor;
		half _Cutout;

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
	#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
	#endif
			UNITY_VERTEX_OUTPUT_STEREO
		};

		float4 _MainTex_ST;
		float4 _CutoutMap_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			o.vertex = UnityObjectToClipPos(v.vertex);
	#ifdef SOFTPARTICLES_ON
			o.projPos = ComputeScreenPos(o.vertex);
			COMPUTE_EYEDEPTH(o.projPos.z);
	#endif
			o.color = v.color * _TintColor;
			o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
			float2 pos = mul(_InverseTransformMatrix, float4(v.vertex.xyz, 1)).xz;
			o.texcoord.zw = (pos - 0.5) * _CutoutMap_ST.xy + _CutoutMap_ST.zw;
			UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}

		sampler2D_float _CameraDepthTexture;
		float _InvFade;

		half4 frag(v2f i) : SV_Target
		{
	#ifdef SOFTPARTICLES_ON
			float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
			float partZ = i.projPos.z;
			float fade = saturate(_InvFade * (sceneZ - partZ));
			i.color.a *= fade;
	#endif

			half4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord.xy);
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
