using UnityEngine;
using System.Collections;

public class ArrowController : ProjectileController {
	
	public Arrow arrow;
	
	void Start () {
		projectile = arrow;
	}

}