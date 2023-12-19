using UnityEngine;
using System.Collections;

public class RFX1_AnimatorEvents : MonoBehaviour
{
    public RFX1_EffectAnimatorProperty Effect1;
    public RFX1_EffectAnimatorProperty Effect2;
    public RFX1_EffectAnimatorProperty Effect3;
    public GameObject Target;

    [HideInInspector] public float HUE = -1;
    [HideInInspector] public float Speed = -1;

    private float oldHUE;
    private float oldSpeed;


    [System.Serializable]
    public class RFX1_EffectAnimatorProperty
    {
        public GameObject Prefab;
        public Transform BonePosition;
        public Transform BoneRotation;
        public float DestroyTime = 10;
        [HideInInspector] public GameObject CurrentInstance;
    }

    void InstantiateEffect(RFX1_EffectAnimatorProperty effect)
    {
        if (effect.Prefab == null) return;
        effect.CurrentInstance = Instantiate(effect.Prefab, effect.BonePosition.position, effect.BoneRotation.rotation);

        if (HUE > -0.9f)
            UpdateColor(effect);
        if (Speed > -0.9f)
            UpdateSpeed(effect);

        if (Target != null)
        {
            var target = effect.CurrentInstance.GetComponent<RFX1_Target>();
            if (target != null) target.Target = Target;
        }
        if(effect.DestroyTime > 0.001f) Destroy(effect.CurrentInstance, effect.DestroyTime);
    }

    public void ActivateEffect1()
    {
        InstantiateEffect(Effect1);
    }

    public void ActivateEffect2()
    {
        InstantiateEffect(Effect2);
    }

    public void ActivateEffect3()
    {
        InstantiateEffect(Effect3);
    }
   
    void LateUpdate()
    {
        UpdateInstance(Effect1);
        UpdateInstance(Effect2);
        UpdateInstance(Effect3);
    }

    void UpdateInstance(RFX1_EffectAnimatorProperty effect)
    {
        if (effect.CurrentInstance != null && effect.BonePosition != null)
        {
            effect.CurrentInstance.transform.position = effect.BonePosition.position;
            if (HUE > -0.9f && Mathf.Abs(oldHUE - HUE) > 0.001f)
            {
                UpdateColor(effect);
            }
            if (Speed > -0.9f && Mathf.Abs(oldSpeed - Speed) > 0.001f)
            {
                UpdateSpeed(effect);
            }
        }
    }

    private void UpdateSpeed(RFX1_EffectAnimatorProperty effect)
    {
        oldSpeed = Speed;
        var projectile = effect.CurrentInstance.GetComponent<RFX1_EffectSettingProjectile>();
        if (projectile == null) projectile = effect.CurrentInstance.AddComponent<RFX1_EffectSettingProjectile>();
        projectile.SpeedMultiplier *= Speed;
    }

    private void UpdateColor(RFX1_EffectAnimatorProperty effect)
    {
        oldHUE = HUE;
        var settingColor = effect.CurrentInstance.GetComponent<RFX1_EffectSettingColor>();
        if (settingColor == null) settingColor = effect.CurrentInstance.AddComponent<RFX1_EffectSettingColor>();
        var hsv = RFX1_ColorHelper.ColorToHSV(settingColor.Color);
        hsv.H = HUE;
        settingColor.Color = RFX1_ColorHelper.HSVToColor(hsv);
    }
}
