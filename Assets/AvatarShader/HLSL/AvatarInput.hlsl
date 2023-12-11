#ifndef UNIVERSAL_AVATAR_INPUT_INCLUDED
#define UNIVERSAL_AVATAR_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Lighting.hlsl"
#include "GPUSkinning.hlsl"
#include "ColorBlending.hlsl"

CBUFFER_START(UnityPerMaterial)
half4 _BaseMap_ST;
half4 _BaseMapArray_ST;
half4 _DebugMipmapTexArray_ST;
half4 _DebugMipmapTex_ST;
half4 _EyesMakeupMapArray_ST;
half4 _BlushMakeupMapArray_ST;
half4 _LipsMakeupMapArray_ST;
half4 _SkinColor;
half4 _DiffuseColor;
half4 _DiffuseColor2;
half4 _LightDirection;
half4 _LightDirection2;
half4 _RimColor;
half4 _RimColor2;
half4 _GradientScale;
half4 _DeepLayerColor;
half4 _MatCapHSVShift;
half4 _AnisoHighlightColor;
half4 _MouthScale;
half4 _HairColor1;
half4 _HairColor2;
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
half _UVTiles;
half _MatCapBlend;
half _UVCutoff;
half _AnisoOffset;
half _AnisoPower;
half _AnisoIntensity;
half _HairUVTileIndex;
half _EyesMakeupMapIndex;
half _EyesMakeupUVTileInedx;
half _BlushMakeupMapIndex;
half _BlushMakeupUVTileInedx;
half _LipsMakeupMapIndex;
half _LipsMakeupUVTileInedx;
half _FacialTatooMapIndex;
half _FacialTatooUVTileInedx;
half _FacialMarkingMapIndex;
half _FacialMarkingUVTileInedx;
half _MouthAOPower;
half _MouthUVTileIndex;
half _MouthAOIntensity;
half _ClearCoatMask;
half _ClearCoatSmoothness;
CBUFFER_END

TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
TEXTURE2D_ARRAY(_BaseMapArray); SAMPLER(sampler_BaseMapArray);
TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
TEXTURE2D_ARRAY(_NormalMapArray); SAMPLER(sampler_NormalMapArray);
TEXTURE2D(_DebugMipmapTex); SAMPLER(sampler_DebugMipmapTex);
TEXTURE2D_ARRAY(_DebugMipmapTexArray); SAMPLER(sampler_DebugMipmapTexArray);
TEXTURE2D(_MARMap); SAMPLER(sampler_MARMap);
TEXTURE2D_ARRAY(_MARMapArray); SAMPLER(sampler_MARMapArray);
TEXTURE2D(_MatCapMap); SAMPLER(sampler_MatCapMap);
TEXTURE2D_ARRAY(_EyesMakeupMapArray); SAMPLER(sampler_EyesMakeupMapArray);
TEXTURE2D_ARRAY(_BlushMakeupMapArray); SAMPLER(sampler_BlushMakeupMapArray);
TEXTURE2D_ARRAY(_LipsMakeupMapArray); SAMPLER(sampler_LipsMakeupMapArray);
TEXTURE2D_ARRAY(_FacialTatooMapArray); SAMPLER(sampler_FacialTatooMapArray);
TEXTURE2D_ARRAY(_FacialMarkingMapArray); SAMPLER(sampler_FacialMarkingMapArray);

half GetUVTileIndex(half2 uv)
{
    return floor(uv.x) + floor(uv.y) * 5;
}

half4 SampleAlbedo(half2 uv)
{
#ifdef _TEX_ARRAY
#if _UVTILING
#if _ONE_TEXTURE
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex);
#else
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex * _UVTiles + GetUVTileIndex(uv));
#endif
#else
    half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv, _BaseMapIndex);
#endif
#else
    half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
#endif
    return col;
}

half3 SampleSkinColor(half4 color)
{
    return lerp(color.rgb, color.rgb * _SkinColor.rgb, color.a);
}

inline void AlphaDiscard(half alpha)
{
    clip(alpha - _Cutoff);
}

inline void UVDiscard(half2 uv)
{
#if _UVTILING
    clip(GetUVTileIndex(uv) - _UVCutoff);
#endif
}

