using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_Target : MonoBehaviour
{
    public GameObject Target;

    private GameObject currentTarget;
    RFX1_TransformMotion transformMotion;
	// Use this for initialization
	void Start ()
	{
	    transformMotion = GetComponentInChildren<RFX1_TransformMotion>();
      UpdateTarget();
  }

    void Update()
    {
        UpdateTarget();
    }
	
	// Update is called once per frame
	void UpdateTarget ()
	{
	    if (Target == null)
	    {
            //Debug.Log("You must set the target!");
	        return;
	    }
	    if (transformMotion == null)
	    {
	        Debug.Log("You must attach the target script on projectile effect!");
	        return;
	    }
	    if (Target != currentTarget)
	    {
	        currentTarget = Target;
	        transformMotion.Target = currentTarget;
	    }
	}
}
