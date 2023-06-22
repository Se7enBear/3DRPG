using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 20;
    public GameObject rockPretab;
    public Transform handpos;
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            targetStats.GetComponent<NavMeshAgent>().isStopped=true;
            targetStats.GetComponent<NavMeshAgent>().velocity=direction*kickForce;
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock=Instantiate(rockPretab,handpos.position,Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;

        }
    }
}
