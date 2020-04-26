using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uim;
    public GameManager gm;
    public SkillManager sm;

    public GameObject Player;

    public Material normal;
    public Material Death;
    public SkinnedMeshRenderer MR;

    bool isHurt = false;
    bool grounded;
    bool AlreadyDead = false;
    public int hurtDuration = 5;
    public int attackDuration = 20;
    public int attackFrames;
    public int hurtFrames;
    public bool isWalking = false;
    public bool isDissolving = false;
    public float _Time = 0f;
    public int DissolveTime;

    public float Health = 10f;

    private Transform destination;
    NavMeshAgent navMeshAgent;
    private Animator animator;
    public float speed = 1;
    public float fovAngle = 110f;
    public float sightDist = 2f;
    private GameObject player;
    //private SphereCollider col;
    private CapsuleCollider attackCol;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        animator = GetComponent<Animator>();
        player = GameObject.Find("Character");
        //col = GetComponent<SphereCollider>();
        attackCol = GetComponent<CapsuleCollider>();
        destination = player.transform;

        _Time = 0f;
        hurtFrames = hurtDuration;

    }

    void Update()
    {
        /*if (attackFrames > 0)
        {
            attackFrames--;
        }
        else if (attackFrames == 0)
        {
            StopAttacking();
        }*/
        if (isHurt)
        {
            hurtFrames--;
            if (hurtFrames == 0)
            {
                MR.material = normal;
                hurtFrames = hurtDuration;
                isHurt = false;
            }
        }
        if (isDissolving)
        {
            _Time += Time.deltaTime * DissolveTime;
            if (_Time >= 8)
            {
                _Time = 0f;
                isDissolving = false;
                gm.RemoveEnemy(transform.gameObject);
            }
            Death.SetFloat("Time", _Time);
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        //Debug.Log("Stage 1");
        destination = player.transform;
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        //Debug.Log("angle is " + angle);
        Debug.DrawRay(transform.position + transform.up, direction.normalized * sightDist, Color.green);
        if (Health <= 0)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("IsDead", true);
            Destroy(this.gameObject, 3f);

        }
        else if (angle < fovAngle * 0.5f)
        {

            //Debug.Log("Stage 2");
            if (Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, 5f/*col.radius*/))

            {
                //Debug.Log("Stage 3");
                if (hit.collider.gameObject == player && !Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, attackCol.radius))
                {
                    //Debug.Log("Stage 4");
                    SetDestination();
                    navMeshAgent.isStopped = false;
                    animator.SetBool("IsMoving", true);
                }
                else if (hit.collider.gameObject == player && Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, attackCol.radius))
                {
                    navMeshAgent.isStopped = true;
                    Debug.Log("is in");
                    animator.SetBool("IsMoving", false);
                    animator.SetTrigger("Attack");
                }
            }
        }
        if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
        {
            animator.SetBool("IsMoving", false);
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    GameObject go = collision.gameObject;
    //    if (go.layer == 11)
    //    {
    //        Destroy(go);
    //        takeDamage();
    //    }
    //}

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

    public void takeDamage(float damageToTake = 1)
    {
        Health -= damageToTake;
        if (GameManager.DebugMode)
        {
            Debug.Log(transform.name + " Took " + damageToTake + " damage, Remaing Health: " + Health);
        }
        if (Health <= 0 && !AlreadyDead)
        {
            AlreadyDead = true;
            MR.material = Death;
            isDissolving = true;
            //GameManager.RemoveEnemy(transform.gameObject);
            uim.updateScore(5);
            sm.grantXP(1);
        }
    }


    public void death()
    {
        Debug.Log("fully died");
    }

    public void Attacking()
    {
        //attackFrames = attackDuration;
        Debug.Log("attacked");
        //animator.SetBool("attacked", true);

        //yield WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void StopAttacking()
    {
        animator.SetBool("attacked", false);
    }

    /**
     *  Checks if the character is grounded or not
     */
    //private bool isGrounded()
    //{
    //    if (Physics.Raycast(transform.position, -transform.up, 0.5f * 2, 1 << 10))
    //    {
    //        if (GameManager.DebugMode)
    //        {
    //            Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.red);
    //        }
    //        return true;
    //    }
    //    else
    //    {
    //        if (GameManager.DebugMode)
    //        {
    //            Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.blue);
    //        }
    //        return false;
    //    }

    //}
}
