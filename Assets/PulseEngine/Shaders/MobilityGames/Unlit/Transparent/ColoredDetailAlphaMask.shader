// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "MobilityGames/Unlit/Transparent/ColoredDetailAlphaMask" {
	Properties {
		_Alpha ("Alpha", Range(0, 1)) = 1.0
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_MaskColor ("Mask Color", Color) = (1,1,1,1)
		_ColoredMask("ColoredMask (RGBA)", 2D) = "white" {}
		_DetailTex("DetailTex (RGB)", 2D) = "white" {}
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
			sampler2D _MainTex;
			fixed4 _MaskColor;
			sampler2D _ColoredMask;
			sampler2D _DetailTex;
	
			struct appdata { 
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1 : TEXCOORD1;
			};
	
			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv  : TEXCOORD0;
			    float2 uv2 : TEXCOORD1;
			};
	
			float4 _MainTex_ST;
			float4 _ColoredMask_ST;
			float4 _DetailTex_ST;

			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    o.uv2 = TRANSFORM_TEX(v.texcoord1, _DetailTex);
			    
			    return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mainCol  = tex2D(_MainTex, i.uv);
				fixed4 maskCol = tex2D(_ColoredMask, i.uv);
				fixed4 detailCol = tex2D(_DetailTex, i.uv2);

				mainCol.rgb = lerp(mainCol, detailCol, maskCol.a).rgb;
				mainCol.rgb = lerp(mainCol, maskCol * _MaskColor, maskCol.a * _MaskColor.a).rgb;
				
				mainCol.a *= _Alpha;
				return mainCol;
			}
			ENDCG
		}
	}
}
