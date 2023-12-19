using UnityEngine;
using System.Collections;

public class RFX1_RandomRotate : MonoBehaviour {

  public int x = 300, y = 300, z = 300;

  private float rangeX, rangeY, rangeZ;
	// Use this for initialization
	void Start ()
	{
	  rangeX = Random.Range(0, 10000)/ 100f;
    rangeY = Random.Range(0, 10000) / 100f;
    rangeZ = Random.Range(0, 10000) / 100f;
	}
	// Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime*Mathf.Sin(Time.time + rangeX)*x,
            Time.deltaTime * Mathf.Sin(Time.time + rangeY)*y,
            Time.deltaTime * Mathf.Sin(Time.time + rangeZ)*z);

    }
}
