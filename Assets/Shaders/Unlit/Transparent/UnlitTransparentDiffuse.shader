// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Maleficent/Shaders/Unlit/Transparent/UnlitTransparentDiffuse" {
	Properties {
		_Color   ("Tint",     Color) = (1,1,1,1) 
	}
	 
	Subshader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		Pass {
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				struct v2f { 
					half4 	pos : SV_POSITION;
				};
				
				// vertex input: position, color
				struct appdata {
				    float4 vertex : POSITION;
				};
				
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					
					return o;
				}
				
				uniform fixed4 _Color;
				
				float4 frag (v2f i) : COLOR
				{
					return _Color;
				}
			ENDCG
		}
	}
}
