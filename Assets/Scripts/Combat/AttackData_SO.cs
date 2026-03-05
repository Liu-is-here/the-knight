using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack", menuName ="Attack")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; // 基本攻击距离
    public float skillRange;    // 远程距离
    public float coolDown;  //CD冷却时间
    public int minDamage;   //最小伤害
    public int maxDamage;   //最大伤害
    public float criticalChance;    //暴击伤害
    public float criticalMultiplier;    //暴击倍率
    
}
