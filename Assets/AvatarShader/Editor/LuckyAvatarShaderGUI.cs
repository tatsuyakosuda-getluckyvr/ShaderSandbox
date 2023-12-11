using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;
using UnityEditor.Rendering;
using System.Security.Cryptography;

public class LuckyAvatarShaderGUI : ShaderGUI
{

    public enum RenderMode
    {
        UNITY_LIT, UNITY_SIMPLE_LIT, CUSTOM_LIT, UNITY_UNLIT
    }

    public enum SurfaceType
    {
        Opaque, Cutout, Transparent
    }

    public enum LightingModel
    {
        LAMBERT, HALF_LAMBERT, MINNAERT
    }

    public enum GradientStyle
    {
        LINEAR, RADIAL
    }

    public enum GradientMode
    {
        LINEAR, EXPONENTIAL
    }

    public enum SkinDetailMapUV
    {
        UV0, UV1, UV2, UV3
    }

    public enum SkinDetailBlendingMode
    {
        NORMAL, DARKEN, MULTIPLY, COLOR_BURN, LINEAR_BURN,
        LIGHTEN, SCREEN, COLOR_DODGE, LINEAR_DODGE,
        OVERLAY, SOFT_LIGHT, HARD_LIGHT, VIVID_LIGHT, LINEAR_LIGHT, LINEAR_LIGHT_ADD_SUB, PIN_LIGHT, HARD_MIX,
        DIFFERENCE, EXCLUSION, SUBTRACT, DIVIDE, NEGATION
    }

    public enum AsymmetricStamping
    {
        BOTH, LEFT, RIGHT
    }

    private class Properties
    {
        public MaterialProperty baseMap;
        public MaterialProperty baseMapArray;
        public MaterialProperty baseMapIndex;
        public MaterialProperty marMap;
        public MaterialProperty marMapArray;
        public MaterialProperty marMapIndex;
        public MaterialProperty metallic;
        public MaterialProperty smoothness;
        public MaterialProperty normalMap;
        public MaterialProperty normalMapArray;
        public MaterialProperty normalMapIndex;
        public MaterialProperty normalStrength;
        public MaterialProperty occlusion;
        public MaterialProperty cutoff;
        public MaterialProperty diffuseColor;
        public MaterialProperty diffuseColor2;
        public MaterialProperty eulerLightDirection;
        public MaterialProperty eulerLightDirection2;
        public MaterialProperty lightDirection;
        public MaterialProperty lightDirection2;
        public MaterialProperty rimColor;
        public MaterialProperty rimColor2;
        public MaterialProperty rimPower;
        public MaterialProperty rimPower2;
        public MaterialProperty gradientScale;
        public MaterialProperty debugMipmapTex;
        public MaterialProperty debugMipmapTexArray;
        public MaterialProperty gradientAngle;
        public MaterialProperty gradientPower;
        public MaterialProperty gradientOffset;
        public MaterialProperty diffuseIntensity;
        public MaterialProperty diffuseIntensity2;
        public MaterialProperty rimIntensity;
        public MaterialProperty rimIntensity2;
        public MaterialProperty uvTiles;
        public MaterialProperty matCapMap;
        public MaterialProperty deepLayerColor;
        public MaterialProperty matCapBlend;
        public MaterialProperty skinColor;
        public MaterialProperty uvCutoff;
        public MaterialProperty matCapHSVShift;
        public MaterialProperty anisoOffset;
        public MaterialProperty anisoPower;
        public MaterialProperty anisoIntensity;
        public MaterialProperty hairUVTileIndex;
        public MaterialProperty anisoHighlightColor;
        public MaterialProperty eyesMakeupMapArray;
        public MaterialProperty eyesMakeupMapArrayIndex;
        public MaterialProperty eyesMakeupUVTileIndex;
        public MaterialProperty blushMakeupMapArray;
        public MaterialProperty blushMakeupMapArrayIndex;
        public MaterialProperty blushMakeupUVTileIndex;
        public MaterialProperty lipsMakeupMapArray;
        public MaterialProperty lipsMakeupMapArrayIndex;
        public MaterialProperty lipsMakeupUVTileIndex;
        public MaterialProperty facialTatooMapArray;
        public MaterialProperty facialTatooMapArrayIndex;
        public MaterialProperty facialTatooUVTileIndex;
        public MaterialProperty facialMarkingMapArray;
        public MaterialProperty facialMarkingMapArrayIndex;
        public MaterialProperty facialMarkingUVTileIndex;
        public MaterialProperty mouthScale;
        public MaterialProperty mouthAOPower;
        public MaterialProperty mouthUVTileIndex;
        public MaterialProperty mouthAOIntensity;
        public MaterialProperty clearCoatMask;
        public MaterialProperty clearCoatSmoothness;
        public MaterialProperty hairColor1;
        public MaterialProperty hairColor2;
    }

    private RenderMode _renderMode;
    private SurfaceType _surfaceType;
    private LightingModel _lightingModel;
    private Material _target;
    private Properties _properties;
    private GradientStyle _gradientStyle;
    private GradientMode _gradientMode;
    private SkinDetailMapUV _eyesMakeupMapUV;
    private SkinDetailBlendingMode _eyesMakeupBlendingMode;
    private SkinDetailMapUV _blushMakeupMapUV;
    private SkinDetailBlendingMode _blushMakeupBlendingMode;
    private SkinDetailMapUV _lipsMakeupMapUV;
    private SkinDetailBlendingMode _lipsMakeupBlendingMode;
    private SkinDetailMapUV _facialTatooMapUV;
    private SkinDetailBlendingMode _facialTatooBlendingMode;
    private AsymmetricStamping _facialTatooAsymmetry;
    private SkinDetailMapUV _facialMarkingMapUV;
    private SkinDetailBlendingMode _facialMarkingBlendingMode;
    private AsymmetricStamping _facialMarkingAsymmetry;
    private bool _enableMARMap;
    private bool _enableNormalMap;
    private bool _enableDiffuseColor;
    private bool _enableReflection;
    private bool _enableRimLight;
    private bool _enableTex2DArray;
    private bool _enableDebugMipmap;
    private bool _enableGradient;
    private bool _enableUVTiling;
    private bool _enableObjectSpaceGradient;
    private bool _enableMatCapMap;
    private bool _enableAnisoHighlight;
    private bool _enableOneTexture;
    private bool _enableEyesMakeupMap;
    private bool _enableBlushMakeupMap;
    private bool _enableLipsMakeupMap;
    private bool _enableFacialTatooMap;
    private bool _enableFacialMarkingMap;
    private bool _enableMouthShadow;
    private bool _enableClearCoat;
    private bool _enableHairColor;

    private static readonly string UNLIT_KEYWORD = "_UNLIT";
    private static readonly string SIMPLE_LIT_KEYWORD = "_SIMPLELIT";
    private static readonly string CUSTOM_LIT_KEYWORD = "_CUSTOMLIT";

    private bool _isOpenSurfaceOptions;
    private bool _beforeIsOpenSurfaceOptions;
    private bool _isOpenSurfaceInputs;
    private bool _beforeIsOpenSurfaceInputs;
    private bool _isOpenLightSettings;
    private bool _beforeIsOpenLightSettings;
    private bool _isOpenAdvancedSettings;
    private bool _beforeIsOpenAdvancedSettings;

    private GUIStyle _headerLabel;
    private GUIStyle _warningLabel;
    private GUIContent _baseMapGUI;
    private GUIContent _marMapGUI;
    private GUIContent _normalMapGUI;
    private GUIContent _debugMipmapGUI;
    private GUIContent _matCapMapGUI;
    private GUIContent _eyesMakeupMapGUI;
    private GUIContent _blushMakeupMapGUI;
    private GUIContent _lipsMakeupMapGUI;
    private GUIContent _facialTatooMapGUI;
    private GUIContent _facialMarkingMapGUI;

    private enum Expandable
    {
        SurfaceOptions = 1 << 0,
        SurfaceInputs = 1 << 1,
        LightingSettings = 1 << 2,
        Advanced = 1 << 3,
    }

    private const string EDITOR_PREFS_KEY_PREFIX = "Material:UI_State:";

    private bool _initialized;

