Shader "MobilityGames/Unlit/Effects/Glitch" {

Category {
	Tags {"Queue"="Geometry" "IgnoreProjector"="True"}
	ZWrite On
	
	SubShader {
		Pass {
			Fog { Mode Off }
			Lighting Off

			BindChannels {
				Bind "Vertex", vertex
				Bind "Color", color
			}
		}
	} 
}
}