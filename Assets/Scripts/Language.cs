using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Language : MonoBehaviour
{
    public bool onSign;

    public Text language;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Up") && onSign)
        {
            if (GameManager.instance.language == 0)
                GameManager.instance.language = 1;
            else
                GameManager.instance.language = 0;
        }

        if (!onSign)
            language.text = "";
        else
        {
            if (GameManager.instance.language == 0)
                language.text = "ÇÑ±¹¾î";
            else
                language.text = "English";
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            onSign = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            onSign = false;
    }
}
