// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/Effects/Pixelate-Transparent" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
		_Amount("Amount", Range(0.0009765625, 0.1)) = 0.0009765625
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
			    float2 texcoord  : TEXCOORD;
			};
			
			struct v2f {
			    float4  pos  : SV_POSITION;
			    float2  uv   : TEXCOORD;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Amount;
			half _Alpha;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				
				float2 newUv = _Amount * float2(floor(i.uv.x / _Amount), floor(i.uv.y / _Amount));
				half4 colorBase = tex2D(_MainTex, newUv);
				
				// Average the current, top, top-right and bottom-right texels
				_Amount *= 1.5f;
				// sample top-right texel	
				colorBase +=  tex2D(_MainTex, float2(newUv.x + _Amount, newUv.y + _Amount));
				// sample top texel
				colorBase += tex2D(_MainTex, float2(newUv.x, newUv.y + _Amount));
				// sample right texel
				colorBase += tex2D(_MainTex, float2(newUv.x + _Amount, newUv.y));
				colorBase *= 0.25f;
				
				colorBase.a *= _Alpha;
								
				return colorBase;
			}
			ENDCG
		}		
	} 
}
