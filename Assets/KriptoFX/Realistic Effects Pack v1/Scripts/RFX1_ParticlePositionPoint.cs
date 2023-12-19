using UnityEngine;

public class RFX1_ParticlePositionPoint : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;

    public RFX1_ShieldCollisionTrigger ShieldCollisionTrigger;
    public float Force = 1;
    public AnimationCurve ForceByTime = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public float ForceLifeTime = 1;

    bool canUpdate;
    
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule mainModule;
    float startTime;

    void Start()
    {
        ShieldCollisionTrigger.CollisionEnter += ShieldCollisionTrigger_CollisionEnter;
        ShieldCollisionTrigger.Detected += ShieldCollisionTrigger_Detected;
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
    }

    private void ShieldCollisionTrigger_Detected(object sender, RFX1_ShieldDetectInfo e)
    {
        RaycastHit hit;
        if (Physics.Raycast(e.DetectedGameObject.transform.position, e.DetectedGameObject.transform.forward, out hit, 10))
        {
            Position = hit.point;
            ManualOnEnable();
        }
    }

    private void ShieldCollisionTrigger_CollisionEnter(object sender, RFX1_ShieldCollisionInfo e)
    {
        Position = e.Hit.point;
        ManualOnEnable();
    }

    public void ManualOnEnable()
    {
        CancelInvoke("ManualOnDisable");
        startTime = Time.time;
        canUpdate = true;
        Invoke("ManualOnDisable", ForceLifeTime);
    }

    void ManualOnDisable()
    {
        canUpdate = false;
    }

    void LateUpdate()
    {
        if (!canUpdate) return;

        var maxParticles = mainModule.maxParticles;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        ps.GetParticles(particles);
        float forceDeltaTime = ForceByTime.Evaluate((Time.time - startTime) / ForceLifeTime) * Time.deltaTime * Force;
        
        var targetTransformedPosition = Vector3.zero;

        if(mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
            targetTransformedPosition = transform.InverseTransformPoint(Position);
        if(mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
            targetTransformedPosition = Position;
        
        int particleCount = ps.particleCount;

        for (int i = 0; i < particleCount; i++)
        {
            var directionToTarget = Vector3.Normalize(targetTransformedPosition - particles[i].position);
            var seekForce = directionToTarget * forceDeltaTime;

            particles[i].position += seekForce;
        }

        ps.SetParticles(particles, particleCount);
    }
}
