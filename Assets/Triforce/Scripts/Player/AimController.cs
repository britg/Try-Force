using UnityEngine;
using System.Collections;
using Vectrosity;

public class AimController : GameController {

	public Vector2 aimPointForSector (Player.Sector sector) {

		switch (sector) {
		case Player.Sector.Top:
			return aimPointForLook();
		case Player.Sector.Right:
			return aimPointForRight();
		case Player.Sector.Left:
			return aimPointForLeft();
		}

		return aimPointForLook();

	}

	public Vector2 aimPointForOrientation (Player.Orientation orientation) {
		switch (orientation) {

		case Player.Orientation.Warrior:
			if (player.orientation == Player.Orientation.Warrior) {
				return aimPointForLook();
			} else if (player.orientation == Player.Orientation.Thief) {
				return aimPointForRight();
			} else if (player.orientation == Player.Orientation.Mage) {
				return aimPointForLeft();
			}
			break;

		case Player.Orientation.Thief:
			if (player.orientation == Player.Orientation.Warrior) {
				return aimPointForRight();
			} else if (player.orientation == Player.Orientation.Thief) {
				return aimPointForLook();
			} else if (player.orientation == Player.Orientation.Mage) {
				return aimPointForLeft();
			}
			break;

		case Player.Orientation.Mage:
			if (player.orientation == Player.Orientation.Warrior) {
				return aimPointForLeft();
			} else if (player.orientation == Player.Orientation.Thief) {
				return aimPointForRight();
			} else if (player.orientation == Player.Orientation.Mage) {
				return aimPointForLook();
			}
			break;

		default:
			return aimPointForLook();

		}
		return aimPointForLook();
	}

	Vector2 aimPointForLook () {
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
