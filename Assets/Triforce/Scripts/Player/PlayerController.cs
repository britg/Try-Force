using UnityEngine;
using System.Collections;

public class PlayerController : GameController {

	public Player player;
	public CircleCollider2D collisionBody;

	void Update () {
		Move(currentInputController.resultantInputVector);
	}

	void Move (Vector3 direction) {
		// raycast in that direction to see if you are going to hit anything
		Vector2 direction2D = direction.XY();
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collisionBody.radius, direction2D, player.moveSpeed);
		bool stop = false;
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null && hit.collider.gameObject.tag == Game.wallTag) {
				direction2D += hit.normal;
			} else {
			}
		}

		if (!stop) {
			transform.position = transform.position + direction2D.to3() * player.moveSpeed;
		}

//		Debug.DrawRay(transform.position, direction * (player.moveSpeed + 0.62f), Color.green);
	}


}
