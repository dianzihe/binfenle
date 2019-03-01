// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Bravo/MatCap/MatCap Diffuse LightProbes" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}	
		_MatCap ("MatCap (RGB)", 2D) = "black" {}
		_RimLightIntensity("Rimlight Intensity", FLOAT) = 2.0
	}
	
	Subshader {
		Tags { "RenderType"="Opaque" }
		Cull Off
		Fog { Color [_AddFog] }
		
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
					half3 	n;
					
					fixed3 	vlight : TEXCOORD3;
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
					
					half3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
					half3 shlight = ShadeSH9 (float4(worldN,1.0));
					o.vlight = shlight;
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _MatCap;
				fixed _RimLightIntensity;
				
				float4 frag (v2f i) : COLOR
				{
					half2 vn;
					vn.x = dot(i.TtoV0, i.n);
					vn.y = dot(i.TtoV1, i.n);

					fixed4 matcapLookup  = tex2D(_MatCap, vn*0.5 + 0.5);
					fixed4 mainTextColor = tex2D(_MainTex,i.uv);
					
					fixed4 c;
					c.rgb = i.vlight;
					return (mainTextColor + matcapLookup * _RimLightIntensity) * c;
				}
			ENDCG
		}
	}
}