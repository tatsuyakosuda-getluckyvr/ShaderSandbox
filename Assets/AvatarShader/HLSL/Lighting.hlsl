#ifndef UNIVERSAL_CUSTOM_LIGHTING_INCLUDED
#define UNIVERSAL_CUSTOM_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct CustomLight
{
    half4 gradientScale;
    half4 mouthScale;
    half3 diffuseColor;
    half3 diffuseColor2;
    half3 lightDirection;
    half3 lightDirection2;
    half3 rimLightColor;
    half3 rimLightColor2;
    half3 anisoHighlightColor;
    half gradientAngle;
    half rimLightPower;
    half rimLightPower2;
    half gradientPower;
    half gradientOffset;
    half diffuseIntensity;
    half diffuseIntensity2;
    half rimIntensity;
    half rimIntensity2;
    half anisoOffset;
    half anisoPower;
    half anisoIntensity;
    half hairUVTileIndex;
    half mouthAOPower;
    half mouthUVTileIndex;
    half mouthAOIntensity;
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
#ifdef _DIFFUSE_COLOR
#ifdef _HALF_LAMBERT
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
#if defined(_REFLECTION) || defined(_RIM_LIGHT) || defined(_CLEARCOAT)
    half nv = saturate(dot(normalWS, viewDirectionWS));
#else
    half nv = 0;
#endif
    return nv;
}

half3 CalculateRimLighting(half nDotV, half3 rimColor, half rimPower)
{
#ifdef _RIM_LIGHT
    return rimColor * pow((1 - nDotV), rimPower);
#else
    return 0;
#endif
}

half3 CalculateReflectionProbe(InputData inputData, SurfaceData surfaceData, half nDotV, half reflectivity, half3 reflectVector, half fresnelTerm)
{
#ifdef _REFLECTION
    half perceptualRoughness = 1 - surfaceData.smoothness;
    half roughness = perceptualRoughness * perceptualRoughness;
    half3 refl_hdr = GlossyEnvironmentReflection(reflectVector, inputData.positionWS, perceptualRoughness, surfaceData.occlusion, inputData.normalizedScreenSpaceUV);
    half surfaceReduction = 1.0 / (roughness * roughness + 1);
    half3 specular = lerp(kDielectricSpec.rgb, surfaceData.albedo, surfaceData.metallic);
    refl_hdr *= surfaceReduction * lerp(specular, saturate(surfaceData.smoothness + reflectivity), fresnelTerm);
    return refl_hdr;
#else
    return 0;
#endif
}

half3 CalculateClearCoatColor(InputData inputData, SurfaceData surfaceData, half nDotV, half reflectivity, half3 reflectVector, half fresnelTerm)
{
    half perceptualRoughness = 1 - surfaceData.clearCoatSmoothness;
    half roughness = perceptualRoughness * perceptualRoughness;
    half3 refl_hdr = GlossyEnvironmentReflection(reflectVector, inputData.positionWS, perceptualRoughness, surfaceData.occlusion, inputData.normalizedScreenSpaceUV);
    half surfaceReduction = 1.0 / (roughness * roughness + 1);
    half3 specular = kDielectricSpec.rgb;
    refl_hdr *= surfaceReduction * lerp(specular, saturate(surfaceData.clearCoatSmoothness + kDielectricSpec.x), fresnelTerm);
    return refl_hdr;
}

