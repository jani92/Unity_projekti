using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{

    public Vector2 size = new Vector2(2f, 2f);
    public bool canBeMoved = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTimer > 0f)
        {
            moveTimer = Mathf.Max(0f, moveTimer - Time.deltaTime);
            transform.position = Vector3.Lerp(new Vector3(moveFrom.x, moveFrom.y, 0f), new Vector3(moveTo.x, moveTo.y, 0f), (moveTime - moveTimer) / moveTime);
            //check for dropping down after a move
            if (moveTimer == 0f && !Physics.CheckBox(transform.position + Vector3.down * (size.y * 0.5f + 0.5f), new Vector3(size.x * 0.5f - 0.5f, 0.1f, 0.1f)))
                Move(Vector2.down, 0.2f, false);
        }
    }

    float moveTime = 1f;
    float moveTimer = 0f;
    Vector2 moveFrom;
    Vector2 moveTo;
    public void Move(Vector2 delta, float time, bool setMoved = true)
    {
        if (moveTimer > 0f)
            return;
        if (setMoved)
            canBeMoved = false;
        moveTime = time;
        moveTimer = time;
        moveFrom = new Vector2(transform.position.x, transform.position.y);
        moveTo = moveFrom + delta;
    }

    public bool CanMove() { return moveTimer == 0f && canBeMoved; }
}
