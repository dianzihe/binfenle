Shader "Custom/DiffuseCharacter"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecMap ("SpecMap (Grayscale)", 2D) = "black" {}
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
		_RimColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_SpecExponentScale ("SpecExponentScale", Range(0.05, 1.0)) = 0.078125
		_GlossAmountScale ("GlossAmountScale", Range(0.05, 2.0)) = 1.0
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf CustomDiffuseSpec
		#pragma debug
		
		fixed _SpecExponentScale;
		fixed _GlossAmountScale;
		half _RimPower;
		fixed3 _RimColor;
		
		inline half4 LightingCustomDiffuseSpec(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half4 litColor;
			
			// Calculate half lambert diffuse
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;

			
			// Calcualte blinn-phong specular
			half3 h = normalize (lightDir + viewDir);			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular * 128.0) * s.Gloss;

			// Final light color			
			litColor.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
			litColor.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;

			return litColor;
		}

		sampler2D _MainTex;
		sampler2D _SpecMap;
		
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_SpecMap;
//			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			half4 spec = tex2D(_SpecMap, IN.uv_MainTex);
			fixed specMask = saturate(spec.r + spec.g + spec.b);
			
			o.Albedo = c.rgb;
			o.Specular = saturate(1.0 - _SpecExponentScale); //TODO: read specular exponent from mask texture (no texture to test with)
			o.Gloss = specMask * _GlossAmountScale;
			o.Alpha = c.a;
			
//			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
//         	o.Emission = _RimColor.rgb * pow (rim, _RimPower);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
