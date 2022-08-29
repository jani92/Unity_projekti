using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingMovement : MonoBehaviour
{
    private Vector3 pos1;
    private Vector3 pos2;
    public Vector3 posDiff = new Vector3(10f, 0f, 0f);
    public float speed = 1.0f;
    private Vector3 dir = Vector3.left;
    private bool isGrounded = false;
    public float jumpspeed = 13f;
    private float ySpeed;
    public float PointA;
    public float PointB;

    void Start()
    {

        pos1 = transform.position;
        pos2 = transform.position + posDiff;

    }
    void FixedUpdate()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
        Vector3 movementDirection = new Vector3(0, 0, 0);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * 10;
        movementDirection.Normalize();

        if (isGrounded == true)
        {
            ySpeed = jumpspeed;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;
        transform.Translate(velocity * Time.deltaTime);
        transform.Translate(dir * speed * Time.deltaTime);
        //move right if x <= pointa
        if (transform.position.x <= PointA)
        {
            dir = Vector3.right;
        }
        //move left if x <= pointb
        else if (transform.position.x >= PointB)
        {
            dir = Vector3.left;
        }
    }

    // Update is called once per frame
    void Update()

    {

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