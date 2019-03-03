// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/GUI/TextGlow" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		_GlowStrength ("Glow Strength", Range(0, 3)) = 1
	}

	SubShader {

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			uniform float _GlowStrength;

			
			float verticalBlur (v2f i)
			{
				float blurSize = 1.0/512.0;
				float blurSize2 = 2.0/512.0;
					
		   		float a = tex2D(_MainTex, float2(i.texcoord.x - blurSize2, i.texcoord.y)).a * 0.05;
		   		a += tex2D(_MainTex, float2(i.texcoord.x - blurSize, i.texcoord.y)).a * 0.30;
		   		a += tex2D(_MainTex, float2(i.texcoord.x, i.texcoord.y)).a * 0.20;
		   		a += tex2D(_MainTex, float2(i.texcoord.x + blurSize, i.texcoord.y)).a * 0.30;
		   		a += tex2D(_MainTex, float2(i.texcoord.x + blurSize2, i.texcoord.y)).a * 0.05;
			
				return a * _GlowStrength;				
			}
			
			float horizontalBlur(v2f i) 
			{
				float blurSize = 1.0/512.0;
				float blurSize2 = 2.0/512.0;

		   		float a = tex2D(_MainTex, float2(i.texcoord.x, i.texcoord.y - blurSize2)).a * 0.05;
		   		a += tex2D(_MainTex, float2(i.texcoord.x, i.texcoord.y - blurSize)).a * 0.30;
		   		a += tex2D(_MainTex, float2(i.texcoord.x, i.texcoord.y + blurSize)).a * 0.30;
		   		a += tex2D(_MainTex, float2(i.texcoord.x, i.texcoord.y + blurSize2)).a * 0.05;
				return a * _GlowStrength;		
			}
						
			v2f vert (appdata_t v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				
				fixed4 col = _Color;
				
				col.a  = verticalBlur(i); 
				col.a += verticalBlur(i);				

		   		
				return col;
			}
			ENDCG 
		}
	} 	

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Color [_Color]
			SetTexture [_MainTex] {
				combine primary, texture * primary
			}
		}
	}
}

