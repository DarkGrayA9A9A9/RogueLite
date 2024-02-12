using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool portalOn;
    public Vector3 targetPosition;

    public static Portal instance;

    void Awake()
    {
        if (Portal.instance == null)
            Portal.instance = this;
    }

    void Update()
    {
        if (portalOn && Input.GetButtonDown("Up") && GameManager.instance.leftMonster < 1)
            PlayerController.instance.Fade(targetPosition);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            portalOn = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            portalOn = false;
    }
}
