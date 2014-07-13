using UnityEngine;
using System.Collections;

public class EnemyController : DamageReceiverController {

	public Enemy enemy;
	public override IDamageReceiver damageReceiver { get { return enemy; } }

	public CircleCollider2D collisionBody;
	public CircleCollider2D detectionRadius;
	public CircleCollider2D weaponRadius;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ReactToAggroState();
	}

	void ReactToAggroState () {

		if (enemy.idle) {
			DetectPlayer();
		}

		if (enemy.aggro) {
			DetectPlayer();
			MoveTowardsPlayer();
		}

		if (enemy.inRange) {
			Attack();
		}

		if (enemy.knockedBack) {
			KnockBack();
		}

		if (enemy.stunned) {
			Stunned();
		}

		if (enemy.dead) {
			Die();
		}

	}

	void DetectPlayerAll () {
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, playerDirection, detectionRadius.radius);
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null && hit.collider.gameObject.tag == Game.wallTag) {
				enemy.EnterIdleState();
				return;
			}

			if (hit.collider != null && hit.collider.gameObject.tag == Game.playerTag) {
				enemy.EnterAggroState();
				if ((hit.fraction*detectionRadius.radius) <= weaponRadius.radius) {
					enemy.EnterInRangeState();
					return;
				}
				return;
			}
		}
	}

	void DetectPlayer () {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, detectionRadius.radius, LayerMask.NameToLayer(Game.enemyLayer));

		if (hit.collider == null) {
			return;
		}

		if (hit.collider.gameObject.tag == Game.wallTag) {
			enemy.EnterIdleState();
			return;
		}

		if (hit.collider.gameObject.tag == Game.playerTag) {
			Debug.Log ("Player");
			enemy.EnterAggroState();
			if ((hit.fraction*detectionRadius.radius) <= weaponRadius.radius) {
				enemy.EnterInRangeState();
				return;
			}
			return;
		}
	}

	void MoveTowardsPlayer () {
		Vector2 direction2D = playerDirection;
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collisionBody.radius, direction2D, enemy.moveSpeed, LayerMask.NameToLayer(Game.enemyLayer));
		foreach (RaycastHit2D hit in hits) {
			if (hit.collider != null) {
				direction2D += hit.normal;
			}
		}
		
		transform.position = transform.position + direction2D.to3() * enemy.moveSpeed;
	}

	AttackController _attackController;
	AttackController attackController { 
		get {
			if (_attackController == null) {
				_attackController = GetComponent<AttackController>(); 
			} 
			return _attackController;
		}
	}

	void Attack () {
		attackController.Attack();
	}


	void KnockBack () {

	}

	void Stunned () {

	}

	DeathController _deathController;
	DeathController deathController { 
		get {
			if (_deathController == null) {
				_deathController = GetComponent<DeathController>(); 
			} 
			return _deathController;
		}
	}

	void Die () {
		deathController.Die();
	}
}
