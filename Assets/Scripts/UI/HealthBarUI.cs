using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;

    Image healthSlider;
    Transform UIbar;
    Transform cam; //希望血条一直面向camera

    CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;

        // the barPoint is above the head of character using this script
        // barPoint = transform.Find("HealthBarPoint");
    }
    void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace) // 如果场景中有其它以worldspace为渲染模式的UI，会出错
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;

                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }
    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0) Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);

        float sliderPercent = (float)currentHealth / maxHealth;
        // Debug.Log("sliderPercent: " + sliderPercent);
        healthSlider.fillAmount = sliderPercent;
        // Debug.Log("currentHealth: " + currentHealth + " maxHealth: " + maxHealth + " sliderPercent: " + sliderPercent);
    }

    //follow
    void LateUpdate()
    {
        // the health bar should follow the target
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;

            // face the camera
            UIbar.forward = -cam.forward;
        }
    }
}
