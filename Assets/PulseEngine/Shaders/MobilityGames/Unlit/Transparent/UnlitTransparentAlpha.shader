// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "MobilityGames/Unlit/Transparent/UnlitTransparentAlpha" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Alpha ("Alpha", float) = 1.0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Lighting Off
		
		SetTexture [_MainTex] { 
			constantColor(1.0, 1.0, 1.0, [_Alpha])
			combine texture * constant
		} 
	}
}
}
