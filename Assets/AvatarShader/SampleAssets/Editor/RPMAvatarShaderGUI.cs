using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;
using UnityEditor.Rendering;
using System;
using System.Resources;

public class RPMAvatarShaderGUI : ShaderGUI
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
        public MaterialProperty mrMap;
        public MaterialProperty mrMapArray;
        public MaterialProperty mrMapIndex;
        public MaterialProperty metallic;
        public MaterialProperty smoothness;
        public MaterialProperty normalMap;
        public MaterialProperty normalMapArray;
        public MaterialProperty normalMapIndex;
        public MaterialProperty normalStrength;
        public MaterialProperty occlusion;
        public MaterialProperty cutoff;
        public MaterialProperty diffuseColor;
        public MaterialProperty eulerLightDirection;
        public MaterialProperty lightDirection;
        public MaterialProperty rimColor;
        public MaterialProperty rimPower;
    }

    private RenderMode _renderMode;
    private SurfaceType _surfaceType;
    private LightingModel _lightingModel;
    private Material _target;
    private Properties _properties;
    private bool _enableMRMap;
    private bool _enableNormalMap;
    private bool _enableDiffuseColor;
    private bool _enableReflection;
    private bool _enableRimLight;
    private bool _enableTex2DArray;

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
            _properties.mrMap = properties.FirstOrDefault(x => x.name == "_MRMap");
            _properties.mrMapArray = properties.FirstOrDefault(x => x.name == "_MRMapArray");
            _properties.mrMapIndex = properties.FirstOrDefault(x => x.name == "_MRMapIndex");
            _properties.metallic = properties.FirstOrDefault(x => x.name == "_Metallic");
            _properties.smoothness = properties.FirstOrDefault(x => x.name == "_Smoothness");
            _properties.normalMap = properties.FirstOrDefault(x => x.name == "_NormalMap");
            _properties.normalMapArray = properties.FirstOrDefault(x => x.name == "_NormalMapArray");
            _properties.normalMapIndex = properties.FirstOrDefault(x => x.name == "_NormalMapIndex");
            _properties.normalStrength = properties.FirstOrDefault(x => x.name == "_NormalStrength");
            _properties.occlusion = properties.FirstOrDefault(x => x.name == "_Occlusion");
            _properties.cutoff = properties.FirstOrDefault(x => x.name == "_Cutoff");
            _properties.diffuseColor = properties.FirstOrDefault(x => x.name == "_DiffuseColor");
            _properties.eulerLightDirection = properties.FirstOrDefault(x => x.name == "_EulerLightDirection");
            _properties.lightDirection = properties.FirstOrDefault(x => x.name == "_LightDirection");
            _properties.rimColor = properties.FirstOrDefault(x => x.name == "_RimColor");
            _properties.rimPower = properties.FirstOrDefault(x => x.name == "_RimPower");
        }

        Initialize();

        _enableMRMap = _target.IsKeywordEnabled("_MARMAP");
        _enableNormalMap = _target.IsKeywordEnabled("_NORMALMAP");
        _enableDiffuseColor = _target.IsKeywordEnabled("_DIFFUSE_COLOR");
        _enableReflection = _target.IsKeywordEnabled("_REFLECTION");
        _enableRimLight = _target.IsKeywordEnabled("_RIM_LIGHT");
        _enableTex2DArray = _target.IsKeywordEnabled("_TEX_ARRAY");

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

            if (_renderMode == RenderMode.UNITY_LIT || _renderMode == RenderMode.CUSTOM_LIT)
            {
                EditorGUI.BeginChangeCheck();
                _enableMRMap = EditorGUILayout.Toggle("Enable MR Map", _enableMRMap);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Updated mar map enabled");

                    if (_enableMRMap) { _target.EnableKeyword("_MARMAP"); }
                    else { _target.DisableKeyword("_MARMAP"); }
                }

                if (_enableMRMap)
                {
                    if (_enableTex2DArray)
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("MR Map", "Metallic(B) Roughness(G)"), _properties.mrMapArray, null);
                        materialEditor.IntegerProperty(_properties.mrMapIndex, _properties.mrMapIndex.displayName);
                    }
                    else
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("MR Map", "Metallic(B) Roughness(G)"), _properties.mrMap, null);
                    }

                    materialEditor.RangeProperty(_properties.smoothness, "Smoothness Scale");
                    materialEditor.RangeProperty(_properties.metallic, "Metallic Scale");
                }
                else
                {
                    materialEditor.RangeProperty(_properties.smoothness, _properties.smoothness.displayName);
                    materialEditor.RangeProperty(_properties.metallic, _properties.metallic.displayName);
                }

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
                else
                {
                    _properties.normalMap.textureValue = null;
                }
            }

            if (_renderMode != RenderMode.UNITY_UNLIT)
            {
                materialEditor.RangeProperty(_properties.occlusion, _properties.occlusion.displayName);
            }
        }

        if (!_enableMRMap) { _properties.mrMap.textureValue = null; }

        if (!_enableNormalMap) { _properties.normalMap.textureValue = null; }

        if (_enableTex2DArray)
        {
            _properties.baseMap.textureValue = null;
            _properties.mrMap.textureValue = null;
            _properties.normalMap.textureValue = null;
        }
        else
        {
            _properties.baseMapArray.textureValue = null;
            _properties.mrMapArray.textureValue = null;
            _properties.normalMapArray.textureValue = null;
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

                    materialEditor.ColorProperty(_properties.diffuseColor, _properties.diffuseColor.displayName);
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
                    materialEditor.ColorProperty(_properties.rimColor, _properties.rimColor.displayName);
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

            materialEditor.RenderQueueField();
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
                _enableNormalMap = false;
                _enableMRMap = false;
                break;
            case RenderMode.UNITY_SIMPLE_LIT:
                _target.DisableKeyword(UNLIT_KEYWORD);
                _target.EnableKeyword(SIMPLE_LIT_KEYWORD);
                _target.DisableKeyword(CUSTOM_LIT_KEYWORD);
                _target.DisableKeyword("_NORMALMAP");
                _target.DisableKeyword("_MARMAP");
                _target.DisableKeyword("_REFLECTION");
                _target.DisableKeyword("_DIFFUSE_COLOR");
                _target.DisableKeyword("_RIM_LIGHT");
                _enableNormalMap = false;
                _enableMRMap = false;
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
