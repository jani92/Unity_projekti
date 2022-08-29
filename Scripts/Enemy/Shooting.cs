using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{
    
    GameObject player;



    bool m_Started;
    public LayerMask m_LayerMask;
    public Transform Player;
    public float range = 5.0f;
    public float bulletImpulse = 20.0f;
    Vector3 startposition;
    public Transform ammusPrefab;
    private Transform ammus;
    private bool onRange = true;
    public AudioSource audioSource;
    public AudioClip ShootingSound;
    public float volume = 0.5f;
    public Rigidbody projectile;


    // Start is called before the first frame update
    
    void Start()
    {
      

        

        startposition = gameObject.transform.position;
        player = GameObject.FindWithTag("Player");


        InvokeRepeating("Shoot", 2, 1f);
        

    }
   
    void Shoot()
    {

        if (onRange)
        {

            audioSource.PlayOneShot(ShootingSound);
            ammus = Instantiate(ammusPrefab, transform.position + transform.forward, transform.rotation);
            projectile = ammus.GetComponent<Rigidbody>();
            
            Rigidbody bullet = (Rigidbody)Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            bullet.AddForce(transform.forward * bulletImpulse, ForceMode.Impulse);

            Destroy(bullet.gameObject, 0.5f);
            Destroy(ammus.gameObject);

        }


    }
    // Update is called once per frame
    void Update()
    {
       

        //if at range look at player
        onRange = Vector3.Distance(transform.position, player.transform.position) < range;

        if (onRange)
            transform.LookAt(player.transform);
    }
    

}
    



