using UnityEngine;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI
{
    static float tollerance = 0.001f;
    public override void OnGUI(MaterialEditor m, MaterialProperty[] properties)
    {
        var _UseMainTex = ShaderGUI.FindProperty("_UseMainTex", properties);
        var _MainTex = ShaderGUI.FindProperty("_MainTex", properties);
        var _TintColor = ShaderGUI.FindProperty("_TintColor", properties);

        var _NormalTex = ShaderGUI.FindProperty("_NormalTex", properties);
        var _MainColor = ShaderGUI.FindProperty("_MainColor", properties);
        var _Distortion = ShaderGUI.FindProperty("_Distortion", properties);

        var _UseRefractive = ShaderGUI.FindProperty("_UseRefractive", properties);
        var _RefractiveStrength = ShaderGUI.FindProperty("_RefractiveStrength", properties);

        var _UseSoft = ShaderGUI.FindProperty("_UseSoft", properties);
        var _InvFade = ShaderGUI.FindProperty("_InvFade", properties);

        var _UseHeight = ShaderGUI.FindProperty("_UseHeight", properties);
        var _HeightTex = ShaderGUI.FindProperty("_HeightTex", properties);
        var _Height = ShaderGUI.FindProperty("_Height", properties);
        var _HeightUVScrollDistort = ShaderGUI.FindProperty("_HeightUVScrollDistort", properties);

        var _UseFresnel = ShaderGUI.FindProperty("_UseFresnel", properties);
        var _FresnelColor = ShaderGUI.FindProperty("_FresnelColor", properties);
        var _FresnelPow = ShaderGUI.FindProperty("_FresnelPow", properties);
        var _FresnelR0 = ShaderGUI.FindProperty("_FresnelR0", properties);
        var _FresnelDistort = ShaderGUI.FindProperty("_FresnelDistort", properties);

        var _UseCutout = ShaderGUI.FindProperty("_UseCutout", properties);
        var _CutoutTex = ShaderGUI.FindProperty("_CutoutTex", properties);
        var _Cutout = ShaderGUI.FindProperty("_Cutout", properties);
        var _CutoutColor = ShaderGUI.FindProperty("_CutoutColor", properties);
        var _CutoutThreshold = ShaderGUI.FindProperty("_CutoutThreshold", properties);

        var _ZWriteMode = ShaderGUI.FindProperty("_ZWriteMode", properties);
        var _CullMode = ShaderGUI.FindProperty("_CullMode", properties);
        var _UseAlphaCliping = ShaderGUI.FindProperty("_UseAlphaCliping", properties);
        var _AlphaClip = ShaderGUI.FindProperty("_AlphaClip", properties);
        var _UseBlending = ShaderGUI.FindProperty("_UseBlending", properties);

        m.ShaderProperty(_UseMainTex, _UseMainTex.displayName);
        if (Mathf.Abs(_UseMainTex.floatValue - 1) < tollerance)
        {
            m.ShaderProperty(_MainTex, _MainTex.displayName);
            m.ShaderProperty(_TintColor, _TintColor.displayName);
        }

        m.ShaderProperty(_NormalTex, _NormalTex.displayName);
        m.ShaderProperty(_MainColor, _MainColor.displayName);
        m.ShaderProperty(_Distortion, _Distortion.displayName);

        m.ShaderProperty(_UseRefractive, _UseRefractive.displayName);
        if (Mathf.Abs(_UseRefractive.floatValue - 1) < tollerance)
            m.ShaderProperty(_RefractiveStrength, _RefractiveStrength.displayName);

        m.ShaderProperty(_UseSoft, _UseSoft.displayName);
        if (Mathf.Abs(_UseSoft.floatValue - 1) < tollerance)
            m.ShaderProperty(_InvFade, _InvFade.displayName);

        m.ShaderProperty(_UseHeight, _UseHeight.displayName);
        if (Mathf.Abs(_UseHeight.floatValue - 1) < tollerance)
        {
            m.ShaderProperty(_HeightTex, _HeightTex.displayName);
            m.ShaderProperty(_Height, _Height.displayName);
            m.ShaderProperty(_HeightUVScrollDistort, _HeightUVScrollDistort.displayName);
        }

        m.ShaderProperty(_UseFresnel, _UseFresnel.displayName);
        if (Mathf.Abs(_UseFresnel.floatValue - 1) < tollerance)
        {
            m.ShaderProperty(_FresnelColor, _FresnelColor.displayName);
            m.ShaderProperty(_FresnelPow, _FresnelPow.displayName);
            m.ShaderProperty(_FresnelR0, _FresnelR0.displayName);
            m.ShaderProperty(_FresnelDistort, _FresnelDistort.displayName);
        }

        m.ShaderProperty(_UseCutout, _UseCutout.displayName);
        if (Mathf.Abs(_UseCutout.floatValue - 1) < tollerance)
        {
            m.ShaderProperty(_CutoutTex, _CutoutTex.displayName);
            m.ShaderProperty(_Cutout, _Cutout.displayName);
            m.ShaderProperty(_CutoutColor, _CutoutColor.displayName);
            m.ShaderProperty(_CutoutThreshold, _CutoutThreshold.displayName);
        }

        m.ShaderProperty(_ZWriteMode, _ZWriteMode.displayName);
        m.ShaderProperty(_CullMode, _CullMode.displayName);
        m.ShaderProperty(_UseAlphaCliping, _UseAlphaCliping.displayName);
        if (Mathf.Abs(_UseAlphaCliping.floatValue - 1) < tollerance)
        {
            m.ShaderProperty(_AlphaClip, _AlphaClip.displayName);
        }
        m.ShaderProperty(_UseBlending, _UseBlending.displayName);
        m.RenderQueueField();
    }
}
