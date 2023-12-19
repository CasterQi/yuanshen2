using UnityEngine;
using System.Collections;

public class RFX1_DeactivateByTime : MonoBehaviour {

    public float DeactivateTime = 3;

	void OnEnable ()
	{
        Invoke("DeactivateThis", DeactivateTime);
    }

    void OnDisable()
    {
        CancelInvoke("DeactivateThis");
    }

    void DeactivateThis()
    {
        gameObject.SetActive(false);
	}
}
