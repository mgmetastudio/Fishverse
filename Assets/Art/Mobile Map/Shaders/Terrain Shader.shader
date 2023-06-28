// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Terrain Amplify Shader"
{
	Properties
	{
		_Basetexture("Base texture", 2D) = "white" {}
		_TextureRotation("Texture Rotation", Float) = 0
		_Blendamount("Blend amount", Float) = 0
		_Hue("Hue", Float) = 0
		_Saturation("Saturation", Float) = 0
		_Value("Value", Float) = 0
		_RMask1("R Mask 1", 2D) = "white" {}
		_PowerR("Power R", Range( 0 , 2)) = 0.5294116
		_MultiplyR("Multiply R", Range( 0 , 100)) = 0
		_GMask2("G Mask 2", 2D) = "white" {}
		_PowerG("Power G", Range( 0 , 2)) = 1.070588
		_MultiplyG("Multiply G", Range( 0 , 100)) = 0
		_TilingBase("Tiling Base", Vector) = (1,1,0,0)
		_OffsetBase("Offset Base", Vector) = (0,0,0,0)
		_TilingTexture1("Tiling Texture 1", 2D) = "white" {}
		_Tiling_1("Tiling_1", Vector) = (0,0,0,0)
		_Normal1("Normal 1", 2D) = "bump" {}
		_NrmlStrength1("NrmlStrength 1", Float) = 0.2
		_TilingTexture2("Tiling Texture 2", 2D) = "white" {}
		_Tiling2("Tiling 2", Vector) = (0,0,0,0)
		_Normal2("Normal 2", 2D) = "bump" {}
		_NormalStrength2("Normal Strength 2", Float) = 0.2
		_TilingTexture3("Tiling Texture 3", 2D) = "white" {}
		_Tiling3("Tiling 3", Vector) = (0,0,0,0)
		_Nrml3("Nrml 3", 2D) = "bump" {}
		_NormalStrength3("Normal Strength 3", Float) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 5.0
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
			sampler2D _TerrainHeightmapTexture;//ASE Terrain Instancing
			sampler2D _TerrainNormalmapTexture;//ASE Terrain Instancing
		#endif//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
			UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
		CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
				float4 _TerrainHeightmapScale;//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
		CBUFFER_END//ASE Terrain Instancing
		uniform sampler2D _Normal1;
		uniform half2 _Tiling_1;
		uniform half _NrmlStrength1;
		uniform sampler2D _Normal2;
		uniform half2 _Tiling2;
		uniform half _NormalStrength2;
		uniform sampler2D _RMask1;
		uniform half2 _TilingBase;
		uniform half2 _OffsetBase;
		uniform half _TextureRotation;
		uniform half _PowerR;
		uniform half _MultiplyR;
		uniform sampler2D _Nrml3;
		uniform half2 _Tiling3;
		uniform half _NormalStrength3;
		uniform sampler2D _GMask2;
		uniform half _PowerG;
		uniform half _MultiplyG;
		uniform sampler2D _Basetexture;
		uniform sampler2D _TilingTexture1;
		uniform sampler2D _TilingTexture2;
		uniform sampler2D _TilingTexture3;
		uniform half _Blendamount;
		uniform half _Hue;
		uniform half _Saturation;
		uniform half _Value;


		void ApplyMeshModification( inout appdata_full v )
		{
			#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);
				
				float4 uvscale = instanceData.z * _TerrainHeightmapRecipSize;
				float4 uvoffset = instanceData.xyxy * uvscale;
				uvoffset.xy += 0.5f * _TerrainHeightmapRecipSize.xy;
				float2 sampleCoords = (patchVertex.xy * uvscale.xy + uvoffset.xy);
				
				float hm = UnpackHeightmap(tex2Dlod(_TerrainHeightmapTexture, float4(sampleCoords, 0, 0)));
				v.vertex.xz = (patchVertex.xy + instanceData.xy) * _TerrainHeightmapScale.xz * instanceData.z;
				v.vertex.y = hm * _TerrainHeightmapScale.y;
				v.vertex.w = 1.0f;
				
				v.texcoord.xy = (patchVertex.xy * uvscale.zw + uvoffset.zw);
				v.texcoord3 = v.texcoord2 = v.texcoord1 = v.texcoord;
				
				#ifdef TERRAIN_INSTANCED_PERPIXEL_NORMAL
					v.normal = float3(0, 1, 0);
					//data.tc.zw = sampleCoords;
				#else
					float3 nor = tex2Dlod(_TerrainNormalmapTexture, float4(sampleCoords, 0, 0)).xyz;
					v.normal = 2.0f * nor - 1.0f;
				#endif
			#endif
		}


		half3 HSVToRGB( half3 c )
		{
			half4 K = half4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			half3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		half3 RGBToHSV(half3 c)
		{
			half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			half4 p = lerp( half4( c.bg, K.wz ), half4( c.gb, K.xy ), step( c.b, c.g ) );
			half4 q = lerp( half4( p.xyw, c.r ), half4( c.r, p.yzx ), step( p.x, c.r ) );
			half d = q.x - min( q.w, q.y );
			half e = 1.0e-10;
			return half3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			ApplyMeshModification(v);;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord36 = i.uv_texcoord * _Tiling_1;
			float2 uv_TexCoord9 = i.uv_texcoord * _Tiling2;
			float2 uv_TexCoord24 = i.uv_texcoord * _TilingBase + _OffsetBase;
			float cos23 = cos( radians( _TextureRotation ) );
			float sin23 = sin( radians( _TextureRotation ) );
			half2 rotator23 = mul( uv_TexCoord24 - half2( 0.5,0.5 ) , float2x2( cos23 , -sin23 , sin23 , cos23 )) + half2( 0.5,0.5 );
			half temp_output_72_0 = saturate( ( pow( tex2D( _RMask1, rotator23 ).r , _PowerR ) * _MultiplyR ) );
			half3 lerpResult58 = lerp( UnpackScaleNormal( tex2D( _Normal1, uv_TexCoord36 ), _NrmlStrength1 ) , UnpackScaleNormal( tex2D( _Normal2, uv_TexCoord9 ), _NormalStrength2 ) , temp_output_72_0);
			float2 uv_TexCoord45 = i.uv_texcoord * _Tiling3;
			half temp_output_71_0 = saturate( ( pow( tex2D( _GMask2, rotator23 ).r , _PowerG ) * _MultiplyG ) );
			half3 lerpResult57 = lerp( lerpResult58 , UnpackScaleNormal( tex2D( _Nrml3, uv_TexCoord45 ), _NormalStrength3 ) , temp_output_71_0);
			o.Normal = lerpResult57;
			half4 lerpResult55 = lerp( tex2D( _TilingTexture1, uv_TexCoord36 ) , tex2D( _TilingTexture2, uv_TexCoord9 ) , temp_output_72_0);
			half4 lerpResult56 = lerp( lerpResult55 , tex2D( _TilingTexture3, uv_TexCoord45 ) , temp_output_71_0);
			half4 blendOpSrc16 = tex2D( _Basetexture, rotator23 );
			half4 blendOpDest16 = lerpResult56;
			half4 lerpBlendMode16 = lerp(blendOpDest16,(( blendOpDest16 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest16 ) * ( 1.0 - blendOpSrc16 ) ) : ( 2.0 * blendOpDest16 * blendOpSrc16 ) ),_Blendamount);
			half3 hsvTorgb86 = RGBToHSV( ( saturate( lerpBlendMode16 )).rgb );
			half3 hsvTorgb87 = HSVToRGB( half3(( hsvTorgb86.x * _Hue ),( hsvTorgb86.y * _Saturation ),( hsvTorgb86.z * _Value )) );
			o.Albedo = hsvTorgb87;
			o.Alpha = 1;
		}

		ENDCG
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
		UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
361.6;282.4;1261.6;675;-182.0115;944.1919;1;True;True
Node;AmplifyShaderEditor.Vector2Node;29;-2424.505,-1465.493;Inherit;False;Property;_OffsetBase;Offset Base;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;28;-2435.106,-1600.735;Inherit;False;Property;_TilingBase;Tiling Base;12;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;27;-2358.857,-1225.065;Inherit;False;Property;_TextureRotation;Texture Rotation;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;26;-2164.917,-1247.521;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-2272.788,-1538.589;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;25;-2234.497,-1410.346;Inherit;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RotatorNode;23;-2032.786,-1386.719;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;32;-1423.237,-1225.618;Inherit;True;Property;_RMask1;R Mask 1;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;68;-1400.822,-1031.372;Inherit;False;Property;_PowerR;Power R;7;0;Create;True;0;0;0;False;0;False;0.5294116;0.5294116;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1634.492,-571.721;Inherit;False;Property;_PowerG;Power G;10;0;Create;True;0;0;0;False;0;False;1.070588;1.070588;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1139.605,-802.883;Inherit;False;Property;_MultiplyR;Multiply R;8;0;Create;True;0;0;0;False;0;False;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;80;-1100.195,-1063.588;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;14;-1914.412,114.7882;Inherit;False;Property;_Tiling2;Tiling 2;19;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;35;-1918.265,-243.9433;Inherit;False;Property;_Tiling_1;Tiling_1;15;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;51;-1652.747,-775.1088;Inherit;True;Property;_GMask2;G Mask 2;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;83;-1524.93,-486.4244;Inherit;False;Property;_MultiplyG;Multiply G;11;0;Create;True;0;0;0;False;0;False;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-822.2054,-964.3829;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;44;-1918.071,540.1602;Inherit;False;Property;_Tiling3;Tiling 3;23;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PowerNode;79;-1348.363,-687.6631;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1653.204,130.309;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1657.057,-228.4224;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1656.863,557.2363;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1384.101,11.35328;Inherit;True;Property;_TilingTexture2;Tiling Texture 2;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;38;-1391.954,-390.5038;Inherit;True;Property;_TilingTexture1;Tiling Texture 1;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;72;-668.6483,-962.8823;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-1191.93,-615.4244;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-1029.471,-613.5338;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-516.2436,-720.7851;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;47;-1387.76,427.9601;Inherit;True;Property;_TilingTexture3;Tiling Texture 3;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1284.172,-1342.725;Inherit;False;Property;_Blendamount;Blend amount;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-1409.466,-1534.982;Inherit;True;Property;_Basetexture;Base texture;0;0;Create;True;0;0;0;False;0;False;-1;None;94ada6a0392a5874799bc35ace898002;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;56;-334.99,-537.5116;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;16;-87.142,-680.0554;Inherit;True;Overlay;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1662.82,311.2471;Inherit;False;Property;_NormalStrength2;Normal Strength 2;21;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1666.673,-47.48434;Inherit;False;Property;_NrmlStrength1;NrmlStrength 1;17;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1652.279,712.6124;Inherit;False;Property;_NormalStrength3;Normal Strength 3;25;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-1393.581,-208.5899;Inherit;True;Property;_Normal1;Normal 1;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;86;215.0351,-805.2308;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;94;261.1212,-665.1716;Inherit;False;Property;_Hue;Hue;3;0;Create;True;0;0;0;False;0;False;0;0.66;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;271.1212,-498.1716;Inherit;False;Property;_Value;Value;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-1398.639,195.88;Inherit;True;Property;_Normal2;Normal 2;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;90;262.0351,-573.2308;Inherit;False;Property;_Saturation;Saturation;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;484.1212,-821.1716;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;485.0351,-720.2308;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;491.1212,-624.1716;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-1402.298,621.252;Inherit;True;Property;_Nrml3;Nrml 3;24;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;58;-388.4534,-157.705;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.HSVToRGBNode;87;647.0351,-799.2308;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;57;-113.0144,-28.24641;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;899.1347,-636.3785;Half;False;True;-1;7;ASEMaterialInspector;0;0;Standard;Terrain Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;True;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;27;0
WireConnection;24;0;28;0
WireConnection;24;1;29;0
WireConnection;23;0;24;0
WireConnection;23;1;25;0
WireConnection;23;2;26;0
WireConnection;32;1;23;0
WireConnection;80;0;32;1
WireConnection;80;1;68;0
WireConnection;51;1;23;0
WireConnection;84;0;80;0
WireConnection;84;1;85;0
WireConnection;79;0;51;1
WireConnection;79;1;70;0
WireConnection;9;0;14;0
WireConnection;36;0;35;0
WireConnection;45;0;44;0
WireConnection;1;1;9;0
WireConnection;38;1;36;0
WireConnection;72;0;84;0
WireConnection;81;0;79;0
WireConnection;81;1;83;0
WireConnection;71;0;81;0
WireConnection;55;0;38;0
WireConnection;55;1;1;0
WireConnection;55;2;72;0
WireConnection;47;1;45;0
WireConnection;15;1;23;0
WireConnection;56;0;55;0
WireConnection;56;1;47;0
WireConnection;56;2;71;0
WireConnection;16;0;15;0
WireConnection;16;1;56;0
WireConnection;16;2;17;0
WireConnection;34;1;36;0
WireConnection;34;5;37;0
WireConnection;86;0;16;0
WireConnection;19;1;9;0
WireConnection;19;5;22;0
WireConnection;93;0;86;1
WireConnection;93;1;94;0
WireConnection;88;0;86;2
WireConnection;88;1;90;0
WireConnection;91;0;86;3
WireConnection;91;1;92;0
WireConnection;43;1;45;0
WireConnection;43;5;46;0
WireConnection;58;0;34;0
WireConnection;58;1;19;0
WireConnection;58;2;72;0
WireConnection;87;0;93;0
WireConnection;87;1;88;0
WireConnection;87;2;91;0
WireConnection;57;0;58;0
WireConnection;57;1;43;0
WireConnection;57;2;71;0
WireConnection;0;0;87;0
WireConnection;0;1;57;0
ASEEND*/
//CHKSM=656440E26C3E916BDCE38D4903B5E0FDBA194478