using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Single<GameManager>
{
    private CinemachineFreeLook followCamera;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public CharacterStats playerStats;
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>(); 
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if(followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer) {
        endGameObservers.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();   
        }
    }
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<Transitionpoint2>())
        {
            if (item.destinationTag == Transitionpoint2.DestinationTag.ENTER)
                return item.transform;
        }
        return null;
    }
}
