Shader "Avatar/GradientChecker"
{
    Properties
    { 
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("BaseColor", Color) = (1, 1, 1, 1)
        _Color2 ("BaseColor2", Color) = (1,1,1,1)
        _GradientScale ("Gradient Scale", Vector) = (1, 1, 1, 1)
        _GradientAngle ("Gradient Angle", Range(-180, 180)) = 0
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _GradientPower ("Gradient Power", Range(1, 100)) = 1
        [KeywordEnum(LINEAR, RADIAL)] _GradientStyle("Gradient Style", Integer) = 0
        [KeywordEnum(LINEAR, EXP)] _GradientMethod("Gradient Method", Integer) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local _GRADIENTMETHOD_LINEAR _GRADIENTMETHOD_EXP
            #pragma shader_feature_local _GRADIENTSTYLE_LINEAR _GRADIENTSTYLE_RADIAL

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float2 uv : TEXCOORD0;
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                float3 positionWS : TEXCOORD1;
            };

            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            half4 _Color2;
            float4 _GradientScale;
            float _GradientAngle;
            float _Alpha;
            float _GradientOffset;
            float _GradientPower;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                // Returning the output.
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half2 gradient = (IN.positionWS.xz - _GradientScale.xy) / (_GradientScale.zw - _GradientScale.xy);
                //half2 gradient = saturate((IN.positionWS.xz - _GradientScale.xy) / (_GradientScale.zw - _GradientScale.xy));
                half angleCos = cos(radians(_GradientAngle));
                half angleSin = sin(radians(_GradientAngle));
                half2x2 rotateMatrix = half2x2(angleCos, -angleSin, angleSin, angleCos);
                half2 center = (_GradientScale.xy + _GradientScale.zw) / 2;
                gradient = mul(gradient - 0.5, rotateMatrix) + 0.5;
#if _GRADIENTMETHOD_LINEAR
#if _GRADIENTSTYLE_LINEAR
                half3 baseColor = lerp(_Color.rgb, _Color2.rgb, saturate(gradient.x + _GradientOffset));
#elif _GRADIENTSTYLE_RADIAL
                half3 baseColor = lerp(_Color.rgb, _Color2.rgb, saturate(length((gradient - 0.5) * 2) + _GradientOffset));
#endif
#elif _GRADIENTMETHOD_EXP
#if _GRADIENTSTYLE_LINEAR
                half3 baseColor = lerp(_Color.rgb, _Color2.rgb, pow(saturate(gradient.x + _GradientOffset), _GradientPower));
#elif _GRADIENTSTYLE_RADIAL
                half3 baseColor = lerp(_Color.rgb, _Color2.rgb, pow(saturate(length((gradient - 0.5) * 2) + _GradientOffset), _GradientPower));
#endif
#endif
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return half4(col.rgb * baseColor, _Alpha);
                //return half4(gradient, 0, _Alpha);
            }
            ENDHLSL
        }
    }
}