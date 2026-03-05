using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameManager : Singleton<GameManager>
{
    // 获取Player的状态
    public CharacterStats playerStats;   //其它GameObject将通过GM来访问PlayerStats
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public CinemachineFreeLook followCamera;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    // 观察者模式，反向注册，让player在生成的时候告诉manager “我是playerStats”
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform;
            followCamera.LookAt = playerStats.transform;
        }
    }
    //在GM中收集所有实现了IEndGameObserver的函数方法
    // 敌人生成的时候添加到endGameObserver，死亡时从列表移除
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if(item.destinationTag == TransitionDestination.DestinationTag.Enter)
            {
                return item.transform;
            }
        }
        return null;
    }
}
