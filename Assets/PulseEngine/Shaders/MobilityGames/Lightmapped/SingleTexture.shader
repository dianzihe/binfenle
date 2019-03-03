// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MobilityGames/Lightmapped/SingleTexture" {
Properties {
    _MainTex ("Main Texture", 2D) = "white" { }
    _LightMap ("LightMap", 2D) = "white" { }
    _LightIntensity ("LightIntensity", Float) = 1
}
SubShader {
    Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _LightMap;

struct appdata {
    float4 vertex : POSITION;
    float4 texcoord : TEXCOORD0;
    float4 texcoord1: TEXCOORD1;
};

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
    float2 lightUV : TEXCOORD1;
};

float4 _MainTex_ST;
float4 _LightMap_ST;
float _LightIntensity;

v2f vert (appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    o.lightUV = TRANSFORM_TEX (v.texcoord1, _LightMap);
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv);
    half4 lightCol = tex2D (_LightMap, i.lightUV) * 2 * _LightIntensity;
    return (texcol * lightCol);
}
ENDCG

    }
}
Fallback "VertexLit"
} 