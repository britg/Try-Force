using UnityEngine;
using System.Collections;

public class WarriorWeaponController : GameController {

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

	}
}
