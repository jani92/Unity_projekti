using UnityEngine;
using UnityEngine.UI;


public class LeftRight : MonoBehaviour

{
    //[SerializeField] private LayerMask platformLayerMask;
    
    public float speed = 1.0f;
    private Vector3 dir = Vector3.right;
    public float distToRight = 1f;
    public float distToLeft = 1f;
    public Vector3 downRight = new Vector3(1f, -1f, 0f);
    public Vector3 downLeft = new Vector3(-1f, -1f, 0f);
    public CapsuleCollider triggerbox;
    public Text debug;
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
       
    }
    private void FixedUpdate()
    {
        transform.Translate(dir * speed * Time.deltaTime);
        if (Physics.Raycast(transform.position, transform.TransformDirection(downRight), distToRight + 0.1f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(downRight) * 5, Color.red);
            
        }
        else
        {

            
            dir = Vector3.left;
            Debug.DrawRay(transform.position, transform.TransformDirection(downRight) * 5, Color.green);
        }
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(downLeft), distToRight + 0.1f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(downLeft) * 5, Color.red);
            
        }
        else
        {

            
            dir = Vector3.right;
            Debug.DrawRay(transform.position, transform.TransformDirection(downLeft) * 5, Color.green);
        }



        //checking collisions left and right
        if (Physics.Raycast(transform.position, Vector3.right, distToRight))
        {

            dir = Vector3.left;
        }

        if (Physics.Raycast(transform.position, Vector3.left, distToLeft))
        {

            dir = Vector3.right;
        }


    }

    // Update is called once per frame
    void Update()
    {
        



    }
    public void OnTriggerEnter(Collider triggerbox)
    {
        anim.Play("Shoot");
        debug.text = "lala";

        
    }





} 
