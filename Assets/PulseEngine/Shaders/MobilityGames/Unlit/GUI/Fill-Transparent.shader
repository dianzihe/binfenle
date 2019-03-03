// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// The overlay texture is set using the UV2 channel. This is the one that will be filled. It can be filled on S uv axis and/or T uv axis.
// The params for it are read from the colors channel of the vertices. (r = fillX, g= fillY, b and a not used yet)
Shader "MobilityGames/Unlit/GUI/Fill-Transparent" {
	Properties {
		_BaseTex ("Base", 2D) = "white" {}
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
						
			struct appdata {
			    float4 vertex    : POSITION;
			    float2 texcoord  : TEXCOORD0;
			    float2 texcoord1 : TEXCOORD1;
			    half4  color 	 : COLOR;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv1 : TEXCOORD0;
			    float2  uv2 : TEXCOORD1;
			    half4   col : COLOR;
			};
			
			sampler2D _BaseTex;
			float4 _BaseTex_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv1 = TRANSFORM_TEX (v.texcoord, _BaseTex);
			    o.uv2 = TRANSFORM_TEX (v.texcoord1, _BaseTex);
			    o.col = v.color;
			    
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				half4 colorBase = tex2D(_BaseTex, i.uv1);
				half4 colorOverlay = tex2D(_BaseTex, i.uv2);
				half4 resultColor = colorOverlay;
				
				float fill = lerp(i.col.r, i.col.g, i.col.a);
				
				if (i.col.b > 0.8f && i.uv2.y > fill) {
					resultColor = colorBase;
				} else if (i.col.b >= 0.5f && i.col.b < 0.8f && i.uv2.y < fill) {
					resultColor = colorBase;
				}
								
				if (i.col.b < 0.25f && i.uv2.x > fill) {
					resultColor = colorBase;
				} else if (i.col.b > 0.3f && i.col.b < 0.5f && i.uv2.x < fill) {
					resultColor = colorBase;
				}
				
				return resultColor;
			}
			ENDCG
		}		
	} 
}
