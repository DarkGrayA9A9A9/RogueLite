using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public enum MonsterType { Goblin, Skeleton }
    public MonsterType monsterType;

    public int random;

    [Header("# Status")]
    public float currentHealth;
    public float maxHealth;
    public float exp;
    public float attack;
    public int money;

    [Header("# Movement")]
    public float moveValue;
    public float moveSpeed;
    public float chaseDistance;
    public float attackDistance;
    public float knockBackPower;

    [Header("# Delay Check")]
    public float lastAttack;
    public float attackDelay;

    [Header("# State Check")]
    public bool attacking;
    public bool platformCheck;
    public bool stun;
    public bool die;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    CapsuleCollider2D coll;

    public GameObject bar;
    public Image healthBar;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        switch (monsterType)
        {
            case MonsterType.Goblin:
                random = Random.Range(3, 8);

                maxHealth = 10 + (10 * (float)GameManager.instance.floor / 5);
                attack = 4 + (4 * (float)GameManager.instance.floor / 5);
                money = random;
                break;
            case MonsterType.Skeleton:
                random = Random.Range(7, 16);

                maxHealth = 20 + (20 * (float)GameManager.instance.floor / 5);
                attack = 10 + (10 * (float)GameManager.instance.floor / 5);
                money = random;
                break;
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (rigid.velocity.y < -25f)
            rigid.velocity = new Vector2(rigid.velocity.x, -25f);

        switch (monsterType)
        {
            case MonsterType.Goblin:
                if (spriter.flipX)
                    coll.offset = new Vector2(-0.18f, -0.55f);
                else
                    coll.offset = new Vector2(0.18f, -0.55f);
                break;
            case MonsterType.Skeleton:
                if (spriter.flipX)
                    coll.offset = new Vector2(-0.3f, -0.03f);
                else
                    coll.offset = new Vector2(0.3f, -0.03f);
                break;
        }

        if (Mathf.Abs(rigid.velocity.x) < 0.01f && !attacking && !die)
        {
            if (gameObject.transform.position.x < PlayerController.instance.playerPosition.x)
                spriter.flipX = false;
            else
                spriter.flipX = true;
        }

        InteractionPlayer();
        PlatformCheck();
    }

    void LateUpdate()
    {
        if (currentHealth <= 0 && !die)
            Dead();

        if (!attacking)
        {
            if (moveValue < -0.01)
                spriter.flipX = true;
            else if (moveValue > 0.01)
                spriter.flipX = false;
        }

        anim.SetFloat("Move", Mathf.Abs(moveValue));
        HealthBar();
        Timer();
    }

    void FixedUpdate()
    {
        MoveLogic();
    }

    void MoveLogic()
    {
        if (!platformCheck || attacking || die || stun)
        {
            moveValue = 0;
            return;
        }

        rigid.velocity = new Vector2(moveValue * moveSpeed, rigid.velocity.y);
    }

    void InteractionPlayer()
    {
        if (attacking || die)
            return;

        if (platformCheck && Vector2.Distance(rigid.transform.position, PlayerController.instance.playerPosition) < chaseDistance && Vector2.Distance(rigid.transform.position, PlayerController.instance.playerPosition) > attackDistance)
        {
            if (rigid.transform.position.x > PlayerController.instance.playerPosition.x)
                moveValue = -1;
            else
                moveValue = 1;
        }
        else
        {
            moveValue = 0;

            if (Vector2.Distance(rigid.transform.position, PlayerController.instance.playerPosition) < attackDistance && lastAttack > attackDelay)
                Attack();
        }
    }

    void PlatformCheck()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + moveValue * 2.5f, rigid.position.y - 3f);
        Debug.DrawRay(frontVec, Vector2.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null && rayHit.distance < 0.5f)
            platformCheck = true;
        else
            platformCheck = false;
    }

    void Attack()
    {
        attacking = true;
        lastAttack = 0;
        anim.SetTrigger("Attack");

        Invoke("AttackExit", 1f);
    }

    void AttackExit()
    {
        attacking = false;
    }

    void Timer()
    {
        if (!attacking)
            lastAttack += Time.deltaTime;
    }

    void HealthBar()
    {
        if (currentHealth == maxHealth || currentHealth <= 0)
        {
            bar.SetActive(false);
            healthBar.enabled = false;
        }
        else
        {
            bar.SetActive(true);
            healthBar.enabled = true;
        }

        healthBar.fillAmount = currentHealth / maxHealth;
    }

    void Dead()
    {
        switch (monsterType)
        {
            case MonsterType.Goblin:
                PlayerController.instance.MonsterDieSound(0);
                break;
            case MonsterType.Skeleton:
                PlayerController.instance.MonsterDieSound(1);
                break;
        }

        PlayerStatus.instance.exp += exp;
        die = true;
        GameManager.instance.money += money;
        GameManager.instance.leftMonster--;
        gameObject.layer = 8;
        anim.SetTrigger("Die");
        Invoke("Destroy", 5f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            GameManager.instance.leftMonster--;
            Destroy();
        } 

        if (collision.gameObject.CompareTag("Player") && !PlayerController.instance.die && !PlayerController.instance.stuned && !PlayerController.instance.invincibility)
        {
            if ((PlayerController.instance.guarding && PlayerController.instance.flipCheck && gameObject.transform.position.x < PlayerController.instance.playerPosition.x) || (PlayerController.instance.guarding && !PlayerController.instance.flipCheck && gameObject.transform.position.x > PlayerController.instance.playerPosition.x))
                return;

            float random = Random.Range(0.5f, 1.5f);
            PlayerStatus.instance.currentHealth -= random * attack;
        }    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            stun = true;
            rigid.velocity = Vector2.zero;

            if (collision.transform.position.x > gameObject.transform.position.x)
                rigid.AddForce(Vector2.left * knockBackPower, ForceMode2D.Impulse);
            else
                rigid.AddForce(Vector2.right * knockBackPower, ForceMode2D.Impulse);

            float random = Random.Range(0.5f, 1.5f);
            currentHealth -= random * PlayerStatus.instance.attack * (PlayerStatus.instance.increaseAttack + PlayerStatus.instance.enforceAttack);

            Invoke("StunExit", 0.25f);
        }

        if (collision.gameObject.CompareTag("PowerAttack"))
        {
            float random = Random.Range(0.5f, 1.5f);
            currentHealth -= random * PlayerStatus.instance.powerAttack * (PlayerStatus.instance.increasePowerAttack + PlayerStatus.instance.enforcePowerAttack);
        }
    }

    void StunExit()
    {
        stun = false;
    }

    void AttackTrigger()
    {
        GameObject atk = transform.GetChild(0).gameObject;
        atk.GetComponent<CapsuleCollider2D>().enabled = true;

        Invoke("AttackTriggerExit", 0.1f);
    }

    void AttackTriggerExit()
    {
        GameObject atk = transform.GetChild(0).gameObject;
        atk.GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
