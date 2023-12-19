using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_RotationFreeze : MonoBehaviour
{
    public bool LockX = true;
    public bool LockY = true;
    public bool LockZ = true;

    private Vector3 startRotation;
    // Use this for initialization
    void Start ()
    {
        startRotation = transform.localRotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    var xRotation = LockX ? startRotation.x : transform.rotation.eulerAngles.x;
	    var yRotation = LockY ? startRotation.y : transform.rotation.eulerAngles.y;
	    var zRotation = LockZ ? startRotation.z : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
    }
}
