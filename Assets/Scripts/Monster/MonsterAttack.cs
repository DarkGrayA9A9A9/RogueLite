using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public enum MonsterType { Goblin, Mushroom, Skeleton }
    public MonsterType monsterType;

    public float attack;

    CapsuleCollider2D coll;
    SpriteRenderer spriter;

    void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        spriter = GetComponentInParent<SpriteRenderer>();
    }

    void Start()
    {
        switch (monsterType)
        {
            case MonsterType.Goblin:
                attack = 7 + (7 * (float)GameManager.instance.floor / 5);
                break;
            case MonsterType.Mushroom:
                attack = 10 + (10 * (float)GameManager.instance.floor / 5);
                break;
            case MonsterType.Skeleton:
                attack = 15 + (15 * (float)GameManager.instance.floor / 5);
                break;
        }
    }

    void Update()
    {
        switch (monsterType)
        {
            case MonsterType.Goblin:
                if (spriter.flipX)
                    transform.rotation = Quaternion.Euler(0, 0, 25f);
                else
                    transform.rotation = Quaternion.Euler(0, 0, -25f);
                break;
            case MonsterType.Mushroom:
                if (spriter.flipX)
                    coll.offset = new Vector2(-0.62f, -0.39f);
                else
                    coll.offset = new Vector2(0.62f, -0.39f);
                break;
            case MonsterType.Skeleton:
                if (spriter.flipX)
                    coll.offset = new Vector2(-2.2f, 0.35f);
                else
                    coll.offset = new Vector2(2.2f, 0.35f);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.instance.die && !PlayerController.instance.stuned && !PlayerController.instance.invincibility)
        {
            if ((PlayerController.instance.guarding && PlayerController.instance.flipCheck && gameObject.transform.position.x < PlayerController.instance.playerPosition.x) || (PlayerController.instance.guarding && !PlayerController.instance.flipCheck && gameObject.transform.position.x > PlayerController.instance.playerPosition.x))
            {
                SoundEffect.instance.Guard();
                return;
            }

            SoundEffect.instance.Hit();
            float random = Random.Range(0.5f, 1.5f);
            PlayerStatus.instance.currentHealth -= random * attack;
        }
    }
}