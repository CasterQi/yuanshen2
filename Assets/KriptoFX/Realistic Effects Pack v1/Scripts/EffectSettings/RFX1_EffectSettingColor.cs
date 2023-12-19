using UnityEngine;
using System.Collections;

public class RFX1_EffectSettingColor : MonoBehaviour
{
    public Color Color = Color.red;
    private Color previousColor;

    void OnEnable()
    {
        UpdateColor();
    }

    void Update()
    {
        if (previousColor != Color)
        {
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        var hue = RFX1_ColorHelper.ColorToHSV(Color).H;
        RFX1_ColorHelper.ChangeObjectColorByHUE(gameObject, hue);

        var transformMotion = GetComponentInChildren<RFX1_TransformMotion>(true);
        if (transformMotion != null) transformMotion.HUE = hue;
        previousColor = Color;
    }

}
