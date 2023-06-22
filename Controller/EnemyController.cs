using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent (typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    protected CharacterStats characterStats;
    private float lasttimeattack;
    private Collider coll;
    [Header("Basic Settings")]
    public float sightRadius;
    protected GameObject attackTarget;
    public bool isGuard;
    public float lookAtTime;
    private float remainLookAtTime;
    private Quaternion guardRotation;


    [Header("Patrol State")]
    public float PatrolRange;
    private Vector3 waypoint;
    private Vector3 guardpoint;
    private Animator anim;
    private float speed;
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerisdead;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        characterStats = GetComponent<CharacterStats>();
        guardpoint = transform.position;
        remainLookAtTime = lookAtTime;
        guardRotation = transform.rotation;
        coll=GetComponent<Collider>();
    }
    private void Start()
    {
        if(isGuard) { enemyStates= EnemyStates.GUARD; }
        else { enemyStates = EnemyStates.PATROL; GetNewWayPoint(); }
        GameManager.Instance.AddObserver(this);
    }
    //private void OnEnable()
   // {
      //  GameManager.Instance.AddObserver(this);
  //  }
    private void Update()
    {
        if(characterStats.currentHealth==0) isDead = true;
        if (!playerisdead)
        {
            SwitchStates();
            SwitchAnimation();
            lasttimeattack -= Time.deltaTime;
        }
    }
   private void OnDisable()
    {if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    void SwitchAnimation()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("chase", isChase);
        anim.SetBool("follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("died", isDead);
    }
    void SwitchStates()
    {
        if(isDead)enemyStates = EnemyStates.DEAD;

        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardpoint)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardpoint;

                    if (Vector3.SqrMagnitude(guardpoint - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.5f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(waypoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if(remainLookAtTime>0)
                        remainLookAtTime-= Time.deltaTime;
                    else
                    GetNewWayPoint();
                }
                else
                {
                    isWalk=true;
                    agent.destination = waypoint;
                }
                break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;
                agent.isStopped = false;
                agent.speed = speed;
                if(!FoundPlayer())
                {
                    isFollow = false;
                    agent.isStopped = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard) enemyStates = EnemyStates.GUARD;
                    else enemyStates = EnemyStates.PATROL;
                    }
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow=false;
                    agent.isStopped = true;
                    if (lasttimeattack < 0)
                    {
                        lasttimeattack = characterStats.AttackData.coolDown;
                        characterStats.isCritical = Random.value < characterStats.AttackData.criticalChance;
                        Attack();
                    }

                }


                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange() )
        {
            anim.SetTrigger("attack");
        }
        if( TargetInSkillRange() )
        {
            anim.SetTrigger("skill");
        }
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders) {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            } 
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.AttackData.attackRange;
        else return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.AttackData.skillRange;
        else return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-PatrolRange, PatrolRange);
        float randomZ=  Random.Range(-PatrolRange, PatrolRange);
        Vector3 randomPoint = new Vector3(guardpoint.x + randomX, transform.position.y, guardpoint.z + randomZ);
        NavMeshHit hit;
        waypoint = NavMesh.SamplePosition(randomPoint, out hit, PatrolRange, 1) ? hit.position : transform.position;
    
    }
     void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        anim.SetBool("victory", true);
        isChase=false;
        playerisdead = true;
        isWalk=false;
        attackTarget = null;

    }
}