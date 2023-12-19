// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FAE/Tree Branch"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_HueVariation("Hue Variation", Color) = (1,0.5,0,0.184)
		[NoScaleOffset]_BumpMap("BumpMap", 2D) = "bump" {}
		_TransmissionColor("Transmission Color", Color) = (1,1,1,0)
		_AmbientOcclusion("AmbientOcclusion", Range( 0 , 1)) = 0
		_MaxWindStrength("MaxWindStrength", Range( 0 , 1)) = 0.1164738
		_FlatLighting("FlatLighting", Range( 0 , 1)) = 0
		_WindAmplitudeMultiplier("WindAmplitudeMultiplier", Float) = 1
		_GradientBrightness("GradientBrightness", Range( 0 , 2)) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Toggle]_UseSpeedTreeWind("UseSpeedTreeWind", Float) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#include "VS_InstancedIndirect.cginc"
		#pragma instancing_options assumeuniformscaling lodfade maxcount:50 procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma exclude_renderers xbox360 psp2 n3ds wiiu 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float2 uv2_texcoord2;
			float4 vertexToFrag332;
		};

		uniform sampler2D _WindVectors;
		uniform float _WindAmplitudeMultiplier;
		uniform float _WindAmplitude;
		uniform float _WindSpeed;
		uniform float4 _WindDirection;
		uniform float _UseSpeedTreeWind;
		uniform float _MaxWindStrength;
		uniform float _WindStrength;
		uniform float _TrunkWindSpeed;
		uniform float _TrunkWindSwinging;
		uniform float _TrunkWindWeight;
		uniform float _FlatLighting;
		uniform sampler2D _BumpMap;
		uniform float _GradientBrightness;
		uniform sampler2D _MainTex;
		uniform float4 _HueVariation;
		uniform float _WindDebug;
		uniform float4 _TransmissionColor;
		uniform float _Smoothness;
		uniform float _AmbientOcclusion;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_60_0 = ( ( _WindSpeed * 0.05 ) * _Time.w );
			float2 appendResult249 = (float2(_WindDirection.x , _WindDirection.z));
			float3 WindVectors99 = UnpackNormal( tex2Dlod( _WindVectors, float4( ( ( _WindAmplitudeMultiplier * _WindAmplitude * ( (ase_worldPos).xz * 0.01 ) ) + ( temp_output_60_0 * appendResult249 ) ), 0, 0.0) ) );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 appendResult250 = (float3(_WindDirection.x , 0.0 , _WindDirection.z));
			float3 _Vector2 = float3(1,1,1);
			float3 break282 = ( ( (float3( 0,0,0 ) + (sin( ( ( temp_output_60_0 * ( _TrunkWindSpeed / ase_objectScale ) ) * appendResult250 ) ) - ( float3(-1,-1,-1) + _TrunkWindSwinging )) * (_Vector2 - float3( 0,0,0 )) / (_Vector2 - ( float3(-1,-1,-1) + _TrunkWindSwinging ))) * _TrunkWindWeight ) * lerp(v.color.a,( v.texcoord1.xy.y * 0.01 ),_UseSpeedTreeWind) );
			float3 appendResult283 = (float3(break282.x , 0.0 , break282.z));
			float3 Wind17 = ( ( ( WindVectors99 * lerp(v.color.g,v.texcoord2.xy.x,_UseSpeedTreeWind) ) * _MaxWindStrength * _WindStrength ) + appendResult283 );
			v.vertex.xyz += Wind17;
			float3 ase_vertexNormal = v.normal.xyz;
			float3 _Vector0 = float3(0,1,0);
			float3 lerpResult94 = lerp( ase_vertexNormal , _Vector0 , _FlatLighting);
			v.normal = lerpResult94;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult236 = normalize( ase_worldlightDir );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult36 = dot( normalizeResult236 , ( 1.0 - ase_worldViewDir ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.vertexToFrag332 = ( ( ( (0.0 + (dotResult36 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * v.color.b ) * _TransmissionColor.a ) * ( _TransmissionColor * ase_lightColor ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap62 = i.uv_texcoord;
			o.Normal = UnpackNormal( tex2D( _BumpMap, uv_BumpMap62 ) );
			float2 uv_MainTex19 = i.uv_texcoord;
			float4 tex2DNode19 = tex2D( _MainTex, uv_MainTex19 );
			float4 lerpResult246 = lerp( ( _GradientBrightness * tex2DNode19 ) , tex2DNode19 , lerp(saturate( ( i.vertexColor.a * 10.0 ) ),( 0.1 * i.uv2_texcoord2.y ),_UseSpeedTreeWind));
			float4 transform204 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float4 lerpResult20 = lerp( lerpResult246 , _HueVariation , ( _HueVariation.a * frac( ( ( transform204.x + transform204.y ) + transform204.z ) ) ));
			float4 Color56 = saturate( lerpResult20 );
			float3 ase_worldPos = i.worldPos;
			float temp_output_60_0 = ( ( _WindSpeed * 0.05 ) * _Time.w );
			float2 appendResult249 = (float2(_WindDirection.x , _WindDirection.z));
			float3 WindVectors99 = UnpackNormal( tex2D( _WindVectors, ( ( _WindAmplitudeMultiplier * _WindAmplitude * ( (ase_worldPos).xz * 0.01 ) ) + ( temp_output_60_0 * appendResult249 ) ) ) );
			float4 lerpResult97 = lerp( Color56 , float4( WindVectors99 , 0.0 ) , _WindDebug);
			o.Albedo = lerpResult97.rgb;
			float4 SSS45 = i.vertexToFrag332;
			o.Emission = SSS45.rgb;
			o.Smoothness = _Smoothness;
			float lerpResult53 = lerp( 1.0 , 0.0 , ( _AmbientOcclusion * ( 1.0 - i.vertexColor.r ) ));
			float AmbientOcclusion218 = lerpResult53;
			o.Occlusion = AmbientOcclusion218;
			o.Alpha = 1;
			float Alpha31 = tex2DNode19.a;
			float lerpResult101 = lerp( Alpha31 , 1.0 , _WindDebug);
			clip( lerpResult101 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Nature/SpeedTree"
	CustomEditor "FAE.TreeBranchShaderGUI"
}
/*ASEBEGIN
Version=15700
1927;29;1905;1004;237.746;723.76;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;100;-420.9563,-190.6743;Float;False;Global;_WindDebug;_WindDebug;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;272;-155.1518,-497.8887;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;274;-109.1185,81.11133;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;85;-3626.292,489.3988;Float;False;2725.568;616.9805;Subsurface;17;45;226;225;215;213;224;214;229;36;232;40;231;236;330;33;34;332;Transmission;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalVertexDataNode;96;272.633,208.2133;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;95;208.6328,558.2144;Float;False;Property;_FlatLighting;FlatLighting;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;307.7924,-795.8511;Float;False;56;Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;284.0998,-712.3729;Float;False;99;WindVectors;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;273;-109.7907,-588.0359;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;241;-3107.428,-410.5003;Float;False;2227.867;732.7858;Comment;21;334;245;333;56;20;30;246;31;203;247;23;83;19;335;24;204;248;336;337;338;339;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;238;-3972.506,-2089.813;Float;False;2833.298;786.479;Comment;24;5;106;59;4;210;90;86;60;209;211;89;212;91;102;99;10;237;15;16;249;284;315;318;319;Leaf wind animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-328.496,-44.40088;Float;False;31;Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2700.494,1188.223;Float;False;1461.06;358.5759;Comment;7;47;50;49;51;108;53;218;AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;93;-111.367,291.2144;Float;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;103;-307.0379,32.42241;Float;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;271;176.8815,94.11133;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;239;-3957.72,-1217.98;Float;False;2848.898;709.3215;Comment;22;283;282;118;143;152;206;144;170;150;242;154;148;171;250;141;194;87;142;168;320;321;322;Global wind animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2069.393,1351.298;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1331.207,-1774.334;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-2002.035,1238.223;Float;False;Constant;_Float5;Float 5;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;94;613.6323,307.2142;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-2384.838,567.6202;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2409.993,1253.799;Float;False;Property;_AmbientOcclusion;AmbientOcclusion;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-378.4811,-1378.025;Float;False;Wind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-869.729,-1375.275;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;49;-2329.393,1396.799;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;7.430328,-312.6622;Float;False;45;SSS;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;292;231.6353,414.0884;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1766.407,-1659.435;Float;False;Property;_MaxWindStrength;MaxWindStrength;6;0;Create;True;0;0;False;0;0.1164738;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;213;-2398.831,721.1215;Float;False;Property;_TransmissionColor;Transmission Color;4;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;237;-1818.446,-1787.913;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;62;-88.8763,-500.0306;Float;True;Property;_BumpMap;BumpMap;3;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-1767.439,561.4199;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;283;-1339.63,-1070.413;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;214;-2240.732,937.2216;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.Vector3Node;154;-3111.927,-969.0889;Float;False;Constant;_Vector1;Vector 1;10;0;Create;True;0;0;False;0;-1,-1,-1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;332;-1423.559,585.8492;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;331;-654.8947,-1397.448;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;245;-3077.694,-96.79821;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;101;260.1581,-43.74419;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;47;-2650.494,1344.798;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1154.397,560.2985;Float;False;SSS;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;334;-3027.829,64.5654;Float;False;Constant;_Float2;Float 2;14;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;254;-89.41714,-236.6956;Float;False;Property;_Smoothness;Smoothness;11;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-3067.625,-1119.186;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;315;-3291.345,-1955.613;Float;False;Global;_WindAmplitude;_WindAmplitude;12;0;Create;True;0;0;False;0;2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;336;-1392.693,-114.7108;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;97;626.5228,-673.5649;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;284;-1764.627,-1563.295;Float;False;Global;_WindStrength;_WindStrength;12;0;Create;True;0;0;False;0;1;0.46;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;249;-3195.306,-1457.416;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-3322.042,-2039.813;Float;False;Property;_WindAmplitudeMultiplier;WindAmplitudeMultiplier;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;291;-120.5628,460.3643;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1132.186,-119.2207;Float;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-3001.746,-1583.413;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-3470.674,-1917.879;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;-2015.23,749.321;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;624.2745,22.64254;Float;False;17;Wind;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;150;-2880.324,-1106.686;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TimeNode;4;-3622.412,-1505.334;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;34;-3586.492,685.3988;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;33;-3603.792,532.3994;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-3544.246,-1617.813;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-3907.72,-909.7822;Float;False;Global;_TrunkWindSpeed;_TrunkWindSpeed;10;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-2005.338,565.3199;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-3922.506,-1624.636;Float;False;Global;_WindSpeed;_WindSpeed;7;0;Create;True;0;0;False;0;0.3;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-3856.645,-1546.413;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;250;-3580.585,-1104.491;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ObjectScaleNode;168;-3848.127,-808.3901;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;5;-3919.962,-1905.495;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;86;-3669.578,-1918.893;Float;False;FLOAT2;0;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-3690.527,-1793.813;Float;False;Constant;_Float7;Float 7;10;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-3342.022,-1167.98;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;194;-3632.326,-890.6907;Float;False;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-3322.006,-1569.036;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;87;-3927.026,-1131.687;Float;False;Global;_WindDirection;_WindDirection;9;0;Create;True;0;0;False;0;1,0,0,0;-0.9450631,0,-0.326888,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;242;-2888.98,-732.9907;Float;False;Constant;_Vector2;Vector 2;10;0;Create;True;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-2049.014,-1089.885;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;318;-2340.084,-1458.298;Float;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-2168.551,-1782.414;Float;False;WindVectors;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;36;-3137.692,548.1992;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;102;-2560.851,-1799.613;Float;True;Global;_WindVectors;_WindVectors;8;0;Create;True;0;0;False;0;None;6c795dd1d1d319e479e68164001557e8;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-3047.296,-359.2004;Float;True;Property;_MainTex;MainTex;1;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-1708.497,-1084.519;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;171;-3197.828,-798.7911;Float;False;Global;_TrunkWindSwinging;_TrunkWindSwinging;10;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;330;-3326.007,693.1129;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;229;-2670.534,584.4202;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;319;-2061.475,-1614.09;Float;False;Property;_UseSpeedTreeWind;UseSpeedTreeWind;12;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;236;-3336.135,536.0207;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-2873.828,-908.7911;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-2765.946,-1767.013;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-2294.521,-923.9849;Float;False;Global;_TrunkWindWeight;_TrunkWindWeight;10;0;Create;True;0;0;False;0;2;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;10;-2285.806,-1641.335;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;246;-1920.755,-286.4068;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;40;-2871.99,852.5;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;231;-2937.638,656.3201;Float;False;Constant;_Float9;Float 9;11;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;-1636.979,-117.2845;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;152;-2439.225,-1114.487;Float;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;-1,-1,-1;False;2;FLOAT3;1,1,1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-2194.692,-355.7448;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-2696.774,-401.3467;Float;False;Property;_GradientBrightness;GradientBrightness;10;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;206;-2469.663,-818.2187;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;203;-2102.101,176.9948;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1892.772,90.23112;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;83;-2177.762,-79.10616;Float;False;Property;_HueVariation;Hue Variation;2;0;Create;True;0;0;False;0;1,0.5,0,0.184;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;-2999.042,-1900.813;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-2267.067,177.2769;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-2433.411,142.0711;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1868.222,-1088.986;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;232;-2936.735,762.0214;Float;False;Constant;_Float4;Float 4;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;322;-2114.224,-778.8527;Float;False;Property;_UseSpeedTreeWind;UseSpeedTreeWind;12;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-2672.731,-250.9719;Float;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;337;-2499.628,-164.041;Float;False;Property;_UseSpeedTreeWind;UseSpeedTreeWind;12;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;320;-2494.148,-640.404;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;338;-3029.034,148.7886;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-2822.082,-87.03672;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;335;-2668.076,-126.742;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;-2725.83,48.92334;Float;False;2;2;0;FLOAT;0.1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;217;-54.78961,-161.1802;Float;False;218;AmbientOcclusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;204;-2669.998,129.9109;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;53;-1805.494,1291.499;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-1547.435,1323.92;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;321;-2220.438,-622.0268;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;946.35,-313.3243;Float;False;True;2;Float;FAE.TreeBranchShaderGUI;0;0;Standard;FAE/Tree Branch;False;False;False;False;False;False;False;False;True;False;False;False;True;False;False;False;True;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;False;True;True;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;20.3;10;25;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Nature/SpeedTree;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;3;Include;VS_InstancedIndirect.cginc;False;;Pragma;instancing_options assumeuniformscaling lodfade maxcount:50 procedural:setup;False;;Pragma;multi_compile GPU_FRUSTUM_ON __;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;272;0;100;0
WireConnection;274;0;100;0
WireConnection;273;0;272;0
WireConnection;271;0;274;0
WireConnection;51;0;50;0
WireConnection;51;1;49;0
WireConnection;15;0;237;0
WireConnection;15;1;16;0
WireConnection;15;2;284;0
WireConnection;94;0;96;0
WireConnection;94;1;93;0
WireConnection;94;2;95;0
WireConnection;224;0;229;0
WireConnection;224;1;40;3
WireConnection;17;0;331;0
WireConnection;123;0;15;0
WireConnection;123;1;283;0
WireConnection;49;0;47;1
WireConnection;292;0;93;0
WireConnection;292;1;291;0
WireConnection;237;0;99;0
WireConnection;237;1;319;0
WireConnection;226;0;225;0
WireConnection;226;1;215;0
WireConnection;283;0;282;0
WireConnection;283;2;282;2
WireConnection;332;0;226;0
WireConnection;331;0;123;0
WireConnection;101;0;32;0
WireConnection;101;1;103;0
WireConnection;101;2;271;0
WireConnection;45;0;332;0
WireConnection;148;0;141;0
WireConnection;148;1;250;0
WireConnection;336;0;20;0
WireConnection;97;0;57;0
WireConnection;97;1;98;0
WireConnection;97;2;273;0
WireConnection;249;0;87;1
WireConnection;249;1;87;3
WireConnection;56;0;336;0
WireConnection;89;0;60;0
WireConnection;89;1;249;0
WireConnection;209;0;86;0
WireConnection;209;1;210;0
WireConnection;215;0;213;0
WireConnection;215;1;214;0
WireConnection;150;0;148;0
WireConnection;90;0;59;0
WireConnection;90;1;106;0
WireConnection;225;0;224;0
WireConnection;225;1;213;4
WireConnection;250;0;87;1
WireConnection;250;2;87;3
WireConnection;86;0;5;0
WireConnection;141;0;60;0
WireConnection;141;1;194;0
WireConnection;194;0;142;0
WireConnection;194;1;168;0
WireConnection;60;0;90;0
WireConnection;60;1;4;4
WireConnection;143;0;152;0
WireConnection;143;1;144;0
WireConnection;99;0;102;0
WireConnection;36;0;236;0
WireConnection;36;1;330;0
WireConnection;102;1;91;0
WireConnection;282;0;118;0
WireConnection;330;0;34;0
WireConnection;229;0;36;0
WireConnection;229;1;231;0
WireConnection;229;2;232;0
WireConnection;229;4;232;0
WireConnection;319;0;10;2
WireConnection;319;1;318;1
WireConnection;236;0;33;0
WireConnection;170;0;154;0
WireConnection;170;1;171;0
WireConnection;91;0;212;0
WireConnection;91;1;89;0
WireConnection;246;0;247;0
WireConnection;246;1;19;0
WireConnection;246;2;337;0
WireConnection;20;0;246;0
WireConnection;20;1;83;0
WireConnection;20;2;30;0
WireConnection;152;0;150;0
WireConnection;152;1;170;0
WireConnection;152;2;242;0
WireConnection;152;4;242;0
WireConnection;247;0;248;0
WireConnection;247;1;19;0
WireConnection;203;0;24;0
WireConnection;30;0;83;4
WireConnection;30;1;203;0
WireConnection;212;0;211;0
WireConnection;212;1;315;0
WireConnection;212;2;209;0
WireConnection;24;0;23;0
WireConnection;24;1;204;3
WireConnection;23;0;204;1
WireConnection;23;1;204;2
WireConnection;118;0;143;0
WireConnection;118;1;322;0
WireConnection;322;0;206;4
WireConnection;322;1;321;0
WireConnection;31;0;19;4
WireConnection;337;0;335;0
WireConnection;337;1;339;0
WireConnection;333;0;245;4
WireConnection;333;1;334;0
WireConnection;335;0;333;0
WireConnection;339;1;338;2
WireConnection;53;0;108;0
WireConnection;53;2;51;0
WireConnection;218;0;53;0
WireConnection;321;0;320;2
WireConnection;0;0;97;0
WireConnection;0;1;62;0
WireConnection;0;2;46;0
WireConnection;0;4;254;0
WireConnection;0;5;217;0
WireConnection;0;10;101;0
WireConnection;0;11;18;0
WireConnection;0;12;94;0
ASEEND*/
//CHKSM=53F99EC675A213843B71FCBF660EF68F8F4548A6