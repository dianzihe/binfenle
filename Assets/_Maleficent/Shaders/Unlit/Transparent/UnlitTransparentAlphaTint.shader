// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Shaders/Unlit/Transparent/UnlitTransparentAlphaTint" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D)    = "white" {}
		_Color   ("Tint",     Color) = (1,1,1,1) 
		_Intensity ("Intensity", Float) = 1
	}
	 
	Subshader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		Pass {
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				struct v2f { 
					half4 	pos : SV_POSITION;
					half2	uv : TEXCOORD0;
				};
				
				// vertex input: position, color
				struct appdata {
				    float4 vertex : POSITION;
				    float4 texcoord : TEXCOORD0;
				};
				
				uniform float4 _MainTex_ST; 
				
				v2f vert (appdata v)
				{ 
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					return o;
				}
				
				
				uniform sampler2D _MainTex;
				uniform fixed4 _Color;
				uniform float _Intensity;
				
				float4 frag (v2f i) : COLOR
				{
					//Main text
					fixed4 mainTextColor;
					fixed4 textColor = tex2D(_MainTex, i.uv);
					fixed4 lum = 0.299 * textColor.r + 0.587 * textColor.g + 0.114 * textColor.b;
					//mainTextColor.rgb = textColor.rgb * (1 - _Color.aaa) + lum * _Color.rgb * _Color.aaa;
					mainTextColor.rgb = textColor.rgb + lum * _Color.rgb * _Color.aaa * _Intensity;
					mainTextColor.a = textColor.a * _Color.a;
					
					return mainTextColor;
				}
			ENDCG
		}
	}
}
