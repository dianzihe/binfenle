Shader "MobilityGames/Unlit/Transparent/SimpleVertexColor" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite Off
	AlphaTest Off
	Blend SrcAlpha OneMinusSrcAlpha
	
	SubShader {
		Pass {
			Fog { Mode Off }
			Lighting Off

			BindChannels {
				Bind "Vertex", vertex
				Bind "texcoord", texcoord // main uses 1st uv
				Bind "Color", color
			}
			
	        SetTexture [_MainTex] {
	           	Combine texture * primary, texture * primary
	        }
		}
	} 
}
}