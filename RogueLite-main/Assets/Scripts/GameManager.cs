using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int floor;
    public bool levelUp;
    public int choose;
    public int language;

    [Header("# Objects")]
    public GameObject[] dungeonMaps;
    public GameObject[] monsterPrefabs;

    GameObject monster;

    public GameObject levelUpSystem;
    public GameObject[] slot;
    public GameObject[] icon;
    public Text[] title;
    public Text[] desc;
    public GameObject[] cursor;

    AudioSource audio;

    [Header("# Sounds")]
    public AudioClip townMusic;
    public AudioClip dungeonMusic;

    public static GameManager instance;

    void Awake()
    {
        audio = GetComponent<AudioSource>();

        Cursor.visible = false;

        InitSkillKorea();

        if (GameManager.instance == null)
            GameManager.instance = this;
    }

    void Update()
    {
        LevelUpChoose();
    }

    void LateUpdate()
    {
        LanguageChange();

        if (PlayerController.instance.playerPosition.x < 125f && audio.clip != townMusic)
        {
            audio.clip = townMusic;
            audio.volume = 0.4f;
            audio.Play();
        }   
        else if (PlayerController.instance.playerPosition.x > 125f && audio.clip != dungeonMusic)
        {
            audio.clip = dungeonMusic;
            audio.volume = 0.25f;
            audio.Play();
        }
    }

    void InitSkillKorea()
    {
        for (int index = 0; index < 7; index++)
        {
            skill[index].id = index;

            switch (index)
            {
                case 0:
                    skill[index].name = "강철 검";
                    skill[index].desc = "일반 공격 피해량 20% 증가";
                    break;
                case 1:
                    skill[index].name = "원심력";
                    skill[index].desc = "강공격 피해량 20% 증가";
                    break;
                case 2:
                    skill[index].name = "윈드 부츠";
                    skill[index].desc = "이동속도 20% 증가";
                    break;
                case 3:
                    skill[index].name = "신의 은총";
                    skill[index].desc = "최대 생명력 20% 증가";
                    break;
                case 4:
                    skill[index].name = "지구력";
                    skill[index].desc = "최대 스태미나 20% 증가";
                    break;
                case 5:
                    skill[index].name = "소물약";
                    skill[index].desc = "생명력 20% 회복";
                    break;
                case 6:
                    skill[index].name = "대물약";
                    skill[index].desc = "생명력 50% 회복";
                    break;
            }
        }
    }

    void InitSkillEnglish()
    {
        for (int index = 0; index < 7; index++)
        {
            skill[index].id = index;

            switch (index)
            {
                case 0:
                    skill[index].name = "Steel Sword";
                    skill[index].desc = "Increase 20% your common attack damage";
                    break;
                case 1:
                    skill[index].name = "Centrifugal Force";
                    skill[index].desc = "Increase 20% your power attack damage";
                    break;
                case 2:
                    skill[index].name = "Wind Boots";
                    skill[index].desc = "Increase 20% your move speed";
                    break;
                case 3:
                    skill[index].name = "God Bless You";
                    skill[index].desc = "Increase 20% your max health";
                    break;
                case 4:
                    skill[index].name = "Endurance";
                    skill[index].desc = "Increase 20% your max stamina";
                    break;
                case 5:
                    skill[index].name = "Potion";
                    skill[index].desc = "Recovery 20% your health";
                    break;
                case 6:
                    skill[index].name = "High Potion";
                    skill[index].desc = "Recovery 50% your health";
                    break;
            }
        }
    }

    void LanguageChange()
    {
        switch (language)
        {
            case 0:
                InitSkillKorea();
                break;
            case 1:
                InitSkillEnglish();
                break;
        }
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
                    PlayerStatus.instance.increaseAttack += 0.2f;
                    break;
                case 1:
                    PlayerStatus.instance.increasePowerAttack += 0.2f;
                    break;
                case 2:
                    PlayerStatus.instance.increaseSpeed += 0.2f;
                    break;
                case 3:
                    PlayerStatus.instance.increaseHealth += 0.2f;
                    break;
                case 4:
                    PlayerStatus.instance.increaseStamina += 0.2f;
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
