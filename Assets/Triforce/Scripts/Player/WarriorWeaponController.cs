using UnityEngine;
using System.Collections;

public class WarriorWeaponController : GameController {

	public GameObject sword;

	void Update () {
		DetectInput();
	}

	void DetectInput () {
		if (currentInputController.attack) {
			Attack();
		}
	}

	void Attack () {
		SwipeSword();
	}

	void SwipeSword () {
		SwordController swordController = sword.GetComponent<SwordController>();
		swordController.Swipe();
	}
}
