using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;

    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level Data")]
    public int currentLevel;
    public int maxLevel;

    public int baseExp;
    public int currentExp;
    public float levelBuff;
    public float levelMultiplier
    {
        // 如果currentLevel为1，则返回1
        // 如果currentLevel为2，则返回1 + 1 * levelBuff
        // 如果currentLevel为3，则返回1 + 2 * levelBuff
        // 以此类推
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, 10);
        baseExp += (int)(baseExp * levelMultiplier);
        Debug.Log("Level Up! Current Level: " + currentLevel + " Base Exp: " + baseExp + " Current Exp: " + currentExp);

        maxHealth = (int)(maxHealth * levelMultiplier);
        currentHealth = maxHealth;
        // Debug.Log("Level Up! Current Level: " + currentLevel + " Max Health: " + maxHealth + " Current Health: " + currentHealth);
    }
    
}
