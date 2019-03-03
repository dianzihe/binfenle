Shader "MobilityGames/Unlit/Transparent/VertexColor-AlphaTest" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite Off
	Alphatest Greater 0
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
	        SetTexture [_MainTex] {
	            constantColor [_Color]
	            Combine previous * constant DOUBLE, previous * constant
	        }  
		}
	} 
}
}