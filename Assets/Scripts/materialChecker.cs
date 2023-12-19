using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialChecker : MonoBehaviour
{

    public bool isGrass, isWater;
    public GameObject GroundCheck;
    public LayerMask GrassMask, WaterMask;
    
    float checkerRadius = .4f;
    float GrassRealseCD = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GrassRealseCD -= Time.deltaTime;
        //isGrass = Physics.CheckSphere(GroundCheck.transform.position, checkerRadius, GrassMask);
        //isWater = Physics.CheckSphere(GroundCheck.transform.position, checkerRadius, WaterMask);
        Ray downRay = new Ray(transform.position + Vector3.up*2f+Vector3.forward*.5f, Vector3.down); ;
        RaycastHit hitInfo1;
        if(Physics.SphereCast(downRay,1.5f,out hitInfo1))
        {

            //Debug.Log(hitInfo1.transform.tag);
            if (hitInfo1.transform.tag == "Grass")
            {
                ThiedPersonMovement.isGrass = true;
                GrassRealseCD = .5f;
            }
            else
            {
                if (GrassRealseCD < 0)
                    ThiedPersonMovement.isGrass = false;
            }
        }
        Ray upRay = new Ray(transform.position  + Vector3.forward * .5f, Vector3.up); ;
        RaycastHit hitInfo2;
        if (Physics.SphereCast(upRay, .5f, out hitInfo2))
        {

            //Debug.Log(hitInfo2.transform.tag+"---2");
            if (hitInfo2.transform.tag == "Water")
            {
                ThiedPersonMovement.isWater = true;
            }
        //    else
        //    {
        //        ThiedPersonMovement.isWater = false;
        //    }
        }
        else
        {
            ThiedPersonMovement.isWater = false;
        }
    }


}
