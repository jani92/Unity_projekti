using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.right * Time.deltaTime);
        if (transform.position.x>= 4) 
            transform.Translate(Vector3.up * Time.deltaTime);
        if (transform.position.y >= 4)
            transform.Translate(Vector3.left * Time.deltaTime);
        if (transform.position.x <= 0)
            transform.Translate(Vector3.down * Time.deltaTime);
        if(transform.position.y <= 0)
            transform.Translate(Vector3.right * Time.deltaTime);
        

    }
}
