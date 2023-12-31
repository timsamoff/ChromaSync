Shader "Custom/StandardWithAxialScanTranslucent"
{
    Properties
    {
        _Color ("Color", Color) = (.5, .5, .5, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
        _Glossiness ("Smoothness", Range (0,1)) = 0.5
        _Metallic ("Metallic", Range (0,1)) = 0.0

        // New properties for axial scan effect
        _ScanOffset ("Scan Offset", Range (0, 1)) = 0
        _ScanComplete ("Scan Complete", Range (0, 1)) = 0
        _FadeOutSpeed ("Fade Out Speed", Range (0, 10)) = 2.0
        _Translucency ("Translucency", Range (0, 1)) = 0.25

        // New property for camera Y-axis direction
        _CameraYDirection("Camera Y Direction", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend

        // Include standard lighting function
        #include "UnityPBSLighting.cginc"

        sampler2D _MainTex;
        fixed4 _Color;
        float _Glossiness;
        float _Metallic;
        float _ScanOffset;
        float _ScanComplete;
        float _FadeOutSpeed;
        float _Translucency;
        fixed3 _CameraYDirection;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            INTERNAL_DATA
        };

        // Function to convert world position to object's local coordinates
        float3 ComputeLocalPos(float3 worldPos)
        {
            return mul(unity_ObjectToWorld, float4(worldPos, 1.0)).xyz;
        }

        // Standard lighting function with axial scan effect
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Only show the scan effect when it's not complete
            if (_ScanComplete < 1)
            {
                // Convert world position to object's local coordinates
                float3 localPos = ComputeLocalPos(IN.worldPos);

                // Calculate the normalized height based on the object's pivot point
                float normalizedHeight = (localPos.y + 0.5) / 1.0; // Assuming object's height is 1.0

                // Use the camera's Y-axis direction for animation
                float cameraYDirection = dot(normalize(_WorldSpaceCameraPos.xyz - IN.worldPos), _CameraYDirection.xyz);

                // Calculate the modified scan offset based on the object's size and camera Y-axis direction
                float modifiedScanOffset = _ScanOffset * 2.0 * cameraYDirection;

                // Determine if the scan is visible based on the modified offset and normalized height
                float scanEffect = 1.0 - step(modifiedScanOffset, normalizedHeight);

                // Discard transparent regions
                if (scanEffect == 0.0)
                    discard;

                // Only apply the scan effect where it's needed
                o.Albedo = float3(1, 1, 1);
                o.Alpha = _Translucency; // Set the alpha to the specified translucency value
            }
            else
            {
                // Gradually reduce the intensity of the scan effect when complete
                float fadeIntensity = 1.0 - saturate((_ScanComplete - 1.0) / _FadeOutSpeed);
                o.Albedo = lerp(o.Albedo, _Color.rgb, fadeIntensity);
                o.Alpha = 1.0;
            }

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }

        ENDCG
    }

    FallBack "Diffuse"
}