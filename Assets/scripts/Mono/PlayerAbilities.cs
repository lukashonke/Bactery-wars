using UnityEngine;

namespace Assets.scripts.Mono
{
    public class PlayerAbilities : MonoBehaviour
    {
        public GameObject shootingPos;
        public GameObject ability1Prefab;

        void Start()
        {
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Ability1();
            }
        }

        private void Ability1()
        {
            GameObject projectile = Instantiate(ability1Prefab, shootingPos.transform.position, Quaternion.identity) as GameObject;
        }
    }
}