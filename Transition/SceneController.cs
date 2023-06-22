using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Single<SceneController>, IEndGameObserver
{
    public GameObject playerPretab;
    GameObject player;
    NavMeshAgent playeragent;
    bool fadeFinished;
    public Scencefade sceneFadePretab;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }
    public void TransitionToMain()
    {
        StartCoroutine(Loadmain()); 
    }
    public void TransitionToDestination(Transitionpoint transitionPoint)
    {
        switch (transitionPoint.transitionType) {
            case Transitionpoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case Transitionpoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    IEnumerator Transition(string sceneName,Transitionpoint2.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)
        {


            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPretab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playeragent = player.GetComponent<NavMeshAgent>();
            playeragent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playeragent.enabled = true;
            yield return null;
        }
    }

    private Transitionpoint2 GetDestination(Transitionpoint2.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<Transitionpoint2>();
        for(int i = 0;i<entrances.Length;i++) {
            if (entrances[i].destinationTag==destinationTag)
                return entrances[i];  
        }
        return null;
    }
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }
    IEnumerator LoadLevel(string scene)
    {
        Scencefade fade = Instantiate(sceneFadePretab);
        if (scene!=""){

            yield return StartCoroutine(fade.FadeOut(2.0f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPretab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.0f));
            yield break;


        } 
    }
    IEnumerator Loadmain()
    {
        Scencefade fade = Instantiate(sceneFadePretab);
        yield return StartCoroutine(fade.FadeOut(2.0f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2.0f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished=false;
            StartCoroutine(Loadmain());
        }
    }
}
