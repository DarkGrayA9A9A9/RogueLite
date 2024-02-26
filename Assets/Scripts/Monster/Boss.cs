using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public enum BossType { Goblin, Mushroom, Skeleton }
    public BossType bossType;

    [Header("# Status")]
    public float currentHealth;
    public float maxHealth;
    public float moveSpeed;
    public float moveValue;

    [Header("# Check")]
    public float timer;
    public float patterTime;
    public bool pattern;
    public bool die;
    public int bulletPatternCnt;
    public int bulletPatternMaxCnt;
    public int attackPatternCnt;
    public int attackPatternMaxCnt;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    GameObject bullet;

    [Header("# Prefabs")]
    public GameObject[] bulletPrefabs;

    [Header("# UI")]
    public Image healthBar;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (die)
            return;

        if (!pattern && timer > patterTime)
        {
            pattern = true;
            timer = 0;
            Pattern();
        }

        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
            Dead();

        if (die)
            StopAllCoroutines();
    }

    void LateUpdate()
    {
        if (die)
            return;

        anim.SetFloat("Move", Mathf.Abs(moveValue));

        LookPlayer();

        if (!pattern && !die)
            timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (die)
            return;

        ApproachPlayer();
    }

    void Dead()
    {
        if (!die)
        {
            switch (bossType)
            {
                case BossType.Goblin:
                    SoundEffect.instance.MonsterDie(0);
                    break;
                case BossType.Mushroom:
                    SoundEffect.instance.MonsterDie(1);
                    break;
                case BossType.Skeleton:
                    SoundEffect.instance.MonsterDie(2);
                    Time.timeScale = 0;
                    Invoke("GameClear", 3f);
                    break;
            }

            anim.ResetTrigger("Hit");
            die = true;
            gameObject.layer = 8;
            anim.SetTrigger("Die");
            GameManager.instance.leftMonster--;

            Invoke("Destroy", 10f);
        }
    }

    void GameClear()
    {
        GameManager.instance.GameClear();
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void LookPlayer()
    {
        if (pattern || die)
        {
            moveValue = 0;
            return;
        }

        if (rigid.transform.position.x > PlayerController.instance.playerPosition.x)
        {
            spriter.flipX = true;
            moveValue = -1;
        }
        else
        {
            spriter.flipX = false;
            moveValue = 1;
        }
    }

    void ApproachPlayer()
    {
        if (pattern || die)
            return;

        rigid.velocity = new Vector2(moveValue * moveSpeed, rigid.velocity.y);
    }

    void Pattern()
    {
        if (die)
            return;

        int random;

        switch (bossType)
        {
            case BossType.Goblin:
                random = Random.Range(0, 2);

                switch (random)
                {
                    case 0:
                        anim.SetTrigger("Attack");
                        break;
                    case 1:
                        int bulletRandom = Random.Range(2, 5);
                        bulletPatternMaxCnt = bulletRandom;
                        bulletPatternCnt = 0;
                        Pattern1Goblin();
                        break;
                }
                break;
            case BossType.Mushroom:
                random = Random.Range(0, 2);

                switch (random)
                {
                    case 0:
                        StartCoroutine(Pattern0Mushroom());
                        break;
                    case 1:
                        int bulletRandom = Random.Range(3, 7);
                        bulletPatternMaxCnt = bulletRandom;
                        bulletPatternCnt = 0;
                        Pattern1Mushroom();
                        break;
                }
                break;
            case BossType.Skeleton:
                random = Random.Range(0, 2);

                switch (random)
                {
                    case 0:
                        StartCoroutine(Pattern0Skeleton());
                        break;
                    case 1:
                        int bulletRandom = Random.Range(5, 10);
                        bulletPatternMaxCnt = bulletRandom;
                        bulletPatternCnt = 0;
                        Pattern1Mushroom();
                        break;
                }
                break;
        }
    }

    void Pattern0Goblin()
    {
        if (die)
            return;

        GameObject atk = transform.GetChild(0).gameObject;
        atk.GetComponent<CapsuleCollider2D>().enabled = true;

        if (spriter.flipX)
            rigid.AddForce(Vector2.left * 50, ForceMode2D.Impulse);
        else
            rigid.AddForce(Vector2.right * 50, ForceMode2D.Impulse);

        Invoke("Pattern0GoblinExit", 2f);
    }

    void Pattern0GoblinExit()
    {
        GameObject atk = transform.GetChild(0).gameObject;
        atk.GetComponent<CapsuleCollider2D>().enabled = false;
        pattern = false;
        timer = 0;
    }

    void Pattern1Goblin()
    {
        if (die)
            return;

        int bulletNum;
        int random;

        if (bulletPatternCnt % 2 == 0)
            random = Random.Range(5, 10);
        else
            random = Random.Range(10, 15);

        bulletNum = random;

        for (int index = 0; index < bulletNum; index++)
        {
            bullet = Instantiate(bulletPrefabs[0]);
            bullet.transform.position = rigid.transform.position;

            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / bulletNum), Mathf.Sin(Mathf.PI * 2 * index / bulletNum));

            bulletRigid.AddForce(dirVec.normalized * 7.5f, ForceMode2D.Impulse);
        }

        if (bulletPatternCnt < bulletPatternMaxCnt)
        {
            Invoke("Pattern1Goblin", 1f);
            bulletPatternCnt++;
        }
        else
            Invoke("Pattern1GoblinExit", 2f);
    }

    void Pattern1GoblinExit()
    {
        pattern = false;
        timer = 0;
    }

    IEnumerator Pattern0Mushroom()
    {
        attackPatternCnt = 0;
        int random = Random.Range(2, 4);
        attackPatternMaxCnt = random;

        yield return new WaitForSeconds(1f);

        while (attackPatternCnt < attackPatternMaxCnt)
        {
            attackPatternCnt++;
            anim.SetTrigger("Attack");

            yield return new WaitForSeconds(0.5f);

            GameObject atk = transform.GetChild(0).gameObject;
            atk.GetComponent<CapsuleCollider2D>().enabled = true;

            yield return new WaitForSeconds(0.2f);

            atk.GetComponent<CapsuleCollider2D>().enabled = false;

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        pattern = false;

        yield return null;
    }

    void Pattern1Mushroom()
    {
        if (die)
            return;

        int bulletNum;
        int random;

        if (bulletPatternCnt % 2 == 0)
            random = Random.Range(10, 20);
        else
            random = Random.Range(20, 30);

        bulletNum = random;

        for (int index = 0; index < bulletNum; index++)
        {
            bullet = Instantiate(bulletPrefabs[0]);
            bullet.transform.position = rigid.transform.position;

            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / bulletNum), Mathf.Sin(Mathf.PI * 2 * index / bulletNum));

            bulletRigid.AddForce(dirVec.normalized * 7.5f, ForceMode2D.Impulse);
        }

        if (bulletPatternCnt < bulletPatternMaxCnt)
        {
            Invoke("Pattern1Mushroom", 1f);
            bulletPatternCnt++;
        }
        else
            Invoke("Pattern1MushroomExit", 1f);
    }

    void Pattern1MushroomExit()
    {
        pattern = false;
        timer = 0;
    }

    IEnumerator Pattern0Skeleton()
    {
        attackPatternCnt = 0;
        int random = Random.Range(3, 5);
        attackPatternMaxCnt = random;

        yield return new WaitForSeconds(1f);

        while (attackPatternCnt < attackPatternMaxCnt)
        {
            yield return new WaitForSeconds(1f);

            attackPatternCnt++;
            anim.SetTrigger("Attack");

            yield return new WaitForSeconds(0.5f);

            GameObject atk = transform.GetChild(0).gameObject;
            atk.GetComponent<CapsuleCollider2D>().enabled = true;

            yield return new WaitForSeconds(0.2f);

            atk.GetComponent<CapsuleCollider2D>().enabled = false;

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        pattern = false;

        yield return null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (pattern)
            return;

        if (collision.gameObject.CompareTag("Attack"))
        {
            SoundEffect.instance.MonsterHit();
            anim.SetTrigger("Hit");
            rigid.velocity = Vector2.zero;

            float random = Random.Range(0.5f, 1.5f);
            currentHealth -= random * PlayerStatus.instance.attack * (PlayerStatus.instance.increaseAttack + PlayerStatus.instance.enforceAttack);
        }

        if (collision.gameObject.CompareTag("PowerAttack"))
        {
            SoundEffect.instance.MonsterHit();
            anim.SetTrigger("Hit");
            rigid.velocity = Vector2.zero;

            float random = Random.Range(0.5f, 1.5f);
            currentHealth -= random * PlayerStatus.instance.powerAttack * (PlayerStatus.instance.increasePowerAttack + PlayerStatus.instance.enforcePowerAttack);
        }
    }
}
