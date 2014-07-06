using UnityEngine;
using System.Collections;
using Vectrosity;

public class AimController : GameController {

	VectorLine topLine;
	VectorLine rightLine;
	VectorLine leftLine;

	Vector2 rightTo {
		get {
			return currentInputController.aimDiff.Rotate(120f) + currentInputController.aimFrom;
		}
	}

	Vector2 leftTo {
		get {
			return currentInputController.aimDiff.Rotate(-120f) + currentInputController.aimFrom;
		}
	}

	void Update () {
		UpdateLines();
	}

	void UpdateLines () {
		if (topLine == null) {
			CreateLines();
		}
		UpdateTopLine();
		UpdateRightLine();
		UpdateLeftLine();
	}

	void CreateLines () {
		topLine = VectorLine.SetLine(Color.green, currentInputController.aimFrom, currentInputController.aimTo);
		rightLine = VectorLine.SetLine(Color.green, currentInputController.aimFrom, rightTo);
		leftLine = VectorLine.SetLine(Color.green, currentInputController.aimFrom, leftTo);
	}

	void UpdateTopLine () {
		topLine.points2[1] = currentInputController.aimTo;
		topLine.Draw();
	}

	void UpdateRightLine () {
		rightLine.points2[1] = rightTo;
		rightLine.Draw();
	}

	void UpdateLeftLine () {
		leftLine.points2[1] = leftTo;
		leftLine.Draw();
	}

}
