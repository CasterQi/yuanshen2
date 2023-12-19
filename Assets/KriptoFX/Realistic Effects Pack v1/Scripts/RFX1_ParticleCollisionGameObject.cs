using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RFX1_ParticleCollisionGameObject : MonoBehaviour
{
    public GameObject InstancedGO;
    public float DestroyDelay = 5;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    ParticleSystem initiatorPS;

    void OnEnable()
    {
        collisionEvents.Clear();
        initiatorPS = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        var aliveEvents = initiatorPS.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < aliveEvents; i++)
        {
            Destroy(Instantiate(InstancedGO, collisionEvents[i].intersection, new Quaternion()), DestroyDelay);
        }
    }
}
