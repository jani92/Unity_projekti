using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshEnemy : MonoBehaviour
{
    public Transform Point1;
    public Transform Point2;
    public Transform Point3;
    public bool Moveback;
    public NavMeshAgent Enemy;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Moveback == true)
        {
            Enemy.SetDestination(Point1.position);
            if (!Enemy.pathPending)
            {
                if (Enemy.remainingDistance <= Enemy.stoppingDistance)
                {
                    Enemy.SetDestination(Point2.position);
                    Moveback = false;
                }
                if (Enemy.remainingDistance <= Enemy.stoppingDistance)
                {
                    Enemy.SetDestination(Point3.position);
                    Moveback = false;
                }
            }

        }
        else
        {
            Enemy.SetDestination(Point2.position);
            if (!Enemy.pathPending)
            {
                if (Enemy.remainingDistance <= Enemy.stoppingDistance)
                {
                    Moveback = true;
                }
            }

        }
    }
}
