Shader "Custom/ScanEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _ScanTex ("Scan Texture", 2D) = "white" { }
        _ScanSpeed ("Scan Speed", Range (0, 10)) = 1
        _OverlayStrength ("Overlay Strength", Range (0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        sampler2D _ScanTex;
        float _ScanSpeed;
        float _OverlayStrength;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ScanTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the base texture
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate distortion based on the scan texture and time
            float scanDistortion = tex2D(_ScanTex, IN.uv_ScanTex + _Time.y * _ScanSpeed).r;

            // Blend the base color with the scanning effect using alpha blending
            o.Albedo = lerp(baseColor.rgb, baseColor.rgb * scanDistortion, _OverlayStrength);
            o.Alpha = baseColor.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}