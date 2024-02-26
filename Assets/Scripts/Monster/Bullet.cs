using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BossType { Goblin, Mushroom, Skeleton }
    public BossType bossType;

    float random;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Invoke("Delete", 15);
    }

    void Delete()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.instance.die)
        {
            if ((PlayerController.instance.guarding && PlayerController.instance.flipCheck && gameObject.transform.position.x < PlayerController.instance.playerPosition.x) || (PlayerController.instance.guarding && !PlayerController.instance.flipCheck && gameObject.transform.position.x > PlayerController.instance.playerPosition.x))
            {
                SoundEffect.instance.Guard();
                gameObject.SetActive(false);
                return;
            }

            SoundEffect.instance.Hit();

            switch (bossType)
            {
                case BossType.Goblin:
                    random = Random.Range(5f, 15f);
                    PlayerStatus.instance.currentHealth -= random;
                    break;
                case BossType.Mushroom:
                    random = Random.Range(8f, 20f);
                    PlayerStatus.instance.currentHealth -= random;
                    break;
                case BossType.Skeleton:
                    random = Random.Range(12f, 25f);
                    PlayerStatus.instance.currentHealth -= random;
                    break;
            }

            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Seild"))
            gameObject.SetActive(false);
    }
}
