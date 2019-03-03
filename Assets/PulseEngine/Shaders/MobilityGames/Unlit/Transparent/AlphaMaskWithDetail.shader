// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/Transparent/AlphaMaskWithDetail" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_AlphaMask("AlphaMask (A)", 2D) = "white" {}
		_DetailTex ("Detail (RGBA)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
	
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
	
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaMask;
			sampler2D _DetailTex;
			
			struct appdata { 
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1: TEXCOORD1;
			};

			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    float2 uv1 : TEXCOORD1;
			};
	
			float4 _MainTex_ST;
			float4 _AlphaMask_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    o.uv1 = TRANSFORM_TEX (v.texcoord1, _AlphaMask);
			    
			    return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mainCol  = tex2D(_MainTex, i.uv);
				fixed4 maskCol = tex2D(_AlphaMask, i.uv1);
				fixed4 detailCol = tex2D(_DetailTex, i.uv);
				
				mainCol = maskCol.a * mainCol + (1 - maskCol.a) * detailCol;

				return mainCol;
			}
			ENDCG
		}
	}
}
