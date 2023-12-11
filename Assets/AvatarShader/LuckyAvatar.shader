Shader "Custom/Avatar"
{
    Properties
    { 
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
        _BaseMapArray ("Albedo (RGB)", 2DArray) = "" {}
        _BaseMapIndex ("BaseMap Array Index", Integer) = 0
        _SkinColor ("SkinColor", Color) = (1, 1, 1, 1)
        _MARMap ("Metallic(R)AO(G)Roughness(B) Texture", 2D) = "white" {}
        _MARMapArray ("Metallic(R)AO(G)Roughness(B) Map Array", 2DArray) = "" {}
        _MARMapIndex ("Metallic(R)AO(G)Roughness(B) Map Array Index", Integer) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapArray ("Normal Map Array", 2DArray) = "bump" {}
        _NormalMapIndex ("Normal Map Array Index", Integer) = 0
        _NormalStrength ("Normal strength", Range(0, 1)) = 1
        _Occlusion ("Ambient Occlusion", Range(0, 1)) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0
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
        _UVTiles ("Number of UV Tiles", Integer) = 4
        _MatCapMap ("MatCapMap", 2D) = "black" {}
        _DeepLayerColor ("DeepLayer Color", Color) = (0, 0, 0, 1)
        _MatCapBlend ("MatCap Blend", Range(0, 1)) = 0.5
        _MatCapHSVShift ("MatCap HSV Shift", Vector) = (0, 0, 0, 0)
        _UVCutoff ("UVTiling Index Cutoff", Integer) = 0
        _AnisoOffset ("Anisotropic Highlight Offset", Range(-1, 1)) = -0.3
        _AnisoPower ("AnisoTropic Highlight Hardness", Float) = 30
        _AnisoIntensity ("Anisotropic Highlight Intensity", Range(0, 1)) = 0.2
        _HairUVTileIndex ("Hair UVTile Index", Integer) = 0
        _AnisoHighlightColor ("Anisotropic Highlight Color", Color) = (1, 1, 1, 1)
        _HeightMap ("Height Map", 2D) = "black" {}
        _EyesMakeupMapArray ("EyesMakeupMap array", 2DArray) = "" {}
        _EyesMakeupMapArrayIndex ("EyesMakeupMap array Index", Integer) = 0
        _EyesMakeupUVTileInedx("EyesMakeup UVTile Index", Integer) = 0
        _BlushMakeupMapArray ("BlushMakeupMap array", 2DArray) = "" {}
        _BlushMakeupMapArrayIndex ("BlushMakeupMap array Index", Integer) = 0
        _BlushMakeupUVTileInedx("BlushMakeup UVTile Index", Integer) = 0
        _LipsMakeupMapArray ("LipsMakeupMap array", 2DArray) = "" {}
        _LipsMakeupMapArrayIndex ("LipsMakeupMap array Index", Integer) = 0
        _LipsMakeupUVTileInedx("LipsMakeup UVTile Index", Integer) = 0
        _FacialTatooMapArray ("FacialTatooMap array", 2DArray) = "" {}
        _FacialTatooMapArrayIndex ("FacialTatooMap array Index", Integer) = 0
        _FacialTatooUVTileInedx("FacialTatoo UVTile Index", Integer) = 0
        _FacialMarkingMapArray ("FacialMarkingMap array", 2DArray) = "" {}
        _FacialMarkingMapArrayIndex ("FacialMarkingMap array Index", Integer) = 0
        _FacialMarkingUVTileInedx("FacialMarking UVTile Index", Integer) = 0
        _MouthScale("Mouth Local Position Scale", Vector) = (0, 0, 0, 0)
        _MouthAOPower ("Mouth AO Gradient Hardness", Range(1, 100)) = 2
        _MouthUVTileIndex ("Mouth UVTile Index", Integer) = 0
        _MouthAOIntensity ("Mouth AO Intensity", Range(0, 1)) = 1
        _ClearCoatMask ("Clear Coat Mask", Range (0, 1)) = 1
        _ClearCoatSmoothness ("Clear Coat Smoothness", Range(0, 1)) = 1
        _HairColor1 ("Hair Color 1", Color) = (1, 1, 1, 1)
        _HairColor2 ("Hair Color 2", Color) = (1, 1, 1, 1)

        [HideInInspector] _EulerLightDirection("Light Dirrection (Euler)", Vector) = (90, 0, 0, 0)
        [HideInInspector] _EulerLightDirection2("Light Dirrection (Euler)2", Vector) = (90, 0, 0, 0)
        [HideInInspector] _DebugMipmapTex ("Debug mipmap texture", 2D) = "white" {}
        [HideInInspector] _DebugMipmapTexArray ("Debug mipmap texture array", 2DArray) = "" {}
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
            #pragma shader_feature_local_fragment _ _ANISOTROPIC_HIGHLIGHT
            #pragma shader_feature_local_fragment _ _ONE_TEXTURE
            #pragma shader_feature_local_fragment _ _EYES_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _EYES_MAKEUP_BURN _EYES_MAKEUP_DARKEN _EYES_MAKEUP_DIFF \
                                                    _EYES_MAKEUP_DODGE _EYES_MAKEUP_DIVIDE _EYES_MAKEUP_EXCLUSION \
                                                    _EYES_MAKEUP_HARDLIGHT _EYES_MAKEUP_HARDMIX _EYES_MAKEUP_LIGHTEN \
                                                    _EYES_MAKEUP_LINEARBURN _EYES_MAKEUP_LINEARDODGE _EYES_MAKEUP_LINEARLIGHT \
                                                    _EYES_MAKEUP_LINEARLIGHTADDSUB _EYES_MAKEUP_MULTIPLY _EYES_MAKEUP_NEGATION \
                                                    _EYES_MAKEUP_SCREEN _EYES_MAKEUP_OVERLAY _EYES_MAKEUP_PINLIGHT _EYES_MAKEUP_SOFTLIGHT \
                                                    _EYES_MAKEUP_VIVIDLIGHT _EYES_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUP_BURN _BLUSH_MAKEUP_DARKEN _BLUSH_MAKEUP_DIFF \
                                                    _BLUSH_MAKEUP_DODGE _BLUSH_MAKEUP_DIVIDE _BLUSH_MAKEUP_EXCLUSION \
                                                    _BLUSH_MAKEUP_HARDLIGHT _BLUSH_MAKEUP_HARDMIX _BLUSH_MAKEUP_LIGHTEN \
                                                    _BLUSH_MAKEUP_LINEARBURN _BLUSH_MAKEUP_LINEARDODGE _BLUSH_MAKEUP_LINEARLIGHT \
                                                    _BLUSH_MAKEUP_LINEARLIGHTADDSUB _BLUSH_MAKEUP_MULTIPLY _BLUSH_MAKEUP_NEGATION \
                                                    _BLUSH_MAKEUP_SCREEN _BLUSH_MAKEUP_OVERLAY _BLUSH_MAKEUP_PINLIGHT _BLUSH_MAKEUP_SOFTLIGHT \
                                                    _BLUSH_MAKEUP_VIVIDLIGHT _BLUSH_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUP_BURN _LIPS_MAKEUP_DARKEN _LIPS_MAKEUP_DIFF \
                                                    _LIPS_MAKEUP_DODGE _LIPS_MAKEUP_DIVIDE _LIPS_MAKEUP_EXCLUSION \
                                                    _LIPS_MAKEUP_HARDLIGHT _LIPS_MAKEUP_HARDMIX _LIPS_MAKEUP_LIGHTEN \
                                                    _LIPS_MAKEUP_LINEARBURN _LIPS_MAKEUP_LINEARDODGE _LIPS_MAKEUP_LINEARLIGHT \
                                                    _LIPS_MAKEUP_LINEARLIGHTADDSUB _LIPS_MAKEUP_MULTIPLY _LIPS_MAKEUP_NEGATION \
                                                    _LIPS_MAKEUP_SCREEN _LIPS_MAKEUP_OVERLAY _LIPS_MAKEUP_PINLIGHT _LIPS_MAKEUP_SOFTLIGHT \
                                                    _LIPS_MAKEUP_VIVIDLIGHT _LIPS_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOOMAP
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_BURN _FACIAL_TATOO_DARKEN _FACIAL_TATOO_DIFF \
                                                    _FACIAL_TATOO_DODGE _FACIAL_TATOO_DIVIDE _FACIAL_TATOO_EXCLUSION \
                                                    _FACIAL_TATOO_HARDLIGHT _FACIAL_TATOO_HARDMIX _FACIAL_TATOO_LIGHTEN \
                                                    _FACIAL_TATOO_LINEARBURN _FACIAL_TATOO_LINEARDODGE _FACIAL_TATOO_LINEARLIGHT \
                                                    _FACIAL_TATOO_LINEARLIGHTADDSUB _FACIAL_TATOO_MULTIPLY _FACIAL_TATOO_NEGATION \
                                                    _FACIAL_TATOO_SCREEN _FACIAL_TATOO_OVERLAY _FACIAL_TATOO_PINLIGHT _FACIAL_TATOO_SOFTLIGHT \
                                                    _FACIAL_TATOO_VIVIDLIGHT _FACIAL_TATOO_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_LEFT _FACIAL_TATOO_RIGT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKINGMAP
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_BURN _FACIAL_MARKING_DARKEN _FACIAL_MARKING_DIFF \
                                                    _FACIAL_MARKING_DODGE _FACIAL_MARKING_DIVIDE _FACIAL_MARKING_EXCLUSION \
                                                    _FACIAL_MARKING_HARDLIGHT _FACIAL_MARKING_HARDMIX _FACIAL_MARKING_LIGHTEN \
                                                    _FACIAL_MARKING_LINEARBURN _FACIAL_MARKING_LINEARDODGE _FACIAL_MARKING_LINEARLIGHT \
                                                    _FACIAL_MARKING_LINEARLIGHTADDSUB _FACIAL_MARKING_MULTIPLY _FACIAL_MARKING_NEGATION \
                                                    _FACIAL_MARKING_SCREEN _FACIAL_MARKING_OVERLAY _FACIAL_MARKING_PINLIGHT _FACIAL_MARKING_SOFTLIGHT \
                                                    _FACIAL_MARKING_VIVIDLIGHT _FACIAL_MARKING_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_LEFT _FACIAL_MARKING_RIGT
            #pragma shader_feature_local_fragment _ _CLEARCOAT
            #pragma shader_feature_local_fragment _ _HAIRCOLOR
            #pragma shader_feature_local _ _MOUTH_SHADOW
            #pragma shader_feature_local _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _MATCAPMAP
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma shader_feature_local _ _EYES_MAKEUP_UV1 _EYES_MAKEUP_UV2 _EYES_MAKEUP_UV3
            #pragma shader_feature_local _ _BLUSH_MAKEUP_UV1 _BLUSH_MAKEUP_UV2 _BLUSH_MAKEUP_UV3
            #pragma shader_feature_local _ _LIPS_MAKEUP_UV1 _LIPS_MAKEUP_UV2 _LIPS_MAKEUP_UV3
            #pragma shader_feature_local _ FACIAL_TATOO_UV1 FACIAL_TATOO_UV2 FACIAL_TATOO_UV3
            #pragma shader_feature_local _ FACIAL_MARKING_UV1 FACIAL_MARKING_UV2 FACIAL_MARKING_UV3
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
            #pragma shader_feature_local_fragment _ _ANISOTROPIC_HIGHLIGHT
            #pragma shader_feature_local_fragment _ _ONE_TEXTURE
            #pragma shader_feature_local_fragment _ _EYES_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _EYES_MAKEUP_BURN _EYES_MAKEUP_DARKEN _EYES_MAKEUP_DIFF \
                                                    _EYES_MAKEUP_DODGE _EYES_MAKEUP_DIVIDE _EYES_MAKEUP_EXCLUSION \
                                                    _EYES_MAKEUP_HARDLIGHT _EYES_MAKEUP_HARDMIX _EYES_MAKEUP_LIGHTEN \
                                                    _EYES_MAKEUP_LINEARBURN _EYES_MAKEUP_LINEARDODGE _EYES_MAKEUP_LINEARLIGHT \
                                                    _EYES_MAKEUP_LINEARLIGHTADDSUB _EYES_MAKEUP_MULTIPLY _EYES_MAKEUP_NEGATION \
                                                    _EYES_MAKEUP_SCREEN _EYES_MAKEUP_OVERLAY _EYES_MAKEUP_PINLIGHT _EYES_MAKEUP_SOFTLIGHT \
                                                    _EYES_MAKEUP_VIVIDLIGHT _EYES_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUP_BURN _BLUSH_MAKEUP_DARKEN _BLUSH_MAKEUP_DIFF \
                                                    _BLUSH_MAKEUP_DODGE _BLUSH_MAKEUP_DIVIDE _BLUSH_MAKEUP_EXCLUSION \
                                                    _BLUSH_MAKEUP_HARDLIGHT _BLUSH_MAKEUP_HARDMIX _BLUSH_MAKEUP_LIGHTEN \
                                                    _BLUSH_MAKEUP_LINEARBURN _BLUSH_MAKEUP_LINEARDODGE _BLUSH_MAKEUP_LINEARLIGHT \
                                                    _BLUSH_MAKEUP_LINEARLIGHTADDSUB _BLUSH_MAKEUP_MULTIPLY _BLUSH_MAKEUP_NEGATION \
                                                    _BLUSH_MAKEUP_SCREEN _BLUSH_MAKEUP_OVERLAY _BLUSH_MAKEUP_PINLIGHT _BLUSH_MAKEUP_SOFTLIGHT \
                                                    _BLUSH_MAKEUP_VIVIDLIGHT _BLUSH_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUP_BURN _LIPS_MAKEUP_DARKEN _LIPS_MAKEUP_DIFF \
                                                    _LIPS_MAKEUP_DODGE _LIPS_MAKEUP_DIVIDE _LIPS_MAKEUP_EXCLUSION \
                                                    _LIPS_MAKEUP_HARDLIGHT _LIPS_MAKEUP_HARDMIX _LIPS_MAKEUP_LIGHTEN \
                                                    _LIPS_MAKEUP_LINEARBURN _LIPS_MAKEUP_LINEARDODGE _LIPS_MAKEUP_LINEARLIGHT \
                                                    _LIPS_MAKEUP_LINEARLIGHTADDSUB _LIPS_MAKEUP_MULTIPLY _LIPS_MAKEUP_NEGATION \
                                                    _LIPS_MAKEUP_SCREEN _LIPS_MAKEUP_OVERLAY _LIPS_MAKEUP_PINLIGHT _LIPS_MAKEUP_SOFTLIGHT \
                                                    _LIPS_MAKEUP_VIVIDLIGHT _LIPS_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOOMAP
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_BURN _FACIAL_TATOO_DARKEN _FACIAL_TATOO_DIFF \
                                                    _FACIAL_TATOO_DODGE _FACIAL_TATOO_DIVIDE _FACIAL_TATOO_EXCLUSION \
                                                    _FACIAL_TATOO_HARDLIGHT _FACIAL_TATOO_HARDMIX _FACIAL_TATOO_LIGHTEN \
                                                    _FACIAL_TATOO_LINEARBURN _FACIAL_TATOO_LINEARDODGE _FACIAL_TATOO_LINEARLIGHT \
                                                    _FACIAL_TATOO_LINEARLIGHTADDSUB _FACIAL_TATOO_MULTIPLY _FACIAL_TATOO_NEGATION \
                                                    _FACIAL_TATOO_SCREEN _FACIAL_TATOO_OVERLAY _FACIAL_TATOO_PINLIGHT _FACIAL_TATOO_SOFTLIGHT \
                                                    _FACIAL_TATOO_VIVIDLIGHT _FACIAL_TATOO_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_LEFT _FACIAL_TATOO_RIGT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKINGMAP
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_BURN _FACIAL_MARKING_DARKEN _FACIAL_MARKING_DIFF \
                                                    _FACIAL_MARKING_DODGE _FACIAL_MARKING_DIVIDE _FACIAL_MARKING_EXCLUSION \
                                                    _FACIAL_MARKING_HARDLIGHT _FACIAL_MARKING_HARDMIX _FACIAL_MARKING_LIGHTEN \
                                                    _FACIAL_MARKING_LINEARBURN _FACIAL_MARKING_LINEARDODGE _FACIAL_MARKING_LINEARLIGHT \
                                                    _FACIAL_MARKING_LINEARLIGHTADDSUB _FACIAL_MARKING_MULTIPLY _FACIAL_MARKING_NEGATION \
                                                    _FACIAL_MARKING_SCREEN _FACIAL_MARKING_OVERLAY _FACIAL_MARKING_PINLIGHT _FACIAL_MARKING_SOFTLIGHT \
                                                    _FACIAL_MARKING_VIVIDLIGHT _FACIAL_MARKING_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_LEFT _FACIAL_MARKING_RIGT
            #pragma shader_feature_local_fragment _ _CLEARCOAT
            #pragma shader_feature_local_fragment _ _HAIRCOLOR
            #pragma shader_feature_local _ _MOUTH_SHADOW
            #pragma shader_feature_local _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _MATCAPMAP
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma shader_feature_local _ _EYES_MAKEUP_UV1 _EYES_MAKEUP_UV2 _EYES_MAKEUP_UV3
            #pragma shader_feature_local _ _BLUSH_MAKEUP_UV1 _BLUSH_MAKEUP_UV2 _BLUSH_MAKEUP_UV3
            #pragma shader_feature_local _ _LIPS_MAKEUP_UV1 _LIPS_MAKEUP_UV2 _LIPS_MAKEUP_UV3
            #pragma shader_feature_local _ FACIAL_TATOO_UV1 FACIAL_TATOO_UV2 FACIAL_TATOO_UV3
            #pragma shader_feature_local _ FACIAL_MARKING_UV1 FACIAL_MARKING_UV2 FACIAL_MARKING_UV3
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
            #pragma shader_feature_local_fragment _ _ANISOTROPIC_HIGHLIGHT
            #pragma shader_feature_local_fragment _ _ONE_TEXTURE
            #pragma shader_feature_local_fragment _ _EYES_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _EYES_MAKEUP_BURN _EYES_MAKEUP_DARKEN _EYES_MAKEUP_DIFF \
                                                    _EYES_MAKEUP_DODGE _EYES_MAKEUP_DIVIDE _EYES_MAKEUP_EXCLUSION \
                                                    _EYES_MAKEUP_HARDLIGHT _EYES_MAKEUP_HARDMIX _EYES_MAKEUP_LIGHTEN \
                                                    _EYES_MAKEUP_LINEARBURN _EYES_MAKEUP_LINEARDODGE _EYES_MAKEUP_LINEARLIGHT \
                                                    _EYES_MAKEUP_LINEARLIGHTADDSUB _EYES_MAKEUP_MULTIPLY _EYES_MAKEUP_NEGATION \
                                                    _EYES_MAKEUP_SCREEN _EYES_MAKEUP_OVERLAY _EYES_MAKEUP_PINLIGHT _EYES_MAKEUP_SOFTLIGHT \
                                                    _EYES_MAKEUP_VIVIDLIGHT _EYES_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _BLUSH_MAKEUP_BURN _BLUSH_MAKEUP_DARKEN _BLUSH_MAKEUP_DIFF \
                                                    _BLUSH_MAKEUP_DODGE _BLUSH_MAKEUP_DIVIDE _BLUSH_MAKEUP_EXCLUSION \
                                                    _BLUSH_MAKEUP_HARDLIGHT _BLUSH_MAKEUP_HARDMIX _BLUSH_MAKEUP_LIGHTEN \
                                                    _BLUSH_MAKEUP_LINEARBURN _BLUSH_MAKEUP_LINEARDODGE _BLUSH_MAKEUP_LINEARLIGHT \
                                                    _BLUSH_MAKEUP_LINEARLIGHTADDSUB _BLUSH_MAKEUP_MULTIPLY _BLUSH_MAKEUP_NEGATION \
                                                    _BLUSH_MAKEUP_SCREEN _BLUSH_MAKEUP_OVERLAY _BLUSH_MAKEUP_PINLIGHT _BLUSH_MAKEUP_SOFTLIGHT \
                                                    _BLUSH_MAKEUP_VIVIDLIGHT _BLUSH_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUPMAP
            #pragma shader_feature_local_fragment _ _LIPS_MAKEUP_BURN _LIPS_MAKEUP_DARKEN _LIPS_MAKEUP_DIFF \
                                                    _LIPS_MAKEUP_DODGE _LIPS_MAKEUP_DIVIDE _LIPS_MAKEUP_EXCLUSION \
                                                    _LIPS_MAKEUP_HARDLIGHT _LIPS_MAKEUP_HARDMIX _LIPS_MAKEUP_LIGHTEN \
                                                    _LIPS_MAKEUP_LINEARBURN _LIPS_MAKEUP_LINEARDODGE _LIPS_MAKEUP_LINEARLIGHT \
                                                    _LIPS_MAKEUP_LINEARLIGHTADDSUB _LIPS_MAKEUP_MULTIPLY _LIPS_MAKEUP_NEGATION \
                                                    _LIPS_MAKEUP_SCREEN _LIPS_MAKEUP_OVERLAY _LIPS_MAKEUP_PINLIGHT _LIPS_MAKEUP_SOFTLIGHT \
                                                    _LIPS_MAKEUP_VIVIDLIGHT _LIPS_MAKEUP_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOOMAP
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_BURN _FACIAL_TATOO_DARKEN _FACIAL_TATOO_DIFF \
                                                    _FACIAL_TATOO_DODGE _FACIAL_TATOO_DIVIDE _FACIAL_TATOO_EXCLUSION \
                                                    _FACIAL_TATOO_HARDLIGHT _FACIAL_TATOO_HARDMIX _FACIAL_TATOO_LIGHTEN \
                                                    _FACIAL_TATOO_LINEARBURN _FACIAL_TATOO_LINEARDODGE _FACIAL_TATOO_LINEARLIGHT \
                                                    _FACIAL_TATOO_LINEARLIGHTADDSUB _FACIAL_TATOO_MULTIPLY _FACIAL_TATOO_NEGATION \
                                                    _FACIAL_TATOO_SCREEN _FACIAL_TATOO_OVERLAY _FACIAL_TATOO_PINLIGHT _FACIAL_TATOO_SOFTLIGHT \
                                                    _FACIAL_TATOO_VIVIDLIGHT _FACIAL_TATOO_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_TATOO_LEFT _FACIAL_TATOO_RIGT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKINGMAP
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_BURN _FACIAL_MARKING_DARKEN _FACIAL_MARKING_DIFF \
                                                    _FACIAL_MARKING_DODGE _FACIAL_MARKING_DIVIDE _FACIAL_MARKING_EXCLUSION \
                                                    _FACIAL_MARKING_HARDLIGHT _FACIAL_MARKING_HARDMIX _FACIAL_MARKING_LIGHTEN \
                                                    _FACIAL_MARKING_LINEARBURN _FACIAL_MARKING_LINEARDODGE _FACIAL_MARKING_LINEARLIGHT \
                                                    _FACIAL_MARKING_LINEARLIGHTADDSUB _FACIAL_MARKING_MULTIPLY _FACIAL_MARKING_NEGATION \
                                                    _FACIAL_MARKING_SCREEN _FACIAL_MARKING_OVERLAY _FACIAL_MARKING_PINLIGHT _FACIAL_MARKING_SOFTLIGHT \
                                                    _FACIAL_MARKING_VIVIDLIGHT _FACIAL_MARKING_SUBTRACT
            #pragma shader_feature_local_fragment _ _FACIAL_MARKING_LEFT _FACIAL_MARKING_RIGT
            #pragma shader_feature_local_fragment _ _CLEARCOAT
            #pragma shader_feature_local_fragment _ _HAIRCOLOR
            #pragma shader_feature_local _ _MOUTH_SHADOW
            #pragma shader_feature_local _ _OBJECT_SPACE_GRADIENT
            #pragma shader_feature_local _ _MATCAPMAP
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP
            #pragma shader_feature_local _ _EYES_MAKEUP_UV1 _EYES_MAKEUP_UV2 _EYES_MAKEUP_UV3
            #pragma shader_feature_local _ _BLUSH_MAKEUP_UV1 _BLUSH_MAKEUP_UV2 _BLUSH_MAKEUP_UV3
            #pragma shader_feature_local _ _LIPS_MAKEUP_UV1 _LIPS_MAKEUP_UV2 _LIPS_MAKEUP_UV3
            #pragma shader_feature_local _ FACIAL_TATOO_UV1 FACIAL_TATOO_UV2 FACIAL_TATOO_UV3
            #pragma shader_feature_local _ FACIAL_MARKING_UV1 FACIAL_MARKING_UV2 FACIAL_MARKING_UV3
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