half3 SampleNormal(half2 uv)
{
#ifdef _NORMALMAP
#ifdef _TEX_ARRAY
#if _UVTILING
#if _ONE_TEXTURE
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, uv, _NormalMapIndex), _NormalStrength);
#else
    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, uv, _NormalMapIndex * _UVTiles + GetUVTileIndex(uv)), _NormalStrength);
#endif
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
#if _ONE_TEXTURE
    half3 res = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, uv, _MARMapIndex).rgb;
#else
    half3 res = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, uv, _MARMapIndex * _UVTiles + GetUVTileIndex(uv)).rgb;
#endif
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

void SampleMatCap(half2 texcoord, inout half4 col)
{
#if _MATCAPMAP
    half3 matCap = SAMPLE_TEXTURE2D(_MatCapMap, sampler_MatCapMap, texcoord).rgb;
    matCap = HsvToRgb(RgbToHsv(matCap) + _MatCapHSVShift.rgb);
    float luminance = Luminance(col.rgb) * 2;
    col.rgb = lerp(col.rgb, lerp(lerp(_DeepLayerColor.rgb, matCap, saturate(luminance)), lerp(matCap, _DeepLayerColor.rgb, saturate(luminance - 1)), step(1, luminance)), _MatCapBlend * col.a);
#endif
}

