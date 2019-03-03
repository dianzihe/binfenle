// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "MobilityGames/Unlit/Transparent/UnlitCutoutCullOff" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100
	
	Cull Off
	//ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Alphatest Greater [_Cutoff]
		AlphaToMask true
		
		
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}
