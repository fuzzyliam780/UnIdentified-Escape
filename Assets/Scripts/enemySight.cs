using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemySight : MonoBehaviour
{
    public Transform destination;
    NavMeshAgent navMeshAgent;

    public float heightMultiplier;
    public float sightDist = 10;

    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        /*if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent componenet is not attached to " + gameObject.name);
        }
        else
        {
            SetDestination();
        }*/
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.green);
        Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.green);
        if(Physics.Raycast (transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                SetDestination();
            }
        }if(Physics.Raycast (transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                SetDestination();
            }
        }if(Physics.Raycast (transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
}
