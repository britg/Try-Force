using UnityEngine;
using System.Collections;

public class ProjectileCollisionController : MonoBehaviour {

	IProjectile projectile {
		get {
			return transform.parent.gameObject.GetComponent<ProjectileController>().projectile;
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.tag != Game.playerTag) {
			var damageReceiver = collision.gameObject.GetComponent<DamageReceiverController>();
			if (damageReceiver != null) {
				damageReceiver.TakeDamageFrom(projectile);
			}
			Destroy(transform.parent.gameObject);
		}
	}
}
