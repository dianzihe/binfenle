// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "MobilityGames/Unlit/Transparent/TexturedAlphaMask" {
	Properties {
		_Alpha ("Alpha", Range(0, 1)) = 1.0
//		_MaskBlendFactor("BlendFactor", Range(0, 1)) = 1.0
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_TextTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_MaskColor ("Mask Color", Color) = (1,1,1,1)
		_AlphaMask("AlphaMask (A)", 2D) = "white" {}
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
	
			fixed _Alpha;
//			fixed _MaskBlendFactor;
			sampler2D _MainTex;
			sampler2D _TextTex;
			fixed4 _MaskColor;
			sampler2D _AlphaMask;
	
			struct appdata { 
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
	
			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};
	
			float4 _MainTex_ST;
			float4 _AlphaMask_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    
			    return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mainCol  = tex2D(_MainTex, i.uv);
				fixed4 maskCol2 = tex2D(_TextTex, i.uv);
				fixed4 maskCol = tex2D(_AlphaMask, i.uv);
				

			  //mainCol.rgb = lerp(mainCol, maskCol * _MaskColor, maskCol.a * _MaskColor.a).rgb;
			  
				mainCol.rgb = lerp(mainCol, maskCol * maskCol2 * _MaskColor, maskCol.a * maskCol2.a * _MaskColor.a * 1.5f).rgb;
				mainCol.a *= _Alpha;

				return mainCol;
			}
			ENDCG
		}
	}
}
