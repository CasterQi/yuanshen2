Shader "Hidden/KriptoFX/PostEffects/Depth" {
   
    SubShader {
       	Tags {"RenderType"="Opaque" }
		
        Pass {
		Cull Off
		
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			float4 test;
            struct v2f {
                float4 pos : SV_POSITION;
				float2 depth : TEXCOORD1;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.depth.xy = o.pos.zw;
                return o;
            }
			//_ProjectionParams.x == 1  android
			//_ProjectionParams.x == -1 PC
            float4 frag(v2f i) : SV_Target {
			#if defined(UNITY_REVERSED_Z)
				float depth = (-i.depth.x*_ProjectionParams.z) / (-i.depth.y * _ProjectionParams.z) ;
			#else
				float depth = (-i.depth.x*_ProjectionParams.z - _ProjectionParams.y*_ProjectionParams.z) / (-i.depth.y * _ProjectionParams.z) ;
			#endif
			
                return (depth);
            }
            ENDCG
        }
    }
}
