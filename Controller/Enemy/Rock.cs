using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public enum RockStates { Hitplayer, HitEnemy, HitNothing }
    public RockStates rockStates;
    [Header("Basic Settings")]
    public float force;
    public GameObject target;
    private Vector3 direction;
    public int damage;
    public GameObject breakEffect;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        FlyToTarget();
        rockStates = RockStates.Hitplayer;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude<1f) { rockStates = RockStates.HitNothing; }
    }
    public void FlyToTarget()
    {
        if (target == null) target = FindObjectOfType<PlayerController>().gameObject;

        direction =(target.transform.position-transform.position+Vector3.up).normalized;
        rb.AddForce(direction * force,ForceMode.Impulse);
    
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.Hitplayer:
                if(other.gameObject.CompareTag("Player")&&other.gameObject.CompareTag("Attackable"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped=true;  
                    other.gameObject.GetComponent<NavMeshAgent>().velocity=direction*force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect,transform.position,Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                
                
                break;

        }
    }
}
