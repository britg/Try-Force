using UnityEngine;
using System.Collections;
using Vectrosity;

public class AimController : GameController {

	public Vector2 aimPointForSector (Player.Sector sector) {

		switch (sector) {
		case Player.Sector.Top:
			return aimPointForTop();
		case Player.Sector.Right:
			return aimPointForRight();
		case Player.Sector.Left:
			return aimPointForLeft();
		}

		return aimPointForTop();

	}

	Vector2 aimPointForTop () {
		return Camera.main.ScreenToWorldPoint(currentInputController.aimTo).XY();
	}

	Vector2 aimPointForRight () {
		Vector2 right = currentInputController.aimDiff.Rotate(120f) + currentInputController.aimFrom;
		return Camera.main.ScreenToWorldPoint(right).XY();
	}

	Vector2 aimPointForLeft () {
		Vector2 left = currentInputController.aimDiff.Rotate(-120f) + currentInputController.aimFrom;
		return Camera.main.ScreenToWorldPoint(left).XY();
	}

}
