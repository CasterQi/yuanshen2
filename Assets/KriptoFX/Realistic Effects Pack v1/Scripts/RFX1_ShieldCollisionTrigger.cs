using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_ShieldCollisionTrigger : MonoBehaviour
{
    public event EventHandler<RFX1_ShieldCollisionInfo> CollisionEnter;
    public event EventHandler<RFX1_ShieldDetectInfo> Detected;
    public float DetectRange = 0;
    public GameObject[] EffectOnCollision;
    public float DestroyTimeDelay = 5;
    public bool CollisionEffectInWorldSpace = true;
    public float CollisionOffset = 0;
    const string layerName = "Collision";
    //public float Radius = 1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DetectRange < 0.001f) return;

        var coll = Physics.OverlapSphere(transform.position, DetectRange);
        foreach (var collider1 in coll)
        {
            if (collider1.name.EndsWith(layerName))
            {
                var handler = Detected;
                if (handler != null)
                    handler(this, new RFX1_ShieldDetectInfo { DetectedGameObject = collider1.gameObject });
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectRange);

    }

    public void OnCollision(RaycastHit hit, GameObject sender = null)
    {
        var handler = CollisionEnter;
        if (handler != null)
            handler(this, new RFX1_ShieldCollisionInfo { Hit = hit });

        foreach (var effect in EffectOnCollision)
        {
            var instance = Instantiate(effect, hit.point + hit.normal * CollisionOffset, new Quaternion()) as GameObject;
            instance.transform.LookAt(hit.point + hit.normal + hit.normal * CollisionOffset);
            if (!CollisionEffectInWorldSpace) instance.transform.parent = transform;
            Destroy(instance, DestroyTimeDelay);
        }
        
    }
}


public class RFX1_ShieldCollisionInfo : EventArgs
{
    public RaycastHit Hit;
}

public class RFX1_ShieldDetectInfo : EventArgs
{
    public GameObject DetectedGameObject;
}