    private void Initialize()
    {
        if (_initialized) return;

        _initialized = true;

        if (_target.IsKeywordEnabled("_UNLIT"))
        {
            _renderMode = RenderMode.UNITY_UNLIT;
        }
        else if (_target.IsKeywordEnabled("_SIMPLELIT"))
        {
            _renderMode = RenderMode.UNITY_SIMPLE_LIT;
        }
        else if (_target.IsKeywordEnabled("_CUSTOMLIT"))
        {
            _renderMode = RenderMode.CUSTOM_LIT;
        }
        else
        {
            _renderMode = RenderMode.UNITY_LIT;
        }

        var surfacetype = _target.GetTag("rendertype", false, "opaque");

        switch (surfacetype)
        {
            case "transparent":
                _surfaceType = SurfaceType.Transparent;
                break;
            case "transparentcutout":
                _surfaceType = SurfaceType.Cutout;
                break;
            case "opaque":
            default:
                _surfaceType = SurfaceType.Opaque;
                break;
        }

        UpdateSurfaceType();

        if (_target.IsKeywordEnabled("_HALF_LAMBERT"))
        {
            _lightingModel = LightingModel.HALF_LAMBERT;
        }
        else if (_target.IsKeywordEnabled("_MINNAERT"))
        {
            _lightingModel = LightingModel.MINNAERT;
        }
        else
        {
            _lightingModel = LightingModel.LAMBERT;
        }

        if (_target.IsKeywordEnabled("_RADIAL_GRADIENT_LIGHT"))
        {
            _gradientStyle = GradientStyle.RADIAL;
        }
        else
        {
            _gradientStyle = GradientStyle.LINEAR;
        }

        if (_target.IsKeywordEnabled("_EXP_GRADIENT_MODE"))
        {
            _gradientMode = GradientMode.EXPONENTIAL;
        }
        else
        {
            _gradientMode = GradientMode.LINEAR;
        }

        if (_target.IsKeywordEnabled("_EYES_MAKEUP_UV1"))
        {
            _eyesMakeupMapUV = SkinDetailMapUV.UV1;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_UV2"))
        {
            _eyesMakeupMapUV = SkinDetailMapUV.UV2;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_UV3"))
        {
            _eyesMakeupMapUV = SkinDetailMapUV.UV3;
        }
        else
        {
            _eyesMakeupMapUV = SkinDetailMapUV.UV0;
        }

        if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_UV1"))
        {
            _blushMakeupMapUV = SkinDetailMapUV.UV1;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_UV2"))
        {
            _blushMakeupMapUV = SkinDetailMapUV.UV2;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_UV3"))
        {
            _blushMakeupMapUV = SkinDetailMapUV.UV3;
        }
        else
        {
            _blushMakeupMapUV = SkinDetailMapUV.UV0;
        }

        if (_target.IsKeywordEnabled("_LIPS_MAKEUP_UV1"))
        {
            _lipsMakeupMapUV = SkinDetailMapUV.UV1;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_UV2"))
        {
            _lipsMakeupMapUV = SkinDetailMapUV.UV2;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_UV3"))
        {
            _lipsMakeupMapUV = SkinDetailMapUV.UV3;
        }
        else
        {
            _lipsMakeupMapUV = SkinDetailMapUV.UV0;
        }

        if (_target.IsKeywordEnabled("_FACIAL_TATOO_LEFT"))
        {
            _facialTatooAsymmetry = AsymmetricStamping.LEFT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_RIGHT"))
        {
            _facialTatooAsymmetry = AsymmetricStamping.RIGHT;
        }
        else
        {
            _facialTatooAsymmetry = AsymmetricStamping.BOTH;
        }

        if (_target.IsKeywordEnabled("_FACIAL_TATOO_UV1"))
        {
            _facialTatooMapUV = SkinDetailMapUV.UV1;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_UV2"))
        {
            _facialTatooMapUV = SkinDetailMapUV.UV2;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_UV3"))
        {
            _facialTatooMapUV = SkinDetailMapUV.UV3;
        }
        else
        {
            _facialTatooMapUV = SkinDetailMapUV.UV0;
        }

        if (_target.IsKeywordEnabled("_FACIAL_MARKING_LEFT"))
        {
            _facialMarkingAsymmetry = AsymmetricStamping.LEFT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_RIGHT"))
        {
            _facialMarkingAsymmetry = AsymmetricStamping.RIGHT;
        }
        else
        {
            _facialMarkingAsymmetry = AsymmetricStamping.BOTH;
        }

        if (_target.IsKeywordEnabled("_FACIAL_MARKING_UV1"))
        {
            _facialMarkingMapUV = SkinDetailMapUV.UV1;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_UV2"))
        {
            _facialMarkingMapUV = SkinDetailMapUV.UV2;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_UV3"))
        {
            _facialMarkingMapUV = SkinDetailMapUV.UV3;
        }
        else
        {
            _facialMarkingMapUV = SkinDetailMapUV.UV0;
        }

        if (_target.IsKeywordEnabled("_EYES_MAKEUP_DARKEN"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.DARKEN;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_MULTIPLY"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.MULTIPLY;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_BURN"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.COLOR_BURN;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_LNEARBURN"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_BURN;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_LIGHTEN"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.LIGHTEN;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_SCREEN"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.SCREEN;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_DODGE"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.COLOR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_LINEAR_DODGE"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_OVERLAY"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.OVERLAY;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_SOFTLIGHT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.SOFT_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_HARDLIGHT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.HARD_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_VIVIDLIGHT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.VIVID_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_LINEARLIGHT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_LINEARLIGHTADDSUB"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_PINLIGHT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.PIN_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_HARDMIX"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.HARD_MIX;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_DIFF"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.DIFFERENCE;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_EXCLUSION"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.EXCLUSION;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_SUBTRACT"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.SUBTRACT;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_DIVIDE"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.DIVIDE;
        }
        else if (_target.IsKeywordEnabled("_EYES_MAKEUP_NEGATION"))
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.NEGATION;
        }
        else
        {
            _eyesMakeupBlendingMode = SkinDetailBlendingMode.NORMAL;
        }

        if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_DARKEN"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.DARKEN;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_MULTIPLY"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.MULTIPLY;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_BURN"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.COLOR_BURN;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_LNEARBURN"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_BURN;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_LIGHTEN"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.LIGHTEN;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_SCREEN"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.SCREEN;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_DODGE"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.COLOR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_LINEAR_DODGE"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_OVERLAY"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.OVERLAY;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_SOFTLIGHT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.SOFT_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_HARDLIGHT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.HARD_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_VIVIDLIGHT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.VIVID_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_LINEARLIGHT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_LINEARLIGHTADDSUB"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_PINLIGHT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.PIN_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_HARDMIX"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.HARD_MIX;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_DIFF"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.DIFFERENCE;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_EXCLUSION"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.EXCLUSION;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_SUBTRACT"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.SUBTRACT;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_DIVIDE"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.DIVIDE;
        }
        else if (_target.IsKeywordEnabled("_BLUSH_MAKEUP_NEGATION"))
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.NEGATION;
        }
        else
        {
            _blushMakeupBlendingMode = SkinDetailBlendingMode.NORMAL;
        }

        if (_target.IsKeywordEnabled("_LIPS_MAKEUP_DARKEN"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.DARKEN;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_MULTIPLY"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.MULTIPLY;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_BURN"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.COLOR_BURN;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_LNEARBURN"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_BURN;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_LIGHTEN"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.LIGHTEN;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_SCREEN"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.SCREEN;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_DODGE"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.COLOR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_LINEAR_DODGE"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_OVERLAY"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.OVERLAY;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_SOFTLIGHT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.SOFT_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_HARDLIGHT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.HARD_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_VIVIDLIGHT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.VIVID_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_LINEARLIGHT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_LINEARLIGHTADDSUB"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_PINLIGHT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.PIN_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_HARDMIX"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.HARD_MIX;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_DIFF"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.DIFFERENCE;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_EXCLUSION"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.EXCLUSION;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_SUBTRACT"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.SUBTRACT;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_DIVIDE"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.DIVIDE;
        }
        else if (_target.IsKeywordEnabled("_LIPS_MAKEUP_NEGATION"))
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.NEGATION;
        }
        else
        {
            _lipsMakeupBlendingMode = SkinDetailBlendingMode.NORMAL;
        }

        if (_target.IsKeywordEnabled("_FACIAL_TATOO_DARKEN"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.DARKEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_MULTIPLY"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.MULTIPLY;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_BURN"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.COLOR_BURN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_LNEARBURN"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.LINEAR_BURN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_LIGHTEN"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.LIGHTEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_SCREEN"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.SCREEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_DODGE"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.COLOR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_LINEAR_DODGE"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.LINEAR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_OVERLAY"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.OVERLAY;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_SOFTLIGHT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.SOFT_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_HARDLIGHT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.HARD_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_VIVIDLIGHT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.VIVID_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_LINEARLIGHT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_LINEARLIGHTADDSUB"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_PINLIGHT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.PIN_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_HARDMIX"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.HARD_MIX;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_DIFF"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.DIFFERENCE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_EXCLUSION"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.EXCLUSION;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_SUBTRACT"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.SUBTRACT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_DIVIDE"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.DIVIDE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_TATOO_NEGATION"))
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.NEGATION;
        }
        else
        {
            _facialTatooBlendingMode = SkinDetailBlendingMode.NORMAL;
        }

        if (_target.IsKeywordEnabled("_FACIAL_MARKING_DARKEN"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.DARKEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_MULTIPLY"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.MULTIPLY;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_BURN"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.COLOR_BURN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_LNEARBURN"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.LINEAR_BURN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_LIGHTEN"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.LIGHTEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_SCREEN"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.SCREEN;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_DODGE"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.COLOR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_LINEAR_DODGE"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.LINEAR_DODGE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_OVERLAY"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.OVERLAY;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_SOFTLIGHT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.SOFT_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_HARDLIGHT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.HARD_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_VIVIDLIGHT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.VIVID_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_LINEARLIGHT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_LINEARLIGHTADDSUB"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_PINLIGHT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.PIN_LIGHT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_HARDMIX"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.HARD_MIX;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_DIFF"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.DIFFERENCE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_EXCLUSION"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.EXCLUSION;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_SUBTRACT"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.SUBTRACT;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_DIVIDE"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.DIVIDE;
        }
        else if (_target.IsKeywordEnabled("_FACIAL_MARKING_NEGATION"))
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.NEGATION;
        }
        else
        {
            _facialMarkingBlendingMode = SkinDetailBlendingMode.NORMAL;
        }

        _isOpenSurfaceOptions = IsExpanded((uint)Expandable.SurfaceOptions);
        _beforeIsOpenSurfaceOptions = _isOpenSurfaceOptions;
        _isOpenSurfaceInputs = IsExpanded((uint)Expandable.SurfaceInputs);
        _beforeIsOpenSurfaceInputs = _isOpenSurfaceInputs;
        _isOpenLightSettings = IsExpanded((uint)Expandable.LightingSettings);
        _beforeIsOpenLightSettings = _isOpenLightSettings;
        _isOpenAdvancedSettings = IsExpanded((uint)Expandable.Advanced);
        _beforeIsOpenAdvancedSettings = _isOpenAdvancedSettings;

        _headerLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 };
        _warningLabel = new GUIStyle(GUI.skin.label);
        _warningLabel.normal.textColor = Color.yellow;
        _baseMapGUI = new GUIContent("BaseMap");
        _marMapGUI = new GUIContent("MARMap", "Metallic(R) AO(G) Roughness(B)");
        _normalMapGUI = new GUIContent("NormalMap");
        _debugMipmapGUI = new GUIContent("DebugMipmapTexture");
        _matCapMapGUI = new GUIContent("MatCapMap");
        _eyesMakeupMapGUI = new GUIContent("EyesMakeupMap");
        _blushMakeupMapGUI = new GUIContent("BlushMakeupMap");
        _lipsMakeupMapGUI = new GUIContent("LipsMakeupMap");
        _facialTatooMapGUI = new GUIContent("FacialTatooMap");
        _facialMarkingMapGUI = new GUIContent("FacialMarkingMap");
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (_target == null) { _target = materialEditor.target as Material; }

        if (_properties == null)
        {
            _properties = new Properties();
            _properties.baseMap = properties.FirstOrDefault(x => x.name == "_BaseMap");
            _properties.baseMapArray = properties.FirstOrDefault(x => x.name == "_BaseMapArray");
            _properties.baseMapIndex = properties.FirstOrDefault(x => x.name == "_BaseMapIndex");
            _properties.marMap = properties.FirstOrDefault(x => x.name == "_MARMap");
            _properties.marMapArray = properties.FirstOrDefault(x => x.name == "_MARMapArray");
            _properties.marMapIndex = properties.FirstOrDefault(x => x.name == "_MARMapIndex");
            _properties.metallic = properties.FirstOrDefault(x => x.name == "_Metallic");
            _properties.smoothness = properties.FirstOrDefault(x => x.name == "_Smoothness");
            _properties.normalMap = properties.FirstOrDefault(x => x.name == "_NormalMap");
            _properties.normalMapArray = properties.FirstOrDefault(x => x.name == "_NormalMapArray");
            _properties.normalMapIndex = properties.FirstOrDefault(x => x.name == "_NormalMapIndex");
            _properties.normalStrength = properties.FirstOrDefault(x => x.name == "_NormalStrength");
            _properties.occlusion = properties.FirstOrDefault(x => x.name == "_Occlusion");
            _properties.cutoff = properties.FirstOrDefault(x => x.name == "_Cutoff");
            _properties.diffuseColor = properties.FirstOrDefault(x => x.name == "_DiffuseColor");
            _properties.diffuseColor2 = properties.FirstOrDefault(x => x.name == "_DiffuseColor2");
            _properties.eulerLightDirection = properties.FirstOrDefault(x => x.name == "_EulerLightDirection");
            _properties.eulerLightDirection2 = properties.FirstOrDefault(x => x.name == "_EulerLightDirection2");
            _properties.lightDirection = properties.FirstOrDefault(x => x.name == "_LightDirection");
            _properties.lightDirection2 = properties.FirstOrDefault(x => x.name == "_LightDirection2");
            _properties.rimColor = properties.FirstOrDefault(x => x.name == "_RimColor");
            _properties.rimColor2 = properties.FirstOrDefault(x => x.name == "_RimColor2");
            _properties.gradientScale = properties.FirstOrDefault(x => x.name == "_GradientScale");
            _properties.rimPower = properties.FirstOrDefault(x => x.name == "_RimPower");
            _properties.rimPower2 = properties.FirstOrDefault(x => x.name == "_RimPower2");
            _properties.debugMipmapTex = properties.FirstOrDefault(x => x.name == "_DebugMipmapTex");
            _properties.debugMipmapTexArray = properties.FirstOrDefault(x => x.name == "_DebugMipmapTexArray");
            _properties.gradientAngle = properties.FirstOrDefault(x => x.name == "_GradientAngle");
            _properties.gradientPower = properties.FirstOrDefault(x => x.name == "_GradientPower");
            _properties.gradientOffset = properties.FirstOrDefault(x => x.name == "_GradientOffset");
            _properties.diffuseIntensity = properties.FirstOrDefault(x => x.name == "_DiffuseIntensity");
            _properties.diffuseIntensity2 = properties.FirstOrDefault(x => x.name == "_DiffuseIntensity2");
            _properties.rimIntensity = properties.FirstOrDefault(x => x.name == "_RimIntensity");
            _properties.rimIntensity2 = properties.FirstOrDefault(x => x.name == "_RimIntensity2");
            _properties.uvTiles = properties.FirstOrDefault(x => x.name == "_UVTiles");
            _properties.matCapMap = properties.FirstOrDefault(x => x.name == "_MatCapMap");
            _properties.deepLayerColor = properties.FirstOrDefault(x => x.name == "_DeepLayerColor");
            _properties.matCapBlend = properties.FirstOrDefault(x => x.name == "_MatCapBlend");
            _properties.skinColor = properties.FirstOrDefault(x => x.name == "_SkinColor");
            _properties.uvCutoff = properties.FirstOrDefault(x => x.name == "_UVCutoff");
            _properties.matCapHSVShift = properties.FirstOrDefault(x => x.name == "_MatCapHSVShift");
            _properties.anisoOffset = properties.FirstOrDefault(x => x.name == "_AnisoOffset");
            _properties.anisoPower = properties.FirstOrDefault(x => x.name == "_AnisoPower");
            _properties.anisoIntensity = properties.FirstOrDefault(x => x.name == "_AnisoIntensity");
            _properties.hairUVTileIndex = properties.FirstOrDefault(x => x.name == "_HairUVTileIndex");
            _properties.anisoHighlightColor = properties.FirstOrDefault(x => x.name == "_AnisoHighlightColor");
            _properties.eyesMakeupMapArray = properties.FirstOrDefault(x => x.name == "_EyesMakeupMapArray");
            _properties.eyesMakeupMapArrayIndex = properties.FirstOrDefault(x => x.name == "_EyesMakeupMapArrayIndex");
            _properties.eyesMakeupUVTileIndex = properties.FirstOrDefault(x => x.name == "_EyesMakeupUVTileInedx");
            _properties.blushMakeupMapArray = properties.FirstOrDefault(x => x.name == "_BlushMakeupMapArray");
            _properties.blushMakeupMapArrayIndex = properties.FirstOrDefault(x => x.name == "_BlushMakeupMapArrayIndex");
            _properties.blushMakeupUVTileIndex = properties.FirstOrDefault(x => x.name == "_BlushMakeupUVTileInedx");
            _properties.lipsMakeupMapArray = properties.FirstOrDefault(x => x.name == "_LipsMakeupMapArray");
            _properties.lipsMakeupMapArrayIndex = properties.FirstOrDefault(x => x.name == "_LipsMakeupMapArrayIndex");
            _properties.lipsMakeupUVTileIndex = properties.FirstOrDefault(x => x.name == "_LipsMakeupUVTileInedx");
            _properties.facialTatooMapArray = properties.FirstOrDefault(x => x.name == "_FacialTatooMapArray");
            _properties.facialTatooMapArrayIndex = properties.FirstOrDefault(x => x.name == "_FacialTatooMapArrayIndex");
            _properties.facialTatooUVTileIndex = properties.FirstOrDefault(x => x.name == "_FacialTatooUVTileInedx");
            _properties.facialMarkingMapArray = properties.FirstOrDefault(x => x.name == "_FacialMarkingMapArray");
            _properties.facialMarkingMapArrayIndex = properties.FirstOrDefault(x => x.name == "_FacialMarkingMapArrayIndex");
            _properties.facialMarkingUVTileIndex = properties.FirstOrDefault(x => x.name == "_FacialMarkingUVTileInedx");
            _properties.mouthScale = properties.FirstOrDefault(x => x.name == "_MouthScale");
            _properties.mouthAOPower = properties.FirstOrDefault(x => x.name == "_MouthAOPower");
            _properties.mouthUVTileIndex = properties.FirstOrDefault(x => x.name == "_MouthUVTileIndex");
            _properties.mouthAOIntensity = properties.FirstOrDefault(x => x.name == "_MouthAOIntensity");
            _properties.clearCoatMask = properties.FirstOrDefault(x => x.name == "_ClearCoatMask");
            _properties.clearCoatSmoothness = properties.FirstOrDefault(x => x.name == "_ClearCoatSmoothness");
            _properties.hairColor1 = properties.FirstOrDefault(x => x.name == "_HairColor1");
            _properties.hairColor2 = properties.FirstOrDefault(x => x.name == "_HairColor2");
        }

        Initialize();

        _enableMARMap = _target.IsKeywordEnabled("_MARMAP");
        _enableNormalMap = _target.IsKeywordEnabled("_NORMALMAP");
        _enableDiffuseColor = _target.IsKeywordEnabled("_DIFFUSE_COLOR");
        _enableReflection = _target.IsKeywordEnabled("_REFLECTION");
        _enableRimLight = _target.IsKeywordEnabled("_RIM_LIGHT");
        _enableTex2DArray = _target.IsKeywordEnabled("_TEX_ARRAY");
        _enableDebugMipmap = _target.IsKeywordEnabled("_DEBUG_MIPMAP");
        _enableGradient = _target.IsKeywordEnabled("_GRADIENT_LIGHT");
        _enableUVTiling = _target.IsKeywordEnabled("_UVTILING");
        _enableObjectSpaceGradient = _target.IsKeywordEnabled("_OBJECT_SPACE_GRADIENT");
        _enableMatCapMap = _target.IsKeywordEnabled("_MATCAPMAP");
        _enableAnisoHighlight = _target.IsKeywordEnabled("_ANISOTROPIC_HIGHLIGHT");
        _enableOneTexture = _target.IsKeywordEnabled("_ONE_TEXTURE");
        _enableEyesMakeupMap = _target.IsKeywordEnabled("_EYES_MAKEUPMAP");
        _enableBlushMakeupMap = _target.IsKeywordEnabled("_BLUSH_MAKEUPMAP");
        _enableLipsMakeupMap = _target.IsKeywordEnabled("_LIPS_MAKEUPMAP");
        _enableFacialTatooMap = _target.IsKeywordEnabled("_FACIAL_TATOOMAP");
        _enableFacialMarkingMap = _target.IsKeywordEnabled("_FACIAL_MARKINGMAP");
        _enableMouthShadow = _target.IsKeywordEnabled("_MOUTH_SHADOW");
        _enableClearCoat = _target.IsKeywordEnabled("_CLEARCOAT");
        _enableHairColor = _target.IsKeywordEnabled("_HAIRCOLOR");

        _isOpenSurfaceOptions = CoreEditorUtils.DrawHeaderFoldout("Surface Options", _isOpenSurfaceOptions);

        if (_beforeIsOpenSurfaceOptions ^ _isOpenSurfaceOptions)
        {
            SetExpanded((uint)Expandable.SurfaceOptions, _isOpenSurfaceOptions);
            _beforeIsOpenSurfaceOptions = _isOpenSurfaceOptions;
        }

        if (_isOpenSurfaceOptions)
        {
            EditorGUI.BeginChangeCheck();
            _renderMode = (RenderMode)EditorGUILayout.EnumPopup("RenderMode", _renderMode);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated render mode");
                UpdateRenderMode();
            }

            EditorGUI.BeginChangeCheck();
            _surfaceType = (SurfaceType)EditorGUILayout.EnumPopup("SurfaceType", _surfaceType);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated render surface type");
                UpdateSurfaceType();
            }

            if (_surfaceType == SurfaceType.Cutout)
            {
                materialEditor.RangeProperty(_properties.cutoff, _properties.cutoff.displayName);
            }
        }

        _isOpenSurfaceInputs = CoreEditorUtils.DrawHeaderFoldout("Surface Inputs", _isOpenSurfaceInputs);

        if (_beforeIsOpenSurfaceInputs ^ _isOpenSurfaceInputs)
        {
            SetExpanded((uint)Expandable.SurfaceInputs, _isOpenSurfaceInputs);
            _beforeIsOpenSurfaceInputs = _isOpenSurfaceInputs;
        }

        if (_isOpenSurfaceInputs)
        {
            EditorGUILayout.LabelField("BaseColor Settings", _headerLabel);
            materialEditor.ColorProperty(_properties.skinColor, "Skin Tint Color");
            EditorGUILayout.Space(5);

            if (_enableTex2DArray)
            {
                materialEditor.TexturePropertySingleLine(_baseMapGUI, _properties.baseMapArray, null);
                materialEditor.IntegerProperty(_properties.baseMapIndex, _properties.baseMapIndex.displayName);
            }
            else
            {
                materialEditor.TexturePropertySingleLine(_baseMapGUI, _properties.baseMap, null);
            }

            if (_renderMode != RenderMode.UNITY_UNLIT)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("SSS Settings", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableMatCapMap = EditorGUILayout.Toggle("MatCapMap", _enableMatCapMap);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated matcap map enabled");

                    if (_enableMatCapMap) { _target.EnableKeyword("_MATCAPMAP"); }
                    else { _target.DisableKeyword("_MATCAPMAP"); }
                }

                if (_enableMatCapMap)
                {
                    materialEditor.TexturePropertySingleLine(_matCapMapGUI, _properties.matCapMap, _properties.deepLayerColor);
                    materialEditor.RangeProperty(_properties.matCapBlend, _properties.matCapBlend.displayName);
                    float h = _properties.matCapHSVShift.vectorValue.x;
                    h = EditorGUILayout.Slider("H", h, 0, 1);
                    float s = _properties.matCapHSVShift.vectorValue.y;
                    s = EditorGUILayout.Slider("S", s, -1, 1);
                    float v = _properties.matCapHSVShift.vectorValue.z;
                    v = EditorGUILayout.Slider("V", v, -1, 1);
                    _properties.matCapHSVShift.vectorValue = new Vector4(h, s, v, 0);
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("MAR Settings", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableMARMap = EditorGUILayout.Toggle("Enable MAR Map", _enableMARMap);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated mar map enabled");

                    if (_enableMARMap) { _target.EnableKeyword("_MARMAP"); }
                    else { _target.DisableKeyword("_MARMAP"); }
                }

                if (_enableMARMap)
                {
                    if (_enableTex2DArray)
                    {
                        materialEditor.TexturePropertySingleLine(_marMapGUI, _properties.marMapArray, null);
                        materialEditor.IntegerProperty(_properties.marMapIndex, _properties.marMapIndex.displayName);
                    }
                    else
                    {
                        materialEditor.TexturePropertySingleLine(_marMapGUI, _properties.marMap, null);
                    }

                    if (_renderMode != RenderMode.UNITY_SIMPLE_LIT)
                    {
                        materialEditor.RangeProperty(_properties.smoothness, "Smoothness Scale");
                        materialEditor.RangeProperty(_properties.metallic, "Metallic Scale");
                    }

                    materialEditor.RangeProperty(_properties.occlusion, "Ambient Occlusion Scale");
                }
                else
                {
                    if (_renderMode != RenderMode.UNITY_SIMPLE_LIT)
                    {
                        materialEditor.RangeProperty(_properties.smoothness, _properties.smoothness.displayName);
                        materialEditor.RangeProperty(_properties.metallic, _properties.metallic.displayName);
                    }

                    materialEditor.RangeProperty(_properties.occlusion, _properties.occlusion.displayName);
                }

                if (_renderMode != RenderMode.UNITY_SIMPLE_LIT)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Normal Settings", _headerLabel);
                    EditorGUI.BeginChangeCheck();
                    _enableNormalMap = EditorGUILayout.Toggle("Enable Normal Map", _enableNormalMap);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_target, "Updated normal map enabled");

                        if (_enableNormalMap) { _target.EnableKeyword("_NORMALMAP"); }
                        else { _target.DisableKeyword("_NORMALMAP"); }
                    }

                    if (_enableNormalMap)
                    {
                        if (_enableTex2DArray)
                        {
                            materialEditor.TexturePropertySingleLine(_normalMapGUI, _properties.normalMapArray, _properties.normalStrength);
                            materialEditor.IntegerProperty(_properties.normalMapIndex, _properties.normalMapIndex.displayName);
                        }
                        else
                        {
                            materialEditor.TexturePropertySingleLine(_normalMapGUI, _properties.normalMap, _properties.normalStrength);
                        }
                    }
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("EyesMakeup Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableEyesMakeupMap = EditorGUILayout.Toggle("EyesMakeup", _enableEyesMakeupMap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated eyesmakeup map enabled");

                if (_enableEyesMakeupMap) { _target.EnableKeyword("_EYES_MAKEUPMAP"); }
                else { _target.DisableKeyword("_EYES_MAKEUPMAP"); }

            }

            if (_enableEyesMakeupMap)
            {
                materialEditor.TexturePropertySingleLine(_eyesMakeupMapGUI, _properties.eyesMakeupMapArray, null);
                materialEditor.IntegerProperty(_properties.eyesMakeupMapArrayIndex, _properties.eyesMakeupMapArrayIndex.displayName);
                EditorGUI.BeginChangeCheck();
                _eyesMakeupMapUV = (SkinDetailMapUV)EditorGUILayout.EnumPopup("UV", _eyesMakeupMapUV);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated uv channel of eyesmakeupmap");

                    switch (_eyesMakeupMapUV)
                    {
                        case SkinDetailMapUV.UV1:
                            _target.EnableKeyword("_EYES_MAKEUP_UV1");
                            _target.DisableKeyword("_EYES_MAKEUP_UV2");
                            _target.DisableKeyword("_EYES_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV2:
                            _target.DisableKeyword("_EYES_MAKEUP_UV1");
                            _target.EnableKeyword("_EYES_MAKEUP_UV2");
                            _target.DisableKeyword("_EYES_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV3:
                            _target.DisableKeyword("_EYES_MAKEUP_UV1");
                            _target.DisableKeyword("_EYES_MAKEUP_UV2");
                            _target.EnableKeyword("_EYES_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV0:
                        default:
                            _target.DisableKeyword("_EYES_MAKEUP_UV1");
                            _target.DisableKeyword("_EYES_MAKEUP_UV2");
                            _target.DisableKeyword("_EYES_MAKEUP_UV3");
                            break;
                    }
                }

                EditorGUI.BeginChangeCheck();
                _eyesMakeupBlendingMode = (SkinDetailBlendingMode)EditorGUILayout.Popup("Blending Mode", (int)_eyesMakeupBlendingMode, new string[]
                {
                    "NORMAL", "DARKEN", "MULTIPLY", "COLOR_BURN", "LINEAR_BURN",
                    "LIGHTEN", "SCREEN", "COLOR_DODGE", "LINEAR_DODGE",
                    "*OVERLAY", "*SOFT_LIGHT", "*HARD_LIGHT", "*VIVID_LIGHT", "LINEAR_LIGHT", "LINEAR_LIGHT_ADD_SUB", "*PIN_LIGHT", "HARD_MIX",
                    "DIFFERENCE", "EXCLUSION", "SUBTRACT", "DIVIDE", "NEGATION"
                });

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated blending mode of eyesmakeup map");
                    ResetEyesMakeupBlendingMode();

                    switch (_eyesMakeupBlendingMode)
                    {
                        case SkinDetailBlendingMode.DARKEN:
                            _target.EnableKeyword("_EYES_MAKEUP_DARKEN");
                            break;
                        case SkinDetailBlendingMode.MULTIPLY:
                            _target.EnableKeyword("_EYES_MAKEUP_MULTIPLY");
                            break;
                        case SkinDetailBlendingMode.COLOR_BURN:
                            _target.EnableKeyword("_EYES_MAKEUP_BURN");
                            break;
                        case SkinDetailBlendingMode.LINEAR_BURN:
                            _target.EnableKeyword("_EYES_MAKEUP_LINEARBURN");
                            break;
                        case SkinDetailBlendingMode.LIGHTEN:
                            _target.EnableKeyword("_EYES_MAKEUP_LIGHTEN");
                            break;
                        case SkinDetailBlendingMode.SCREEN:
                            _target.EnableKeyword("_EYES_MAKEUP_SCREEN");
                            break;
                        case SkinDetailBlendingMode.COLOR_DODGE:
                            _target.EnableKeyword("_EYES_MAKEUP_DODGE");
                            break;
                        case SkinDetailBlendingMode.LINEAR_DODGE:
                            _target.EnableKeyword("_EYES_MAKEUP_LINEARDODGE");
                            break;
                        case SkinDetailBlendingMode.OVERLAY:
                            _target.EnableKeyword("_EYES_MAKEUP_OVERLAY");
                            break;
                        case SkinDetailBlendingMode.SOFT_LIGHT:
                            _target.EnableKeyword("_EYES_MAKEUP_SOFTLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_LIGHT:
                            _target.EnableKeyword("_EYES_MAKEUP_HARDLIGHT");
                            break;
                        case SkinDetailBlendingMode.VIVID_LIGHT:
                            _target.EnableKeyword("_EYES_MAKEUP_VIVIDLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT:
                            _target.EnableKeyword("_EYES_MAKEUP_LINEARLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB:
                            _target.EnableKeyword("_EYES_MAKEUP_LINEARLIGHTADDSUB");
                            break;
                        case SkinDetailBlendingMode.PIN_LIGHT:
                            _target.EnableKeyword("_EYES_MAKEUP_PINLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_MIX:
                            _target.EnableKeyword("_EYES_MAKEUP_HARDMIX");
                            break;
                        case SkinDetailBlendingMode.DIFFERENCE:
                            _target.EnableKeyword("_EYES_MAKEUP_DIFF");
                            break;
                        case SkinDetailBlendingMode.EXCLUSION:
                            _target.EnableKeyword("_EYES_MAKEUP_EXCLUSION");
                            break;
                        case SkinDetailBlendingMode.SUBTRACT:
                            _target.EnableKeyword("_EYES_MAKEUP_SUBTRACT");
                            break;
                        case SkinDetailBlendingMode.NEGATION:
                            _target.EnableKeyword("_EYES_MAKEUP_NEGATION");
                            break;
                        case SkinDetailBlendingMode.NORMAL:
                        default:
                            break;
                    }
                }

                EditorGUILayout.LabelField("*These blending mode is more expensive than others", _warningLabel);

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.eyesMakeupUVTileIndex, _properties.eyesMakeupUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("BlushMakeup Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableBlushMakeupMap = EditorGUILayout.Toggle("BlushMakeup", _enableBlushMakeupMap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated blushmakeup map enabled");

                if (_enableBlushMakeupMap) { _target.EnableKeyword("_BLUSH_MAKEUPMAP"); }
                else { _target.DisableKeyword("_BLUSH_MAKEUPMAP"); }
            }

            if (_enableBlushMakeupMap)
            {
                materialEditor.TexturePropertySingleLine(_blushMakeupMapGUI, _properties.blushMakeupMapArray, null);
                materialEditor.IntegerProperty(_properties.blushMakeupMapArrayIndex, _properties.blushMakeupMapArrayIndex.displayName);
                EditorGUI.BeginChangeCheck();
                _blushMakeupMapUV = (SkinDetailMapUV)EditorGUILayout.EnumPopup("UV", _blushMakeupMapUV);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated uv channel of blushmakeupmap");

                    switch (_blushMakeupMapUV)
                    {
                        case SkinDetailMapUV.UV1:
                            _target.EnableKeyword("_BLUSH_MAKEUP_UV1");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV2");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV2:
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV1");
                            _target.EnableKeyword("_BLUSH_MAKEUP_UV2");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV3:
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV1");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV2");
                            _target.EnableKeyword("_BLUSH_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV0:
                        default:
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV1");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV2");
                            _target.DisableKeyword("_BLUSH_MAKEUP_UV3");
                            break;
                    }
                }

                EditorGUI.BeginChangeCheck();
                _blushMakeupBlendingMode = (SkinDetailBlendingMode)EditorGUILayout.Popup("Blending Mode", (int)_blushMakeupBlendingMode, new string[]
                {
                    "NORMAL", "DARKEN", "MULTIPLY", "COLOR_BURN", "LINEAR_BURN",
                    "LIGHTEN", "SCREEN", "COLOR_DODGE", "LINEAR_DODGE",
                    "*OVERLAY", "*SOFT_LIGHT", "*HARD_LIGHT", "*VIVID_LIGHT", "LINEAR_LIGHT", "LINEAR_LIGHT_ADD_SUB", "*PIN_LIGHT", "HARD_MIX",
                    "DIFFERENCE", "EXCLUSION", "SUBTRACT", "DIVIDE", "NEGATION"
                });

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated blending mode of blushmakeup map");
                    ResetBlushMakeupBlendingMode();

                    switch (_blushMakeupBlendingMode)
                    {
                        case SkinDetailBlendingMode.DARKEN:
                            _target.EnableKeyword("_BLUSH_MAKEUP_DARKEN");
                            break;
                        case SkinDetailBlendingMode.MULTIPLY:
                            _target.EnableKeyword("_BLUSH_MAKEUP_MULTIPLY");
                            break;
                        case SkinDetailBlendingMode.COLOR_BURN:
                            _target.EnableKeyword("_BLUSH_MAKEUP_BURN");
                            break;
                        case SkinDetailBlendingMode.LINEAR_BURN:
                            _target.EnableKeyword("_BLUSH_MAKEUP_LINEARBURN");
                            break;
                        case SkinDetailBlendingMode.LIGHTEN:
                            _target.EnableKeyword("_BLUSH_MAKEUP_LIGHTEN");
                            break;
                        case SkinDetailBlendingMode.SCREEN:
                            _target.EnableKeyword("_BLUSH_MAKEUP_SCREEN");
                            break;
                        case SkinDetailBlendingMode.COLOR_DODGE:
                            _target.EnableKeyword("_BLUSH_MAKEUP_DODGE");
                            break;
                        case SkinDetailBlendingMode.LINEAR_DODGE:
                            _target.EnableKeyword("_BLUSH_MAKEUP_LINEARDODGE");
                            break;
                        case SkinDetailBlendingMode.OVERLAY:
                            _target.EnableKeyword("_BLUSH_MAKEUP_OVERLAY");
                            break;
                        case SkinDetailBlendingMode.SOFT_LIGHT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_SOFTLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_LIGHT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_HARDLIGHT");
                            break;
                        case SkinDetailBlendingMode.VIVID_LIGHT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_VIVIDLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_LINEARLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB:
                            _target.EnableKeyword("_BLUSH_MAKEUP_LINEARLIGHTADDSUB");
                            break;
                        case SkinDetailBlendingMode.PIN_LIGHT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_PINLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_MIX:
                            _target.EnableKeyword("_BLUSH_MAKEUP_HARDMIX");
                            break;
                        case SkinDetailBlendingMode.DIFFERENCE:
                            _target.EnableKeyword("_BLUSH_MAKEUP_DIFF");
                            break;
                        case SkinDetailBlendingMode.EXCLUSION:
                            _target.EnableKeyword("_BLUSH_MAKEUP_EXCLUSION");
                            break;
                        case SkinDetailBlendingMode.SUBTRACT:
                            _target.EnableKeyword("_BLUSH_MAKEUP_SUBTRACT");
                            break;
                        case SkinDetailBlendingMode.NEGATION:
                            _target.EnableKeyword("_BLUSH_MAKEUP_NEGATION");
                            break;
                        case SkinDetailBlendingMode.NORMAL:
                        default:
                            break;
                    }
                }

                EditorGUILayout.LabelField("*These blending mode is more expensive than others", _warningLabel);

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.blushMakeupUVTileIndex, _properties.blushMakeupUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("LipsMakeup Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableLipsMakeupMap = EditorGUILayout.Toggle("LipsMakeup", _enableLipsMakeupMap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated lipsmakeup map enabled");

                if (_enableLipsMakeupMap) { _target.EnableKeyword("_LIPS_MAKEUPMAP"); }
                else { _target.DisableKeyword("_LIPS_MAKEUPMAP"); }

            }

            if (_enableLipsMakeupMap)
            {
                materialEditor.TexturePropertySingleLine(_lipsMakeupMapGUI, _properties.lipsMakeupMapArray, null);
                materialEditor.IntegerProperty(_properties.lipsMakeupMapArrayIndex, _properties.lipsMakeupMapArrayIndex.displayName);
                EditorGUI.BeginChangeCheck();
                _lipsMakeupMapUV = (SkinDetailMapUV)EditorGUILayout.EnumPopup("UV", _lipsMakeupMapUV);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated uv channel of lipsmakeupmap");

                    switch (_lipsMakeupMapUV)
                    {
                        case SkinDetailMapUV.UV1:
                            _target.EnableKeyword("_LIPS_MAKEUP_UV1");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV2");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV2:
                            _target.DisableKeyword("_LIPS_MAKEUP_UV1");
                            _target.EnableKeyword("_LIPS_MAKEUP_UV2");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV3:
                            _target.DisableKeyword("_LIPS_MAKEUP_UV1");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV2");
                            _target.EnableKeyword("_LIPS_MAKEUP_UV3");
                            break;
                        case SkinDetailMapUV.UV0:
                        default:
                            _target.DisableKeyword("_LIPS_MAKEUP_UV1");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV2");
                            _target.DisableKeyword("_LIPS_MAKEUP_UV3");
                            break;
                    }
                }

                EditorGUI.BeginChangeCheck();
                _lipsMakeupBlendingMode = (SkinDetailBlendingMode)EditorGUILayout.Popup("Blending Mode", (int)_lipsMakeupBlendingMode, new string[]
                {
                    "NORMAL", "DARKEN", "MULTIPLY", "COLOR_BURN", "LINEAR_BURN",
                    "LIGHTEN", "SCREEN", "COLOR_DODGE", "LINEAR_DODGE",
                    "*OVERLAY", "*SOFT_LIGHT", "*HARD_LIGHT", "*VIVID_LIGHT", "LINEAR_LIGHT", "LINEAR_LIGHT_ADD_SUB", "*PIN_LIGHT", "HARD_MIX",
                    "DIFFERENCE", "EXCLUSION", "SUBTRACT", "DIVIDE", "NEGATION"
                });

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated blending mode of lipsmakeup map");
                    ResetLipsMakeupBlendingMode();

                    switch (_lipsMakeupBlendingMode)
                    {
                        case SkinDetailBlendingMode.DARKEN:
                            _target.EnableKeyword("_LIPS_MAKEUP_DARKEN");
                            break;
                        case SkinDetailBlendingMode.MULTIPLY:
                            _target.EnableKeyword("_LIPS_MAKEUP_MULTIPLY");
                            break;
                        case SkinDetailBlendingMode.COLOR_BURN:
                            _target.EnableKeyword("_LIPS_MAKEUP_BURN");
                            break;
                        case SkinDetailBlendingMode.LINEAR_BURN:
                            _target.EnableKeyword("_LIPS_MAKEUP_LINEARBURN");
                            break;
                        case SkinDetailBlendingMode.LIGHTEN:
                            _target.EnableKeyword("_LIPS_MAKEUP_LIGHTEN");
                            break;
                        case SkinDetailBlendingMode.SCREEN:
                            _target.EnableKeyword("_LIPS_MAKEUP_SCREEN");
                            break;
                        case SkinDetailBlendingMode.COLOR_DODGE:
                            _target.EnableKeyword("_LIPS_MAKEUP_DODGE");
                            break;
                        case SkinDetailBlendingMode.LINEAR_DODGE:
                            _target.EnableKeyword("_LIPS_MAKEUP_LINEARDODGE");
                            break;
                        case SkinDetailBlendingMode.OVERLAY:
                            _target.EnableKeyword("_LIPS_MAKEUP_OVERLAY");
                            break;
                        case SkinDetailBlendingMode.SOFT_LIGHT:
                            _target.EnableKeyword("_LIPS_MAKEUP_SOFTLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_LIGHT:
                            _target.EnableKeyword("_LIPS_MAKEUP_HARDLIGHT");
                            break;
                        case SkinDetailBlendingMode.VIVID_LIGHT:
                            _target.EnableKeyword("_LIPS_MAKEUP_VIVIDLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT:
                            _target.EnableKeyword("_LIPS_MAKEUP_LINEARLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB:
                            _target.EnableKeyword("_LIPS_MAKEUP_LINEARLIGHTADDSUB");
                            break;
                        case SkinDetailBlendingMode.PIN_LIGHT:
                            _target.EnableKeyword("_LIPS_MAKEUP_PINLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_MIX:
                            _target.EnableKeyword("_LIPS_MAKEUP_HARDMIX");
                            break;
                        case SkinDetailBlendingMode.DIFFERENCE:
                            _target.EnableKeyword("_LIPS_MAKEUP_DIFF");
                            break;
                        case SkinDetailBlendingMode.EXCLUSION:
                            _target.EnableKeyword("_LIPS_MAKEUP_EXCLUSION");
                            break;
                        case SkinDetailBlendingMode.SUBTRACT:
                            _target.EnableKeyword("_LIPS_MAKEUP_SUBTRACT");
                            break;
                        case SkinDetailBlendingMode.NEGATION:
                            _target.EnableKeyword("_LIPS_MAKEUP_NEGATION");
                            break;
                        case SkinDetailBlendingMode.NORMAL:
                        default:
                            break;
                    }
                }

                EditorGUILayout.LabelField("*These blending mode is more expensive than others", _warningLabel);

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.lipsMakeupUVTileIndex, _properties.lipsMakeupUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("FacialTatoo Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableFacialTatooMap = EditorGUILayout.Toggle("FacialTatoo", _enableFacialTatooMap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated facialtatoo map enabled");

                if (_enableFacialTatooMap) { _target.EnableKeyword("_FACIAL_TATOOMAP"); }
                else { _target.DisableKeyword("_FACIAL_TATOOMAP"); }

            }

            if (_enableFacialTatooMap)
            {
                materialEditor.TexturePropertySingleLine(_facialTatooMapGUI, _properties.facialTatooMapArray, null);
                materialEditor.IntegerProperty(_properties.facialTatooMapArrayIndex, _properties.facialTatooMapArrayIndex.displayName);
                EditorGUI.BeginChangeCheck();
                _facialTatooMapUV = (SkinDetailMapUV)EditorGUILayout.EnumPopup("UV", _facialTatooMapUV);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated uv channel of facialTatoomap");

                    switch (_facialTatooMapUV)
                    {
                        case SkinDetailMapUV.UV1:
                            _target.EnableKeyword("_FACIAL_TATOO_UV1");
                            _target.DisableKeyword("_FACIAL_TATOO_UV2");
                            _target.DisableKeyword("_FACIAL_TATOO_UV3");
                            break;
                        case SkinDetailMapUV.UV2:
                            _target.DisableKeyword("_FACIAL_TATOO_UV1");
                            _target.EnableKeyword("_FACIAL_TATOO_UV2");
                            _target.DisableKeyword("_FACIAL_TATOO_UV3");
                            break;
                        case SkinDetailMapUV.UV3:
                            _target.DisableKeyword("_FACIAL_TATOO_UV1");
                            _target.DisableKeyword("_FACIAL_TATOO_UV2");
                            _target.EnableKeyword("_FACIAL_TATOO_UV3");
                            break;
                        case SkinDetailMapUV.UV0:
                        default:
                            _target.DisableKeyword("_FACIAL_TATOO_UV1");
                            _target.DisableKeyword("_FACIAL_TATOO_UV2");
                            _target.DisableKeyword("_FACIAL_TATOO_UV3");
                            break;
                    }
                }

                EditorGUI.BeginChangeCheck();
                _facialTatooBlendingMode = (SkinDetailBlendingMode)EditorGUILayout.Popup("Blending Mode", (int)_facialTatooBlendingMode, new string[]
                {
                    "NORMAL", "DARKEN", "MULTIPLY", "COLOR_BURN", "LINEAR_BURN",
                    "LIGHTEN", "SCREEN", "COLOR_DODGE", "LINEAR_DODGE",
                    "*OVERLAY", "*SOFT_LIGHT", "*HARD_LIGHT", "*VIVID_LIGHT", "LINEAR_LIGHT", "LINEAR_LIGHT_ADD_SUB", "*PIN_LIGHT", "HARD_MIX",
                    "DIFFERENCE", "EXCLUSION", "SUBTRACT", "DIVIDE", "NEGATION"
                });

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated blending mode of facialTatoo map");
                    ResetFacialTatooBlendingMode();

                    switch (_facialTatooBlendingMode)
                    {
                        case SkinDetailBlendingMode.DARKEN:
                            _target.EnableKeyword("_FACIAL_TATOO_DARKEN");
                            break;
                        case SkinDetailBlendingMode.MULTIPLY:
                            _target.EnableKeyword("_FACIAL_TATOO_MULTIPLY");
                            break;
                        case SkinDetailBlendingMode.COLOR_BURN:
                            _target.EnableKeyword("_FACIAL_TATOO_BURN");
                            break;
                        case SkinDetailBlendingMode.LINEAR_BURN:
                            _target.EnableKeyword("_FACIAL_TATOO_LINEARBURN");
                            break;
                        case SkinDetailBlendingMode.LIGHTEN:
                            _target.EnableKeyword("_FACIAL_TATOO_LIGHTEN");
                            break;
                        case SkinDetailBlendingMode.SCREEN:
                            _target.EnableKeyword("_FACIAL_TATOO_SCREEN");
                            break;
                        case SkinDetailBlendingMode.COLOR_DODGE:
                            _target.EnableKeyword("_FACIAL_TATOO_DODGE");
                            break;
                        case SkinDetailBlendingMode.LINEAR_DODGE:
                            _target.EnableKeyword("_FACIAL_TATOO_LINEARDODGE");
                            break;
                        case SkinDetailBlendingMode.OVERLAY:
                            _target.EnableKeyword("_FACIAL_TATOO_OVERLAY");
                            break;
                        case SkinDetailBlendingMode.SOFT_LIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_SOFTLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_LIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_HARDLIGHT");
                            break;
                        case SkinDetailBlendingMode.VIVID_LIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_VIVIDLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_LINEARLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB:
                            _target.EnableKeyword("_FACIAL_TATOO_LINEARLIGHTADDSUB");
                            break;
                        case SkinDetailBlendingMode.PIN_LIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_PINLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_MIX:
                            _target.EnableKeyword("_FACIAL_TATOO_HARDMIX");
                            break;
                        case SkinDetailBlendingMode.DIFFERENCE:
                            _target.EnableKeyword("_FACIAL_TATOO_DIFF");
                            break;
                        case SkinDetailBlendingMode.EXCLUSION:
                            _target.EnableKeyword("_FACIAL_TATOO_EXCLUSION");
                            break;
                        case SkinDetailBlendingMode.SUBTRACT:
                            _target.EnableKeyword("_FACIAL_TATOO_SUBTRACT");
                            break;
                        case SkinDetailBlendingMode.NEGATION:
                            _target.EnableKeyword("_FACIAL_TATOO_NEGATION");
                            break;
                        case SkinDetailBlendingMode.NORMAL:
                        default:
                            break;
                    }
                }

                EditorGUILayout.LabelField("*These blending mode is more expensive than others", _warningLabel);

                EditorGUI.BeginChangeCheck();
                _facialTatooAsymmetry = (AsymmetricStamping)EditorGUILayout.EnumPopup("AsymmetricFacialTatoo", _facialTatooAsymmetry);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated asymmeric facial tatoo");

                    switch (_facialTatooAsymmetry)
                    {
                        case AsymmetricStamping.LEFT:
                            _target.EnableKeyword("_FACIAL_TATOO_LEFT");
                            _target.DisableKeyword("_FACIAL_TATOO_RIGHT");
                            break;
                        case AsymmetricStamping.RIGHT:
                            _target.EnableKeyword("_FACIAL_TATOO_RIGHT");
                            _target.DisableKeyword("_FACIAL_TATOO_LEFT");
                            break;
                        case AsymmetricStamping.BOTH:
                            _target.DisableKeyword("_FACIAL_TATOO_LEFT");
                            _target.DisableKeyword("_FACIAL_TATOO_RIGHT");
                            break;
                    }
                }

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.facialTatooUVTileIndex, _properties.facialTatooUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("FacialMarking Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableFacialMarkingMap = EditorGUILayout.Toggle("FacialMarking", _enableFacialMarkingMap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated facialmarking map enabled");

                if (_enableFacialMarkingMap) { _target.EnableKeyword("_FACIAL_MARKINGMAP"); }
                else { _target.DisableKeyword("_FACIAL_MARKINGMAP"); }

            }

            if (_enableFacialMarkingMap)
            {
                materialEditor.TexturePropertySingleLine(_facialMarkingMapGUI, _properties.facialMarkingMapArray, null);
                materialEditor.IntegerProperty(_properties.facialMarkingMapArrayIndex, _properties.facialMarkingMapArrayIndex.displayName);
                EditorGUI.BeginChangeCheck();
                _facialMarkingMapUV = (SkinDetailMapUV)EditorGUILayout.EnumPopup("UV", _facialMarkingMapUV);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated uv channel of facialMarkingmap");

                    switch (_facialMarkingMapUV)
                    {
                        case SkinDetailMapUV.UV1:
                            _target.EnableKeyword("_FACIAL_MARKING_UV1");
                            _target.DisableKeyword("_FACIAL_MARKING_UV2");
                            _target.DisableKeyword("_FACIAL_MARKING_UV3");
                            break;
                        case SkinDetailMapUV.UV2:
                            _target.DisableKeyword("_FACIAL_MARKING_UV1");
                            _target.EnableKeyword("_FACIAL_MARKING_UV2");
                            _target.DisableKeyword("_FACIAL_MARKING_UV3");
                            break;
                        case SkinDetailMapUV.UV3:
                            _target.DisableKeyword("_FACIAL_MARKING_UV1");
                            _target.DisableKeyword("_FACIAL_MARKING_UV2");
                            _target.EnableKeyword("_FACIAL_MARKING_UV3");
                            break;
                        case SkinDetailMapUV.UV0:
                        default:
                            _target.DisableKeyword("_FACIAL_MARKING_UV1");
                            _target.DisableKeyword("_FACIAL_MARKING_UV2");
                            _target.DisableKeyword("_FACIAL_MARKING_UV3");
                            break;
                    }
                }

                EditorGUI.BeginChangeCheck();
                _facialMarkingBlendingMode = (SkinDetailBlendingMode)EditorGUILayout.Popup("Blending Mode", (int)_facialMarkingBlendingMode, new string[]
                {
                    "NORMAL", "DARKEN", "MULTIPLY", "COLOR_BURN", "LINEAR_BURN",
                    "LIGHTEN", "SCREEN", "COLOR_DODGE", "LINEAR_DODGE",
                    "*OVERLAY", "*SOFT_LIGHT", "*HARD_LIGHT", "*VIVID_LIGHT", "LINEAR_LIGHT", "LINEAR_LIGHT_ADD_SUB", "*PIN_LIGHT", "HARD_MIX",
                    "DIFFERENCE", "EXCLUSION", "SUBTRACT", "DIVIDE", "NEGATION"
                });

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated blending mode of facialMarking map");
                    ResetFacialMarkingBlendingMode();

                    switch (_facialMarkingBlendingMode)
                    {
                        case SkinDetailBlendingMode.DARKEN:
                            _target.EnableKeyword("_FACIAL_MARKING_DARKEN");
                            break;
                        case SkinDetailBlendingMode.MULTIPLY:
                            _target.EnableKeyword("_FACIAL_MARKING_MULTIPLY");
                            break;
                        case SkinDetailBlendingMode.COLOR_BURN:
                            _target.EnableKeyword("_FACIAL_MARKING_BURN");
                            break;
                        case SkinDetailBlendingMode.LINEAR_BURN:
                            _target.EnableKeyword("_FACIAL_MARKING_LINEARBURN");
                            break;
                        case SkinDetailBlendingMode.LIGHTEN:
                            _target.EnableKeyword("_FACIAL_MARKING_LIGHTEN");
                            break;
                        case SkinDetailBlendingMode.SCREEN:
                            _target.EnableKeyword("_FACIAL_MARKING_SCREEN");
                            break;
                        case SkinDetailBlendingMode.COLOR_DODGE:
                            _target.EnableKeyword("_FACIAL_MARKING_DODGE");
                            break;
                        case SkinDetailBlendingMode.LINEAR_DODGE:
                            _target.EnableKeyword("_FACIAL_MARKING_LINEARDODGE");
                            break;
                        case SkinDetailBlendingMode.OVERLAY:
                            _target.EnableKeyword("_FACIAL_MARKING_OVERLAY");
                            break;
                        case SkinDetailBlendingMode.SOFT_LIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_SOFTLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_LIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_HARDLIGHT");
                            break;
                        case SkinDetailBlendingMode.VIVID_LIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_VIVIDLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_LINEARLIGHT");
                            break;
                        case SkinDetailBlendingMode.LINEAR_LIGHT_ADD_SUB:
                            _target.EnableKeyword("_FACIAL_MARKING_LINEARLIGHTADDSUB");
                            break;
                        case SkinDetailBlendingMode.PIN_LIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_PINLIGHT");
                            break;
                        case SkinDetailBlendingMode.HARD_MIX:
                            _target.EnableKeyword("_FACIAL_MARKING_HARDMIX");
                            break;
                        case SkinDetailBlendingMode.DIFFERENCE:
                            _target.EnableKeyword("_FACIAL_MARKING_DIFF");
                            break;
                        case SkinDetailBlendingMode.EXCLUSION:
                            _target.EnableKeyword("_FACIAL_MARKING_EXCLUSION");
                            break;
                        case SkinDetailBlendingMode.SUBTRACT:
                            _target.EnableKeyword("_FACIAL_MARKING_SUBTRACT");
                            break;
                        case SkinDetailBlendingMode.NEGATION:
                            _target.EnableKeyword("_FACIAL_MARKING_NEGATION");
                            break;
                        case SkinDetailBlendingMode.NORMAL:
                        default:
                            break;
                    }
                }

                EditorGUILayout.LabelField("*These blending mode is more expensive than others", _warningLabel);

                EditorGUI.BeginChangeCheck();
                _facialMarkingAsymmetry = (AsymmetricStamping)EditorGUILayout.EnumPopup("AsymmetricFacialMarking", _facialMarkingAsymmetry);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated asymmeric facial marking");

                    switch (_facialMarkingAsymmetry)
                    {
                        case AsymmetricStamping.LEFT:
                            _target.EnableKeyword("_FACIAL_MARKING_LEFT");
                            _target.DisableKeyword("_FACIAL_MARKING_RIGHT");
                            break;
                        case AsymmetricStamping.RIGHT:
                            _target.EnableKeyword("_FACIAL_MARKING_RIGHT");
                            _target.DisableKeyword("_FACIAL_MARKING_LEFT");
                            break;
                        case AsymmetricStamping.BOTH:
                            _target.DisableKeyword("_FACIAL_MARKING_LEFT");
                            _target.DisableKeyword("_FACIAL_MARKING_RIGHT");
                            break;
                    }
                }

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.facialMarkingUVTileIndex, _properties.facialMarkingUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Mouth Settings", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableClearCoat = EditorGUILayout.Toggle("Clear Coat", _enableClearCoat);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated clearcoat enabled");

                if (_enableClearCoat) { _target.EnableKeyword("_CLEARCOAT"); }
                else { _target.DisableKeyword("_CLEARCOAT"); }
            }

            if (_enableClearCoat)
            {
                materialEditor.RangeProperty(_properties.clearCoatMask, _properties.clearCoatMask.displayName);
                materialEditor.RangeProperty(_properties.clearCoatSmoothness, _properties.clearCoatSmoothness.displayName);

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.mouthUVTileIndex, _properties.mouthUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Hair Color", _headerLabel);
            EditorGUI.BeginChangeCheck();
            _enableHairColor = EditorGUILayout.Toggle("Hair Color", _enableHairColor);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated hair color enabled");

                if (_enableHairColor) { _target.EnableKeyword("_HAIRCOLOR"); }
                else { _target.DisableKeyword("_HAIRCOLOR"); }
            }

            if (_enableHairColor)
            {
                materialEditor.ColorProperty(_properties.hairColor1, _properties.hairColor1.displayName);
                materialEditor.ColorProperty(_properties.hairColor2, _properties.hairColor2.displayName);

                if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.hairUVTileIndex, _properties.hairUVTileIndex.displayName); }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Common Settings", _headerLabel);
            EditorGUILayout.LabelField("UV0");

            if (_enableTex2DArray) { materialEditor.TextureScaleOffsetProperty(_properties.baseMapArray); }
            else { materialEditor.TextureScaleOffsetProperty(_properties.baseMap); }

            if (_eyesMakeupMapUV == SkinDetailMapUV.UV1 || _blushMakeupMapUV == SkinDetailMapUV.UV1 || _lipsMakeupMapUV == SkinDetailMapUV.UV1)
            {
                EditorGUILayout.LabelField("UV1");
                materialEditor.TextureScaleOffsetProperty(_properties.eyesMakeupMapArray);
            }

            if (_eyesMakeupMapUV == SkinDetailMapUV.UV2 || _blushMakeupMapUV == SkinDetailMapUV.UV2 || _lipsMakeupMapUV == SkinDetailMapUV.UV2)
            {
                EditorGUILayout.LabelField("UV2");
                materialEditor.TextureScaleOffsetProperty(_properties.blushMakeupMapArray);
            }

            if (_eyesMakeupMapUV == SkinDetailMapUV.UV3 || _blushMakeupMapUV == SkinDetailMapUV.UV3 || _lipsMakeupMapUV == SkinDetailMapUV.UV3)
            {
                EditorGUILayout.LabelField("UV3");
                materialEditor.TextureScaleOffsetProperty(_properties.lipsMakeupMapArray);
            }
        }

        if (_renderMode == RenderMode.CUSTOM_LIT)
        {
            _isOpenLightSettings = CoreEditorUtils.DrawHeaderFoldout("Lighting Settings", _isOpenLightSettings);

            if (_beforeIsOpenLightSettings ^ _isOpenLightSettings)
            {
                SetExpanded((uint)Expandable.LightingSettings, _isOpenLightSettings);
                _beforeIsOpenLightSettings = _isOpenLightSettings;
            }

            if (_isOpenLightSettings)
            {
                EditorGUILayout.LabelField("Gradient Settings", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableGradient = EditorGUILayout.Toggle("Enable Gradient", _enableGradient);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated gradient enabled");

                    if (_enableGradient) { _target.EnableKeyword("_GRADIENT_LIGHT"); }
                    else { _target.DisableKeyword("_GRADIENT_LIGHT"); }
                }

                if (_enableGradient)
                {
                    EditorGUI.BeginChangeCheck();
                    _enableObjectSpaceGradient = EditorGUILayout.Toggle("ObjectSpace", _enableObjectSpaceGradient);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_target, "Updated object space enabled");

                        if (_enableObjectSpaceGradient) { _target.EnableKeyword("_OBJECT_SPACE_GRADIENT"); }
                        else { _target.DisableKeyword("_OBJECT_SPACE_GRADIENT"); }
                    }
                    {
                        var xy = new Vector2(_properties.gradientScale.vectorValue.x, _properties.gradientScale.vectorValue.y);
                        var zw = new Vector2(_properties.gradientScale.vectorValue.z, _properties.gradientScale.vectorValue.w);
                        xy = EditorGUILayout.Vector2Field("gradient scale (min coord)", xy);
                        zw = EditorGUILayout.Vector2Field("gradient scale (max coord)", zw);
                        _properties.gradientScale.vectorValue = new Vector4(xy.x, xy.y, zw.x, zw.y);
                    }
                    materialEditor.RangeProperty(_properties.gradientOffset, _properties.gradientOffset.displayName);

                    EditorGUI.BeginChangeCheck();
                    _gradientStyle = (GradientStyle)EditorGUILayout.EnumPopup("Gradient Style", _gradientStyle);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_target, "Updated gradient style");

                        switch (_gradientStyle)
                        {
                            case GradientStyle.RADIAL:
                                _target.EnableKeyword("_RADIAL_GRADIENT_LIGHT");
                                break;
                            default:
                                _target.DisableKeyword("_RADIAL_GRADIENT_LIGHT");
                                break;
                        }
                    }

                    if (_gradientStyle != GradientStyle.RADIAL)
                    {
                        materialEditor.RangeProperty(_properties.gradientAngle, _properties.gradientAngle.displayName);
                    }

                    EditorGUI.BeginChangeCheck();
                    _gradientMode = (GradientMode)EditorGUILayout.EnumPopup("Gradient Mode", _gradientMode);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_target, "Updated gradient mode");

                        switch (_gradientMode)
                        {
                            case GradientMode.EXPONENTIAL:
                                _target.EnableKeyword("_EXP_GRADIENT_MODE");
                                break;
                            default:
                                _target.DisableKeyword("_EXP_GRADIENT_MODE");
                                break;
                        }
                    }

