using UnityEngine;
using System.Collections;

public class RFX1_EffectSettingProjectile : MonoBehaviour
{
    public float FlyDistanceForProjectiles = 30;
    public float SpeedMultiplier = 1;
    public LayerMask CollidesWith = ~0;

    float startSpeed;
    private float oldSpeedMultiplier;
    
    void Awake()
    {
        oldSpeedMultiplier = SpeedMultiplier;
        var transformMotion = GetComponentInChildren<RFX1_TransformMotion>(true);
        if (transformMotion != null)
        {
            startSpeed = transformMotion.Speed;
        }
    }

    void OnEnable()
    {
        var transformMotion = GetComponentInChildren<RFX1_TransformMotion>(true);
        if (transformMotion != null)
        {
            transformMotion.Distance = FlyDistanceForProjectiles;
            transformMotion.CollidesWith = CollidesWith;
            transformMotion.Speed = startSpeed * SpeedMultiplier;
        }
    }

    void Update()
    {
        if (Mathf.Abs(oldSpeedMultiplier - SpeedMultiplier) > 0.001f)
        {
            OnEnable();
        }
    }
}
