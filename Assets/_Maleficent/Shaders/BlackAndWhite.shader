// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Effects/Unlit/B&W" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D)    = "white" {}
		_color ("Main Color", Color) = (1,1,1,1)
		_bwBlend ("Black & White blend", Range (0, 1)) = 0
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
				
				float4 _color;
				float _bwBlend;
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = v.texcoord;
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				half4 frag (v2f i) : COLOR{
					half4 c = tex2D(_MainTex, i.uv) * _color;
		            half lum = c.r*.3 + c.g*.59 + c.b*.11;
		            half3 bw = half3( lum, lum, lum );
				
					c.rgb = lerp(c.rgb, bw, _bwBlend);
            		return c;
				}
			ENDCG
		}
	}
}
