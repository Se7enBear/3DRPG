using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPretab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visiableTime;
    Image healthSlider;
    Transform UIbar;
    Transform cam;
    private float timeLeft;
    CharacterStats currentStats;
    private void Awake()
    {
        currentStats =GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += updateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode==RenderMode.WorldSpace) {
               UIbar= Instantiate(healthUIPretab, canvas.transform).transform;
                healthSlider=UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }
    private void updateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLeft = visiableTime;
        float sliderPercent =(float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

    }
    void LateUpdate()
    {
          if(UIbar!=null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            if(timeLeft<=0&&!alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
                
            }
            else timeLeft-=Time.deltaTime;

        }
    }
}