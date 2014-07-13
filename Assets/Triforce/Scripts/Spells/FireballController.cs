using UnityEngine;
using System.Collections;

public class FireballController : ProjectileController {

	public Fireball fireball;

	void Start () {
		projectile = fireball;
	}
	
}