                    if (_gradientMode == GradientMode.EXPONENTIAL)
                    {
                        materialEditor.RangeProperty(_properties.gradientPower, _properties.gradientPower.displayName);
                    }

                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("MainLight Settings", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableDiffuseColor = EditorGUILayout.Toggle("Enable Diffuse Color", _enableDiffuseColor);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated diffuse color enabled");

                    if (_enableDiffuseColor) { _target.EnableKeyword("_DIFFUSE_COLOR"); }
                    else { _target.DisableKeyword("_DIFFUSE_COLOR"); }
                }

                if (_enableDiffuseColor)
                {
                    EditorGUI.BeginChangeCheck();
                    _lightingModel = (LightingModel)EditorGUILayout.EnumPopup("Lighting Model", _lightingModel);

                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (_lightingModel)
                        {
                            case LightingModel.HALF_LAMBERT:
                                _target.EnableKeyword("_HALF_LAMBERT");
                                _target.DisableKeyword("_MINNAERT");
                                break;
                            case LightingModel.MINNAERT:
                                _target.DisableKeyword("_HALF_LAMBERT");
                                _target.EnableKeyword("_MINNAERT");
                                break;
                            case LightingModel.LAMBERT:
                            default:
                                _target.DisableKeyword("_HALF_LAMBERT");
                                _target.DisableKeyword("_MINNAERT");
                                break;
                        }
                    }

