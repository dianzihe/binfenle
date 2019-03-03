Shader "MobilityGames/Unlit/Opaque/2TextureBlending" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Texture2 ("Texture 2", 2D) = "white" {}
    	_Blend ("Blend", Range (0, 1) ) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
	    Pass {
	        SetTexture[_MainTex]
	        SetTexture[_Texture2] { 
	            ConstantColor (0,0,0, [_Blend])
	            Combine texture Lerp(constant) previous
	        }       
	    }
	}
	FallBack "Diffuse"
}