using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Single<SaveManager>
{
    string scenceName="Level";
    public string SceneName { get { return PlayerPrefs.GetString(scenceName); } }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadPlayerData();
        }
    }
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.CharacterData, GameManager.Instance.playerStats.CharacterData.name);
    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.CharacterData, GameManager.Instance.playerStats.CharacterData.name);
    }
    public void Save(Object data,string key)
    {
        var jsonData=JsonUtility.ToJson(data,true);  
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(scenceName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void Load(Object data,string key)
    {
        if(PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);
        }
    }
}
