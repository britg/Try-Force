using UnityEngine;
using System.Collections;

public class SwordController : GameController {

	public Sword sword;
	public GameObject swipe;

	void Start () {
		EndSwipe();
	}

	void Update () {
	
	}

	public void Swipe () {
		swipe.SetActive(true);
		transform.localEulerAngles = sword.arcStart;
		iTween.RotateTo(gameObject, iTween.Hash ("time", sword.swipeDuration,
		                                         "rotation", sword.arcEnd,
		                                         "islocal", true,
		                                         "oncomplete", "EndSwipe"));
	}

	void EndSwipe () {
		swipe.SetActive(false);
	}


}
