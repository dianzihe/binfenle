Shader "MobilityGames/Unlit/Transparent/SimpleVertexColorCubemap" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_Cube ("Cubemap", CUBE) = "" {}
	_SpecularIntensity ("Specular Intensity", Range(0, 1)) = 1
}
SubShader {
	Tags { "RenderType" = "Opaque" }
	CGPROGRAM
	#pragma surface surf SimpleLambert nolightmap nodirlightmap approxview novertexlights

      half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
          half4 c;
          c.rgb = s.Albedo;
          c.a = s.Alpha;
          return c;
      }
	struct Input {
		float2 uv_MainTex;
		float3 worldRefl;
		fixed4 color : COLOR;
	};
	
	sampler2D _MainTex;
	samplerCUBE _Cube;
	float _SpecularIntensity;		
		
	void surf (Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * IN.color.rgb;
		o.Emission = texCUBE (_Cube, IN.worldRefl).rgb * _SpecularIntensity;
	}
	ENDCG
} 
Fallback "Diffuse"
}

