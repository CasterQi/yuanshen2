Shader "KriptoFX/RFX1/LightningParticles" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Main Texture", 2D) = "white" {}
	_DistortTex1("Distort Texture1", 2D) = "white" {}
	_DistortTex2("Distort Texture2", 2D) = "white" {}
	_DistortSpeed("Distort Speed Scale (xy/zw)", Vector) = (1,0.1,1,0.1) 
	_Offset("Offset", Float) = 0
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
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DistortTex1;
			sampler2D _DistortTex2;
			half4 _TintColor;
			half _Cutoff;
			float4 _DistortSpeed;
			half _Offset;
			
			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 uvMain : TEXCOORD0;
				float4 uvDistort : TEXCOORD1;
			};
			
			float4 _MainTex_ST;
			float4 _DistortTex1_ST;
			float4 _DistortTex2_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.color = v.color;
				float2 worlPos = mul(unity_ObjectToWorld, v.vertex);
				o.uvMain.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvDistort.xy = TRANSFORM_TEX(worlPos, _DistortTex1);
				o.uvDistort.zw = TRANSFORM_TEX(worlPos, _DistortTex2);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 distort1 = tex2D(_DistortTex1, i.uvDistort.xy + _DistortSpeed.x * i.color.r / 10) * 2 - 1;
				half4 distort2 = tex2D(_DistortTex1, i.uvDistort.xy - _DistortSpeed.x * i.color.r / 10 * 1.4 + float2(0.4, 0.6)) * 2 - 1;
				half4 distort3 = tex2D(_DistortTex2, i.uvDistort.zw + _DistortSpeed.z * i.color.r / 10) * 2 - 1;
				half4 distort4 = tex2D(_DistortTex2, i.uvDistort.zw - _DistortSpeed.z * i.color.r / 10 * 1.25 + float2(0.3, 0.7)) * 2 - 1;
				half offset = saturate(tex2D(_MainTex, i.uvMain).g + _Offset);
				
				half tex = tex2D(_MainTex, i.uvMain + (distort1.xy + distort2.xy) * _DistortSpeed.y * offset + (distort3.xy + distort4.xy) * _DistortSpeed.w * offset).r;
				half alpha = tex2D(_MainTex, i.uvMain * 7 + _Time.x * 5).b;
				
				clip(i.color.a - alpha);
				half4 col = 2.0f * _TintColor * tex *  i.color.a;
				
				return col;
			}
			ENDCG 
		}
	}	
}
}
