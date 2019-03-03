Shader "MobilityGames/Unlit/Transparent/SimpleVertexColorOverlay" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite Off
	ZTest Off
	Cull Back Lighting Off Fog { Mode Off } 
	Blend SrcAlpha OneMinusSrcAlpha
	
	SubShader {
		Pass {

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