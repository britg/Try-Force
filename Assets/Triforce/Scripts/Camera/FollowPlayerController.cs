using UnityEngine;
using System.Collections;

public class FollowPlayerController : MonoBehaviour {

	public Transform playerTransform;

	// Use this for initialization
	void Start () {
    Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.x = playerTransform.position.x;
		pos.y = playerTransform.position.y;
		transform.position = pos;
	}
}
