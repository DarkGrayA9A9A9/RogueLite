using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public enum BossType { Goblin, Mushroom, Skeleton }
    public BossType bossType;

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
        switch (bossType)
        {
            case BossType.Goblin:
                attack = 10 + (10 * (float)GameManager.instance.floor / 5);
                break;
            case BossType.Mushroom:
                attack = 15 + (15 * (float)GameManager.instance.floor / 5);
                break;
            case BossType.Skeleton:
                attack = 20 + (20 * (float)GameManager.instance.floor / 5);
                break;
        }
    }

    void Update()
    {
        switch (bossType)
        {
            case BossType.Goblin:
                if (spriter.flipX)
                    transform.rotation = Quaternion.Euler(0, 0, 25f);
                else
                    transform.rotation = Quaternion.Euler(0, 0, -25f);
                break;
            case BossType.Mushroom:
                if (spriter.flipX)
                    coll.offset = new Vector2(-0.62f, -0.67f);
                else
                    coll.offset = new Vector2(0.62f, -0.67f);
                break;
            case BossType.Skeleton:
                if (spriter.flipX)
                    coll.offset = new Vector2(-2.2f, 0.16f);
                else
                    coll.offset = new Vector2(2.2f, 0.16f);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.instance.die)
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
