// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/GUI/RadialFeather" {
	Properties {
		_ColorTex ("Color", 2D) = "white" {}
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
			
			sampler2D _ColorTex;
			sampler2D _Mask;
			
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			float4 _ColorTex_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _ColorTex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				half4 c  = tex2D(_ColorTex, i.uv);
				
				float2 dist = i.uv - float2(0.5, 0.5);
				
				float alpha = c.a * saturate((0.25 - dot(dist, dist)) * 15);
				//Unoptimized version
				//float alpha = (0.5 - length(dist)) * 15;
				
				c.a = alpha;
				return c;
			}
			ENDCG
		}		
	} 
}
