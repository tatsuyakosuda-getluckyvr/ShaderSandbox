#ifndef CUSTOM_GPU_SKINNING_INCLUDED
#define CUSTOM_GPU_SKINNING_INCLUDED

struct SVertInSkin
{
    float weight0, weight1, weight2, weight3;
    int index0, index1, index2, index3;
};

StructuredBuffer<SVertInSkin> _Skin;
StructuredBuffer<float4x4> _Bones;

half3 GetVertexPosition(half3 vertex, uint vId)
{
    SVertInSkin si = _Skin[vId];
    half3 res = 0;
    res += si.weight0 * mul(_Bones[si.index0], half4(vertex, 1)).xyz;
    res += si.weight1 * mul(_Bones[si.index1], half4(vertex, 1)).xyz;
    res += si.weight2 * mul(_Bones[si.index2], half4(vertex, 1)).xyz;
    res += si.weight3 * mul(_Bones[si.index3], half4(vertex, 1)).xyz;
    return res;
}

half3 GetNormal(half3 normal, uint vId)
{
    SVertInSkin si = _Skin[vId];
    half3 res = 0;
    res += si.weight0 * mul(_Bones[si.index0], half4(normal, 0)).xyz;
    res += si.weight1 * mul(_Bones[si.index1], half4(normal, 0)).xyz;
    res += si.weight2 * mul(_Bones[si.index2], half4(normal, 0)).xyz;
    res += si.weight3 * mul(_Bones[si.index3], half4(normal, 0)).xyz;
    return res;
}

half3 GetTangent(half3 tangent, uint vId)
{
    SVertInSkin si = _Skin[vId];
    half3 res = 0;
    res += si.weight0 * mul((float3x3)_Bones[si.index0], tangent).xyz;
    res += si.weight1 * mul((float3x3)_Bones[si.index1], tangent).xyz;
    res += si.weight2 * mul((float3x3)_Bones[si.index2], tangent).xyz;
    res += si.weight3 * mul((float3x3)_Bones[si.index3], tangent).xyz;
    return res;
}

#endif
