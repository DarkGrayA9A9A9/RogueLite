using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public struct Skill
    {
        public int id;
        public string name;
        public string desc;
    }

    public Skill[] skill = new Skill[7];
    public Skill[] chooseSkill = new Skill[3];

    public int leftMonster;

    [Header("# Random Variable")]
    public int dungeonNum;
    public int monsterNum;

    [Header("# State Check")]
    public bool levelUp;
    public int choose;

    [Header("# Object Array")]
    public GameObject[] dungeonMaps;
    public GameObject[] monsterPrefabs;

    GameObject monster;

    public GameObject levelUpSystem;
    public GameObject[] slot;
    public GameObject[] icon;
    public Text[] title;
    public Text[] desc;
    public GameObject[] cursor;

    public static GameManager instance;

    void Awake()
    {
        Cursor.visible = false;

        InitSkill();

        if (GameManager.instance == null)
            GameManager.instance = this;
    }

    void Update()
    {
        LevelUpChoose();
    }
    
    void InitSkill()
    {
        for (int index = 0; index < 7; index++)
        {
            skill[index].id = index;

            switch (index)
            {
                case 0:
                    skill[index].name = "��ö ��";
                    skill[index].desc = "�Ϲ� ���� ���ط� ����";
                    break;
                case 1:
                    skill[index].name = "���ɷ�";
                    skill[index].desc = "������ ���ط� ����";
                    break;
                case 2:
                    skill[index].name = "���� ����";
                    skill[index].desc = "�̵��ӵ� ����";
                    break;
                case 3:
                    skill[index].name = "������� ��ȣ";
                    skill[index].desc = "����� ����";
                    break;
                case 4:
                    skill[index].name = "������";
                    skill[index].desc = "���¹̳� ����";
                    break;
                case 5:
                    skill[index].name = "�ҹ���";
                    skill[index].desc = "����� 20% ȸ��\"";
                    break;
                case 6:
                    skill[index].name = "�빰��";
                    skill[index].desc = "����� 50% ȸ��";
                    break;
            }
        }
    }

    void LateUpdate()
    {
        
    }

    public void DungeonSetting()
    {
        dungeonNum = Random.Range(0, 5);
        monsterNum = Random.Range(6, 16);
        leftMonster = monsterNum;

        for (int index = 0; index < 5; index++)
        {
            if (index == dungeonNum)
                dungeonMaps[index].SetActive(true);
            else
                dungeonMaps[index].SetActive(false);
        }

        for (int index = 0; index < monsterNum; index++)
        {
            int random = Random.Range(0, 2);
            float x = Random.Range(200f, 300f);
            float y = Random.Range(0f, 50f);
            monster = Instantiate(monsterPrefabs[random]);
            monster.transform.position = new Vector3(x, y, 0);
            monster.SetActive(true);
        }
    }

    public void LevelUpSystem()
    {
        levelUp = true;
        choose = 0;
        Time.timeScale = 0;
        levelUpSystem.SetActive(true);

        for (int index = 0; index < 21; index++)
        {
            icon[index].SetActive(false);
        }

        for (int index = 0; index < 3; index++)
        {
            int random = Random.Range(0, 7);
            chooseSkill[index] = skill[random];

            icon[chooseSkill[index].id + (index * 7)].SetActive(true);
            title[index].text = chooseSkill[index].name;
            desc[index].text = chooseSkill[index].desc;
        }
    }

    void LevelUpChoose()
    {
        if (!levelUp)
            return;

        if (Input.GetKeyDown(KeyCode.W) && choose > 0)
            choose--;

        if (Input.GetKeyDown(KeyCode.S) && choose < 2)
            choose++;

        switch (choose)
        {
            case 0:
                cursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, 150);
                cursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, 150);
                break;
            case 1:
                cursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, 0);
                cursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, 0);
                break;
            case 2:
                cursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, -150);
                cursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, -150);
                break;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return))
        {
            switch (chooseSkill[choose].id)
            {
                case 0:
                    PlayerStatus.instance.increaseAttack += 0.1f;
                    break;
                case 1:
                    PlayerStatus.instance.increasePowerAttack += 0.1f;
                    break;
                case 2:
                    PlayerStatus.instance.increaseSpeed += 0.1f;
                    break;
                case 3:
                    PlayerStatus.instance.increaseHealth += 0.1f;
                    break;
                case 4:
                    PlayerStatus.instance.increaseStamina += 0.1f;
                    break;
                case 5:
                    PlayerStatus.instance.currentHealth += PlayerStatus.instance.maxHealth * PlayerStatus.instance.increaseHealth * 0.2f;
                    break;
                case 6:
                    PlayerStatus.instance.currentHealth += PlayerStatus.instance.maxHealth * PlayerStatus.instance.increaseHealth * 0.5f;
                    break;
            }
            
            levelUp = false;
            levelUpSystem.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
