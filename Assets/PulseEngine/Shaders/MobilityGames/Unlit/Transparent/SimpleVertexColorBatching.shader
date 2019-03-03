Shader "MobilityGames/Unlit/Transparent/SimpleVertexColorBatching" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent"}
	LOD 100
	
	Cull Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}
