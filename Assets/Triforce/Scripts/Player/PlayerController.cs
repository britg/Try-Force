using UnityEngine;
using System.Collections;

public class PlayerController : GameController {

	public Player player;
	public CircleCollider2D collisionBody;

	void Update () {
		Move(currentInputController.resultantInputVector);
	}

	void Move (Vector3 direction) {
		Vector2 direction2D = direction.XY();
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collisionBody.radius, direction2D, player.moveSpeed);
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null && !hit.collider.isTrigger) {
				direction2D += hit.normal;
			}
		}

		transform.position = transform.position + direction2D.to3() * player.moveSpeed;
	}


}
