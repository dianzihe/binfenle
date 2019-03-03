Shader "MobilityGames/Unlit/Opaque/MobileRim" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
//	_BumpMap ("Bumpmap", 2D) = "bump" {}
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	_AmbientLightStrength ("Ambient Light Strength", Range(0.0, 2.0)) = 1.5
}
SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma surface surf None

half4 LightingNone (SurfaceOutput s, half3 lightDir, half atten) {
          return half4(0.0, 0.0, 0.0, 0.0);
      }

sampler2D _MainTex;
samplerCUBE _Cube;
//sampler2D _BumpMap;
fixed4 _Color;
fixed4 _ReflectColor;
float4 _RimColor;
float _RimPower;
float _AmbientLightStrength;

struct Input {
	float2 uv_MainTex;
//	float2 uv_BumpMap;
	float3 worldRefl;
	float3 viewDir;
	
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	//c = _Color;
	//o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
	o.Albedo = c.rgb * _AmbientLightStrength;
	
	
	
	fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
	reflcol *= tex.a;
	//reflcol = 0.0;
	half rim = (1.0 - saturate(dot (normalize(IN.viewDir), o.Normal)));
	o.Emission = (reflcol.rgb * _ReflectColor.rgb * _RimColor.rgb * pow (rim, _RimPower));
	//o.Emission = 0.0;
	o.Alpha = reflcol.a * _ReflectColor.a;

}
ENDCG
}
	
FallBack "Diffuse"
} 

  
//Shader "Mobile/Mobile Rim" {
// Properties {
//      _MainTex ("Texture", 2D) = "white" {}
//      _BumpMap ("Bumpmap", 2D) = "bump" {}
//      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
//      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
//    }
//    SubShader {
//      Tags { "RenderType" = "Opaque" }
//      CGPROGRAM
//      #pragma surface surf Lambert
//      struct Input {
//          float2 uv_MainTex;
//          float2 uv_BumpMap;
//          float3 viewDir;
//      };
//      sampler2D _MainTex;
//      sampler2D _BumpMap;
//      float4 _RimColor;
//      float _RimPower;
//      void surf (Input IN, inout SurfaceOutput o) {
//          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
//          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
//          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
//      }
//      ENDCG
//    } 
//    Fallback "Diffuse"
//  }
  
//  Shader "Mobile/Mobile Rim" {
//    Properties {  
//  _MainTex ("Texture", 2D) = "white" {} 
	
//      _Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
//      _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
//      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
//
//    }  
//    SubShader {  
//      Tags { "RenderType" = "Opaque" }  
//      CGPROGRAM  
//      #pragma surface surf Lambert  
//      struct Input {  
//          float2 uv_MainTex;  
//          float3 worldRefl;
//          float3 viewDir;
//      };  
//      sampler2D _MainTex;  
//      samplerCUBE _Cube;
//      float _RimPower;
//      void surf (Input IN, inout SurfaceOutput o) {  
//          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.5;
//          half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
//          o.Emission = texCUBE (_Cube, IN.worldRefl).rgb * pow(rim,_RimPower);
//      }  
//      ENDCG  
//    }   
//    Fallback "Diffuse"  
//}
