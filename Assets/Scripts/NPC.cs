using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool onNPC;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetButtonDown("Up") && onNPC && !GameManager.instance.paused)
            GameManager.instance.EnforceSystemOn();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            onNPC = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            onNPC = false;
    }
}
