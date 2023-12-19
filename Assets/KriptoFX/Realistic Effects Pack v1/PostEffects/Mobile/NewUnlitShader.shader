Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 projPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			sampler2D _CameraDepthTextureSelf;
				sampler2D _CameraDepthTexture;
			sampler2D _BloomTextureSelf;
			fixed4 frag (v2f i) : SV_Target
			{
			float z = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
		
			float z1 = tex2Dproj(_CameraDepthTextureSelf, UNITY_PROJ_COORD(i.projPos));
			//return z1;
			float d = 1-Linear01Depth (UNITY_SAMPLE_DEPTH(z));
			d = pow(d, 100);
			float3 col = tex2Dproj(_BloomTextureSelf, UNITY_PROJ_COORD(i.projPos)).rgb;
			//return float4(z1, z1,z1, 1);
			//return float4(z1*10, z*10, z*10, 1);
			return float4(pow(z1, 20), pow(z, 20), pow(z, 20), 1);
			//return float4(z1, col.r, col.g, 1);
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				//return col;
			}
			ENDCG
		}
	}
}
