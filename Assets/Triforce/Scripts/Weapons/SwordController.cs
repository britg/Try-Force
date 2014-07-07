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
		StartSwipe();
		Invoke ("EndSwipe", sword.swipeDuration);
	}

	void StartSwipe () {
		swipe.SetActive(true);
	}

	void EndSwipe () {
		swipe.SetActive(false);
	}


}
