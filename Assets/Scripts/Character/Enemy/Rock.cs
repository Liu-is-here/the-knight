using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing } // state of the rock
    public RockStates rockState;
    private Rigidbody rb;
    public GameObject launcher; // 记录是谁扔出的石头
    [Header("Basic Settings")]
    public int damage;
    public GameObject target;//石头人击打目标
    private Vector3 direction;
    public GameObject breakEffect;

    [Header("Throwing Settings (Golem)")]
public float forceByEnemy = 15f;
public float upwardForceByEnemy = 3f; // 向上补偿

[Header("Counter Settings (Player)")]
public float forceByPlayer = 25f;
public float upwardForceByPlayer = 8f; // 玩家反击通常需要更强的向上力来克服重力
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rockState = RockStates.HitPlayer;
        FlyToTarget();
    }

    public void FlyToTarget(bool isHitByPlayer = false)
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }

        // 1. 获取基础位移向量 (目标位置 - 当前位置)
        Vector3 directionVec = target.transform.position - transform.position;
        
        // 2. 修正高度差计算 (directionVec.y 本身就是高度差)
        float heightDiff = directionVec.y;

        // 3. 根据来源选择不同的力度设置
        float currentForce = isHitByPlayer ? forceByPlayer : forceByEnemy;
        float currentUpward = isHitByPlayer ? upwardForceByPlayer : upwardForceByEnemy;

        // 4. 动态高度补偿
        // 如果目标在高处，我们需要额外的向上力来抵消重力，防止撞到台阶边缘
        if (heightDiff > 0)
        {
            currentUpward += heightDiff * 1.2f; 
        }

        // 5. 合成最终方向向量并归一化
        // 我们手动构造一个“斜向上”的初速度方向
        direction = (directionVec.normalized + Vector3.up * (currentUpward / currentForce)).normalized;

        // 6. 物理执行
        // 关键：必须清空速度为 Vector3.zero，Vector3.one 会导致初速度偏移
        rb.velocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero; // 同时清空旋转速度，让受力更纯净

        // 使用 Impulse 瞬间施加力
        rb.AddForce(direction * currentForce, ForceMode.Impulse);
    }
    public void HitByPlayer()
    {
        // 直接判断当初扔我的那个人还在不在
        if (launcher != null)
        {
            target = launcher;
            rockState = RockStates.HitEnemy;
            FlyToTarget(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision other) // when the rock collides with other objects
    {
        switch (rockState)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = false; 
                    
                    direction = (other.gameObject.transform.position - launcher.transform.position).normalized;
                    if (other.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        var otherRb = other.gameObject.GetComponent<Rigidbody>();
                        otherRb.AddForce(direction * forceByEnemy, ForceMode.Impulse);
                    }
                    rockState = RockStates.HitNothing;
                }
                if (other.gameObject.CompareTag("Ground"))
                {
                    rockState = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);

                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
            
        }
    }
}