half4 UniversalFragmentCustomLighting(InputData inputData, SurfaceData surfaceData, CustomLight light, half3 positionOS, half tileIndex)
{
#if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, debugColor)) { return debugColor; }
#endif

#ifdef _GRADIENT_LIGHT
#if _OBJECT_SPACE_GRADIENT
    half2 gradient = (positionOS.xy - light.gradientScale.xy) / (light.gradientScale.zw - light.gradientScale.xy);
#else
    half2 gradient = (inputData.positionWS.xz - light.gradientScale.xy) / (light.gradientScale.zw - light.gradientScale.xy);
#endif

    half angleCos = cos(radians(light.gradientAngle));
    half angleSin = sin(radians(light.gradientAngle));
    half2x2 rotateMatrix = half2x2(angleCos, -angleSin, angleSin, angleCos);
    gradient = mul(gradient - 0.5, rotateMatrix) + 0.5;

#if _RADIAL_GRADIENT_LIGHT
#if _EXP_GRADIENT_MODE
    half weight = pow(saturate(length((gradient - 0.5) * 2) + light.gradientOffset), light.gradientPower);
#else
    half weight = saturate(length((gradient - 0.5) * 2) + light.gradientOffset);
#endif
#else
#if _EXP_GRADIENT_MODE
    half weight = pow(saturate(gradient.x + light.gradientOffset), light.gradientPower);
#else
    half weight = saturate(gradient.x + light.gradientOffset);
#endif
#endif

    half3 diffuseColor = lerp(light.diffuseColor.rgb, light.diffuseColor2.rgb, weight);
    half3 lightDirection = lerp(light.lightDirection.xyz, light.lightDirection2.xyz, weight);
    half3 rimColor = lerp(light.rimLightColor.rgb, light.rimLightColor2.rgb, weight);
    half lightIntensity = lerp(light.diffuseIntensity, light.diffuseIntensity2, weight);
    half rimIntensity = lerp(light.rimIntensity, light.rimIntensity2, weight);
    half rimPower = lerp(light.rimLightPower, light.rimLightPower2, weight);
#else
    half3 diffuseColor = light.diffuseColor.rgb;
    half3 lightDirection = light.lightDirection.xyz;
    half3 rimColor = light.rimLightColor.rgb;
    half lightIntensity = light.diffuseIntensity;
    half rimIntensity = light.rimIntensity;
    half rimPower = light.rimLightPower;
#endif

    half3 ambientColor = inputData.bakedGI;
    ambientColor += CalculateDiffusion(diffuseColor, lightDirection, inputData.normalWS, inputData.viewDirectionWS, 1 - surfaceData.smoothness) * lightIntensity;
    half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
    half3 col = surfaceData.albedo * oneMinusReflectivity * ambientColor;

#if defined(_REFLECTION) || defined(_RIM_LIGHT) || defined(_CLEARCOAT)
    half nv = CalculateNdotV(inputData.normalWS, inputData.viewDirectionWS);
#endif

#ifdef _RIM_LIGHT
    col += CalculateRimLighting(nv, rimColor, rimPower) * rimIntensity;
#endif

#if defined(_REFLECTION) || defined(_CLEARCOAT)
    half fresnelTerm = Pow4(1 - nv);
    half3 reflectVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
#endif

#ifdef _REFLECTION
    col += CalculateReflectionProbe(inputData, surfaceData, nv, 1 - oneMinusReflectivity, reflectVector, fresnelTerm);
#endif

#if _CLEARCOAT
    half3 coatColor = CalculateClearCoatColor(inputData, surfaceData, nv, 1 - oneMinusReflectivity, reflectVector, fresnelTerm) * surfaceData.clearCoatMask;
    half coatFresnel = kDielectricSpec.x + kDielectricSpec.a * fresnelTerm;
    col = lerp(col * (1 - coatFresnel * surfaceData.clearCoatMask) + coatColor, col, step(1, abs(tileIndex - light.mouthUVTileIndex)));
#endif

#ifdef _ANISOTROPIC_HIGHLIGHT
    half3 halfDir = normalize(lightDirection + inputData.viewDirectionWS);
    half hdota = dot(normalize(inputData.normalWS), halfDir);
    half a = max(0, sin(radians((hdota + light.anisoOffset) * 180)));
    half spec = saturate(pow(a, light.anisoPower * lightIntensity) * light.anisoIntensity * lightIntensity);
    col = lerp(col + spec * light.anisoHighlightColor * diffuseColor, col, step(1, abs(tileIndex - light.hairUVTileIndex)));
#endif

    col = col * surfaceData.occlusion;

#ifdef _MOUTH_SHADOW
    col = lerp(col * min(1, pow(abs(positionOS.z / light.mouthScale.w - light.mouthScale.y), light.mouthAOPower) * light.mouthAOIntensity), col, step(1, abs(tileIndex - light.mouthUVTileIndex)));
#endif

    return half4(col, surfaceData.alpha);
}

#endif
