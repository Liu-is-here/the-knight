using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    public static T Instance { get { return instance; } }
    
    protected virtual void Awake()
    {
        if(instance != null) 
            Destroy(gameObject);
        else
            instance = (T) this;
    }
    // 给其它GameObject提供接口，用于判断当前的单例模式是否已经生成
    public static bool IsInitialized
    {
        get { return instance != null; }
    }
    // 自我销毁
    protected virtual void OnDestroy()
    {
        if(instance == this) 
        {
            instance = null;
        }
    }
}
