#ifndef UNIVERSAL_AVATAR_PASS_INCLUDED
#define UNIVERSAL_AVATAR_PASS_INCLUDED

struct Attributes
{
    half2 uv0        : TEXCOORD0;
    half4 positionOS : POSITION;
    half3 normalOS   : NORMAL;
    half4 tangentOS  : TANGENT;
#if defined(_EYES_MAKEUP_UV1) || defined(_BLUSH_MAKEUP_UV1) || defined(_LIPS_MAKEUP_UV1) || defined(_FACIAL_TATOO_UV1) || defined(_FACIAL_MARKING_UV1)
    half2 uv1        : TEXCOORD1;
#endif
#if defined(_EYES_MAKEUP_UV2) || defined(_BLUSH_MAKEUP_UV2) || defined(_LIPS_MAKEUP_UV2) || defined(_FACIAL_TATOO_UV2) || defined(_FACIAL_MARKING_UV2)
    half2 uv2        : TEXCOORD2;
#endif
#if defined(_EYES_MAKEUP_UV3) || defined(_BLUSH_MAKEUP_UV3) || defined(_LIPS_MAKEUP_UV3) || defined(_FACIAL_TATOO_UV3) || defined(_FACIAL_MARKING_UV3)
    half2 uv3        : TEXCOORD3;
#endif
    half4 vColor : COLOR;
#ifdef UNITY_ANY_INSTANCING_ENABLED
    uint instanceID : INSTANCEID_SEMANTIC;
#endif
};

struct Varyings
{
    half4 positionCS : SV_POSITION;
    half4 uv01       : TEXCOORD0;
    half4 uv23       : TEXCOORD1;
    half3 positionWS : TEXCOORD2;
    half3 normalWS   : TEXCOORD3;
    half4 vColor     : TEXCOORD4;
#ifdef _NORMALMAP
    half4 tangentWS   : TEXCOORD5;
#endif
    half3 vertexSH    : TEXCOORD6;
    half3 vertexLight : TEXCOORD7;
#if defined(_OBJECT_SPACE_GRADIENT) || defined(_MOUTH_SHADOW)
    half3 positionOS  : TEXCOORD8;
#endif
#ifdef _MATCAPMAP
    half2 matCapCoord : TEXCOORD9;
#endif

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

#ifdef _CUSTOM_GPU_SKINNING
    input.positionOS.xyz = GetVertexPosition(input.positionOS.xyz, vId);
    input.normalOS = GetNormal(input.normalOS, vId);
    input.tangentOS.xyz = GetTangent(input.tangentOS.xyz, vId);
#endif

#if defined(_OBJECT_SPACE_GRADIENT) || defined(_MOUTH_SHADOW)
    o.positionOS = input.positionOS.xyz;
#endif

    o.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    o.positionCS = TransformWorldToHClip(o.positionWS);
    o.normalWS = TransformObjectToWorldNormal(input.normalOS);
    o.vColor = input.vColor;

#ifdef _TEX_ARRAY
    o.uv01.xy = TRANSFORM_TEX(input.uv0, _BaseMapArray);
#else
    o.uv01.xy = TRANSFORM_TEX(input.uv0, _BaseMap);
#endif

#if defined(_EYES_MAKEUP_UV1) || defined(_BLUSH_MAKEUP_UV1) || defined(_LIPS_MAKEUP_UV1) || defined(_FACIAL_TATOO_UV1) || defined(_FACIAL_MARKING_UV1)
    o.uv01.zw = TRANSFORM_TEX(input.uv1, _EyesMakeupMapArray);
#endif

#if defined(_EYES_MAKEUP_UV2) || defined(_BLUSH_MAKEUP_UV2) || defined(_LIPS_MAKEUP_UV2) || defined(_FACIAL_TATOO_UV2) || defined(_FACIAL_MARKING_UV2)
    o.uv23.xy = TRANSFORM_TEX(input.uv2, _BlushMakeupMapArray);
#endif

#if defined(_EYES_MAKEUP_UV3) || defined(_BLUSH_MAKEUP_UV3) || defined(_LIPS_MAKEUP_UV3) || defined(_FACIAL_TATOO_UV3) || defined(_FACIAL_MARKING_UV3)
    o.uv23.zw = TRANSFORM_TEX(input.uv3, _LipsMakeupMapArray);
#endif

#ifdef _NORMALMAP
    o.tangentWS = half4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
#endif

