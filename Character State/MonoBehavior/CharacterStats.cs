using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO CharacterData;
    public AttackData_SO AttackData;
    [HideInInspector]
    public bool isCritical;
    private void Awake()
    {
        if(templateData != null)
        {
            CharacterData=Instantiate(templateData);
        }
    }
    #region read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (CharacterData != null)
                return CharacterData.maxHealth;
            else return 0;
        }
        set
        {
            CharacterData.maxHealth = value;
        }
    }
         public int currentHealth
    {
        get
        {
            if (CharacterData != null)
                return CharacterData.currentHealth;
            else return 0;
        }
        set
        {
            CharacterData.currentHealth = value;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (CharacterData != null)
                return CharacterData.baseDefence;
            else return 0;
        }
        set
        {
            CharacterData.baseDefence = value;
        }
    }

    public int currentDefence
    {
        get
        {
            if (CharacterData != null)
                return CharacterData.currentDefence;
            else return 0;
        }
        set
        {
            CharacterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Battle
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.currentDamage() - defener.currentDefence,0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("hit");
        }
        UpdateHealthBarOnAttack?.Invoke(currentHealth, MaxHealth);
        if (currentHealth <= 0)
        {
            attacker.CharacterData.UpdateExp(CharacterData.killPoint);
        }
    }

    public void TakeDamage(int damage,CharacterStats defener)
    {

        int currentDamge = Mathf.Max(damage - defener.currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - currentDamge, 0);
        UpdateHealthBarOnAttack?.Invoke(currentHealth, MaxHealth);
        GameManager.Instance.playerStats.CharacterData.UpdateExp(CharacterData.killPoint);
    }
    private int currentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(AttackData.minDamge, AttackData.maxDamge);
        if (isCritical)
        {
            coreDamage = coreDamage * AttackData.criticalMultiplier;
            
        }
        return (int)coreDamage;
    }




    #endregion
}





