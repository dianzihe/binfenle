Shader "MobilityGames/Unlit/Particles/Additive-Culled" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		AlphaTest Off
		Cull Back Lighting Off ZWrite Off Fog { Mode Off }
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					combine texture * primary
				}
			}
		}	
	}
}
