using UnityEngine;
using System.Collections;

public class BodyController : GameController {

	public IDamageReceiver damageReceiver {
		get {
			return transform.parent.gameObject.GetComponent<DamageReceiverController>().damageReceiver;
		}
	}

	public void TakeDamageFrom (IProjectile projectile) {
		Debug.Log ("Taking damage from projectile " + projectile);
		damageReceiver.TakeDamageFrom(projectile);
	}

	public void TakeDamageFrom (IWeapon weapon) {
		Debug.Log ("Taking damage from weapon " + weapon);
		damageReceiver.TakeDamageFrom(weapon);
	}

	public void TakeDamageFrom (ISpell spell) {
		Debug.Log ("Taking damage from spell " + spell);
		damageReceiver.TakeDamageFrom(spell);
	}

}
