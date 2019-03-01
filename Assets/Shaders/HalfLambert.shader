Shader "_Maleficent/HalfLambert" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {  "Queue"="Transparent" "RenderType"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf CustomCharacter

		half4 LightingCustomCharacter(SurfaceOutput  s, half3 lightDir, half3 viewDir, half atten)
		{
			// Calculate diffuse term (half lambert; aka Wrapped Lambertian term)
			half NdotL = dot(s.Normal, lightDir);
			
			half halfLambert = NdotL * 0.5 + 0.5;
			
			half4 litColor;		
			litColor.a = s.Alpha;
			litColor.rgb = s.Albedo * _LightColor0.rgb * (halfLambert * atten * 2);
			
			return litColor;
		}


		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
