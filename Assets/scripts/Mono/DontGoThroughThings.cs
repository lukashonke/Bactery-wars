using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;

namespace Assets.scripts.Mono
{
	public class DontGoThroughThings : MonoBehaviour
	{
		// Careful when setting this to true - it might cause double
		// events to be fired - but it won't pass through the trigger
		public bool sendTriggerMessage = false;

		public LayerMask layerMask = 1 << Utils.OBSTACLES_LAYER; //make sure we aren't in this layer 
		public float skinWidth = 0.1f; //probably doesn't need to be changed 

		private float minimumExtent;
		private float partialExtent;
		private float sqrMinimumExtent;
		private Vector2 previousPosition;
		private Rigidbody2D myRigidbody;
		private Collider2D myCollider;

		//initialize values 
		void Start()
		{
			myRigidbody = GetComponent<Rigidbody2D>();
			myCollider = GetComponent<Collider2D>();
			previousPosition = myRigidbody.position;
			minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
			partialExtent = minimumExtent * (1.0f - skinWidth);
			sqrMinimumExtent = minimumExtent * minimumExtent;
		}

		void FixedUpdate()
		{
			//have we moved more than our minimum extent? 
			Vector2 movementThisStep = myRigidbody.position - previousPosition;
			float movementSqrMagnitude = movementThisStep.sqrMagnitude;

			if (movementSqrMagnitude > sqrMinimumExtent)
			{
				float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
				RaycastHit2D hitInfo = Physics2D.Raycast(previousPosition, movementThisStep, movementMagnitude, layerMask.value);

				//check for obstructions we might have missed 
				if (hitInfo)
				{
					if (!hitInfo.collider)
						return;

					if (hitInfo.collider.isTrigger)
						hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

					if (!hitInfo.collider.isTrigger)
					{
						Vector3 pos = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
						pos.z = 0;

						Tile t = WorldHolder.instance.activeMap.GetClosestGroundTile(pos);

						Debug.Log(t.tileType);

						myRigidbody.position = WorldHolder.instance.activeMap.GetTileWorldPosition(t);
					}

				}
			}

			previousPosition = myRigidbody.position;
		}
	}
}
