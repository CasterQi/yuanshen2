using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Material heartedModel; 
    public Material originalModel;
    public Material dissolve;
    public Shader dissolve_shader;
    float randomWalkTime;//随机行走时
    float randomRot;
    float t = -.5f;
    int HP=5;
    public Animator animator;
    float timer_eat=15f;
    float timer_walk = -15f;
    Material model;
    public AK.Wwise.Event Event;
    bool isEventPosted = false;
    public AK.Wwise.Event EventCry;
    float cryCD;
    
    // Start is called before the first frame update
    void Start()
    {
        cryCD = Random.Range(10, 30);
        //var tm = GetComponentInChildren<RFX1_TransformMotion>(true);
        //if (tm != null) tm.CollisionEnter += Tm_CollisionEnter;
        timer_eat = Random.Range(2, 15);
        model = GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (cryCD < 0) { EventCry.Post(gameObject); cryCD = Random.Range(30, 45); }

        if (timer_eat<0&HP>0&timer_walk<0){
            if(Random.Range(1,100)<50){
                randomWalkTime = Random.Range(3, 6);
                animator.SetBool("isWalking", true);
                Invoke("animationStop", randomWalkTime);
                timer_walk = Random.Range(20,35);
                randomRot = Random.Range(-30, 30);
                transform.rotation = Quaternion.Euler(0, randomRot+ transform.rotation.eulerAngles.y, 0);
            }else{
                animator.SetBool("isEating", true);
                Invoke("animationStop", 5);
                timer_eat = Random.Range(15, 20);
            }

        }
        if(randomWalkTime>0){
            this.transform.position += transform.forward*Time.deltaTime*.9f;

        }


        if(HP<=0){
            if (!isEventPosted) { Event.Post(gameObject); isEventPosted = true; }
            //Vector3 vector = new Vector3(this.transform.position.x, this.transform.position.y -Time.deltaTime, this.transform.position.z);
            //this.transform.position -= Vector3.down*Time.deltaTime;
            animator.SetBool("isEating", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isDead", true);
            //model.CopyPropertiesFromMaterial(dissolve);
            model.shader = dissolve_shader;
            //dissolve
            Material mats = GetComponentInChildren<SkinnedMeshRenderer>().material;
            mats.SetFloat("_Cutoff", t *.2f );
            t += Time.deltaTime;

            // Unity does not allow meshRenderer.materials[0]...
            model = mats;
            //enddissolve
            Destroy(this.gameObject, 6f);
        }
        timer_eat -= Time.deltaTime;
        timer_walk -= Time.deltaTime;
        randomWalkTime -= Time.deltaTime;
        cryCD -= Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (other.tag=="enemy")
        {
            //do nothing;
        }else if(other.tag=="sword"){
            HP--;
            model.CopyPropertiesFromMaterial(heartedModel);
            Invoke("backtooriginalmaterial", .1f);
        }else if(other.tag == "spell"){
            HP -= 5;
            model.CopyPropertiesFromMaterial(heartedModel);
            Invoke("backtooriginalmaterial", .1f);
        }

    }

    void animationStop(){
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", false);
    }

    void backtooriginalmaterial(){
        model.CopyPropertiesFromMaterial(originalModel);
    }

    //private void Tm_CollisionEnter(object sender, RFX1_TransformMotion.RFX1_CollisionInfo e)
    //{
    //    Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
    //}

}
