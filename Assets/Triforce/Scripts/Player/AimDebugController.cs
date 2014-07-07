using UnityEngine;
using System.Collections;
using Vectrosity;

public class AimDebugController : GameController {

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

	void Start () {
		DrawSectors();
	}
	
	void Update () {
		UpdateLines();
	}

	void DrawSectors () {
		Vector2 screenTop = new Vector2(Screen.width/2, Screen.height);
		Vector2 screenTopDiff = screenTop - currentInputController.aimFrom;
		Vector2 scaleFactor = new Vector2(10, 10);
		Vector2 topRightPoint = Vector2.Scale(screenTopDiff.Rotate(60f), scaleFactor) + currentInputController.aimFrom;
		Vector2 topLeftPoint = Vector2.Scale(screenTopDiff.Rotate(-60f), scaleFactor) + currentInputController.aimFrom;
		Vector2 bottomPoint = Vector2.Scale(screenTopDiff.Rotate(180f), scaleFactor) + currentInputController.aimFrom;

		VectorLine.SetLine(Color.red, currentInputController.aimFrom, topRightPoint);
		VectorLine.SetLine(Color.red, currentInputController.aimFrom, topLeftPoint);
		VectorLine.SetLine(Color.red, currentInputController.aimFrom, bottomPoint);
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
