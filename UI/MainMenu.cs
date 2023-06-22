using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newBtn;
    Button contineBtn;
    Button quitBtn;

    PlayableDirector director;
    private void Awake()
    {
        newBtn = transform.GetChild(1).GetComponent<Button>();
        contineBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn= transform.GetChild(3).GetComponent<Button>();


        newBtn.onClick.AddListener(PlayTimeline);
        quitBtn.onClick.AddListener(QuitGame);
        contineBtn.onClick.AddListener (ContinueGame);
        director=FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }
    void PlayTimeline()
    {
        director.Play();
    }
    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }
    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
