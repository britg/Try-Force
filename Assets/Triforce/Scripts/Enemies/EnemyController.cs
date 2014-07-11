using UnityEngine;
using System.Collections;

public class EnemyController : GameController {

	public Enemy enemy;
	public CircleCollider2D collisionBody;
	public CircleCollider2D detectionRadius;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DetectPlayer();
		MoveTowardsPlayer();
		Attack();
	}

	void DetectPlayer () {
		if (!enemy.idle) {
			return;
		}

		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, playerDirection, detectionRadius.radius);
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null && hit.collider.gameObject.tag == Game.playerTag) {
				Debug.Log ("Hit palyer");
				EnterAggroState();
			}
		}
	}

	void EnterAggroState () {
		enemy.EnterAggroState();
	}

	void MoveTowardsPlayer () {
		if (!enemy.aggro) {
			return;
		}
		Vector2 direction2D = playerDirection;
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collisionBody.radius, direction2D, enemy.moveSpeed);
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null) {
				direction2D += hit.normal;
			}
		}
		
		transform.position = transform.position + direction2D.to3() * enemy.moveSpeed;
	}

	void Attack () {

	}
}
