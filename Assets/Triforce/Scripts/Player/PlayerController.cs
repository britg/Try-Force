using UnityEngine;
using System.Collections;

public class PlayerController : GameController {

	public Player player;
	public CircleCollider2D collisionBody;

	void Start () {
	}

	void Update () {
		Move(currentInputController.resultantInputVector);
	}

	void Move (Vector3 direction) {
		Vector2 direction2D = direction.XY() * Time.timeScale;
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collisionBody.radius, direction2D, player.moveSpeed);
		foreach (RaycastHit2D hit in hits) {
			if (shouldPushBack(hit.collider)) {
				direction2D += hit.normal;
			}
		}

		transform.position = transform.position + direction2D.to3() * player.moveSpeed;
	}

	bool shouldPushBack (Collider2D collider) {
		if (collider == null) {
			return false;
		}

		if (collider.isTrigger) {
			return false;
		}

		if (collider.tag == Game.projectileTag || collider.tag == Game.spellTag) {
			return false;
		}

		return true;
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == Game.enemyWeaponTag) {
			Die();
		}
	}

	void Die () {
		NotificationCenter.PostNotification(this, Notifications.Dead);
	}

}
