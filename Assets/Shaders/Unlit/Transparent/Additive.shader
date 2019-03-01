Shader "_Maleficent/Shaders/Unlit/Transparent/Additive" {
Properties {
	_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Back Lighting Off ZWrite On Fog { Mode Off }
		
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
					combine constant * primary DOUBLE
				}
				SetTexture [_MainTex] {
					combine texture * previous// DOUBLE
				}
			}
		}	
	}
}