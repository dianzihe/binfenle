// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Shine" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D)    = "white" {}
		_ShineTex ("Shine", 2D) = "white" {}
		_Factor ("Divide Factor", FLOAT) = 0
		_DisplacementY ("DisplacementY", FLOAT) = 0
	}
	
	Subshader {
		Tags { 
			"Queue"="Transparent" 
			"RenderType"="Opaque"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Lighting Off
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				struct v2f { 
					half4 	pos : SV_POSITION;
					half2	uv : TEXCOORD0;
					half4	cachePos : NORMAL1;
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
					o.cachePos = v.texcoord;
					o.cachePos.z = UNITY_MATRIX_V[0][3];
					o.cachePos.w = UNITY_MATRIX_V[3][1];
					
					return o;
				} 
				
				uniform sampler2D _MainTex;
				uniform sampler2D _ShineTex;
				uniform fixed _Factor;
				uniform fixed _DisplacementY;
				
				float4 frag (v2f i) : COLOR
				{
					//Main text
					fixed4 mainTextColor = tex2D(_MainTex, i.uv);
					half2 UVcoords = half2(i.cachePos.x / _Factor, i.cachePos.y / _Factor);

					UVcoords.x += fmod(_Time.z + i.cachePos.z * 0.42, 6.0) - 1;
					UVcoords.y += _DisplacementY; 
					
					fixed4 shineColor = tex2D(_ShineTex, UVcoords);
					
					mainTextColor.rgb += shineColor.aaa;
					
					return mainTextColor;
				}
			ENDCG
		}
	}
}
