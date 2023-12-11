#ifndef CUSTOM_COLOR_BLENDING_INCLUDED
#define CUSTOM_COLOR_BLENDING_INCLUDED

half3 BlendBurn(half3 base, half3 blend, half opacity)
{
    half3 o = 1.0 - (1.0 - blend) / (base + 0.000000000001);
    return lerp(base, o, opacity);
}

half3 BlendDarken(half3 base, half3 blend, half opacity)
{
    half3 o = min(blend, base);
    return lerp(base, o, opacity);
}

half3 BlendDifference(half3 base, half3 blend, half opacity)
{
    half3 o = abs(blend - base);
    return lerp(base, o, opacity);
}

half3 BlendDodge(half3 base, half3 blend, half opacity)
{
    half3 o = base / (1.0 - clamp(blend, 0.000001, 0.999999));
    return lerp(base, o, opacity);
}

half3 BlendDivide(half3 base, half3 blend, half opacity)
{
    half3 o = base / (blend + 0.000000000001);
    return lerp(base, o, opacity);
}

half3 BlendExclusion(half3 base, half3 blend, half opacity)
{
    half3 o = blend + base - (2.0 * blend * base);
    return lerp(base, o, opacity);
}

half3 BlendHardLight(half3 base, half3 blend, half opacity)
{
    half3 res1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half3 res2 = 2.0 * base * blend;
    half3 zeroOrOne = step(blend, 0.5);
    half3 o = res2 * zeroOrOne + (1 - zeroOrOne) * res1;
    return lerp(base, o, opacity);
}

half3 BlendHardMix(half3 base, half3 blend, half opacity)
{
    half3 o = step(1.0 - base, blend);
    return lerp(base, o, opacity);
}

half3 BlendLighten(half3 base, half3 blend, half opacity)
{
    half3 o = max(base, blend);
    return lerp(base, o, opacity);
}

half3 BlendLinearBurn(half3 base, half3 blend, half opacity)
{
    half3 o = base + blend - 1.0;
    return lerp(base, o, opacity);
}

half3 BlendLinearDodge(half3 base, half3 blend, half opacity)
{
    half3 o = base + blend;
    return lerp(base, o, opacity);
}

half3 BlendLinearLight(half3 base, half3 blend, half opacity)
{
    half3 o = blend < 0.5 ? max(base + (2.0 * blend) - 1.0, 0) : min(base + 2.0 * (blend - 0.5), 1);
    return lerp(base, o, opacity);
}

half3 BlendLinearLightAddSub(half3 base, half3 blend, half opacity)
{
    half3 o = blend + 2.0 * base - 1.0;
    return lerp(base, o, opacity);
}

half3 BlendMultiply(half3 base, half3 blend, half opacity)
{
    half3 o = base * blend;
    return lerp(base, o, opacity);
}

half3 BlendNegation(half3 base, half3 blend, half opacity)
{
    half3 o = 1.0 - abs(1.0 - blend - base);
    return lerp(base, o, opacity);
}

half3 BlendScreen(half3 base, half3 blend, half opacity)
{
    half3 o = 1.0 - (1.0 - blend) * (1.0 - base);
    return lerp(base, o, opacity);
}

half3 BlendOverlay(half3 base, half3 blend, half opacity)
{
    half3 res1 = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);
    half3 res2 = 2.0 * base * blend;
    half3 zeroOrOne = step(base, 0.5);
    half3 o = res2 * zeroOrOne + (1.0 - zeroOrOne) * res1;
    return lerp(base, o, opacity);
}

half3 BlendPinLight(half3 base, half3 blend, half opacity)
{
    half3 check = step(0.5, blend);
    half3 res1 = check * max(2.0 * (base - 0.5), blend);
    half3 o = res1 + (1.0 - check) * min(2.0 * base, blend);
    return lerp(base, o, opacity);
}

half3 BlendSoftLight(half3 base, half3 blend, half opacity)
{
    half3 res1 = 2.0 * base * blend + base * base * (1.0 - 2.0 * blend);
    half3 res2 = sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
    half3 zeroOrOne = step(0.5, blend);
    half3 o = res2 * zeroOrOne + (1.0 - zeroOrOne) * res1;
    return lerp(base, o, opacity);
}

half3 BlendVividLight(half3 base, half3 blend, half opacity)
{
    base = clamp(base, 0.000001, 0.999999);
    half3 res1 = 1.0 - (1.0 - blend) / (2.0 * base);
    half3 res2 = blend / (2.0 * (1.0 - base));
    half3 zeroOrOne = step(0.5, base);
    half3 o = res2 * zeroOrOne + (1.0 - zeroOrOne) * res1;
    return lerp(base, o, opacity);
}

half3 BlendSubtract(half3 base, half3 blend, half opacity)
{
    half3 o = base - blend;
    return lerp(base, o, opacity);
}

half3 BlendOverwrite(half3 base, half3 blend, half opacity)
{
    return lerp(base, blend, opacity);
}

#endif