using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorAbility : MonoBehaviour
{
    public Transform anchorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attached)
        {
            shotTimer += Time.deltaTime;
            anchor.position = Vector3.Lerp(anchorShotPos, anchoredObject.TransformPoint(anchorHitPos), Mathf.Min(1f, Mathf.Pow(shotTimer * 16f, 2f)));
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, anchor.TransformPoint(Vector3.left * 0.75f));
        }
    }

    float reelingSpeed = 15f;
    [System.NonSerialized] public Vector2 shooterReelPos;
    [System.NonSerialized] public Vector2 targetReelPos;
    public void Reel()
    {
        if (shotTimer > 0.1f)
        {
            shooterReelPos += anchorDir * Time.deltaTime * reelingSpeed;
            //targetReelPos -= anchorDir * Time.deltaTime * reelingSpeed;
        }
    }

    public bool Attached() { return attached; }
    public bool CanRelease() { return shotTimer > 0.3f; }
    public Vector2 AnchorDir() { return anchorDir; }
    public float ShotTimer() { return shotTimer; }
    public Transform AnchoredObject() { return anchoredObject; }
    public enum ObjectType { enemy, platform }
    public ObjectType AnchoredObjectType() { return anchoredObjType; }

    bool attached = false;
    float shotTimer = 0f;
    Vector3 anchorShotPos;
    Vector3 anchorHitPos;
    Transform anchoredObject;
    ObjectType anchoredObjType;
    //float anchoredObjOriginalDrag;
    Vector2 anchorDir;
    Transform anchor;
    LineRenderer lineRend;
    public void ShootAnchor(Vector2 pos, Vector2 dir, float range)
    {
        if (attached)
            return;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(pos.x, pos.y, 0f), new Vector3(dir.x, dir.y, 0f), out hit, range) && (hit.transform.tag == "KillableEnemy" || (hit.transform.tag == "MovablePlatform" && hit.transform.GetComponent<MovablePlatform>().CanMove())))
        {
            attached = true;
            shotTimer = 0f;
            anchorShotPos = new Vector3(pos.x, pos.y, transform.position.z);
            anchoredObject = hit.transform;
            anchoredObjType = (hit.transform.tag == "KillableEnemy" ? ObjectType.enemy : ObjectType.platform);
            //anchoredObjOriginalDrag = anchoredObject.drag;
            anchorHitPos = anchoredObject.InverseTransformPoint(hit.point);
            anchorDir = dir;
            anchor = Instantiate(anchorPrefab, anchorShotPos, Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg + 90f)));
            lineRend = anchor.GetComponentInChildren<LineRenderer>();
            lineRend.SetPosition(0, anchorShotPos);
            lineRend.SetPosition(1, anchor.position);

            shooterReelPos = anchorShotPos;
            targetReelPos = anchoredObject.position;
        }
    }

    public void ShootAutoAimAnchor(Vector2 pos, float range)
    {
        if (attached)
            return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("KillableEnemy");
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("MovablePlatform");
        GameObject closestTarget = null;
        float closestDist = 0f;
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(pos, new Vector2(enemy.transform.position.x, enemy.transform.position.y));
            if (dist <= range && !Physics.Raycast(enemy.transform.position, new Vector3(pos.x, pos.y, 0f) - enemy.transform.position, Vector3.Distance(enemy.transform.position, new Vector3(pos.x, pos.y, 0f))))
            {
                if (closestTarget == null || dist < closestDist)
                {
                    closestTarget = enemy;
                    closestDist = dist;
                }
            }
        }
        foreach (GameObject platform in platforms)
        {
            float dist = Vector2.Distance(pos, new Vector2(platform.transform.position.x, platform.transform.position.y));
            if (platform.GetComponent<MovablePlatform>().CanMove() && dist <= range && !Physics.Raycast(platform.transform.position, new Vector3(pos.x, pos.y, 0f) - platform.transform.position, Vector3.Distance(platform.transform.position, new Vector3(pos.x, pos.y, 0f))))
            {
                if (closestTarget == null || dist < closestDist)
                {
                    closestTarget = platform;
                    closestDist = dist;
                }
            }
        }
        if (closestTarget != null)
        {
            Vector2 dir = (new Vector2(closestTarget.transform.position.x, closestTarget.transform.position.y) - pos).normalized;
            attached = true;
            shotTimer = 0f;
            anchorShotPos = new Vector3(pos.x, pos.y, transform.position.z);
            anchoredObject = closestTarget.transform;
            anchoredObjType = (closestTarget.transform.tag == "KillableEnemy" ? ObjectType.enemy : ObjectType.platform);
            //anchoredObjOriginalDrag = anchoredObject.drag;
            anchorHitPos = anchoredObject.InverseTransformPoint(anchoredObject.position);
            anchorDir = dir;
            anchor = Instantiate(anchorPrefab, anchorShotPos, Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg + 90f)));
            lineRend = anchor.GetComponentInChildren<LineRenderer>();
            lineRend.SetPosition(0, anchorShotPos);
            lineRend.SetPosition(1, anchor.position);

            shooterReelPos = anchorShotPos;
            targetReelPos = anchoredObject.position;
        }
    }

    public void ReleaseAnchor()
    {
        if (!attached)
            return;
        //if (anchoredObject != null)
        //    anchoredObject.drag = anchoredObjOriginalDrag;
        attached = false;
        Destroy(anchor.gameObject);
    }
}
