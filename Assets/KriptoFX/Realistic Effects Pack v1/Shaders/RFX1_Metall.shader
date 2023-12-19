Shader "KriptoFX/RFX1/Metall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[HDR]_TintColor ("Tint Color", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal (RG)", 2D) = "bump" {}
		_Scale("Bump Scale", Float) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Cutoff("Cutout", Range(0,1)) = 0.5
		_ReflTex ("Cubemap", CUBE) = "" {}
	}
	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

	

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR0;
			//float2 uv_BumpTex;
			//float3 worldRefl;
			//  INTERNAL_DATA
		};
		
		sampler2D _MainTex;
		sampler2D _BumpMap;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half4 _TintColor;
		//float _Cutout;
		half _Scale;
		 samplerCUBE _ReflTex;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _Scale);
			
			o.Albedo = saturate(c * dot(texCUBE (_ReflTex, o.Normal.xyy).rgb, 0.33) * 1.5);
			o.Emission = _TintColor * c * IN.color;
			//clip(c.a - _Cutout);
			// o.Emission = texCUBE (_Cube, IN.worldRefl).rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	 Fallback "Transparent/Cutout/Diffuse"
}
