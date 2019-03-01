Shader "MobilityGames/Lit/CharacterShader" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NormalMap ("NormalMap", 2D) = "bump" {}
		_Mask1 ("Mask1 (R)DetailMap, (G)Diffuse/Fresnel map, (B)Metalness, (A)SelfIllum", 2D) = "black" {}
		_Mask2 ("Mask2 (R)SpecIntensity, (G)RimIntensity, (B)SpecColorTintToBase, (A)SpecExponent", 2D) = "black" {}
		_RimLightScale ("RimLightScale", Float) = 2.0
		_RimColor ("RimColor", Color) = (1.0, 1.0, 1.0, 0.0)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf CustomCharacter
		#pragma debug
		
		struct ExtSurfaceOutput
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed4 Mask1;
			fixed4 Mask2;

			half Specular; 
			fixed Gloss;
			fixed Alpha;
		};
		
		fixed _RimLightScale;
		fixed4 _RimColor;
		
		inline half4 LightingCustomCharacter(ExtSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half4 litColor;
			
			// Calculate diffuse term (half lambert; aka Wrapped Lambertian term)
			half NdotL = dot(s.Normal, lightDir);
			//TODO: externalize half lambert wrap range (default: 0.5) ? (only if required)
			half halfLambert = NdotL * 0.5 + 0.5;
					
			litColor.a = s.Alpha;
			litColor.rgb = s.Albedo * _LightColor0.rgb * (halfLambert * atten * 2);
			
			return litColor;
		}

		struct Input 
		{
			float2 uv_MainTex;
			half3 viewDir;
		};

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _Mask1;
		sampler2D _Mask2;
		
		
		void surf (Input IN, inout ExtSurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			
			o.Mask1 = tex2D(_Mask1, IN.uv_MainTex);
			o.Mask2 = tex2D(_Mask2, IN.uv_MainTex);
			
			//TODO: this area for calcualting rim lighting can be done in a much more packed and optimized manner
			// Calculate initial fresnel term
			half3 fresnelTerm = 0.0;
			half fresnel = 1.0 - saturate(dot(o.Normal, normalize(IN.viewDir)));
			
			fresnelTerm.r = 0.2 + 0.85 * pow(1.0 - max(0, dot(normalize(IN.viewDir), o.Normal)), 4.0); // specular
			fresnelTerm.r *= fresnelTerm.r;
			fresnelTerm.r = max(fresnelTerm.r, o.Mask1.b);
			
			fresnelTerm.g = fresnel * fresnel; // Rim lighting?
			
			fresnelTerm.b = fresnelTerm.g;
	
			// Calculate rim lighting
			half3 rimLighting = (fresnelTerm.g * _RimLightScale) * o.Mask2.g;
			rimLighting *= saturate( dot(o.Normal, half3(0.0, 1.0, 0.0)) );
			rimLighting *= _RimColor;
			rimLighting *= (1.0 - o.Mask1.b); // metalness
			
			o.Emission = rimLighting;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
