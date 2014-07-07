using UnityEngine;
using System.Collections;

public class MouseInputController : GameController, IInputController {

	public Camera uiCamera;

	// Moving

	public bool forward { get { return Input.GetKey(KeyCode.W); } }
	public bool right { get { return Input.GetKey(KeyCode.D); } }
	public bool back { get { return Input.GetKey(KeyCode.S); } }
	public bool left { get { return Input.GetKey(KeyCode.A); } }
	public bool rotateRight { get { return Input.GetKeyDown(KeyCode.E); } }
	public bool rotateLeft { get { return Input.GetKeyDown(KeyCode.Q); } }
	public Vector3 resultantInputVector { get { return InputVector(); } }

	Vector3 InputVector () {
		Vector3 resultant = Vector3.zero;

		if (forward) {
			resultant += Vector3.up;
		}

		if (right) {
			resultant += Vector3.right;
		}

		if (back) {
			resultant += Vector3.down;
		}

		if (left) {
			resultant += Vector3.left;
		}

		return resultant.normalized;
	}

	// Aiming

	Vector2 _aimFrom = Vector2.zero;
	public Vector2 aimFrom {
		get {
			if (_aimFrom == Vector2.zero) {
				_aimFrom = Camera.main.WorldToScreenPoint(playerObj.transform.position);
			}
			return _aimFrom;
		}
	}

	public Vector2 aimTo {
		get {
			return Input.mousePosition.XY();
		}
	}

	public Vector2 aimDiff {
		get {
			return aimTo-aimFrom;
		}
	}

	// Attack
	bool uiHit { get { return CheckUIHit(); } }
	public bool attack { get { return Input.GetMouseButtonDown(0) && !uiHit; } }


	bool CheckUIHit () {
		Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		bool didHit = Physics.Raycast(ray, out hit);

		return didHit;
	}

}
