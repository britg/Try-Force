using UnityEngine;
using System.Collections;

public class DamageReceiverController : GameController {

	public void TakeDamageFrom (IProjectile projectile) {
		Debug.Log ("Taking damage from " + projectile);
	}

	public void TakeDamageFrom (IWeapon weapon) {
		Debug.Log ("Taking damage from " + weapon);
	}

}
