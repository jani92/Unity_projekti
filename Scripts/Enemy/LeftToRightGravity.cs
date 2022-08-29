using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeftToRightGravity : MonoBehaviour
    
{

    public float speed = 1.0f;
    private Vector3 dir = Vector3.left;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
