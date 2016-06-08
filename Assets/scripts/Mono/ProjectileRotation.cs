// Copyright (c) 2015, Lukas Honke
// ========================
using UnityEngine;

namespace Assets.scripts.Mono
{
    public class ProjectileRotation : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0, 0, 1), 20);
        }
    }

}