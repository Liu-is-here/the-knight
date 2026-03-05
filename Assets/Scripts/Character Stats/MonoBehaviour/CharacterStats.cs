using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical; //是否暴击

    #region Read_from_Data_SO
    //Properties
    public int MaxHealth
    {
        get { if(characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }   
    public int CurrentHealth
    {
        get { if(characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }   

    public int BaseDefence
    {
        get { if(characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }   
    public int CurrentDefence
    {
        get { if(characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }   
    #endregion

    #region Read_from_Attack_Data_SO
    public float attackRange
    {
        get { if(attackData != null) return attackData.attackRange; else return 1; } // 默认近程攻击距离为1
        set { attackData.attackRange = value; }
    }
    public float skillRange
    {
        get { if(attackData != null) return attackData.skillRange; else return 5; } // 默认远程攻击距离为5
        set { attackData.attackRange = value; }
    }

    public float coolDown
    {
        get { if(attackData != null) return attackData.coolDown; else return 1; }   // 默认CD冷却时间为1
        set { attackData.coolDown = value; }
    }

    public int minDamage
    {
        get { if (attackData != null) return attackData.minDamage; else return 0; } // 最小伤害
        set { attackData.minDamage = value; }
    }
    public int maxDamage
    {
        get { if(attackData != null) return attackData.maxDamage; else return 0; } // 默认最大伤害为0
        set { attackData.maxDamage = value; }
    }
    public float criticalChance
    {
        get { if(attackData != null) return attackData.criticalChance; else return 0;}  //默认暴击伤害为0
        set { attackData.criticalChance = value; }
    }

    public float criticalMultiplier
    {
        get { if(attackData != null) return attackData.criticalMultiplier; else return 0;}
        set { attackData.criticalMultiplier = value; }
    }

    #endregion

    #region

    void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
    public void TakeDamege(CharacterStats attacker, CharacterStats defender)
    {
        // input:  CharacterStats attacker, CharacterStats defender
        // output: attacker.attackDamage - defender.defence   

        int damage = Math.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        Debug.Log("角色受击！当前生命值：" + CurrentHealth);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }



        // LEVEL UP
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }

        // Update UI: 使用一个Action，当Action发生时激活所有注册它的函数
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        // TODO:DEATH
    }
    public void TakeDamage(int damage, CharacterStats defender)
    {
        int CurrentDamage = Math.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        // Debug.Log("角色受击！当前生命值：" + CurrentHealth);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

    }
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        // 计算暴击率
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }
    #endregion
}
