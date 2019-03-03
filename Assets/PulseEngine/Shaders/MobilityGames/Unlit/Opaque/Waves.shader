// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Unlit/Opaque/Waves" {
	Properties {	    
	    _MainTex ("Texture", 2D) = "white" { }
	    _Speed ("Wave Speed", Float) = 1.6
	    _Scale ("Wave Scale", Float) = 1.5
	    _ScrollDir ("Scroll Amount (On X, On Y, Factor, NOT Used)", Vector) = (0.1, 0.1, 0.3, 0)
	}
	
	SubShader {
	    Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			half _Speed;
			half _Scale;
			half4 _ScrollDir;
			
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};

			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			uniform float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;			    
			    o.pos = UnityObjectToClipPos (v.vertex);
				o.pos.y += sin(_Time.z * _Speed + v.vertex.x + v.vertex.z) * _Scale;
				
			    _MainTex_ST.zw = half2(_ScrollDir.x, _ScrollDir.y) * _Time.z * _ScrollDir.z;
			    _MainTex_ST.z = fmod(_MainTex_ST.z, 1.0);
			    _MainTex_ST.w = fmod(_MainTex_ST.w, 1.0);
			    
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				return tex2D (_MainTex, i.uv);
			}
			ENDCG
	    }
	}
	Fallback "VertexLit"
} 