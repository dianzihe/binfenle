// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/Effects/Blur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Blur ("Blur", Range(0, 1)) = 0
	}
	SubShader {

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			float _Blur;
			
			v2f vert (appdata v) 
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR 
			{
				half4 resultColor  = tex2D(_MainTex, i.uv);
				for (float j = -2; j < 2; j++) { 
					float2 newCoord = i.uv + float2(0, j);//delta * (j / maxJ);
					float4 m = tex2D(_MainTex, newCoord);
					resultColor = lerp (resultColor, m, 0.25);
				}
				return resultColor;
			}
			ENDCG
		}		
	} 
}
