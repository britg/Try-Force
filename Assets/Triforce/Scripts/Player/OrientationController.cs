using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

public class OrientationController : GameController {

	Player.Orientation previousOrientation;

	void Start () {
		FsmVariables.GlobalVariables.GetFsmString("Orientation").Value = player.orientation.ToString();
	}

	void Update () {
	}


	void UpdateOrientation () {
		FsmVariables.GlobalVariables.GetFsmString("Orientation").Value = player.orientation.ToString();

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
		if (aimController == null) {
			return;
		}
		Vector2 aimPoint = aimController.aimPointForSector(Player.Sector.Top);
		playerController.transform.LookAt2D(aimPoint);
	}
}
