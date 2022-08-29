using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class following_Enemy : MonoBehaviour
{    
    GameObject player;   
    public float speed = 3f;
    public float viewDistance = 5F;
    Vector3 startposition;

    // Start is called before the first frame update
    void Start()
    {
        startposition = gameObject.transform.position;       
        player = GameObject.FindWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {
        //rotate to look at the player
        //transform.LookAt(player.transform);
        //transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation


        //move towards the player
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistance &&
            !Physics.Raycast(transform.position, player.transform.position - transform.position, Vector3.Distance(transform.position, player.transform.position)))
        {
            Vector3 direction = Vector3.Normalize(player.transform.position - transform.position);
            transform.Translate(direction * speed * Time.deltaTime); //Start moving towards player when distance is 5
        } else
        {
            transform.Translate(0f, Mathf.Cos(Time.time * 2f) * Time.deltaTime, 0f);
        }
    }
    public void ResetPosition()
    {
        enemyreset = false;
    }

    bool enemyreset = false;
    float deathFreezeTimer = 0f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (enemyreset)
                return;
            enemyreset = true;
            deathFreezeTimer = 0.5f;
            gameObject.transform.position = startposition;
            StartCoroutine(Oota());
        }

    }
    IEnumerator Oota()
    {
        yield return new WaitForSeconds(5f);
    }
    
    private void FixedUpdate()
    {
        if (enemyreset)
        {
            deathFreezeTimer = Mathf.Max(0f, deathFreezeTimer - Time.deltaTime);
            if (deathFreezeTimer == 0f) ResetPosition();
            return;
        }

    }
   
}
