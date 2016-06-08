// Copyright (c) 2015, Lukas Honke
// ========================
using UnityEngine;

namespace Assets.scripts.Mono
{
    public class FadeOut : MonoBehaviour
    {
        private float size;
        private float rate;
        // Use this for initialization
        void Start()
        {
            size = 1.0f;
            rate = Random.Range(0.9f, 0.99f);
        }

        // Update is called once per frame
        void Update()
        {
            size *= rate;

            if (size <= 0)
                Destroy(gameObject);
            else
                transform.localScale *= size;
        }
    }

}
