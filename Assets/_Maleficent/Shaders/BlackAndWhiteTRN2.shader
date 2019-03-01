Shader "Bravo/Black & White/Diffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _bwBlend ("Black & White blend", Range (0, 1)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
        float4 _Color;
        float _bwBlend;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            half lum = c.r*.3 + c.g * .59 + c.b * .11;
            half3 bw = half3( lum, lum, lum );
 
            o.Albedo = lerp(c.rgb, bw, _bwBlend);
            o.Alpha = c.a;
        }
        ENDCG
    }
 
    Fallback "VertexLit"
}
