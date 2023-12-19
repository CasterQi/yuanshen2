using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_ParticleInfinite : MonoBehaviour {
    public float Delay = 3;
    ParticleSystem ps;
    ParticleSystem.MainModule main;
    float oldSimulation;
	// Use this for initialization
	void OnEnable () {
        if (ps == null)
        {
            ps = GetComponent<ParticleSystem>();
            main = ps.main;
            oldSimulation = main.simulationSpeed;
        }
        else
        {
            main.simulationSpeed = oldSimulation;
        }
        CancelInvoke("UpdateParticles");
        Invoke("UpdateParticles", Delay);
    }
	
	// Update is called once per frame
	void UpdateParticles () {
        main.simulationSpeed = 0;
	}
}
