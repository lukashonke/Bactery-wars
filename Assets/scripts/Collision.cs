using UnityEngine;
using System.Collections;
using Assets.scripts.Mono;

public class Collision : MonoBehaviour {

    public PlayerData data;

	// Use this for initialization
	void Start ()
    {
        data = GetComponentInParent<PlayerData>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        
        Debug.Log("TEST BUM");
        data.player.Status.ReceiveDamage(100);
    }
}
