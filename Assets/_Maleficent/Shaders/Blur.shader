// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Effects/Unlit/Blur" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D)    = "white" {}
		_Size ("Viewport Size", range (0,1)) = 1
	}
	 
	Subshader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Lighting Off
			
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
				
				float _Size;
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = v.texcoord * _Size;
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				float4 frag (v2f i) : COLOR{
					return tex2D(_MainTex, i.uv);
				}
			ENDCG
		}
	}
}
