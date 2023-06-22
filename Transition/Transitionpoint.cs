using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitionpoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }
    [Header("Transition Info")]
    public string sceneName;
    public TransitionType transitionType;
    public Transitionpoint2.DestinationTag destinationTag;
    private bool canTrans;
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E)&&canTrans==true) 
        {
        SceneController.Instance.TransitionToDestination(this);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans = true;
    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans= false;
    }
}
