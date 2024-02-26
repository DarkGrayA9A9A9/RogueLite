using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    CapsuleCollider2D coll;

    public static PlayerAttack instance;

    void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (PlayerAttack.instance == null)
            PlayerAttack.instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.flipCheck)
            coll.offset = new Vector2(-2f, coll.offset.y);
        else
            coll.offset = new Vector2(2f, coll.offset.y);
    }

    public void AttackOn()
    {
        coll.enabled = true;

        Invoke("AttackOff", 0.3f);
    }

    void AttackOff()
    {
        coll.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SoundEffect.instance.MonsterHit();
        }
    }
}
