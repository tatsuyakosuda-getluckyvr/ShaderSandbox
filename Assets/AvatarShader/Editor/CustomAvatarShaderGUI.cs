using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;
using UnityEditor.Rendering;

public class CustomAvatarShaderGUI : ShaderGUI
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

    private class Properties
    {
        public MaterialProperty baseMap;
        public MaterialProperty baseMapArray;
        public MaterialProperty baseMapIndex;
        public MaterialProperty baseColor;
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
        public MaterialProperty lightDirection;
        public MaterialProperty rimColor;
        public MaterialProperty rimColor2;
        public MaterialProperty rimPower;
        public MaterialProperty gradientScale;
        public MaterialProperty debugMipmapTex;
        public MaterialProperty debugMipmapTexArray;
        public MaterialProperty gradientAngle;
    }

    private RenderMode _renderMode;
    private SurfaceType _surfaceType;
    private LightingModel _lightingModel;
    private Material _target;
    private Properties _properties;
    private bool _enableMARMap;
    private bool _enableNormalMap;
    private bool _enableDiffuseColor;
    private bool _enableReflection;
    private bool _enableRimLight;
    private bool _enableTex2DArray;
    private bool _enableDebugMipmap;
    private bool _enableRadialGradient;
    private bool _enableGradient;

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

        _isOpenSurfaceOptions = IsExpanded((uint)Expandable.SurfaceOptions);
        _beforeIsOpenSurfaceOptions = _isOpenSurfaceOptions;
        _isOpenSurfaceInputs = IsExpanded((uint)Expandable.SurfaceInputs);
        _beforeIsOpenSurfaceInputs = _isOpenSurfaceInputs;
        _isOpenLightSettings = IsExpanded((uint)Expandable.LightingSettings);
        _beforeIsOpenLightSettings = _isOpenLightSettings;
        _isOpenAdvancedSettings = IsExpanded((uint)Expandable.Advanced);
        _beforeIsOpenAdvancedSettings = _isOpenAdvancedSettings;
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
            _properties.baseColor = properties.FirstOrDefault(x => x.name == "_BaseColor");
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
            _properties.lightDirection = properties.FirstOrDefault(x => x.name == "_LightDirection");
            _properties.rimColor = properties.FirstOrDefault(x => x.name == "_RimColor");
            _properties.rimColor2 = properties.FirstOrDefault(x => x.name == "_RimColor2");
            _properties.gradientScale = properties.FirstOrDefault(x => x.name == "_GradientScale");
            _properties.rimPower = properties.FirstOrDefault(x => x.name == "_RimPower");
            _properties.debugMipmapTex = properties.FirstOrDefault(x => x.name == "_DebugMipmapTex");
            _properties.debugMipmapTexArray = properties.FirstOrDefault(x => x.name == "_DebugMipmapTexArray");
            _properties.gradientAngle = properties.FirstOrDefault(x => x.name == "_GradientAngle");
        }

        Initialize();

        _enableMARMap = _target.IsKeywordEnabled("_MARMAP");
        _enableNormalMap = _target.IsKeywordEnabled("_NORMALMAP");
        _enableDiffuseColor = _target.IsKeywordEnabled("_DIFFUSE_COLOR");
        _enableReflection = _target.IsKeywordEnabled("_REFLECTION");
        _enableRimLight = _target.IsKeywordEnabled("_RIM_LIGHT");
        _enableTex2DArray = _target.IsKeywordEnabled("_TEX_ARRAY");
        _enableDebugMipmap = _target.IsKeywordEnabled("_DEBUG_MIPMAP");
        _enableRadialGradient = _target.IsKeywordEnabled("_RADIAL_GRADIENT_LIGHT");
        _enableGradient = _target.IsKeywordEnabled("_GRADIENT_LIGHT");

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
            if (_enableTex2DArray)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), _properties.baseMapArray, _properties.baseColor);
                materialEditor.IntegerProperty(_properties.baseMapIndex, _properties.baseMapIndex.displayName);
            }
            else
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), _properties.baseMap, _properties.baseColor);
            }

            if (_renderMode != RenderMode.UNITY_UNLIT)
            {
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
                        materialEditor.TexturePropertySingleLine(new GUIContent("MAR Map", "Metallic(R) AO(G) Roughness(B)"), _properties.marMapArray, null);
                        materialEditor.IntegerProperty(_properties.marMapIndex, _properties.marMapIndex.displayName);
                    }
                    else
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("MAR Map", "Metallic(R) AO(G) Roughness(B)"), _properties.marMap, null);
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
                            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), _properties.normalMapArray, _properties.normalStrength);
                            materialEditor.IntegerProperty(_properties.normalMapIndex, _properties.normalMapIndex.displayName);
                        }
                        else
                        {
                            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), _properties.normalMap, _properties.normalStrength);
                        }
                    }
                }
            }

            if (_enableTex2DArray) { materialEditor.TextureScaleOffsetProperty(_properties.baseMapArray); }
            else { materialEditor.TextureScaleOffsetProperty(_properties.baseMap); }
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
                    _enableRadialGradient = EditorGUILayout.Toggle("Enable Radial Gradient", _enableRadialGradient);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_target, "Updated radial gradient enabled");

                        if (_enableRadialGradient) { _target.EnableKeyword("_RADIAL_GRADIENT_LIGHT"); }
                        else { _target.DisableKeyword("_RADIAL_GRADIENT_LIGHT"); }
                    }

                    materialEditor.VectorProperty(_properties.gradientScale, _properties.gradientScale.displayName);

                    if (!_enableRadialGradient)
                    {
                        materialEditor.RangeProperty(_properties.gradientAngle, _properties.gradientAngle.displayName);
                    }
                }

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
                        materialEditor.ColorProperty(_properties.diffuseColor, _properties.diffuseColor.displayName);
                        materialEditor.ColorProperty(_properties.diffuseColor2, _properties.diffuseColor2.displayName);
                    }
                    else
                    {
                        materialEditor.ColorProperty(_properties.diffuseColor, _properties.diffuseColor.displayName);
                    }

                    _properties.eulerLightDirection.vectorValue = EditorGUILayout.Vector2Field("Light Direction", _properties.eulerLightDirection.vectorValue);
                    var dir = _properties.eulerLightDirection.vectorValue;
                    var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(dir.x, dir.y, dir.z), Vector3.one);
                    var lightpos = -matrix.GetColumn(2);
                    _properties.lightDirection.vectorValue = new Vector4(lightpos.x, lightpos.y, lightpos.z, 0);
                }

                EditorGUI.BeginChangeCheck();
                _enableReflection = EditorGUILayout.Toggle("Enable Reflection", _enableReflection);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated reflection enabled");

                    if (_enableReflection) { _target.EnableKeyword("_REFLECTION"); }
                    else { _target.DisableKeyword("_REFLECTION"); }
                }

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
                        materialEditor.ColorProperty(_properties.rimColor, _properties.rimColor.displayName);
                        materialEditor.ColorProperty(_properties.rimColor2, _properties.rimColor2.displayName);
                    }
                    else
                    {
                        materialEditor.ColorProperty(_properties.rimColor, _properties.rimColor.displayName);
                        _properties.rimColor2.colorValue = _properties.rimColor.colorValue;
                    }

                    materialEditor.RangeProperty(_properties.rimPower, _properties.rimPower.displayName);
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
            _enableTex2DArray = EditorGUILayout.Toggle("Enable Tex2DArray", _enableTex2DArray);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_target, "Updated tex2darray enabled");

                if (_enableTex2DArray) { _target.EnableKeyword("_TEX_ARRAY"); }
                else { _target.DisableKeyword("_TEX_ARRAY"); }
            }

            EditorGUI.BeginChangeCheck();
            _enableDebugMipmap = EditorGUILayout.Toggle("Enable Debug Mipmap", _enableDebugMipmap);

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
                    materialEditor.TexturePropertySingleLine(new GUIContent(_properties.debugMipmapTexArray.displayName), _properties.debugMipmapTexArray);
                    materialEditor.TextureScaleOffsetProperty(_properties.debugMipmapTexArray);
                }
                else
                {
                    materialEditor.TexturePropertySingleLine(new GUIContent(_properties.debugMipmapTex.displayName), _properties.debugMipmapTex);
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

}
