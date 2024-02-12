using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [Header("# Status")]
    public float currentHealth;
    public float maxHealth;
    public float currentStamina;
    public float maxStamina;
    public int level;
    public float exp;
    public float[] nextExp;

    [Header("# Default Value")]
    public float attack;
    public float powerAttack;

    [Header("# Additional Value")]
    public float increaseAttack; // ���ط� ����
    public float increasePowerAttack; // ������ ���ط� ����
    public float increaseSpeed; // �̵��ӵ� ����
    public float increaseHealth; // ����� ����
    public float increaseStamina; // ���¹̳� ����

    [Header("# UI")]
    public Image healthBar;
    public Image steminaBar;
    public Image expBar;
    public Text healthText;
    public Text levelText;

    public static PlayerStatus instance;

    void Awake()
    {
        if (PlayerStatus.instance == null)
            PlayerStatus.instance = this;
    }

    
    void Update()
    {
        LevelUp();
    }

    void LateUpdate()
    {
        StatusSetting();
        UISetting();
    }

    void StatusSetting()
    {
        if (currentHealth > maxHealth * increaseHealth)
            currentHealth = maxHealth * increaseHealth;

        if (currentStamina > maxStamina * increaseStamina)
            currentStamina = maxStamina * increaseStamina;
    }

    void UISetting()
    {
        healthBar.fillAmount = currentHealth / (maxHealth * increaseHealth);
        steminaBar.fillAmount = currentStamina / (maxStamina * increaseStamina);
        expBar.fillAmount = exp / nextExp[level];

        healthText.text = currentHealth.ToString("F0") + " / " + (maxHealth * increaseHealth).ToString("F0");
        levelText.text = "Lv." + (level + 1).ToString();

        if (currentHealth < 0)
            healthText.text = "0 / " + (maxHealth * increaseHealth).ToString("F0");
        else
            healthText.text = currentHealth.ToString("F0") + " / " + (maxHealth * increaseHealth).ToString("F0");
    }

    void LevelUp()
    {
        if (exp >= nextExp[level])
        {
            exp -= nextExp[level];
            level++;
            GameManager.instance.LevelUpSystem();
        }
    }
}
