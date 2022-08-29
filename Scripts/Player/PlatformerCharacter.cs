using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCharacter : MonoBehaviour
{

    public Vector2 size = new Vector2(1f, 1f);
    public Vector2 gravity = new Vector2(0f, -40f);
    public float airResistance = 0f;
    public float wallFriction = 0.01f;

    protected Vector2 prePosition;
    protected Vector2 position;
    protected Vector2 velocity;
    protected bool grounded;
    protected bool ceilingHit;
    protected bool wallHitRight;
    protected bool wallHitLeft;


    protected virtual void Start()
    {
        //Time.timeScale = 0.2f;
    }

    //fysiikkojen päivitys fixedupdatessa, jotta fysiikat käyttäytyisivät samalla tavalla eri fps lukemilla.
    protected virtual void FixedUpdate()
    {
        UpdatePhysics(Time.fixedDeltaTime);
    }

    //liikutetaan hahmoa fysiikkojen mukaisesti ja tarkistetaan törmäyksiä
    protected virtual void Update()
    {
        ApplyPhysics(Time.deltaTime);
        SetPosition();

        if (finishCollisionFlag)
            Finish();

        if (deathCollisionFlag || (enemyCollision != null/* && !enemyStompedFlag*/))
        {
            Die();
        }

        //if (enemyCollision != null && enemyStompedFlag)
        //{
        //    Stomp(enemyCollision);
        //}

        finishCollisionFlag = false;
        deathCollisionFlag = false;
        //enemyStompedFlag = false;
        enemyCollision = null;
    }

    void SetPosition()
    {
        transform.position = position;// Vector2.Lerp(transform.position, position, Time.deltaTime * 100f);
    }

    public void Teleport(Vector2 pos)
    {
        position = pos;
        prePosition = position;
        velocity = Vector2.zero;
        ResetCollisionFlags();
    }

    protected virtual void Die() { }
    protected virtual void Finish() { }
    //protected virtual void Stomp(Transform enemy) { }

    protected void ApplyDrag(float x, float y)
    {
        velocity = new Vector2(
            Mathf.Lerp(velocity.x, velocity.x / 2f, x),
            Mathf.Lerp(velocity.y, velocity.y / 2f, y)
            );
    }

    void UpdatePhysics(float deltaTime)
    {
        velocity += gravity * deltaTime;
        ApplyDrag(deltaTime * airResistance, deltaTime * airResistance + (wallHitRight || wallHitLeft ? wallFriction : 0f));

        //change velocities
        if (grounded)
            velocity = new Vector2(velocity.x, Mathf.Max(0f, velocity.y));
        if (ceilingHit)
            velocity = new Vector2(velocity.x, Mathf.Min(0f, velocity.y));
        if (wallHitRight && velocity.x > 0f)
        {
            float speedChange = velocity.x - Mathf.MoveTowards(velocity.x, 0f, deltaTime * 80f);
            velocity = new Vector2(velocity.x - speedChange, velocity.y + Mathf.Abs(speedChange) * 0.25f);
        }
        if (wallHitLeft && velocity.x < 0f)
        {
            float speedChange = velocity.x - Mathf.MoveTowards(velocity.x, 0f, deltaTime * 80f);
            velocity = new Vector2(velocity.x - speedChange, velocity.y + Mathf.Abs(speedChange) * 0.25f);
        }
    }
    void ApplyPhysics(float deltaTime)
    {
        ResetCollisionFlags();

        float steps = Mathf.Round(Mathf.Clamp(1f + velocity.magnitude * deltaTime * 10f, 1f, 20f));
        for (float step = 0f; step < steps; step += 1f)
        {
            position += velocity * deltaTime / steps;
            Collide();
        }
        //Debug.Log(steps);
    }
    void ResetCollisionFlags()
    {
        grounded = false;
        ceilingHit = false;
        wallHitRight = false;
        wallHitLeft = false;
    }

    void Collide()
    {
        float spacing = 0.01f;
        float wallContact = 0.01f;

        //x collisions
        Vector2 posWithPreY = new Vector2(position.x, prePosition.y);

        float rightMiddle = RaycastPenetration(posWithPreY, Vector2.right, size.x * 0.5f + wallContact);
        float rightUpper = RaycastPenetration(posWithPreY + Vector2.up * (size.y * 0.5f - spacing), Vector2.right, size.x * 0.5f + wallContact);
        float rightLower = (velocity.y < 0f ? 0f : RaycastPenetration(posWithPreY - Vector2.up * (size.y * 0.5f - spacing), Vector2.right, size.x * 0.5f + wallContact));

        float leftMiddle = RaycastPenetration(posWithPreY, Vector2.left, size.x * 0.5f + wallContact);
        float leftUpper = RaycastPenetration(posWithPreY + Vector2.up * (size.y * 0.5f - spacing), Vector2.left, size.x * 0.5f + wallContact);
        float leftLower = (velocity.y < 0f ? 0f : RaycastPenetration(posWithPreY - Vector2.up * (size.y * 0.5f - spacing), Vector2.left, size.x * 0.5f + wallContact));

        //fix x position
        position += Vector2.right * (Mathf.Max(0f, Mathf.Max(leftMiddle - wallContact, Mathf.Max(leftUpper - wallContact, leftLower - wallContact))) -
            Mathf.Max(0f, Mathf.Max(rightMiddle - wallContact, Mathf.Max(rightUpper - wallContact, rightLower - wallContact))));

        //y collisions
        Vector2 posWithPreX = new Vector2(prePosition.x, position.y);

        float downMiddle = RaycastPenetration(posWithPreX, Vector2.down, size.y * 0.5f + wallContact);
        float downRight = RaycastPenetration(posWithPreX + Vector2.right * (size.x * 0.5f - spacing), Vector2.down, size.y * 0.5f + wallContact);
        float downLeft = RaycastPenetration(posWithPreX + Vector2.left * (size.x * 0.5f - spacing), Vector2.down, size.y * 0.5f + wallContact);

        float upMiddle = RaycastPenetration(posWithPreX, Vector2.up, size.y * 0.5f + wallContact);
        float upRight = RaycastPenetration(posWithPreX + Vector2.right * (size.x * 0.5f - spacing), Vector2.up, size.y * 0.5f + wallContact);
        float upLeft = RaycastPenetration(posWithPreX - Vector2.right * (size.x * 0.5f - spacing), Vector2.up, size.y * 0.5f + wallContact);

        //fix y position
        position += Vector2.up * (Mathf.Max(0f, Mathf.Max(downMiddle - wallContact, Mathf.Max(downRight - wallContact, downLeft - wallContact))) -
            Mathf.Max(0f, Mathf.Max(upMiddle - wallContact, Mathf.Max(upRight - wallContact, upLeft - wallContact))));

        prePosition = position;

        if (velocity.y < 0f && (downMiddle > 0 || downRight > 0f || downLeft > 0f))
            grounded = true;
        if (upMiddle > 0f || upRight > 0f || upLeft > 0f)
            ceilingHit = true;
        if (rightMiddle > 0f || rightUpper > 0f || rightLower > 0f)
            wallHitRight = true;
        if (leftMiddle > 0f || leftUpper > 0f || leftLower > 0f)
            wallHitLeft = true;
    }

    Transform enemyCollision = null;
    //bool enemyStompedFlag = false;
    bool deathCollisionFlag = false;
    bool finishCollisionFlag = false;
    float RaycastPenetration(Vector2 pos, Vector2 dir, float length)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, dir, out hit, length)) {
            if (hit.transform.tag == "Death")
                deathCollisionFlag = true;
            if (hit.transform.tag == "Finish")
                finishCollisionFlag = true;
            if (hit.transform.tag == "Enemy" || hit.transform.tag == "KillableEnemy")
            {
                enemyCollision = hit.transform;
                return 0f;
            }
            //if (hit.transform.tag == "KillableEnemy")
            //{
            //    enemyCollision = hit.transform;
            //    if (dir.x == 0f && dir.y < 0f)
            //        enemyStompedFlag = true;
            //    return 0f;
            //}
            return length - hit.distance;
        } else {
            return 0f;
        }
    }


}
