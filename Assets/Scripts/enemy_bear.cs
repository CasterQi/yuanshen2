using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_bear : MonoBehaviour
{
    Transform player;
    public Material heartedModel; 
    public Material originalModel;
    public Material dissolve;
    public Shader dissolve_shader;
    public int attackRadius = 28;
    public int enemyCatgory = 0;
    public AK.Wwise.Event Event;
    public AK.Wwise.Event EventDie;
    float cryCD;
    float randomWalkTime;//随机行走时
    float randomRot;
    int HP=5;
    public Animator animator;
    float timer_eat=15f;
    float timer_walk = -15f;
    float scriptInitTime = 0f;
    Material model;
    float t2 = 1.5f;
    float t = -.5f;
    bool isEventPosted = false;
    bosscontroller bosscontroller;
    Rigidbody rigidbody;
    public GameObject[] EffectPrefabs;
    float objTimer =0 ;
    bool isboomed = false;
    bool isboom1 = false;
    float boomcd = 3f;
    // Start is called before the first frame update
    void Start()
    {
        //var tm = GetComponentInChildren<RFX1_TransformMotion>(true);
        //if (tm != null) tm.CollisionEnter += Tm_CollisionEnter;
        Event.Post(gameObject);
        cryCD = Random.Range(10,20);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer_eat = Random.Range(2, 15);
        model = GetComponentInChildren<SkinnedMeshRenderer>().material;
        bosscontroller=GameObject.Find("C22").GetComponent<bosscontroller>();
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        objTimer += Time.deltaTime;
        //闲逛
        //if(timer_eat<0&HP>0&timer_walk<0){
        //    if(Random.Range(1,100)<50){
        //        randomWalkTime = Random.Range(3, 6);
        //        animator.SetBool("isWalking", true);
        //        Invoke("animationStop", randomWalkTime);
        //        timer_walk = Random.Range(20,35);
        //        randomRot = Random.Range(-30, 30);
        //        transform.rotation = Quaternion.Euler(0, randomRot+ transform.rotation.eulerAngles.y, 0);
        //    }else{
        //        animator.SetBool("isStanding", true);
        //        Invoke("animationStop", 5);
        //        timer_eat = Random.Range(15, 20);
        //    }
        //}
        //if(randomWalkTime>0){
        //    this.transform.position += transform.forward*Time.deltaTime*.9f;
        //}
        if (isboom1& boomcd < 0)
        {
            Destroy(Instantiate(EffectPrefabs[1], this.transform), 2f);
            boomcd = 1.5f;
            //Invoke("boom2", .5f);//Random.value*2
        }
        boomcd -= Time.deltaTime;
        if (bosscontroller.isBoom&objTimer>8&isboomed==false)
        {
            isboomed = true;
            Destroy(Instantiate(EffectPrefabs[0], this.transform), 2f);
            rigidbody.velocity += Vector3.up * 10;
            Invoke("boom1", 1f);
            Invoke("boom2", 2.5f+ Random.value * 2);//
            return;

        }


        if (cryCD < 0) { Event.Post(gameObject);cryCD = Random.Range(10,20); }
        if(Vector3.Distance(transform.position,player.position)<attackRadius&HP>0){
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 1f);
            transform.LookAt(player.position);
            animator.SetBool("isWalking", true);
        }else{
            animationStop();
        }



        if(HP<=0){
            if (!isEventPosted) { EventDie.Post(gameObject); isEventPosted = true; }
            //Vector3 vector = new Vector3(this.transform.position.x, this.transform.position.y -Time.deltaTime, this.transform.position.z);
            //this.transform.position -= Vector3.down*Time.deltaTime;
            animator.SetBool("isStanding", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isDead", true);
            //model.CopyPropertiesFromMaterial(dissolve);
            //model.shader = dissolve_shader;
            //dissolve
            Material mats = GetComponentInChildren<SkinnedMeshRenderer>().material;
            mats.SetFloat("_Cutoff", t *.2f );
            t += Time.deltaTime;
            // Unity does not allow meshRenderer.materials[0]...
            model = mats;
            //enddissolve
            Destroy(this.gameObject, 5f);
        }
        if(scriptInitTime<0&t2>-.1f){
            //model.shader = dissolve_shader;
            //dissolve
            Material mats = GetComponentInChildren<SkinnedMeshRenderer>().material;
            mats.SetFloat("_Cutoff", t2);
            model = mats;

        }
        t2 -= Time.deltaTime * .15f;
        timer_eat -= Time.deltaTime;
        timer_walk -= Time.deltaTime;
        randomWalkTime -= Time.deltaTime;
        scriptInitTime -= Time.deltaTime;
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
        }else if(other.tag == "Player"|other.tag == "spell"){
            HP -= 5;
            model.CopyPropertiesFromMaterial(heartedModel);
            Invoke("backtooriginalmaterial", .1f);
        }

    }

    void animationStop(){
        //animator.SetBool("isStanding", false);
        animator.SetBool("isWalking", false);
    }

    void backtooriginalmaterial(){
        model.CopyPropertiesFromMaterial(originalModel);
    }

    void boom1() {

        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.useGravity = false;
        Destroy(Instantiate(EffectPrefabs[1], this.transform), 2f);
        isboom1 = true;
    }

    void boom2()
    {
        //Destroy(Instantiate(EffectPrefabs[2], this.transform), 3f);
        
        GameObject projectile = Instantiate(EffectPrefabs[2], transform.position, Quaternion.identity) as GameObject; //Spawns the selected projectile
        projectile.transform.LookAt(GameObject.Find("missileTarget").transform); //Sets the projectiles rotation to look at the point clicked
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 300); //Set the speed of the projectile by applying force to the rigidbody
        //projectile.tag = "hit";
        HP = -99;
        Destroy(projectile, 6f);


    }
    //private void Tm_CollisionEnter(object sender, RFX1_TransformMotion.RFX1_CollisionInfo e)
    //{
    //    Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
    //}

}
