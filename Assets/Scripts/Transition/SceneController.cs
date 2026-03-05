using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                // 异步加载
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName) // 如果场景不同，则异步加载
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            // 在同一场景下，直接传送
            player = GameManager.Instance.playerStats.gameObject; // 获取player
            playerAgent = player.GetComponent<NavMeshAgent>(); // 获取playerAgent，防止传送后继续移动
            playerAgent.enabled = false; // 禁用playerAgent，防止传送后继续移动
            // 设置player的位置和朝向
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true; // 启用playerAgent，传送结束后可以移动
        }


        yield return null;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        foreach (var destination in FindObjectsOfType<TransitionDestination>())
        {
            if (destination.destinationTag == destinationTag)
                return destination;
        }
        return null;
    }
    IEnumerator LoadLevel(string scene)
    {
        if (scene != "")
        {
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //SaveManager.Instance.LoadPlayerData();
            yield break;
        }
    }
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }
    public void TransitionToPreviousLevel()
    {
        // Debug.Log("TransitionToPreviousLevel: " + SaveManager.Instance.SceneName);
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
}
