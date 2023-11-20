#ifndef UNIVERSAL_AVATAR_PASS_INCLUDED
#define UNIVERSAL_AVATAR_PASS_INCLUDED

struct Attributes
{
    half2 uv         : TEXCOORD0;
    half2 lightmapUV : TEXCOORD1;
    half4 positionOS : POSITION;
    half3 normalOS   : NORMAL;
    half4 tangentOS  : TANGENT;
#ifdef UNITY_ANY_INSTANCING_ENABLED
    uint instanceID : INSTANCEID_SEMANTIC;
#endif
};

struct Varyings
{
    half2 uv         : TEXCOORD0;
    half4 positionCS : SV_POSITION;
    half3 positionWS : TEXCOORD1;
    half3 normalWS   : TEXCOORD2;
#ifdef _NORMALMAP
    half4 tangentWS  : TEXCOORD3;
#endif
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
    half3 vertexLight : TEXCOORD5;
    half3 positionOS : TEXCOORD6;
#ifdef UNITY_ANY_INSTANCING_ENABLED
    uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
    uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
    uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
};

Varyings AvatarPassVertex(Attributes input, uint vId : SV_VertexID)
{
    Varyings o = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#if _CUSTOM_GPU_SKINNING
    input.positionOS.xyz = GetVertexPosition(input.positionOS.xyz, vId);
    input.normalOS = GetNormal(input.normalOS, vId);
    input.tangentOS.xyz = GetTangent(input.tangentOS.xyz, vId);
#endif
    o.positionOS = input.positionOS.xyz;
    o.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    o.positionCS = TransformWorldToHClip(o.positionWS);
    o.normalWS = TransformObjectToWorldNormal(input.normalOS);
#ifdef _TEX_ARRAY
    o.uv = TRANSFORM_TEX(input.uv, _BaseMapArray);
#else
    o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
#endif
#ifdef _NORMALMAP
    o.tangentWS = half4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
#endif
    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, o.lightmapUV);
    OUTPUT_SH(o.normalWS.xyz, o.vertexSH);
    o.vertexLight = VertexLighting(o.positionWS, o.normalWS);
    return o;
}

inline void InitializeInputData(Varyings input, half3 normalTS, out InputData lightingInput)
{
    lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS); // In ShaderVariablesFunctions.hlsl
    lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS); // In Shadows.hlsl
    lightingInput.positionCS = input.positionCS;
#ifdef _NORMALMAP
    half3x3 tangent2World = CreateTangentToWorld(input.normalWS, input.tangentWS.xyz, input.tangentWS.w);
    half3 normalWS = normalize(TransformTangentToWorld(normalTS, tangent2World));
    lightingInput.tangentToWorld = tangent2World;
    lightingInput.normalWS = normalWS;
#else
    lightingInput.normalWS = input.normalWS;
#endif
    lightingInput.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, lightingInput.normalWS);
    lightingInput.vertexLighting = input.vertexLight;
    lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    lightingInput.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

inline void InitializeSurfaceData(float2 uv, out SurfaceData surfaceData)
{
    surfaceData = (SurfaceData)0;
    half4 col = SampleAlbedo(uv);
    AlphaDiscard(col.a);
    surfaceData.albedo = col.rgb;
    surfaceData.alpha = col.a;
    half3 mas = SampleMAR(uv);
    surfaceData.metallic = mas.r;
    surfaceData.smoothness = mas.b;
    surfaceData.occlusion = mas.g;
    surfaceData.normalTS = SampleNormal(uv);
}

half4 AvatarPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceInput;
    InitializeSurfaceData(input.uv, surfaceInput);

    InputData lightingInput;
    InitializeInputData(input, surfaceInput.normalTS, lightingInput);

    half4 col = CalculateFinalColor(lightingInput, surfaceInput);
#ifdef _DEBUG_MIPMAP
    col = DebugMipmap(input.uv, col);
#endif
    return col;
}

#endif