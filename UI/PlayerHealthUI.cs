using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;
    private void Awake()
    {
        levelText = transform.GetChild(1).GetComponent<Text>();
        healthSlider= transform.GetChild(2).GetChild(0).GetComponent<Image>();
        expSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }
    private void Update()
    {
        levelText.text = "Level" + GameManager.Instance.playerStats.CharacterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();    
    }

    void UpdateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.currentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount= sliderPercent;
    }
    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.CharacterData.currentExp / GameManager.Instance.playerStats.CharacterData.baseExp;
        expSlider.fillAmount= sliderPercent;
    }
}
