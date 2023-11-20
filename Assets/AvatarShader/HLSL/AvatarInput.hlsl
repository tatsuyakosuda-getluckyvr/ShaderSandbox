#ifndef UNIVERSAL_AVATAR_INPUT_INCLUDED
#define UNIVERSAL_AVATAR_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Lighting.hlsl"
#include "GPUSkinning.hlsl"

CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseMapArray_ST;
half4 _DebugMipmapTexArray_ST;
half4 _DebugMipmapTex_ST;
half4 _BaseColor;
half4 _DiffuseColor;
half4 _DiffuseColor2;
half4 _LightDirection;
half4 _LightDirection2;
half4 _RimColor;
half4 _RimColor2;
half4 _GradientScale;
half _GradientAngle;
half _RimPower;
half _RimPower2;
half _Smoothness;
half _Metallic;
half _NormalStrength;
half _Occlusion;
half _Cutoff;
half _BaseMapIndex;
half _NormalMapIndex;
half _MARMapIndex;
half _GradientPower;
half _GradientOffset;
half _DiffuseIntensity;
half _DiffuseIntensity2;
half _RimIntensity;
half _RimIntensity2;
half _UVTilingX;
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
#ifdef _TEX_ARRAY
#if _UVTILING
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex * _UVTilingX + floor(uv.x)) * _BaseColor;
#else
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex) * _BaseColor;
#endif
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
#ifdef _NORMALMAP
#ifdef _TEX_ARRAY
#if _UVTILING
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, uv, _NormalMapIndex * _UVTilingX + floor(uv.x)), _NormalStrength);
#else
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, uv, _NormalMapIndex), _NormalStrength);
#endif
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
#ifdef _MARMAP
#ifdef _TEX_ARRAY
#if _UVTILING
    half3 res = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, uv, _MARMapIndex * _UVTilingX + floor(uv.x)).rgb;
#else
    half3 res = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, uv, _MARMapIndex).rgb;
#endif
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
    light.diffuseColor2 = _DiffuseColor2.rgb;
    light.lightDirection = _LightDirection.rgb;
    light.lightDirection2 = _LightDirection2.rgb;
    light.rimLightColor = _RimColor.rgb;
    light.rimLightColor2 = _RimColor2.rgb;
    light.rimLightPower = _RimPower;
    light.rimLightPower2 = _RimPower2;
    light.gradientScale = _GradientScale;
    light.gradientAngle = _GradientAngle;
    light.gradientPower = _GradientPower;
    light.gradientOffset = _GradientOffset;
    light.diffuseIntensity = _DiffuseIntensity;
    light.diffuseIntensity2 = _DiffuseIntensity2;
    light.rimIntensity = _RimIntensity;
    light.rimIntensity2 = _RimIntensity2;
    return light;
}

half4 CalculateFinalColor(InputData inputData, SurfaceData surfaceData)
{
#ifdef _UNLIT
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
#ifdef _TEX_ARRAY
    uv = (uv - _BaseMapArray_ST.zw) / _BaseMapArray_ST.xy;
    col *= SAMPLE_TEXTURE2D_ARRAY(_DebugMipmapTexArray, sampler_DebugMipmapTexArray, TRANSFORM_TEX(uv, _DebugMipmapTexArray), 0);
#else
    uv = (uv - _BaseMap_ST.zw) / _BaseMap_ST.xy;
    col *= SAMPLE_TEXTURE2D(_DebugMipmapTex, sampler_DebugMipmapTex, TRANSFORM_TEX(uv, _DebugMipmapTex));
#endif
    return col;
}

#endif