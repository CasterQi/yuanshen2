using UnityEngine;

[ExecuteInEditMode]
public class RFX1_ParticleGravityPoint : MonoBehaviour
{
    public Transform target;
    public float Force = 1;
    public AnimationCurve ForceByTime = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public float ForceLifeTime = 1;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule mainModule;
    float startTime;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    void LateUpdate()
    {
        var maxParticles = mainModule.maxParticles;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        int particleCount = ps.GetParticles(particles);
        
        float forceDeltaTime = ForceByTime.Evaluate((Time.time - startTime) / ForceLifeTime) * Time.deltaTime * Force;
       
        var targetTransformedPosition = Vector3.zero;

        if(mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
            targetTransformedPosition = transform.InverseTransformPoint(target.position);
        if(mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
            targetTransformedPosition = target.position;
       
        for (int i = 0; i < particleCount; i++)
        {
            var directionToTarget = Vector3.Normalize(targetTransformedPosition - particles[i].position);
            var seekForce = directionToTarget * forceDeltaTime;
            
            particles[i].velocity += seekForce;
        }

        ps.SetParticles(particles, particleCount);
    }
}
