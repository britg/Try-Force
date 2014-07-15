using UnityEngine;
using System.Collections;

public class ThiefWeaponController : GameController {

	public GameObject arrowPrefab;
	
	Vector2 currentTarget {
		get {
			return aimController.aimPointForOrientation(Player.Orientation.Thief);
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
		FireBow();
	}

	void FireBow () {
		GameObject arrowObj = (GameObject)Instantiate(arrowPrefab);
		arrowObj.transform.position = transform.position;
    arrowObj.transform.rotation = transform.rotation;
	}
}
