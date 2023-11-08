#ifndef UNIVERSAL_CUSTOM_LIGHTING_INCLUDED
#define UNIVERSAL_CUSTOM_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct CustomLight
{
    half3 diffuseColor;
    half3 lightDirection;
    half3 rimLightColor;
    half rimLightPower;
};

// https://www.jordanstevenstechart.com/lighting-models
half3 LightingHalfLambert(half3 lightColor, half3 lightDir, half3 normal)
{
    half nl = saturate(dot(normal, lightDir));
    half halfLambertDiffusion = pow(nl * 0.5 + 0.5, 2);
    return lightColor * halfLambertDiffusion;
}

// https://www.jordanstevenstechart.com/lighting-models
half3 LightingMinnaert(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half roughness)
{
    half nl = saturate(dot(normal, lightDir));
    half nv = saturate(dot(normal, viewDir));
    half3 minnart = saturate(nl * pow(nl * nv, max(roughness, 0.0001)));
    return lightColor * minnart;
}

half3 CalculateDiffusion(half3 color, half3 direction, half3 normalWS, half3 viewDirectionWS, half perceptualRoughness)
{
#if _DIFFUSE_COLOR
#if _HALF_LAMBERT
    half3 diffuseColor = LightingHalfLambert(color, direction, normalWS);
#elif _MINNAERT
    half3 diffuseColor = LightingMinnaert(color, direction, normalWS, viewDirectionWS, perceptualRoughness);
#else
    half3 diffuseColor = LightingLambert(color, direction, normalWS);
#endif
    return diffuseColor;
#else
    return 0;
#endif
}

half CalculateNdotV(half3 normalWS, half3 viewDirectionWS)
{
#if defined(_REFLECTION) || defined(_RIM_LIGHT)
    half nv = saturate(dot(normalWS, viewDirectionWS));
#else
    half nv = 0;
#endif
    return nv;
}

half3 CalculateRimLighting(half nDotV, half3 rimColor, half rimPower)
{
#if _RIM_LIGHT
    return rimColor * pow((1 - nDotV), rimPower);
#else
    return 0;
#endif
}

half3 CalculateReflectionProbe(InputData inputData, SurfaceData surfaceData, half nDotV, half reflectivity)
{
#if _REFLECTION
    half perceptualRoughness = 1 - surfaceData.smoothness;
    half roughness = perceptualRoughness * perceptualRoughness;
    half3 reflectVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
    half3 refl_hdr = GlossyEnvironmentReflection(reflectVector, inputData.positionWS, perceptualRoughness, surfaceData.occlusion, inputData.normalizedScreenSpaceUV);
    half surfaceReduction = 1.0 / (roughness * roughness + 1);
    half3 specular = lerp(kDielectricSpec.rgb, surfaceData.albedo, surfaceData.metallic);
    refl_hdr *= surfaceReduction * lerp(specular, saturate(surfaceData.smoothness + reflectivity), Pow4(1 - nDotV));
    return refl_hdr;
#else
    return 0;
#endif
}

half4 UniversalFragmentCustomLighting(InputData inputData, SurfaceData surfaceData, CustomLight light)
{
#if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, debugColor)) { return debugColor; }
#endif

    half3 ambientColor = inputData.bakedGI;
    ambientColor += CalculateDiffusion(light.diffuseColor.rgb, light.lightDirection.xyz, inputData.normalWS, inputData.viewDirectionWS, 1 - surfaceData.smoothness);
    half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
    half3 col = surfaceData.albedo * oneMinusReflectivity * ambientColor;
    half nv = CalculateNdotV(inputData.normalWS, inputData.viewDirectionWS);
    col += CalculateRimLighting(nv, light.rimLightColor.rgb, light.rimLightPower);
    col += CalculateReflectionProbe(inputData, surfaceData, nv, 1 - oneMinusReflectivity);
    return half4(col * surfaceData.occlusion, surfaceData.alpha);
}

#endif
