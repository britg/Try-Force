using UnityEngine;
using System.Collections;

public class WeaponCollisionController : MonoBehaviour {

	IWeapon weapon {
		get {
			return transform.parent.gameObject.GetComponent<WeaponController>().weapon;
		}
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.tag != Game.playerTag) {
			var damageReceiver = collision.gameObject.GetComponent<BodyController>();
			if (damageReceiver != null) {
				damageReceiver.TakeDamageFrom(weapon);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag != Game.playerTag) {
			var damageReceiver = collider.gameObject.GetComponent<BodyController>();
			if (damageReceiver != null) {
				damageReceiver.TakeDamageFrom(weapon);
			}
		}
	}
}
