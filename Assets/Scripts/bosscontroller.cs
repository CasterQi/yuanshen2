using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class bosscontroller : MonoBehaviour
{
    public GameObject[] EffectPrefabs;
    public Animator animator;
    Transform player;
    public GameObject thirdPersion;
    float HP = 30;
    float attackRadius = 30f;
    bool isSkill = false;
    Rigidbody rigidbody;
    float timerX;
    public GameObject camera;
    CinemachineFreeLook freelook;
    public bool isBoom = false;
    public AK.Wwise.Event[] events;
    float skillcd = 30f;
    float fireCD = 30f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rigidbody = this.GetComponent<Rigidbody>();
        
        freelook = camera.GetComponent<CinemachineFreeLook>();


    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 30 & distance > 5 & HP > 0&isSkill == false)
        {
            animator.SetInteger("stepX", 3);
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 1f);
            transform.LookAt(player.position);
            
        }
        else if (distance < 5 & HP > 0 &isSkill==false)
        {
            transform.LookAt(player.position);
            animator.SetInteger("stepX", 1);
        }
        else if(isSkill == false)
        {
            animator.SetInteger("stepX", 0);
        }
        if(isSkill == false&distance<15&skillcd<0)
        {
            UI.textMark = 5;
            print(distance);
            skill1();
        }
        if(HP < 10)
        {
            darkMode.toDark = true;
            //isSkill = true;
            skillcd = 9999f;
        }
        if (HP < 0&fireCD<0)
        {
            
            darkMode.toDark = false;
            UI.textMark = 6;
            Destroy(this.gameObject, 2f);
            Destroy(Instantiate(EffectPrefabs[2], this.transform), 2f);
            //Destroy(Instantiate(EffectPrefabs[4], this.transform), 2f);
            GameObject projectile = Instantiate(EffectPrefabs[2], transform.position, Quaternion.identity) as GameObject; //Spawns the selected projectile
            projectile.transform.rotation = Random.rotation; //Sets the projectiles rotation to look at the point clicked
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 300); //Set the speed of the projectile by applying force to the rigidbody
            fireCD = 0.35f;                                                                            //projectile.tag = "hit";

            Destroy(projectile, 6f);
            
        }
        skillcd -= Time.deltaTime;
        fireCD -= Time.deltaTime;
        
    }
   
    void skill1()
    {   //技能开始
        UI.textMark = 5;
        GameObject.Find("Canvas").GetComponentInChildren<UI>().isboomX = true;

        skillcd = 30f;
        timerX = Time.time;
        isSkill = true;
        animator.SetInteger("stepX", 2);
        Destroy(Instantiate(EffectPrefabs[0],this.transform), 2f);
        rigidbody.velocity += Vector3.up * 15;
        freelook.LookAt = this.transform;
        events[0].Post(gameObject);
        Invoke("skill1C", 1.0f);
        Invoke("skill1D", 2.0f);
        Invoke("skill1A", 3.3f);
        Invoke("skill1B", 3.5f);
        
        //Vector3 targetPos = new Vector3(this.transform.position.x,this.transform.position.y+2, this.transform.position.z);
        //this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, 2 * Time.deltaTime);
        //
        //StartCoroutine(MoveToPosition(targetPos));
    }
    void skill1C()
    {   //悬停空中
        animator.SetInteger("stepX", 0);
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.useGravity = false;
        freelook.LookAt = this.transform;
        events[1].Post(gameObject);
        Destroy(Instantiate(EffectPrefabs[2], this.transform), 2f);
        darkMode.toDark = !darkMode.toDark;


    }
    void skill1D()
    {   //开始下落
        isBoom = true;
        rigidbody.useGravity = true;
        freelook.LookAt = thirdPersion.transform;
        
    }
    void skill1A()
    {   //坐地
        Destroy(Instantiate(EffectPrefabs[1], this.transform), 2f);
        isBoom = false;
        events[2].Post(gameObject);

    }
    void skill1B()
    {
        
        isSkill = false;
    }

    //IEnumerator MoveToPosition(Vector3 targetPos)
    //{
    //    int i = 1;
    //    while (true)
    //    {
    //        if (Vector3.Distance(this.transform.position, targetPos) < .5f)
    //        {
    //            yield break;
    //        }
    //        this.transform.localPosition = Vector3.MoveTowards(this.transform.position, targetPos, 2 * Time.deltaTime);
    //        print(i++);
    //        yield return 0;
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.tag == "enemy")
        {
            //do nothing;
        }
        else if (other.tag == "sword")
        {
            HP--;
            //model.CopyPropertiesFromMaterial(heartedModel);
            Invoke("backtooriginalmaterial", .1f);
        }
        else if (other.tag == "Player" | other.tag == "spell")
        {
            HP -= 5;
            //model.CopyPropertiesFromMaterial(heartedModel);
            Invoke("backtooriginalmaterial", .1f);
        }

    }
}
