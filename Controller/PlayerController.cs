using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private float lastattacktime;

    private GameObject attackTarget;
    private float attackTime;
    private bool isDead;
    private float stopDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        move.Instance.OnmouseClicked += MovetoTarget;
        move.Instance.OnEnemyClicked += AttacktoTarget;
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    private void Start()
    {

        SaveManager.Instance.LoadPlayerData();
    } 
    private void OnDisable()
    {
        if (!move.IsInitialized) return;
        move.Instance.OnmouseClicked -= MovetoTarget;
        move.Instance.OnEnemyClicked -= AttacktoTarget;
    }


    private void Update()
    {
        SwitchAnimation();
        isDead = characterStats.currentHealth == 0;
        if(isDead)
        {
            GameManager.Instance.NotifyObservers();
        }
        attackTime -= Time.deltaTime;
    }
    public void MovetoTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance=stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    public void AttacktoTarget(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.AttackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("died", isDead);
    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.AttackData.attackRange;
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.AttackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;
        if (attackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            lastattacktime = characterStats.AttackData.coolDown;
            yield return null;
        }
    }
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
             
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);

            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
