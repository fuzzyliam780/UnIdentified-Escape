using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetBool("Door Open", true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("ExitTimer");
        }
    }


    IEnumerator ExitTimer()
    {
        yield return new WaitForSeconds(2);
        animator.SetBool("Door Open", false);
    }
}

