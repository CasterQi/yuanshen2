using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RFX1_ParticleCollisionDecal : MonoBehaviour
{
    public ParticleSystem DecalParticles;
    public bool IsBilboard;
    public bool InstantiateWhenZeroSpeed;
    public float MaxGroundAngleDeviation = 45;
    public float MinDistanceBetweenDecals = 0.1f;
    public float MinDistanceBetweenSurface = 0.03f;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    ParticleSystem.Particle[] particles;

    ParticleSystem initiatorPS;
    List<GameObject> collidedGameObjects = new List<GameObject>();

    void OnEnable()
    {
        collisionEvents.Clear();
        collidedGameObjects.Clear();
        initiatorPS = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[DecalParticles.main.maxParticles];
        if (InstantiateWhenZeroSpeed) InvokeRepeating("CollisionDetect", 0, 0.1f);
    }

    void OnDisable()
    {
        if (InstantiateWhenZeroSpeed) CancelInvoke("CollisionDetect");
    }

    void CollisionDetect()
    {
        int aliveParticles = 0;

        if (InstantiateWhenZeroSpeed)
            aliveParticles = DecalParticles.GetParticles(particles);
        foreach (var collidedGameObject in collidedGameObjects)
        {
            OnParticleCollisionManual(collidedGameObject, aliveParticles);
        }
    }

    private void OnParticleCollisionManual(GameObject other, int aliveParticles = -1)
    {
        collisionEvents.Clear();
        var aliveEvents = initiatorPS.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < aliveEvents; i++)
        {
            var angle = Vector3.Angle(collisionEvents[i].normal, Vector3.up);
            if (angle > MaxGroundAngleDeviation) continue;
            if (InstantiateWhenZeroSpeed)
            {
                if (collisionEvents[i].velocity.sqrMagnitude > 0.1f) continue;
                var isNearDistance = false;
                for (int j = 0; j < aliveParticles; j++)
                {
                    var distance = Vector3.Distance(collisionEvents[i].intersection, particles[j].position);
                    if (distance < MinDistanceBetweenDecals) isNearDistance = true;
                }
                if (isNearDistance) continue;
            }
            var emiter = new ParticleSystem.EmitParams();
            emiter.position = collisionEvents[i].intersection;
            var rotation = Quaternion.LookRotation(-collisionEvents[i].normal).eulerAngles;
            rotation.z = Random.Range(0, 360);
            emiter.rotation3D = rotation;

            DecalParticles.Emit(emiter, 1);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (InstantiateWhenZeroSpeed)
        {
            if (!collidedGameObjects.Contains(other))
                collidedGameObjects.Add(other);
        }
        else
        {
            OnParticleCollisionManual(other);
        }
    }
}
