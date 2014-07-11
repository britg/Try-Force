using UnityEngine;
using System.Collections;

public class MageWeaponController : GameController {

	public GameObject fireballPrefab;

	Vector2 currentTarget {
		get {
			return aimController.aimPointForOrientation(Player.Orientation.Mage);
		}
	}

	void Update () {
		DetectInput();
	}
	
	void DetectInput () {
		if (currentInputController.attack) {
			Attack();
		}
	}
	
	void Attack () {
		ShootFireball();
	}

	void ShootFireball () {
		GameObject fireballObj = (GameObject)Instantiate(fireballPrefab);
		fireballObj.transform.position = transform.position;
		FireballController fireballController = fireballObj.GetComponent<FireballController>();
		fireballController.SetDestination(currentTarget);
	}
}
