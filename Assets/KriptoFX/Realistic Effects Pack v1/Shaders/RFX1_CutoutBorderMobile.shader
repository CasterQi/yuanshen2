Shader "KriptoFX/RFX1/CutoutBorderMobile" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB) Cutout (A)", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionTex("Emission (A)", 2D) = "black" {}
		_BumpTex ("Normal (RGB)", 2D) = "gray" {}
		_Cutoff ("_Cutoff", Range(-0.1,1)) = 0
		//_Cutout2 ("Cutout2", Range(0,1)) = 0
		[HDR]_BorderColor ("Border Color", Color) = (1,1,1,1)
		_CutoutThickness ("Cutout Thickness", Range(0,1)) = 0.03
	}
	SubShader {
		Tags { "RenderType"="Tranparent"  "IgnoreProjector" = "True" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionTex;
		sampler2D _BumpTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpTex; 
			float2 uv_EmissionTex;
			float4 color: COLOR;
		};

		half _Glossiness;
		half _Metallic;
		half4 _Color;
		half4 _BorderColor;
		half4 _EmissionColor;
		half _CutoutThickness;
		half _Cutoff;

		void surf (Input IN, inout SurfaceOutput o) {

			// Albedo comes from a texture tinted by color
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			// Metallic and smoothness come from slider variables
			
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));
			

			o.Alpha = c.a;
			_Cutoff += (1-IN.color.a);
			clip(c.a - _Cutoff);
			//if(c.a < _Cutout) discard;
			if(c.a < _Cutoff + _CutoutThickness) o.Emission = _BorderColor;
			else o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor *  IN.color.rgb;
		}
		ENDCG
	}
	 Fallback "Transparent/Cutout/Diffuse"
}
