// ============================================================================
//  Custom/PlayerXRay  —  URP mobile-optimised X-Ray / Silhouette shader
//
//  Dùng cho Phương án A: đổi material khi player bị occlude.
//  ZTest Always  → render xuyên qua mọi depth (tường, vật thể, v.v.)
//  ZWrite Off    → không ghi depth buffer, không ảnh hưởng vật thể sau
//  Fresnel rim   → viền sáng ở cạnh nhân vật, giữa trong suốt hơn
//
//  SETUP MATERIAL:
//  1. Tạo material mới → shader "Custom/PlayerXRay"
//  2. Đặt tên "M_PlayerXRay"
//  3. Color: ví dụ (0, 0.8, 1, 0.55) — xanh cyan, 55% alpha
//  4. Rim Power: 2.5  (cao hơn = viền mỏng hơn)
//  5. Rim Alpha Boost: 0.7
//  6. Gán vào PlayerVisibilityTintURP.XRayMaterial
// ============================================================================
Shader "Custom/PlayerXRay"
{
    Properties
    {
        _Color         ("Tint Color",      Color)         = (0, 0.8, 1, 0.55)
        _RimPower      ("Rim Power",       Range(0.5, 8)) = 2.5
        _RimAlphaBoost ("Rim Alpha Boost", Range(0, 1))   = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent+100"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector"= "True"
        }

        // ── Single pass: ZTest Always → vẽ xuyên tường ───────────────────
        Pass
        {
            Name "PlayerXRay"
            Tags { "LightMode" = "UniversalForward" }

            ZTest  Always       // Render bất kể depth
            ZWrite Off          // Không ghi depth buffer
            Cull   Back
            Blend  SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // GPU instancing — tiết kiệm draw call trên mobile
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half  _RimPower;
                half  _RimAlphaBoost;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half3  normalWS   : TEXCOORD0;
                half3  viewDirWS  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs posInputs    = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.normalWS   = (half3)normalInputs.normalWS;
                OUT.viewDirWS  = (half3)GetWorldSpaceViewDir(posInputs.positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 N = normalize(IN.normalWS);
                half3 V = normalize(IN.viewDirWS);

                // Fresnel rim: cạnh nhân vật sáng hơn, giữa mờ → hiệu ứng silhouette
                half  rim       = 1.0h - saturate(dot(N, V));
                half  rimFactor = pow(rim, _RimPower);

                half4 color = _Color;
                // Alpha = base alpha + viền được boost thêm
                color.a = saturate(color.a + rimFactor * _RimAlphaBoost);

                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
