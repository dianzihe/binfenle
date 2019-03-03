// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// The overlay texture is set using the UV2 channel. This is the one that will be filled. It can be filled on S uv axis and/or T uv axis.
// The params for it are read from the colors channel of the vertices. (r = fillX, g= fillY, b and a not used yet)
Shader "MobilityGames/Unlit/Effects/LineRenderer-TransparentEnd" {
	Properties {
		_BaseTex ("Base", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
						
			struct appdata {
			    float4 vertex    : POSITION;
			    float2 texcoord  : TEXCOORD0;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			    float2 uv2 : TEXCOORD1;
			};
			
			sampler2D _BaseTex;
			float4 _BaseTex_ST;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _BaseTex);
			    o.uv2 = v.texcoord;
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				half4 colorBase = tex2D(_BaseTex, i.uv);
				colorBase.a *= log(1 - i.uv2.x) * 0.25 + 1.0;
				colorBase.a *= log(i.uv2.x) * 0.25 + 1.0;
				colorBase.a *= 1.25;
				return colorBase;
			}
			ENDCG
		}		
	} 
}
