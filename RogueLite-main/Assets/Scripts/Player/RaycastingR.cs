using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingR : MonoBehaviour
{
    public float distance;
    public bool raycasting;

    public static RaycastingR instance;

    void Awake()
    {
        if (RaycastingR.instance == null)
            RaycastingR.instance = this;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector2.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null && rayHit.distance < this.distance)
            raycasting = true;
        else
            raycasting = false;
    }
}
