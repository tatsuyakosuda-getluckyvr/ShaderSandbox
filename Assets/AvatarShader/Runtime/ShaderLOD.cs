using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderLOD
{

    private const string AVATAR_SHADER = "Custom/Avatar";

    public static void Initialize()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Shader.Find(AVATAR_SHADER).maximumLOD = 200;
#else
        Shader.Find(AVATAR_SHADER).maximumLOD = 300;
#endif
    }

}
