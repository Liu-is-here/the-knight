using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;   //for key name 'event'



public class MouseManager : Singleton<MouseManager>
{
    RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    public Texture2D point, doorway, attack, target, arrow;

    // 重写唤醒方法
    protected override void Awake()
    {
        base.Awake();   // 父类Awake
        
        // 子类
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo))
        {
            // 切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, Vector2.zero, CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, Vector2.zero, CursorMode.Auto);
                    break;
                case "Attackable":
                    Cursor.SetCursor(attack, Vector2.zero, CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, Vector2.zero, CursorMode.Auto);
                    break;
                // 如果标签不匹配任何已知情况，保持鼠标指针为point
                default:
                    Cursor.SetCursor(point, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            // 如果点击到标签为“Ground”的物体，检查事件是否存在，触发事件。
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            // 如果点击到标签为“Ground”的物体，检查事件是否存在，触发事件。
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            // 如果点击到标签为“Ground”的物体，检查事件是否存在，触发事件。
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}
