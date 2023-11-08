#ifndef UNIVERSAL_AVATAR_INPUT_INCLUDED
#define UNIVERSAL_AVATAR_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Lighting.hlsl"

CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseMapArray_ST;
half4 _DebugMipmapTexArray_ST;
half4 _DebugMipmapTex_ST;
half4 _BaseColor;
half4 _DiffuseColor;
half4 _LightDirection;
half4 _RimColor;
half _RimPower;
half _Smoothness;
half _Metallic;
half _NormalStrength;
half _Occlusion;
half _Cutoff;
half _BaseMapIndex;
half _NormalMapIndex;
half _MARMapIndex;
CBUFFER_END

TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
TEXTURE2D_ARRAY(_BaseMapArray); SAMPLER(sampler_BaseMapArray);
TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
TEXTURE2D_ARRAY(_NormalMapArray); SAMPLER(sampler_NormalMapArray);
TEXTURE2D(_DebugMipmapTex); SAMPLER(sampler_DebugMipmapTex);
TEXTURE2D_ARRAY(_DebugMipmapTexArray); SAMPLER(sampler_DebugMipmapTexArray);

TEXTURE2D(_MARMap); SAMPLER(sampler_MARMap);
TEXTURE2D_ARRAY(_MARMapArray); SAMPLER(sampler_MARMapArray);

half4 SampleAlbedo(half2 uv)
{
#if _TEX_ARRAY
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex) * _BaseColor;
#else
    half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
#endif
    return col;
}

inline void AlphaDiscard(half alpha)
{
    clip(alpha - _Cutoff);
}

half3 SampleNormal(half2 uv)
{
#if _NORMALMAP
#if _TEX_ARRAY
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, uv, _NormalMapIndex), _NormalStrength);
#else
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv), _NormalStrength);
#endif
#else
    half3 normalTS = half3(0, 0, 1);
#endif
    return normalTS;
}

half3 SampleMAR(half2 uv)
{
#if _MARMAP
#if _TEX_ARRAY
    half3 res = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, uv, _MARMapIndex).rgb;
#else
    half3 res = SAMPLE_TEXTURE2D(_MARMap, sampler_MARMap, uv).rgb;
#endif
#else
    half3 res = half3(1, 1, 0);
#endif
    res.r *= _Metallic;
    res.g *= _Occlusion;
    res.b = (1 - res.b) * _Smoothness;
    return res;
}

CustomLight GetCustomLight()
{
    CustomLight light = (CustomLight)0;
    light.diffuseColor = _DiffuseColor.rgb;
    light.lightDirection = _LightDirection.rgb;
    light.rimLightColor = _RimColor.rgb;
    light.rimLightPower = _RimPower;
    return light;
}

half4 CalculateFinalColor(InputData inputData, SurfaceData surfaceData)
{
#if _UNLIT
    half4 col = UniversalFragmentUnlit(inputData, surfaceData);
#elif _SIMPLELIT
    half4 col = UniversalFragmentBlinnPhong(inputData, surfaceData);
#elif _CUSTOMLIT
    half4 col = UniversalFragmentCustomLighting(inputData, surfaceData, GetCustomLight());
#else
    half4 col = UniversalFragmentPBR(inputData, surfaceData);
#endif
    return col;
}

half4 DebugMipmap(float2 uv, half4 col)
{
#if _TEX_ARRAY
    uv = (uv - _BaseMapArray_ST.zw) / _BaseMapArray_ST.xy;
    col *= SAMPLE_TEXTURE2D_ARRAY(_DebugMipmapTexArray, sampler_DebugMipmapTexArray, TRANSFORM_TEX(uv, _DebugMipmapTexArray), 0);
#else
    uv = (uv - _BaseMap_ST.zw) / _BaseMap_ST.xy;
    col *= SAMPLE_TEXTURE2D(_DebugMipmapTex, sampler_DebugMipmapTex, TRANSFORM_TEX(uv, _DebugMipmapTex));
#endif
    return col;
}

#endif