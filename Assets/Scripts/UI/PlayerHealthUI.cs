using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;   
public class PlayerHealthUI : MonoBehaviour
{
    TextMeshProUGUI levelTMP;
    Image healthSlider;
    Image expSlider;

    void Awake()
    {
        levelTMP = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    void Update()
    {
        levelTMP.text = "Lv." + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpdateHealthUI();
        UpdateExp();
    }
    void UpdateHealthUI()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
        // Debug.Log("sliderPercent: " + sliderPercent);
    }
    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
        // Debug.Log("sliderPercent: " + sliderPercent);
    }
}
