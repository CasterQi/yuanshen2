using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_ActivateGravityAfterCollision : MonoBehaviour
{

    public RFX1_TransformMotion TransformMotion;
    public Vector2 Gravity = new Vector2(1, 1);
    ParticleSystem ps;
    ParticleSystem.MinMaxCurve startGravity;
    bool isInitialized;

    void OnEnable()
    {
        TransformMotion.CollisionEnter += TransformMotion_CollisionEnter;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        if (!isInitialized)
        {
            isInitialized = true;
            startGravity = main.gravityModifier;
        }
        else main.gravityModifier = startGravity;
    }

    void OnDisable()
    {
        TransformMotion.CollisionEnter -= TransformMotion_CollisionEnter;
    }

    private void TransformMotion_CollisionEnter(object sender, RFX1_TransformMotion.RFX1_CollisionInfo e)
    {
        var main = ps.main;
        main.gravityModifier = new ParticleSystem.MinMaxCurve(Gravity.x, Gravity.y);
    }
}
