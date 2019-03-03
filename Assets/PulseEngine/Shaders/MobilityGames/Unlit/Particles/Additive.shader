Shader "MobilityGames/Unlit/Particles/Additive" {
Properties {
	_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Back Lighting Off ZWrite Off Fog { Mode Off }
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		// ---- Dual texture cards
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					constantColor [_Color]
					combine constant * primary
				}
				SetTexture [_MainTex] {
					combine texture * previous DOUBLE
				}
			}
		}	
	}
}
