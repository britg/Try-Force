using UnityEngine;
using System.Collections;

public class DeathController : MonoBehaviour {

	public void Die () {
		Debug.Log ("Dying!");
		Destroy (gameObject);
	}
}
