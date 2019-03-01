// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Shaders/Unlit/Transparent/UnlitTransparentTexturedAlpha" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D)    = "white" {}
		_Color   ("Tint",     Color) = (1,1,1,1) 
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
				
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = v.texcoord;
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform fixed4 _Color;
				
				float4 frag (v2f i) : COLOR
				{
					//Main text
					fixed4 mainTextColor;
					fixed4 textColor = tex2D(_MainTex, i.uv);
					mainTextColor.rgb = textColor.aaa * _Color.rgb;
					mainTextColor.a = _Color.a * textColor.a;
					
					return mainTextColor;
				}
			ENDCG
		}
	}
}
