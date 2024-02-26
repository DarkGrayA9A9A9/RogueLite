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
    public Skill[] showSelectSkill = new Skill[3];

    public int leftMonster;
    public int money;

    [Header("# Random Variable")]
    public int dungeonNum;
    public int monsterNum;

    [Header("# State Check")]
    public float delay;
    public int floor;
    public int selectSkill;
    public int language;
    public bool levelUp;
    public bool enforce;
    public int selectEnforce;

    [Header("# Pause Menu")]
    public bool paused;
    public bool statusMenu;
    public bool optionMenu;
    public int selectMenu;
    public int selectOption;

    [Header("# Objects")]
    public GameObject[] dungeonMaps;
    public GameObject[] monsterPrefabs;
    public GameObject[] bossPrefabs;

    GameObject monster;

    [Header("# LevelUp UI")]
    public GameObject levelUpSystem;
    //public GameObject[] skillSlot;
    public GameObject[] skillIcon;
    public Text[] skillTitle;
    public Text[] skillDesc;
    public GameObject[] skillCursor;

    [Header("# In Game UI")]
    public GameObject monsterIcon;
    public Text showFloor;
    public Text showLeftMonster;
    public Text showMoney;
    public GameObject clearText;

    [Header("# Pause Menu UI")]
    public GameObject pauseMenu;
    public GameObject[] menuCursor;
    public Text[] pauseTexts;

    [Header("# Status Menu UI")]
    public GameObject statusMenuCanvas;
    public Text attackValue;
    public Text powerAttackValue;
    public Text healthValue;
    public Text staminaValue;
    public Text speedValue;
    public Text[] statusTexts;

    [Header("# Option Menu UI")]
    public GameObject optionMenuCanvas;
    public Image BGMSlider;
    public Image SESlider;
    public GameObject optionCursor;
    public Text[] optionTexts;

    [Header("# Enforce System")]
    public GameObject enforceSystemCanvas;
    public Text[] enforceTexts;
    public Text enforceAttack;
    public Text enforcePowerAttack;
    public Text enforceHealth;
    public Text enforceStamina;
    public Text enforceSpeed;
    public GameObject enforceCursor;
    public Text priceAttack;
    public Text pricePowerAttack;
    public Text priceHealth;
    public Text priceStamina;
    public Text priceSpeed;

    public int attackLV;
    public int powerAttackLV;
    public int healthLV;
    public int staminaLV;
    public int speedLV;
    public int[] enforcePrice;

    AudioSource audio;

    [Header("# Sounds")]
    public AudioClip townMusic;
    public AudioClip dungeonMusic;

    public float BGMValue;
    public float SEValue;

    public static GameManager instance;

    void Awake()
    {
        audio = GetComponent<AudioSource>();

        Cursor.visible = false;

        if (GameManager.instance == null)
            GameManager.instance = this;
    }

    void Start()
    {
        InitKorea();
        OptionLoad();
        Load();
    }

    void Update()
    {
        PauseMenuSelect();
        StatusMenu();
        OptionMenu();
        LevelUpSelect();
        EnforceSystem();

        if (Input.GetKeyDown(KeyCode.Escape) && delay > 0.1f)
            PauseMenu();
    }

    void LateUpdate()
    {
        ShowInfo();
        LanguageChange();

        if (PlayerController.instance.playerPosition.x < 125f && audio.clip != townMusic)
        {
            audio.clip = townMusic;
            audio.Play();
        }   
        else if (PlayerController.instance.playerPosition.x > 125f && audio.clip != dungeonMusic)
        {
            audio.clip = dungeonMusic;
            audio.Play();
        }

        if (audio.clip == townMusic)
            audio.volume = BGMValue;
        else if (audio.clip == dungeonMusic)
            audio.volume = BGMValue * 0.6f;

        delay += Time.unscaledDeltaTime;
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Attack", PlayerStatus.instance.enforceAttack);
        PlayerPrefs.SetFloat("PowerAttack", PlayerStatus.instance.enforcePowerAttack);
        PlayerPrefs.SetFloat("Health", PlayerStatus.instance.enforceHealth);
        PlayerPrefs.SetFloat("Stamina", PlayerStatus.instance.enforceStamina);
        PlayerPrefs.SetFloat("Speed", PlayerStatus.instance.enforceSpeed);
        PlayerPrefs.SetInt("AttackLV", attackLV);
        PlayerPrefs.SetInt("PowerAttackLV", powerAttackLV);
        PlayerPrefs.SetInt("HealthLV", healthLV);
        PlayerPrefs.SetInt("StaminaLV", staminaLV);
        PlayerPrefs.SetInt("SpeedLV", speedLV);
        PlayerPrefs.SetInt("Money", money);
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey("Attack"))
            return;

        PlayerStatus.instance.enforceAttack = PlayerPrefs.GetFloat("Attack");
        PlayerStatus.instance.enforcePowerAttack = PlayerPrefs.GetFloat("PowerAttack");
        PlayerStatus.instance.enforceHealth = PlayerPrefs.GetFloat("Health");
        PlayerStatus.instance.enforceStamina = PlayerPrefs.GetFloat("Stamina");
        PlayerStatus.instance.enforceSpeed = PlayerPrefs.GetFloat("Speed");
        attackLV = PlayerPrefs.GetInt("AttackLV");
        powerAttackLV = PlayerPrefs.GetInt("PowerAttackLV");
        healthLV = PlayerPrefs.GetInt("HealthLV");
        staminaLV = PlayerPrefs.GetInt("StaminaLV");
        speedLV = PlayerPrefs.GetInt("SpeedLV");
        money = PlayerPrefs.GetInt("Money");
    }

    void OptionSave()
    {
        PlayerPrefs.SetFloat("BGMValue", BGMSlider.fillAmount);
        PlayerPrefs.SetFloat("SEValue", SESlider.fillAmount);
        PlayerPrefs.SetInt("Language", language);
    }

    void OptionLoad()
    {
        if (!PlayerPrefs.HasKey("Language"))
            return;

        BGMSlider.fillAmount = PlayerPrefs.GetFloat("BGMValue");
        SESlider.fillAmount = PlayerPrefs.GetFloat("SEValue");
        BGMValue = PlayerPrefs.GetFloat("BGMValue");
        SEValue = PlayerPrefs.GetFloat("SEValue");
        language = PlayerPrefs.GetInt("Language");
    }

    public void EnforceSystemOn()
    {
        if (paused)
            return;

        if (!enforce)
        {
            selectEnforce = 0;
            enforce = true;
            enforceSystemCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void EnforceSystem()
    {
        if (!enforce)
            return;

        enforceAttack.text = attackLV.ToString();
        enforcePowerAttack.text = powerAttackLV.ToString();
        enforceHealth.text = healthLV.ToString();
        enforceStamina.text = staminaLV.ToString();
        enforceSpeed.text = speedLV.ToString();

        ShowPrice();

        if (Input.GetButtonDown("Up") && selectEnforce > 0)
        {
            SoundEffect.instance.Menu();
            selectEnforce--;
        }

        if (Input.GetButtonDown("Down") && selectEnforce < 4)
        {
            SoundEffect.instance.Menu();
            selectEnforce++;
        }

        switch (selectEnforce)
        {
            case 0:
                enforceCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-610, 120);
                break;
            case 1:
                enforceCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-610, 40);
                break;
            case 2:
                enforceCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-610, -40);
                break;
            case 3:
                enforceCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-610, -120);
                break;
            case 4:
                enforceCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-610, -200);
                break;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return) && delay > 0.1f)
        {
            delay = 0;

            switch (selectEnforce)
            {
                case 0:
                    if (enforcePrice[attackLV] <= money && attackLV < enforcePrice.Length)
                    {
                        SoundEffect.instance.Enforce();
                        PlayerStatus.instance.enforceAttack += 0.2f;
                        money -= enforcePrice[attackLV];
                        attackLV++;
                        Save();
                    }   
                    break;
                case 1:
                    if (enforcePrice[powerAttackLV] <= money && powerAttackLV < enforcePrice.Length)
                    {
                        SoundEffect.instance.Enforce();
                        PlayerStatus.instance.enforcePowerAttack += 0.2f;
                        money -= enforcePrice[powerAttackLV];
                        powerAttackLV++;
                        Save();
                    }
                    break;
                case 2:
                    if (enforcePrice[healthLV] <= money && healthLV < enforcePrice.Length)
                    {
                        SoundEffect.instance.Enforce();
                        PlayerStatus.instance.enforceHealth += 0.2f;
                        money -= enforcePrice[healthLV];
                        healthLV++;
                        Save();
                    }
                    break;
                case 3:
                    if (enforcePrice[staminaLV] <= money && staminaLV < enforcePrice.Length)
                    {
                        SoundEffect.instance.Enforce();
                        PlayerStatus.instance.enforceStamina += 0.2f;
                        money -= enforcePrice[staminaLV];
                        staminaLV++;
                        Save();
                    }
                    break;
                case 4:
                    if (enforcePrice[speedLV] <= money && speedLV < enforcePrice.Length)
                    {
                        SoundEffect.instance.Enforce();
                        PlayerStatus.instance.enforceSpeed += 0.2f;
                        money -= enforcePrice[speedLV];
                        speedLV++;
                        Save();
                    } 
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            delay = 0;
            enforce = false;
            enforceSystemCanvas.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void ShowPrice()
    {
        if (language == 0)
        {
            priceAttack.text = enforcePrice[attackLV].ToString() + " 원";
            pricePowerAttack.text = enforcePrice[powerAttackLV].ToString() + " 원";
            priceHealth.text = enforcePrice[healthLV].ToString() + " 원";
            priceStamina.text = enforcePrice[staminaLV].ToString() + " 원";
            priceSpeed.text = enforcePrice[speedLV].ToString() + " 원";
        }
        else
        {
            priceAttack.text = enforcePrice[attackLV].ToString() + " G";
            pricePowerAttack.text = enforcePrice[powerAttackLV].ToString() + " G";
            priceHealth.text = enforcePrice[healthLV].ToString() + " G";
            priceStamina.text = enforcePrice[staminaLV].ToString() + " G";
            priceSpeed.text = enforcePrice[speedLV].ToString() + " G";
        }
    }

    void PauseMenu()
    {
        if (enforce)
            return;

        if (!paused)
        {
            SoundEffect.instance.Menu();
            selectMenu = 0;
            paused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            if (statusMenu)
            {
                SoundEffect.instance.Menu();
                statusMenu = false;
            } 
            else if (optionMenu)
            {
                SoundEffect.instance.Menu();
                optionMenu = false;
            } 
            else
            {
                SoundEffect.instance.Menu();
                paused = false;
                statusMenu = false;
                optionMenu = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    void PauseMenuSelect()
    {
        if (!paused || statusMenu || optionMenu)
            return;

        if (Input.GetButtonDown("Up") && selectMenu > 0)
        {
            SoundEffect.instance.Menu();
            selectMenu--;
        }
            

        if (Input.GetButtonDown("Down") && selectMenu < 3)
        {
            SoundEffect.instance.Menu();
            selectMenu++;
        }
        

        switch (selectMenu)
        {
            case 0:
                menuCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-185, 100);
                menuCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(185, 100);
                break;
            case 1:
                menuCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-185, 0);
                menuCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(185, 0);
                break;
            case 2:
                menuCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-185, -100);
                menuCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(185, -100);
                break;
            case 3:
                menuCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-185, -200);
                menuCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(185, -200);
                break;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return))
        {
            delay = 0;
            SoundEffect.instance.Menu();

            switch (selectMenu)
            {
                case 0:
                    paused = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1;
                    break;
                case 1:
                    delay = 0;
                    statusMenu = true;
                    statusMenuCanvas.SetActive(true);
                    attackValue.text = ((PlayerStatus.instance.increaseAttack + PlayerStatus.instance.enforceAttack) * 100).ToString("F0") + "%";
                    powerAttackValue.text = ((PlayerStatus.instance.increasePowerAttack + PlayerStatus.instance.enforcePowerAttack) * 100).ToString("F0") + "%";
                    healthValue.text = ((PlayerStatus.instance.increaseHealth + PlayerStatus.instance.enforceHealth) * 100).ToString("F0") + "%";
                    staminaValue.text = ((PlayerStatus.instance.increaseStamina + PlayerStatus.instance.enforceStamina) * 100).ToString("F0") + "%";
                    speedValue.text = ((PlayerStatus.instance.increaseSpeed + +PlayerStatus.instance.enforceSpeed) * 100).ToString("F0") + "%";
                    break;
                case 2:
                    delay = 0;
                    optionMenu = true;
                    optionMenuCanvas.SetActive(true);
                    break;
                case 3:
                    Application.Quit();
                    break;
            }
        }
    }

    void StatusMenu()
    {
        if (!statusMenu || delay < 0.1f)
            return;

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            delay = 0;
            SoundEffect.instance.Menu();
            statusMenu = false;
            statusMenuCanvas.SetActive(false);
        }
    }

    void OptionMenu()
    {
        if (!optionMenu || delay < 0.1f)
            return;

        if (Input.GetButtonDown("Up") && selectOption > 0)
        {
            SoundEffect.instance.Menu();
            selectOption--;
        }

        if (Input.GetButtonDown("Down") && selectOption < 2)
        {
            SoundEffect.instance.Menu();
            selectOption++;
        }

        switch (selectOption)
        {
            case 0:
                optionCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195, 130);

                if (Input.GetKeyDown(KeyCode.A) && BGMValue > 0.01f)
                {
                    SoundEffect.instance.Menu();
                    BGMValue -= 0.1f;
                }

                if (Input.GetKeyDown(KeyCode.D) && BGMValue < 0.99f)
                {
                    SoundEffect.instance.Menu();
                    BGMValue += 0.1f;
                }
                break;
            case 1:
                optionCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195, 60);

                if (Input.GetKeyDown(KeyCode.A) && SEValue > 0.01f)
                {
                    SoundEffect.instance.Menu();
                    SEValue -= 0.1f;
                }

                if (Input.GetKeyDown(KeyCode.D) && SEValue < 0.99f)
                {
                    SoundEffect.instance.Menu();
                    SEValue += 0.1f;
                }
                break;
            case 2:
                optionCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195, -10);

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    SoundEffect.instance.Menu();

                    if (language == 0)
                        language = 1;
                    else
                        language = 0;
                }
                
                break;
        }

        BGMSlider.fillAmount = BGMValue;
        SESlider.fillAmount = SEValue;

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            delay = 0;
            OptionSave();
            SoundEffect.instance.Menu();
            optionMenu = false;
            optionMenuCanvas.SetActive(false);
            selectOption = 0;
            optionCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195, 130);
        }

        SoundEffect.instance.SoundEffectSetting();
    }


    void InitKorea()
    {
        pauseTexts[0].text = "일시정지";
        pauseTexts[1].text = "계속하기";
        pauseTexts[2].text = "스테이터스";
        pauseTexts[3].text = "환경설정";
        pauseTexts[4].text = "게임종료";

        statusTexts[0].text = "스테이터스";
        statusTexts[1].text = "기본공격";
        statusTexts[2].text = "강공격";
        statusTexts[3].text = "생명력";
        statusTexts[4].text = "스태미나";
        statusTexts[5].text = "이동속도";

        optionTexts[0].text = "환경설정";
        optionTexts[1].text = "배경음";
        optionTexts[2].text = "효과음";
        optionTexts[3].text = "한국어";

        enforceTexts[0].text = "업그레이드";
        enforceTexts[1].text = "기본공격";
        enforceTexts[2].text = "강공격";
        enforceTexts[3].text = "생명력";
        enforceTexts[4].text = "스태미나";
        enforceTexts[5].text = "이동속도";

        // Levelup System
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

    void InitEnglish()
    {
        pauseTexts[0].text = "Paused";
        pauseTexts[1].text = "Resume";
        pauseTexts[2].text = "Status";
        pauseTexts[3].text = "Setting";
        pauseTexts[4].text = "Exit";

        statusTexts[0].text = "Status";
        statusTexts[1].text = "Common Attack";
        statusTexts[2].text = "Power Attack";
        statusTexts[3].text = "Health";
        statusTexts[4].text = "Stamina";
        statusTexts[5].text = "Speed";

        optionTexts[0].text = "Setting";
        optionTexts[1].text = "BGM";
        optionTexts[2].text = "SE";
        optionTexts[3].text = "English";

        enforceTexts[0].text = "Upgrade";
        enforceTexts[1].text = "Common Attack";
        enforceTexts[2].text = "Power Attack";
        enforceTexts[3].text = "Health";
        enforceTexts[4].text = "Stamina";
        enforceTexts[5].text = "Speed";

        // Levelup System
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
                InitKorea();
                break;
            case 1:
                InitEnglish();
                break;
        }
    }

    void ShowInfo()
    {
        if (language == 0)
            showMoney.text = money.ToString() + " 원";
        else
            showMoney.text = money.ToString() + " G";

        if (floor > 0)
        {
            showLeftMonster.text = leftMonster.ToString();

            if (language == 0)
                showFloor.text = "던전 " + floor.ToString() + "층";
            else
                showFloor.text = "Floor " + floor.ToString();

            if (!monsterIcon.activeSelf)
                monsterIcon.SetActive(true);
        }
        else
        {
            showLeftMonster.text = "";
            showFloor.text = "";

            if (monsterIcon.activeSelf)
                monsterIcon.SetActive(false);
        }
    }

    public void DungeonSetting()
    {
        floor++;

        if (floor % 5 == 0)
        {
            dungeonNum = 4;
            monsterNum = 1;
            leftMonster = 1;
        } 
        else
        {
            dungeonNum = Random.Range(0, 5);
            monsterNum = Random.Range(6, 16);
            leftMonster = monsterNum;
        }
            

        for (int index = 0; index < 5; index++)
        {
            if (index == dungeonNum)
                dungeonMaps[index].SetActive(true);
            else
                dungeonMaps[index].SetActive(false);
        }

        for (int index = 0; index < monsterNum; index++)
        {
            float x;
            float y;

            if (floor == 5)
            {
                x = 250;
                y = 15;
            }
            else
            {
                x = Random.Range(200f, 300f);
                y = Random.Range(0f, 50f);
            }

            if (floor <= 5)
            {
                if (floor == 5)
                    monster = Instantiate(bossPrefabs[0]);
                else
                    monster = Instantiate(monsterPrefabs[0]);
            }
            else if (floor <= 10)
            {
                if (floor == 10)
                    monster = Instantiate(bossPrefabs[1]);
                else
                    monster = Instantiate(monsterPrefabs[1]);
            }
            else
            {
                if (floor == 15)
                    monster = Instantiate(bossPrefabs[2]);
                else
                    monster = Instantiate(monsterPrefabs[2]);
            }

            monster.transform.position = new Vector3(x, y, 0);
            monster.SetActive(true);
        }
    }

    public void LevelUpSystem()
    {
        levelUp = true;
        selectSkill = 0;
        Time.timeScale = 0;
        levelUpSystem.SetActive(true);

        for (int index = 0; index < 21; index++)
        {
            skillIcon[index].SetActive(false);
        }

        for (int index = 0; index < 3; index++)
        {
            int random = Random.Range(0, 7);
            showSelectSkill[index] = skill[random];

            skillIcon[showSelectSkill[index].id + (index * 7)].SetActive(true);
            skillTitle[index].text = showSelectSkill[index].name;
            skillDesc[index].text = showSelectSkill[index].desc;
        }
    }

    void LevelUpSelect()
    {
        if (!levelUp)
            return;

        if (Input.GetButtonDown("Up") && selectSkill > 0)
        {
            SoundEffect.instance.Menu();
            selectSkill--;
        }

        if (Input.GetButtonDown("Down") && selectSkill < 2)
        {
            SoundEffect.instance.Menu();
            selectSkill++;
        }

        switch (selectSkill)
        {
            case 0:
                skillCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, 150);
                skillCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, 150);
                break;
            case 1:
                skillCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, 0);
                skillCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, 0);
                break;
            case 2:
                skillCursor[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-340, -150);
                skillCursor[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(340, -150);
                break;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Return))
        {
            delay = 0;
            SoundEffect.instance.Menu();

            switch (showSelectSkill[selectSkill].id)
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

    public void GameClear()
    {
        Save();
        SoundEffect.instance.Clear();
        clearText.SetActive(true);
        Invoke("ClearExit", 5f);
    }

    void ClearExit()
    {
        Application.Quit();
    }
}
