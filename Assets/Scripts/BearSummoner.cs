using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSummoner : MonoBehaviour
{
    public GameObject prefab;
    public Transform player;
    public int SummoneredNum=10;
    public float SummonerCD=10f;
    public int SummonerRadius = 18;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, player.position)<SummonerRadius&SummoneredNum>0&SummonerCD<0) {
            Vector3 initPos = new Vector3(transform.position.x + Random.Range(-3, 3), transform.position.y, transform.position.z + Random.Range(-3, 3));
            Instantiate(prefab, initPos,new Quaternion(0,0,0,1));
            SummoneredNum--;
            SummonerCD = 10f;
            Debug.Log(Vector3.Distance(this.transform.position, player.position));
        }
        SummonerCD-=Time.deltaTime;
    }
}
