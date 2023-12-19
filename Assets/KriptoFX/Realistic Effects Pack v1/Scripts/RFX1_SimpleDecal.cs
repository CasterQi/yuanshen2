using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_SimpleDecal : MonoBehaviour
{
    public float Offset = 0.05f;
    private Transform t;
    
	// Use this for initialization
	void Awake ()
	{
	    t = transform;
	}

    private RaycastHit hit;
    // Update is called once per frame
    void LateUpdate ()
	{
	    if (Physics.Raycast(t.parent.position + Vector3.up / 2, Vector3.down, out hit))
	    {
	        transform.position = hit.point + Vector3.up * Offset;
	        transform.rotation = Quaternion.LookRotation(-hit.normal);
	    }
	}
}
