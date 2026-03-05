using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
public class SaveManager : Singleton<SaveManager>
{
    public string sceneNameKey = "sceneNameKey";
    public string SceneName { get {return PlayerPrefs.GetString(sceneNameKey);}}
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    // PlayerPrefs & JsonUtility
    public void Save(Object data, string key)
    {
        //PlayerPrefs 属于注册表存储，而Json是文件存储。
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneNameKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data); // 将json数据转换为对象
        }
    }
}
