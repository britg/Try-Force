using UnityEngine;
using System.Collections;

public class DeathGUIController : MonoBehaviour {

	public GameObject deathInterface;

	// Use this for initialization
	void Start () {
		NotificationCenter.AddObserver(this, Notifications.Dead);
		deathInterface.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDead () {
		deathInterface.SetActive(true);
	}

	public void Restart () {
		Application.LoadLevel(0);
	}
}
