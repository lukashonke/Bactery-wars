// Copyright (c) 2015, Lukas Honke
// ========================
using UnityEngine;

namespace Assets.scripts.Mono
{
    public class MoveToMouse : MonoBehaviour
    {
        Rigidbody2D rb;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - transform.position;
            dir.Normalize();
            rb.AddForce(dir * 100, ForceMode2D.Force);
        }
    }
}