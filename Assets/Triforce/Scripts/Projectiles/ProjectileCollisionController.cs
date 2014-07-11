using UnityEngine;
using System.Collections;

public class ProjectileCollisionController : MonoBehaviour {

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.tag != Game.playerTag) {
			Destroy(transform.parent.gameObject);
		}
	}
}
