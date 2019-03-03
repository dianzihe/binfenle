Shader "MobilityGames/Unlit/Transparent/MobileTransparentVertexColorZWriteOnCullOff" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite On
	Alphatest Greater 0
	Blend SrcAlpha OneMinusSrcAlpha 
	Cull Off
	
	SubShader {
		Pass {
			ColorMaterial AmbientAndDiffuse
			Fog { Mode Off }
			Lighting Off
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