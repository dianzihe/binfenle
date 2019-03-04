// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Bravo/MatCap/MatCap Diffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}	
		_MatCap ("MatCap (RGB)", 2D) = "black" {}
		_RimLightIntensity("Rimlight Intensity", FLOAT) = 2.0
		_Color ("Color", Color) = (1,1,1,1)
	}
	
	Subshader {
		Tags { 
			"RenderType"="Opaque" 
			"Queue"="Transparent" 
		}
		Cull Off
		Fog { Color [_AddFog] }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			Name "BASE"
			Tags { "LightMode" = "ForwardBase" }
			
			CGPROGRAM
				#pragma exclude_renderers xbox360
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				
				struct v2f { 
					half4 	pos : SV_POSITION;
					half2	uv : TEXCOORD0;
					half3	TtoV0 : TEXCOORD1;
					half3	TtoV1 : TEXCOORD2;
					half3 	n : NORMAL1;
				};
				
				v2f vert (appdata_tan v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = v.texcoord;
					
					TANGENT_SPACE_ROTATION;
					o.n = mul(rotation, v.normal);
					o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
					o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _MatCap;
				fixed _RimLightIntensity;
				float4 _Color;
				
				float4 frag (v2f i) : COLOR
				{
					half2 vn;
					vn.x = dot(i.TtoV0, i.n);
					vn.y = dot(i.TtoV1, i.n);

					fixed4 matcapLookup  = tex2D(_MatCap, vn * 0.5 + 0.5);
					fixed4 mainTextColor = tex2D(_MainTex, i.uv);
					
					
					return (mainTextColor + matcapLookup * _RimLightIntensity) * _Color;
				}
			ENDCG
		}
	}
}