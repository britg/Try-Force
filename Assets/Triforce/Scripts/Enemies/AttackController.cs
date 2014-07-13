using UnityEngine;
using System.Collections;

public class AttackController : GameController {

	public GameObject weaponObj;
	public SwordController swordController;

	bool midAttack = false;

	public void Attack () {
		if (midAttack) {
			return;
		}
		TurnWeaponTowardsPlayer();
		swordController.Swipe();
		midAttack = true;
	}

	void TurnWeaponTowardsPlayer () {
		weaponObj.transform.LookAt2D(playerObj.transform.position);
	}

}
