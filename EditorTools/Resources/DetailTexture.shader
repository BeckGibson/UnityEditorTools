
Shader "Optimised/DetailTexture"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DetailMap("DetailMap (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdbase
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

		#include "UnityCG.cginc"
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		UNITY_DECLARE_TEX2D(_MainTex);
		UNITY_DECLARE_TEX2D(_DetailMap);
		float4 _MainTex_ST;
		float4 _DetailMap_ST;
		float4 _Color;

		struct appdata
		{
			float4 vertex   : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 pack0 : TEXCOORD0;
	#ifndef LIGHTMAP_OFF
			float2 lmap : TEXCOORD1;
	#else
			float3 vlight : TEXCOORD1;
	#endif

			LIGHTING_COORDS(2, 3)
			UNITY_VERTEX_OUTPUT_STEREO
		};


		inline float3 LightingLambertVS(float3 normal, float3 lightDir)
		{
			float diff = max(0, dot(normal, lightDir));
			return _LightColor0.rgb * diff;
		}


		v2f vert(appdata v)
		{
			UNITY_SETUP_INSTANCE_ID(v);
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);

			o.pos = UnityObjectToClipPos(v.vertex);

			o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);


	#ifndef LIGHTMAP_OFF
			o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#else
			float3 worldN = UnityObjectToWorldNormal(v.normal);
			o.vlight = ShadeSH9(float4(worldN, 1.0));
			o.vlight += LightingLambertVS(worldN, _WorldSpaceLightPos0.xyz);
	#endif

			TRANSFER_VERTEX_TO_FRAGMENT(o);
			UNITY_TRANSFER_FOG(o, o.pos);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 surfaceColor;
			surfaceColor = UNITY_SAMPLE_TEX2D(_MainTex, i.pack0.xy);

			fixed4 detailColor;
			detailColor = UNITY_SAMPLE_TEX2D(_DetailMap, i.pack0.xy * 10);

			surfaceColor *= detailColor * _Color;

			fixed atten = LIGHT_ATTENUATION(i);
			fixed4 finalColor = 0;


	#ifdef LIGHTMAP_OFF
			finalColor.rgb = surfaceColor.rgb * i.vlight * atten;
	#else
			fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy));

			finalColor.rgb = surfaceColor.rgb * lm;

	#endif


			finalColor.a = surfaceColor.a;


			return finalColor;
		}
		ENDCG
	}
	}
}
