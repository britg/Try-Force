﻿using UnityEngine;
using System.Collections;

public class OrientationController : GameController {

	Player.Orientation previousOrientation;

	void Update () {
		DetectRotate();
		DetectPivot();
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
		previousOrientation = player.orientation;
		player.RotateCounterClockwise();
		UpdateOrientation();
	}

	void RotateLeft () {
		previousOrientation = player.orientation;
		player.RotateClockwise();
		UpdateOrientation();
	}

	void UpdateOrientation () {
		Debug.Log ("previous orientation was " + previousOrientation);
		if (player.warriorFace) {
			iTween.RotateTo(gameObject, iTween.Hash ("rotation", new Vector3(0f, 0f, playerController.transform.eulerAngles.z), "time", player.rotateSpeed));
		}

		if (player.mageFace) {
			iTween.RotateTo(gameObject, iTween.Hash ("rotation", new Vector3(0f, 0f, playerController.transform.eulerAngles.z - 120f), "time", player.rotateSpeed));
		}

		if (player.thiefFace) {
			iTween.RotateTo(gameObject, iTween.Hash ("rotation", new Vector3(0f, 0f, playerController.transform.eulerAngles.z + 120f), "time", player.rotateSpeed));
		}
	}

	void DetectPivot () {
		Vector2 aimPoint = aimController.aimPointForSector(Player.Sector.Top);
		playerController.transform.LookAt2D(aimPoint);
	}
}