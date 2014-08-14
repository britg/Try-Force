using UnityEngine;
using System.Collections;

public class PlayerNotificationController : GameController {

  HUDText hudText;

	// Use this for initialization
	void Start () {
    hudText = GetComponent<HUDText>();
    FollowPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	  
	}

  public void ChangeHitpoints (float change) {
    hudText.Add(change.ToString("F0"), Color.red, 0f);
  }

  void FollowPlayer () {
    UIFollowTarget followTarget = gameObject.AddComponent<UIFollowTarget>();
    followTarget.target = playerObj.transform;
  }

}
