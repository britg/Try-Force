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
		Invoke("Restart", 0.5f);
	}

	public void Restart () {
		Application.LoadLevel(0);
	}
}
