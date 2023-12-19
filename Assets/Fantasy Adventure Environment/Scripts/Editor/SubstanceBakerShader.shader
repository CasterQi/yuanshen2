// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FAE/Substance Baker"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "white" {}
		_ParallaxMap("_ParallaxMap", 2D) = "white" {}
		_Tessellation("Tessellation", Range( 0.1 , 24)) = 4
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _ParallaxMap;
		uniform float4 _ParallaxMap_ST;
		uniform float _Tessellation;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			float4 temp_cast_2 = (_Tessellation).xxxx;
			return temp_cast_2;
		}

		void vertexDataFunc( inout appdata v )
		{
			float4 uv_ParallaxMap = float4(v.texcoord * _ParallaxMap_ST.xy + _ParallaxMap_ST.zw, 0 ,0);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( tex2Dlod( _ParallaxMap, uv_ParallaxMap ) * float4( ase_vertexNormal , 0.0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode1.rgb;
			o.Smoothness = tex2DNode1.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "FAE.SubstanceBakerGUI"
}
/*ASEBEGIN
Version=13203
221;92;1307;655;-1165.503;261.9839;1;True;True
Node;AmplifyShaderEditor.NormalVertexDataNode;6;1522.188,450.8279;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;1434.188,242.8279;Float;True;Property;_ParallaxMap;_ParallaxMap;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;1850.188,327.8279;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;1453.188,9.828138;Float;True;Property;_BumpMap;BumpMap;1;0;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;1457.188,-216.1719;Float;True;Property;_MainTex;MainTex;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;1441.014,628.4747;Float;False;Property;_Tessellation;Tessellation;4;0;4;0.1;24;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2139.29,-72.82691;Float;False;True;6;Float;FAE.SubstanceBakerGUI;0;0;Standard;FAE/Substance Baker;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;3;32;20;20;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;4;1;6;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;4;1;4
WireConnection;0;11;4;0
WireConnection;0;14;7;0
ASEEND*/
//CHKSM=EEEEF8038BC6D1FF762F337B1BBD6590E3FE2E5F