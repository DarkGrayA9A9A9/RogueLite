using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSield : MonoBehaviour
{
    BoxCollider2D coll;

    void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.guarding)
            coll.enabled = true;
        else
            coll.enabled = false;

        if (PlayerController.instance.flipCheck)
            coll.offset = new Vector2(-1f, coll.offset.y);
        else
            coll.offset = new Vector2(1f, coll.offset.y);
    }
}
