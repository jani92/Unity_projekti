using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumps : MonoBehaviour
{
    
    public Vector3 posDiff = new Vector3(10f, 0f, 0f);
    public float speed = 1.0f;
    private Vector3 dir = Vector3.left;
    private bool isGrounded = false;
    public float jumpspeed = 13f;
    private float ySpeed;
    public float distToDown = 0.1f;
    

    void Start()
    {
       
        

    }
    void FixedUpdate()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
        Vector3 movementDirection = new Vector3(0, 0, 0);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * 10;
        movementDirection.Normalize();

        if (isGrounded == true)
        {
            GroundCheck();
            ySpeed = jumpspeed;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;
        transform.Translate(velocity * Time.deltaTime);
        transform.Translate(dir * speed * Time.deltaTime);
        //move right if x <= -4
        if (transform.position.x <= -4)
        {
            dir = Vector3.right;
        }
        //move left if x <= -4
        else if (transform.position.x >= 4)
        {
            dir = Vector3.left;
        }
    }

    // Update is called once per frame
    void Update()
        
    {

    }
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToDown))
        {

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

}
        
       
    

