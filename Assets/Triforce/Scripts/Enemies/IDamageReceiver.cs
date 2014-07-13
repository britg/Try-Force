using UnityEngine;
using System.Collections;

public interface IDamageReceiver {

	int hitPoints { get; }

	void TakeDamageFrom (IProjectile projectile);
	void TakeDamageFrom (IWeapon weapon);
	void TakeDamageFrom (ISpell spell);

}
