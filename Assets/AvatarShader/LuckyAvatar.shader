Shader "Custom/Avatar"
{
    Properties
    { 
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
        _BaseMapArray ("Albedo (RGB)", 2DArray) = "white" {}
        _BaseMapIndex ("BaseMap Array Index", Integer) = 0
        _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
        _MARMap ("Metallic(R)AO(G)Roughness(B) Texture", 2D) = "white" {}
        _MARMapArray ("Metallic(R)AO(G)Roughness(B) Texture Array", 2DArray) = "white" {}
        _MARMapIndex ("Metallic(R)AO(G)Roughness(B) Texture Array Index", Integer) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapArray ("Normal Map Array", 2DArray) = "bump" {}
        _NormalMapIndex ("Normal Map Array Index", Integer) = 0
        _NormalStrength ("Normal strength", Range(0, 1)) = 1
        _Occlusion ("Ambient Occlusion", Range(0, 1)) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
        _DiffuseColor2 ("Diffuse Color2", Color) = (1, 1, 1, 1)
        _LightDirection ("Light Direction", Vector) = (0, 0, 0, 0)
        _LightDirection2 ("Light Direction2", Vector) = (0, 0, 0, 0)
        _RimColor ("Rim Light Color", Color) = (1, 1, 1, 1)
        _RimColor2 ("Rim Light Color2", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Light Hardness", Range(0.0, 5.0)) = 4.0
        _RimPower2 ("Rim Light Hardness2", Range(0.0, 5.0)) = 4.0
        _GradientScale ("Gradient Scale", Vector) = (0, 0, 1, 1)
        _GradientAngle ("Gradient Angle", Range(-180, 180)) = 0
        _GradientPower ("Gradient Hardness", Range(1, 100)) = 1
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _DiffuseIntensity ("MainLight Intensity", Range(0, 10)) = 1
        _DiffuseIntensity2 ("MainLight Intensity2", Range(0, 10)) = 1
        _RimIntensity ("RimLight Intensity", Range(0, 10)) = 1
        _RimIntensity2 ("RimLight Intensity2", Range(0, 10)) = 1
        _UVTilingX ("UV Tiling X", Integer) = 4

        [HideInInspector] _EulerLightDirection("Light Dirrection (Euler)", Vector) = (90, 0, 0, 0)
        [HideInInspector] _EulerLightDirection2("Light Dirrection (Euler)2", Vector) = (90, 0, 0, 0)
        [HideInInspector] _DebugMipmapTex ("Debug mipmap texture", 2D) = "white" {}
        [HideInInspector] _DebugMipmapTexArray ("Debug mipmap texture array", 2DArray) = "white" {}
        [HideInInspector] _SrcBlend ("SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("ZWrite", Float) = 1
        [HideInInspector] _Cull ("Culling", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 300

        Pass
        {
            Name "UniversalForward"
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex AvatarPassVertex
            #pragma fragment AvatarPassFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #pragma shader_feature_local_fragment _ _SIMPLELIT _UNLIT _CUSTOMLIT
            #pragma shader_feature_local_fragment _ _MARMAP
            #pragma shader_feature_local_fragment _ _DIFFUSE_COLOR
            #pragma shader_feature_local_fragment _ _REFLECTION
            #pragma shader_feature_local_fragment _ _RIM_LIGHT
            #pragma shader_feature_local_fragment _ _HALF_LAMBERT _MINNAERT
            #pragma shader_feature_local_fragment _ _DEBUG_MIPMAP
            #pragma shader_feature_local_fragment _ _GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _RADIAL_GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _EXP_GRADIENT_MODE
            #pragma shader_feature_local_fragment _ _UVTILING
            #pragma shader_feature_local_fragment _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma multi_compile _ _CUSTOM_GPU_SKINNING

            #include "HLSL/AvatarInput.hlsl"
            #include "HLSL/AvatarForwardPass.hlsl"
            ENDHLSL
        }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        // without normal map
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex AvatarPassVertex
            #pragma fragment AvatarPassFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #pragma shader_feature_local_fragment _ _SIMPLELIT _UNLIT _CUSTOMLIT
            #pragma shader_feature_local_fragment _ _MARMAP
            #pragma shader_feature_local_fragment _ _DIFFUSE_COLOR
            #pragma shader_feature_local_fragment _ _REFLECTION
            #pragma shader_feature_local_fragment _ _RIM_LIGHT
            #pragma shader_feature_local_fragment _ _HALF_LAMBERT _MINNAERT
            #pragma shader_feature_local_fragment _ _DEBUG_MIPMAP
            #pragma shader_feature_local_fragment _ _GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _RADIAL_GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _EXP_GRADIENT_MODE
            #pragma shader_feature_local_fragment _ _UVTILING
            #pragma shader_feature_local_fragment _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma multi_compile _ _CUSTOM_GPU_SKINNING

            #undef _NORMALMAP

            #include "HLSL/AvatarInput.hlsl"
            #include "HLSL/AvatarForwardPass.hlsl"
            ENDHLSL
        }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        // unlit
        LOD 100

        Pass
        {
            Name "UniversalForward"
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex AvatarPassVertex
            #pragma fragment AvatarPassFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #pragma shader_feature_local_fragment _ _SIMPLELIT _UNLIT _CUSTOMLIT
            #pragma shader_feature_local_fragment _ _MARMAP
            #pragma shader_feature_local_fragment _ _DIFFUSE_COLOR
            #pragma shader_feature_local_fragment _ _REFLECTION
            #pragma shader_feature_local_fragment _ _RIM_LIGHT
            #pragma shader_feature_local_fragment _ _HALF_LAMBERT _MINNAERT
            #pragma shader_feature_local_fragment _ _DEBUG_MIPMAP
            #pragma shader_feature_local_fragment _ _GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _RADIAL_GRADIENT_LIGHT
            #pragma shader_feature_local_fragment _ _EXP_GRADIENT_MODE
            #pragma shader_feature_local_fragment _ _UVTILING
            #pragma shader_feature_local_fragment _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma multi_compile _ _CUSTOM_GPU_SKINNING

            #undef _NORMALMAP
            #undef _MARMAP
            #undef _SIMPLELIT
            #undef _CUSTOMLIT
            #define _UNLIT

            #include "HLSL/AvatarInput.hlsl"
            #include "HLSL/AvatarForwardPass.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "LuckyAvatarShaderGUI"
}