                    if (_enableGradient)
                    {
                        EditorGUILayout.LabelField("MainLight #1");
                        EditorGUI.indentLevel++;
                        materialEditor.ColorProperty(_properties.diffuseColor, _properties.diffuseColor.displayName);
                        {
                            _properties.eulerLightDirection.vectorValue = EditorGUILayout.Vector2Field("Light Direction", _properties.eulerLightDirection.vectorValue);
                            var dir = _properties.eulerLightDirection.vectorValue;
                            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(dir.x, dir.y, dir.z), Vector3.one);
                            var lightpos = -matrix.GetColumn(2);
                            _properties.lightDirection.vectorValue = new Vector4(lightpos.x, lightpos.y, lightpos.z, 0);
                        }
                        materialEditor.RangeProperty(_properties.diffuseIntensity, _properties.diffuseIntensity.displayName);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.LabelField("MainLight #2");
                        EditorGUI.indentLevel++;
                        materialEditor.ColorProperty(_properties.diffuseColor2, _properties.diffuseColor2.displayName);
                        {
                            _properties.eulerLightDirection2.vectorValue = EditorGUILayout.Vector2Field("Light Direction2", _properties.eulerLightDirection2.vectorValue);
                            var dir = _properties.eulerLightDirection2.vectorValue;
                            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(dir.x, dir.y, dir.z), Vector3.one);
                            var lightpos = -matrix.GetColumn(2);
                            _properties.lightDirection2.vectorValue = new Vector4(lightpos.x, lightpos.y, lightpos.z, 0);
                        }
                        materialEditor.RangeProperty(_properties.diffuseIntensity2, _properties.diffuseIntensity2.displayName);
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        materialEditor.ColorProperty(_properties.diffuseColor, _properties.diffuseColor.displayName);
                        _properties.diffuseColor2.colorValue = _properties.diffuseColor.colorValue;
                        _properties.eulerLightDirection.vectorValue = EditorGUILayout.Vector2Field("Light Direction", _properties.eulerLightDirection.vectorValue);
                        var dir = _properties.eulerLightDirection.vectorValue;
                        var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(dir.x, dir.y, dir.z), Vector3.one);
                        var lightpos = -matrix.GetColumn(2);
                        _properties.lightDirection.vectorValue = new Vector4(lightpos.x, lightpos.y, lightpos.z, 0);
                        materialEditor.RangeProperty(_properties.diffuseIntensity, _properties.diffuseIntensity.displayName);
                    }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("RimLight Settings", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableRimLight = EditorGUILayout.Toggle("Enable Rim Light", _enableRimLight);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated rim light enabled");

                    if (_enableRimLight) { _target.EnableKeyword("_RIM_LIGHT"); }
                    else { _target.DisableKeyword("_RIM_LIGHT"); }
                }

                if (_enableRimLight)
                {
                    if (_enableGradient)
                    {
                        EditorGUILayout.LabelField("RimLight #1");
                        EditorGUI.indentLevel++;
                        materialEditor.ColorProperty(_properties.rimColor, _properties.rimColor.displayName);
                        materialEditor.RangeProperty(_properties.rimPower, _properties.rimPower.displayName);
                        materialEditor.RangeProperty(_properties.rimIntensity, _properties.rimIntensity.displayName);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.LabelField("RimLight #2");
                        EditorGUI.indentLevel++;
                        materialEditor.ColorProperty(_properties.rimColor2, _properties.rimColor2.displayName);
                        materialEditor.RangeProperty(_properties.rimPower2, _properties.rimPower2.displayName);
                        materialEditor.RangeProperty(_properties.rimIntensity2, _properties.rimIntensity2.displayName);
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        materialEditor.ColorProperty(_properties.rimColor, _properties.rimColor.displayName);
                        _properties.rimColor2.colorValue = _properties.rimColor.colorValue;
                        materialEditor.RangeProperty(_properties.rimPower, _properties.rimPower.displayName);
                        materialEditor.RangeProperty(_properties.rimIntensity, _properties.rimIntensity.displayName);
                    }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Anisotropic Highlight", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableAnisoHighlight = EditorGUILayout.Toggle("Anisotropic Highlight", _enableAnisoHighlight);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated aniso highlight enabled");

                    if (_enableAnisoHighlight) { _target.EnableKeyword("_ANISOTROPIC_HIGHLIGHT"); }
                    else { _target.DisableKeyword("_ANISOTROPIC_HIGHLIGHT"); }
                }

                if (_enableAnisoHighlight)
                {
                    materialEditor.ColorProperty(_properties.anisoHighlightColor, _properties.anisoHighlightColor.displayName);
                    materialEditor.RangeProperty(_properties.anisoOffset, _properties.anisoOffset.displayName);
                    materialEditor.FloatProperty(_properties.anisoPower, _properties.anisoPower.displayName);
                    materialEditor.RangeProperty(_properties.anisoIntensity, _properties.anisoIntensity.displayName);

                    if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.hairUVTileIndex, _properties.hairUVTileIndex.displayName); }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Mouth AO Gradient Map", _headerLabel);
                EditorGUI.BeginChangeCheck();
                _enableMouthShadow = EditorGUILayout.Toggle("Mouth Shadow", _enableMouthShadow);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated mouth shadow enabled");

                    if (_enableMouthShadow) { _target.EnableKeyword("_MOUTH_SHADOW"); }
                    else { _target.DisableKeyword("_MOUTH_SHADOW"); }
                }

                if (_enableMouthShadow)
                {
                    var scale = _properties.mouthScale.vectorValue;
                    scale.y = EditorGUILayout.FloatField("LocalScale min z", scale.y);
                    scale.w = EditorGUILayout.FloatField("LocalScale max z", scale.w);
                    _properties.mouthScale.vectorValue = scale;
                    materialEditor.RangeProperty(_properties.mouthAOPower, _properties.mouthAOPower.displayName);
                    materialEditor.RangeProperty(_properties.mouthAOIntensity, _properties.mouthAOIntensity.displayName);

                    if (_enableUVTiling) { materialEditor.IntegerProperty(_properties.mouthUVTileIndex, _properties.mouthUVTileIndex.displayName); }
                }

                EditorGUILayout.Space(10);
                EditorGUI.BeginChangeCheck();
                _enableReflection = EditorGUILayout.Toggle("ReflectionProbe", _enableReflection);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated reflection enabled");

                    if (_enableReflection) { _target.EnableKeyword("_REFLECTION"); }
                    else { _target.DisableKeyword("_REFLECTION"); }
                }

            }
        }

        _isOpenAdvancedSettings = CoreEditorUtils.DrawHeaderFoldout("Advanced Settings", _isOpenAdvancedSettings);

        if (_beforeIsOpenAdvancedSettings ^ _isOpenAdvancedSettings)
        {
            SetExpanded((uint)Expandable.Advanced, _isOpenAdvancedSettings);
            _beforeIsOpenAdvancedSettings = _isOpenAdvancedSettings;
        }

        if (_isOpenAdvancedSettings)
        {
            EditorGUI.BeginChangeCheck();
            _enableTex2DArray = EditorGUILayout.Toggle("Tex2DArray", _enableTex2DArray);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated tex2darray enabled");

                if (_enableTex2DArray) { _target.EnableKeyword("_TEX_ARRAY"); }
                else { _target.DisableKeyword("_TEX_ARRAY"); }
            }

            EditorGUI.BeginChangeCheck();
            _enableUVTiling = EditorGUILayout.Toggle("UV Tiling", _enableUVTiling);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated uv tiling enabled");

                if (_enableUVTiling) { _target.EnableKeyword("_UVTILING"); }
                else { _target.DisableKeyword("_UVTILING"); }
            }

            if (_enableUVTiling)
            {
                materialEditor.IntegerProperty(_properties.uvTiles, _properties.uvTiles.displayName);
                materialEditor.IntegerProperty(_properties.uvCutoff, _properties.uvCutoff.displayName);

                EditorGUI.BeginChangeCheck();
                _enableOneTexture = EditorGUILayout.Toggle("Using one texture", _enableOneTexture);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated one texture enabled");

                    if (_enableOneTexture) { _target.EnableKeyword("_ONE_TEXTURE"); }
                    else { _target.DisableKeyword("_ONE_TEXTURE"); }
                }
            }

            EditorGUI.BeginChangeCheck();
            _enableDebugMipmap = EditorGUILayout.Toggle("Debug Mipmap", _enableDebugMipmap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated debug mipmap enabled");

                if (_enableDebugMipmap) { _target.EnableKeyword("_DEBUG_MIPMAP"); }
                else { _target.DisableKeyword("_DEBUG_MIPMAP"); }
            }

            if (_enableDebugMipmap)
            {
                if (_enableTex2DArray)
                {
                    materialEditor.TexturePropertySingleLine(_debugMipmapGUI, _properties.debugMipmapTexArray);
                    materialEditor.TextureScaleOffsetProperty(_properties.debugMipmapTexArray);
                }
                else
                {
                    materialEditor.TexturePropertySingleLine(_debugMipmapGUI, _properties.debugMipmapTex);
                    materialEditor.TextureScaleOffsetProperty(_properties.debugMipmapTex);
                }
            }

            materialEditor.RenderQueueField();
        }

        if (!_enableMARMap)
        {
            if (_enableTex2DArray) { _properties.marMapArray.textureValue = null; }
            else { _properties.marMap.textureValue = null; }
        }

        if (!_enableNormalMap)
        {
            if (_enableTex2DArray) { _properties.normalMapArray.textureValue = null; }
            else { _properties.normalMap.textureValue = null; }
        }

        if (!_enableDebugMipmap)
        {
            if (_enableTex2DArray) { _properties.debugMipmapTexArray.textureValue = null; }
            else { _properties.debugMipmapTex.textureValue = null; }
        }

        if (_enableTex2DArray)
        {
            _properties.baseMap.textureValue = null;
            _properties.marMap.textureValue = null;
            _properties.normalMap.textureValue = null;
            _properties.debugMipmapTex.textureValue = null;
        }
        else
        {
            _properties.baseMapArray.textureValue = null;
            _properties.marMapArray.textureValue = null;
            _properties.normalMapArray.textureValue = null;
            _properties.debugMipmapTexArray.textureValue = null;
        }

        if (!_enableMatCapMap)
        {
            //_properties.matCapMap.textureValue = null;
        }

        if (!_enableEyesMakeupMap) { _properties.eyesMakeupMapArray.textureValue = null; }

        if (!_enableBlushMakeupMap) { _properties.blushMakeupMapArray.textureValue = null; }

        if (!_enableLipsMakeupMap) { _properties.lipsMakeupMapArray.textureValue = null; }

        if (!_enableFacialTatooMap) { _properties.facialTatooMapArray.textureValue = null; }

        if (!_enableFacialMarkingMap) { _properties.facialMarkingMapArray.textureValue = null; }
    }

    private void UpdateSurfaceType()
    {
        switch (_surfaceType)
        {
            case SurfaceType.Opaque:
                _target.renderQueue = (int)RenderQueue.Geometry;
                _target.SetOverrideTag("RenderType", "Opaque");
                _target.SetInt("_SrcBlend", (int)BlendMode.One);
                _target.SetInt("_DstBlend", (int)BlendMode.Zero);
                _target.SetInt("_ZWrite", 1);
                _target.SetFloat("_Cutoff", 0);
                break;
            case SurfaceType.Cutout:
                _target.renderQueue = (int)RenderQueue.AlphaTest;
                _target.SetOverrideTag("RenderType", "TransparentCutout");
                _target.SetInt("_SrcBlend", (int)BlendMode.One);
                _target.SetInt("_DstBlend", (int)BlendMode.Zero);
                _target.SetInt("_ZWrite", 1);
                break;
            case SurfaceType.Transparent:
                _target.renderQueue = (int)RenderQueue.Transparent;
                _target.SetOverrideTag("RenderType", "Transparent");
                _target.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                _target.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                _target.SetInt("_ZWrite", 0);
                _target.SetFloat("_Cutoff", 0);
                break;
        }
    }

    private void UpdateRenderMode()
    {
        switch (_renderMode)
        {
            case RenderMode.UNITY_UNLIT:
                _target.EnableKeyword(UNLIT_KEYWORD);
                _target.DisableKeyword(SIMPLE_LIT_KEYWORD);
                _target.DisableKeyword(CUSTOM_LIT_KEYWORD);
                _target.DisableKeyword("_NORMALMAP");
                _target.DisableKeyword("_MARMAP");
                _target.DisableKeyword("_MATCAPMAP");
                _target.DisableKeyword("_REFLECTION");
                _target.DisableKeyword("_DIFFUSE_COLOR");
                _target.DisableKeyword("_RIM_LIGHT");
                break;
            case RenderMode.UNITY_SIMPLE_LIT:
                _target.DisableKeyword(UNLIT_KEYWORD);
                _target.EnableKeyword(SIMPLE_LIT_KEYWORD);
                _target.DisableKeyword(CUSTOM_LIT_KEYWORD);
                _target.DisableKeyword("_NORMALMAP");
                _target.DisableKeyword("_REFLECTION");
                _target.DisableKeyword("_DIFFUSE_COLOR");
                _target.DisableKeyword("_RIM_LIGHT");
                break;
            case RenderMode.CUSTOM_LIT:
                _target.DisableKeyword(UNLIT_KEYWORD);
                _target.DisableKeyword(SIMPLE_LIT_KEYWORD);
                _target.EnableKeyword(CUSTOM_LIT_KEYWORD);
                break;
            case RenderMode.UNITY_LIT:
            default:
                _target.DisableKeyword(UNLIT_KEYWORD);
                _target.DisableKeyword(SIMPLE_LIT_KEYWORD);
                _target.DisableKeyword(CUSTOM_LIT_KEYWORD);
                _target.DisableKeyword("_REFLECTION");
                _target.DisableKeyword("_DIFFUSE_COLOR");
                _target.DisableKeyword("_RIM_LIGHT");
                break;
        }
    }

    private bool IsExpanded(uint mask)
    {
        string key = EDITOR_PREFS_KEY_PREFIX + _target.shader.name;

        if (EditorPrefs.HasKey(key))
        {
            uint state = (uint)EditorPrefs.GetInt(key);
            return (state & mask) > 0;
        }

        uint defaultState = uint.MaxValue & ~(uint)Expandable.Advanced;
        EditorPrefs.SetInt(key, (int)defaultState);
        return (defaultState & mask) > 0;
    }

    private void SetExpanded(uint mask, bool value)
    {
        string key = EDITOR_PREFS_KEY_PREFIX + _target.shader.name;
        uint state = (uint)EditorPrefs.GetInt(key);

        if (value)
        {
            state |= mask;
        }
        else
        {
            mask = ~mask;
            state &= mask;
        }

        EditorPrefs.SetInt(key, (int)state);
    }

    private void ResetEyesMakeupBlendingMode()
    {
        _target.DisableKeyword("_EYES_MAKEUP_BURN");
        _target.DisableKeyword("_EYES_MAKEUP_DARKEN");
        _target.DisableKeyword("_EYES_MAKEUP_DIFF");
        _target.DisableKeyword("_EYES_MAKEUP_DODGE");
        _target.DisableKeyword("_EYES_MAKEUP_DIVIDE");
        _target.DisableKeyword("_EYES_MAKEUP_EXCLUSION");
        _target.DisableKeyword("_EYES_MAKEUP_HARDLIGHT");
        _target.DisableKeyword("_EYES_MAKEUP_HARDMIX");
        _target.DisableKeyword("_EYES_MAKEUP_LIGHTEN");
        _target.DisableKeyword("_EYES_MAKEUP_LINEARBURN");
        _target.DisableKeyword("_EYES_MAKEUP_LINEARDODGE");
        _target.DisableKeyword("_EYES_MAKEUP_LINEARLIGHT");
        _target.DisableKeyword("_EYES_MAKEUP_LINEARLIGHTADDSUB");
        _target.DisableKeyword("_EYES_MAKEUP_MULTIPLY");
        _target.DisableKeyword("_EYES_MAKEUP_NEGATION");
        _target.DisableKeyword("_EYES_MAKEUP_SCREEN");
        _target.DisableKeyword("_EYES_MAKEUP_OVERLAY");
        _target.DisableKeyword("_EYES_MAKEUP_PINLIGHT");
        _target.DisableKeyword("_EYES_MAKEUP_SOFTLIGHT");
        _target.DisableKeyword("_EYES_MAKEUP_VIVIDLIGHT");
        _target.DisableKeyword("_EYES_MAKEUP_SUBTRACT");
    }

    private void ResetBlushMakeupBlendingMode()
    {
        _target.DisableKeyword("_BLUSH_MAKEUP_BURN");
        _target.DisableKeyword("_BLUSH_MAKEUP_DARKEN");
        _target.DisableKeyword("_BLUSH_MAKEUP_DIFF");
        _target.DisableKeyword("_BLUSH_MAKEUP_DODGE");
        _target.DisableKeyword("_BLUSH_MAKEUP_DIVIDE");
        _target.DisableKeyword("_BLUSH_MAKEUP_EXCLUSION");
        _target.DisableKeyword("_BLUSH_MAKEUP_HARDLIGHT");
        _target.DisableKeyword("_BLUSH_MAKEUP_HARDMIX");
        _target.DisableKeyword("_BLUSH_MAKEUP_LIGHTEN");
        _target.DisableKeyword("_BLUSH_MAKEUP_LINEARBURN");
        _target.DisableKeyword("_BLUSH_MAKEUP_LINEARDODGE");
        _target.DisableKeyword("_BLUSH_MAKEUP_LINEARLIGHT");
        _target.DisableKeyword("_BLUSH_MAKEUP_LINEARLIGHTADDSUB");
        _target.DisableKeyword("_BLUSH_MAKEUP_MULTIPLY");
        _target.DisableKeyword("_BLUSH_MAKEUP_NEGATION");
        _target.DisableKeyword("_BLUSH_MAKEUP_SCREEN");
        _target.DisableKeyword("_BLUSH_MAKEUP_OVERLAY");
        _target.DisableKeyword("_BLUSH_MAKEUP_PINLIGHT");
        _target.DisableKeyword("_BLUSH_MAKEUP_SOFTLIGHT");
        _target.DisableKeyword("_BLUSH_MAKEUP_VIVIDLIGHT");
        _target.DisableKeyword("_BLUSH_MAKEUP_SUBTRACT");
    }

    private void ResetLipsMakeupBlendingMode()
    {
        _target.DisableKeyword("_LIPS_MAKEUP_BURN");
        _target.DisableKeyword("_LIPS_MAKEUP_DARKEN");
        _target.DisableKeyword("_LIPS_MAKEUP_DIFF");
        _target.DisableKeyword("_LIPS_MAKEUP_DODGE");
        _target.DisableKeyword("_LIPS_MAKEUP_DIVIDE");
        _target.DisableKeyword("_LIPS_MAKEUP_EXCLUSION");
        _target.DisableKeyword("_LIPS_MAKEUP_HARDLIGHT");
        _target.DisableKeyword("_LIPS_MAKEUP_HARDMIX");
        _target.DisableKeyword("_LIPS_MAKEUP_LIGHTEN");
        _target.DisableKeyword("_LIPS_MAKEUP_LINEARBURN");
        _target.DisableKeyword("_LIPS_MAKEUP_LINEARDODGE");
        _target.DisableKeyword("_LIPS_MAKEUP_LINEARLIGHT");
        _target.DisableKeyword("_LIPS_MAKEUP_LINEARLIGHTADDSUB");
        _target.DisableKeyword("_LIPS_MAKEUP_MULTIPLY");
        _target.DisableKeyword("_LIPS_MAKEUP_NEGATION");
        _target.DisableKeyword("_LIPS_MAKEUP_SCREEN");
        _target.DisableKeyword("_LIPS_MAKEUP_OVERLAY");
        _target.DisableKeyword("_LIPS_MAKEUP_PINLIGHT");
        _target.DisableKeyword("_LIPS_MAKEUP_SOFTLIGHT");
        _target.DisableKeyword("_LIPS_MAKEUP_VIVIDLIGHT");
        _target.DisableKeyword("_LIPS_MAKEUP_SUBTRACT");
    }

    private void ResetFacialTatooBlendingMode()
    {
        _target.DisableKeyword("_FACIAL_TATOO_BURN");
        _target.DisableKeyword("_FACIAL_TATOO_DARKEN");
        _target.DisableKeyword("_FACIAL_TATOO_DIFF");
        _target.DisableKeyword("_FACIAL_TATOO_DODGE");
        _target.DisableKeyword("_FACIAL_TATOO_DIVIDE");
        _target.DisableKeyword("_FACIAL_TATOO_EXCLUSION");
        _target.DisableKeyword("_FACIAL_TATOO_HARDLIGHT");
        _target.DisableKeyword("_FACIAL_TATOO_HARDMIX");
        _target.DisableKeyword("_FACIAL_TATOO_LIGHTEN");
        _target.DisableKeyword("_FACIAL_TATOO_LINEARBURN");
        _target.DisableKeyword("_FACIAL_TATOO_LINEARDODGE");
        _target.DisableKeyword("_FACIAL_TATOO_LINEARLIGHT");
        _target.DisableKeyword("_FACIAL_TATOO_LINEARLIGHTADDSUB");
        _target.DisableKeyword("_FACIAL_TATOO_MULTIPLY");
        _target.DisableKeyword("_FACIAL_TATOO_NEGATION");
        _target.DisableKeyword("_FACIAL_TATOO_SCREEN");
        _target.DisableKeyword("_FACIAL_TATOO_OVERLAY");
        _target.DisableKeyword("_FACIAL_TATOO_PINLIGHT");
        _target.DisableKeyword("_FACIAL_TATOO_SOFTLIGHT");
        _target.DisableKeyword("_FACIAL_TATOO_VIVIDLIGHT");
        _target.DisableKeyword("_FACIAL_TATOO_SUBTRACT");
    }

    private void ResetFacialMarkingBlendingMode()
    {
        _target.DisableKeyword("_FACIAL_MARKING_BURN");
        _target.DisableKeyword("_FACIAL_MARKING_DARKEN");
        _target.DisableKeyword("_FACIAL_MARKING_DIFF");
        _target.DisableKeyword("_FACIAL_MARKING_DODGE");
        _target.DisableKeyword("_FACIAL_MARKING_DIVIDE");
        _target.DisableKeyword("_FACIAL_MARKING_EXCLUSION");
        _target.DisableKeyword("_FACIAL_MARKING_HARDLIGHT");
        _target.DisableKeyword("_FACIAL_MARKING_HARDMIX");
        _target.DisableKeyword("_FACIAL_MARKING_LIGHTEN");
        _target.DisableKeyword("_FACIAL_MARKING_LINEARBURN");
        _target.DisableKeyword("_FACIAL_MARKING_LINEARDODGE");
        _target.DisableKeyword("_FACIAL_MARKING_LINEARLIGHT");
        _target.DisableKeyword("_FACIAL_MARKING_LINEARLIGHTADDSUB");
        _target.DisableKeyword("_FACIAL_MARKING_MULTIPLY");
        _target.DisableKeyword("_FACIAL_MARKING_NEGATION");
        _target.DisableKeyword("_FACIAL_MARKING_SCREEN");
        _target.DisableKeyword("_FACIAL_MARKING_OVERLAY");
        _target.DisableKeyword("_FACIAL_MARKING_PINLIGHT");
        _target.DisableKeyword("_FACIAL_MARKING_SOFTLIGHT");
        _target.DisableKeyword("_FACIAL_MARKING_VIVIDLIGHT");
        _target.DisableKeyword("_FACIAL_MARKING_SUBTRACT");
    }

}
