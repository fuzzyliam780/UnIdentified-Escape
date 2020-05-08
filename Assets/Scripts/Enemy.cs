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

    [SerializeField]
    bool patrolWaiting;

    [SerializeField]
    float totalWaitTime = 3f;

    [SerializeField]
    float switchProbability = 0.2f;

    [SerializeField]
    List<GameObject> patrolPoints;

    public Material normal;
    public Material Death;
    public SkinnedMeshRenderer MR;

    bool isHurt = false;
    bool grounded;
    public int hurtDuration = 5;
    //public int attackDuration = 20;
    //public int attackFrames;
    public int hurtFrames;
    public bool isWalking = false;
    public bool isDissolving = false;
    public float _Time = 0f;
    public int DissolveTime;
    public float distanceBetweenEnemy;

    public float Health = 10f;

    private Transform destination;
    NavMeshAgent navMeshAgent;
    private Animator animator;
    public int damageDone;
    public float speed = 1;
    public float fovAngle = 110f;
    public float sightDist = 2f;
    public GameObject player;
    private SphereCollider col;
    private CapsuleCollider attackCol;
    private float distance = 100;
    private bool isIn = false;

    public int currentPatrolIndex;
    public bool travelling;
    public bool waiting;
    public bool patrolFoward;
    public float waitTimer = 3;

    void Awake()
    {
        if (uim == null)
        {
            uim = GameObject.Find("MainCanvas").GetComponent<UIManager>();
        }
        if (gm == null)
        {
            gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }
        if (sm == null)
        {
            sm = GameObject.Find("Game Manager").GetComponent<SkillManager>();
        }
    }

    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        animator = GetComponent<Animator>();
        player = GameObject.Find("Character");
        col = GetComponent<SphereCollider>();
        attackCol = GetComponent<CapsuleCollider>();
        destination = player.transform;

        _Time = 0f;
        hurtFrames = hurtDuration;

        foreach (GameObject Wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            patrolPoints.Add(Wp);
        }


        if (patrolPoints != null && patrolPoints.Count >= 2)
        {
            currentPatrolIndex = UnityEngine.Random.Range(0, patrolPoints.Count);

            SetPatrolDestination();
        }

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
                animator.SetBool("IsDead", false);
                gm.RemoveEnemy(transform.gameObject);
            }
            Death.SetFloat("Time", _Time);
        }
        distance = Vector3.Distance(this.transform.position, player.transform.position);

        if (gm.getIfSeen() == false)
        {
            if (travelling && navMeshAgent.remainingDistance <= 1.0f)
            {
                travelling = false;
                animator.SetBool("IsMoving", false);

                if (patrolWaiting)
                {
                    waiting = true;
                    waitTimer = 0f;
                }
                else
                {
                    ChangePatrolPoint();
                    SetPatrolDestination();
                }
            }

            if (waiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= totalWaitTime)
                {
                    waiting = false;

                    ChangePatrolPoint();
                    SetPatrolDestination();
                }

            }
        }

    }

    void FixedUpdate()
    {
        RaycastHit hit;
        //Debug.Log("Stage 1");
        destination = player.transform;
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        bool playerSpotted = gm.getIfSeen();
        //Debug.Log("angle is " + angle);
        Debug.DrawRay(transform.position + transform.up, direction.normalized * sightDist, Color.green);
        if (Health <= 0)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("IsDead", true);

        }
        else
        {
            if (angle < fovAngle * 0.5f)
            {

                //Debug.Log("Stage 2");
                if (Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, col.radius))

                {
                    //Debug.Log("Stage 3");
                    if (hit.collider.gameObject == player && !Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, attackCol.radius) && !isIn && !playerSpotted)
                    {
                        //Debug.Log("Stage 4: " + distance);
                        SetDestination();
                        if (navMeshAgent.isStopped)
                        {
                            navMeshAgent.isStopped = false;

                        }

                        animator.SetBool("IsMoving", true);

                        if (gm.getIfSeen() == false)
                            gm.setIfSeen(true);

                    }
                    if (distance <= distanceBetweenEnemy)
                    {
                        //hit.collider.gameObject == player && Physics.Raycast(transform.position + Vector3.up, direction.normalized * sightDist, out hit, attackCol.radius)
                        isIn = true;
                        navMeshAgent.isStopped = true;
                        Debug.Log("is in");
                        animator.SetBool("IsMoving", false);
                        animator.SetTrigger("Attack");
                    }

                }
            }
            if (distance > distanceBetweenEnemy)
            {
                isIn = false;
            }
            if (playerSpotted && !isIn)
            {
                SetDestination();
                navMeshAgent.isStopped = false;
                animator.SetBool("IsMoving", true);
            }
            if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
            {
                animator.SetBool("IsMoving", false);
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

    private void SetPatrolDestination()
    {
        if (patrolPoints != null)
        {
            Vector3 targetVector = patrolPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            animator.SetBool("IsMoving", true);
            travelling = true;
        }
    }

    public void takeDamage(float damageToTake = 1)
    {
        gm.setIfSeen(true);
        Health -= damageToTake;
        if (GameManager.DebugMode)
        {
            Debug.Log(transform.name + " Took " + damageToTake + " damage, Remaing Health: " + Health);
        }
        if (Health <= 0)
        {
            MR.material = Death;
            navMeshAgent.isStopped = true;
            isDissolving = true;
            uim.updateScore(5);
            sm.grantXP(1);
        }
    }

    public void Attacking()
    {
        //attackFrames = attackDuration;
        Debug.Log("attacked");
        //animator.SetBool("attacked", true);

        //yield WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void damage()
    {
        Debug.Log("delt damage");
        player.GetComponent<Player>().takeDamage(damageDone);
    }
    public void StopAttacking()
    {
        animator.SetBool("attacked", false);
    }

    public void Dissolve()
    {
        isDissolving = true;
    }



    private bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, 0.5f * 2, 1 << 10))
        {
            if (GameManager.DebugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.red);
            }
            return true;
        }
        else
        {
            if (GameManager.DebugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.blue);
            }
            return false;
        }

    }

    public void Ground()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f, 10))
        {
            navMeshAgent.Warp(new Vector3(transform.position.x, hit.point.y, transform.position.z));
        }
    }

    private void ChangePatrolPoint()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= switchProbability)
        {
            patrolFoward = !patrolFoward;
        }

        if (patrolFoward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
        else
        {
            if (--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolPoints.Count - 1;
            }
        }
    }
}
