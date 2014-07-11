using UnityEngine;
using System.Collections;

public class ProjectileCollisionController : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag != "Player") {
			Destroy(transform.parent.gameObject);
		}
	}
}