    OUTPUT_SH(o.normalWS.xyz, o.vertexSH);
    o.vertexLight = VertexLighting(o.positionWS, o.normalWS);

#ifdef _MATCAPMAP
    // change normal vector worldspace to viewspace and normalize it range from 0 to 1
    o.matCapCoord = mul((half3x3)UNITY_MATRIX_V, o.normalWS).xy * 0.5 + 0.5;
#endif

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

inline void InitializeSurfaceData(Varyings input, out SurfaceData surfaceData)
{
    surfaceData = (SurfaceData)0;
    half4 col = SampleAlbedo(input.uv01.xy);
    AlphaDiscard(col.a);
    UVDiscard(input.uv01.xy);
    col.rgb = SampleSkinColor(col);
    surfaceData.albedo = col.rgb;
    surfaceData.alpha = col.a;
    half3 mas = SampleMAR(input.uv01.xy);
    surfaceData.metallic = mas.r;
    surfaceData.smoothness = mas.b;
    surfaceData.occlusion = mas.g;
    surfaceData.normalTS = SampleNormal(input.uv01.xy);
    SampleClearCoat(surfaceData.clearCoatMask, surfaceData.clearCoatSmoothness);
    SampleHairColor(input.uv01.xy, input.vColor.b, surfaceData.albedo);

#ifdef _MATCAPMAP
    SampleMatCap(input.matCapCoord, half4(surfaceData.albedo, surfaceData.alpha));
#endif

#ifdef _EYES_MAKEUP_UV1
    SampleEyesMakeupMap(input.uv01.zw, half4(surfaceData.albedo, surfaceData.alpha));
#elif _EYES_MAKEUP_UV2
    SampleEyesMakeupMap(input.uv23.xy, half4(surfaceData.albedo, surfaceData.alpha));
#elif _EYES_MAKEUP_UV3
    SampleEyesMakeupMap(input.uv23.zw, half4(surfaceData.albedo, surfaceData.alpha));
#else
    SampleEyesMakeupMap(input.uv01.xy, half4(surfaceData.albedo, surfaceData.alpha));
#endif

#ifdef _BLUSH_MAKEUP_UV1
    SampleBlushMakeupMap(input.uv01.zw, half4(surfaceData.albedo, surfaceData.alpha));
#elif _BLUSH_MAKEUP_UV2
    SampleBlushMakeupMap(input.uv23.xy, half4(surfaceData.albedo, surfaceData.alpha));
#elif _BLUSH_MAKEUP_UV3
    SampleBlushMakeupMap(input.uv23.zw, half4(surfaceData.albedo, surfaceData.alpha));
#else
    SampleBlushMakeupMap(input.uv01.xy, half4(surfaceData.albedo, surfaceData.alpha));
#endif

#ifdef _LIPS_MAKEUP_UV1
    SampleLipsMakeupMap(input.uv01.zw, half4(surfaceData.albedo, surfaceData.alpha));
#elif _LIPS_MAKEUP_UV2
    SampleLipsMakeupMap(input.uv23.xy, half4(surfaceData.albedo, surfaceData.alpha));
#elif _LIPS_MAKEUP_UV3
    SampleLipsMakeupMap(input.uv23.zw, half4(surfaceData.albedo, surfaceData.alpha));
#else
    SampleLipsMakeupMap(input.uv01.xy, half4(surfaceData.albedo, surfaceData.alpha));
#endif

    half alpha = surfaceData.alpha;
#ifdef _FACIAL_TATOO_LEFT
    alpha *= input.vColor.g;
#elif _FACIAL_TATOO_RIGHT
    alpha *= (1 - input.vColor.g);
#endif

#ifdef FACIAL_TATOO_UV1
    SampleFacialTatooMap(input.uv01.zw, half4(surfaceData.albedo, alpha));
#elif FACIAL_TATOO_UV2
    SampleFacialTatooMap(input.uv23.xy, half4(surfaceData.albedo, alpha));
#elif FACIAL_TATOO_UV3
    SampleFacialTatooMap(input.uv23.zw, half4(surfaceData.albedo, alpha));
#else
    SampleFacialTatooMap(input.uv01.xy, half4(surfaceData.albedo, alpha));
#endif

    alpha = surfaceData.alpha;
#ifdef _FACIAL_MARKING_LEFT
    alpha *= input.vColor.g;
#elif _FACIAL_MARKING_RIGHT
    alpha *= (1 - input.vColor.g);
#endif

#ifdef FACIAL_MARKING_UV1
    SampleFacialMarkingMap(input.uv01.zw, half4(surfaceData.albedo, alpha));
#elif FACIAL_MARKING_UV2
    SampleFacialMarkingMap(input.uv23.xy, half4(surfaceData.albedo, alpha));
#elif FACIAL_MARKING_UV3
    SampleFacialMarkingMap(input.uv23.zw, half4(surfaceData.albedo, alpha));
#else
    SampleFacialMarkingMap(input.uv01.xy, half4(surfaceData.albedo, alpha));
#endif
}

half4 AvatarPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceInput;
    InitializeSurfaceData(input, surfaceInput);

    InputData lightingInput;
    InitializeInputData(input, surfaceInput.normalTS, lightingInput);

#if defined(_OBJECT_SPACE_GRADIENT) || defined(_MOUTH_SHADOW)
    half3 positionOS = input.positionOS.xyz;
#else
    half3 positionOS = 0;
#endif

#ifdef _UVTILING
    half tileIndex = GetUVTileIndex(input.uv01.xy);
#else
    half tileIndex = 0;
#endif

    half4 col = CalculateFinalColor(lightingInput, surfaceInput, positionOS, tileIndex);

#ifdef _DEBUG_MIPMAP
    col = DebugMipmap(input.uv, col);
#endif

    return col;
}

#endif