void SampleEyesMakeupMap(half2 uv, inout half4 col)
{
#if _EYES_MAKEUPMAP
    half4 detail = SAMPLE_TEXTURE2D_ARRAY(_EyesMakeupMapArray, sampler_EyesMakeupMapArray, uv, _EyesMakeupMapIndex);
    half alpha = detail.a * col.a * abs(step(1, abs(GetUVTileIndex(uv) - _EyesMakeupUVTileInedx)) - 1);

#ifdef _EYES_MAKEUP_BURN
    col.rgb = BlendBurn(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_DARKEN
    col.rgb = BlendDarken(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_DIFF
    col.rgb = BlendDifference(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_DODGE
    col.rgb = BlendDodge(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_DIVIDE
    col.rgb = BlendDivide(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_EXCLUSION
    col.rgb = BlendExclusion(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_HARDLIGHT
    col.rgb = BlendHardLight(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_HARDMIX
    col.rgb = BlendHardMix(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_LIGHTEN
    col.rgb = BlendLighten(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_LINEARBURN
    col.rgb = BlendLinearBurn(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_LINEARDODGE
    col.rgb = BlendLinearDodge(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_LINEARLIGHT
    col.rgb = BlendLinearLight(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_LINEARLIGHTADDSUB
    col.rgb = BlendLinearLightAddSub(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_MULTIPLY
    col.rgb = BlendMultiply(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_NEGATION
    col.rgb = BlendNegation(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_SCREEN
    col.rgb = BlendScreen(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_OVERLAY
    col.rgb = BlendOverlay(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_PINLIGHT
    col.rgb = BlendPinLight(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_SOFTLIGHT
    col.rgb = BlendSoftLight(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_VIVIDLIGHT
    col.rgb = BlendVividLight(col.rgb, detail.rgb, alpha);
#elif _EYES_MAKEUP_SUBTRACT
    col.rgb = BlendSubtract(col.rgb, detail.rgb, alpha);
#else
    col.rgb = BlendOverwrite(col.rgb, detail.rgb, alpha);
#endif
#endif
}

void SampleBlushMakeupMap(half2 uv, inout half4 col)
{
#if _BLUSH_MAKEUPMAP
    half4 detail = SAMPLE_TEXTURE2D_ARRAY(_BlushMakeupMapArray, sampler_BlushMakeupMapArray, uv, _BlushMakeupMapIndex);
    half alpha = detail.a * col.a * abs(step(1, abs(GetUVTileIndex(uv) - _BlushMakeupUVTileInedx)) - 1);

#ifdef _BLUSH_MAKEUP_BURN
    col.rgb = BlendBurn(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_DARKEN
    col.rgb = BlendDarken(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_DIFF
    col.rgb = BlendDifference(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_DODGE
    col.rgb = BlendDodge(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_DIVIDE
    col.rgb = BlendDivide(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_EXCLUSION
    col.rgb = BlendExclusion(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_HARDLIGHT
    col.rgb = BlendHardLight(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_HARDMIX
    col.rgb = BlendHardMix(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_LIGHTEN
    col.rgb = BlendLighten(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_LINEARBURN
    col.rgb = BlendLinearBurn(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_LINEARDODGE
    col.rgb = BlendLinearDodge(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_LINEARLIGHT
    col.rgb = BlendLinearLight(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_LINEARLIGHTADDSUB
    col.rgb = BlendLinearLightAddSub(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_MULTIPLY
    col.rgb = BlendMultiply(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_NEGATION
    col.rgb = BlendNegation(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_SCREEN
    col.rgb = BlendScreen(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_OVERLAY
    col.rgb = BlendOverlay(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_PINLIGHT
    col.rgb = BlendPinLight(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_SOFTLIGHT
    col.rgb = BlendSoftLight(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_VIVIDLIGHT
    col.rgb = BlendVividLight(col.rgb, detail.rgb, alpha);
#elif _BLUSH_MAKEUP_SUBTRACT
    col.rgb = BlendSubtract(col.rgb, detail.rgb, alpha);
#else
    col.rgb = BlendOverwrite(col.rgb, detail.rgb, alpha);
#endif
#endif
}

void SampleFacialTatooMap(half2 uv, inout half4 col)
{
#if _FACIAL_TATOOMAP
    half4 detail = SAMPLE_TEXTURE2D_ARRAY(_FacialTatooMapArray, sampler_FacialTatooMapArray, uv, _FacialTatooMapIndex);
    half alpha = detail.a * col.a * abs(step(1, abs(GetUVTileIndex(uv) - _FacialTatooUVTileInedx)) - 1);

#ifdef _FACIAL_TATOO_BURN
    col.rgb = BlendBurn(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_DARKEN
    col.rgb = BlendDarken(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_DIFF
    col.rgb = BlendDifference(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_DODGE
    col.rgb = BlendDodge(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_DIVIDE
    col.rgb = BlendDivide(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_EXCLUSION
    col.rgb = BlendExclusion(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_HARDLIGHT
    col.rgb = BlendHardLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_HARDMIX
    col.rgb = BlendHardMix(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_LIGHTEN
    col.rgb = BlendLighten(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_LINEARBURN
    col.rgb = BlendLinearBurn(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_LINEARDODGE
    col.rgb = BlendLinearDodge(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_LINEARLIGHT
    col.rgb = BlendLinearLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_LINEARLIGHTADDSUB
    col.rgb = BlendLinearLightAddSub(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_MULTIPLY
    col.rgb = BlendMultiply(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_NEGATION
    col.rgb = BlendNegation(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_SCREEN
    col.rgb = BlendScreen(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_OVERLAY
    col.rgb = BlendOverlay(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_PINLIGHT
    col.rgb = BlendPinLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_SOFTLIGHT
    col.rgb = BlendSoftLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_VIVIDLIGHT
    col.rgb = BlendVividLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_TATOO_SUBTRACT
    col.rgb = BlendSubtract(col.rgb, detail.rgb, alpha);
#else
    col.rgb = BlendOverwrite(col.rgb, detail.rgb, alpha);
#endif
#endif
}

void SampleFacialMarkingMap(half2 uv, inout half4 col)
{
#if _FACIAL_MARKINGMAP
    half4 detail = SAMPLE_TEXTURE2D_ARRAY(_FacialMarkingMapArray, sampler_FacialMarkingMapArray, uv, _FacialMarkingMapIndex);
    half alpha = detail.a * col.a * abs(step(1, abs(GetUVTileIndex(uv) - _FacialMarkingUVTileInedx)) - 1);

#ifdef _FACIAL_MARKING_BURN
    col.rgb = BlendBurn(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_DARKEN
    col.rgb = BlendDarken(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_DIFF
    col.rgb = BlendDifference(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_DODGE
    col.rgb = BlendDodge(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_DIVIDE
    col.rgb = BlendDivide(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_EXCLUSION
    col.rgb = BlendExclusion(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_HARDLIGHT
    col.rgb = BlendHardLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_HARDMIX
    col.rgb = BlendHardMix(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_LIGHTEN
    col.rgb = BlendLighten(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_LINEARBURN
    col.rgb = BlendLinearBurn(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_LINEARDODGE
    col.rgb = BlendLinearDodge(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_LINEARLIGHT
    col.rgb = BlendLinearLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_LINEARLIGHTADDSUB
    col.rgb = BlendLinearLightAddSub(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_MULTIPLY
    col.rgb = BlendMultiply(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_NEGATION
    col.rgb = BlendNegation(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_SCREEN
    col.rgb = BlendScreen(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_OVERLAY
    col.rgb = BlendOverlay(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_PINLIGHT
    col.rgb = BlendPinLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_SOFTLIGHT
    col.rgb = BlendSoftLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_VIVIDLIGHT
    col.rgb = BlendVividLight(col.rgb, detail.rgb, alpha);
#elif _FACIAL_MARKING_SUBTRACT
    col.rgb = BlendSubtract(col.rgb, detail.rgb, alpha);
#else
    col.rgb = BlendOverwrite(col.rgb, detail.rgb, alpha);
#endif
#endif
}

void SampleLipsMakeupMap(half2 uv, inout half4 col)
{
#if _LIPS_MAKEUPMAP
    half4 detail = SAMPLE_TEXTURE2D_ARRAY(_LipsMakeupMapArray, sampler_LipsMakeupMapArray, uv, _LipsMakeupMapIndex);
    half alpha = detail.a * col.a * abs(step(1, abs(GetUVTileIndex(uv) - _LipsMakeupUVTileInedx)) - 1);

#ifdef _LIPS_MAKEUP_BURN
    col.rgb = BlendBurn(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_DARKEN
    col.rgb = BlendDarken(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_DIFF
    col.rgb = BlendDifference(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_DODGE
    col.rgb = BlendDodge(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_DIVIDE
    col.rgb = BlendDivide(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_EXCLUSION
    col.rgb = BlendExclusion(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_HARDLIGHT
    col.rgb = BlendHardLight(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_HARDMIX
    col.rgb = BlendHardMix(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_LIGHTEN
    col.rgb = BlendLighten(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_LINEARBURN
    col.rgb = BlendLinearBurn(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_LINEARDODGE
    col.rgb = BlendLinearDodge(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_LINEARLIGHT
    col.rgb = BlendLinearLight(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_LINEARLIGHTADDSUB
    col.rgb = BlendLinearLightAddSub(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_MULTIPLY
    col.rgb = BlendMultiply(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_NEGATION
    col.rgb = BlendNegation(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_SCREEN
    col.rgb = BlendScreen(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_OVERLAY
    col.rgb = BlendOverlay(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_PINLIGHT
    col.rgb = BlendPinLight(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_SOFTLIGHT
    col.rgb = BlendSoftLight(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_VIVIDLIGHT
    col.rgb = BlendVividLight(col.rgb, detail.rgb, alpha);
#elif _LIPS_MAKEUP_SUBTRACT
    col.rgb = BlendSubtract(col.rgb, detail.rgb, alpha);
#else
    col.rgb = BlendOverwrite(col.rgb, detail.rgb, alpha);
#endif
#endif
}

void SampleClearCoat(inout half mask, inout half smoothness)
{
    half2 res = half2(_ClearCoatMask, _ClearCoatSmoothness);
    mask = res.r;
    smoothness = res.g;
}

void SampleHairColor(half2 uv, half interpolator, inout half3 col)
{
#ifdef _HAIRCOLOR
#ifdef _UVTILING
    col = lerp(lerp(_HairColor1.rgb, _HairColor2.rgb, interpolator), col, step(1, abs(GetUVTileIndex(uv) - _HairUVTileIndex)));
#else
    col = lerp(_HairColor1.rgb, _HairColor2.rgb, interpolator);
#endif
#endif
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
    light.anisoOffset = _AnisoOffset;
    light.anisoPower = _AnisoPower;
    light.anisoIntensity = _AnisoIntensity;
    light.hairUVTileIndex = _HairUVTileIndex;
    light.anisoHighlightColor = _AnisoHighlightColor.rgb;
    light.mouthScale = _MouthScale;
    light.mouthAOPower = _MouthAOPower;
    light.mouthUVTileIndex = _MouthUVTileIndex;
    light.mouthAOIntensity = _MouthAOIntensity;
    return light;
}

half4 CalculateFinalColor(InputData inputData, SurfaceData surfaceData, half3 positionOS, half tileIndex)
{
#ifdef _UNLIT
    half4 col = UniversalFragmentUnlit(inputData, surfaceData);
#elif _SIMPLELIT
    half4 col = UniversalFragmentBlinnPhong(inputData, surfaceData);
#elif _CUSTOMLIT
    half4 col = UniversalFragmentCustomLighting(inputData, surfaceData, GetCustomLight(), positionOS, tileIndex);
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