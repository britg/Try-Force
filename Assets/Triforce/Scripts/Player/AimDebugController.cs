using UnityEngine;
using System.Collections;
using Vectrosity;

public class AimDebugController : GameController {

	bool showLines = false;

	VectorLine topLine;
	VectorLine rightLine;
	VectorLine leftLine;

	VectorLine debugLineBottom;
	VectorLine debugLineRight;
	VectorLine debugLineLeft;
	
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

		if (showLines) {
			DrawSectors();
		}
	}
	
	void Update () {
		if (showLines) {
			UpdateLines();
		}
	}

	public void ToggleDebug () {
		showLines = !showLines;

		if (showLines) {
			DrawSectors();
		} else {
			RemoveAll();
		}
	}

	void RemoveAll () {
		VectorLine.Destroy(ref debugLineRight);
		VectorLine.Destroy(ref debugLineLeft);
		VectorLine.Destroy(ref debugLineBottom);
		VectorLine.Destroy(ref topLine);
		VectorLine.Destroy(ref rightLine);
		VectorLine.Destroy(ref leftLine);
	}

	void DrawSectors () {
		Vector2 screenTop = new Vector2(Screen.width/2, Screen.height);
		Vector2 screenTopDiff = screenTop - currentInputController.aimFrom;
		Vector2 scaleFactor = new Vector2(10, 10);
		Vector2 topRightPoint = Vector2.Scale(screenTopDiff.Rotate(60f), scaleFactor) + currentInputController.aimFrom;
		Vector2 topLeftPoint = Vector2.Scale(screenTopDiff.Rotate(-60f), scaleFactor) + currentInputController.aimFrom;
		Vector2 bottomPoint = Vector2.Scale(screenTopDiff.Rotate(180f), scaleFactor) + currentInputController.aimFrom;

		debugLineRight = VectorLine.SetLine(Color.red, currentInputController.aimFrom, topRightPoint);
		debugLineLeft = VectorLine.SetLine(Color.red, currentInputController.aimFrom, topLeftPoint);
		debugLineBottom = VectorLine.SetLine(Color.red, currentInputController.aimFrom, bottomPoint);
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
