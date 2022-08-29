using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthHover_movement : MonoBehaviour
{

    private Vector3 pos1; //
    private Vector3 pos2;
    public Vector3 posDiff = new Vector3(10f, 0f, 0f);
    public float speed = 1.0f;
    public float volume = 0.5f;

    float moveTimer = 0f;
    public AudioSource audioSource;
    public AudioClip idle;

    // Start is called before the first frame update
    void Start()
    {
        // Takes start position from game scene
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        pos1 = transform.position;
        pos2 = transform.position + posDiff;
    }

    // Update is called once per frame
    void Update()
    {
        moveTimer += Time.deltaTime;

         //Enemy movement back and forth or up and down
        transform.position = Vector3.Lerp(pos1, pos2, (Mathf.Sin(speed * moveTimer) + 1.0f) / 2.0f);          
    }
    public class PlayAudio : MonoBehaviour
    {
        public AudioSource audioSource;

    }
}
