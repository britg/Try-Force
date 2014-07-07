using UnityEngine;
using System.Collections;

public class PlayerController : GameController {

	public Player player;

	public GameObject body;

	void Update () {
		Move(currentInputController.resultantInputVector);
		DetectRotate();
	}

	void Move (Vector3 direction) {
		transform.position = transform.position + direction * player.moveSpeed;
	}

	void DetectRotate () {
		if (currentInputController.rotateRight) {
			RotateRight();
		}
		if (currentInputController.rotateLeft) {
			RotateLeft();
		}
	}

	void RotateRight () {
		player.RotateCounterClockwise();
		UpdateOrientation();
	}

	void RotateLeft () {
		player.RotateClockwise();
		UpdateOrientation();
	}

	void UpdateOrientation () {
		if (player.warriorFace) {
			iTween.RotateTo(body, iTween.Hash ("rotation", Vector3.zero, "time", player.rotateSpeed));
		}

		if (player.mageFace) {
			iTween.RotateTo(body, iTween.Hash ("rotation", new Vector3(0f, 0f, -120f), "time", player.rotateSpeed));
		}

		if (player.thiefFace) {
			iTween.RotateTo(body, iTween.Hash ("rotation", new Vector3(0f, 0f, 120f), "time", player.rotateSpeed));
		}
	}

	public void Attack () {

	}

}
