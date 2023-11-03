Shader "Custom/Avatar"
{
    Properties
    { 
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
        _BaseMapArray ("Albedo (RGB)", 2DArray) = "white" {}
        _BaseMapIndex ("BaseMap Array Index", Integer) = 0
        _BaseColor ("BaseColor", Color) = (1, 1, 1, 1)
        _MARMap ("Metallic(R)AO(G)Roughness(B) Texture", 2D) = "white" {}
        _MARMapArray ("Metallic(R)AO(G)Roughness(B) Texture Array", 2DArray) = "black" {}
        _MARMapIndex ("Metallic(R)AO(G)Roughness(B) Texture Array Index", Integer) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 1
        _Metallic ("Metallic", Range(0, 1)) = 1
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapArray ("Normal Map Array", 2DArray) = "bump" {}
        _NormalMapIndex ("Normal Map Array Index", Integer) = 0
        _NormalStrength ("Normal strength", Range(0, 1)) = 1
        _Occlusion ("Ambient Occlusion", Range(0, 1)) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
        _EulerLightDirection("Light Dirrection (Euler)", Vector) = (0, 0, 0, 0)
        _LightDirection ("Light Direction", Vector) = (0, 0, 0, 0)
        _RimColor ("Rim Light Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Light Power", Range(0.0, 5.0)) = 2.0

        [HideInInspector] _DebugMipmapTex ("Debug mipmap texture", 2D) = "white" {}
        [HideInInspector] _DebugMipmapTexArray ("Debug mipmap texture array", 2DArray) = "white" {}
        [HideInInspector] _SrcBlend ("SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("ZWrite", Float) = 1
        [HideInInspector] _Cull ("Culling", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "UniversalForward"
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #pragma shader_feature_local_fragment _ _SIMPLELIT _UNLIT _CUSTOMLIT
            #pragma shader_feature_local_fragment _ _MARMAP
            #pragma shader_feature_local_fragment _ _DIFFUSE_COLOR
            #pragma shader_feature_local_fragment _ _REFLECTION
            #pragma shader_feature_local_fragment _ _RIM_LIGHT
            #pragma shader_feature_local_fragment _ _HALF_LAMBERT _MINNAERT
            #pragma shader_feature_local_fragment _ _DEBUG_MIPMAP
            #pragma shader_feature_local _ _TEX_ARRAY
            #pragma shader_feature_local _ _NORMALMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"

            struct Attributes
            {
                half2 uv         : TEXCOORD0;
                half2 lightmapUV : TEXCOORD1;
                half4 positionOS : POSITION;
                half3 normalOS   : NORMAL;
                half4 tangentOS  : TANGENT;
#if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
#endif
            };

            struct Varyings
            {
                half2 uv         : TEXCOORD0;
                half4 positionCS : SV_POSITION;
                half3 positionWS : TEXCOORD1;
                half3 normalWS   : TEXCOORD2;
#if _NORMALMAP
                half4 tangentWS  : TEXCOORD3;
#endif
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
                half3 vertexLight : TEXVCOORD5;
#if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D_ARRAY(_BaseMapArray); SAMPLER(sampler_BaseMapArray);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D_ARRAY(_NormalMapArray); SAMPLER(sampler_NormalMapArray);
            TEXTURE2D(_MARMap); SAMPLER(sampler_MARMap);
            TEXTURE2D_ARRAY(_MARMapArray); SAMPLER(sampler_MARMapArray);
            TEXTURE2D(_DebugMipmapTex); SAMPLER(sampler_DebugMipmapTex);
            TEXTURE2D_ARRAY(_DebugMipmapTexArray); SAMPLER(sampler_DebugMipmapTexArray);

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseMap_ST;
            half4 _BaseMapArray_ST;
            half4 _NormalMap_ST;
            half4 _NormalMapArray_ST;
            half4 _MRMap_ST;
            half4 _MRMapArray_ST;
            half4 _DebugMipmapTexArray_ST;
            half4 _DebugMipmapTex_ST;
            half4 _BaseColor;
            half4 _DiffuseColor;
            half4 _LightDirection;
            half4 _RimColor;
            half _Smoothness;
            half _Metallic;
            half _NormalStrength;
            half _Occlusion;
            half _Cutoff;
            half _RimPower;
            half _BaseMapIndex;
            half _NormalMapIndex;
            half _MARMapIndex;
            CBUFFER_END

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

            half4 UniversalFragmentCustomLighting(InputData inputData, SurfaceData surfaceData)
            {
#if defined(DEBUG_DISPLAY)
                half4 debugColor;

                if (CanDebugOverrideOutputColor(inputData, surfaceData, debugColor)) { return debugColor; }
#endif
                half3 ambientColor = inputData.bakedGI;
#if _DIFFUSE_COLOR
#if _HALF_LAMBERT
                half3 diffuseColor = LightingHalfLambert(_DiffuseColor.rgb, _LightDirection.rgb, inputData.normalWS);
#elif _MINNAERT
                half3 diffuseColor = LightingMinnaert(_DiffuseColor.rgb, _LightDirection.rgb, inputData.normalWS, inputData.viewDirectionWS, 1 - surfaceData.smoothness);
#else
                half3 diffuseColor = LightingLambert(_DiffuseColor.rgb, _LightDirection.rgb, inputData.normalWS);
#endif
                ambientColor += diffuseColor;
#endif
                half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
                half3 col = surfaceData.albedo * oneMinusReflectivity * ambientColor;
#if defined(_REFLECTION) || defined(_RIM_LIGHT)
                half nv = saturate(dot(inputData.normalWS, inputData.viewDirectionWS));
#if defined(_RIM_LIGHT)
                half rimPower = 1 - nv;
                half3 rimColor = _RimColor.rgb * pow(rimPower, _RimPower);
                col += rimColor;
#endif
#if defined(_REFLECTION)
                half fresnelTerm = Pow4(1.0 - nv);
                half perceptualRoughness = 1 - surfaceData.smoothness;
                half roughness = perceptualRoughness * perceptualRoughness;
                half3 reflectVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
                half3 refl_hdr = GlossyEnvironmentReflection(reflectVector, inputData.positionWS, perceptualRoughness, _Occlusion, inputData.normalizedScreenSpaceUV);
                half surfaceReduction = 1.0 / (roughness * roughness + 1);
                half reflectivity = 1.0 - oneMinusReflectivity;
                half3 specular = lerp(kDielectricSpec.rgb, surfaceData.albedo, surfaceData.metallic);
                refl_hdr *= surfaceReduction * lerp(specular, saturate(surfaceData.smoothness + reflectivity), fresnelTerm);
                col += refl_hdr;
#endif
#endif
                return half4(col * surfaceData.occlusion, surfaceData.alpha);
            }

            Varyings vert(Attributes input)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(input.normalOS);
#if _TEX_ARRAY
                o.uv = TRANSFORM_TEX(input.uv, _BaseMapArray);
#else
                o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
#endif
#if _NORMALMAP
                o.tangentWS = half4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
#endif
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, o.lightmapUV);
                OUTPUT_SH(o.normalWS.xyz, o.vertexSH);
                o.vertexLight = VertexLighting(o.positionWS, o.normalWS);
                return o;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
#if _TEX_ARRAY
                half4 col = SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, input.uv, _BaseMapIndex) * _BaseColor;
#else
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
#endif
                clip(col.a - _Cutoff);
                half3 normalWS = normalize(input.normalWS);
#if _NORMALMAP
#if _TEX_ARRAY
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D_ARRAY(_NormalMapArray, sampler_NormalMapArray, input.uv, _NormalMapIndex), _NormalStrength);
#else
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalStrength);
#endif
                half3x3 tangent2World = CreateTangentToWorld(normalWS, input.tangentWS.xyz, input.tangentWS.w);
                normalWS = normalize(TransformTangentToWorld(normalTS, tangent2World));
#endif
#if !defined(_UNLIT) && !defined(_SIMPLELIT)
#if _MARMAP
#if _TEX_ARRAY
                half3 mar = SAMPLE_TEXTURE2D_ARRAY(_MARMapArray, sampler_MARMapArray, input.uv, _MARMapIndex).rgb;
#else
                half3 mar = SAMPLE_TEXTURE2D(_MARMap, sampler_MARMap, input.uv).rgb;
#endif
#else
                half3 mar = half3(1, 1, 0);
#endif
#endif

                InputData lightingInput = (InputData)0; // in URP/ShaderLib/Input.hlsl
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = normalWS;
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS); // In ShaderVariablesFunctions.hlsl
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS); // In Shadows.hlsl
                lightingInput.positionCS = input.positionCS;
