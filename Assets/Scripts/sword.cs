using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour
{
    public float speed = 10;
    public float liveTime = 10f;
    public float power = 1.0f;
    protected Transform mtransform;




    // Use this for initialization
    void Start()
    {
        mtransform = this.transform;
        //Destroy(this.gameObject, liveTime);
    }

    // Update is called once per frame
    void Update()
    {
        //mtransform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag.CompareTo("enemy"))!= 0 )
      //  if ("enemy".CompareTo((string)other.tag) != 0)
        {
            return;
        }else{
           
            //Destroy(this.gameObject);
        }
    }
}