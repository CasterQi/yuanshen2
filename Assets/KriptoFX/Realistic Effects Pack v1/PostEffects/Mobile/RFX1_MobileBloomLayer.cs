using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RFX1_MobileBloomLayer : MonoBehaviour
{

	// Use this for initialization
	void Start () {
        gameObject.layer = LayerMask.NameToLayer("BloomMobileEffect");
	}
}