#if _NORMALMAP
                lightingInput.tangentToWorld = tangent2World;
#endif
                lightingInput.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
                lightingInput.vertexLighting = input.vertexLight;
                lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                lightingInput.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);

                SurfaceData surfaceInput = (SurfaceData)0;
                surfaceInput.albedo = col.rgb;
                surfaceInput.alpha = col.a;
                surfaceInput.emission = 0;
#if !defined(_UNLIT) && !defined(_SIMPLELIT)
                surfaceInput.specular = 0;
                surfaceInput.metallic = mar.r * _Metallic;
                surfaceInput.smoothness = (1 - mar.b) * _Smoothness;
#endif
#if _NORMALMAP
                surfaceInput.normalTS = normalTS;
#endif
#if !_UNLIT
                surfaceInput.occlusion = mar.g * _Occlusion;
#endif
                surfaceInput.clearCoatMask = 0;
                surfaceInput.clearCoatSmoothness = 0;

#if _UNLIT
                col = UniversalFragmentUnlit(lightingInput, surfaceInput);
#elif _SIMPLELIT
                col = UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
#elif _CUSTOMLIT
                col = UniversalFragmentCustomLighting(lightingInput, surfaceInput);
#else
                col = UniversalFragmentPBR(lightingInput, surfaceInput);
#endif
#if _DEBUG_MIPMAP
#if _TEX_ARRAY
                input.uv = (input.uv - _BaseMapArray_ST.zw) / _BaseMapArray_ST.xy;
                return SAMPLE_TEXTURE2D_ARRAY(_DebugMipmapTexArray, sampler_DebugMipmapTexArray, TRANSFORM_TEX(input.uv, _DebugMipmapTexArray), 0);
#else
                input.uv = (input.uv - _BaseMap_ST.zw) / _BaseMap_ST.xy;
                return SAMPLE_TEXTURE2D(_DebugMipmapTex, sampler_DebugMipmapTex, TRANSFORM_TEX(input.uv, _DebugMipmapTex));
#endif
#else
                return col;
#endif
            }
            ENDHLSL
        }
    }

    CustomEditor "CustomAvatarShaderGUI"
}