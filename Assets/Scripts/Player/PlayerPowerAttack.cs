using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPowerAttack : MonoBehaviour
{
    public float last;
    public float delay;

    CapsuleCollider2D coll;

    void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
    }

    
    void Update()
    {
        if (PlayerController.instance.powerAttacking)
        {
            if (last > delay)
                last = 0;

            if (last < delay / 2)
            {
                if (!coll.enabled)
                {
                    coll.enabled = true;
                    PlayerController.instance.PowerAttackSound();
                }
            } 
            else
                coll.enabled = false;
        }
        else
            coll.enabled = false;
    }

    void LateUpdate()
    {
        last += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            PlayerController.instance.MonsterAttackSound();
    }
}
