// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Bravo/MatCap/MatCap Diffuse Bumped Specular Lightprobes" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}	
		_BumpMap ("Bumpmap (RGB)", 2D) = "bump" {}
		_MatCap ("MatCap (RGB)", 2D) = "black" {}
		_Specular ("Specular", 2D) = "white" {}
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
					half4	pos : SV_POSITION;
					half2	uv : TEXCOORD0;
					half3	TtoV0 : TEXCOORD1;
					half3	TtoV1 : TEXCOORD2;
					
					fixed3 	vlight : TEXCOORD3 ;
				};
				
				uniform half4 _BumpMap_ST;
				
				v2f vert (appdata_tan v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord,_BumpMap);
					
					TANGENT_SPACE_ROTATION;
					o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
					o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);
					
					half3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
					half3 shlight = ShadeSH9 (float4(worldN,1.0));
					o.vlight = shlight;
					
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _BumpMap;
				uniform sampler2D _MatCap;
				uniform sampler2D _Specular;
				uniform fixed _RimLightIntensity;
				
				float4 frag (v2f i) : COLOR
				{
					half3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
					
					half2 vn;
					vn.x = dot(i.TtoV0, normal);
					vn.y = dot(i.TtoV1, normal);
					
					fixed4 matcapLookup  = tex2D(_MatCap, vn*0.5 + 0.5);
					fixed4 mainTextColor = tex2D(_MainTex,i.uv);
					fixed4 specularTextColor = tex2D(_Specular,i.uv);
					
					matcapLookup.a = 1;
					
					fixed4 c;
					c.rgb = i.vlight;
					c.a = 1;
					
					return (mainTextColor + matcapLookup * _RimLightIntensity * specularTextColor) * c;
				}
			ENDCG
		}
	}
}